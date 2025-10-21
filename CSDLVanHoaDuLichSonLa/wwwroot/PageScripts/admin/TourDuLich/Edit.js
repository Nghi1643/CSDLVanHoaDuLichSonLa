// Global Variables & Constants
const baseUrl = getRootLink();

let tempDaPhuongTiens = [];
let tempLoTrinhs = [];
let tempLoTrinhNoiDungs = [];
let currentEditingDaPhuongTienIndex = -1;
let currentEditingLoTrinhIndex = -1;
let currentEditingNoiDungIndex = -1;
let autoSaveTimer = null;
// Document Ready - Main Initialization
$(document).ready(function () {
    initializeApplication();
    bindEvents();

    // Load dữ liệu tour hiện tại
    if (TourId) {
        setTimeout(() => {
            loadTourData(TourId);
        }, 100);
    }
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
    CheckDateMinMax("#ngayBatDauEdit", "#ngayKetThucEdit");
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
    initNgonNgu("#ngonNguDichEdit");
    initTheLoaiCoSan("#theLoaiCoSanEdit", "Chọn thể loại");
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
    CheckLengthEach('#moTaEdit', 1000, null);
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
    $("#formTourEdit").on("submit", handleFormSubmit);
    $("#btnSaveEdit").on("click", handleSaveClick);

    $(document).on('change', '#ngonNguDichEdit', function () {
        let maNgonNgu = $(this).val();

        if (TourId && maNgonNgu) {
            loadTranslation(TourId, maNgonNgu);
        } else {
            if (!TourId || !maNgonNgu) {
                $('#tenTour').val('');
                $('#thoiGian').val('');
                $('#moTaEdit').val('');
                $('#traiNghiem').val('');
                $('#goiY').val('');
            }
        }
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
            // Nếu có ID (đã lưu trong DB), gọi API xóa
            if (tempLoTrinhs[index].lichTrinhID) {
                deleteLichTrinhFromDB(tempLoTrinhs[index].lichTrinhID, index);
            } else {
                // Nếu chưa lưu DB, chỉ xóa khỏi temp
                tempLoTrinhs.splice(index, 1);
                refreshTempLoTrinhTable();
                showNotification(1, 'Đã xóa lộ trình khỏi danh sách');
            }
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
    $(document).on('change', '#theLoaiCoSanEdit', function () {
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
        dataEdit();
    } catch (error) {
        showNotification(0, "Có lỗi xảy ra khi xử lý dữ liệu");
        showNotification(0, error.message);
        resetSubmitButton();
    }
}

function handleSaveClick(e) {
    e.preventDefault();
    $("#formTourEdit").submit();
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
                        initTheLoaiCoSan("#theLoaiCoSanEdit", "Chọn thể loại");
                    }, 200);
                });
                resetDaPhuongTienModal();
            }
        })
        .fail(function (xhr, status, error) {
            showNotification(0, 'Có lỗi xảy ra khi thêm thể loại');
        });
}

// Load dữ liệu tour hiện tại
function loadTourData(tourId) {
    getDataWithApi('GET', `/api/TourDuLichApi/ChiTiet/${tourId}`).then(data => {
        if (data && data.isSuccess && data.value) {
            const tour = data.value;
            populateFormData(tour);

            // Load lộ trình và thể loại
            loadItineraries(tourId);
            loadCategories(tourId);
        }
    }).catch(error => {
        console.error('Lỗi khi tải dữ liệu tour:', error);
        showNotification(0, 'Không thể tải dữ liệu tour');
    });
}

