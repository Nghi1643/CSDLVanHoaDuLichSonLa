const baseUrl = getRootLink();
let translations = [];

console.log(coQuanId)

$(document).ready(function () {
    initDatePicker();
    initDanhMucChung_NoAll(24, "#loaiHinh", "")
    initDanhMucChung_NoAll(34, "#coQuanChuQuan", "")
    initDanhMucChung_NoAll(25, "#theLoaiAdd", "")
    initNgonNgu("#ngonNguDich")
    initNgonNgu("#ngonNguAdd")
    initTinhThanhPho()
    initSelect2();
    previewPicture()

    if (isEdit && coQuanId) {
        setTimeout(function() {
            loadCoQuanData(coQuanId)
            initAnPhamTable(coQuanId)
        }, 100);
    }

    // Event handler cho thay đổi tỉnh/thành phố
    $(document).on('change', '#tinhThanhPho', function() {
        const tinhID = $(this).val();
        if (tinhID) {
            initPhuongXa(tinhID);
        } else {
            $("#phuongXa").empty().append(`<option value="">Chọn</option>`);
        }
    });


    $("#btnSaveAdd").on("click", function () {
        dataEdit()
    })

    // Khi thay đổi ngôn ngữ dịch, reset các input trong phần đa ngữ
    $(document).on('change', '#ngonNguDich', function() {
        let maNgonNgu = $(this).val();
        const coQuanId = $('#coQuanId').val();
        
        if (coQuanId && maNgonNgu) {
            // Load bản dịch cho ngôn ngữ được chọn
            loadToChucTranslation(coQuanId, maNgonNgu);
        } else {
            // Nếu chưa có cơ quan (tạo mới) hoặc chưa chọn ngôn ngữ, clear các trường dịch
            if (!coQuanId) {
                $('#tenCoQuanDich').val('');
                $('#diaChiDich').val('');
                $('#gioiThieuDich').val('');
                $('#ghiChuDich').val('');
            }
        }
    });

    $("#formToChuc").on("submit", function (e) {
        e.preventDefault();
        
        const submitBtn = $(this).find('button[type="submit"]');
        if (submitBtn.prop('disabled')) {
            return;
        }
        
        submitBtn.prop('disabled', true);
        const originalText = submitBtn.find('#submitText').text();
        submitBtn.find('#submitText').text('Đang xử lý...');
        
        try {
            dataEdit();
        } catch (error) {
            console.error('Lỗi khi xử lý form:', error);
        } finally {
            setTimeout(() => {
                submitBtn.prop('disabled', false);
                submitBtn.find('#submitText').text(originalText);
            }, 1000);
        }
    });

    $("#formAdd").on("submit", function (e) {
        e.preventDefault();

        const submitBtn = $(this).find('button[type="submit"]');
        if (submitBtn.prop('disabled')) {
            return;
        }
        
        submitBtn.prop('disabled', true);
        const originalText = submitBtn.find('#submitText').text();
        submitBtn.find('#submitText').text('Đang xử lý...')

        try {
            dataAddAnPham(coQuanId);
            initAnPhamTable(coQuanId);
        } catch (error) {
            console.error('Lỗi khi xử lý form:', error);
        } finally {
            setTimeout(() => {
                submitBtn.prop('disabled', false);
                submitBtn.find('#submitText').text(originalText);
            }, 1000);
        }
    });

    // Event handler cho nút thêm ấn phẩm
    $('#btnThemAnPham').on('click', function() {
        // Reset form
        $(this).find('input[type=text], textarea').val('');
        $(this).find('select').val('vi').trigger('change');
        $(this).find('input[type=radio][value=1], input[type=checkbox][value=1]').prop('checked', true);
        $('#modalAdd').modal('show');
    });

    $('#tableAnPham tbody').on('click', '.edit-command-btn', function () {
        var id = $(this).attr("ID").match(/\d+/)[0];
        var data = $('#tableAnPham').DataTable().row(id).data();
        $('#idAnPhamAdd').val(data.baoChiAnPhamID);

        // Các input text
        $('#tanSuatAdd').val(data.tanSuatID).trigger('change');
        $('#linhVucChuyenSauAdd').val(data.linhVucChuyenSauID).trigger('change');
        // Radio trạng thái
        $(`input[name="trangThaiAdd"][value="${data.trangThai ? "1" : "0"}"]`).prop('checked', true);

        translations = [];
        getDataWithApi('GET', `/api/BaoChiAnPhamApi/BanDich/${data.baoChiAnPhamID}`).then(data => {
            if (data && data.isSuccess && data.value) {
                translations = data.value
                var banDich_vi = translations.find(el => el.maNgonNgu === 'vi')
                $('#ngonNguAdd').val('vi').trigger('change');
                $('#tenAnPhamAdd').val(banDich_vi.tenAnPham);
                $('#doiTuongDocGiaAdd').val(banDich_vi.doiTuongDocGia);
            } else {
                console.log(data.error)
        }
        })

        $('#modalAdd').modal('show');
    });
    
    $('#tableAnPham tbody').on('click', '.delete-command-btn', function () {
        var id = $(this).attr("ID").match(/\d+/)[0];
        var data = $('#tableAnPham').DataTable().row(id).data();

        $('#idDelete').val(data.baoChiAnPhamID);
        $('#nameDelete').text(`${data.tenAnPham}`);

        $('#modalDelete').modal('show');
    });

    $("#modalAdd").on('hidden.bs.modal', function () {
        translations = []
        $(this).find('input[type=text], textarea').val('');
        $(this).find('select').val('vi').trigger('change');
        $(this).find('input[type=radio][value=1], input[type=checkbox][value=1]').prop('checked', true);
    });
});

