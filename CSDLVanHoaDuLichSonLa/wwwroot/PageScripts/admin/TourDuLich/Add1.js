const baseUrl = getRootLink();

let tempDaPhuongTiens = [];
let tempLoTrinhs = [];
let tempLoTrinhNoiDungs = []; // Lưu nội dung của lộ trình đang edit
let currentEditingDaPhuongTienIndex = -1;
let currentEditingLoTrinhIndex = -1;
let currentEditingNoiDungIndex = -1;

$(document).ready(function () {

    //#region Xử lý các function chung
    initSelect2()
    initValidation()
    initDatePicker()
    initToChuc_NoAll(1, "#toChuc", "") // Tổ chức
    initDanhMucChung_NoAll(1, "#loaiHinh", "") // Loại hình tour
    initDanhMucChung_NoAll(2, "#phanKhuc", "") // Phân khúc tour  
    initDanhMucChung_NoAll(3, "#phuongTien", "") // Phương tiện
    initDiaDiem_NoAll("#diemDi", "Chọn điểm đi") // Điểm đi
    initDiaDiem_NoAll("#diemDen", "Chọn điểm đến") // Điểm đến
    initDiaDiem_NoAll("#diaDiemLoTrinhAdd", "Chọn địa điểm") // Địa điểm lộ trình
    initTempLoTrinhTable()
    initNgonNgu(".ngonNguDich") // Ngôn ngữ hướng dẫn
    initNgonNgu("#ngonNguDichAdd") // Ngôn ngữ dịch
    initTheLoaiCoSan("#theLoaiCoSanAdd", "Chọn thể loại") // Thể loại có sẵn
    initDanhMucChung_NoAll(1, "#linhVucAdd", "") // Lĩnh vực (modal địa điểm)
    initLinhVuc_MultiSelect("#linhVucModalAdd", "") // Lĩnh vực multi-select (modal thể loại)
    initNgonNgu("#ngonNguDichDiaDiemAdd") // Ngôn ngữ địa điểm
    initTinhThanhPho() // Tỉnh thành phố
    CheckDateMinMax("#ngayBatDauAdd", "#ngayKetThucAdd")
    initTempDaPhuongTienTable()

    function initValidation() {
        CheckLengthEach('#tenTour', 255, null);
        CheckLengthEach('#maDinhDanh', 50, null);
        CheckLengthEach('#thoiGian', 100, null);
        CheckLengthEach('#moTaAdd', 1000, null);
        CheckLengthEach('#traiNghiem', 1000, null);
        CheckLengthEach('#goiY', 1000, null);

        // Validation số khách
        $('#soKhachToiDa').on('blur', function () {
            const min = parseInt($('#soKhachToiThieu').val()) || 0;
            const max = parseInt($(this).val()) || 0;

            if (min > 0 && max > 0 && max < min) {
                showNotification(0, 'Số khách tối đa phải lớn hơn số khách tối thiểu!');
                $(this).val('');
            }
        });

        // Validation giá tour
        $('#giaNguoi').on('blur', function () {
            const giaTour = parseFloat($('#giaTour').val()) || 0;
            const giaNguoi = parseFloat($(this).val()) || 0;

            if (giaTour > 0 && giaNguoi > 0 && giaNguoi > giaTour) {
                showNotification(0, 'Giá/người không nên lớn hơn giá tour!');
            }
        });
    }

    $('#tinhThanhPhoAdd').on('change', function () {
        let tinhID = $(this).val();
        initPhuongXa(tinhID)
    })
    //#endregion Xử lý các function chung

    //#region Xử lý thông tin tour du lịch
    $(document).on('change', '#ngonNguDichAdd', function () {
        $('#tenTour').val('');
        $('#thoiGian').val('');
        $('#moTaAdd').val('');
        $('#traiNghiem').val('');
        $('#goiY').val('');
    });

    // ✅ Pattern từ DaoTaoBoiDuong: Form submit handling với button state
    $("#formSuKienAdd").on("submit", function (e) {
        e.preventDefault();

        const submitBtn = $(this).find('button[type="submit"]');
        if (submitBtn.prop('disabled')) {
            return;
        }

        submitBtn.prop('disabled', true);
        const originalText = submitBtn.text();
        submitBtn.data('original-text', originalText);
        submitBtn.text('Đang xử lý...');

        try {
            dataAdd();
        } catch (error) {
            showNotification(0, "Có lỗi xảy ra khi xử lý dữ liệu");
            showNotification(0, error.message);
            resetSubmitButton();
        }
    });

    // ✅ Cập nhật click handler để consistent với form submit
    $("#btnSaveAdd").on("click", function (e) {
        e.preventDefault();
        $("#formSuKienAdd").submit(); // Trigger form submit để có consistent handling
    });
    //#endregion Xử lý thông tin tour du lịch

    //#region Xử lý lộ trình tour  
    $("#btnModalAddLoTrinh").on("click", function () {
        $('#formAddLoTrinh')[0].reset();
        currentEditingLoTrinhIndex = -1;
        resetLoTrinhModal();
        $('#modalAddLoTrinh').modal('show');
    });

    $("#formAddLoTrinh").on("submit", function (e) {
        e.preventDefault();
        saveLoTrinhToTemp();
    });

    $(document).on('click', '.edit-temp-LoTrinh', function () {
        const index = $(this).data('index');
        editTempLoTrinh(index);
    });

    $(document).on('click', '.delete-temp-LoTrinh', function () {
        const index = $(this).data('index');
        const tenDiaDiem = tempLoTrinhs[index].tenDiaDiem || 'lộ trình này';

        if (confirm(`Bạn có chắc chắn muốn xóa "${tenDiaDiem}" khỏi lộ trình?`)) {
            tempLoTrinhs.splice(index, 1);
            refreshTempLoTrinhTable();
            showNotification(1, 'Đã xóa lộ trình khỏi danh sách');
        }
    });

    $("#btnAddDiaDiem").on("click", function () {
        dataAddDiaDiem()
    })

    // ✅ THÊM: Event cho nút thêm hoạt động bằng Enter
    $("#hoatDongLoTrinhAdd").on("keypress", function (e) {
        if (e.which === 13 && !e.shiftKey) { // Enter without Shift
            e.preventDefault();
            addNoiDungToLoTrinh();
        }
    });
    //#endregion Xử lý lộ trình tour

    //#region Xử lý thể loại tour
    $(document).on('change', '#theLoaiCoSanAdd', function () {
        let theLoaiCoSan = $(this).val();
        const selectedText = $(this).find('option:selected').text();

        if (theLoaiCoSan) {
            // KIỂM TRA: Nếu đã có thể loại thì thay thế
            if (tempDaPhuongTiens.length > 0) {
                if (confirm('Đã có thể loại được chọn. Bạn có muốn thay thế không?')) {
                    tempDaPhuongTiens = [];
                } else {
                    $(this).val('');
                    return;
                }
            }

            $(this).blur();

            const theLoaiData = {
                theLoaiID: theLoaiCoSan,
                tenTheLoai: selectedText,
                moTa: 'Thể loại có sẵn',
                linhVucIDs: [],
                tenLinhVuc: '',
                trangThai: true,
                isExisting: true
            };

            tempDaPhuongTiens.push(theLoaiData);
            refreshTempDaPhuongTienTable();
            showNotification(1, `Đã chọn thể loại: ${selectedText}`);
            $(this).val('');
        }
    });

    $('#btnAddDaPhuongTien').on('click', function () {
        currentEditingDaPhuongTienIndex = -1;
        resetDaPhuongTienModal();
        $('#modalAddDaPhuongTien').modal('show');
    });

    $(document).on('click', '.edit-temp-DaPhuongTien', function () {
        const index = $(this).data('index');
        editTempDaPhuongTien(index);
    });

    $(document).on('click', '.delete-temp-TheLoai', function () {
        const index = $(this).data('index');
        const tenTheLoai = tempDaPhuongTiens[index].tenTheLoai || 'thể loại này';

        if (confirm(`Bạn có chắc chắn muốn xóa "${tenTheLoai}" khỏi danh sách?`)) {
            tempDaPhuongTiens.splice(index, 1);
            refreshTempDaPhuongTienTable();
            showNotification(1, 'Đã xóa thể loại khỏi danh sách');
        }
    });

    $("#formAddDaPhuongTien").on("submit", function (e) {
        e.preventDefault();

        let tenTheLoai = $("#tenTheLoaiAdd").val().trim();
        let moTa = $("#moTaTheLoaiAdd").val().trim();
        let linhVucIDs = $("#linhVucModalAdd").val() || [];
        let trangThai = $("input[name='trangThai']:checked").val() == "1" ? true : false;

        // Validation
        if (!tenTheLoai) {
            showNotification(0, 'Vui lòng nhập tên thể loại');
            return;
        }

        if (!linhVucIDs || linhVucIDs.length === 0) {
            showNotification(0, 'Vui lòng chọn ít nhất một lĩnh vực');
            return;
        }

        // ✅ Gọi API thêm mới thể loại
        apiAddTheLoai(tenTheLoai, moTa, linhVucIDs, trangThai)
            .done(function (response) {
                if (response && response.isSuccess && response.value) {
                    // Lấy tên lĩnh vực từ select để hiển thị
                    let tenLinhVuc = [];
                    linhVucIDs.forEach(id => {
                        let text = $("#linhVucModalAdd option[value='" + id + "']").text();
                        if (text) tenLinhVuc.push(text);
                    });

                    const theLoaiData = {
                        theLoaiID: response.value.theLoaiID, // ✅ Lấy TheLoaiID từ response
                        tenTheLoai: tenTheLoai,
                        moTa: moTa,
                        linhVucIDs: linhVucIDs,
                        tenLinhVuc: tenLinhVuc.join(', '),
                        trangThai: trangThai,
                        isExisting: true
                    };

                    tempDaPhuongTiens = [theLoaiData];
                    refreshTempDaPhuongTienTable();
                    $('#modalAddDaPhuongTien').modal('hide');
                    $('#modalAddDaPhuongTien').one('hidden.bs.modal', function () {
                        setTimeout(function () {
                            initTheLoaiCoSan("#theLoaiCoSanAdd", "Chọn thể loại");
                        }, 200);
                    });
                    resetDaPhuongTienModal();
                }
            })
            .fail(function (xhr, status, error) {
                console.error('Lỗi API:', error);
            });
    });
    //#endregion Xử lý thể loại tour
});

