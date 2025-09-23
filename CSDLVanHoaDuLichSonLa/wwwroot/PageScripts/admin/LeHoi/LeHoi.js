const baseUrl = getRootLink();

$(document).ready(function () {
    initDatePicker();
    initDanhMucChung_NoAll(37, "#the-loai-search", "Thể loại lễ hội")
    initDanhMucChung_NoAll(38, "#cap-do-search", "")
    initDanhMucChung_NoAll(6, "#tan-suat-search", "")
    initNgonNgu("#ngon-ngu-search")
    initSelect2();
    initTable();

    $('#tim-kiem').on('click', async function () {
        initTable()
    });

    $('#tat-ca').on('click', async function () {
        $('#tu-khoa-search').val("");
        $('#co-quan-chu-quan-search').val("").trigger('change');
        $('#loai-hinh-search').val("-1").trigger('change');
        $('#pham-vi-hoat-dong-search').val("").trigger('change');
        $('#trang-thai-search').val("-1").trigger('change');
        $('#ngon-ngu-search').val("vi").trigger('change');
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
            console.error('Lỗi khi xử lý form:', error);
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
        url: `${baseUrl}/api/VH_LeHoiApi/Gets?MaNgonNgu=vi`,
        type: "GET",
        contentType: 'application/json; charset=utf-8',
        dataSrc: function (data) {
            if (data && data.isSuccess && data.value.length > 0) {
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
            targets: 1, // Tên nhà xuất bản
            render: function (data, type, row, meta) {
                return `<div class="group-info">
                    <div class="info-main">
                        <a href="${baseUrl}/AdminTool/XuatBan/Details?id=${row.leHoiID}" class="text-primary text-decoration-none">
                            ${row.tenLeHoiChild || ''}
                        </a>
                    </div>
                    <div class="info-sub">${row.tenToChuc || ''}</div>
                </div>`;
            }
        },
        {
            targets: 2, // Thể loại
            render: function (data, type, row, meta) {
                return row.theLoai || '';
            }
        },
        {
            targets: 3, // Cấp độ
            render: function (data, type, row, meta) {
                return row.capDo || '';
            }
        },
        {
            targets: 4, // Tần suất
            render: function (data, type, row, meta) {
                return row.tanSuat || 0;
            }
        },
        {
            targets: 5, // Ấn phẩm/Xuất bản
            render: function (data, type, row, meta) {
                return row.thoiGianDienRa || '';
            }
        },
        {
            targets: 6, // Trạng thái
            render: function (data, type, row, meta) {
                if (row.trangThai) {
                    return `<span class="TrangThai green-text">Duyệt</span>`;
                } else {
                    return `<span class="TrangThai red-text">Chưa duyệt</span>`;
                }
            }
        },
        {
            targets: 7, // Chức năng
            render: function (data, type, row, meta) {
                let html = "";
                if (permitedEdit) {
                    html += `<a href="${baseUrl}/AdminTool/LeHoi/Edit?id=${row.leHoiID}" data-toggle="tooltip" title="Chỉnh sửa" class="text-yellow me-2">
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
        { "data": "tenLeHoiChild", "class": "left-align name-text" },
        { "data": "", "width": "15%", "class": "left-align" },
        { "data": "", "width": "15%", "class": "left-align" },
        { "data": "", "width": "7%", "class": "left-align" },
        { "data": "", "width": "15%", "class": "left-align name-text" },
        { "data": "", "width": "8%", "class": "left-align" },
        { "data": "", "width": "8%", "class": "center-align group-icon-action" }
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

        $('#idDelete').val(data.leHoiID);
        $('#nameDelete').text(`${data.tenLeHoiChild}`);

        $('#modalDelete').modal('show');
    });
}

function dataDelete() {
    let id = $('#idDelete').val();
    $.ajax({
        url: `${baseUrl}/api/VH_LeHoiApi/Delete?LeHoiID=${id}`,
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