function initTinhThanhPho() {
    getDataWithApi('GET', '/api/CapTinhApi/Gets').then(data => {
        if (data && data.isSuccessed && data.resultObj) {
            $("#tinhThanhPho").empty().append(`<option value="">Chọn</option>`);
            data.resultObj.forEach(item => {
                const displayText = item.noiDung || item.tenTinh || 'Không rõ tên';
                $("#tinhThanhPho").append(`<option value="${item.tinhID}">${displayText}</option>`);
            });
        }
    })
}

function initPhuongXa(tinhID = null) {
    getDataWithApi('GET', `/api/CapXaApi/GetByMaTinh?MaNgonNgu=vi&MaTinh=${tinhID ? `${tinhID}` : ''}`).then(data => {
        $("#phuongXa").empty().append(`<option value="">Chọn</option>`);
        if (data && data.isSuccess && data.value) {
            data.value.forEach(item => {
                const displayText = item.noiDung || item.tenXa || 'Không rõ tên';
                $("#phuongXa").append(`<option value="${item.xaID}">${displayText}</option>`);
            });
        }
    })
}

function dataEdit() {
    //Thông tin chung
    let id = $('#coQuanId').val();
    let maDinhDanh = $('#maCoQuan').val();
    let loaiHinh = $('#loaiHinh').val();
    let quyMo = $('#quyMo').val();
    let dienThoai = $('#soDienThoai').val();
    let hopThu = $('#hopThu').val();
    let website = $('#website').val();
    let tinhID = $('#tinhThanhPho').val();
    let xaID = $('#phuongXa').val();
    let ngayThanhLap = $('#ngayThanhLap').val();
    let soGiayPhep = $('#soGiayPhep').val();
    let thuTu = $('#thuTu').val();
    let trangThai = $("input[name='trangThai']:checked").val();
    let toaDoX = $('#toaDoX').val();
    let toaDoY = $('#toaDoY').val();

    //Đa ngữ
    let ngonNguDich = $('#ngonNguDich').val();
    let tenCoQuan = $('#tenCoQuanDich').val();
    let diaChi = $('#diaChiDich').val();
    let gioiThieu = $('#gioiThieuDich').val();
    let ghiChu = $('#ghiChuDich').val();

    //Thông tin dư thừa
    let coQuanChuQuan = $('#coQuanChuQuan').val();
    let phamViHoatDong = $('#phamViHoatDong').val();

    let dt = {
        "loaiToChucID": 98, // ID cho nhà xuất bản
        "linhVucID": 5, // ID cho lĩnh vực văn hóa
        "maDinhDanh": maDinhDanh?.trim() || null,
        "trangThaiID": parseInt(trangThai) || 1,
        "hopThu": hopThu?.trim() || null,
        "dienThoai": dienThoai?.trim() || null,
        "website": website?.trim() || null,
        "ngayThanhLap": formatDateToSendWithoutTime(ngayThanhLap) || null,
        "soGiayPhepHoatDong": soGiayPhep?.trim() || null,
        "coQuanChuQuanID": parseInt(coQuanChuQuan) || 0,
        "phamViHoatDongID": parseInt(phamViHoatDong) || 0,
        "loaiHinhID": loaiHinh,
        "toaDoX": toaDoX?.trim() || null,
        "toaDoY": toaDoY?.trim() || null,
        "tinhID": tinhID,
        "xaID": xaID,
        "quyMo": quyMo?.trim() || null,
        "toChuc_NoiDungs": [
            {
                "id": 0,
                "maNgonNgu": ngonNguDich || "vi",
                "tenToChuc": tenCoQuan?.trim(),
                "diaChi": diaChi?.trim() || null,
                "gioiThieu": gioiThieu?.trim() || null,
                "ghiChu": ghiChu?.trim() || null
            }
        ]
    };

    $.ajax({
        type: 'PUT',
        url: `${baseUrl}/api/ToChucApi/ChinhSua/${id}`,
        async: false,
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(dt),
        success: function (data) {
            if (data.isSuccess && data.value) {
                showNotification(1, "Chỉnh sửa nhà xuất bản thành công");
                console.log(data)
                setTimeout(() => {
                    window.location.href = `${baseUrl}/AdminTool/XuatBan/NhaXuatBan`;
                }, 1500);
            } else {
                showNotification(0, "Chỉnh sửa nhà xuất bản không thành công");
                console.log(data.error)
            }
        },
        error: function (err) {
            console.log(err)
        }
    })
}

