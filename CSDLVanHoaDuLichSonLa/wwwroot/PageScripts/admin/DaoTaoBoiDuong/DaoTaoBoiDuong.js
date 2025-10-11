const baseUrl = getRootLink();

$(document).ready(function () {
    initDatePicker();
    initLinhVuc("#linh-vuc-search");
    initDiaDiem("#dia-diem-to-chuc-search");
    initToChuc(1, "#to-chuc-search");
    initDanhMucChung(6, "#trang-thai-search", "TrangThaiID")// TrangThaiID
    initNgonNgu("#ngon-ngu-search")
    initSelect2();
    initTable();
    console.log("ready");

    $('#tim-kiem').on('click', async function () {
        initTable()
    });

    $('#tat-ca').on('click', async function () {
        $('#tu-khoa-search').val("");
        $('#linh-vuc-search').val("-1").trigger('change');
        $('#dia-diem-to-chuc-search').val("-1").trigger('change');
        $('#to-chuc-search').val("-1").trigger('change');
        $('#trang-thai-search').val("-1").trigger('change');
        $('#ngon-ngu-search').val("-1").trigger('change');
        initTable()
    });

    document.getElementById("tu-khoa-search").addEventListener("keypress", function (event) {
        if (event.key === "Enter") {
            event.preventDefault();
            initTable()
        }
    });

    $("#formDelete").on("submit", function (e) {
        e.preventDefault();

        const submitBtn = $(this).find('button[type="submit"]');
        if (submitBtn.prop('disabled')) {
            return;
        }

        submitBtn.prop('disabled', true);
        const originalText = submitBtn.text();
        submitBtn.text('Đang xóa...');

        try {
            dataDelete();
        } catch (error) {
        } finally {
            setTimeout(() => {
                submitBtn.prop('disabled', false);
                submitBtn.text(originalText);
            }, 1000);
        }
    });
});

