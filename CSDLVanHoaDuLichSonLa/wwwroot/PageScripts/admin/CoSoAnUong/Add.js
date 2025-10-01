const baseUrl = getRootLink();

$(document).ready(function () {
    initDatePicker();
    initDanhMucChung_NoAll(27, "#loaiHinh", "") // đúng là 27
    initDiaDiem(); // Khởi tạo danh sách địa điểm
    initNgonNgu("#ngonNguDich")
    initSelect2();

    // Khi thay đổi ngôn ngữ dịch, reset các input trong phần đa ngữ
    $(document).on('change', '#ngonNguDich', function () {
        $('#tenCoSo').val('');
        $('#nguoiDaiDien').val('');
        $('#diaChi').val('');
        $('#moTa').val('');
    });

    $("#formToChuc").on("submit", function (e) {
        e.preventDefault();

        const submitBtn = $(this).find('button[type="submit"]');
        if (submitBtn.prop('disabled')) {
            return;
        }

        submitBtn.prop('disabled', true);
        const originalText = submitBtn.text();
        submitBtn.text('Đang xử lý...');

        try {
            // Validate trước khi submit
            if (validateRating()) {
                dataAdd();
            } else {
                submitBtn.prop('disabled', false);
                submitBtn.text(originalText);
            }
        } catch (error) {
            console.error('Lỗi khi xử lý form:', error);
            showNotification(0, "Có lỗi xảy ra khi xử lý dữ liệu");
            submitBtn.prop('disabled', false);
            submitBtn.text(originalText);
        }
    });

    $("#ngonNguDich").on("change", function () {
        let maNgonNgu = $(this).val();
        const coSoId = $('#coSoId').val();

        if (coSoId && maNgonNgu) {
            console.log('Chuyển ngôn ngữ dịch cho cơ sở hiện tại:', maNgonNgu);
            // Load bản dịch cho ngôn ngữ được chọn
            loadCoSoTranslation(coSoId, maNgonNgu);
        } else {
            // Nếu chưa có cơ sở (tạo mới) hoặc chưa chọn ngôn ngữ, clear các trường dịch
            if (!coSoId) {
                $('#tenCoSo').val('');
                $('#nguoiDaiDien').val('');
                $('#diaChi').val('');
                $('#moTa').val('');
            }
        }
    });
});

function dataAdd() {
    // Thông tin cơ bản
    let sucChua = $('#sucChua').val();
    let loaiHinh = $('#loaiHinh').val();
    let soDienThoai = $('#soDienThoai').val();
    let hopThu = $('#hopThu').val();
    let website = $('#website').val();
    let gioMoCua = $('#gioMoCua').val();
    let gioDongCua = $('#gioDongCua').val();
    let ngayNghi = $('#ngayNghi').val();
    let diaDiem = $('#diaDiem').val(); 
    let luotDanhGia = $('#luotDanhGia').val();
    let diemDanhGia = $('#diemDanhGia').val();
    let thuTu = $('#thuTu').val();
    let trangThai = $("input[name='trangThai']:checked").val();

    // Các tính năng dịch vụ
    let tiecGiaDinh = $("input[name='tiecGiaDinh']:checked").val();
    let tiecDongNguoi = $("input[name='tiecDongNguoi']:checked").val();
    let coPhongRieng = $("input[name='coPhongRieng']:checked").val();
    let coKhuVuiChoiTreEm = $("input[name='coKhuVuiChoiTreEm']:checked").val();
    let chuanDuLich = $("input[name='chuanDuLich']:checked").val();

    // Thông tin đa ngữ
    let ngonNguDich = $('#ngonNguDich').val();
    let tenCoSo = $('#tenCoSo').val();
    let nguoiDaiDien = $('#nguoiDaiDien').val();
    let diaChi = $('#diaChi').val();
    let moTa = $('#moTa').val();

    // Validation
    if (!tenCoSo?.trim()) {
        showNotification(0, "Vui lòng nhập tên cơ sở");
        $('#tenCoSo').focus();
        return;
    }

    if (!loaiHinh) {
        showNotification(0, "Vui lòng chọn loại hình dịch vụ");
        $('#loaiHinh').focus();
        return;
    }

    if (!diaDiem) {
        showNotification(0, "Vui lòng chọn địa điểm");
        $('#diaDiem').focus();
        return;
    }

    if (!diaChi?.trim()) {
        showNotification(0, "Vui lòng nhập địa chỉ");
        $('#diaChi').focus();
        return;
    }

    // Tạo đối tượng cơ sở ăn uống
    let coSoAnUong = {
        "CoSoAnUongID": "00000000-0000-0000-0000-000000000000", // GUID empty cho thêm mới
        "DiaDiemID": diaDiem,
        "LoaiDichVuAnUongID": parseInt(loaiHinh),
        "SoLuotDanhGia": luotDanhGia ? parseInt(luotDanhGia) : null,
        "DiemDanhGia": diemDanhGia ? parseFloat(diemDanhGia) : null,
        "SucChua": sucChua ? parseInt(sucChua) : null,
        "TiecGiaDinh": tiecGiaDinh === "1",
        "TiecDongNguoi": tiecDongNguoi === "1",
        "CoPhongRieng": coPhongRieng === "1",
        "CoVuiChoiTreEm": coKhuVuiChoiTreEm === "1",
        "ChuanDuLich": chuanDuLich === "1",
        "DienThoai": soDienThoai?.trim() || null,
        "HopThu": hopThu?.trim() || null,
        "LienKet": website?.trim() || null,
        "NgayNghiHangTuan": ngayNghi?.trim() || null,
        "GioMoCua": gioMoCua || null,
        "GioDongCua": gioDongCua || null,
        "TrangThai": parseInt(trangThai) || 1
    };

    // Tạo đối tượng nội dung đa ngữ
    let noiDungBanDich = [{
        "MaNgonNgu": ngonNguDich || "vi",
        "TenCoSo": tenCoSo?.trim(),
        "MoTa": moTa?.trim() || null,
        "DiaChi": diaChi?.trim(),
        "NguoiDaiDien": nguoiDaiDien?.trim() || null
    }];

    //console.log('Dữ liệu cơ sở ăn uống:', coSoAnUong);
    //console.log('Dữ liệu nội dung bản dịch:', noiDungBanDich);


    let formData = new FormData();
    formData.append('CoSoAnUong', JSON.stringify(coSoAnUong));
    formData.append('CoSoAnUong_NoiDung', JSON.stringify(noiDungBanDich));

    $.ajax({
        type: 'POST',
        url: `${baseUrl}/api/CoSoAnUongApi/ThemMoi`,
        async: false,
        contentType: false,
        processData: false,
        data: formData,
        success: function (data) {
            if (data.isSuccess && data.value) {
                showNotification(1, "Thêm mới cơ sở ăn uống thành công");
                console.log('Phản hồi từ server:', data);
                setTimeout(() => {
                    window.location.href = `${baseUrl}/AdminTool/CoSoAnUong`;
                }, 1500);
            } else {
                showNotification(0, data.message || "Thêm mới cơ sở ăn uống không thành công");
                console.error('Lỗi từ server:', data.error || data.message);
            }
        },
        error: function (xhr, status, error) {
            console.error('Lỗi AJAX:', error);
            console.error('Chi tiết lỗi:', xhr.responseText);
            showNotification(0, "Có lỗi xảy ra khi kết nối đến server");
        },
        complete: function () {
            // Re-enable submit button
            const submitBtn = $('#formToChuc').find('button[type="submit"]');
            submitBtn.prop('disabled', false);
            submitBtn.text('Lưu');
        }
    });
}

