const baseUrl = getRootLink();

$(document).ready(function () {
    initDatePicker();
    initDanhMucChung(27, "#loai-dich-vu-an-uong-search", "")
    initNgonNgu("#ngon-ngu-search")
    initSelect2();
    initTable();

    $('#tim-kiem').on('click', async function () {
        initTable()
    });

    $('#tat-ca').on('click', async function () {
        $('#tu-khoa-search').val("");
        $('#loai-dich-vu-an-uong-search').val("").trigger('change');
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
function formatTime(timeStr) {
    if (!timeStr) return "";
    const [hour, minute] = timeStr.split(":");
    return `${hour.padStart(2, "0")}:${minute.padStart(2, "0")}`;
}
function initTable() {
    const tableApi = {
        url: `${baseUrl}/api/CoSoAnUongApi/DanhSach`,
        type: "POST",
        data: function (d) {
            var loaiDichVuAnUongID = $('#loai-dich-vu-an-uong-search').val()
            var maNgonNgu = $('#ngon-ngu-search').val()
            var trangThai = $('#trang-thai-search').val()
            return JSON.stringify({
                tuKhoa: $('#tu-khoa-search').val() || null,
                loaiDichVuAnUongID: loaiDichVuAnUongID == "-1" ? null : loaiDichVuAnUongID,
                trangThai: trangThai == "-1" ? null : (trangThai === "1"),
                maNgonNgu: maNgonNgu == "-1" ? null : maNgonNgu
            });
        },
        contentType: 'application/json; charset=utf-8',
        dataSrc: function (data) {
            if (data && data.isSuccess && data.value.length > 0) {
                data.value.forEach((item, index) => {
                    item.stt = index + 1;
                    // Format ngày thành lập
                    //if (item.ngayThanhLap) {
                    //    const date = new Date(item.ngayThanhLap);
                    //    item.ngayThanhLapFormatted = date.toLocaleDateString('vi-VN');
                    //}
                });
                return data.value;
            }
            return [];
        },
    };

    const tableDefs = [
        {
            targets: 1, // Cột Tên cơ sở
            render: function (data, type, row, meta) {
                return `<div class="group-info">
                    <div class="info-main">
                        <a href="${baseUrl}/AdminTool/CoSoAnUong/Details?id=${row.coSoAnUongID}" class="text-primary text-decoration-none">
                            ${row.tenCoSo || ''}
                        </a>
                    </div>
                    <div class="info-sub">${row.diaChi || ''}</div>
                </div>`;
            }
        },
        {
            targets: 2, // Cột Loại dịch vụ
            render: function (data, type, row, meta) {
                return row.tenLoaiDichVuAnUong || '';
            }
        },
        {
            targets: 3, // Cột Thời gian hoạt động
            render: function (data, type, row, meta) {
                return `<div class="group-info">
                    <div class="info-main">
                        ${row.gioMoCua ? "Từ: " + formatTime(row.gioMoCua) + "<br/>" : ""}
                        ${row.gioDongCua ? "Đến: " + formatTime(row.gioDongCua) : ""}
                    </div>
                    <div class="info-sub" style="color:red"> ${row.ngayNghiHangTuan ? "Ngày nghỉ: " + row.ngayNghiHangTuan :''}</div>
                </div>`;
            }
        },
        //<span style="font-size: 18px;">&#9733;</span>
        {
            targets: 4, // Cột điểm đánh giá
            render: function (data, type, row, meta) {
                return `<div class="group-info">
                            <span>${row.diemDanhGia ? row.diemDanhGia+"/5":"-" }</span>
                        </div>`;
            }
        },
        {
            targets: 5, // Cột trạng thái
            render: function (data, type, row, meta) {
                if (row.trangThai == 1) {
                    return `<span class="TrangThai green-text">Duyệt </span>`;
                } else {
                    return `<span class="TrangThai red-text">Chưa duyệt</span>`;
                }
            }
        },
        {
            targets: 6, // Cột chức năng
            render: function (data, type, row, meta) {
                let html = "";
                if (permitedEdit) {
                    html += `<a href="${baseUrl}/AdminTool/CoSoAnUong/Edit?id=${row.coSoAnUongID}" data-toggle="tooltip" title="Chỉnh sửa" class="text-yellow me-2">
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
        { "data": "thoiGianHoatDong", "width": "20%", "class": "left-align" },
        { "data": "hangSao", "width": "10%", "class": "left-align" },
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

        $('#idDelete').val(data.coSoAnUongID);
        $('#nameDelete').text(`${data.tenCoSo}`);

        $('#modalDelete').modal('show');
    });
}

function dataDelete() {
    let id = $('#idDelete').val();
    $.ajax({
        url: `${baseUrl}/api/CoSoAnUongApi/Xoa/${id}`,
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