function populateFormData(tour) {
    // Populate form fields với dữ liệu tour
    $('#maDinhDanh').val(tour.maDinhDanh || '');
    $('#toChuc').val(tour.toChucID || '').trigger('change');
    $('#phanKhuc').val(tour.phanKhucID || '').trigger('change');
    $('#loaiHinh').val(tour.loaiHinhID || '').trigger('change');
    $('#diemDi').val(tour.diemDiID || '').trigger('change');
    $('#diemDen').val(tour.diemDenID || '').trigger('change');
    $('#phuongTien').val(tour.phuongTienID || '').trigger('change');
    $('#ngonNguDich').val(tour.maNgonNgu || '').trigger('change');

    if (tour.ngayKhoiHanh) {
        $('#ngayBatDauEdit').val(formatDateForInput(tour.ngayKhoiHanh));
    }
    if (tour.ngayKetThuc) {
        $('#ngayKetThucEdit').val(formatDateForInput(tour.ngayKetThuc));
    }

    $('#soKhachToiThieu').val(tour.soKhachToiThieu || '');
    $('#soKhachToiDa').val(tour.soKhachToiDa || '');
    $('#giaTour').val(tour.giaTour || '');
    $('#giaNguoi').val(tour.giaNguoi || '');
    $('#soLuotDanhGia').val(tour.soLuotDanhGia || '');
    $('#diemDanhGia').val(tour.diemDanhGia || '');

    $(`input[name='trangThai'][value='${tour.trangThai ? 1 : 0}']`).prop('checked', true);

    // Load nội dung đa ngữ
    $('#ngonNguDichEdit').val(tour.maNgonNgu || 'vi');
    $('#tenTour').val(tour.tenTour || '');
    $('#thoiGian').val(tour.thoiGian || '');
    $('#moTaEdit').val(tour.moTa || '');
    $('#traiNghiem').val(tour.traiNghiem || '');
    $('#goiY').val(tour.goiY || '');

    //$('#ngonNguDichEdit').val('vi').trigger('change');
    //setTimeout(() => {
    //    if (tour.maNgonNgu) {
    //        $('#ngonNguDichEdit').val(tour.maNgonNgu).trigger('change');
    //    }
    //}, 300);
}
function loadTranslation(tourID, maNgonNgu) {
    getDataWithApi('GET', `/api/TourDuLichApi/BanDich/${tourID}/${maNgonNgu}`).then(data => {
        console.log(data, "Load bản dịch tour du lịch");
        if (data && data.isSuccess && data.value) {
            const translation = data.value;
            // Populate các trường dịch với dữ liệu đa ngữ
            $("#tenTour").val(translation.tenTour || '');
            $("#thoiGian").val(translation.thoiGian || '');
            $("#moTaEdit").val(translation.moTa || '');
            $("#traiNghiem").val(translation.traiNghiem || '');
            $("#goiY").val(translation.goiY || '');
        } else {
            // Nếu không có bản dịch, clear các trường dịch
            $("#tenTour").val('');
            $("#thoiGian").val('');
            $("#moTaEdit").val('');
            $("#traiNghiem").val('');
            $("#goiY").val('');
        }
    }).catch(error => {
        console.error('Lỗi khi tải bản dịch:', error);
    });
}
function loadItineraries(tourId) {
    getDataWithApi('GET', `/api/TourDuLichApi/LichTrinh/DanhSach/${tourId}`).then(data => {
        if (data && data.isSuccess && data.value) {
            const groupedData = {};

            data.value.forEach(item => {
                const key = `${item.lichTrinhID}_${item.thuTu}`;

                if (!groupedData[key]) {
                    // Tạo nhóm mới cho lộ trình
                    groupedData[key] = {
                        lichTrinhID: item.lichTrinhID,
                        diaDiemID: item.diaDiemID,
                        tenDiaDiem: item.tenDiaDiem,
                        diaChi: item.diaChi,
                        thoiDiem: item.thoiDiem,
                        thuTu: item.thuTu,
                        noiDungs: []
                    };
                }

                // Thêm hoạt động vào nhóm (nếu có hoạt động)
                if (item.hoatDong && item.hoatDong.trim()) {
                    groupedData[key].noiDungs.push({
                        hoatDong: item.hoatDong,
                        soGio: item.soGio
                    });
                }
            });

            tempLoTrinhs = Object.values(groupedData).sort((a, b) => a.thuTu - b.thuTu);

            console.log('Dữ liệu lộ trình đã nhóm:', tempLoTrinhs);
            refreshTempLoTrinhTable();
        }
    }).catch(error => {
        console.error('Lỗi khi tải lộ trình:', error);
    });
}

