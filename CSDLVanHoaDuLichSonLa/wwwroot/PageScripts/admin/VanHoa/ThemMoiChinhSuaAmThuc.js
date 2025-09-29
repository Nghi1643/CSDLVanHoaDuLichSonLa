const baseUrl = getRootLink();
const allowedExtensions = {
    1: ".jpg,.jpeg,.png,.gif,.webp,.bmp",                // Ảnh
    2: ".mp4,.webm,.ogg,.mov,.avi",                      // Video
    3: ".mp3,.wav,.m4a",                                 // Audio
    4: ".doc,.docx,.pdf,.xlsx,.xls,.dot"                      // File
};
const guidEmpty = "00000000-0000-0000-0000-000000000000";

$(document).ready(function () {
    initSelect2();
    initDatePicker();

    let amThucTranslations = [];
    let daPhuongTientranslations = [];
    ; (async function () {
        await Promise.all([
            initNgonNgu(),
            initKieuMonAmThuc(),
            initCheDoAnAmThuc(),
            initCoQuanBanHanhVanBan()
        ]);
        initThongTinChung();
        initTableDaPhuongTien();
        initTableVanBanTaiLieu();
        reloadAccess()
    })();

    $("#formDelete").on("submit", function (e) {
        e.preventDefault();
        let type = $('#idTypeDelete').val();
        if (type == "1") {
            dataDeleteDaPhuongTien();
        } else if (type == "2") {
            dataDeleteVanBanTaiLieu()
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
        if (checkEmptyBlank(monAnUongID)) {
            return;
        }

        try {
            const res = await fetch(`/api/AmThucApi/ChiTiet/${monAnUongID}`, {
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
                $("#monAnUongID").val(data.value.monAnUongID || "");
                $("#previewAnhDaiDien").attr("src", data.value.anhDaiDien || "/images/no-image.png");
                $("#urlAnhDaiDien").val(data.value.anhDaiDien || "");
                $("#kieuMonID").val(data.value.kieuMonID || "").trigger("change");
                $("#cheDoAnID").val(data.value.cheDoAnID || "").trigger("change");
                $("#thuTu").val(data.value.thuTu || 1);
                if (data.value.dacSan === true || data.value.dacSan === 1) {
                    $("#dacSanCo").prop("checked", true);
                } else {
                    $("#dacSanKhong").prop("checked", true);
                }
                if (data.value.suDung === true || data.value.suDung === 1) {
                    $("#suDungCo").prop("checked", true);
                } else {
                    $("#suDungKhong").prop("checked", true);
                }

                // Bản dịch
                amThucTranslations = data.value.banDich || [];

                let tiengViet = amThucTranslations.find(el => el.maNgonNgu === "vi") || null;
                if (tiengViet) {
                    $("#ngonNgu").val("vi").trigger("change");
                    $("#tenMon").val(tiengViet.tenMon || "");
                    $("#traiNghiem").val(tiengViet.traiNghiem || "");
                    $("#nguyenLieu").val(tiengViet.nguyenLieu || "");
                    $("#moTa").val(tiengViet.moTa || "");
                } else {
                    $("#tenMon, #traiNghiem, #nguyenLieu, #moTa").val("");
                }
            } else {
                showNotification(0, data.error);
            }
        }
        catch (err) {
            showNotification(0, err.message)
        }
    }

    function saveTempAmThucTranslation() {
        let maNgonNgu = $("#ngonNgu").val();
        let tenMon = $("#tenMon").val().trim();
        let nguyenLieu = $("#nguyenLieu").val().trim();
        let traiNghiem = $("#traiNghiem").val().trim();
        let moTa = $("#moTa").val().trim();

        if (!maNgonNgu) return; // phải chọn ngôn ngữ

        // tìm index ngôn ngữ trong mảng
        let index = amThucTranslations.findIndex(t => t.maNgonNgu === maNgonNgu);

        if (!tenMon && !moTa && !nguyenLieu && !traiNghiem) {
            // Nếu tất cả đều trống → xóa bản dịch
            if (index !== -1) {
                amThucTranslations.splice(index, 1);
            }
        } else {
            if (index !== -1) {
                // cập nhật bản dịch đã có
                amThucTranslations[index].tenMon = tenMon;
                amThucTranslations[index].nguyenLieu = nguyenLieu;
                amThucTranslations[index].traiNghiem = traiNghiem;
                amThucTranslations[index].moTa = moTa;
            } else {
                // thêm mới
                amThucTranslations.push({
                    maNgonNgu: maNgonNgu,   
                    tenMon: tenMon,
                    nguyenLieu: nguyenLieu,
                    traiNghiem: traiNghiem,
                    moTa: moTa
                });
            }
        }
    }

    // Sự kiện blur trên input/textarea → tự động lưu
    $("#tenMon, #traiNghiem, #nguyenLieu, #moTa")
        .on("blur", function () {
            saveTempAmThucTranslation();
        });

    // Sự kiện đổi ngôn ngữ → bind lại dữ liệu đã lưu
    $("#ngonNgu").on("change", function () {
        let maNgonNgu = $(this).val();
        let exist = amThucTranslations.find(t => t.maNgonNgu === maNgonNgu);

        if (exist) {
            $("#tenMon").val(exist.tenMon);
            $("#traiNghiem").val(exist.traiNghiem);
            $("#nguyenLieu").val(exist.nguyenLieu);
            $("#moTa").val(exist.moTa);
        } else {
            // reset nếu chưa có dữ liệu
            $("#tenMon").val("");
            $("#traiNghiem").val("");
            $("#nguyenLieu").val("");
            $("#moTa").val("");
        }
    });


    $("#btn-save").on("click", function (e) {
        saveTempAmThucTranslation();
        amThucTranslations = amThucTranslations.filter(t => t.tenMon && t.tenMon.trim() !== "");
        dataAddAmThuc();
        e.preventDefault();
    });
    $("#btn-save-and-close").on("click", async function (e) {
        saveTempAmThucTranslation();
        amThucTranslations = amThucTranslations.filter(t => t.tenMon && t.tenMon.trim() !== "");
        // await dataAddAmThuc();
        e.preventDefault();

        setTimeout(function () {
            window.location.href = '/AdminTool/AmThuc';
        }, 2000);

    });

    async function dataAddAmThuc() {
        // Lấy file ảnh và url (nếu có)
        let anhDaiDien = $('#anhDaiDien')[0]?.files[0] || null;
        let urlAnhDaiDien = $('#urlAnhDaiDien').val() || null;

        // Lấy dữ liệu từ input/select
        let kieuMonID = $('#kieuMonID').val() || null;
        let cheDoAnID = $('#cheDoAnID').val() || null;
        let thuTu = $('#thuTu').val() || 1;
        let trangThai = $("input[name='suDung']:checked").val();
        let dacSan = $("input[name='dacSan']:checked").val();

        // Lấy bản dịch tiếng Việt (bắt buộc phải có)
        let tenTiengViet = amThucTranslations.find(el => el.maNgonNgu === 'vi')?.tenMon || null;

        // Validate các trường bắt buộc
        if ((anhDaiDien == null && checkEmptyBlank(urlAnhDaiDien)) || !tenTiengViet || !dacSan || !kieuMonID || !cheDoAnID) {
            showNotification(0, 'Vui lòng nhập đầy đủ thông tin bắt buộc (*), bao gồm bản dịch tiếng Việt');
            return null;
        }

        let amThuc = {
            monAnUongID: monAnUongID || null,
            kieuMonID: kieuMonID,
            cheDoAnID: cheDoAnID,
            ThuTu: thuTu != "" ? Number(thuTu) : 1,
            dacSan: dacSan == '1' ? true : false,
            SuDung: trangThai == '1' ? true : false,
            AnhDaiDien: urlAnhDaiDien
        };

        // Chuẩn bị formData để gửi lên server
        let formData = new FormData();
        formData.append("AmThuc", JSON.stringify(amThuc));
        formData.append("AmThuc_NoiDung", JSON.stringify(amThucTranslations)); 
        if (anhDaiDien) {
            formData.append("AnhDaiDien", anhDaiDien);
        }

        if (checkEmptyBlank(monAnUongID)) {
            try {
                const res = await fetch('/api/AmThucApi/ThemMoi', {
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
                    monAnUongID = data.value.monAnUongID
                    initThongTinChung();
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
                const res = await fetch(`/api/AmThucApi/ChinhSua/${monAnUongID}`, {
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
                    monAnUongID = data.value.monAnUongID
                    initThongTinChung();
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

    function saveTempTranslationDPT() {
        let maNgonNgu = $("#ngonNguDPT").val();
        let tieuDe = $("#tieuDeDPT").val().trim();
        let moTa = $("#moTaDPT").val().trim();

        if (!maNgonNgu) return; // bắt buộc chọn ngôn ngữ và nhập tên món

        // Kiểm tra ngôn ngữ đã tồn tại trong mảng chưa
        let index = daPhuongTientranslations.findIndex(t => t.maNgonNgu === maNgonNgu);
        if (tieuDe === "" && moTa === "") {
            // Nếu trống cả tiêu đề và mô tả -> xóa bản dịch khỏi mảng
            if (index !== -1) {
                daPhuongTientranslations.splice(index, 1);
            }
        } else {
            if (index !== -1) {
                daPhuongTientranslations[index].tieuDe = tieuDe;
                daPhuongTientranslations[index].moTa = moTa;
            } else {
                daPhuongTientranslations.push({
                    maNgonNgu: maNgonNgu,
                    tieuDe: tieuDe,
                    moTa: moTa
                });
            }
        }
    }

    // Sự kiện blur trên input/textarea → tự động lưu
    $("#tieuDeDPT, #moTaDPT")
        .on("blur", function () {
            saveTempTranslationDPT();
        });

    // Sự kiện đổi ngôn ngữ → bind lại dữ liệu đã lưu
    $("#ngonNguDPT").on("change", function () {
        let maNgonNgu = $(this).val();
        let exist = daPhuongTientranslations.find(t => t.maNgonNgu === maNgonNgu);

        if (exist) {
            $("#tieuDeDPT").val(exist.tieuDe);
            $("#moTaDPT").val(exist.moTa);
        } else {
            // reset nếu chưa có dữ liệu
            $("#tieuDeDPT").val("");
            $("#moTaDPT").val("");
        }
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
                    DoiTuongSoHuuID: monAnUongID || guidEmpty,
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
                $("#showFileDPT").text(data.duongDanFile);
                $("#duongDanFileDPT").val(data.duongDanFile);
            } else {
                $("#showFileDPT").text("");
            }
            $("#tacGiaDPT").val(data.tacGia);
            $("#thuTuDPT").val(data.thuTuHienThi);

            // Radio trạng thái
            $(`input[name='trangThaiAdd'][value='${data.suDung == true ? "1" : "0"}'`).prop('checked', true);

            daPhuongTientranslations = []
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
                daPhuongTientranslations = data.value
            } else {
                console.log(data.error)
            }

            var banDich_vi = daPhuongTientranslations.find(el => el.maNgonNgu === 'vi')
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
        $('#showFileDPT').text("");
        $('#duongDanFile').val("");
    });

    async function dataAddDaPhuongTien() {
        if (monAnUongID == null || monAnUongID == guidEmpty) {
            showNotification(0, 'Vui lòng lưu thông tin món ăn trước khi thêm đa phương tiện')
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
            DoiTuongSoHuuID: monAnUongID,
            LoaiMedia: Number(loaiMedia),
            LoaiDoiTuong: "VH_AmThuc",
            DuongDanFile: duongDanFile,
            TacGia: tacGia,
            ThuTuHienThi: thuTu || 1,
            SuDung: trangThai == "1" ? true : false
        };

        let formData = new FormData();
        formData.append("DaPhuongTien", JSON.stringify(thongTinChung));
        formData.append("DaPhuongTien_NoiDung", JSON.stringify(daPhuongTientranslations));

        if (fileDPT) {
            formData.append("File", fileDPT);
        }

        if (daPhuongTienID == null || daPhuongTienID == guidEmpty) {
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
                    daPhuongTientranslations = []
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
                    daPhuongTientranslations = []

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
        daPhuongTientranslations = []
        $(this).find('input[type=text], input[type=number], textarea').val('');
        $(this).find('select').val('vi').trigger('change');
        $(this).find('input[type=radio][value=1], input[type=checkbox][value=1]').prop('checked', true);
        $('#showFileDPT').text('')
        $('#fileDPT').val(null)
    });
    //#endregion ĐA PHƯƠNG TIỆN

    //#region VĂN BẢN TÀI LIỆU
    async function initTableVanBanTaiLieu() {
        const tableApi = {
            url: `${baseUrl}/api/VanBanTaiLieuApi/DanhSach`,
            type: "POST",
            data: function (d) {
                return JSON.stringify({
                    doiTuongSoHuuID: monAnUongID || guidEmpty,
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
            $('#idTypeDelete').val(2);

            $('#modalDelete').modal('show');
        });
    }

    $('#btnSaveVBTL').on('click', function () {
        dataAddVanBanTaiLieu()
    });
    async function dataAddVanBanTaiLieu() {
        if (monAnUongID == null || monAnUongID == guidEmpty) {
            showNotification(0, 'Vui lòng lưu thông tin chung trước khi thêm văn bản tài liệu')
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

        if ((fileVBTL == null && checkEmptyBlank(duongDanFileVBTL)) || checkEmptyBlank(soKyHieuVBTL) || checkEmptyBlank(trichYeuVBTL)
            || checkEmptyBlank(ngayBanHanhVBTL) || checkEmptyBlank(ngayHieuLucVBTL) || checkEmptyBlank(coQuanBanHanhVBTL)) {
            showNotification(0, 'Vui lòng nhập đầy đủ thông tin bắt buộc (*), bắt buộc có bản dịch tiếng Việt')
            return;
        }
        let formData = new FormData()

        formData.append("vanBanID", idVBTL);
        formData.append("doiTuongSoHuuID", diSanID);
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

    async function initKieuMonAmThuc() {
        try {
            const res = await fetch('/api/DanhMucChungApi/DanhSach', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    //DanhMucID nào thì đổi sau
                    loaiDanhMucID: 16,
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
                    $("#kieuMonID").append(`<option value="${el.danhMucID}">${el.tenDanhMuc}</option>`);
                });
            } else {
                showNotification(0, data.error)
            }
        }
        catch (err) {
            console.log(err.message)
        }
    };

    async function initCheDoAnAmThuc() {
        try {
            const res = await fetch('/api/DanhMucChungApi/DanhSach', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    //DanhMucID nào thì đổi sau
                    loaiDanhMucID: 16,
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
                    $("#cheDoAnID").append(`<option value="${el.danhMucID}">${el.tenDanhMuc}</option>`);
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
        if (checkEmptyBlank(monAnUongID)) {
            $("#thong-tin-da-phuong-tien-tab").closest("li").hide();
            $("#van-ban-tai-lieu-tab").closest("li").hide();
        } else {
            $("#thong-tin-da-phuong-tien-tab").closest("li").show();
            $("#van-ban-tai-lieu-tab").closest("li").show();
        }
    }
    //#endregion HÀM LOAD DỮ LIỆU CHO CÁC COMBOBOX

})