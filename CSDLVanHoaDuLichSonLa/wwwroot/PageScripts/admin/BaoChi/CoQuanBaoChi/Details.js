const baseUrl = getRootLink();

$(document).ready(function () {
    if (coQuanId) {
        loadCoQuanData(coQuanId);
        initAnPhamTable(coQuanId);
    }
});

function loadCoQuanData(id) {
    let data = JSON.stringify({
        loaiToChucID: 97, // cơ quan báo chí
        toChucID: id
    })

    getDataWithApi('POST', '/api/ToChucApi/DanhSach', data).then(data => {
        console.log(data)
        if (data && data.isSuccess && data.value) {
            displayCoQuanInfo(data.value[0]);
            updatePageTitle(data.value[0]);
        } else {
            showNotification(0, 'Không tìm thấy thông tin cơ quan báo chí');
        }
    })
}

function displayCoQuanInfo(data) {
    // Hiển thị thông tin cơ bản
    $('#maCoQuan').text(data.maDinhDanh || '-');
    $('#loaiHinh').text(data.loaiHinh || '-');
    $('#coQuanChuQuan').text(data.coQuanChuQuan || '-');
    $('#phamViHoatDong').text(data.phamViHoatDong || '-');
    $('#soDienThoai').text(data.dienThoai || '-');
    $('#hopThu').text(data.hopThu || '-');
    $('#quyMo').text(data.quyMo || '-');
    
    // Hiển thị website với link nếu có
    if (data.website) {
        $('#website').html(`<a href="${data.website}" target="_blank" class="text-primary">${data.website}</a>`);
    } else {
        $('#website').text('-');
    }

    $('#soGiayPhep').text(data.soGiayPhepHoatDong || '-');
    $('#thuTu').text(data.thuTu || '-');
    
    // Hiển thị ngày thành lập
    if (data.ngayThanhLap) {
        const date = new Date(data.ngayThanhLap);
        $('#ngayThanhLap').text(date.toLocaleDateString('vi-VN'));
    } else {
        $('#ngayThanhLap').text('-');
    }

    // Hiển thị trạng thái
    if (data.trangThaiID == 1) {
        $('#trangThai').html('<span class="TrangThai green-text">Duyệt</span>');
    } else {
        $('#trangThai').html('<span class="TrangThai red-text">Chưa duyệt</span>');
    }

    // Hiển thị thông tin đa ngữ
    $('#ngonNguDich').text(data.ngonNguName || '-');
    $('#tenCoQuanDich').text(data.tenToChuc || '-');
    $('#diaChiDich').text(data.diaChi || '-');
    $('#gioiThieuDich').text(data.gioiThieu || '-');
    $('#ghiChuDich').text(data.ghiChu || '-');
}

function updatePageTitle(data) {
    let title = `Thông tin ${data.tenToChuc}`;
    let htmlTitle = ``;
    if (data.tenToChuc) {
        htmlTitle = `<div class='mainTitle'>${data.tenToChuc}</div>`;
        if (data.maDinhDanh) {
            htmlTitle += `<div class='subTitle'>(${data.maDinhDanh})</div>`;
        }
    }
    $('#pageTitle').html(htmlTitle);
    document.title = title;
}

function initAnPhamTable(coQuanBaoChiID) {
    const tableApi = {
        url: `${baseUrl}/api/BaoChiAnPhamApi/DanhSach`,
        type: "POST",
        data: function (d) {
            return JSON.stringify({
                toChucID: coQuanBaoChiID,
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
            targets: 1,
            render: function (data, type, row, meta) {
                return `<span class="detail-command-btn" id=n-"${meta.row}">${data}</span>`;
            }
        },
        {
            targets: 5,
            render: function (data, type, row, meta) {
                if (row.trangThai) {
                    return `<span class="TrangThai green-text">Đã phát hành</span>`;
                }
                else {
                    return `<span class="TrangThai yellow-text">Chưa phát hành</span>`;
                }
            }
        }
    ];

    const tableCols = [
        { "data": "stt", "width": "40px", "class": "center-align" },
        { "data": "tenAnPham", "width": "", "class": "left-align name-text" },
        { "data": "tenTanSuat", "width": "15%", "class": "left-align" },
        { "data": "tenLinhVucChuyenSau", "width": "15%", "class": "left-align" },
        { "data": "soLuong", "width": "150px", "class": "center-align" },
        { "data": "trangThai", "width": "180px", "class": "left-align" },
    ];

    initDataTableConfigNoSearch('tableAnPham', tableApi, tableDefs, tableCols);
}