// Function load bản dịch cho cơ sở (nếu cần thiết cho chức năng edit)
//function loadCoSoTranslation(coSoId, maNgonNgu) {
//    getDataWithApi('GET', `/api/CoSoAnUongApi/BanDich/${coSoId}/${maNgonNgu}`)
//        .then(data => {
//            if (data && data.isSuccess && data.value) {
//                const translation = data.value;
//                $('#tenCoSo').val(translation.tenCoSo || '');
//                $('#nguoiDaiDien').val(translation.nguoiDaiDien || '');
//                $('#diaChi').val(translation.diaChi || '');
//                $('#moTa').val(translation.moTa || '');
//            }
//        })
//        .catch(error => {
//            console.error('Lỗi khi load bản dịch:', error);
//        });
//}

// Validation functions
//function validateTimeRange() {
//    const gioMoCua = $('#gioMoCua').val();
//    const gioDongCua = $('#gioDongCua').val();

//    if (gioMoCua && gioDongCua) {
//        if (gioMoCua >= gioDongCua) {
//            showNotification(0, "Giờ mở cửa phải nhỏ hơn giờ đóng cửa");
//            return false;
//        }
//    }
//    return true;
//}

function validateRating() {
    const diemDanhGia = parseFloat($('#diemDanhGia').val());

    if (diemDanhGia && (diemDanhGia < 0 || diemDanhGia > 5)) {
        showNotification(0, "Điểm đánh giá phải nằm trong khoảng 0 đến 5");
        return false;
    }

    return true;
}
function initDiaDiem() {
    const requestData = {
        MaNgonNgu: "vi", 
        TrangThai: true 
    };

    $.ajax({
        type: 'POST',
        url: `${baseUrl}/api/DiaDiemApi/DanhSach`,
        contentType: 'application/json',
        data: JSON.stringify(requestData),
        success: function (response) {
            if (response && response.isSuccess && response.value) {
                $("#diaDiem").empty().append(`<option value="">Chọn địa điểm</option>`);

                response.value.forEach(item => {
                    const displayText = item.tenDiaDiem || 'Không rõ tên';
                    const addressText = item.diaChi ? ` - ${item.diaChi}` : '';
                    const fullText = displayText + addressText;

                    $("#diaDiem").append(`<option value="${item.diaDiemID}">${fullText}</option>`);
                });

            } else {
              
                showNotification(0, "Không thể load danh sách địa điểm");
            }
        },
        error: function (xhr, status, error) {
           
            showNotification(0, "Lỗi kết nối khi tải danh sách địa điểm");
        }
    });
}