//#region Function Xử lý chung
function initTinhThanhPho() {
    getDataWithApi('GET', '/api/CapTinhApi/Gets').then(data => {
        if (data && data.isSuccessed && data.resultObj) {
            $("#tinhThanhPhoAdd").empty().append(`<option value="">Chọn</option>`);
            data.resultObj.forEach(item => {
                const displayText = item.noiDung || item.tenTinh || 'Không rõ tên';
                $("#tinhThanhPhoAdd").append(`<option value="${item.tinhID}">${displayText}</option>`);
            });
        }
    })
}

function initPhuongXa(tinhID = null) {
    getDataWithApi('GET', `/api/CapXaApi/GetByMaTinh?MaNgonNgu=vi&MaTinh=${tinhID ? `${tinhID}` : ''}`).then(data => {
        $("#phuongXaAdd").empty().append(`<option value="">Chọn</option>`);
        if (data && data.isSuccess && data.value) {
            data.value.forEach(item => {
                const displayText = item.noiDung || item.tenXa || 'Không rõ tên';
                $("#phuongXaAdd").append(`<option value="${item.xaID}">${displayText}</option>`);
            });
        }
    })
}

function initLinhVuc_MultiSelect(elementId, placeholder) {
    getDataWithApi('GET', `/api/LinhVucApi/Gets`).then(response => {
        if (response && response.isSuccess && response.value) {
            $(elementId).empty();

            response.value.forEach(item => {
                const displayText = item.ten || 'Không rõ tên';
                $(elementId).append(`<option value="${item.linhVucID || item.id}">${displayText}</option>`);
            });

            if ($(elementId).hasClass('form-control')) {
                $(elementId).select2({
                    placeholder: placeholder || "Chọn lĩnh vực",
                    allowClear: true,
                    closeOnSelect: false,
                    width: '100%'
                });
            }
        } else {
            showNotification(0, "Không thể load danh sách lĩnh vực");
        }
    }).catch(error => {
        console.error('Lỗi khi load danh mục chung:', error);
        showNotification(0, "Lỗi kết nối khi tải danh sách lĩnh vực");
    });
}

