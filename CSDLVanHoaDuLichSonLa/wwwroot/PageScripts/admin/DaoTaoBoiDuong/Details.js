const baseUrl = getRootLink();

$(document).ready(function () {
    if (daoTaoID) {
        loadDaoTaoBoiDuongData(daoTaoID);
    } else {
        console.log("Không có ID đào tạo bồi dưỡng hợp lệ");
    }
});

function loadDaoTaoBoiDuongData(id) {

    getDataWithApi('GET', `/api/DaoTaoBoiDuongApi/ChiTiet/${id}`).then(data => {
        console.log('Dữ liệu đào tạo bồi dưỡng:', data);
        if (data && data.isSuccess && data.value) {
            displayDaoTaoBoiDuongInfo(data.value);
            updatePageTitle(data.value);
        } else {
            showNotification(0, 'Không tìm thấy thông tin đào tạo bồi dưỡng');
        }
    }).catch(error => {
        console.error('Lỗi khi tải dữ liệu:', error);
        showNotification(0, 'Có lỗi xảy ra khi tải dữ liệu');
    });
}

function displayDaoTaoBoiDuongInfo(data) {

    $('#linhVuc').text(data.tenLinhVuc || '-');
    $('#toChuc').text(data.tenToChuc || '-');
    $('#diaDiem').text(data.tenDiaDiem || '-');
    $('#trangThai').text(data.tenTrangThai || '-');

    $('#batDau').text(data.batDau ? formatDateTime(data.batDau) : '-');
    $('#ketThuc').text(data.ketThuc ? formatDateTime(data.ketThuc) : '-');

    $('#noiDungDaoTao').text(data.noiDungDaoTao || '-');
    $('#donVi').text(data.donVi || '-');

    //$('#nguoiTao').text(data.nguoiTao || '-');
    //$('#ngayTao').text(data.ngayTao ? formatDate(data.ngayTao) : '-');
    //$('#nguoiCapNhat').text(data.nguoiCapNhat || '-');
    //$('#ngayCapNhat').text(data.ngayCapNhat ? formatDate(data.ngayCapNhat) : '-');

}

function updatePageTitle(data) {
    //let title = `Chi tiết ${data.noiDungDaoTao || 'đào tạo bồi dưỡng'}`;
    let title = `Chi tiết đào tạo bồi dưỡng`;
    let htmlTitle = ``;
    if (data.noiDungDaoTao) {
        htmlTitle = `<div class='mainTitle'> Chi tiết tập huấn, bồi dưỡng</div>`;
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
        return date.toLocaleDateString('vi-VN');
    } catch (error) {
        console.error('Lỗi format date:', error);
        return dateString;
    }
}

function formatDateTime(dateTimeString) {
    if (!dateTimeString) return '-';
    try {
        const date = new Date(dateTimeString);
        return date.toLocaleString('vi-VN', {
            year: 'numeric',
            month: '2-digit',
            day: '2-digit',
            hour: '2-digit',
            minute: '2-digit',
            hour12: false
        });
    } catch (error) {
        return dateTimeString;
    }
}