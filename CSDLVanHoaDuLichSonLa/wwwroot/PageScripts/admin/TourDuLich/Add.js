// Global Variables & Constants
const baseUrl = getRootLink();

let tempDaPhuongTiens = [];
let tempLoTrinhs = [];
let tempLoTrinhNoiDungs = [];
let currentEditingDaPhuongTienIndex = -1;
let currentEditingLoTrinhIndex = -1;
let currentEditingNoiDungIndex = -1;

// Document Ready - Main Initialization
$(document).ready(function () {
    initializeApplication();
    bindEvents();
});

// Application Initialization
function initializeApplication() {
    initializeFormComponents();
    initializeValidation();
    initializeDataTables();
    initializeDropdowns();
}

function initializeFormComponents() {
    initSelect2();
    initDatePicker();
    CheckDateMinMax("#ngayBatDauAdd", "#ngayKetThucAdd");
}

function initializeDropdowns() {
    initToChuc_NoAll(1, "#toChuc", "");
    initDanhMucChung_NoAll(1, "#loaiHinh", "");
    initDanhMucChung_NoAll(2, "#phanKhuc", "");
    initDanhMucChung_NoAll(3, "#phuongTien", "");
    initDiaDiem_NoAll("#diemDi", "Chọn điểm đi");
    initDiaDiem_NoAll("#diemDen", "Chọn điểm đến");
    initDiaDiem_NoAll("#diaDiemLoTrinhAdd", "Chọn địa điểm");
    initNgonNgu(".ngonNguDich");
    initNgonNgu("#ngonNguDichAdd");
    initTheLoaiCoSan("#theLoaiCoSanAdd", "Chọn thể loại");
    initDanhMucChung_NoAll(1, "#linhVucAdd", "");
    initLinhVuc_MultiSelect("#linhVucModalAdd", "");
    initNgonNgu("#ngonNguDichDiaDiemAdd");
    initTinhThanhPho();
}

function initializeDataTables() {
    initTempLoTrinhTable();
    initTempDaPhuongTienTable();
}

function initializeValidation() {
    CheckLengthEach('#tenTour', 255, null);
    CheckLengthEach('#maDinhDanh', 50, null);
    CheckLengthEach('#thoiGian', 100, null);
    CheckLengthEach('#moTaAdd', 1000, null);
    CheckLengthEach('#traiNghiem', 1000, null);
    CheckLengthEach('#goiY', 1000, null);

    $('#soKhachToiDa').on('blur', function () {
        const min = parseInt($('#soKhachToiThieu').val()) || 0;
        const max = parseInt($(this).val()) || 0;

        if (min > 0 && max > 0 && max < min) {
            showNotification(0, 'Số khách tối đa phải lớn hơn số khách tối thiểu!');
            $(this).val('');
        }
    });

    $('#giaNguoi').on('blur', function () {
        const giaTour = parseFloat($('#giaTour').val()) || 0;
        const giaNguoi = parseFloat($(this).val()) || 0;

        if (giaTour > 0 && giaNguoi > 0 && giaNguoi > giaTour) {
            showNotification(0, 'Giá/người không nên lớn hơn giá tour!');
        }
    });
}

// Event Binding
function bindEvents() {
    bindFormEvents();
    bindItineraryEvents();
    bindCategoryEvents();
    bindLocationEvents();
}

function bindFormEvents() {
    $("#formSuKienAdd").on("submit", handleFormSubmit);
    $("#btnSaveAdd").on("click", handleSaveClick);

    $(document).on('change', '#ngonNguDichAdd', function () {
        $('#tenTour').val('');
        $('#thoiGian').val('');
        $('#moTaAdd').val('');
        $('#traiNghiem').val('');
        $('#goiY').val('');
    });

    $('#tinhThanhPhoAdd').on('change', function () {
        let tinhID = $(this).val();
        initPhuongXa(tinhID);
    });
}

function bindItineraryEvents() {
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

    $("#hoatDongLoTrinhAdd").on("keypress", function (e) {
        if (e.which === 13 && !e.shiftKey) {
            e.preventDefault();
            addNoiDungToLoTrinh();
        }
    });
}