function loadCategories(tourId) {
    // Load thể loại tour hiện tại
    // Có thể cần API riêng để lấy thể loại của tour
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

    if (!$("#ngonNguDichEdit").val()) {
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
    const submitBtn = $('#formTourEdit').find('button[type="submit"]');
    const originalText = submitBtn.data('original-text') || 'Cập nhật';
    submitBtn.prop('disabled', false);
    submitBtn.text(originalText);
}

// Main Data Processing
function dataEdit() {
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

    const NgayBatDau = $("#ngayBatDauEdit").val() ? $("#ngayBatDauEdit").val().split('/').reverse().join('-') : null;
    const NgayKetThuc = $("#ngayKetThucEdit").val() ? $("#ngayKetThucEdit").val().split('/').reverse().join('-') : null;
    const TrangThai = $("input[name='trangThai']:checked").val() == "1";

    const MaNgonNgu = $("#ngonNguDichEdit").val() || "vi";
    const tenTour = $("#tenTour").val().trim();
    const thoiGian = $("#thoiGian").val().trim();
    const MoTa = $("#moTaEdit").val().trim();
    const traiNghiem = $("#traiNghiem").val().trim();
    const goiY = $("#goiY").val().trim();

    const tourDuLichObject = {
        TourID: TourId,
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
        TourID: TourId,
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
        type: 'PUT',
        url: `${baseUrl}/api/TourDuLichApi/ChinhSua/${TourId}`,
        async: false,
        contentType: false,
        processData: false,
        data: formData,
        success: function (data) {
            if (data.isSuccess && data.value) {
                // Xử lý lộ trình
                processItineraries(TourId);

                setTimeout(() => {
                    window.location.href = `${baseUrl}/AdminTool/TourDuLich`;
                }, 1500);

            } else {
                showNotification(0, data.message || "Cập nhật tour du lịch không thành công");
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
                LichTrinhID: loTrinh.lichTrinhID || 0,
                TourID: tourID,
                DiaDiemID: loTrinh.diaDiemID,
                ThoiDiem: convertThoiDiem(loTrinh.thoiDiem),
                ThuTu: loTrinh.thuTu
            },
            NoiDungs: loTrinh.noiDungs && loTrinh.noiDungs.length > 0
                ? loTrinh.noiDungs.map(nd => ({
                    LichTrinhID: loTrinh.lichTrinhID || null,
                    HoatDong: nd.hoatDong || '',
                    SoGio: nd.soGio || ''
                }))
                : [{
                    LichTrinhID: loTrinh.lichTrinhID || null,
                    HoatDong: '',
                    SoGio: ''
                }]
        }));
        //alert('Dữ liệu tạm thời: ' + JSON.stringify(tempLoTrinhs));
        //alert('Dữ liệu lộ trình gửi lên server: ' + JSON.stringify(danhSachLichTrinh));
        const result = updateAllLichTrinhSync(danhSachLichTrinh);

        if (result && result.isSuccess) {
            const successCount = result.value ? result.value.length : 0;
            showNotification(1, `Cập nhật tour du lịch thành công với ${successCount}/${tempLoTrinhs.length} lộ trình`);
        } else {
            showNotification(1, "Cập nhật tour du lịch thành công nhưng không thể cập nhật lộ trình");
        }
    } else {
        showNotification(1, "Cập nhật tour du lịch thành công");
    }
}

function updateAllLichTrinhSync(danhSachLichTrinh) {
   
    let formData = new FormData();
    formData.append('DanhSachLichTrinh', JSON.stringify(danhSachLichTrinh));
    //alert('tour id: '+TourId)
    let result = null;
    $.ajax({
        type: 'PUT',
        url: `${baseUrl}/api/TourDuLichApi/LichTrinh/ChinhSua/${TourId}`,
        async: false,
        processData: false,
        contentType: false,
        data: formData,
        success: function (response) {
            //alert('success'+JSON.stringify(danhSachLichTrinh))
            result = response;
        },
        error: function (xhr, status, error) {
            //alert('eror'+JSON.stringify(danhSachLichTrinh))
            result = { isSuccess: false, error: error };
        }
    });

    return result;
}

function deleteLichTrinhFromDB(lichTrinhID, index) {
    $.ajax({
        type: 'DELETE',
        url: `${baseUrl}/api/TourDuLichApi/LichTrinh/Xoa/${lichTrinhID}`,
        success: function (response) {
            if (response && response.isSuccess) {
                tempLoTrinhs.splice(index, 1);
                refreshTempLoTrinhTable();
                showNotification(1, 'Đã xóa lộ trình thành công');
            } else {
                showNotification(0, 'Không thể xóa lộ trình');
            }
        },
        error: function (xhr, status, error) {
            showNotification(0, 'Có lỗi xảy ra khi xóa lộ trình');
        }
    });
}

