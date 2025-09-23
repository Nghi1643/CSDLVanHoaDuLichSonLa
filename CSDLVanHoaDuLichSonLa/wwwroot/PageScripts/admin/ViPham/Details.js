const baseUrl = getRootLink();

$(document).ready(function () {
    if (viPhamId) {
        loadViPhamData(viPhamId);
        initVanBanTable(viPhamId);
    }

    async function loadViPhamData(id) {
        try {
            const res = await fetch(`${baseUrl}/api/ViPhamApi/ChiTiet/${id}`, {
                method: 'GET',
                headers: { 'Content-Type': 'application/json' }
            });
            if (res.ok) {
                const data = await res.json();
                if (data && data.isSuccess && data.value) {
                    displayViPhamInfo(data.value);
                } else {
                    showNotification(0, 'Không tìm thấy thông tin vi phạm');
                }
            } else {
                showNotification(0, 'Lỗi khi tải dữ liệu');
            }
        } catch (error) {
            console.error('Lỗi khi tải dữ liệu vi phạm:', error);
            showNotification(0, 'Có lỗi xảy ra khi tải dữ liệu');
        }
    }

    function displayViPhamInfo(data) {
        $('#ngayPhatHien').text(data.ngayPhatHien ? new Date(data.ngayPhatHien).toLocaleDateString('vi-VN') : '-');
        $('#hinhThucXuLy').text(data.hinhThucXuLy || '-');
        $('#donViViPham').text(data.donViViPhamName || '-');
        $('#loaiDonVi').text(data.loaiDonViName || '-');
        $('#thuTu').text(data.thuTu || '-');
        $('#trangThai').html(data.trangThai == 1 ? '<span class="TrangThai green-text">Duyệt</span>' : '<span class="TrangThai red-text">Chưa duyệt</span>');
        $('#moTa').text(data.moTa || '-');
    }

    function initVanBanTable(viPhamId) {
        const tableApi = {
            url: `${baseUrl}/api/ViPhamVanBanApi/DanhSach`,
            type: "POST",
            data: function (d) {
                return JSON.stringify({ viPhamId: viPhamId });
            },
            contentType: 'application/json; charset=utf-8',
            dataSrc: function (data) {
                if (data && data.isSuccess && data.value && data.value.length > 0) {
                    data.value.forEach((item, index) => {
                        item.stt = index + 1;
                    });
                    return data.value;
                }
                return [];
            },
        };

        const tableDefs = [
            // Có thể custom render cho các cột nếu cần
        ];

        const tableCols = [
            { "data": "stt", "width": "40px", "class": "center-align" },
            { "data": "soKyHieu", "class": "left-align" },
            { "data": "trichYeu", "class": "left-align" },
            { "data": "loaiDoiTuong", "class": "left-align" },
            { "data": "coQuanBanHanh", "class": "left-align" },
            { "data": "ngayBanHanh", "class": "left-align" },
            { "data": "ngayHieuLuc", "class": "left-align" },
            { "data": "tepDinhKem", "class": "left-align" }
        ];

        initDataTableConfigNoSearch('tableVanBan', tableApi, tableDefs, tableCols);
    }
});