function bindCategoryEvents() {
    $(document).on('change', '#theLoaiCoSanAdd', function () {
        let theLoaiCoSan = $(this).val();
        const selectedText = $(this).find('option:selected').text();

        if (theLoaiCoSan) {
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

    $("#formAddDaPhuongTien").on("submit", handleCategoryFormSubmit);
}

function bindLocationEvents() {
    $("#btnAddDiaDiem").on("click", function () {
        dataAddDiaDiem();
    });
}

// Form Handlers
function handleFormSubmit(e) {
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
}

function handleSaveClick(e) {
    e.preventDefault();
    $("#formSuKienAdd").submit();
}

function handleCategoryFormSubmit(e) {
    e.preventDefault();

    let tenTheLoai = $("#tenTheLoaiAdd").val().trim();
    let moTa = $("#moTaTheLoaiAdd").val().trim();
    let linhVucIDs = $("#linhVucModalAdd").val() || [];
    let trangThai = $("input[name='trangThai']:checked").val() == "1" ? true : false;

    if (!tenTheLoai) {
        showNotification(0, 'Vui lòng nhập tên thể loại');
        return;
    }

    if (!linhVucIDs || linhVucIDs.length === 0) {
        showNotification(0, 'Vui lòng chọn ít nhất một lĩnh vực');
        return;
    }

    apiAddTheLoai(tenTheLoai, moTa, linhVucIDs, trangThai)
        .done(function (response) {
            if (response && response.isSuccess && response.value) {
                let tenLinhVuc = [];
                linhVucIDs.forEach(id => {
                    let text = $("#linhVucModalAdd option[value='" + id + "']").text();
                    if (text) tenLinhVuc.push(text);
                });

                const theLoaiData = {
                    theLoaiID: response.value.theLoaiID,
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
            showNotification(0, 'Có lỗi xảy ra khi thêm thể loại');
        });
}

// Validation Functions
function validateBeforeSubmit() {
    const errors = [];

    if (!$("#tenTour").val().trim()) {
        errors.push("Tên tour không được để trống");
    }

    if (!$("#maDinhDanh").val().trim()) {
        errors.push("Mã định danh không được để trống");
    }

    if (!$("#ngonNguDichAdd").val()) {
        errors.push("Vui lòng chọn ngôn ngữ");
    }

    if (tempDaPhuongTiens.length === 0) {
        errors.push("Vui lòng chọn thể loại tour");
    }

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

function resetSubmitButton() {
    const submitBtn = $('#formSuKienAdd').find('button[type="submit"]');
    const originalText = submitBtn.data('original-text') || 'Lưu';
    submitBtn.prop('disabled', false);
    submitBtn.text(originalText);
}

// Main Data Processing
function dataAdd() {
    const validationErrors = validateBeforeSubmit();
    if (validationErrors.length > 0) {
        showNotification(0, 'Vui lòng kiểm tra lại:<br>' + validationErrors.join('<br>'));
        resetSubmitButton();
        return;
    }

    const formData = buildTourData();
    submitTourData(formData);
}

function buildTourData() {
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

    const MaNgonNgu = $("#ngonNguDichAdd").val() || "vi";
    const tenTour = $("#tenTour").val().trim();
    const thoiGian = $("#thoiGian").val().trim();
    const MoTa = $("#moTaAdd").val().trim();
    const traiNghiem = $("#traiNghiem").val().trim();
    const goiY = $("#goiY").val().trim();

    const tourDuLichObject = {
        MaDinhDanh: maDinhDanh,
        ToChucID: toChucID,
        TheLoaiID: theLoaiID,
        NgayKhoiHanh: NgayBatDau,
        NgayKetThuc: NgayKetThuc,
        TrangThai: TrangThai,
        LoaiHinhID: loaiHinhID,
        PhanKhucID: phanKhucID,
        DiemDiID: diemDiID,
        DiemDenID: diemDenID,
        PhuongTienID: phuongTienID,
        MaNgonNgu: ngonNguHuongDan,
        SoKhachToiThieu: $("#soKhachToiThieu").val() ? parseInt($("#soKhachToiThieu").val()) : null,
        SoKhachToiDa: $("#soKhachToiDa").val() ? parseInt($("#soKhachToiDa").val()) : null,
        GiaTour: $("#giaTour").val() ? parseFloat($("#giaTour").val()) : null,
        GiaNguoi: $("#giaNguoi").val() ? parseFloat($("#giaNguoi").val()) : null,
        SoLuotDanhGia: parseInt($("#soLuotDanhGia").val() || "0"),
        DiemDanhGia: parseFloat($("#diemDanhGia").val() || "0")
    };

    const tourDuLichNoiDungArray = [{
        TourID: null,
        MaNgonNgu: MaNgonNgu,
        TenTour: tenTour,
        ThoiGian: thoiGian,
        MoTa: MoTa,
        TraiNghiem: traiNghiem,
        GoiY: goiY
    }];

    const tourDuLichJsonString = JSON.stringify(tourDuLichObject);
    const noiDungJsonString = JSON.stringify(tourDuLichNoiDungArray);

    let formData = new FormData();
    formData.append('TourDuLich', tourDuLichJsonString);
    formData.append('TourDuLich_NoiDung', noiDungJsonString);

    return formData;
}

function submitTourData(formData) {
    $.ajax({
        type: 'POST',
        url: `${baseUrl}/api/TourDuLichApi/ThemMoi`,
        async: false,
        contentType: false,
        processData: false,
        data: formData,
        success: function (data) {
            if (data.isSuccess && data.value) {
                const tourID = data.value.tourID || data.value.TourID;

                if (!tourID) {
                    showNotification(0, "Không thể lấy TourID từ response");
                    resetSubmitButton();
                    return;
                }

                processItineraries(tourID);

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

function processItineraries(tourID) {
    if (tempLoTrinhs.length > 0) {
        const danhSachLichTrinh = tempLoTrinhs.map(loTrinh => ({
            LichTrinh: {
                //LichTrinhID: 0, 
                TourID: tourID,
                DiaDiemID: loTrinh.diaDiemID,
                ThoiDiem: convertThoiDiem(loTrinh.thoiDiem),
                ThuTu: loTrinh.thuTu
            },
            NoiDungs: loTrinh.noiDungs && loTrinh.noiDungs.length > 0
                ? loTrinh.noiDungs.map(nd => ({
                    LichTrinhID: null,
                    HoatDong: nd.hoatDong || '',
                    SoGio: nd.soGio || ''
                }))
                : [{
                    LichTrinhID: null,
                    HoatDong: '',
                    SoGio: ''
                }]
        }));

        const result = addAllLichTrinhSync(danhSachLichTrinh);

        if (result && result.isSuccess) {
            const successCount = result.value ? result.value.length : 0;
            showNotification(1, `Thêm mới tour du lịch thành công với ${successCount}/${tempLoTrinhs.length} lộ trình`);
        } else {
            showNotification(1, "Thêm mới tour du lịch thành công nhưng không thể thêm lộ trình");
        }
    } else {
        showNotification(1, "Thêm mới tour du lịch thành công");
    }
}

function addAllLichTrinhSync(danhSachLichTrinh) {
    let formData = new FormData();
    formData.append('DanhSachLichTrinh', JSON.stringify(danhSachLichTrinh));

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
            result = { isSuccess: false, error: error };
        }
    });

    return result;
}

function convertThoiDiem(thoiDiem) {
    return thoiDiem && thoiDiem.includes('/') ?
        thoiDiem.split('/').reverse().join('-') :
        thoiDiem;
}

//function addLichTrinhSyncOLD(tourID, loTrinhData) {
//    const convertedThoiDiem = loTrinhData.thoiDiem && loTrinhData.thoiDiem.includes('/') ?
//        loTrinhData.thoiDiem.split('/').reverse().join('-') :
//        loTrinhData.thoiDiem;

//    const tourDuLich_LichTrinh = {
//        TourID: tourID,
//        DiaDiemID: loTrinhData.diaDiemID,
//        ThoiDiem: convertedThoiDiem,
//        ThuTu: loTrinhData.thuTu
//    };

//    const tourDuLich_LichTrinh_NoiDung = loTrinhData.noiDungs && loTrinhData.noiDungs.length > 0
//        ? loTrinhData.noiDungs.map(nd => ({
//            HoatDong: nd.hoatDong || '',
//            SoGio: nd.soGio || ''
//        }))
//        : [{
//            LichTrinhID: null,
//            HoatDong: '',
//            SoGio: ''
//        }];

//    let formData = new FormData();
//    formData.append('TourDuLich_LichTrinh', JSON.stringify(tourDuLich_LichTrinh));
//    formData.append('TourDuLich_LichTrinh_NoiDung', JSON.stringify(tourDuLich_LichTrinh_NoiDung));
//    alert(JSON.stringify(tourDuLich_LichTrinh));
//    alert(JSON.stringify(tourDuLich_LichTrinh_NoiDung));
//    let result = null;
//    $.ajax({
//        type: 'POST',
//        url: `${baseUrl}/api/TourDuLichApi/LichTrinh/ThemMoi`,
//        async: false,
//        processData: false,
//        contentType: false,
//        data: formData,
//        success: function (response) {
//            result = response;
//        },
//        error: function (xhr, status, error) {
//            result = { isSuccess: false, error: error };
//        }
//    });

//    return result;
//}

// Itinerary Management
function initTempLoTrinhTable() {
    const tableDefs = [
        {
            targets: 0,
            render: function (data, type, row, meta) {
                return row.thuTu || '';
            }
        },
        {
            targets: 1,
            render: function (data, type, row, meta) {
                return `<div class="group-info">
                    <div class="info-main">${row.tenDiaDiem || ''}</div>
                    <div class="info-sub">${row.diaChi || ''}</div>
                </div>`;
            }
        },
        {
            targets: 2,
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
            targets: 3,
            render: function (data, type, row, meta) {
                return row.thoiDiem ? formatDateTimeDisplay(row.thoiDiem) : 'Chưa xác định';
            }
        },
        {
            targets: 4,
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

    const thuTuInt = parseInt(thuTu);
    if (thuTuInt < 1 || thuTuInt > 255) {
        showNotification(0, 'Thứ tự phải từ 1 đến 255');
        return;
    }

    const existingThuTu = tempLoTrinhs.find((item, index) =>
        item.thuTu == thuTuInt && index !== currentEditingLoTrinhIndex
    );

    if (existingThuTu) {
        showNotification(0, `Thứ tự ${thuTu} đã tồn tại trong lộ trình`);
        return;
    }

    const tenDiaDiem = $("#diaDiemLoTrinhAdd option:selected").text();
    const diaChi = $("#diaDiemLoTrinhAdd option:selected").data('address') || '';

    const loTrinhData = {
        diaDiemID: diaDiemID,
        tenDiaDiem: tenDiaDiem,
        diaChi: diaChi,
        thoiDiem: thoiDiem,
        thuTu: thuTuInt,
        noiDungs: [...tempLoTrinhNoiDungs]
    };

    try {
        if (currentEditingLoTrinhIndex >= 0) {
            tempLoTrinhs[currentEditingLoTrinhIndex] = loTrinhData;
            showNotification(1, 'Cập nhật lộ trình thành công');
        } else {
            tempLoTrinhs.push(loTrinhData);
            showNotification(1, 'Thêm lộ trình thành công');
        }

        tempLoTrinhs.sort((a, b) => a.thuTu - b.thuTu);
        refreshTempLoTrinhTable();
        $('#modalAddLoTrinh').modal('hide');
        resetLoTrinhModal();

    } catch (error) {
        showNotification(0, 'Có lỗi xảy ra khi lưu lộ trình');
    }
}

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

function editTempLoTrinh(index) {
    currentEditingLoTrinhIndex = index;
    const item = tempLoTrinhs[index];

    $("#diaDiemLoTrinhAdd").val(item.diaDiemID);
    $("#thoiDiemLoTrinhAdd").val(item.thoiDiem);
    $("#thuTuLoTrinhAdd").val(item.thuTu);

    tempLoTrinhNoiDungs = [...(item.noiDungs || [])];
    refreshNoiDungTable();

    $('#modalAddLoTrinh').modal('show');
}

function resetLoTrinhModal() {
    $("#formAddLoTrinh")[0].reset();
    currentEditingLoTrinhIndex = -1;
    currentEditingNoiDungIndex = -1;
    tempLoTrinhNoiDungs = [];
    refreshNoiDungTable();
}

// Category Management
function initTempDaPhuongTienTable() {
    const tableDefs = [
        {
            targets: 1,
            render: function (data, type, row, meta) {
                return row.tenTheLoai || '';
            }
        },
        {
            targets: 2,
            render: function (data, type, row, meta) {
                return row.tenLinhVuc || 'Chưa chọn lĩnh vực';
            }
        },
        {
            targets: 3,
            render: function (data, type, row, meta) {
                return row.moTa || '';
            }
        },
        {
            targets: 4,
            render: function (data, type, row, meta) {
                if (row.trangThai || row.SuDung) {
                    return `<span class="TrangThai green-text">Duyệt</span>`;
                } else {
                    return `<span class="TrangThai red-text">Chưa duyệt</span>`;
                }
            }
        },
        {
            targets: 5,
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

// Location Management
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
            if (data.isSuccess && data.value) {
                var newDiaDiemId = data.value.diaDiemID || data.value.id || data.value;

                initDiaDiem_NoAll("#diemDi", "Chọn điểm đi");
                initDiaDiem_NoAll("#diemDen", "Chọn điểm đến");
                initDiaDiem_NoAll("#diaDiemLoTrinhAdd", "Chọn địa điểm");

                $('#modalAddDiaDiem').modal('hide');
                showNotification(1, "Thêm địa điểm mới thành công");
            }
        },
        error: function (err) {
            showNotification(0, "Có lỗi khi thêm địa điểm mới");
        }
    })
}

// Utility Functions
function formatDateTimeDisplay(dateTimeString) {
    if (!dateTimeString) return '';

    try {
        const date = new Date(dateTimeString);
        if (isNaN(date.getTime())) return dateTimeString;

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

// Initialization Functions
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
        showNotification(0, "Lỗi kết nối khi tải danh sách lĩnh vực");
    });
}

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

// API Functions
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

    return $.ajax({
        type: 'POST',
        url: `${baseUrl}/api/TourDuLichApi/TheLoai/ThemMoi`,
        processData: false,
        contentType: false,
        data: formData,
        success: function (response) {
            if (response && response.isSuccess && response.value) {
                showNotification(1, "Thêm mới thể loại thành công");
            } else {
                showNotification(0, response.error || "Thêm mới thể loại không thành công");
            }
        },
        error: function (xhr, status, error) {
            showNotification(0, "Có lỗi xảy ra khi thêm thể loại");
        }
    });
}