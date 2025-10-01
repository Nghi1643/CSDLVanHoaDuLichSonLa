const baseUrl = getRootLink();
let translations = [];

$(document).ready(function () {
    initDatePicker();
    initDanhMucChung_NoAll(1, "#loaiHinh", "") // đúng là 27
    initDiaDiem(); // Khởi tạo danh sách địa điểm
    initNgonNgu("#ngonNguDich")
    initSelect2();

    if (isEdit && coSoAnUongId) {
        setTimeout(function () {
            loadCoSoAnUongData(coSoAnUongId)
        }, 500);
    }

    // Khi thay đổi ngôn ngữ dịch, reset các input trong phần đa ngữ
    $(document).on('change', '#ngonNguDich', function () {
        let maNgonNgu = $(this).val();
        const coSoId = $('#coSoAnUongId').val();

        if (coSoId && maNgonNgu) {
            // Load bản dịch cho ngôn ngữ được chọn
            //loadCoSoTranslation(coSoId, maNgonNgu);
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
            if (validateRating()) {
                dataEdit();
            } else {
                submitBtn.prop('disabled', false);
                submitBtn.text(originalText);
            }
        } catch (error) {
            console.error('Lỗi khi xử lý form:', error);
            submitBtn.prop('disabled', false);
            submitBtn.text(originalText);
        }
    });
});

