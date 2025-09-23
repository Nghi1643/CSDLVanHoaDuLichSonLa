const baseUrl = getRootLink();

$(document).ready(function () {
    initSelect2();
    initDatePicker();

    (async function () {
        await initNgonNgu();
        await initLoaiDonVi();
        await initDonViViPham();
        initTable();
    })();

    $('#tim-kiem').on('click', async function () {
        initTable();
    });

    $('#tat-ca').on('click', async function () {
        $('#tu-khoa-search').val("");
        $('#loai-don-vi-search').val("").trigger('change');
        $('#don-vi-vi-pham-search').val("").trigger('change');
        $('#trang-thai-xu-ly-search').val("-1").trigger('change');
        $('#tu-ngay-search').val("");
        $('#den-ngay-search').val("");
        $('#ngon-ngu-search').val("").trigger('change');
        initTable();
    });

    async function initTable() {
        const tableApi = {
            url: `${baseUrl}/api/ViPhamApi/DanhSach`,
            type: "POST",
            data: function (d) {
                return JSON.stringify({
                    tuKhoa: $('#tu-khoa-search').val() || null,
                    //loaiDonVi: $('#loai-don-vi-search').val() || null,
                    donViViPham: $('#don-vi-vi-pham-search').val() == "-1" ? null : $('#don-vi-vi-pham-search').val(),
                    trangThaiXuLy: $('#trang-thai-xu-ly-search').val() == "-1" ? null : $('#trang-thai-xu-ly-search').val(),
                    //tuNgay: $('#tu-ngay-search').val() || null,
                    //denNgay: $('#den-ngay-search').val() || null,
                    maNgonNgu: $('#ngon-ngu-search').val() == "-1" ? null : $('#ngon-ngu-search').val(),
                });
            },
            contentType: 'application/json; charset=utf-8',
            dataSrc: function (data) {
                if (data && data.isSuccess && data.value && data.value.length > 0) {
                    console.log(data)
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
                targets: 2, // Cột Thời gian phát hiện
                render: function (data, type, row, meta) {
                    return formatDateTime(row.ngayPhatHien);
                }
            },
            {
                targets: 6, // Cột Ấn phẩm
                render: function (data, type, row, meta) {
                    return `<div class="group-info">
                        <div class="info-main">
                            <a href="${baseUrl}/AdminTool/BaoChi/AnPhamKenhPhatSong?id=${row.toChucID}" class="text-primary text-decoration-none">
                                ${row.soLuongAnPham || 0}
                            </a>
                        </div>
                    </div>`;
                }
            },
            {
                targets: 6, // Cột trạng thái
                render: function (data, type, row, meta) {
                    if (row.trangThaiXuly) {
                        return `<span class="TrangThai green-text">Duyệt</span>`;
                    } else {
                        return `<span class="TrangThai red-text">Chưa duyệt</span>`;
                    }
                }
            },
            {
                targets: 7, // Cột chức năng
                render: function (data, type, row, meta) {
                    let html = "";
                    if (permitedEdit) {
                        html += `<a href="${baseUrl}/AdminTool/BaoChi/Add?id=${row.toChucID}" data-toggle="tooltip" title="Chỉnh sửa" class="text-yellow me-2">
                                    <i class="hgi-icon hgi-edit"></i>
                                </a>`;
                    }
                    if (permitedDelete) {
                        html += `<i data-toggle="tooltip" title="Xóa" class="delete-command-btn text-red cursor-pointer" id="delete-${meta.row}">
                                    <i class="hgi-icon hgi-delete"></i>
                                </i>`;
                    }
                    if (!permitedEdit && !permitedDelete) {
                        html = `<span class="text-muted">Chỉ xem</span>`;
                    }
                    return html;
                }
            }
        ];

        const tableCols = [
            { "data": "stt", "width": "40px", "class": "center-align" },
            { "data": "noiDungViPham", "class": "left-align name-text" },
            { "data": "ngayPhatHien", "class": "left-align" },
            { "data": "loaiDonVi", "class": "left-align" },
            { "data": "hinhThucXuLy", "class": "left-align" },
            { "data": "", "class": "left-align" },
            { "data": "trangThaiXuly", "class": "left-align" },
            { "data": "", "class": "center-align group-icon-action" }
        ];

        initDataTableConfigNoSearch('dataGrid', tableApi, tableDefs, tableCols);
    }

    async function initNgonNgu() {
        try {
            const res = await fetch(`${baseUrl}/api/NgonNguApi/DanhSach`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ trangThai: true, tuKhoa: null })
            });
            if (!res.ok) throw new Error(await res.text());
            const data = await res.json();
            if (data && data.isSuccess && data.value) {
                $("#ngon-ngu-search, #ngonNguDich").empty();
                $("#ngon-ngu-search, #ngonNguDich").append(`<option value="-1">Tất cả</option>`);
                data.value.forEach(lang => {
                    $("#ngon-ngu-search, #ngonNguDich").append(`<option value="${lang.maNgonNgu?.toLowerCase()}">${lang.tenNgonNgu}</option>`);
                });
            }
        } catch (err) { console.log('Lỗi khi tải danh sách ngôn ngữ:', err.message); }
    }

    async function initLoaiDonVi() {
        // Gọi API lấy loại đơn vị
        try {
            const res = await fetch(`${baseUrl}/api/DanhMucChungApi/DanhSach`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ loaiDanhMucID: "13", trangThai: true }) // XX: ID loại đơn vị
            });
            if (!res.ok) throw new Error(await res.text());
            const data = await res.json();
            if (data && data.isSuccess && data.value) {
                $("#loai-don-vi-search, #loaiDonVi").empty();
                $("#loai-don-vi-search, #loaiDonVi").append(`<option value="-1">Tất cả</option>`);
                data.value.forEach(item => {
                    $("#loai-don-vi-search, #loaiDonVi").append(`<option value="${item.danhMucID}">${item.tenDanhMuc}</option>`);
                });
            }
        } catch (err) { console.log('Lỗi khi tải loại đơn vị:', err.message); }
    }

    async function initDonViViPham() {
        // Gọi API lấy đơn vị vi phạm
        try {
            const res = await fetch(`${baseUrl}/api/ToChucApi/DanhSach`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ loaiToChucID: 97 }) // 97, 98: ID loại tổ chức vi phạm
            });
            if (!res.ok) throw new Error(await res.text());
            const data = await res.json();
            if (data && data.isSuccess && data.value) {
                $("#don-vi-vi-pham-search, #donViViPham").empty();
                $("#don-vi-vi-pham-search, #donViViPham").append(`<option value="-1">Tất cả</option>`);
                data.value.forEach(item => {
                    $("#don-vi-vi-pham-search, #donViViPham").append(`<option value="${item.toChucID}">${item.tenToChuc}</option>`);
                });
            }
        } catch (err) { console.log('Lỗi khi tải đơn vị vi phạm:', err.message); }
    }

    // Thêm các sự kiện khác nếu cần
});