function initTable() {
    const tableApi = {
        url: `${baseUrl}/api/DaoTaoBoiDuongApi/DanhSach`,
        type: "POST",
        data: function (d) {
            var linhVucID = $('#linh-vuc-search').val();
            var diaDiemID = $('#dia-diem-to-chuc-search').val();
            var toChucID = $('#to-chuc-search').val();
            var trangThaiID = $('#trang-thai-search').val();
            var maNgonNgu = $('#ngon-ngu-search').val();

            return JSON.stringify({
                tuKhoa: $('#tu-khoa-search').val() || null,
                linhVucID: linhVucID == "-1" ? null : linhVucID,
                diaDiemID: diaDiemID == "-1" ? null : diaDiemID,
                toChucID: toChucID == "-1" ? null : toChucID,
                trangThaiID: trangThaiID == "-1" ? null : trangThaiID,
                maNgonNgu: maNgonNgu == "-1" ? null : maNgonNgu
            });
        },
        contentType: 'application/json; charset=utf-8',
        dataSrc: function (data) {
            if (data && data.isSuccess && data.value.length > 0) {
                data.value.forEach((item, index) => {
                    item.stt = index + 1;

                    if (item.batDau) {
                        const date = new Date(item.batDau);
                        item.batDauFormatted = date.toLocaleString('vi-VN', {
                            year: 'numeric',
                            month: '2-digit',
                            day: '2-digit',
                            hour: '2-digit',
                            minute: '2-digit',
                            hour12: false
                        });
                    }
                    if (item.ketThuc) {
                        const date = new Date(item.ketThuc);
                        item.ketThucFormatted = date.toLocaleString('vi-VN', {
                            year: 'numeric',
                            month: '2-digit',
                            day: '2-digit',
                            hour: '2-digit',
                            minute: '2-digit',
                            hour12: false
                        });
                    }
                });
                return data.value;
            }
            return [];
        },
    };

    const tableDefs = [
        {
            targets: 1, // Cột Nội dung đào tạo
            render: function (data, type, row, meta) {
                const maxLength = 100; // Giới hạn 100 ký tự
                const fullText = row.noiDungDaoTao || '';
                const truncatedText = fullText.length > maxLength
                    ? fullText.substring(0, maxLength) + '...'
                    : fullText;

                return `<div class="group-info">
                            <div class="info-main">
                                <a href="${baseUrl}/AdminTool/DaoTaoBoiDuong/Details?id=${row.daoTaoID}" 
                                   class="text-primary text-decoration-none" 
                                   title="${fullText}">
                                    ${truncatedText}
                                </a>
                            </div>
                        </div>`;
            }
        },
        {
            targets: 2, // Cột Đơn vị tổ chức
            render: function (data, type, row, meta) {
                return row.tenToChuc || row.donVi || '';
            }
        },
        {
            targets: 3, // Cột Thời gian 
            render: function (data, type, row, meta) {
                return `<div class="group-info">
                    <div class="info-main">
                        ${row.batDauFormatted ? "Từ: " + row.batDauFormatted + "<br/>" : ""}
                        ${row.ketThucFormatted ? "Đến: " + row.ketThucFormatted : ""}
                    </div>
                </div>`;
            }
        },
        {
            targets: 4, // Cột Địa điểm
            render: function (data, type, row, meta) {
                return row.tenDiaDiem || '';
            }
        },
        {
            targets: 5, // Cột Trạng thái
            render: function (data, type, row, meta) {
                return row.tenTrangThai || '';
            }
        },
        {
            targets: 6, // Cột chức năng
            render: function (data, type, row, meta) {
                let html = "";
                if (permitedEdit) {
                    html += `<a href="${baseUrl}/AdminTool/DaoTaoBoiDuong/Edit?id=${row.daoTaoID}" data-toggle="tooltip" title="Chỉnh sửa" class="text-yellow me-2">
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
        { "data": "stt", "width": "40px", "class": "left-align" },
        { "data": "noiDungDaoTao", "class": "left-align name-text" },
        { "data": "tenToChuc", "width": "15%", "class": "left-align" },
        { "data": "thoiGian", "width": "15%", "class": "left-align" },
        { "data": "tenDiaDiem", "width": "15%", "class": "left-align" },
        { "data": "tenTrangThai", "width": "8%", "class": "left-align" },
        { "data": "", "width": "10%", "class": "center-align group-icon-action" }
    ];

    if (!permitedEdit && !permitedDelete) {
        tableCols.pop();
        tableDefs.pop();
    }

    initDataTableConfigNoSearch('dataGrid', tableApi, tableDefs, tableCols);

    // Event handler cho nút xóa
    $('#dataGrid tbody').on('click', '.delete-command-btn', function () {
        var id = $(this).attr("ID").match(/\d+/)[0];
        var data = $('#dataGrid').DataTable().row(id).data();

        $('#idDelete').val(data.daoTaoID);
        $('#nameDelete').text(`${data.noiDungDaoTao}`);

        $('#modalDelete').modal('show');
    });
}

function dataDelete() {
    let id = $('#idDelete').val();
    $.ajax({
        url: `${baseUrl}/api/DaoTaoBoiDuongApi/Xoa/${id}`,
        type: 'DELETE',
        contentType: 'application/json',
        success: function (data) {
            if (data && data.isSuccess && data.value) {
                showNotification(1, 'Xoá thành công')
                $('#modalDelete').modal('hide');
                $('#dataGrid').DataTable().ajax.reload();
            } else {
                showNotification(0, data.error)
            }
        },
        error: function (err) {
            showNotification(0, 'Có lỗi xảy ra, vui lòng thử lại sau')
        }
    })
}

function initLinhVuc(elementId = "#linhVuc", placeholder = "Chọn lĩnh vực") {
    getDataWithApi('GET', '/api/LinhVucApi/Gets').then(response => {
        if (response && response.isSuccess && response.value) {
            $(elementId).empty()
                //.append(`<option value="">${placeholder}</option>`)
                .append(`<option value="-1">Tất cả</option>`);

            response.value.forEach(item => {
                $(elementId).append(`<option value="${item.linhVucID}">${item.ten}</option>`);
            });
        } else {
            showNotification(0, "Không thể load danh sách lĩnh vực");
        }
    }).catch(error => {
        console.error('Lỗi khi load lĩnh vực:', error);
        showNotification(0, "Lỗi kết nối khi tải danh sách lĩnh vực");
    });
}

function initDiaDiem(elementId = "#diaDiem", placeholder = "Chọn địa điểm") {
    const requestData = JSON.stringify({
        MaNgonNgu: "vi",
        TrangThai: true
    });

    getDataWithApi('POST', '/api/DiaDiemApi/DanhSach', requestData).then(response => {
        if (response && response.isSuccess && response.value) {
            $(elementId).empty()
                //.append(`<option value="">${placeholder}</option>`)
                .append(`<option value="-1">Tất cả</option>`);

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
        console.error('Lỗi khi load địa điểm:', error);
        showNotification(0, "Lỗi kết nối khi tải danh sách địa điểm");
    });
}

function initToChuc(loaiToChucID , elementId = "#toChuc", placeholder = "Chọn tổ chức") {
    const requestData = JSON.stringify({
        loaiToChucID: loaiToChucID,
        maNgonNgu: "vi",
        trangThai: true
    });

    getDataWithApi('POST', '/api/ToChucApi/DanhSach', requestData).then(response => {
        if (response && response.isSuccess && response.value) {
            $(elementId).empty()
                //.append(`<option value="">${placeholder}</option>`)
                .append(`<option value="-1">Tất cả</option>`);

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