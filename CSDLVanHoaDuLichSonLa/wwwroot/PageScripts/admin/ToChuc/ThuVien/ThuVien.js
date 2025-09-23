const baseUrl = getRootLink();

$(document).ready(function () {
    initDatePicker();
    initDanhMucChung(22, "#loai-hinh-search", "")
    initNgonNgu("#ngon-ngu-search")
    initSelect2();
    initTable();

    $('#tim-kiem').on('click', async function () {
        initTable()
    });

    $('#tat-ca').on('click', async function () {
        $('#tu-khoa-search').val("");
        $('#co-quan-chu-quan-search').val("").trigger('change');
        $('#loai-hinh-search').val("").trigger('change');
        $('#pham-vi-hoat-dong-search').val("").trigger('change');
        $('#trang-thai-search').val("-1").trigger('change');
        $('#ngon-ngu-search').val("").trigger('change');
        initTable()
    });

    document.getElementById("tu-khoa-search").addEventListener("keypress", function (event) {
        if (event.key === "Enter") {
            event.preventDefault();
            initTable()
        }
    });

    $("#formDelete").on("submit", async function (e) {
        e.preventDefault();
        
        const submitBtn = $(this).find('button[type="submit"]');
        if (submitBtn.prop('disabled')) {
            return;
        }
        
        submitBtn.prop('disabled', true);
        const originalText = submitBtn.text();
        submitBtn.text('Đang xóa...');
        
        try {
            await dataDelete();
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
        url: `${baseUrl}/api/ToChucApi/DanhSach`,
        type: "POST",
        data: function (d) {
            var coQuanChuQuan = $('#co-quan-chu-quan-search').val()
            var loaiHinhID = $('#loai-hinh-search').val()
            //var phamViHoatDongID = $('#pham-vi-hoat-dong-search').val()
            var maNgonNgu = $('#ngon-ngu-search').val()
            var suDung = $('#trang-thai-search').val()

            console.log(coQuanChuQuan, loaiHinhID, maNgonNgu, suDung);
            return JSON.stringify({
                loaiToChucID: 94, // ID cho thư viện
                tuKhoa: $('#tu-khoa-search').val() || null,
                coQuanChuQuan: coQuanChuQuan == "-1" ? null : coQuanChuQuan,
                loaiHinhID: loaiHinhID == "-1" ? null : loaiHinhID,
                //phamViHoatDongID: phamViHoatDongID == "-1" ? null : phamViHoatDongID,
                trangThaiID: suDung == "-1" ? null : Number(suDung),
                maNgonNgu: maNgonNgu == "-1" ? null : maNgonNgu
            });
        },
        contentType: 'application/json; charset=utf-8',
        dataSrc: function (data) {
            if (data && data.isSuccess && data.value.length > 0) {
                console.log(data)
                data.value.forEach((item, index) => {
                    item.stt = index + 1;
                    // Format ngày thành lập
                    if (item.ngayThanhLap) {
                        const date = new Date(item.ngayThanhLap);
                        item.ngayThanhLapFormatted = date.toLocaleDateString('vi-VN');
                    }
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
                return `<div class="group-info">
                    <div class="info-main">
                        <a href="${baseUrl}/AdminTool/ThuVien/Details?id=${row.toChucID}" class="text-primary text-decoration-none">
                            ${row.tenToChuc || ''}
                        </a>
                    </div>
                    <div class="info-sub">${row.maDinhDanh || ''}</div>
                </div>`;
            }
        },
        {
            targets: 4, // Cột số lượng cán bộ
            render: function (data, type, row, meta) {
                return formatDateWithoutTime(row.ngayThanhLap);
            }
        },
        {
            targets: 5, // Cột Giải thưởng
            render: function (data, type, row, meta) {
                return `<div class="group-info">
                    <div class="info-main">
                        <a href="${baseUrl}/AdminTool/ThuVien/AnPhamKenhPhatSong?id=${row.toChucID}" class="text-primary text-decoration-none">
                            ${row.soLuongGiaiThuong || 0}
                        </a>
                    </div>
                </div>`;
            }
        },
        {
            targets: 6, // Cột trạng thái
            render: function (data, type, row, meta) {
                if (row.trangThaiID == 1) {
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
                    html += `<a href="${baseUrl}/AdminTool/ThuVien/Add?id=${row.toChucID}" data-toggle="tooltip" title="Chỉnh sửa" class="text-yellow me-2">
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
        { "data": "tenToChuc", "class": "left-align name-text" },
        { "data": "loaiHinh", "width": "12%", "class": "left-align" },
        { "data": "quyMo", "width": "10%", "class": "left-align" },
        { "data": "", "width": "10%", "class": "left-align" },
        { "data": "soLuongGiaiThuong", "width": "12%", "class": "left-align name-text" },
        { "data": "trangThaiID", "width": "8%", "class": "left-align" },
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

        $('#idDelete').val(data.toChucID);
        $('#nameDelete').text(`${data.tenToChuc}`);

        $('#modalDelete').modal('show');
    });
}

function dataDelete() {
    let id = $('#idDelete').val();
    $.ajax({
        url: `${baseUrl}/api/ToChucApi/Xoa/${id}`,
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