// ✅ Function validation tổng thể
function validateBeforeSubmit() {
    const errors = [];

    // Validate thông tin cơ bản
    if (!$("#tenTour").val().trim()) {
        errors.push("Tên tour không được để trống");
    }

    if (!$("#maDinhDanh").val().trim()) {
        errors.push("Mã định danh không được để trống");
    }

    if (!$("#ngonNguDichAdd").val()) {
        errors.push("Vui lòng chọn ngôn ngữ");
    }

    // Validate thể loại
    if (tempDaPhuongTiens.length === 0) {
        errors.push("Vui lòng chọn thể loại tour");
    }

    // Validate lộ trình
    if (tempLoTrinhs.length > 0) {
        const thuTuSet = new Set();
        tempLoTrinhs.forEach((item, index) => {
            if (!item.diaDiemID) {
                errors.push(`Lộ trình ${index + 1}: Chưa chọn địa điểm`);
            }
            if (!item.thuTu) {
                errors.push(`Lộ trình ${index + 1}: Chưa nhập thứ tự`);
            } else if (thuTuSet.has(item.thuTu)) {
                errors.push(`Thứ tự ${item.thuTu} bị trùng lặp`);
            } else {
                thuTuSet.add(item.thuTu);
            }
            if (!item.thoiDiem) {
                errors.push(`Lộ trình ${index + 1}: Chưa nhập thời điểm`);
            }
        });
    }

    return errors;
}

// ✅ Helper function theo pattern DaoTaoBoiDuong
function resetSubmitButton() {
    const submitBtn = $('#formSuKienAdd').find('button[type="submit"]');
    const originalText = submitBtn.data('original-text') || 'Lưu';
    submitBtn.prop('disabled', false);
    submitBtn.text(originalText);
}
//#endregion Function Xử lý chung

