const baseUrl = getRootLink();

$(document).ready(function () {
    initDatePicker();
    initDanhMucChung(4, "#hinh-thuc-xuc-tien-search", "")// HinhThucID
    initDanhMucChung(5, "#phuong-thuc-truyen-thong-search", "")// TruyenThongID
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
        $('#hinh-thuc-xuc-tien-search').val("").trigger('change');
        $('#phuong-thuc-truyen-thong-search').val("").trigger('change');
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
        } finally {
            setTimeout(() => {
                submitBtn.prop('disabled', false);
                submitBtn.text(originalText);
            }, 1000);
        }
    });
});

function initTable() {
    console.log("gọi api");
    const tableApi = {
        url: `${baseUrl}/api/ThongTinXucTienApi/DanhSach`,
        type: "POST",
        data: function (d) {
            var hinhThucID = $('#hinh-thuc-xuc-tien-search').val()
            var truyenThongID = $('#phuong-thuc-truyen-thong-search').val()
            var maNgonNgu = $('#ngon-ngu-search').val()
            var trangThaiID = $('#trang-thai-search').val()
            return JSON.stringify({
                tuKhoa: $('#tu-khoa-search').val() || null,
                hinhThucID: hinhThucID == "-1" ? null : hinhThucID,
                truyenThongID: truyenThongID == "-1" ? null : truyenThongID,
                trangThaiID: trangThaiID == "-1" ? null : trangThaiID,
                maNgonNgu: maNgonNgu == "-1" ? null : maNgonNgu
            });
        },
        contentType: 'application/json; charset=utf-8',
        dataSrc: function (data) {
            if (data && data.isSuccess && data.value.length > 0) {
                data.value.forEach((item, index) => {
                    item.stt = index + 1;

                    if (item.ngayBatDau) {
                        const date = new Date(item.ngayBatDau);
                        item.ngayBatDauFormatted = date.toLocaleDateString('vi-VN');
                    }
                    if (item.ngayKetThuc) {
                        const date = new Date(item.ngayKetThuc);
                        item.ngayKetThucFormatted = date.toLocaleDateString('vi-VN');
                    }
                });
                return data.value;
            }
            return [];
        },
    };
    //<div class="info-sub">${row.diaChi || ''}</div>
    const tableDefs = [
        {
            targets: 1, // Cột Tên Tiêu đề
            render: function (data, type, row, meta) {
                return `<div class="group-info">
                    <div class="info-main">
                        <a href="${baseUrl}/AdminTool/ThongTinXucTien/Details?id=${row.xucTienID}" class="text-primary text-decoration-none">
                            ${row.tieuDe || ''}
                        </a>
                    </div>
                    
                </div>`;
            }
        },
        {
            targets: 2, // Cột Tên Tổ Chức
            render: function (data, type, row, meta) {
                return row.tenToChuc || '';
            }
        },
        {
            targets: 3, // Cột Thời gian 
            render: function (data, type, row, meta) {
                return `<div class="group-info">
                    <div class="info-main">
                        ${row.ngayBatDauFormatted ? "Từ: " + row.ngayBatDauFormatted + "<br/>" : ""}
                        ${row.ngayKetThucFormatted ? "Đến: " + row.ngayKetThucFormatted : ""}
                    </div>
                    
                </div>`;
            }
        },
        //<span style="font-size: 18px;">&#9733;</span>
        {
            targets: 4, // Cột Chiến lược
            render: function (data, type, row, meta) {
                return `<div class="group-info">
                            <span>${row.tenHinhThuc+'</br>' ?? ""}</span>
                            <span>${row.tenTruyenThong + '</br>' ?? ""}</span>
                            <span>${row.tenDoiTuong + '</br>' ?? ""}</span>
                        </div>`;
            }
        },
        {
            targets: 5, // Cột trạng thái
            render: function (data, type, row, meta) {
                return row.tenTrangThai;
            }
        },
        {
            targets: 6, // Cột chức năng
            render: function (data, type, row, meta) {
                let html = "";
                if (permitedEdit) {
                    html += `<a href="${baseUrl}/AdminTool/ThongTinXucTien/Edit?id=${row.xucTienID}" data-toggle="tooltip" title="Chỉnh sửa" class="text-yellow me-2">
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
        { "data": "tenCoSo", "class": "left-align name-text" },
        { "data": "loaiDichVu", "width": "15%", "class": "left-align" },
        { "data": "thoiGianHoatDong", "width": "15%", "class": "left-align" },
        { "data": "hangSao", "width": "15%", "class": "left-align" },
        { "data": "trangThai", "width": "8%", "class": "left-align" },
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

        $('#idDelete').val(data.xucTienID);
        $('#nameDelete').text(`${data.tieuDe}`);

        $('#modalDelete').modal('show');
    });
}

function dataDelete() {
    let id = $('#idDelete').val();
    $.ajax({
        url: `${baseUrl}/api/ThongTinXucTienApi/Xoa/${id}`,
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
