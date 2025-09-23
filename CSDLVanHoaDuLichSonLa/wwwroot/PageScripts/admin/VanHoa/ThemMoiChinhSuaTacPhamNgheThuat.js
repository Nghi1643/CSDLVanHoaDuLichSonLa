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
    let tacPhamTranslations = [];
    let daPhuongTientranslations = [];
    ; (async function () {
        await Promise.all([
            initNgonNgu(),
            initLoaiHinhTacPham(),
        ]);
        initThongTinChung();
        //initTableDaPhuongTien();
        reloadAccess()
    })();

    $("#formDelete").on("submit", function (e) {
        e.preventDefault();
        let type = $('#idTypeDelete').val();
        if (type == "1") {
            dataDeleteDaPhuongTien();
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
        if (checkEmptyBlank(tacPhamID)) {
            return;
        }

        try {
            const res = await fetch(`/api/TacPhamApi/ChiTiet/${tacPhamID}`, {
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

                if (data.value.hinhAnh && data.value.hinhAnh.trim() !== "") {
                    $("#previewAnhDaiDien").attr("src", data.value.hinhAnh.trim());
                    $("#urlAnhDaiDien").val(data.value.hinhAnh.trim());
                }

                // Loại hình tác phẩm
                $("#theLoaiID").val(data.value.theLoaiID).trigger("change");

                // Mã định danh
                $("#maDinhDanh").val((data.value.maDinhDanh || "").trim());

                // Năm công bố
                $("#namCongBo").val((data.value.namCongBo || "").toString().trim());

                // Thứ tự
                $("#thuTu").val(data.value.thuTu || 1);

                // Trạng thái duyệt (radio)
                if (data.value.suDung == 1) {
                    $("#suDung").prop("checked", true);
                } else {
                    $("#khongSuDung").prop("checked", true);
                }

                // Bản dịch
                tacPhamTranslations = data.value.banDich || [];
                let tiengViet = tacPhamTranslations.find(el => el.maNgonNgu === "vi") || null;
                if (tiengViet) {
                    $("#ngonNgu").val("vi").trigger("change");
                    $("#tenTacPham").val((tiengViet.tenTacPham || "").trim());
                    $("#moTa").val((tiengViet.moTa || "").trim());
                    $("#noiDung").val((tiengViet.noiDung || "").trim());
                    $("#tacGia").val((tiengViet.tacGia || "").trim());
                }
            } else {
                showNotification(0, data.error);
            }

        }
        catch (err) {
            showNotification(0, err.message)
        }
    }

    // Hàm lưu tạm cho 1 ngôn ngữ
    // Hàm lưu tạm cho 1 ngôn ngữ (TacPhamNgheThuat + tác giả)
    function saveTempTacPhamTranslation() {
        let maNgonNgu = $("#ngonNgu").val();
        if (!maNgonNgu) return; // bắt buộc chọn ngôn ngữ

        let tenTacPham = $("#tenTacPham").val().trim();
        let moTa = $("#gioiThieu").val().trim();
        let noiDung = $("#noiDung").val().trim();
        let tacGia = $("#tacGia").val().trim(); // 👈 thêm tác giả

        // tìm index ngôn ngữ trong mảng
        let index = tacPhamTranslations.findIndex(t => t.maNgonNgu === maNgonNgu);

        // nếu tất cả đều trống → xóa bản dịch
        if (!tenTacPham && !moTa && !noiDung && !tacGia) {
            if (index !== -1) {
                tacPhamTranslations.splice(index, 1);
            }
        } else {
            if (index !== -1) {
                // cập nhật
                tacPhamTranslations[index].tenTacPham = tenTacPham;
                tacPhamTranslations[index].moTa = moTa;
                tacPhamTranslations[index].noiDung = noiDung;
                tacPhamTranslations[index].tacGia = tacGia;
            } else {
                // thêm mới
                tacPhamTranslations.push({
                    maNgonNgu: maNgonNgu,
                    tenTacPham: tenTacPham,
                    moTa: moTa,
                    noiDung: noiDung,
                    tacGia: tacGia
                });
            }
        }
    }

    // Sự kiện blur trên input/textarea → tự động lưu
    $("#tenTacPham, #moTa, #noiDung, #tacGia")
        .on("blur", function () {
            saveTempTacPhamTranslation();
        });

    // Sự kiện đổi ngôn ngữ → bind lại dữ liệu đã lưu
    $("#ngonNgu").on("change", function () {
        let maNgonNgu = $(this).val();
        let exist = tacPhamTranslations.find(t => t.maNgonNgu === maNgonNgu);

        if (exist) {
            $("#tenTacPham").val(exist.tenTacPham || "");
            $("#gioiThieu").val(exist.moTa || "");
            $("#noiDung").val(exist.noiDung || "");
            $("#tacGia").val(exist.tacGia || "");
        } else {
            // reset nếu chưa có dữ liệu
            $("#tenTacPham").val("");
            $("#gioiThieu").val("");
            $("#noiDung").val("");
            $("#tacGia").val("");
        }
    });

    $("#btn-save").on("click", function (e) {
        saveTempTacPhamTranslation();
        tacPhamTranslations = tacPhamTranslations.filter(t => t.tenTacPham && t.tenTacPham.trim() !== ""); // loại bỏ các bản dịch không có tên hiện vật)
        dataAddThongTinChung();
        e.preventDefault();
    });

    $("#btn-save-and-close").on("click", function (e) {
        saveTempTacPhamTranslation();
        tacPhamTranslations = tacPhamTranslations.filter(t => t.tenTacPham && t.tenTacPham.trim() !== ""); // loại bỏ các bản dịch không có tên hiện vật)
        dataAddThongTinChung();
        e.preventDefault();

        setTimeout(function () {
            window.location.href = '/AdminTool/TacPhamNgheThuat';
        }, 2000)
    });

    async function dataAddThongTinChung() {
        // Lấy file ảnh và url (nếu có)
        let anhDaiDien = $('#anhDaiDien')[0]?.files[0] || null;
        let urlAnhDaiDien = $('#urlAnhDaiDien').val() || null;

        // Lấy dữ liệu từ input/select
        let theLoaiID = $('#theLoaiID').val() || null;
        let maDinhDanh = $('#maDinhDanh').val()?.trim() || null;
        let namCongBo = $('#namCongBo').val()?.trim() || null;
        let suDung = $('input[name="trangThai"]:checked').val() === "1";
        let thuTu = $('#thuTu').val() || 1;

        // Lấy bản dịch tiếng Việt (bắt buộc phải có)
        let tenTiengViet = tacPhamTranslations.find(el => el.maNgonNgu === 'vi')?.tenTacPham || null;

        // Validate các trường bắt buộc
        if ((anhDaiDien == null && checkEmptyBlank(urlAnhDaiDien)) || !tenTiengViet || !maDinhDanh || !theLoaiID) {
            showNotification(0, 'Vui lòng nhập đầy đủ thông tin bắt buộc (*), bao gồm bản dịch tiếng Việt');
            return null;
        }

        // Gom object thông tin chung
        let hienVat = {
            TacPhamID: tacPhamID || null,
            TheLoaiID: theLoaiID,
            linhVucID: 2,
            MaDinhDanh: maDinhDanh,
            NamCongBo: namCongBo,
            ThuTu: thuTu != "" ? Number(thuTu) : 1,
            SuDung: suDung == '1' ? true : false,
            HinhAnh: urlAnhDaiDien
        };

        // Chuẩn bị formData để gửi lên server
        let formData = new FormData();
        formData.append("TacPham", JSON.stringify(hienVat));
        formData.append("TacPham_NoiDung", JSON.stringify(tacPhamTranslations)); // mảng bản dịch
        if (anhDaiDien) {
            formData.append("HinhAnh", anhDaiDien);
        }

        if (checkEmptyBlank(tacPhamID)) {
            try {
                const res = await fetch('/api/TacPhamApi/ThemMoi', {
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
                    tacPhamTranslations = []
                    tacPhamID = data.value.tacPhamID
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
                const res = await fetch(`/api/TacPhamApi/ChinhSua/${tacPhamID}`, {
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
                    tacPhamID = data.value.tacPhamID
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


    //#region BIỂU DIỄN
    $('#btn-show-bieu-dien').on('click', function () {
        $('#formBieuDien').removeClass("d-none");
        $(this).addClass("d-none");
    })
    $('#btn-cancel-bieu-dien').on('click', function () {
        $('#formBieuDien').addClass("d-none");
        $('#btn-show-bieu-dien').removeClass("d-none")
    })

    // Hàm lưu tạm cho 1 ngôn ngữ
    function saveTempTranslationDPT() {
        let maNgonNgu = $("#ngonNguDPT").val();
        let tieuDe = $("#tieuDeDPT").val().trim();
        let moTa = $("#moTaDPT").val().trim();

        if (!maNgonNgu || !tieuDe) return; // bắt buộc chọn ngôn ngữ và nhập tên di tích

        // Kiểm tra ngôn ngữ đã tồn tại trong mảng chưa
        let exist = daPhuongTientranslations.find(t => t.maNgonNgu === maNgonNgu);
        if (exist) {
            exist.tieuDe = tieuDe;
            exist.moTa = moTa;
        } else {
            daPhuongTientranslations.push({
                maNgonNgu: maNgonNgu,
                tieuDe: tieuDe,
                moTa: moTa
            });
        }
        console.log("Lưu tạm:", daPhuongTientranslations);
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
        console.log("Sau khi đổi ngôn ngữ:", daPhuongTientranslations);
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
                    DoiTuongSoHuuID: tacPhamID || guidEmpty,
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
                    if (permitedEdit) {
                        html += `<i data-toggle="tooltip" title="Chỉnh sửa" class="edit-command-btn text-yellow" id=n-"${meta.row}">
                                   <i class="hgi-icon hgi-edit"></i>
                                </i>`;
                    }
                    if (permitedDelete) {
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
        $('#showFileEdit').text("");
        $('#duongDanFile').val("");
    });
    async function dataAddDaPhuongTien() {
        if (tacPhamID == null || tacPhamID == guidEmpty) {
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
            DoiTuongSoHuuID: tacPhamID,
            LoaiMedia: Number(loaiMedia),
            LoaiDoiTuong: "VH_HienVat",
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
        $('#showFileEdit').text('')
        $('#fileDPT').val(null)
    });
    //#endregion ĐA PHƯƠNG TIỆN


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
                });
                $("#ngonNgu").val('vi').trigger('change');
            } else {
                showNotification(0, data.error)
            }
        }
        catch (err) {
            showNotification(0, err.message)
        }
    };

    async function initLoaiHinhTacPham() {
        try {
            const res = await fetch('/api/DanhMucChungApi/DanhSach', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    loaiDanhMucID: 20,
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
                    $("#theLoaiID").append(`<option value="${el.danhMucID}">${el.tenDanhMuc}</option>`);
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
        if (checkEmptyBlank(tacPhamID)) {
            $("#bieu-dien-tab").closest("li").hide();
        } else {
            $("#bieu-dien-tab").closest("li").show();
        }
    }
    //#endregion HÀM LOAD DỮ LIỆU CHO CÁC COMBOBOX
    
})