//#region Function Xử lý thông tin tour du lịch
function dataAdd() {
    // ✅ Validation trước khi gửi
    const validationErrors = validateBeforeSubmit();
    if (validationErrors.length > 0) {
        showNotification(0, 'Vui lòng kiểm tra lại:<br>' + validationErrors.join('<br>'));
        resetSubmitButton(); // ✅ Reset button khi validation fail
        return;
    }

    // Thu thập dữ liệu từ form
    const maDinhDanh = $("#maDinhDanh").val().trim();
    const toChucID = $("#toChuc").val() || null;
    const theLoaiID = tempDaPhuongTiens.length > 0 ?
        (tempDaPhuongTiens[0].theLoaiID || tempDaPhuongTiens[0].id) : null;
    const loaiHinhID = $("#loaiHinh").val() || null;
    const phanKhucID = $("#phanKhuc").val() || null;
    const diemDiID = $("#diemDi").val() || null;
    const diemDenID = $("#diemDen").val() || null;
    const phuongTienID = $("#phuongTien").val() || null;
    const ngonNguHuongDan = $("#ngonNguDich").val() || null;

    const NgayBatDau = $("#ngayBatDauAdd").val() ? $("#ngayBatDauAdd").val().split('/').reverse().join('-') : null;
    const NgayKetThuc = $("#ngayKetThucAdd").val() ? $("#ngayKetThucAdd").val().split('/').reverse().join('-') : null;
    const TrangThai = $("input[name='trangThai']:checked").val() == "1";

    // Nội dung đa ngữ
    const MaNgonNgu = $("#ngonNguDichAdd").val() || "vi";
    const tenTour = $("#tenTour").val().trim();
    const thoiGian = $("#thoiGian").val().trim();
    const MoTa = $("#moTaAdd").val().trim();
    const traiNghiem = $("#traiNghiem").val().trim();
    const goiY = $("#goiY").val().trim();

    // ✅ OBJECT 1: TourDuLich entity (theo đúng Domain model)
    const tourDuLichObject = {
        MaDinhDanh: maDinhDanh,
        ToChucID: toChucID,
        TheLoaiID: theLoaiID,
        NgayKhoiHanh: NgayBatDau,
        NgayKetThuc: NgayKetThuc,
        TrangThai: TrangThai,
        // Tour-specific fields
        LoaiHinhID: loaiHinhID,
        PhanKhucID: phanKhucID,
        DiemDiID: diemDiID,
        DiemDenID: diemDenID,
        PhuongTienID: phuongTienID,
        NgonNguHuongDan: ngonNguHuongDan,
        SoKhachToiThieu: $("#soKhachToiThieu").val() ? parseInt($("#soKhachToiThieu").val()) : null,
        SoKhachToiDa: $("#soKhachToiDa").val() ? parseInt($("#soKhachToiDa").val()) : null,
        GiaTour: $("#giaTour").val() ? parseFloat($("#giaTour").val()) : null,
        GiaNguoi: $("#giaNguoi").val() ? parseFloat($("#giaNguoi").val()) : null,
        SoLuotDanhGia: parseInt($("#soLuotDanhGia").val() || "0"),
        DiemDanhGia: parseFloat($("#diemDanhGia").val() || "0")
    };

    // ✅ OBJECT 2: List<TourDuLich_NoiDung> (nội dung đa ngữ)
    const tourDuLichNoiDungArray = [{
        TourID: null, // Sẽ được server set
        MaNgonNgu: MaNgonNgu,
        TenTour: tenTour,
        ThoiGian: thoiGian,
        MoTa: MoTa,
        TraiNghiem: traiNghiem,
        GoiY: goiY
    }];

    // ✅ CONVERT TO JSON STRINGS (điều quan trọng!)
    const tourDuLichJsonString = JSON.stringify(tourDuLichObject);
    const noiDungJsonString = JSON.stringify(tourDuLichNoiDungArray);

    // ✅ CREATE FORMDATA với đúng tên fields
    let formData = new FormData();
    formData.append('TourDuLich', tourDuLichJsonString); // ✅ Tên field đúng
    formData.append('TourDuLich_NoiDung', noiDungJsonString); // ✅ Tên field đúng

    console.log('🚀 Sending data to API...');
    console.log('📤 TourDuLich JSON:', tourDuLichJsonString);
    console.log('📤 TourDuLich_NoiDung JSON:', noiDungJsonString);

    $.ajax({
        type: 'POST',
        url: `${baseUrl}/api/TourDuLichApi/ThemMoi`,
        async: false, // ✅ Giữ async: false như DaoTaoBoiDuong để đơn giản
        contentType: false,
        processData: false,
        data: formData,
        success: function (data) { // ✅ Không cần async ở đây nữa
            console.log('📥 API Response:', data);

            if (data.isSuccess && data.value) {
                const tourID = data.value.tourID || data.value.TourID;


                console.log('✅ TourID received:', tourID);

                if (!tourID) {
                    showNotification(0, "Không thể lấy TourID từ response");
                    resetSubmitButton();
                    return;
                }

                // ✅ Xử lý lộ trình đồng bộ (không async)
                if (tempLoTrinhs.length > 0) {
                    let successCount = 0;
                    let totalCount = tempLoTrinhs.length;

                    // ✅ Xử lý tuần tự từng lộ trình
                    for (let i = 0; i < tempLoTrinhs.length; i++) {
                        try {
                            const result = addLichTrinhSync(tourID, tempLoTrinhs[i]);
                            if (result && result.isSuccess) {
                                successCount++;
                            }
                        } catch (error) {
                            console.error(`Error adding itinerary ${i + 1}:`, error);
                        }
                    }

                    // ✅ Thông báo kết quả
                    if (successCount > 0) {
                        showNotification(1, `Thêm mới tour du lịch thành công với ${successCount}/${totalCount} lộ trình`);
                    } else {
                        showNotification(1, "Thêm mới tour du lịch thành công nhưng không thể thêm lộ trình");
                    }
                } else {
                    showNotification(1, "Thêm mới tour du lịch thành công");
                }

                // ✅ Redirect theo pattern DaoTaoBoiDuong
                setTimeout(() => {
                    window.location.href = `${baseUrl}/AdminTool/TourDuLich`;
                }, 1500);

            } else {
                showNotification(0, data.message || "Thêm mới tour du lịch không thành công");
                resetSubmitButton();
            }
        },
        error: function (xhr, status, error) {
            showNotification(0, "Có lỗi xảy ra khi kết nối đến server");
            resetSubmitButton();
        }
    });
}

// ✅ Helper function cho synchronous lộ trình
function addLichTrinhSync(tourID, loTrinhData) {
    // ✅ Fix date format cho ThoiDiem
    const convertedThoiDiem = loTrinhData.thoiDiem && loTrinhData.thoiDiem.includes('/') ?
        loTrinhData.thoiDiem.split('/').reverse().join('-') :
        loTrinhData.thoiDiem;

    const tourDuLich_LichTrinh = {
        //LichTrinhID: null,
        TourID: tourID,
        DiaDiemID: loTrinhData.diaDiemID,
        ThoiDiem: convertedThoiDiem, // ✅ Sử dụng date đã convert
        ThuTu: loTrinhData.thuTu
    };

    const tourDuLich_LichTrinh_NoiDung = loTrinhData.noiDungs && loTrinhData.noiDungs.length > 0
        ? loTrinhData.noiDungs.map(nd => ({
            //LichTrinhID: null,
            HoatDong: nd.hoatDong || '',
            SoGio: nd.soGio || ''
        }))
        : [{
            LichTrinhID: null,
            HoatDong: '',
            SoGio: ''
        }];

    let formData = new FormData();
    formData.append('TourDuLich_LichTrinh', JSON.stringify(tourDuLich_LichTrinh));
    formData.append('TourDuLich_LichTrinh_NoiDung', JSON.stringify(tourDuLich_LichTrinh_NoiDung));

    let result = null;
    $.ajax({
        type: 'POST',
        url: `${baseUrl}/api/TourDuLichApi/LichTrinh/ThemMoi`,
        async: false,
        processData: false,
        contentType: false,
        data: formData,
        success: function (response) {
            result = response;
        },
        error: function (xhr, status, error) {
            console.error('LichTrinh API Error:', error);
            result = { isSuccess: false, error: error };
        }
    });

    return result;
}
//#endregion Function Xử lý thông tin tour du lịch