function convertThoiDiem(thoiDiem) {
    return thoiDiem && thoiDiem.includes('/') ?
        thoiDiem.split('/').reverse().join('-') :
        thoiDiem;
}

function loadTranslation(tourID, maNgonNgu) {
    getDataWithApi('GET', `/api/TourDuLichApi/BanDich/${tourID}/${maNgonNgu}`).then(data => {
        if (data && data.isSuccess && data.value) {
            const translation = data.value;
            // Populate các trường dịch với dữ liệu đa ngữ
            $("#tenTour").val(translation.tenTour || '');
            $("#thoiGian").val(translation.thoiGian || '');
            $("#moTaEdit").val(translation.moTa || '');
            $("#traiNghiem").val(translation.traiNghiem || '');
            $("#goiY").val(translation.goiY || '');
        } else {
            // Nếu không có bản dịch, clear các trường dịch
            $("#tenTour").val('');
            $("#thoiGian").val('');
            $("#moTaEdit").val('');
            $("#traiNghiem").val('');
            $("#goiY").val('');
        }
    }).catch(error => {
        console.error('Lỗi khi tải bản dịch:', error);
    });
}

// Utility Functions
function formatDateForInput(dateString) {
    if (!dateString) return '';

    const date = new Date(dateString);
    if (isNaN(date.getTime())) return '';

    const day = date.getDate().toString().padStart(2, '0');
    const month = (date.getMonth() + 1).toString().padStart(2, '0');
    const year = date.getFullYear();

    return `${day}/${month}/${year}`;
}

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

// Itinerary Management (tương tự Add.js)
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
        lichTrinhID: currentEditingLoTrinhIndex >= 0 ? tempLoTrinhs[currentEditingLoTrinhIndex].lichTrinhID : null,
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

// Category Management (tương tự Add.js)
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

// Location Management (tương tự Add.js)
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

// Initialization Functions (tương tự Add.js)
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

function initTheLoaiCoSan(elementId = "#theLoaiCoSanEdit", placeholder = "Chọn thể loại") {
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

function initToChuc_NoAll(loaiToChucID, elementId = "#toChuc", placeholder = "Chọn tổ chức") {
    const requestData = JSON.stringify({
        loaiToChucID: loaiToChucID,
        maNgonNgu: "vi",
        trangThai: true
    });

    getDataWithApi('POST', '/api/ToChucApi/DanhSach', requestData).then(response => {
        if (response && response.isSuccess && response.value) {
            $(elementId).empty().append(`<option value="">${placeholder}</option>`);

            response.value.forEach(item => {
                const displayText = item.tenToChuc || item.noiDung || 'Không rõ tên';
                $(elementId).append(`<option value="${item.toChucID || item.id}">${displayText}</option>`);
            });
        } else {
            showNotification(0, "Không thể load danh sách tổ chức");
        }
    }).catch(error => {
        console.error('Lỗi khi load tổ chức:', error);
        showNotification(0, "Lỗi kết nối khi tải danh sách tổ chức");
    });
}

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

// Load thể loại hiện tại của tour
function loadCategories(tourId) {
    // Load thể loại tour hiện tại từ API
    getDataWithApi('GET', `/api/TourDuLichApi/ChiTiet/${tourId}`).then(data => {
        if (data && data.isSuccess && data.value && data.value.theLoaiID) {
            // Nếu tour đã có thể loại, load thông tin thể loại
            loadCategoryDetails(data.value.theLoaiID);
        }
    }).catch(error => {
        console.error('Lỗi khi tải thể loại tour:', error);
    });
}

function loadCategoryDetails(theLoaiID) {
    getDataWithApi('GET', `/api/TourDuLichApi/TheLoai/ChiTiet/${theLoaiID}?maNgonNgu=vi`).then(data => {
        if (data && data.isSuccess && data.value) {
            const theLoai = data.value;

            // Tạo object thể loại và thêm vào temp
            const theLoaiData = {
                theLoaiID: theLoai.theLoaiID,
                tenTheLoai: theLoai.tenTheLoai,
                moTa: theLoai.moTa,
                linhVucIDs: theLoai.linhVucIDs || [],
                tenLinhVuc: theLoai.tenLinhVuc || '',
                trangThai: theLoai.trangThai,
                isExisting: true
            };

            tempDaPhuongTiens = [theLoaiData];
            refreshTempDaPhuongTienTable();
        }
    }).catch(error => {
        console.error('Lỗi khi tải chi tiết thể loại:', error);
    });
}

// Xử lý modal và reset form
function resetModal(modal) {
    $(modal).find('form')[0].reset();
    $(modal).find('select').val('').trigger('change');
    $(modal).find('input[type="radio"]:first').prop('checked', true);
}

// Xử lý các function validation và format date
function formatDateToSaveAsString(dateString) {
    if (!dateString) return null;

    // Nếu đã là format yyyy-mm-dd thì return luôn
    if (dateString.includes('-') && dateString.split('-')[0].length === 4) {
        return dateString;
    }

    // Nếu là format dd/mm/yyyy thì convert
    if (dateString.includes('/')) {
        const parts = dateString.split('/');
        if (parts.length === 3) {
            return `${parts[2]}-${parts[1].padStart(2, '0')}-${parts[0].padStart(2, '0')}`;
        }
    }

    return dateString;
}

function formatDateWithoutTime(dateString) {
    if (!dateString) return '';

    const date = new Date(dateString);
    if (isNaN(date.getTime())) return '';

    const day = date.getDate().toString().padStart(2, '0');
    const month = (date.getMonth() + 1).toString().padStart(2, '0');
    const year = date.getFullYear();

    return `${day}/${month}/${year}`;
}

// Thêm các function xử lý file upload và preview (nếu cần)
function previewPicture() {
    // Xử lý preview hình ảnh nếu có
    $(document).on('change', 'input[type="file"]', function () {
        const file = this.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = function (e) {
                const preview = $(this).closest('.form-group').find('.image-preview');
                if (preview.length) {
                    preview.attr('src', e.target.result);
                }
            }.bind(this);
            reader.readAsDataURL(file);
        }
    });
}