function loadCoQuanData(id) {
    console.log(id)
    let data = JSON.stringify({
        loaiToChucID: 98, // nhà xuất bản
        toChucID: id
    })

    getDataWithApi('POST', '/api/ToChucApi/DanhSach', data).then(data => {
        console.log(data)
        if (data && data.isSuccess && data.value) {
            const item = data.value[0];
            // Thông tin chung
            $('#pageTitle').text(`Chỉnh sửa ${item.tenToChuc}`);
            $('#coQuanId').val(item.toChucID);
            $('#maCoQuan').val(item.maDinhDanh);
            $('#loaiHinh').val(item.loaiHinhID).trigger('change');
            $('#quyMo').val(item.quyMo);
            $('#soDienThoai').val(item.dienThoai);
            $('#hopThu').val(item.hopThu);
            $('#website').val(item.website);
            $('#tinhThanhPho').val(item.tinhID).trigger('change');
            setTimeout(function() {
                $('#phuongXa').val(item.xaID).trigger('change');
            }, 100);
            $('#ngayThanhLap').val(formatDateWithoutTime(item.ngayThanhLap));
            $('#soGiayPhep').val(item.soGiayPhepHoatDong);
            $('#thuTu').val(item.thuTu || 1);
            $('#toaDoX').val(item.toaDoX)
            $('#toaDoY').val(item.toaDoY)
            $(`input[name="trangThai"][value="${item.trangThaiID}"]`).prop('checked', true);

            // Đa ngữ
            $('#tenCoQuanDich').val(item.tenToChuc);
            $('#diaChiDich').val(item.diaChi);
            $('#gioiThieuDich').val(item.gioiThieu);
            $('#ghiChuDich').val(item.ghiChu);

            // Thông tin phụ
            $('#coQuanChuQuan').val(item.coQuanChuQuanID).trigger('change');
            $('#phamViHoatDong').val(item.phamViHoatDongID).trigger('change');
        } else {
            showNotification(0, 'Không tìm thấy thông tin nhà xuất bản');
        }
    })
}