//#region Function Xử lý lộ trình tour
function initTempLoTrinhTable() {
    const tableDefs = [
        {
            targets: 0, // Thứ tự
            render: function (data, type, row, meta) {
                return row.thuTu || '';
            }
        },
        {
            targets: 1, // Địa điểm
            render: function (data, type, row, meta) {
                return `<div class="group-info">
                    <div class="info-main">${row.tenDiaDiem || ''}</div>
                    <div class="info-sub">${row.diaChi || ''}</div>
                </div>`;
            }
        },
        {
            targets: 2, // Hoạt động (hiển thị tất cả hoạt động)
            render: function (data, type, row, meta) {
                if (row.noiDungs && row.noiDungs.length > 0) {
                    let hoatDongList = row.noiDungs.map(nd =>
                        `${nd.hoatDong}${nd.soGio ? ' (' + nd.soGio + ' giờ)' : ''}`
                    ).join('<br>');
                    return `<div class="group-info">
                        <div class="info-main">${hoatDongList}</div>
                    </div>`;
                }
                return 'Chưa có hoạt động';
            }
        },
        {
            targets: 3, // Thời điểm đến
            render: function (data, type, row, meta) {
                return row.thoiDiem ? formatDateTimeDisplay(row.thoiDiem) : 'Chưa xác định';
            }
        },
        {
            targets: 4, // Chức năng
            render: function (data, type, row, meta) {
                return `
                    <i data-toggle="tooltip" title="Chỉnh sửa" class="edit-temp-LoTrinh text-yellow cursor-pointer me-2" data-index="${meta.row}">
                        <i class="hgi-icon hgi-edit"></i>
                    </i>
                    <i data-toggle="tooltip" title="Xóa" class="delete-temp-LoTrinh text-red cursor-pointer" data-index="${meta.row}">
                        <i class="hgi-icon hgi-delete"></i>
                    </i>
                `;
            }
        }
    ];

    const tableCols = [
        { "data": "", "width": "80px", "class": "center-align" },
        { "data": "", "class": "left-align" },
        { "data": "", "width": "40%", "class": "left-align" },
        { "data": "", "width": "20%", "class": "center-align" },
        { "data": "", "width": "100px", "class": "center-align group-icon-action" }
    ];

    if ($.fn.DataTable.isDataTable('#dataGridDiaDiem')) {
        $('#dataGridDiaDiem').DataTable().destroy();
    }

    $('#dataGridDiaDiem').DataTable({
        data: tempLoTrinhs.map((item, index) => ({ ...item, stt: index + 1 })),
        columns: tableCols,
        columnDefs: tableDefs,
        paging: false,
        searching: false,
        info: false,
        ordering: false,
        autoWidth: false,
        responsive: true,
        language: {
            emptyTable: "Chưa có lộ trình nào được thêm"
        }
    });
}

function refreshTempLoTrinhTable() {
    if ($.fn.DataTable.isDataTable('#dataGridDiaDiem')) {
        const table = $('#dataGridDiaDiem').DataTable();
        const dataWithStt = tempLoTrinhs.map((item, index) => ({ ...item, stt: index + 1 }));
        table.clear().rows.add(dataWithStt).draw();
    } else {
        initTempLoTrinhTable();
    }
}

function saveLoTrinhToTemp() {
    let diaDiemID = $("#diaDiemLoTrinhAdd").val();
    let thoiDiem = $("#thoiDiemLoTrinhAdd").val();
    let thuTu = $("#thuTuLoTrinhAdd").val();

    // Validation cơ bản
    if (!diaDiemID) {
        showNotification(0, 'Vui lòng chọn địa điểm');
        return;
    }

    if (!thoiDiem) {
        showNotification(0, 'Vui lòng nhập thời điểm đến dự kiến');
        return;
    }

    if (!thuTu) {
        showNotification(0, 'Vui lòng nhập thứ tự ghé thăm');
        return;
    }

    // Kiểm tra thứ tự hợp lệ (1-255)
    const thuTuInt = parseInt(thuTu);
    if (thuTuInt < 1 || thuTuInt > 255) {
        showNotification(0, 'Thứ tự phải từ 1 đến 255');
        return;
    }

    // Kiểm tra trùng thứ tự
    const existingThuTu = tempLoTrinhs.find((item, index) =>
        item.thuTu == thuTuInt && index !== currentEditingLoTrinhIndex
    );

    if (existingThuTu) {
        showNotification(0, `Thứ tự ${thuTu} đã tồn tại trong lộ trình`);
        return;
    }

    const tenDiaDiem = $("#diaDiemLoTrinhAdd option:selected").text();
    const diaChi = $("#diaDiemLoTrinhAdd option:selected").data('address') || '';

    // ✅ THAY ĐỔI: Lưu nhiều nội dung thay vì 1
    const loTrinhData = {
        diaDiemID: diaDiemID,
        tenDiaDiem: tenDiaDiem,
        diaChi: diaChi,
        thoiDiem: thoiDiem,
        thuTu: thuTuInt,
        noiDungs: [...tempLoTrinhNoiDungs] // Copy array nội dung
    };

    try {
        if (currentEditingLoTrinhIndex >= 0) {
            tempLoTrinhs[currentEditingLoTrinhIndex] = loTrinhData;
            showNotification(1, 'Cập nhật lộ trình thành công');
        } else {
            tempLoTrinhs.push(loTrinhData);
            showNotification(1, 'Thêm lộ trình thành công');
        }

        // Sắp xếp lại theo thứ tự
        tempLoTrinhs.sort((a, b) => a.thuTu - b.thuTu);

        refreshTempLoTrinhTable();
        $('#modalAddLoTrinh').modal('hide');
        resetLoTrinhModal();

    } catch (error) {
        console.error('Lỗi khi lưu lộ trình:', error);
        showNotification(0, 'Có lỗi xảy ra khi lưu lộ trình');
    }
}

