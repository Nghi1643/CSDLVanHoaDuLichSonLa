const baseUrl = getRootLink();

$(document).ready(function () {
    if (coSoAnUongId) {
        loadCoSoAnUongData(coSoAnUongId);
    }
});

function loadCoSoAnUongData(id) {
    console.log('Loading cơ sở ăn uống data:', id);

    // Gọi API để lấy thông tin cơ sở ăn uống
    getDataWithApi('GET', `/api/CoSoAnUongApi/ChiTiet/${id}`).then(data => {
        console.log('Dữ liệu cơ sở ăn uống:', data);
        if (data && data.isSuccess && data.value) {
            displayCoSoAnUongInfo(data.value);
            updatePageTitle(data.value);
        } else {
            showNotification(0, 'Không tìm thấy thông tin cơ sở ăn uống');
        }
    }).catch(error => {
        console.error('Lỗi khi tải dữ liệu:', error);
        showNotification(0, 'Có lỗi xảy ra khi tải dữ liệu');
    });
}

function displayCoSoAnUongInfo(data) {
    // Hiển thị thông tin cơ bản
    $('#tenLoaiDichVuAnUong').text(data.tenLoaiDichVuAnUong || '-');
    $('#tenDiaDiem').text(data.tenDiaDiem || '-');
    $('#sucChua').text(data.sucChua ? `${data.sucChua} người` : '-');
    $('#dienThoai').text(data.dienThoai || '-');
    $('#hopThu').text(data.hopThu || '-');

    // Hiển thị website với link nếu có
    if (data.lienKet) {
        $('#lienKet').html(`<a href="${data.lienKet}" target="_blank" class="text-primary">${data.lienKet}</a>`);
    } else {
        $('#lienKet').text('-');
    }

    // Hiển thị giờ hoạt động
    $('#gioMoCua').text(formatTime(data.gioMoCua) || '-');
    $('#gioDongCua').text(formatTime(data.gioDongCua) || '-');
    $('#ngayNghiHangTuan').text(data.ngayNghiHangTuan || '-');

    // Hiển thị đánh giá
    $('#soLuotDanhGia').text(data.soLuotDanhGia || '0');
    $('#diemDanhGia').text(data.diemDanhGia ? `${data.diemDanhGia}/5` : '-');

    // Hiển thị trạng thái
    if (data.trangThai === true) {
        $('#trangThai').html('<span class="TrangThai green-text">Duyệt</span>');
    } else {
        $('#trangThai').html('<span class="TrangThai red-text">Chưa duyệt</span>');
    }

    // Hiển thị tính năng dịch vụ
    displayServiceFeature('#tiecGiaDinh', data.tiecGiaDinh);
    displayServiceFeature('#tiecDongNguoi', data.tiecDongNguoi);
    displayServiceFeature('#coPhongRieng', data.coPhongRieng);
    displayServiceFeature('#coVuiChoiTreEm', data.coVuiChoiTreEm);
    displayServiceFeature('#chuanDuLich', data.chuanDuLich);

    // Hiển thị thông tin đa ngữ
    $('#tenCoSo').text(data.tenCoSo || '-');
    $('#nguoiDaiDien').text(data.nguoiDaiDien || '-');
    $('#diaChi').text(data.diaChi || '-');
    $('#moTa').text(data.moTa || '-');

    // Hiển thị thông tin người tạo/cập nhật
    $('#nguoiTao').text(data.nguoiTao || '-');
    $('#ngayTao').text(data.ngayTao ? formatDate(data.ngayTao) : '-');
    $('#nguoiCapNhat').text(data.nguoiCapNhat || '-');
    $('#ngayCapNhat').text(data.ngayCapNhat ? formatDate(data.ngayCapNhat) : '-');
}

function displayServiceFeature(selector, value) {
    if (value === true) {
        $(selector).html('<span class="badge badge-success">Có</span>');
    } else if (value === false) {
        $(selector).html('<span class="badge badge-secondary">Không</span>');
    } else {
        $(selector).text('-');
    }
}

function updatePageTitle(data) {
    let title = `Chi tiết ${data.tenCoSo || 'cơ sở ăn uống'}`;
    let htmlTitle = ``;
    if (data.tenCoSo) {
        htmlTitle = `<div class='mainTitle'>${data.tenCoSo}</div>`;
        if (data.tenLoaiDichVuAnUong) {
            htmlTitle += `<div class='subTitle'>(${data.tenLoaiDichVuAnUong})</div>`;
        }
    }
    $('#pageTitle').html(htmlTitle);
    document.title = title;
}

function formatDate(dateString) {
    if (!dateString) return '-';
    try {
        const date = new Date(dateString);
        return date.toLocaleDateString('vi-VN');
    } catch (error) {
        return dateString;
    }
}

function formatTime(timeString) {
    if (!timeString) return '-';
    try {
        // Nếu timeString có format "HH:mm:ss", chỉ lấy HH:mm
        const timeParts = timeString.split(':');
        if (timeParts.length >= 2) {
            return `${timeParts[0]}:${timeParts[1]}`;
        }
        return timeString;
    } catch (error) {
        return timeString;
    }
}