// Xử lý media data switching
function switchMediaData() {
    $(document).on('change', 'input[name="loaiMediaDPT"]', function () {
        const mediaType = $(this).val();
        const fileInput = $('#fileDPT');
        const urlInput = $('#duongDanFileDPT');

        // Reset cả hai input
        fileInput.val('');
        urlInput.val('');

        // Hiển thị/ẩn input tương ứng
        if (mediaType === '4') { // URL
            $('.file-upload-section').hide();
            $('.url-input-section').show();
        } else { // File upload
            $('.file-upload-section').show();
            $('.url-input-section').hide();
        }
    });
}

// Xử lý select2 cho các dropdown có search
function initSelect2Advanced(selector, options = {}) {
    const defaultOptions = {
        allowClear: true,
        width: '100%',
        placeholder: 'Chọn...'
    };

    $(selector).select2(Object.assign(defaultOptions, options));
}

// Xử lý validation cho các field số
function validateNumericInput(selector, min = 0, max = null) {
    $(selector).on('input', function () {
        let value = parseFloat($(this).val());

        if (isNaN(value)) {
            $(this).val('');
            return;
        }

        if (value < min) {
            $(this).val(min);
            showNotification(0, `Giá trị không được nhỏ hơn ${min}`);
        }

        if (max !== null && value > max) {
            $(this).val(max);
            showNotification(0, `Giá trị không được lớn hơn ${max}`);
        }
    });
}

// Xử lý keyboard shortcuts
function bindKeyboardShortcuts() {
    $(document).on('keydown', function (e) {
        // Ctrl + S để lưu
        if (e.ctrlKey && e.which === 83) {
            e.preventDefault();
            $('#btnSaveEdit').click();
        }

        // Escape để đóng modal
        if (e.which === 27) {
            $('.modal').modal('hide');
        }
    });
}

// Xử lý auto-save (nếu cần)
function setupAutoSave() {
    let autoSaveTimer;
    const autoSaveDelay = 30000; // 30 giây

    $('input, textarea, select').on('change input', function () {
        clearTimeout(autoSaveTimer);
        autoSaveTimer = setTimeout(function () {
            // Thực hiện auto-save ở đây nếu cần
            console.log('Auto-save triggered');
        }, autoSaveDelay);
    });
}

// Xử lý loading states
function showLoadingState(element, text = 'Đang xử lý...') {
    const originalText = $(element).text();
    $(element).data('original-text', originalText);
    $(element).prop('disabled', true).text(text);
}

function hideLoadingState(element) {
    const originalText = $(element).data('original-text') || 'Lưu';
    $(element).prop('disabled', false).text(originalText);
}

