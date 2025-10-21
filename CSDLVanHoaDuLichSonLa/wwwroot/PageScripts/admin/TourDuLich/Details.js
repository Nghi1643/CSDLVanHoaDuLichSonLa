const baseUrl = getRootLink();

$(document).ready(function () {
    if (tourId) {
        loadTourData(tourId);
        loadLichTrinhData(tourId);
    }
});

function loadTourData(tourId) {
    getDataWithApi('GET', `/api/TourDuLichApi/ChiTiet/${tourId}`).then(data => {
        if (data && data.isSuccess && data.value) {
            let tour = data.value;

            // Cập nhật title trang
            let title = `Chi tiết tour ${tour.tenTour}`;
            let htmlTitle = ``;
            if (tour.tenTour) {
                htmlTitle = `<div class='mainTitle'>${tour.tenTour}</div>`;
                if (tour.maDinhDanh) {
                    htmlTitle += `<div class='subTitle'>(${tour.maDinhDanh})</div>`;
                }
            }
            $('#pageTitle').html(htmlTitle);
            document.title = title;

            // Thông tin cơ bản
            $("#tenTour").text(tour.tenTour || '-');
            $("#maDinhDanh").text(tour.maDinhDanh || '-');
            $("#thoiGian").text(tour.thoiGian || '-');
            $("#tenToChuc").text(tour.tenToChuc || '-');
            $("#tenTheLoai").text(tour.tenTheLoai || '-');
            $("#tenLoaiHinh").text(tour.tenLoaiHinh || '-');
            $("#tenPhanKhuc").text(tour.tenPhanKhuc || '-');

            // Địa điểm và thời gian
            $("#tenDiemDi").text(tour.tenDiemDi || '-');
            $("#tenDiemDen").text(tour.tenDiemDen || '-');
            $("#ngayKhoiHanh").text(formatDateWithoutTime(tour.ngayKhoiHanh));
            $("#ngayKetThuc").text(formatDateWithoutTime(tour.ngayKetThuc));
            $("#tenPhuongTien").text(tour.tenPhuongTien || '-');
            $("#tenNgonNgu").text(tour.tenNgonNgu || '-');

            // Trạng thái
            const trangThaiText = tour.trangThai ? 'Đã duyệt' : 'Chưa duyệt';
            const trangThaiClass = tour.trangThai ? 'badge-success' : 'badge-warning';
            $("#trangThai").text(trangThaiText).removeClass().addClass(`badge ${trangThaiClass}`);

            // Thông tin khách và giá
            $("#soKhachToiThieu").text(tour.soKhachToiThieu || '0');
            $("#soKhachToiDa").text(tour.soKhachToiDa || '0');
            $("#giaTour").text(tour.giaTour ? formatCurrency(tour.giaTour) : '-');
            $("#giaNguoi").text(tour.giaNguoi ? formatCurrency(tour.giaNguoi) : '-');

            // Đánh giá
            $("#soLuotDanhGia").text(tour.soLuotDanhGia || '0');
            $("#diemDanhGia").text(tour.diemDanhGia ? `${tour.diemDanhGia}/5 ⭐` : '-');

            // Nội dung mô tả
            $("#moTa").html(tour.moTa || '-');
            $("#traiNghiem").html(tour.traiNghiem || '-');
            $("#goiY").html(tour.goiY || '-');
        }
    }).catch(error => {
        showNotification(0, 'Không thể tải thông tin tour du lịch');
    });
}

function loadLichTrinhData(tourId) {
    getDataWithApi('GET', `/api/TourDuLichApi/LichTrinh/DanhSach/${tourId}?maNgonNgu=vi`).then(data => {
        if (data && data.isSuccess && data.value) {
            // ✅ NHÓM DỮ LIỆU theo LichTrinhID
            const groupedData = groupLichTrinhData(data.value);
            initLichTrinhTable(groupedData);
        }
    }).catch(error => {
        showNotification(0, 'Không thể tải lộ trình tour');
    });
}

// ✅ THÊM: Function nhóm dữ liệu theo LichTrinhID
function groupLichTrinhData(rawData) {
    const grouped = {};

    rawData.forEach(item => {
        const lichTrinhID = item.lichTrinhID;

        if (!grouped[lichTrinhID]) {
            // Tạo object lộ trình chính
            grouped[lichTrinhID] = {
                lichTrinhID: item.lichTrinhID,
                tourID: item.tourID,
                diaDiemID: item.diaDiemID,
                thoiDiem: item.thoiDiem,
                thuTu: item.thuTu,
                tenDiaDiem: item.tenDiaDiem,
                diaChi: item.diaChi,
                hoatDongs: [] // Array chứa các hoạt động
            };
        }

        // Thêm hoạt động vào array (nếu có)
        if (item.hoatDong || item.soGio) {
            grouped[lichTrinhID].hoatDongs.push({
                hoatDong: item.hoatDong || '',
                soGio: item.soGio || ''
            });
        }
    });

    // Convert object thành array và sắp xếp theo thứ tự
    return Object.values(grouped).sort((a, b) => (a.thuTu || 0) - (b.thuTu || 0));
}

