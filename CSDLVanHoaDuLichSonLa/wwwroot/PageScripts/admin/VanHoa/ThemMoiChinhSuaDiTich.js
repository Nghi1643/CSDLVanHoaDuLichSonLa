const baseUrl = getRootLink();
const guidEmpty = '00000000-0000-0000-0000-000000000000';
const allowedExtensions = {
    1: ".jpg,.jpeg,.png,.gif,.webp,.bmp",                // Ảnh
    2: ".mp4,.webm,.ogg,.mov,.avi",                      // Video
    3: ".mp3,.wav,.m4a",                                 // Audio
    4: ".doc,.docx,.pdf,.xlsx,.xls,.dot"                      // File
};
$(document).ready(function () {
    initSelect2();
    initDatePicker();
    let translations = [];
    let translationsDPT = [];
    let translationsTT = [];
    let banDichTT = [];
    let objTLTT = [];
    ; (async function () {
        await Promise.all([
            initNgonNgu(),
            initCapXepHang(),
            initLoaiHinhDiTich(),
            initDonViQuanLy(),
            initCoQuanBanHanhVanBan()
        ]);
        initThongTinChung();
        initTableDaPhuongTien();
        initTableTrungTu();
        initTableVanBanTaiLieu();
        initTableTLTT()

        reloadAccess()
    })();

    $("#formDelete").on("submit", function (e) {
        e.preventDefault();
        let type = $('#idTypeDelete').val();
        if (type == "1") {
            dataDeleteDaPhuongTien();
        } else if (type == "2") {
            dataDeleteTrungTu();
        } else if (type == "3") {
            dataDeleteVanBanTaiLieu();
        } else if (type == "4") {
            dataDeleteTLTT();
        }
    });

    //#region THÔNG TIN CHUNG

    document.getElementById('anhDaiDien').addEventListener('change', function (event) {
        const file = event.target.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = function (e) {
                document.getElementById('previewAnhDaiDien').setAttribute('src', e.target.result);
            };
            reader.readAsDataURL(file);
        }
    });

    async function initThongTinChung() {
        if (checkEmptyBlank(diTichId)) {
            return;
        }

        try {
            const res = await fetch(`/api/DiTichApi/ChiTiet/${diTichId}`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                }
            })
            if (!res.ok) {
                var errText = await res.text();
                throw new Error(errText);
            }
            data = await res.json();
            if (data && data.isSuccess && data.value) {
                let v = data.value;
                // Ảnh đại diện
                if (v.anhDaiDien) {
                    $("#previewAnhDaiDien").attr("src", v.anhDaiDien);
                }

                // Mã di tích
                $("#maDiTich").val(v.maDinhDanh || "");
                // Cấp xếp hạng
                $("#capXepHang").val(v.xepHangID).trigger("change");
                // Ngày công nhận
                $("#ngayCongNhan").val(v.ngayCongNhan ? formatShortDate(v.ngayCongNhan) : '');
                // Tình trạng
                $("#tinhTrang").val(v.trangThaiID).trigger("change");
                // Đơn vị quản lý
                $("#donViQuanLy").val(v.toChucQuanLyID).trigger("change");
                // Loại hình di tích
                $("#loaiHinhDiTich").val(v.loaiHinhID).trigger("change");
                // Thứ tự
                $("#thuTu").val(v.thuTu || 1);

                // Trạng thái (radio)
                if (v.suDung == 1) {
                    $("#trangThaiDuyet").prop("checked", true);
                } else {
                    $("#trangThaiChuaDuyet").prop("checked", true);
                }
                translations = v.banDich
                var tiengViet = translations.find(el => el.maNgonNgu === 'vi') || null
                if(tiengViet != null)
                $("#ngonNgu").val('vi').trigger('change');
                $("#tenDiTich").val(tiengViet.tenDiTich);
                $("#tenGoiKhac").val(tiengViet.tenGoiKhac);
                $("#nienDai").val(tiengViet.nienDai);
                $("#moTaDiTich").val(tiengViet.moTaDiTich);

            } else {
                showNotification(0, data.error)
            }
        }
        catch (err) {
            showNotification(0, err.message)
        }
    }

    // Hàm lưu tạm cho 1 ngôn ngữ
    function saveTempTranslation() {
        let maNgonNgu = $("#ngonNgu").val();
        let tenDiTich = $("#tenDiTich").val().trim();
        let tenGoiKhac = $("#tenGoiKhac").val().trim();
        let nienDai = $("#nienDai").val().trim();
        let moTaDiTich = $("#moTaDiTich").val().trim();

        if (!maNgonNgu || !tenDiTich) return; // bắt buộc chọn ngôn ngữ và nhập tên di tích

        // Kiểm tra ngôn ngữ đã tồn tại trong mảng chưa
        let exist = translations.find(t => t.maNgonNgu === maNgonNgu);
        if (exist) {
            exist.tenDiTich = tenDiTich;
            exist.tenGoiKhac = tenGoiKhac;
            exist.nienDai = nienDai;
            exist.moTaDiTich = moTaDiTich;
        } else {
            translations.push({
                maNgonNgu: maNgonNgu,
                tenDiTich: tenDiTich,
                tenGoiKhac: tenGoiKhac,
                nienDai: nienDai,
                moTaDiTich: moTaDiTich
            });
        }
        console.log("Lưu tạm:", translations);
    }

    // Sự kiện blur trên input/textarea → tự động lưu
    $("#tenDiTich, #tenGoiKhac, #nienDai, #moTaDiTich, #coSoVatChatChinh, #coSoVatChatKhac, #chuongTrinhTraiNghiem")
        .on("blur", function () {
            saveTempTranslation();
        });

    // Sự kiện đổi ngôn ngữ → bind lại dữ liệu đã lưu
    $("#ngonNgu").on("change", function () {
        let maNgonNgu = $(this).val();
        let exist = translations.find(t => t.maNgonNgu === maNgonNgu);

        if (exist) {
            $("#tenDiTich").val(exist.tenDiTich);
            $("#tenGoiKhac").val(exist.tenGoiKhac);
            $("#nienDai").val(exist.nienDai);
            $("#moTaDiTich").val(exist.moTaDiTich);
        } else {
            // reset nếu chưa có dữ liệu
            $("#tenDiTich").val("");
            $("#tenGoiKhac").val("");
            $("#nienDai").val("");
            $("#moTaDiTich").val("");
        }

        console.log("Sau khi đổi ngôn ngữ:", translations);
    });


    $("#btn-save").on("click", function (e) {
        saveTempTranslation();
        dataAdd();
        e.preventDefault();
    });

    $("#btn-save-and-close").on("click", function (e) {
        saveTempTranslation();
        translations = translations.filter(t => t.tenDiTich && t.tenDiTich.trim() !== ""); // loại bỏ các bản dịch không có tên)
        dataAdd();
        e.preventDefault();

        setTimeout(function () {
            window.location.href = '/AdminTool/DiSanVanHoaVatThe';
        }, 2000)
    });
       
    async function dataAdd() {
        let anhDaiDien = $('#anhDaiDien')[0]?.files[0] || null;
        let maDiTich = $('#maDiTich').val()?.trim() || null;
        let loaiHinhDiTich = $('#loaiHinhDiTich').val() || null;
        let capXepHang = $('#capXepHang').val() || null;
        let ngayCongNhan = $('#ngayCongNhan').val()?.trim() || null;
        let tinhTrang = $('#tinhTrang').val() || null;
        let donViQuanLy = $('#donViQuanLy').val() || null;
        let thuTu = $('#thuTu').val() || 1;
        let trangThai = $("input[name='trangThai']:checked").val();
        let urlAnhDaiDien = $('#urlAnhDaiDien').val() || null

        var tenTiengViet = translations.find(el => el.maNgonNgu === 'vi')?.tenDiTich || null
        if (translations?.length <= 0 || tenTiengViet == null) {
            showNotification(0, 'Vui lòng nhập đầy đủ thông tin bắt buộc (*), bắt buộc có bản dịch tiếng Việt')
            return;
        }

        let thongTinChung = {
            diTichID: diTichId || "00000000-0000-0000-0000-000000000000",
            maDinhDanh: maDiTich,
            anhDaiDien: urlAnhDaiDien,
            xepHangID: capXepHang,
            ngayCongNhan: ngayCongNhan != null ? formatDateToSearch(ngayCongNhan) : null,
            trangThaiID: tinhTrang,
            toChucQuanLyID: donViQuanLy,
            loaiHinhID: loaiHinhDiTich,
            thuTu: thuTu,
            suDung: trangThai == "1" ? true : false,
        };

        let formData = new FormData()
        formData.append("DiTich", JSON.stringify(thongTinChung));
        formData.append("DiTich_NoiDung", JSON.stringify(translations));

        if (anhDaiDien != null) {
            formData.append("AnhDaiDien", anhDaiDien);
        }

        if (checkEmptyBlank(diTichId)) {
            try {
                const res = await fetch('/api/DiTichApi/ThemMoi', {
                    method: 'POST',
                    body: formData
                })

                if (!res.ok) {
                    var errText = await res.text();
                    throw new Error(errText);
                }
                data = await res.json();

                if (data && data.isSuccess && data.value) {
                    showNotification(1, 'Thêm mới thành công')
                    translations = []
                    diTichId = data.value.diTichID
                    reloadAccess()
                } else {
                    showNotification(0, data.error)
                }
            }
            catch (err) {
                console.log(err.message)
            }
        } else {
            try {
                const res = await fetch(`/api/DiTichApi/ChinhSua/${diTichId}`, {
                    method: 'PUT',
                    body: formData
                })

                if (!res.ok) {
                    var errText = await res.text();
                    throw new Error(errText);
                }

                data = await res.json();

                if (data && data.isSuccess && data.value) {
                    showNotification(1, 'Chỉnh sửa thành công')
                    diTichId = data.value.diTichID
                    reloadAccess()
                } else {
                    showNotification(0, data.error)
                }
            }
            catch (err) {
                console.log(err.message)
            }
        }
    }
    //#endregion THÔNG TIN CHUNG

    //#region ĐA PHƯƠNG TIỆN

    // Hàm lưu tạm cho 1 ngôn ngữ
    function saveTempTranslationDPT() {
        let maNgonNgu = $("#ngonNguDPT").val();
        let tieuDe = $("#tieuDeDPT").val().trim();
        let moTa = $("#moTaDPT").val().trim();

        if (!maNgonNgu || !tieuDe) return; // bắt buộc chọn ngôn ngữ và nhập tên di tích

        // Kiểm tra ngôn ngữ đã tồn tại trong mảng chưa
        let exist = translationsDPT.find(t => t.maNgonNgu === maNgonNgu);
        if (exist) {
            exist.tieuDe = tieuDe;
            exist.moTa = moTa;
        } else {
            translationsDPT.push({
                maNgonNgu: maNgonNgu,
                tieuDe: tieuDe,
                moTa: moTa
            });
        }
        console.log("Lưu tạm:", translationsDPT);
    }

    // Sự kiện blur trên input/textarea → tự động lưu
    $("#tieuDeDPT, #moTaDPT")
        .on("blur", function () {
            saveTempTranslationDPT();
        });

    // Sự kiện đổi ngôn ngữ → bind lại dữ liệu đã lưu
    $("#ngonNguDPT").on("change", function () {
        let maNgonNgu = $(this).val();
        let exist = translationsDPT.find(t => t.maNgonNgu === maNgonNgu);

        if (exist) {
            $("#tieuDeDPT").val(exist.tieuDe);
            $("#moTaDPT").val(exist.moTa);
        } else {
            // reset nếu chưa có dữ liệu
            $("#tieuDeDPT").val("");
            $("#moTaDPT").val("");
        }
        console.log("Sau khi đổi ngôn ngữ:", translationsDPT);
    });

    $("input[name='loaiMediaDPT']").on("change", function () {
        const loai = $(this).val();
        $("#fileDPT").val(null)
        $("#fileDPT").attr("accept", allowedExtensions[loai] || "");

    });

    // Khởi tạo theo radio được chọn mặc định
    const loaiInit = $("input[name='loaiMediaDPT']:checked").val();
    $("#fileDPT").attr("accept", allowedExtensions[loaiInit] || "");

    async function initTableDaPhuongTien() {
        const tableApi = {
            url: `${baseUrl}/api/DaPhuongTienApi/DanhSach`,
            type: "POST",
            data: function (d) {
                return JSON.stringify({
                    DoiTuongSoHuuID: diTichId || guidEmpty,
                    maNgonNgu: 'vi'
                });
            },
            contentType: 'application/json; charset=utf-8',
            dataSrc: function (data) {
                if (data && data.isSuccess && data.value.length > 0) {
                    data.value.forEach((item, index) => {
                        item.stt = index + 1;
                    });
                    return data.value;
                }
                return [];
            },
        };

        const tableDefs = [
            {
                targets: 1,
                render: function (data, type, row, meta) {
                    let tenFile = data ? data.split('/').pop() : '';
                    return `<span class="detail-command-btn text-blue" id=n-"${meta.row}">${tenFile}</span>`;
                }
            },
            {
                targets: 5,
                render: function (data, type, row, meta) {
                    if (data == 1) {
                        return `Hình ảnh`;
                    } else if (data == 2) {
                        return `Video`;
                    } else if (data == 3) {
                        return `Audio`;
                    } else if (data == 4) {
                        return `File`;
                    }
                    return ``;
                }
            },
            {
                targets: 6,
                render: function (data, type, row, meta) {
                    if (row.suDung) {
                        return `<i class="hgi-icon hgi-check"></i>`;
                    }
                    else {
                        return `<i class="hgi-icon hgi-cancel"></i>`;
                    }
                }
            },
            {
                targets: 7,
                render: function (data, type, row, meta) {
                    let html = ""
                    if (1) {
                        html += `<i data-toggle="tooltip" title="Chỉnh sửa" class="edit-command-btn text-yellow" id=n-"${meta.row}">
                                   <i class="hgi-icon hgi-edit"></i>
                                </i>`;
                    }
                    if (1) {
                        html += `<i data-toggle="tooltip" title="Xoá" class="delete-command-btn text-red" id=n-"${meta.row}">
                                   <i class="hgi-icon hgi-delete"></i>
                                </i>`
                    }
                    return html
                }
            }
        ];

        const tableCols = [
            { "data": "stt", "width": "40px", "class": "center-align" },
            { "data": "duongDanFile", "width": "25%", "class": "left-align" },
            { "data": "tieuDe", "width": "25%", "class": "left-align" },
            { "data": "moTa", "width": "120px", "class": "left-align" },
            { "data": "tacGia", "width": "120px", "class": "center-align" },
            { "data": "loaiMedia", "width": "120px", "class": "center-align" },
            { "data": "suDung", "width": "120px", "class": "center-align" },
            { "data": "", "width": "120px", "class": "center-align group-icon-action" },
        ];

        //if (!permitedEdit && !permitedDelete) {
        //    tableCols.pop();
        //    tableDefs.pop();
        //}

        initDataTableConfigNoSearch('dataGridDaPhuongTien', tableApi, tableDefs, tableCols);

        $('#dataGridDaPhuongTien tbody').on('click', '.edit-command-btn', async function () {
            var id = $(this).attr("ID").match(/\d+/)[0];
            var data = $('#dataGridDaPhuongTien').DataTable().row(id).data();
            $('#idDPT').val(data.daPhuongTienID);

            // Các input text
            $("input[name='loaiMediaDPT'][value='" + data.loaiMedia + "']").prop("checked", true);
            if (data.duongDanFile) {
                $("#showFileEdit").text(data.duongDanFile);
                $("#duongDanFileDPT").val(data.duongDanFile);
            } else {
                $("#showFileEdit").text("");
            }
            $("#tacGiaDPT").val(data.tacGia);
            $("#thuTuDPT").val(data.thuTuHienThi);

            // Radio trạng thái
            $(`input[name='trangThaiAdd'][value='${data.suDung == true ? "1" : "0"}'`).prop('checked', true);

            translationsDPT = []
            const res = await fetch(`/api/DaPhuongTienApi/BanDich/${data.daPhuongTienID}`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                }
            })

            if (!res.ok) {
                var errText = await res.text();
                throw new Error(errText);
            }
            data = await res.json();

            if (data && data.isSuccess && data.value) {
                translationsDPT = data.value
            } else {
                console.log(data.error)
            }

            var banDich_vi = translationsDPT.find(el => el.maNgonNgu === 'vi')
            $('#ngonNguDPT').val('vi').trigger('change');
            $('#tieuDeDPT').val(banDich_vi.tieuDe);
            $('#moTaDPT').val(banDich_vi.moTa);


            $('#modalAddDaPhuongTien').modal('show');
        });
        $('#dataGridDaPhuongTien tbody').on('click', '.delete-command-btn', function () {
            var id = $(this).attr("ID").match(/\d+/)[0];
            var data = $('#dataGridDaPhuongTien').DataTable().row(id).data();

            $('#idDelete').val(data.daPhuongTienID);
            $('#nameDelete').text(`${data.tieuDe}`);
            $('#idTypeDelete').val(1);

            $('#modalDelete').modal('show');
        });

        //$('#dataGridDaPhuongTien tbody').on('click', '.detail-command-btn', function () {
        //    var id = $(this).attr("ID").match(/\d+/)[0];
        //    var data = $('#dataGrid').DataTable().row(id).data();

        //    $('#linhVucChuyenSauDetail').text(data.tenLinhVucChuyenSau || "");
        //    $('#tanSuatDetail').text(data.tenTanSuat || "");
        //    //$('#thuTuDetail').text(data.thuTu);
        //    $('#trangThaiDetail').text(data.trangThai == true ? "Đã phát hành" : "Chưa phát hành");

        //    $('#ngonNguDetail').text(data.tenNgonNgu || "");
        //    $('#tenAnPhamDetail').text(data.tenAnPham || "");
        //    $('#doiTuongDocGiaDetail').text(data.doiTuongDocGia || "");

        //    // Thông tin audit
        //    $('#ngayTaoDetail').text(formatDateTime(data.ngayCapNhat));
        //    $('#nguoiTaoDetail').text(data.tenNguoiCapNhat || "");
        //    $('#ngayCapNhatDetail').text(formatDateTime(data.ngayHieuChinh));
        //    $('#nguoiCapNhatDetail').text(data.tenNguoiHieuChinh || "");

        //    $('#modalDetail').modal('show');
        //});

    }

    $('#btnSaveDPT').on('click', function () {
        dataAddDaPhuongTien();
    });

    $('#deleteFileDPT').on('click', function () {
        $('#showFileEdit').text("");
        $('#duongDanFile').val("");
    });
    async function dataAddDaPhuongTien() {
        if (diTichId == null || diTichId == guidEmpty) {
            showNotification(0, 'Vui lòng lưu thông tin di tích trước khi thêm đa phương tiện')
            return;
        }
        let daPhuongTienID = $('#idDPT').val() || guidEmpty
        let loaiMedia = $("input[name='loaiMediaDPT']:checked").val();
        let fileDPT = $("#fileDPT")[0].files[0] || null;
        let tacGia = $("#tacGiaDPT").val();
        let thuTu = $("#thuTuDPT").val();
        let trangThai = $("input[name='trangThaiDPT']:checked").val();
        let duongDanFile = $("#duongDanFileDPT").val();

        if (!fileDPT && checkEmptyBlank(duongDanFile)) {
            showNotification(0, 'Vui lòng nhập đầy đủ thông tin bắt buộc (*), bắt buộc có bản dịch tiếng Việt')
            return;
        }


        let thongTinChung = {
            DaPhuongTienID: daPhuongTienID,
            DoiTuongSoHuuID: diTichId,
            LoaiMedia: Number(loaiMedia),
            LoaiDoiTuong: "VH_DiTich",
            DuongDanFile: duongDanFile,
            TacGia: tacGia,
            ThuTuHienThi: thuTu || 1,
            SuDung: trangThai == "1" ? true : false
        };

        let formData = new FormData();
        formData.append("DaPhuongTien", JSON.stringify(thongTinChung));
        formData.append("DaPhuongTien_NoiDung", JSON.stringify(translationsDPT));

        if (fileDPT) {
            formData.append("File", fileDPT);
        }

        if (daPhuongTienID != null || daPhuongTienID != guidEmpty) {
            try {
                const res = await fetch('/api/DaPhuongTienApi/ThemMoi', {
                    method: 'POST',
                    body: formData
                })

                if (!res.ok) {
                    var errText = await res.text();
                    throw new Error(errText);
                }
                data = await res.json();

                if (data && data.isSuccess && data.value) {
                    showNotification(1, 'Thêm mới thành công')
                    translationsDPT = []
                    $('#dataGridDaPhuongTien').DataTable().ajax.reload();
                    $('#modalAddDaPhuongTien').modal('hide');
                } else {
                    showNotification(0, data.error)
                }
            }
            catch (err) {
                console.log(err.message)
            }
        } else {
            try {
                const res = await fetch(`/api/DaPhuongTienApi/ChinhSua/${daPhuongTienID}`, {
                    method: 'PUT',
                    body: formData
                })

                if (!res.ok) {
                    var errText = await res.text();
                    throw new Error(errText);
                }

                data = await res.json();

                if (data && data.isSuccess && data.value) {
                    showNotification(1, 'Chỉnh sửa thành công')
                    translationsDPT = []

                    $('#dataGridDaPhuongTien').DataTable().ajax.reload();
                    $('#modalAddDaPhuongTien').modal('hide');
                } else {
                    showNotification(0, data.error)
                }
            }
            catch (err) {
                console.log(err.message)
            }
        }
    }
    async function dataDeleteDaPhuongTien() {
        let id = $('#idDelete').val();
        try {
            const res = await fetch(`/api/DaPhuongTienApi/Xoa/${id}`, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json'
                }
            })

            if (!res.ok) {
                var errText = await res.text();
                throw new Error(errText);
            }

            data = await res.json();

            if (data && data.isSuccess && data.value) {
                showNotification(1, 'Xoá thành công')
                $('#modalDelete').modal('hide');
                $('#dataGridDaPhuongTien').DataTable().ajax.reload();
            } else {
                console.log(data.error)
            }
        }
        catch (err) {
            showNotification(0, err.message)
        }
    }

    // reset modal
    $("#modalAddDaPhuongTien").on('hidden.bs.modal', function () {
        translationsDPT = []
        $(this).find('input[type=text], input[type=number], textarea').val('');
        $(this).find('select').val('vi').trigger('change');
        $(this).find('input[type=radio][value=1], input[type=checkbox][value=1]').prop('checked', true);
        $('#showFileEdit').text('')
        $('#fileDPT').val(null)
    });
    //#endregion


    //#region VĂN BẢN TÀI LIỆU
    async function initTableVanBanTaiLieu() {
        const tableApi = {
            url: `${baseUrl}/api/VanBanTaiLieuApi/DanhSach`,
            type: "POST",
            data: function (d) {
                return JSON.stringify({
                    doiTuongSoHuuID: diTichId || guidEmpty,
                    maNgonNgu: 'vi'
                });
            },
            contentType: 'application/json; charset=utf-8',
            dataSrc: function (data) {
                if (data && data.isSuccess && data.value.length > 0) {
                    data.value.forEach((item, index) => {
                        item.stt = index + 1;
                        });
                    return data.value;
                }
                return [];
            },
        };

        const tableDefs = [
            {
                targets: 4,
                render: function (data, type, row, meta) {
                    let ngay = data ? formatShortDate(data) : '';
                    return `<span>${ngay}</span>`;
                }
            },
            {
                targets: 5, 
                render: function (data, type, row, meta) {
                    let ngay = data ? formatShortDate(data) : '';
                    return `<span>${ngay}</span>`;
                }
            },
            {
                targets: 7,
                render: function (data, type, row, meta) {
                    let html = ""
                    html += `<i data-toggle="tooltip" title="Chỉnh sửa" class="edit-command-btn text-yellow" id=n-"${meta.row}">
                                   <i class="hgi-icon hgi-edit"></i>
                                </i>`;
                    html += `<i data-toggle="tooltip" title="Xoá" class="delete-command-btn text-red" id=n-"${meta.row}">
                                   <i class="hgi-icon hgi-delete"></i>
                                </i>`
                    return html
                }
            }
        ];

        const tableCols = [
            { "data": "stt", "width": "40px", "class": "center-align" },
            { "data": "soKyHieu", "width": "120px", "class": "left-align" },
            { "data": "trichYeu", "width": "", "class": "left-align" },
            { "data": "tenCoQuanBanHanh", "width": "25%", "class": "left-align" },
            { "data": "ngayBanHanh", "width": "120px", "class": "center-align" },
            { "data": "ngayHieuLuc", "width": "120px", "class": "center-align" },
            { "data": "duongDanFile", "width": "120px", "class": "center-align" },
            { "data": "", "width": "120px", "class": "center-align group-icon-action" },
        ];

        initDataTableConfigNoSearch('dataGridVanBanTaiLieu', tableApi, tableDefs, tableCols);

        $('#dataGridVanBanTaiLieu tbody').on('click', '.edit-command-btn', async function () {
            var id = $(this).attr("ID").match(/\d+/)[0];
            var data = $('#dataGridVanBanTaiLieu').DataTable().row(id).data();
            $('#idVBTL').val(data.vanBanID);

            if (data.duongDanFile) {
                $("#showFileVBTL").text(data.duongDanFile);
                $("#duongDanFileVBTL").val(data.duongDanFile);
            } else {
                $("#showFileVBTL").text("");
            }
            $("#trichYeuVBTL").val(data.trichYeu || '');
            $("#soKyHieuVBTL").val(data.soKyHieu || '');
            $("#nguoiKyVBTL").val(data.nguoiKy || '');
            $("#noiDungVBTL").val(data.noiDung || '');
            $("#ngayBanHanhVBTL").val(data.ngayBanHanh != null ? formatShortDate(data.ngayBanHanh) : "");
            $("#ngayHieuLucVBTL").val(data.ngayHieuLuc != null ? formatShortDate(data.ngayHieuLuc) : "");
            $("#coQuanBanHanhVBTL").val(data.coQuanBanHanhID || '').trigger('change');

            $('#modalAddVanBanTaiLieu').modal('show');
        });
        $('#dataGridVanBanTaiLieu tbody').on('click', '.delete-command-btn', function () {
            var id = $(this).attr("ID").match(/\d+/)[0];
            var data = $('#dataGridVanBanTaiLieu').DataTable().row(id).data();

            $('#idDelete').val(data.vanBanID);
            $('#nameDelete').text(`${data.trichYeu}`);
            $('#idTypeDelete').val(3);

            $('#modalDelete').modal('show');
        });

        //$('#dataGridDaPhuongTien tbody').on('click', '.detail-command-btn', function () {
        //    var id = $(this).attr("ID").match(/\d+/)[0];
        //    var data = $('#dataGrid').DataTable().row(id).data();

        //    $('#linhVucChuyenSauDetail').text(data.tenLinhVucChuyenSau || "");
        //    $('#tanSuatDetail').text(data.tenTanSuat || "");
        //    //$('#thuTuDetail').text(data.thuTu);
        //    $('#trangThaiDetail').text(data.trangThai == true ? "Đã phát hành" : "Chưa phát hành");

        //    $('#ngonNguDetail').text(data.tenNgonNgu || "");
        //    $('#tenAnPhamDetail').text(data.tenAnPham || "");
        //    $('#doiTuongDocGiaDetail').text(data.doiTuongDocGia || "");

        //    // Thông tin audit
        //    $('#ngayTaoDetail').text(formatDateTime(data.ngayCapNhat));
        //    $('#nguoiTaoDetail').text(data.tenNguoiCapNhat || "");
        //    $('#ngayCapNhatDetail').text(formatDateTime(data.ngayHieuChinh));
        //    $('#nguoiCapNhatDetail').text(data.tenNguoiHieuChinh || "");

        //    $('#modalDetail').modal('show');
        //});
    }

    $('#btnSaveVBTL').on('click', function () {
        dataAddVanBanTaiLieu()
    });
    async function dataAddVanBanTaiLieu() {
        if (diTichId == null || diTichId == guidEmpty) {
            showNotification(0, 'Vui lòng lưu thông tin di tích trước khi thêm văn bản tài liệu')
            return;
        }

        let idVBTL = $('#idVBTL').val()?.trim();
        let duongDanFileVBTL = $('#duongDanFileVBTL').val()?.trim();
        let fileVBTL = $('#fileVBTL')[0]?.files[0] || null;
        let soKyHieuVBTL = $('#soKyHieuVBTL').val()?.trim();
        let trichYeuVBTL = $('#trichYeuVBTL').val()?.trim();
        let nguoiKyVBTL = $('#nguoiKyVBTL').val()?.trim();
        let noiDungVBTL = $('#noiDungVBTL').val()?.trim();
        let coQuanBanHanhVBTL = $('#coQuanBanHanhVBTL').val();
        let ngayBanHanhVBTL = $('#ngayBanHanhVBTL').val();
        let ngayHieuLucVBTL = $('#ngayHieuLucVBTL').val();

        if ((fileVBTL == null && checkEmptyBlank(duongDanFileVBTL)) || checkEmptyBlank(soKyHieuVBTL) || checkEmptyBlank(trichYeuVBTL)) {
            showNotification(0, 'Vui lòng nhập đầy đủ thông tin bắt buộc (*), bắt buộc có bản dịch tiếng Việt')
            return;
        }
        let formData = new FormData()

        formData.append("vanBanID", idVBTL);
        formData.append("doiTuongSoHuuID", diTichId);
        formData.append("duongDanFile", duongDanFileVBTL);
        formData.append("soKyHieu", soKyHieuVBTL);
        formData.append("trichYeu", trichYeuVBTL);
        formData.append("nguoiKy", nguoiKyVBTL);
        formData.append("noiDung", noiDungVBTL);
        formData.append("coQuanBanHanhID", coQuanBanHanhVBTL);
        formData.append("ngayBanHanh", formatDateToSearch(ngayBanHanhVBTL));
        formData.append("ngayHieuLuc", formatDateToSearch(ngayHieuLucVBTL));
        formData.append("File", fileVBTL);

        if (checkEmptyBlank(idVBTL)) {
            try {
                const res = await fetch('/api/VanBanTaiLieuApi/ThemMoi', {
                    method: 'POST',
                    body: formData
                })

                if (!res.ok) {
                    var errText = await res.text();
                    throw new Error(errText);
                }
                data = await res.json();

                if (data && data.isSuccess && data.value) {
                    showNotification(1, 'Thêm mới thành công')
                    translations = []
                    $('#modalAddVanBanTaiLieu').modal('hide');
                    $('#dataGridVanBanTaiLieu').DataTable().ajax.reload();
                } else {
                    showNotification(0, data.error)
                }
            }
            catch (err) {
                console.log(err.message)
            }
        } else {
            try {
                const res = await fetch(`/api/VanBanTaiLieuApi/ChinhSua/${idVBTL}`, {
                    method: 'PUT',
                    body: formData
                })

                if (!res.ok) {
                    var errText = await res.text();
                    throw new Error(errText);
                }

                data = await res.json();

                if (data && data.isSuccess && data.value) {
                    showNotification(1, 'Chỉnh sửa thành công')
                    $('#modalAddVanBanTaiLieu').modal('hide');
                    $('#dataGridVanBanTaiLieu').DataTable().ajax.reload();
                } else {
                    showNotification(0, data.error)
                }
            }
            catch (err) {
                console.log(err.message)
            }
        }
    }
    async function dataDeleteVanBanTaiLieu() {
        let id = $('#idDelete').val();

        try {
            const res = await fetch(`/api/VanBanTaiLieuApi/Xoa/${id}`, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json'
                }
            })

            if (!res.ok) {
                var errText = await res.text();
                throw new Error(errText);
            }

            data = await res.json();

            if (data && data.isSuccess && data.value) {
                showNotification(1, 'Xoá thành công')
                $('#modalDelete').modal('hide');
                $('#dataGridVanBanTaiLieu').DataTable().ajax.reload();
            } else {
                console.log(data.error)
            }
        }
        catch (err) {
            showNotification(0, err.message)
        }
    }

    // reset modal
    $("#modalAddVanBanTaiLieu").on('hidden.bs.modal', function () {
        $(this).find('input[type=text], input[type=number], textarea').val('');
        $(this).find('select').val('vi').trigger('change');
        $(this).find('input[type=radio][value=1], input[type=checkbox][value=1]').prop('checked', true);
        $('#modalAddTitle').text('Thêm mới văn bản tài liệu')
        $('#showFileVBTL').text('')
        $('#fileVBTL').val(null)
    });
    //#endregion

    //#region TRÙNG TU DI TÍCH

    // Hàm lưu tạm cho 1 ngôn ngữ
    function saveTempTranslationTT() {
        let maNgonNgu = $("#ngonNguTT").val();
        let congViec = $("#congViecTT").val()?.trim();
        let toChucThucHien = $("#toChucThucHienTT").val()?.trim();
        let thoiGian = $("#thoiGianTT").val()?.trim();
        let mucDich = $("#mucDichTT").val()?.trim();

        if (!maNgonNgu || !congViec || !toChucThucHien || !thoiGian || !mucDich) return; // bắt buộc chọn ngôn ngữ và nhập tên di tích

        // Kiểm tra ngôn ngữ đã tồn tại trong mảng chưa
        let exist = translationsTT.find(t => t.maNgonNgu === maNgonNgu);
        if (exist) {
            exist.congViec = congViec;
            exist.toChucThucHien = toChucThucHien;
            exist.thoiGian = thoiGian;
            exist.mucDich = mucDich;
        } else {
            translationsTT.push({
                maNgonNgu: maNgonNgu,
                congViec: congViec,
                toChucThucHien: toChucThucHien,
                thoiGian: thoiGian,
                mucDich: mucDich
            });
        }
        console.log("Lưu tạm:", translationsTT);
    }

    // Sự kiện blur trên input/textarea → tự động lưu
    $("#congViecTT, #toChucThucHienTT, #thoiGianTT, #mucDichTT")
        .on("blur", function () {
            saveTempTranslationTT();
        });

    // Sự kiện đổi ngôn ngữ → bind lại dữ liệu đã lưu
    $("#ngonNguTT").on("change", function () {
        let maNgonNgu = $(this).val();

        let exist = translationsTT.find(t => t.maNgonNgu === maNgonNgu);
        if (exist) {
            $("#congViecTT").val(exist.congViec)
            $("#toChucThucHienTT").val(exist.toChucThucHien)
            $("#thoiGianTT").val(exist.thoiGian)
            $("#mucDichTT").val(exist.mucDich)
        } else {
            $("#congViecTT").val("")
            $("#toChucThucHienTT").val("")
            $("#thoiGianTT").val("")
            $("#mucDichTT").val("")
        }
        console.log("Sau khi đổi ngôn ngữ:", translationsTT);
    });

    async function initTableTrungTu() {
        const tableApi = {
            url: `${baseUrl}/api/TrungTuDiTichApi/DanhSach`,
            type: "POST",
            data: function (d) {
                return JSON.stringify({
                    diTichID: diTichId || guidEmpty,
                });
            },
            contentType: 'application/json; charset=utf-8',
            dataSrc: function (data) {
                if (data && data.isSuccess && data.value.length > 0) {
                    data.value.forEach((item, index) => {
                        item.stt = index + 1;
                    });
                    banDichTT = data.value
                    return data.value.filter(e => e.maNgonNgu == 'vi');
                }
                return [];
            },
        };

        const tableDefs = [
            {
                targets: 5,
                render: function (data, type, row, meta) {
                    let html = ""
                    html += `<i data-toggle="tooltip" title="Chỉnh sửa" class="edit-command-btn text-yellow" id=n-"${meta.row}">
                                   <i class="hgi-icon hgi-edit"></i>
                                </i>`;
                    html += `<i data-toggle="tooltip" title="Xoá" class="delete-command-btn text-red" id=n-"${meta.row}">
                                   <i class="hgi-icon hgi-delete"></i>
                                </i>`
                    return html
                }
            }
        ];

        const tableCols = [
            { "data": "stt", "width": "40px", "class": "center-align" },
            { "data": "congViec", "width": "120px", "class": "left-align" },
            { "data": "toChucThucHien", "width": "", "class": "left-align" },
            { "data": "thoiGian", "width": "25%", "class": "left-align" },
            { "data": "mucDich", "width": "120px", "class": "center-align" },
            { "data": "", "width": "120px", "class": "center-align group-icon-action" },
        ];

        initDataTableConfigNoSearch('dataGridTrungTu', tableApi, tableDefs, tableCols);

        $('#dataGridTrungTu tbody').on('click', '.edit-command-btn', async function () {
            var id = $(this).attr("ID").match(/\d+/)[0];
            var data = $('#dataGridTrungTu').DataTable().row(id).data();

            $("#idTT").val(data.id)
            translationsTT = banDichTT;

            let tiengViet = banDichTT.find(t => t.maNgonNgu === 'vi' && t.lanTrungTu == data.lanTrungTu);
            $("#congViecTT").val(tiengViet.congViec || '');
            $("#toChucThucHienTT").val(tiengViet.toChucThucHien || '');
            $("#thoiGianTT").val(tiengViet.thoiGian || '');
            $("#mucDichTT").val(tiengViet.mucDich || '');

            // reload lại bảng văn bản tài liệu trùng tu
            $('#dataGridTLTT').DataTable().ajax.reload();

        });
        $('#dataGridTrungTu tbody').on('click', '.delete-command-btn', function () {
            var id = $(this).attr("ID").match(/\d+/)[0];
            var data = $('#dataGridTrungTu').DataTable().row(id).data();

            $('#idDelete').val(data.lanTrungTu);
            $('#nameDelete').text(`${data.congViec}`);
            $('#idTypeDelete').val(2);

            $('#modalDelete').modal('show');
        });

        //$('#dataGridDaPhuongTien tbody').on('click', '.detail-command-btn', function () {
        //    var id = $(this).attr("ID").match(/\d+/)[0];
        //    var data = $('#dataGrid').DataTable().row(id).data();

        //    $('#linhVucChuyenSauDetail').text(data.tenLinhVucChuyenSau || "");
        //    $('#tanSuatDetail').text(data.tenTanSuat || "");
        //    //$('#thuTuDetail').text(data.thuTu);
        //    $('#trangThaiDetail').text(data.trangThai == true ? "Đã phát hành" : "Chưa phát hành");

        //    $('#ngonNguDetail').text(data.tenNgonNgu || "");
        //    $('#tenAnPhamDetail').text(data.tenAnPham || "");
        //    $('#doiTuongDocGiaDetail').text(data.doiTuongDocGia || "");

        //    // Thông tin audit
        //    $('#ngayTaoDetail').text(formatDateTime(data.ngayCapNhat));
        //    $('#nguoiTaoDetail').text(data.tenNguoiCapNhat || "");
        //    $('#ngayCapNhatDetail').text(formatDateTime(data.ngayHieuChinh));
        //    $('#nguoiCapNhatDetail').text(data.tenNguoiHieuChinh || "");

        //    $('#modalDetail').modal('show');
        //});
    }

    $('#btnSaveTT').on('click', function () {
        dataAddTrungTu()
    });
    async function dataAddTrungTu() {
        if (diTichId == null || diTichId == guidEmpty) {
            showNotification(0, 'Vui lòng lưu thông tin di tích trước khi thêm văn bản tài liệu')
            return;
        }
        let idTrungTu = $('#idTT').val()?.trim();
        let exist = translationsTT.find(t => t.maNgonNgu === 'vi');
        if (!exist) {
            showNotification(0, 'Vui lòng nhập đầy đủ thông tin bắt buộc (*), bắt buộc có bản dịch tiếng Việt')
            return;
        }

        var lanTrungTu = $('#dataGridTrungTu').DataTable().rows().count();

        if (checkEmptyBlank(idTrungTu)) {
            lanTrungTu = lanTrungTu + 1;
        }

        translationsTT = translationsTT.map(item => ({
            ...item,
            diTichID: diTichId,
            lanTrungTu: lanTrungTu
        }));

        let formData = new FormData()
        formData.append("data", JSON.stringify(translationsTT));

        try {
            const res = await fetch('/api/TrungTuDiTichApi/ThemMoi', {
                method: 'POST',
                body: formData
            })

            if (!res.ok) {
                var errText = await res.text();
                throw new Error(errText);
            }
            data = await res.json();

            if (data && data.isSuccess && data.value && data.value.length > 0) {
                translationsTT = []
                showNotification(1, 'Cập nhật thành công')
                if (objTLTT?.length > 0) {
                    for (const item of objTLTT) {
                        item.doiTuongSoHuuID = data.value[0].id
                        dataAddTLTT(item)
                    }
                }
                $('#modalAddTrungTu').modal('hide');
                $('#dataGridTrungTu').DataTable().ajax.reload();
            } else {
                showNotification(0, data.error)
            }
        }
        catch (err) {
            console.log(err.message)
        }
    }
    async function dataDeleteTrungTu() {
        let id = $('#idDelete').val();

        try {
            const res = await fetch(`/api/TrungTuDiTichApi/Xoa/${id}/${diTichId}`, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json'
                }
            })

            if (!res.ok) {
                var errText = await res.text();
                throw new Error(errText);
            }

            data = await res.json();

            if (data && data.isSuccess && data.value) {
                showNotification(1, 'Xoá thành công')
                $('#modalDelete').modal('hide');
                $('#dataGridTrungTu').DataTable().ajax.reload();
            } else {
                console.log(data.error)
            }
        }
        catch (err) {
            showNotification(0, err.message)
        }
    }

    async function initTableTLTT() {
        const tableApi = {
            url: `${baseUrl}/api/VanBanTaiLieuApi/DanhSach`,
            type: "POST",
            data: function (d) {
                return JSON.stringify({
                    doiTuongSoHuuID: $('#idTT').val() || guidEmpty,
                    maNgonNgu: 'vi'
                });
            },
            contentType: 'application/json; charset=utf-8',
            dataSrc: function (data) {
                if (data && data.isSuccess && data.value.length > 0) {
                    data.value.forEach((item, index) => {
                        item.stt = index + 1;
                    });
                    objTLTT = data.value
                    return data.value;
                }
                return [];
            },
        };

        const tableDefs = [
            {
                targets: 4,
                render: function (data, type, row, meta) {
                    let ngay = data ? formatShortDate(data) : '';
                    return `<span>${ngay}</span>`;
                }
            },
            {
                targets: 5,
                render: function (data, type, row, meta) {
                    let ngay = data ? formatShortDate(data) : '';
                    return `<span>${ngay}</span>`;
                }
            },
            {
                targets: 7,
                render: function (data, type, row, meta) {
                    let html = ""
                    html += `<i data-toggle="tooltip" title="Chỉnh sửa" class="edit-command-btn text-yellow" id=n-"${meta.row}">
                                   <i class="hgi-icon hgi-edit"></i>
                                </i>`;
                    html += `<i data-toggle="tooltip" title="Xoá" class="delete-command-btn text-red" id=n-"${meta.row}">
                                   <i class="hgi-icon hgi-delete"></i>
                                </i>`
                    return html
                }
            }
        ];

        const tableCols = [
            { "data": "stt", "width": "40px", "class": "center-align" },
            { "data": "soKyHieu", "width": "120px", "class": "left-align" },
            { "data": "trichYeu", "width": "", "class": "left-align" },
            { "data": "tenCoQuanBanHanh", "width": "25%", "class": "left-align" },
            { "data": "ngayBanHanh", "width": "120px", "class": "center-align" },
            { "data": "ngayHieuLuc", "width": "120px", "class": "center-align" },
            { "data": "duongDanFile", "width": "120px", "class": "center-align" },
            { "data": "", "width": "120px", "class": "center-align group-icon-action" },
        ];

        initDataTableConfigNoSearch('dataGridTLTT', tableApi, tableDefs, tableCols);

        $('#dataGridTLTT tbody').on('click', '.edit-command-btn', async function () {
            var id = $(this).attr("ID").match(/\d+/)[0];
            var data = $('#dataGridTLTT').DataTable().row(id).data();

            $('#idTLTT').val(data.vanBanID);

            if (data.duongDanFile) {
                $("#showFileTLTT").text(data.duongDanFile);
                $("#duongDanFileTLTT").val(data.duongDanFile);
            } else {
                $("#showFileTLTT").text("");
            }

            $("#trichYeuTLTT").val(data.trichYeu || '');
            $("#soKyHieuTLTT").val(data.soKyHieu || '');
            $("#nguoiKyTLTT").val(data.nguoiKy || '');
            $("#noiDungTLTT").val(data.noiDung || '');
            $("#ngayBanHanhTLTT").val(data.ngayBanHanh != null ? formatShortDate(data.ngayBanHanh) : "");
            $("#ngayHieuLucTLTT").val(data.ngayHieuLuc != null ? formatShortDate(data.ngayHieuLuc) : "");
            $("#coQuanBanHanhTLTT").val(data.coQuanBanHanhID || '').trigger('change');

            $('#modalAddTLTT').modal('show');
        });

        $('#dataGridTLTT tbody').on('click', '.delete-command-btn', function () {
            var id = $(this).attr("ID").match(/\d+/)[0];
            var data = $('#dataGridTLTT').DataTable().row(id).data();

            $('#idDelete').val(data.vanBanID);
            $('#nameDelete').text(`${data.trichYeu}`);
            $('#idTypeDelete').val(4);

            $('#modalDelete').modal('show');
        });

        //$('#dataGridDaPhuongTien tbody').on('click', '.detail-command-btn', function () {
        //    var id = $(this).attr("ID").match(/\d+/)[0];
        //    var data = $('#dataGrid').DataTable().row(id).data();

        //    $('#linhVucChuyenSauDetail').text(data.tenLinhVucChuyenSau || "");
        //    $('#tanSuatDetail').text(data.tenTanSuat || "");
        //    //$('#thuTuDetail').text(data.thuTu);
        //    $('#trangThaiDetail').text(data.trangThai == true ? "Đã phát hành" : "Chưa phát hành");

        //    $('#ngonNguDetail').text(data.tenNgonNgu || "");
        //    $('#tenAnPhamDetail').text(data.tenAnPham || "");
        //    $('#doiTuongDocGiaDetail').text(data.doiTuongDocGia || "");

        //    // Thông tin audit
        //    $('#ngayTaoDetail').text(formatDateTime(data.ngayCapNhat));
        //    $('#nguoiTaoDetail').text(data.tenNguoiCapNhat || "");
        //    $('#ngayCapNhatDetail').text(formatDateTime(data.ngayHieuChinh));
        //    $('#nguoiCapNhatDetail').text(data.tenNguoiHieuChinh || "");

        //    $('#modalDetail').modal('show');
        //});
    }
    $('#btnSaveTLTT').on('click', function () {
        dataAddTLTT_Temp()
    });

    async function dataAddTLTT_Temp() {
       
        let idTLTT = $('#idTLTT').val()?.trim();
        let duongDanFileTLTT = $('#duongDanFileTLTT').val()?.trim();
        let fileTLTT = $('#fileTLTT')[0]?.files[0] || null;
        let soKyHieuTLTT = $('#soKyHieuTLTT').val()?.trim();
        let trichYeuTLTT = $('#trichYeuTLTT').val()?.trim();
        let nguoiKyTLTT = $('#nguoiKyTLTT').val()?.trim();
        let noiDungTLTT = $('#noiDungTLTT').val()?.trim();
        let coQuanBanHanhTLTT = $('#coQuanBanHanhTLTT').val();
        let ngayBanHanhTLTT = $('#ngayBanHanhTLTT').val();
        let ngayHieuLucTLTT = $('#ngayHieuLucTLTT').val();


        if ((fileTLTT == null && checkEmptyBlank(duongDanFileTLTT))
            || checkEmptyBlank(soKyHieuTLTT)
            || checkEmptyBlank(trichYeuTLTT)) {

            showNotification(0, 'Vui lòng nhập đầy đủ thông tin bắt buộc (*), bắt buộc có bản dịch tiếng Việt');
            return;
        }

        let obj = {
            vanBanID: idTLTT,
            doiTuongSoHuuID: diTichId,
            duongDanFile: duongDanFileTLTT,
            soKyHieu: soKyHieuTLTT,
            trichYeu: trichYeuTLTT,
            nguoiKy: nguoiKyTLTT,
            noiDung: noiDungTLTT,
            coQuanBanHanhID: coQuanBanHanhTLTT,
            ngayBanHanh: ngayBanHanhTLTT != "" ? formatDateToSearch(ngayBanHanhTLTT) : null,
            ngayHieuLuc: ngayHieuLucTLTT != "" ? formatDateToSearch(ngayHieuLucTLTT) : null,
            file: fileTLTT
        };

        if (checkEmptyBlank(idTLTT)) {
            obj.vanBanID = `temp_${Date.now()}`; // id tạm để quản lý
            // Gán số thứ tự (lấy theo mảng hiện tại + 1)
            obj.stt = objTLTT.length + 1;

            objTLTT.push(obj);

            // Thêm vào DataTable
            $('#dataGridTLTT').DataTable().row.add(obj).draw(false);
        } else {
            let index = objTLTT.findIndex(x => x.vanBanID === idTLTT);
            if (index !== -1) {
                obj.stt = objTLTT[index].stt; // Giữ nguyên stt cũ
                objTLTT[index] = obj;

                // Cập nhật trong DataTable
                let table = $('#dataGridTLTT').DataTable();
                let row = table.row(function (idx, data, node) {
                    return data.vanBanID === obj.vanBanID;
                });
                if (row) {
                    row.data(obj).draw(false);
                }
            } else {
                objTLTT.push(obj); // phòng trường hợp id có giá trị nhưng chưa nằm trong mảng
            }
        }
        console.log("Danh sách văn bản trùng tu:", objTLTT);
        $('#modalAddTLTT').modal('hide');
    }
    async function dataDeleteTLTT() {
        let id = $('#idDelete').val();
        console.log(id);
        objTLTT = objTLTT.filter(e => e.id != id)
        $('#dataGridTLTT').DataTable().ajax.reload();
        $('#modalDelete').modal('hide');
    }

    async function dataAddTLTT(obj) {
        let formData = new FormData();

        // Nếu vanBanID có chứa 'temp_' thì gán null
        if (obj.vanBanID && obj.vanBanID.toString().startsWith("temp_")) {
            obj.vanBanID = "";
        }

        for (const key in obj) {
            if (obj.hasOwnProperty(key)) {
                const value = obj[key];

                // Nếu null hoặc undefined thì append chuỗi rỗng để C# nhận null
                if (value === null || value === undefined) {
                    formData.append(key, "");
                }
                // Nếu là FileList hoặc mảng file
                else if (Array.isArray(value) && value.length > 0 && value[0] instanceof File) {
                    value.forEach(f => formData.append(key, f));
                }
                // Nếu là File hoặc Blob đơn lẻ
                else if (value instanceof File || value instanceof Blob) {
                    formData.append(key, value);
                }
                // Nếu là object (Date, Guid, object con, …) thì stringify
                else if (typeof value === "object") {
                    formData.append(key, JSON.stringify(value));
                }
                // Primitive (string, number, bool)
                else {
                    formData.append(key, value);
                }
            }
        }

        try {
            let url = "";
            let method = "";

            if (checkEmptyBlank(obj.vanBanID)) {
                url = "/api/VanBanTaiLieuApi/ThemMoi";
                method = "POST";
            } else {
                url = `/api/VanBanTaiLieuApi/ChinhSua/${obj.vanBanID}`;
                method = "PUT";
            }

            const res = await fetch(url, {
                method: method,
                body: formData
            });

            if (!res.ok) {
                const errText = await res.text();
                throw new Error(errText);
            }

            const data = await res.json();

            if (data && data.isSuccess && data.value) {
                console.log("Lưu văn bản trùng tu thành công:", obj);
            } else {
                showNotification(0, data.error);
            }
        } catch (err) {
            console.error(err.message);
            showNotification(0, err.message);
        }
    }
   
    $("#modalAddTLTT").on('hidden.bs.modal', function () {
        $(this).find('input[type=text], input[type=number], textarea').val('');
        $(this).find('select').val('vi').trigger('change');
        $(this).find('input[type=radio][value=1], input[type=checkbox][value=1]').prop('checked', true);
        $('#modalAddTitleTT').text('Thêm mới văn bản trùng tu')
        $('#showFileTLTT').text('')
        $('#fileTLTT').val(null)

    });
    $("#modalAddTrungTu").on('hidden.bs.modal', function () {
        translationsTT = []
        $(this).find('input[type=text], input[type=number], textarea').val('');
        $(this).find('select').val('vi').trigger('change');
        $(this).find('input[type=radio][value=1], input[type=checkbox][value=1]').prop('checked', true);
        $('#modalAddTitle').text('Thêm mới trùng tu')
    });
   

  

    //#region HÀM LOAD DỮ LIỆU CHO CÁC COMBOBOX
    async function initNgonNgu() {
        try {
            const res = await fetch('/api/NgonNguApi/DanhSach', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    trangThai: null,
                    tuKhoa: null
                })
            })

            if (!res.ok) {
                var errText = await res.text();
                throw new Error(errText);
            }
            data = await res.json();

            if (data && data.isSuccess && data.value) {
                data.value.forEach(lang => {
                    $("#ngonNgu").append(`<option value="${lang.maNgonNgu?.toLowerCase()}">${lang.tenNgonNgu}</option>`);
                    $("#ngonNguDPT").append(`<option value="${lang.maNgonNgu?.toLowerCase()}">${lang.tenNgonNgu}</option>`);
                    $("#ngonNguTT").append(`<option value="${lang.maNgonNgu?.toLowerCase()}">${lang.tenNgonNgu}</option>`);
                });
                $("#ngonNgu").val('vi').trigger('change');
                $("#ngonNguDPT").val('vi').trigger('change');
                $("#ngonNguTT").val('vi').trigger('change');
            } else {
                showNotification(0, data.error)
            }
        }
        catch (err) {
            showNotification(0, err.message)
        }
    };

    async function initLoaiHinhDiTich() {
        try {
            const res = await fetch('/api/DanhMucChungApi/DanhSach', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    loaiDanhMucID: 18,
                    tuKhoa: null
                })
            })

            if (!res.ok) {
                var errText = await res.text();
                throw new Error(errText);
            }
            data = await res.json();

            if (data && data.isSuccess && data.value) {
                data.value.forEach(el => {
                    $("#loaiHinhDiTich").append(`<option value="${el.danhMucID}">${el.tenDanhMuc}</option>`);
                });
            } else {
                showNotification(0, data.error)
            }
        }
        catch (err) {
            console.log(err.message)
        }
    };

    async function initCapXepHang() {
        try {
            const res = await fetch('/api/DanhMucChungApi/DanhSach', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    loaiDanhMucID: 17,
                    tuKhoa: null
                })
            })

            if (!res.ok) {
                var errText = await res.text();
                throw new Error(errText);
            }
            data = await res.json();

            if (data && data.isSuccess && data.value) {
                data.value.forEach(el => {
                    $("#capXepHang").append(`<option value="${el.danhMucID}">${el.tenDanhMuc}</option>`);
                });
            } else {
                showNotification(0, data.error)
            }
        }
        catch (err) {
            console.log(err.message)
        }
    };

    async function initCoQuanBanHanhVanBan() {
        try {
            const res = await fetch('/api/ToChucApi/DanhSach', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    loaiToChucID: 170
                })
            })

            if (!res.ok) {
                var errText = await res.text();
                throw new Error(errText);
            }
            data = await res.json();


            if (data && data.isSuccess && data.value) {
                data.value.forEach(el => {
                    $("#coQuanBanHanhVBTL").append(`<option value="${el.toChucID}">${el.tenToChuc}</option>`);
                    $("#coQuanBanHanhTLTT").append(`<option value="${el.toChucID}">${el.tenToChuc}</option>`);
                });
            } else {
                showNotification(0, data.error)
            }
        }
        catch (err) {
            console.log(err.message)
        }
    };

    async function initDonViQuanLy() {
        try {
            const res = await fetch('/api/ToChucApi/DanhSach', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    loaiToChucID: 122
                })
            })

            if (!res.ok) {
                var errText = await res.text();
                throw new Error(errText);
            }
            data = await res.json();


            if (data && data.isSuccess && data.value) {
                data.value.forEach(el => {
                    $("#donViQuanLy").append(`<option value="${el.toChucID}">${el.tenToChuc}</option>`);
                });
            } else {
                showNotification(0, data.error)
            }
        }
        catch (err) {
            console.log(err.message)
        }
    };

    function reloadAccess() {
        if (checkEmptyBlank(diTichId)) {
            $("#thong-tin-dia-diem-tab").closest("li").hide();
            $("#thong-tin-da-phuong-tien-tab").closest("li").hide();
            $("#thong-tin-trung-tu-tab").closest("li").hide();
            $("#van-ban-tai-lieu-tab").closest("li").hide();
        } else {
            $("#thong-tin-dia-diem-tab").closest("li").show();
            $("#thong-tin-da-phuong-tien-tab").closest("li").show();
            $("#thong-tin-trung-tu-tab").closest("li").show();
            $("#van-ban-tai-lieu-tab").closest("li").show();
        }
    }
    //#endregion
    
})