function loadToChucTranslation(toChucID, maNgonNgu) {
    getDataWithApi('GET', `/api/ToChucApi/BanDich/${toChucID}?maNgonNgu=${maNgonNgu}`).then(data => {
        if (data && data.isSuccess && data.value.length > 0) {
            const translation = data.value[0];
            // Populate các trường dịch với dữ liệu đa ngữ
            $('#tenCoQuanDich').val(translation.tenToChuc || '');
            $('#diaChiDich').val(translation.diaChi || '');
            $('#gioiThieuDich').val(translation.gioiThieu || '');
            $('#ghiChuDich').val(translation.ghiChu || '');
        } else {
            // Nếu không có bản dịch, clear các trường dịch
            $('#tenCoQuanDich').val('');
            $('#diaChiDich').val('');
            $('#gioiThieuDich').val('');
            $('#ghiChuDich').val('');
        }
    })
}

function initAnPhamTable(coQuanBaoChiID) {
    const tableApi = {
        url: `${baseUrl}/api/XuatBanAnPhamApi/DanhSach`,
        type: "POST",
        data: function (d) {
            return JSON.stringify({
                nhaXuatBanID: coQuanBaoChiID,
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
            targets: 1, // Cột tên cơ quan báo chí
            render: function (data, type, row, meta) {
                return `<div class="group-info have-image">
                    <div class="have-image">
                        <img src="${row.duongDanHinhAnhBia || '/assets/images/vector/no-image.png'}" alt="${row.tenAnPham || ''}" onerror="this.onerror=null;this.src='/assets/images/vector/no-image.png';" />
                    </div>
                    <div class="group-info">
                    <div class="info-main"><div>${row.tenAnPham || ''}</div></div>
                    <div class="info-sub">${row.maDinhDanhXBAP || ''}</div></div>
                </div>`;
            }
        },
        {
            targets: 3, // Năm xuất bản
            render: function (data, type, row, meta) {
                return data || '-';
            }
        },
        {
            targets: 5, // Giá bìa
            render: function (data, type, row, meta) {
                if (data && data > 0) {
                    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(data);
                }
                return '-';
            }
        },
        {
            targets: 6, // Cột trạng thái
            render: function (data, type, row, meta) {
                if (row.trangThai) {
                    return `<i class="hgi-icon hgi-check text-green"></i>`;
                } else {
                    return `<i class="hgi-icon hgi-cancel text-red"></i>`;
                }
            }
        },
        {
            targets: 7, // Cột chức năng
            render: function (data, type, row, meta) {
                const editIcon = `<i data-toggle="tooltip" title="Chỉnh sửa" class="edit-command-btn text-yellow cursor-pointer me-2" data-index="${meta.row}">
                    <i class="hgi-icon hgi-edit"></i>
                </i>`;
                
                const deleteIcon = `<i data-toggle="tooltip" title="Xóa khỏi danh sách" class="delete-command-btn text-red cursor-pointer" data-index="${meta.row}">
                        <i class="hgi-icon hgi-delete"></i>
                    </i>`;
                
                return editIcon + deleteIcon;;
            }
        }
    ];

    const tableCols = [
        { "data": "stt", "width": "40px", "class": "center-align" },
        { "data": "tenAnPham", "width": "25%", "class": "left-align" },
        { "data": "tenTheLoai", "width": "120px", "class": "left-align" },
        { "data": "namXuatBan", "width": "100px", "class": "center-align" },
        { "data": "", "width": "100px", "class": "center-align", "defaultContent": "-" },
        { "data": "giaBia", "width": "100px", "class": "left-align" },
        { "data": "trangThai", "width": "120px", "class": "center-align" },
        { "data": "", "width": "80px", "class": "center-align group-icon-action" }
    ];

    initDataTableConfigNoSearch('tableAnPham', tableApi, tableDefs, tableCols);
}

function dataAddAnPham(coQuanBaoChiID) {
    let idAnPham = $('#idAnPhamAdd').val();
    let anhBia = $('#anhBiaAdd')[0]?.files[0] || null;
    let maDinhDanh = $('#maDinhDanhAdd').val()?.trim() || null;
    let maQuocTe = $('#maQuocTeAdd').val()?.trim() || null;
    let namXuatBan = $('#namXuatBanAdd').val();
    let soLanTaiBan = $('#soLanTaiBanAdd').val();
    let theLoai = $('#theLoaiAdd').val() || null;
    let soLuongIn = $('#soLuongInAdd').val();
    let giaBia = $('#giaBiaAdd').val();
    let trangThai = $("input[name='trangThaiAdd']:checked").val();

    let maNgonNgu = $("#ngonNguAdd").val();
    let tenAnPham = $("#tenAnPhamAdd").val().trim();
    let tacGia = $("#tacGiaAdd").val().trim();
    let moTa = $("#moTaAdd").val().trim();

    let thongTinChung = {
        "XuatBanAnPhamID": idAnPham || null,
        "MaDinhDanhXBAP": maDinhDanh,
        "MaQuocTe": maQuocTe,
        "NhaXuatBanID": coQuanBaoChiID,
        "NamXuatBan": namXuatBan !== "" ? Number(namXuatBan) : null,
        "SoLanTaiBan": soLanTaiBan !== "" ? Number(soLanTaiBan) : 0,
        "TheLoaiID": theLoai,
        "SoLuongIn": soLuongIn !== "" ? Number(soLuongIn) : 0,
        "GiaBia": giaBia !== "" ? Number(giaBia) : 0,
        "TrangThai": trangThai === "1",
    };

    let thongTinDaNgu = [{
        maNgonNgu: maNgonNgu,
        tenAnPham: tenAnPham.trim(),
        tacGia: tacGia.trim(),
        moTa: moTa.trim(),
    }]

    let formData = new FormData()
    formData.append("XuatBanAnPham", JSON.stringify(thongTinChung));
    formData.append("XuatBanAnPham_NoiDung", JSON.stringify(thongTinDaNgu));

    if (anhBia != null) { 
        formData.append("FileDinhKem", anhBia);
    }

    if (idAnPham) {
        $.ajax({
            type: 'PUT',
            url: `${baseUrl}/api/XuatBanAnPhamApi/ChinhSua/${idAnPham}`,
            async: false,
            contentType: false,
            processData: false,
            data: formData,
            success: function (data) {
                if (data.isSuccess && data.value) {
                    showNotification(1, "Chỉnh sửa ấn phẩm xuất bản thành công");
                    $('#modalAdd').modal('hide');
                } else {
                    showNotification(0, "Chỉnh sửa ấn phẩm xuất bản không thành công");
                }
            },
            error: function (err) {
                console.log(err)
            }
        })
    } else {
        $.ajax({
            type: 'POST',
            url: `${baseUrl}/api/XuatBanAnPhamApi/ThemMoi`,
            async: false,
            contentType: false,
            processData: false,
            data: formData,
            success: function (data) {
                if (data.isSuccess && data.value) {
                    showNotification(1, "Thêm mới ấn phẩm xuất bản thành công");
                    $('#modalAdd').modal('hide');
                } else {
                    showNotification(0, "Thêm mới ấn phẩm xuất bản không thành công");
                }
            },
            error: function (err) {
                console.log(err)
            }
        })
    }
}

function previewPicture() {
    // Xử lý preview ảnh bìa
    $('#anhBiaAdd').on('change', function(e) {
        const file = e.target.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = function(ev) {
                $('#showAnhBiaAdd').attr('src', ev.target.result);
            };
            reader.readAsDataURL(file);
        } else {
            $('#showAnhBiaAdd').attr('src', '/assets/images/vector/addImage.png');
        }
    });
}

function dataDelete() {
    let id = $('#idDelete').val();
    $.ajax({
        url: `${baseUrl}/api/ToChucApi/Xoa/${id}`,
        type: 'DELETE',
        contentType: 'application/json',
        success: function (data) {
            if (data && data.isSuccess && data.value) {
                showNotification(1, 'Xoá thành công')
                $('#modalDelete').modal('hide');
                $('#dataGrid').DataTable().ajax.reload();
            } else {
                showNotification(0, data.error)
            }
        },
        error: function (err) {
            showNotification(0, 'Có lỗi xảy ra, vui lòng thử lại sau')
        }
    })
}