// ✅ THÊM: Functions quản lý nội dung lộ trình
function addNoiDungToLoTrinh() {
    let hoatDong = $("#hoatDongLoTrinhAdd").val().trim();
    let soGio = $("#soGioLoTrinhAdd").val().trim();

    if (!hoatDong) {
        showNotification(0, 'Vui lòng nhập hoạt động');
        return;
    }

    const noiDungData = {
        hoatDong: hoatDong,
        soGio: soGio || null
    };

    if (currentEditingNoiDungIndex >= 0) {
        tempLoTrinhNoiDungs[currentEditingNoiDungIndex] = noiDungData;
        currentEditingNoiDungIndex = -1;
        showNotification(1, 'Cập nhật hoạt động thành công');
    } else {
        tempLoTrinhNoiDungs.push(noiDungData);
        showNotification(1, 'Thêm hoạt động thành công');
    }

    // Reset input
    $("#hoatDongLoTrinhAdd").val('');
    $("#soGioLoTrinhAdd").val('');

    refreshNoiDungTable();
}

function editNoiDung(index) {
    currentEditingNoiDungIndex = index;
    const noiDung = tempLoTrinhNoiDungs[index];

    $("#hoatDongLoTrinhAdd").val(noiDung.hoatDong);
    $("#soGioLoTrinhAdd").val(noiDung.soGio || '');
}

function deleteNoiDung(index) {
    if (confirm('Bạn có chắc chắn muốn xóa hoạt động này?')) {
        tempLoTrinhNoiDungs.splice(index, 1);
        refreshNoiDungTable();
        showNotification(1, 'Đã xóa hoạt động');
    }
}

function refreshNoiDungTable() {
    let html = '';
    tempLoTrinhNoiDungs.forEach((noiDung, index) => {
        html += `
            <tr>
                <td>${index + 1}</td>
                <td>${noiDung.hoatDong}</td>
                <td>${noiDung.soGio ? noiDung.soGio + ' giờ' : ''}</td>
                <td>
                    <i class="edit-noiDung text-yellow cursor-pointer me-2" onclick="editNoiDung(${index})" title="Chỉnh sửa">
                        <i class="hgi-icon hgi-edit"></i>
                    </i>
                    <i class="delete-noiDung text-red cursor-pointer" onclick="deleteNoiDung(${index})" title="Xóa">
                        <i class="hgi-icon hgi-delete"></i>
                    </i>
                </td>
            </tr>
        `;
    });

    $('#tableNoiDungLoTrinh tbody').html(html);
}

// ✅ THAY ĐỔI: Edit lộ trình
function editTempLoTrinh(index) {
    currentEditingLoTrinhIndex = index;
    const item = tempLoTrinhs[index];

    $("#diaDiemLoTrinhAdd").val(item.diaDiemID);
    $("#thoiDiemLoTrinhAdd").val(item.thoiDiem);
    $("#thuTuLoTrinhAdd").val(item.thuTu);

    // Load nội dung của lộ trình
    tempLoTrinhNoiDungs = [...(item.noiDungs || [])];
    refreshNoiDungTable();

    $('#modalAddLoTrinh').modal('show');
}

// ✅ THAY ĐỔI: Reset modal
function resetLoTrinhModal() {
    $("#formAddLoTrinh")[0].reset();
    currentEditingLoTrinhIndex = -1;
    currentEditingNoiDungIndex = -1;
    tempLoTrinhNoiDungs = [];
    refreshNoiDungTable();
}

function formatDateTimeDisplay(dateTimeString) {
    if (!dateTimeString) return '';

    try {
        const date = new Date(dateTimeString);
        if (isNaN(date.getTime())) return dateTimeString; // ✅ THÊM dòng này

        const day = date.getDate().toString().padStart(2, '0');
        const month = (date.getMonth() + 1).toString().padStart(2, '0');
        const year = date.getFullYear();
        const hours = date.getHours().toString().padStart(2, '0');
        const minutes = date.getMinutes().toString().padStart(2, '0');

        return `${day}/${month}/${year} ${hours}:${minutes}`;
    } catch (error) {
        return dateTimeString;
    }
}

