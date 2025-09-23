
const baseUrl = getRootLink();

$(document).ready(function () {
    initSelect2()
    initDatePicker()
    initDanhMucChung_NoAll(39, "#cap-do-search", "") // Loại đối tượng - Cấp độ
    initDanhMucChung_NoAll(42, "#trang-thai-su-kien-search", "") // Trạng thái sự kiện
    initDanhMucChung_NoAll(25, "#theLoaiAdd", "") // Thể loại - Lĩnh vực
    initDanhMucChung_NoAll(9, "#linhVucAdd", "") // Lĩnh vực
    initDanhMucChung_NoAll(5, "#gioi-tinh-search", "")
    initNgonNgu("#ngon-ngu-search")
    initTable()

    $('#tim-kiem').on('click', async function () {
        initTable()
    });

    $('#tat-ca').on('click', async function () {
        $('#tu-khoa-search').val("");
        $('#cap-do-search').val("").trigger('change');
        $('#trang-thai-su-kien-search').val("").trigger('change');
        $("#ngay-bat-dau-search").val("");
        $("#ngay-ket-thuc-search").val("");
        $('#trang-thai-search').val("-1").trigger('change');
        $('#ngon-ngu-search').val("").trigger('change');
        initTable()
    });
})

function initTable() {
    const tableApi = {
        url: `${baseUrl}/api/DM_CaNhan_VanDongVienApi/Gets?MaNgonNgu=vi`,
        type: "GET",
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
            targets: 1, // Cột tên cơ quan báo chí
            render: function (data, type, row, meta) {
                return `<a href="${baseUrl}/AdminTool/NhanSu/Details_VanDongVien?id=${row.caNhanID}" class="text-primary text-decoration-none"><div class="group-info have-image round-image">
                        <div class="have-image">
                            <img src="${row.duongDanHinhAnhBia || '/assets/images/vector/no-image.png'}" alt="${row.tenDaPhuongTien || ''}" onerror="this.onerror=null;this.src='/assets/images/vector/no-image.png';" />
                        </div>
                        <div class="group-info">
                        <div class="info-main"><div>${row.hoTen || ''}</div></div>
                        <div class="info-sub">${row.maDinhDanh || ''}</div></div>
                </div></a>`;
            }
        },
        {
            targets: 2,
            render: function (data, type, row, meta) {
                return formatDateWithoutTime(row.ngaySinh);
            }
        },
        {
            targets: 4, // Tần suất
            render: function (data, type, row, meta) {
                return row.tenTrangThai || "";
            }
        },
        {
            targets: 5, // Tần suất
            render: function (data, type, row, meta) {
                return `<a href="${baseUrl}/AdminTool/NhanSu/Details_VanDongVien?id=${row.caNhanID}">
                            ${row.soLuongMon} môn
                        </a>`
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
                    html += `<a href="${baseUrl}/AdminTool/NhanSu/Edit_VanDongVien?id=${row.caNhanID}" data-toggle="tooltip" title="Chỉnh sửa" class="text-yellow me-2">
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
        { "data": "", "class": "left-align name-text" },
        { "data": "", "width": "10%", "class": "left-align" },
        { "data": "gioiTinh", "width": "120px", "class": "left-align" },
        { "data": "", "width": "20%", "class": "left-align" },
        { "data": "", "width": "160px", "class": "center-align name-text" },
        { "data": "", "width": "120px", "class": "left-align" },
        { "data": "", "width": "120px", "class": "center-align group-icon-action" }
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

        $('#idDelete').val(data.caNhanID);
        $('#nameDelete').text(`${data.hoTen}`);
        $('#modalDelete').modal('show');
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
}

function dataDelete() {
    let id = $('#idDelete').val();
    $.ajax({
        url: `${baseUrl}/api/DM_CaNhan_VanDongVienApi/Delete?caNhanID=${id}`,
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