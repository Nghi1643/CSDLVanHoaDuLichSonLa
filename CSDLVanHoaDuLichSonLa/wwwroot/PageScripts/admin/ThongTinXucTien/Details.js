const baseUrl = getRootLink();

$(document).ready(function () {
    if (xucTienID) {
        loadThongTinXucTienData(xucTienID);
    }
});

function loadThongTinXucTienData(id) {
    console.log('Loading thông tin xúc tiến data:', id);

    // Gọi API để lấy thông tin xúc tiến
    getDataWithApi('GET', `/api/ThongTinXucTienApi/ChiTiet/${id}`).then(data => {
        console.log('Dữ liệu thông tin xúc tiến:', data);
        if (data && data.isSuccess && data.value) {
            displayThongTinXucTienInfo(data.value);
            updatePageTitle(data.value);
        } else {
            showNotification(0, 'Không tìm thấy thông tin xúc tiến');
        }
    }).catch(error => {
        console.error('Lỗi khi tải dữ liệu:', error);
        showNotification(0, 'Có lỗi xảy ra khi tải dữ liệu');
    });
}

function displayThongTinXucTienInfo(data) {
    // Hiển thị thông tin cơ bản
    $('#tenLinhVuc').text(data.tenLinhVuc || '-');
    $('#tenToChuc').text(data.tenToChuc || '-');
    $('#tenDoiTuong').text(data.tenDoiTuong || '-');
    $('#tenHinhThuc').text(data.tenHinhThuc || '-');
    $('#tenTruyenThong').text(data.tenTruyenThong || '-');

    // Hiển thị ngày bắt đầu và kết thúc (chỉ hiển thị ngày, không hiển thị giờ)
    $('#ngayBatDau').text(data.ngayBatDau ? formatDate(data.ngayBatDau) : '-');
    $('#ngayKetThuc').text(data.ngayKetThuc ? formatDate(data.ngayKetThuc) : '-');

    // Hiển thị trạng thái
    $('#tenTrangThai').text(data.tenTrangThai || '-');
    console.log("---------"+data.tenTrangThai)
    // Hiển thị thông tin đa ngữ
    $('#tieuDe').text(data.tieuDe || '-');
    $('#moTa').text(data.moTa || '-');

    // Hiển thị thông tin người tạo/cập nhật
    $('#nguoiTao').text(data.nguoiTao || '-');
    $('#ngayTao').text(data.ngayTao ? formatDate(data.ngayTao) : '-');
    $('#nguoiCapNhat').text(data.nguoiCapNhat || '-');
    $('#ngayCapNhat').text(data.ngayCapNhat ? formatDate(data.ngayCapNhat) : '-');
}

//function displayTrangThai(tenTrangThai, trangThaiID) {
//    if (tenTrangThai) {
//        // Hiển thị theo tên trạng thái
//        $('#tenTrangThai').html(`<span class="badge">${tenTrangThai}</span>`);
//    }
//    //else {
//    //    // Fallback theo ID nếu không có tên
//    //    switch (trangThaiID) {
//    //        case 1:
//    //            $('#tenTrangThai').html('<span class="badge badge-success">Đã duyệt</span>');
//    //            break;
//    //        case 2:
//    //            $('#tenTrangThai').html('<span class="badge badge-warning">Chờ duyệt</span>');
//    //            break;
//    //        case 3:
//    //            $('#tenTrangThai').html('<span class="badge badge-danger">Từ chối</span>');
//    //            break;
//    //        default:
//    //            $('#tenTrangThai').html('<span class="badge badge-secondary">Không xác định</span>');
//    //    }
//    //}
//}

function updatePageTitle(data) {
    let title = `Chi tiết ${data.tieuDe || 'thông tin xúc tiến'}`;
    let htmlTitle = ``;
    if (data.tieuDe) {
        htmlTitle = `<div class='mainTitle'>${data.tieuDe}</div>`;
        if (data.tenLinhVuc) {
            htmlTitle += `<div class='subTitle'>(${data.tenLinhVuc})</div>`;
        }
    }
    $('#pageTitle').html(htmlTitle);
    document.title = title;
}

function formatDate(dateString) {
    if (!dateString) return '-';
    try {
        const date = new Date(dateString);
        // Chỉ hiển thị ngày, không hiển thị giờ
        return date.toLocaleDateString('vi-VN');
    } catch (error) {
        console.error('Lỗi format date:', error);
        return dateString;
    }
}