function dataAddDiaDiem() {
    const DiaDiemID = null;
    const DiaDiemCapChaID = null;
    const LinhVucID = $("#linhVucAdd").val();
    const XaID = $("#phuongXaAdd").val();
    const TinhID = $("#tinhThanhPhoAdd").val();
    const KinhDo = $("#kinhDoAdd").val();
    const ViDo = $("#viDoAdd").val();
    const CaoDo = $("#caoDoAdd").val();
    const BanDo = null;
    const HinhAnh = null;
    const NguoiKhuyetTat = $("#nguoiKhuyetTatAdd").is(":checked");
    const NhaVeSinh = $("#nhaVeSinhAdd").is(":checked");
    const BaiDoXe = $("#baiDoXeAdd").is(":checked");

    const MaNgonNguDiaDiem = $("#ngonNguDichDiaDiemAdd").val();
    const ThoiGianHoatDong = $("#thoiGianHoatDongDichAdd").val();
    const TenDiaDiem = $("#tenDiaDiemDichAdd").val();
    const SucChua = $("#sucChuaDichAdd").val();
    const HeToaDo = null;
    const DiaChiDiaDiem = $("#diaChiDichAdd").val();
    const MoTaDiaDiem = $("#moTaDichAdd").val();

    let diaDiem = {
        DiaDiemID: DiaDiemID,
        DiaDiemCapChaID: DiaDiemCapChaID,
        LinhVucID: LinhVucID,
        XaID: XaID,
        TinhID: TinhID,
        KinhDo: KinhDo,
        ViDo: ViDo,
        CaoDo: CaoDo,
        BanDo: BanDo,
        HinhAnh: HinhAnh,
        NguoiKhuyetTat: NguoiKhuyetTat,
        NhaVeSinh: NhaVeSinh,
        BaiDoXe: BaiDoXe,
    }

    let diaDiem_NoiDung = [{
        DiaDiemID: DiaDiemID,
        MaNgonNgu: MaNgonNguDiaDiem,
        ThoiGianHoatDong: ThoiGianHoatDong,
        TenDiaDiem: TenDiaDiem,
        SucChua: SucChua,
        HeToaDo: HeToaDo,
        DiaChi: DiaChiDiaDiem,
        MoTa: MoTaDiaDiem
    }]

    let formData = new FormData()
    formData.append("DiaDiem", JSON.stringify(diaDiem))
    formData.append("DiaDiem_NoiDung", JSON.stringify(diaDiem_NoiDung))

    $.ajax({
        type: 'POST',
        url: `${baseUrl}/api/DiaDiemApi/ThemMoi`,
        async: false,
        contentType: false,
        processData: false,
        data: formData,
        success: function (data) {
            console.log(data)
            if (data.isSuccess && data.value) {
                var newDiaDiemId = data.value.diaDiemID || data.value.id || data.value;
                console.log("Địa điểm vừa thêm mới có ID:", newDiaDiemId);

                // Cập nhật lại dropdown
                initDiaDiem_NoAll("#diemDi", "Chọn điểm đi");
                initDiaDiem_NoAll("#diemDen", "Chọn điểm đến");
                initDiaDiem_NoAll("#diaDiemLoTrinhAdd", "Chọn địa điểm");

                $('#modalAddDiaDiem').modal('hide');
                showNotification(1, "Thêm địa điểm mới thành công");
            }
        },
        error: function (err) {
            console.log(err)
            showNotification(0, "Có lỗi khi thêm địa điểm mới");
        }
    })
}
//#endregion Function Xử lý lộ trình tour

//#region Function Xử lý thể loại tour
function initTempDaPhuongTienTable() {
    const tableDefs = [
        {
            targets: 1, // Tên thể loại
            render: function (data, type, row, meta) {
                return row.tenTheLoai || '';
            }
        },
        {
            targets: 2, // Lĩnh vực
            render: function (data, type, row, meta) {
                return row.tenLinhVuc || 'Chưa chọn lĩnh vực';
            }
        },
        {
            targets: 3, // Mô tả
            render: function (data, type, row, meta) {
                return row.moTa || '';
            }
        },
        {
            targets: 4, // Trạng thái
            render: function (data, type, row, meta) {
                if (row.trangThai || row.SuDung) {
                    return `<span class="TrangThai green-text">Duyệt</span>`;
                } else {
                    return `<span class="TrangThai red-text">Chưa duyệt</span>`;
                }
            }
        },
        {
            targets: 5, // Chức năng
            render: function (data, type, row, meta) {
                let actions = '';

                if (!row.isExisting) {
                    actions += `<i data-toggle="tooltip" title="Chỉnh sửa" class="edit-temp-DaPhuongTien text-yellow cursor-pointer me-2" data-index="${meta.row}">
                        <i class="hgi-icon hgi-edit"></i>
                    </i>`;
                }

                actions += `<i data-toggle="tooltip" title="Xóa khỏi danh sách" class="delete-temp-TheLoai text-red cursor-pointer" data-index="${meta.row}">
                    <i class="hgi-icon hgi-delete"></i>
                </i>`;

                return actions;
            }
        }
    ];

    const tableCols = [
        { "data": "stt", "width": "40px", "class": "center-align" },
        { "data": "", "class": "left-align" },
        { "data": "", "width": "25%", "class": "left-align" },
        { "data": "", "class": "left-align" },
        { "data": "", "width": "15%", "class": "center-align" },
        { "data": "", "width": "120px", "class": "center-align group-icon-action" }
    ];

    if ($.fn.DataTable.isDataTable('#dataGridDaPhuongTien')) {
        $('#dataGridDaPhuongTien').DataTable().destroy();
    }

    $('#dataGridDaPhuongTien').DataTable({
        data: tempDaPhuongTiens.map((item, index) => ({ ...item, stt: index + 1 })),
        columns: tableCols,
        columnDefs: tableDefs,
        paging: false,
        searching: false,
        info: false,
        ordering: false,
        autoWidth: false,
        responsive: true,
        language: {
            emptyTable: "Chưa có thể loại nào được chọn"
        }
    });
}