function initLichTrinhTable(lichTrinhData) {
    const tableDefs = [
        {
            targets: 0, // Thứ tự
            render: function (data, type, row, meta) {
                return row.thuTu || '-';
            }
        },
        {
            targets: 1, // Địa điểm
            render: function (data, type, row, meta) {
                return `<div class="group-info">
                    <div class="info-main">${row.tenDiaDiem || '-'}</div>
                    <div class="info-sub">${row.diaChi || ''}</div>
                </div>`;
            }
        },
        {
            targets: 2, //
            render: function (data, type, row, meta) {
                if (row.hoatDongs && row.hoatDongs.length > 0) {
                    // Lọc bỏ hoạt động trống
                    const validActivities = row.hoatDongs.filter(hd =>
                        hd.hoatDong && hd.hoatDong.trim() !== ''
                    );

                    if (validActivities.length > 0) {
                        const activityList = validActivities.map((hd, index) =>
                            `<div class="activity-item">
                                <span class="activity-number">${index + 1}.</span>
                                <span class="activity-text">${hd.hoatDong}</span>
                                ${hd.soGio ? `<span class="activity-time">(${hd.soGio} giờ)</span>` : ''}
                            </div>`
                        ).join('');

                        return `<div class="activities-list">${activityList}</div>`;
                    }
                }
                return '<span class="text-muted">Chưa có hoạt động</span>';
            }
        },
        {
            targets: 3, 
            render: function (data, type, row, meta) {
                if (row.hoatDongs && row.hoatDongs.length > 0) {
                    const validTimes = row.hoatDongs.filter(hd =>
                        hd.soGio && hd.soGio.trim() !== ''
                    );

                    if (validTimes.length > 0) {
                        // Thử tính tổng thời gian nếu là số
                        let totalHours = 0;
                        let hasNumericTime = false;

                        validTimes.forEach(hd => {
                            const hours = parseFloat(hd.soGio);
                            if (!isNaN(hours)) {
                                totalHours += hours;
                                hasNumericTime = true;
                            }
                        });

                        if (hasNumericTime && totalHours > 0) {
                            return `<div class="time-info">
                                <div class="total-time">${totalHours} giờ</div>
                                <div class="time-count">(${validTimes.length} hoạt động)</div>
                            </div>`;
                        } else {
                            // Hiển thị danh sách thời gian nếu không phải số
                            const timeList = validTimes.map(hd => hd.soGio).join(', ');
                            return `<div class="time-info">${timeList}</div>`;
                        }
                    }
                }
                return '-';
            }
        },
        {
            targets: 4, // Thời điểm đến
            render: function (data, type, row, meta) {
                return row.thoiDiem ? formatDateTimeDisplay(row.thoiDiem) : '-';
            }
        }
    ];

    const tableCols = [
        { "data": "", "width": "60px", "class": "center-align" },
        { "data": "", "width": "25%", "class": "left-align" },
        { "data": "", "width": "40%", "class": "left-align" },
        { "data": "", "width": "15%", "class": "center-align" },
        { "data": "", "width": "20%", "class": "center-align" }
    ];

    // Destroy existing DataTable if exists
    if ($.fn.DataTable.isDataTable('#dataGridLoTrinh')) {
        $('#dataGridLoTrinh').DataTable().destroy();
    }

    $('#dataGridLoTrinh').DataTable({
        data: lichTrinhData,
        columns: tableCols,
        columnDefs: tableDefs,
        paging: false,
        searching: false,
        info: false,
        ordering: false,
        autoWidth: false,
        responsive: true,
        language: {
            emptyTable: "Chưa có lộ trình nào"
        }
    });
}

// Utility functions
function formatDateWithoutTime(dateString) {
    if (!dateString) return '-';

    try {
        const date = new Date(dateString);
        if (isNaN(date.getTime())) return dateString;

        const day = date.getDate().toString().padStart(2, '0');
        const month = (date.getMonth() + 1).toString().padStart(2, '0');
        const year = date.getFullYear();

        return `${day}/${month}/${year}`;
    } catch (error) {
        return dateString;
    }
}

function formatDateTimeDisplay(dateTimeString) {
    if (!dateTimeString) return '-';

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

function formatCurrency(amount) {
    if (!amount) return '0 VNĐ';

    return new Intl.NumberFormat('vi-VN', {
        style: 'currency',
        currency: 'VND'
    }).format(amount);
}