// Xử lý confirmation dialogs
function showConfirmDialog(message, callback) {
    if (confirm(message)) {
        callback();
    }
}

// Xử lý error logging
function logError(error, context = '') {
    console.error(`Error in ${context}:`, error);

    // Có thể gửi error log về server nếu cần
    // sendErrorToServer(error, context);
}

// Xử lý browser back button
function handleBrowserNavigation() {
    window.addEventListener('beforeunload', function (e) {
        // Kiểm tra xem có thay đổi chưa lưu không
        if (hasUnsavedChanges()) {
            e.preventDefault();
            e.returnValue = 'Bạn có thay đổi chưa được lưu. Bạn có chắc chắn muốn rời khỏi trang?';
        }
    });
}

function hasUnsavedChanges() {
    // Logic kiểm tra thay đổi chưa lưu
    // Có thể so sánh giá trị hiện tại với giá trị ban đầu
    return false; // Placeholder
}

// Xử lý responsive design
function handleResponsiveLayout() {
    function checkScreenSize() {
        if ($(window).width() < 768) {
            // Mobile layout
            $('.desktop-only').hide();
            $('.mobile-only').show();
        } else {
            // Desktop layout
            $('.desktop-only').show();
            $('.mobile-only').hide();
        }
    }

    $(window).on('resize', checkScreenSize);
    checkScreenSize(); // Initial check
}

// Khởi tạo tooltip và popover
function initTooltipsAndPopovers() {
    $('[data-toggle="tooltip"]').tooltip({
        container: 'body',
        delay: { show: 500, hide: 100 }
    });

    $('[data-toggle="popover"]').popover({
        container: 'body',
        trigger: 'hover'
    });
}

// Cleanup function khi rời khỏi trang
function cleanup() {
    // Destroy DataTables
    if ($.fn.DataTable.isDataTable('#dataGridDiaDiem')) {
        $('#dataGridDiaDiem').DataTable().destroy();
    }
    if ($.fn.DataTable.isDataTable('#dataGridDaPhuongTien')) {
        $('#dataGridDaPhuongTien').DataTable().destroy();
    }

    // Destroy tooltips and popovers
    $('[data-toggle="tooltip"]').tooltip('dispose');
    $('[data-toggle="popover"]').popover('dispose');

    // Clear timers
    clearTimeout(autoSaveTimer);

    // Unbind events
    $(document).off('keydown');
    $(window).off('resize');
}

// Bind cleanup khi rời khỏi trang
$(window).on('beforeunload', cleanup);

// Thêm các utility functions
const Utils = {
    formatCurrency: function (amount) {
        if (amount == null || amount === '') return '';
        return Number(amount).toLocaleString('vi-VN', {
            style: 'currency',
            currency: 'VND'
        });
    },

    formatNumber: function (number) {
        if (number == null || number === '') return '';
        return Number(number).toLocaleString('vi-VN');
    },

    validateEmail: function (email) {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return emailRegex.test(email);
    },

    validatePhone: function (phone) {
        const phoneRegex = /^[0-9]{10,11}$/;
        return phoneRegex.test(phone.replace(/\s/g, ''));
    },

    debounce: function (func, wait) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    },

    throttle: function (func, limit) {
        let inThrottle;
        return function () {
            const args = arguments;
            const context = this;
            if (!inThrottle) {
                func.apply(context, args);
                inThrottle = true;
                setTimeout(() => inThrottle = false, limit);
            }
        };
    }
};

// Export các function cần thiết nếu sử dụng module system
if (typeof module !== 'undefined' && module.exports) {
    module.exports = {
        loadTourData,
        dataEdit,
        Utils
    };
}

// Khởi tạo các component bổ sung khi document ready
$(document).ready(function () {
    // Khởi tạo các component bổ sung
    initTooltipsAndPopovers();
    bindKeyboardShortcuts();
    handleResponsiveLayout();

    // Setup validation cho các input số
    validateNumericInput('#soKhachToiThieu', 1, 1000);
    validateNumericInput('#soKhachToiDa', 1, 1000);
    validateNumericInput('#giaTour', 0);
    validateNumericInput('#giaNguoi', 0);
    validateNumericInput('#diemDanhGia', 1, 5);

    // Setup auto-save nếu cần
    // setupAutoSave();
});