function refreshTempDaPhuongTienTable() {
    if ($.fn.DataTable.isDataTable('#dataGridDaPhuongTien')) {
        const table = $('#dataGridDaPhuongTien').DataTable();
        const dataWithStt = tempDaPhuongTiens.map((item, index) => ({ ...item, stt: index + 1 }));
        table.clear().rows.add(dataWithStt).draw();
    } else {
        initTempDaPhuongTienTable();
    }
}

function editTempDaPhuongTien(index) {
    currentEditingDaPhuongTienIndex = index;
    const item = tempDaPhuongTiens[index];

    $("#tenTheLoaiAdd").val(item.tenTheLoai || '');
    $("#moTaTheLoaiAdd").val(item.moTa || '');

    if (item.linhVucIDs && Array.isArray(item.linhVucIDs)) {
        $("#linhVucModalAdd").val(item.linhVucIDs).trigger('change');
    }

    $('input[name="trangThai"][value="' + (item.trangThai ? "1" : "0") + '"]').prop('checked', true);

    $('#modalAddDaPhuongTien').modal('show');
}

function resetDaPhuongTienModal() {
    $("#formAddDaPhuongTien")[0].reset();
    $("#tenTheLoaiAdd").val('');
    $("#moTaTheLoaiAdd").val('');
    $("#linhVucModalAdd").val(null).trigger('change');
    $('input[name="trangThai"][value="1"]').prop('checked', true);
    currentEditingDaPhuongTienIndex = -1;
}

function createRequestDaPhuongTiens() {
    return tempDaPhuongTiens.filter(item => !item.isExisting).map(item => ({
        TheLoai: {
            TenTheLoai: item.tenTheLoai,
            MoTa: item.moTa,
            LinhVucIDs: item.linhVucIDs,
            TrangThai: item.trangThai
        }
    }));
}
//#endregion Function Xử lý thể loại tour

//#region Function hỗ trợ
function initTheLoaiCoSan(elementId = "#theLoaiCoSanAdd", placeholder = "Chọn thể loại") {
    getDataWithApi('GET', '/api/TourDuLichApi/TheLoai/DanhSach?maNgonNgu=vi', null).then(response => {
        if (response && response.isSuccess && response.value) {
            $(elementId).empty().append(`<option value="">${placeholder}</option>`);

            response.value.forEach(item => {
                const displayText = item.tenTheLoai || item.noiDung || 'Không rõ tên';
                $(elementId).append(`<option value="${item.theLoaiID || item.id}">${displayText}</option>`);
            });
        } else {
            showNotification(0, "Không thể load danh sách thể loại");
        }
    }).catch(error => {
        console.error('Lỗi khi load thể loại:', error);
        showNotification(0, "Lỗi kết nối khi tải danh sách thể loại");
    });
}

function initDiaDiem_NoAll(elementId = "#diaDiem", placeholder = "Chọn địa điểm") {
    const requestData = JSON.stringify({
        MaNgonNgu: "vi",
        TrangThai: true
    });

    getDataWithApi('POST', '/api/DiaDiemApi/DanhSach', requestData).then(response => {
        if (response && response.isSuccess && response.value) {
            $(elementId).empty().append(`<option value="">${placeholder}</option>`);

            response.value.forEach(item => {
                const displayText = item.tenDiaDiem || item.noiDung || 'Không rõ tên';
                const addressText = item.diaChi ? ` - ${item.diaChi}` : '';
                const fullText = displayText + addressText;

                $(elementId).append(`<option value="${item.diaDiemID || item.id}">${fullText}</option>`);
            });
        } else {
            showNotification(0, "Không thể load danh sách địa điểm");
        }
    }).catch(error => {
        showNotification(0, "Lỗi kết nối khi tải danh sách địa điểm");
    });
}

// ✅ Bước 1: API thêm thể loại
function apiAddTheLoai(tenTheLoai, moTa, linhVucIDs, trangThai, maNgonNgu = "vi") {
    const tourDuLichTheLoai = {
        TrangThai: trangThai
    };

    const noiDung = [{
        MaNgonNgu: maNgonNgu,
        TenTheLoai: tenTheLoai,
        MoTa: moTa
    }];

    const listLinhVucID = Array.isArray(linhVucIDs) ? linhVucIDs.join(',') : linhVucIDs;

    let formData = new FormData();
    formData.append('TourDuLich_TheLoai', JSON.stringify(tourDuLichTheLoai));
    formData.append('TourDuLich_TheLoai_NoiDung', JSON.stringify(noiDung));
    formData.append('ListLinhVucID', listLinhVucID);

    console.log('📤 Sending TheLoai data:', { tenTheLoai, linhVucIDs });

    return $.ajax({
        type: 'POST',
        url: `${baseUrl}/api/TourDuLichApi/TheLoai/ThemMoi`, // ✅ Bước 1 endpoint
        processData: false,
        contentType: false,
        data: formData,
        success: function (response) {
            console.log('📥 TheLoai API Response:', response);
            if (response && response.isSuccess && response.value) {
                showNotification(1, "✅ Thêm mới thể loại thành công");
            } else {
                showNotification(0, response.error || "❌ Thêm mới thể loại không thành công");
            }
        },
        error: function (xhr, status, error) {
            showNotification(0, "❌ Có lỗi xảy ra khi thêm thể loại");
            console.error('Chi tiết lỗi:', xhr.responseText);
        }
    });
}
//#endregion Function hỗ trợ