function dataEdit() {
    // Thông tin cơ bản
    let id = $('#coSoAnUongId').val();
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

    // Validation thủ công cho thời gian
    if (gioMoCua && gioDongCua && gioMoCua >= gioDongCua) {
        showNotification(0, "Giờ mở cửa phải nhỏ hơn giờ đóng cửa");
        return;
    }

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

    if (!diaChi?.trim()) {
        showNotification(0, "Vui lòng nhập địa chỉ");
        $('#diaChi').focus();
        return;
    }

    // Tạo đối tượng cơ sở ăn uống
    let coSoAnUong = {
        "CoSoAnUongID": id,
        "DiaDiemID": diaDiem && diaDiem.trim() !== "" ? diaDiem : null,
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
        "TrangThai": trangThai === "1"
    };

    // Tạo đối tượng nội dung đa ngữ
    let noiDungBanDich = [{
        "MaNgonNgu": ngonNguDich || "vi",
        "TenCoSo": tenCoSo?.trim(),
        "MoTa": moTa?.trim() || null,
        "DiaChi": diaChi?.trim(),
        "NguoiDaiDien": nguoiDaiDien?.trim() || null
    }];

    let formData = new FormData();
    formData.append('CoSoAnUong', JSON.stringify(coSoAnUong));
    formData.append('CoSoAnUong_NoiDung', JSON.stringify(noiDungBanDich));

    $.ajax({
        type: 'PUT',
        url: `${baseUrl}/api/CoSoAnUongApi/ChinhSua/${id}`,
        async: false,
        contentType: false,
        processData: false,
        data: formData,
        success: function (data) {
            if (data.isSuccess && data.value) {
                showNotification(1, "Chỉnh sửa cơ sở ăn uống thành công");
                console.log('Phản hồi từ server:', data);
                setTimeout(() => {
                    window.location.href = `${baseUrl}/AdminTool/CoSoAnUong`;
                }, 1500);
            } else {
                showNotification(0, data.message || "Chỉnh sửa cơ sở ăn uống không thành công");
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
            submitBtn.text(isEdit ? 'Cập nhật' : 'Lưu');
        }
    });
}

function loadCoSoAnUongData(id) {
    console.log('Loading cơ sở ăn uống data:', id);

    // Gọi API để lấy thông tin cơ sở ăn uống
    getDataWithApi('GET', `/api/CoSoAnUongApi/ChiTiet/${id}`).then(data => {
        console.log('Dữ liệu cơ sở ăn uống:', data);
        if (data && data.isSuccess && data.value) {
            const item = data.value;

            // Thông tin chung
            $('#pageTitle').text(`Chỉnh sửa ${item.tenCoSo || 'cơ sở ăn uống'}`);
            $('#coSoAnUongId').val(item.coSoAnUongID);
            $('#sucChua').val(item.sucChua);
            $('#loaiHinh').val(item.loaiDichVuAnUongID).trigger('change');
            $('#soDienThoai').val(item.dienThoai);
            $('#hopThu').val(item.hopThu);
            $('#website').val(item.lienKet);
            $('#gioMoCua').val(item.gioMoCua);
            $('#gioDongCua').val(item.gioDongCua);
            $('#ngayNghi').val(item.ngayNghiHangTuan);
            $('#luotDanhGia').val(item.soLuotDanhGia);
            $('#diemDanhGia').val(item.diemDanhGia);
            $('#thuTu').val(item.thuTu || 1);

            // Địa điểm
            setTimeout(() => {
                $('#diaDiem').val(item.diaDiemID).trigger('change');
            }, 300);

            // Trạng thái
            $(`input[name="trangThai"][value="${item.trangThai}"]`).prop('checked', true);

            // Các tính năng dịch vụ
            $(`input[name="tiecGiaDinh"][value="${item.tiecGiaDinh ? "1" : "0"}"]`).prop('checked', true);
            $(`input[name="tiecDongNguoi"][value="${item.tiecDongNguoi ? "1" : "0"}"]`).prop('checked', true);
            $(`input[name="coPhongRieng"][value="${item.coPhongRieng ? "1" : "0"}"]`).prop('checked', true);
            $(`input[name="coKhuVuiChoiTreEm"][value="${item.coVuiChoiTreEm ? "1" : "0"}"]`).prop('checked', true);
            $(`input[name="chuanDuLich"][value="${item.chuanDuLich ? "1" : "0"}"]`).prop('checked', true);

            // Thông tin đa ngữ (mặc định tiếng Việt)
            $('#tenCoSo').val(item.tenCoSo);
            $('#nguoiDaiDien').val(item.nguoiDaiDien);
            $('#diaChi').val(item.diaChi);
            $('#moTa').val(item.moTa);
            $('#ngonNguDich').val('vi').trigger('change');
        } else {
            showNotification(0, 'Không tìm thấy thông tin cơ sở ăn uống');
        }
    }).catch(error => {
        console.error('Lỗi khi load dữ liệu:', error);
        showNotification(0, 'Có lỗi xảy ra khi tải dữ liệu');
    });
}

//function loadCoSoTranslation(coSoAnUongID, maNgonNgu) {
//    getDataWithApi('GET', `/api/CoSoAnUongApi/BanDich/${coSoAnUongID}/${maNgonNgu}`).then(data => {
//        if (data && data.isSuccess && data.value && data.value.length > 0) {
//            const translation = data.value[0];
//            // Populate các trường dịch với dữ liệu đa ngữ
//            $('#tenCoSo').val(translation.tenCoSo || '');
//            $('#nguoiDaiDien').val(translation.nguoiDaiDien || '');
//            $('#diaChi').val(translation.diaChi || '');
//            $('#moTa').val(translation.moTa || '');
//        } else {
//            // Nếu không có bản dịch, clear các trường dịch
//            $('#tenCoSo').val('');
//            $('#nguoiDaiDien').val('');
//            $('#diaChi').val('');
//            $('#moTa').val('');
//        }
//    }).catch(error => {
//        console.error('Lỗi khi load bản dịch:', error);
//    });
//}

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

                console.log(`Đã load thành công ${response.value.length} địa điểm`);
            } else {
                console.error('API trả về lỗi:', response);
                showNotification(0, "Không thể load danh sách địa điểm");
            }
        },
        error: function (xhr, status, error) {
            console.error('Lỗi khi gọi API DiaDiem:', error);
            showNotification(0, "Lỗi kết nối khi tải danh sách địa điểm");
        }
    });
}

function validateRating() {
    const diemDanhGia = parseFloat($('#diemDanhGia').val());

    if (diemDanhGia && (diemDanhGia < 0 || diemDanhGia > 5)) {
        showNotification(0, "Điểm đánh giá phải nằm trong khoảng 0 đến 5");
        return false;
    }

    return true;
}