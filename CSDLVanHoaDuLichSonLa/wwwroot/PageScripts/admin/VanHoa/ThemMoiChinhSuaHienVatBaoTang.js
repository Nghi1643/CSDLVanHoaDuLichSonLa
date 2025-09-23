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
    let hienVatTranslations = [];
    let daPhuongTientranslations = [];
    ; (async function () {
        await Promise.all([
            initNgonNgu(),
            initChatLieu(),
            initBaoTang(),
            initLoaiHienVat()
        ]);
        initThongTinChung();
        initTableDaPhuongTien();
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
        if (checkEmptyBlank(hienVatId)) {
            return;
        }

        try {
            const res = await fetch(`/api/HienVatApi/ChiTiet/${hienVatId}`, {
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
                    $("#urlAnhDaiDien").val(v.anhDaiDien);
                }

                // Mã hiện vật
                $("#maHienVat").val(v.maHienVat || "");

                // Lĩnh vực
                $("#linhVucID").val(v.linhVucID).trigger("change");

                // Chất liệu
                $("#chatLieuID").val(v.chatLieuID).trigger("change");

                // Phương thức tặng/mua
                $("#phuongThucTangMuaID").val(v.phuongThucTangMuaID).trigger("change");

                // Bảo tàng
                $("#baoTangID").val(v.baoTangID).trigger("change");

                // Trạng thái (select)
                $("#trangThaiID").val(v.trangThaiID).trigger("change");

                // Thứ tự
                $("#thuTu").val(v.thuTu || 1);

                // Trạng thái duyệt (radio)
                if (v.suDung == 1) {
                    $("#trangThaiDuyet").prop("checked", true);
                } else {
                    $("#trangThaiChuaDuyet").prop("checked", true);
                }

                // Bản dịch
                hienVatTranslations = v.banDich || [];

                let tiengViet = hienVatTranslations.find(el => el.maNgonNgu === "vi") || null;
                if (tiengViet != null) {
                    $("#ngonNgu").val("vi").trigger("change");
                    $("#tenHienVat").val(tiengViet.tenHienVat || "");
                    $("#tenGoiKhac").val(tiengViet.tenGoiKhac || "");
                    $("#soLuong").val(tiengViet.soLuong || "");
                    $("#mauSac").val(tiengViet.mauSac || "");
                    $("#kichThuoc").val(tiengViet.kichThuoc || "");
                    $("#nienDai").val(tiengViet.nienDai || "");
                    $("#nguonGoc").val(tiengViet.nguonGoc || "");
                    $("#khaoTa").val(tiengViet.khaoTa || "");
                    $("#tinhTrangSuuTam").val(tiengViet.tinhTrangSuuTam || "");
                    $("#hoanCanhSuuTam").val(tiengViet.hoanCanhSuuTam || "");
                    $("#chuHienVat").val(tiengViet.chuHienVat || "");
                    $("#nguoiSuuTam").val(tiengViet.nguoiSuuTam || "");
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
    function saveTempHienVatTranslation() {
        let maNgonNgu = $("#ngonNgu").val();
        if (!maNgonNgu) return; // bắt buộc chọn ngôn ngữ

        let tenHienVat = $("#tenHienVat").val().trim();
        let tenGoiKhac = $("#tenGoiKhac").val().trim();
        let soLuong = $("#soLuong").val().trim();
        let mauSac = $("#mauSac").val().trim();
        let kichThuoc = $("#kichThuoc").val().trim();
        let nienDai = $("#nienDai").val().trim();
        let nguonGoc = $("#nguonGoc").val().trim();
        let khaoTa = $("#khaoTa").val().trim();
        let tinhTrangSuuTam = $("#tinhTrangSuuTam").val().trim();
        let hoanCanhSuuTam = $("#hoanCanhSuuTam").val().trim();
        let chuHienVat = $("#chuHienVat").val().trim();
        let nguoiSuuTam = $("#nguoiSuuTam").val().trim();

        // tìm index ngôn ngữ trong mảng
        let index = hienVatTranslations.findIndex(t => t.maNgonNgu === maNgonNgu);

        // nếu tất cả đều trống → xóa bản dịch
        if (
            !tenHienVat && !tenGoiKhac && !soLuong && !mauSac && !kichThuoc &&
            !nienDai && !nguonGoc && !khaoTa && !tinhTrangSuuTam &&
            !hoanCanhSuuTam && !chuHienVat && !nguoiSuuTam
        ) {
            if (index !== -1) {
                hienVatTranslations.splice(index, 1);
            }
        } else {
            if (index !== -1) {
                // cập nhật
                hienVatTranslations[index].tenHienVat = tenHienVat;
                hienVatTranslations[index].tenGoiKhac = tenGoiKhac;
                hienVatTranslations[index].soLuong = soLuong;
                hienVatTranslations[index].mauSac = mauSac;
                hienVatTranslations[index].kichThuoc = kichThuoc;
                hienVatTranslations[index].nienDai = nienDai;
                hienVatTranslations[index].nguonGoc = nguonGoc;
                hienVatTranslations[index].khaoTa = khaoTa;
                hienVatTranslations[index].tinhTrangSuuTam = tinhTrangSuuTam;
                hienVatTranslations[index].hoanCanhSuuTam = hoanCanhSuuTam;
                hienVatTranslations[index].chuHienVat = chuHienVat;
                hienVatTranslations[index].nguoiSuuTam = nguoiSuuTam;
            } else {
                // thêm mới
                hienVatTranslations.push({
                    maNgonNgu: maNgonNgu,
                    tenHienVat: tenHienVat,
                    tenGoiKhac: tenGoiKhac,
                    soLuong: soLuong,
                    mauSac: mauSac,
                    kichThuoc: kichThuoc,
                    nienDai: nienDai,
                    nguonGoc: nguonGoc,
                    khaoTa: khaoTa,
                    tinhTrangSuuTam: tinhTrangSuuTam,
                    hoanCanhSuuTam: hoanCanhSuuTam,
                    chuHienVat: chuHienVat,
                    nguoiSuuTam: nguoiSuuTam
                });
            }
        }
    }

    // Sự kiện blur trên input/textarea → tự động lưu
    $("#tenHienVat, #tenGoiKhac, #soLuong, #mauSac, #kichThuoc, #nienDai, #nguonGoc, #khaoTa, #tinhTrangSuuTam, #hoanCanhSuuTam, #chuHienVat, #nguoiSuuTam")
        .on("blur", function () {
            saveTempHienVatTranslation();
        });

    // Sự kiện đổi ngôn ngữ → bind lại dữ liệu đã lưu
    $("#ngonNgu").on("change", function () {
        let maNgonNgu = $(this).val();
        let exist = hienVatTranslations.find(t => t.maNgonNgu === maNgonNgu);

        if (exist) {
            $("#tenHienVat").val(exist.tenHienVat);
            $("#tenGoiKhac").val(exist.tenGoiKhac);
            $("#soLuong").val(exist.soLuong);
            $("#mauSac").val(exist.mauSac);
            $("#kichThuoc").val(exist.kichThuoc);
            $("#nienDai").val(exist.nienDai);
            $("#nguonGoc").val(exist.nguonGoc);
            $("#khaoTa").val(exist.khaoTa);
            $("#tinhTrangSuuTam").val(exist.tinhTrangSuuTam);
            $("#hoanCanhSuuTam").val(exist.hoanCanhSuuTam);
            $("#chuHienVat").val(exist.chuHienVat);
            $("#nguoiSuuTam").val(exist.nguoiSuuTam);
        } else {
            // reset nếu chưa có dữ liệu
            $("#tenHienVat").val("");
            $("#tenGoiKhac").val("");
            $("#soLuong").val("");
            $("#mauSac").val("");
            $("#kichThuoc").val("");
            $("#nienDai").val("");
            $("#nguonGoc").val("");
            $("#khaoTa").val("");
            $("#tinhTrangSuuTam").val("");
            $("#hoanCanhSuuTam").val("");
            $("#chuHienVat").val("");
            $("#nguoiSuuTam").val("");
        }
    });


    $("#btn-save").on("click", function (e) {
        saveTempHienVatTranslation();
        hienVatTranslations = hienVatTranslations.filter(t => t.tenHienVat && t.tenHienVat.trim() !== ""); // loại bỏ các bản dịch không có tên hiện vật)
        dataAddHienVat();
        e.preventDefault();
    });

    $("#btn-save-and-close").on("click", function (e) {
        saveTempHienVatTranslation();
        hienVatTranslations = hienVatTranslations.filter(t => t.tenHienVat && t.tenHienVat.trim() !== ""); // loại bỏ các bản dịch không có tên hiện vật)
        dataAddHienVat();
        e.preventDefault();

        setTimeout(function () {
            window.location.href = '/AdminTool/DiSanVanHoaVatThe';
        }, 2000)
    });
   
    async function dataAddHienVat() {
        // Lấy file ảnh và url (nếu có)
        let anhDaiDien = $('#anhDaiDien')[0]?.files[0] || null;
        let urlAnhDaiDien = $('#urlAnhDaiDien').val() || null;

        // Lấy dữ liệu từ input/select
        let loaiHienVatID = $('#loaiHienVatID').val() || null;
        let maHienVat = $('#maHienVat').val()?.trim() || null;
        let chatLieuID = $('#chatLieuID').val() || null;
        let phuongThucTangMuaID = $('#phuongThucTangMuaID').val() || null;
        let baoTangID = $('#baoTangID').val() || null;
        let trangThaiID = $('#trangThaiID').val() || null;
        let thuTu = $('#thuTu').val() || 1;
        let trangThai = $("input[name='trangThai']:checked").val();

        // Lấy bản dịch tiếng Việt (bắt buộc phải có)
        let tenTiengViet = hienVatTranslations.find(el => el.maNgonNgu === 'vi')?.tenHienVat || null;

        // Validate các trường bắt buộc
        if ((anhDaiDien == null && checkEmptyBlank(urlAnhDaiDien)) || !tenTiengViet || !maHienVat || !chatLieuID || !loaiHienVatID) {
            showNotification(0, 'Vui lòng nhập đầy đủ thông tin bắt buộc (*), bao gồm bản dịch tiếng Việt');
            return null;
        }

        // Gom object thông tin chung
        let hienVat = {
            LoaiHienVatID: loaiHienVatID,
            MaHienVat: maHienVat,
            ChatLieuID: chatLieuID,
            PhuongThucTangMuaID: phuongThucTangMuaID,
            BaoTangID: baoTangID,
            TrangThaiID: trangThaiID,
            ThuTu: thuTu != "" ? Number(thuTu) : 1,
            SuDung: trangThai == '1' ? true : false,
            AnhDaiDien: urlAnhDaiDien
        };

        // Chuẩn bị formData để gửi lên server
        let formData = new FormData();
        formData.append("HienVat", JSON.stringify(hienVat));
        formData.append("HienVat_NoiDung", JSON.stringify(hienVatTranslations)); // mảng bản dịch
        if (anhDaiDien) {
            formData.append("AnhDaiDien", anhDaiDien);
        }

        if (checkEmptyBlank(hienVatId)) {
            try {
                const res = await fetch('/api/HienVatApi/ThemMoi', {
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
                    hienVatTranslations = []
                    hienVatId = data.value.hienVatID
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
                const res = await fetch(`/api/HienVatApi/ChinhSua/${hienVatId}`, {
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
                    hienVatId = data.value.hienVatID
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
                    DoiTuongSoHuuID: hienVatId || guidEmpty,
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
        if (hienVatId == null || hienVatId == guidEmpty) {
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
            DoiTuongSoHuuID: hienVatId,
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

    async function initChatLieu() {
        try {
            const res = await fetch('/api/DanhMucChungApi/DanhSach', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    loaiDanhMucID: 35,
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
                    $("#chatLieuID").append(`<option value="${el.danhMucID}">${el.tenDanhMuc}</option>`);
                });
            } else {
                showNotification(0, data.error)
            }
        }
        catch (err) {
            console.log(err.message)
        }
    };

    async function initBaoTang() {
        try {
            const res = await fetch('/api/ToChucApi/DanhSach', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    loaiToChucID: 95
                })
            })

            if (!res.ok) {
                var errText = await res.text();
                throw new Error(errText);
            }
            data = await res.json();


            if (data && data.isSuccess && data.value) {
                data.value.forEach(el => {
                    $("#baoTangID").append(`<option value="${el.toChucID}">${el.tenToChuc}</option>`);
                });
            } else {
                showNotification(0, data.error)
            }
        }
        catch (err) {
            console.log(err.message)
        }
    };

    async function initLoaiHienVat() {
        try {
            const res = await fetch('/api/DanhMucChungApi/DanhSach', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    loaiDanhMucID: 45,
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
                    $("#loaiHienVatID").append(`<option value="${el.danhMucID}">${el.tenDanhMuc}</option>`);
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
        if (checkEmptyBlank(hienVatId)) {
            $("#thong-tin-da-phuong-tien-tab").closest("li").hide();
        } else {
            $("#thong-tin-da-phuong-tien-tab").closest("li").show();
        }
    }
    //#endregion HÀM LOAD DỮ LIỆU CHO CÁC COMBOBOX
    
})