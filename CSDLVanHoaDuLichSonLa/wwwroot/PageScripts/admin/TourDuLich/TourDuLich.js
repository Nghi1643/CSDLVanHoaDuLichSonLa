
const baseUrl = getRootLink();

$(document).ready(function () {
    initSelect2()
    initDatePicker()
    initToChuc(1, "#to-chuc-search");
    initTheLoai("#the-loai-search", "")
    initDanhMucChung(1, "#cach-to-chuc-search", "")
    initDanhMucChung(2, "#phan-khuc-search", "")
    initDanhMucChung(3, "#phuong-tien-search", "")
    initNgonNgu("#ngon-ngu-search")
    initTable()

    $('#tim-kiem').on('click', async function () {
        initTable()
    });

    $('#tat-ca').on('click', async function () {
        $('#tu-khoa-search').val("");
        $('#to-chuc-search').val("").trigger('change');
        $('#the-loai-search').val("").trigger('change');
        $("#cach-to-chuc-search").val("").trigger('change');
        $("#phan-khuc-search").val("").trigger('change');
        $("#phuong-tien-search").val("").trigger('change');
        $('#trang-thai-search').val("-1").trigger('change');
        $('#ngon-ngu-search').val("").trigger('change');
        initTable()
    });
})

function initTable() {
    const tableApi = {
        url: `${baseUrl}/api/TourDuLichApi/DanhSach`,
        type: "POST",
        data: function (d) {
            var toChucID = $('#to-chuc-search').val();
            var theLoaiID = $('#the-loai-search').val();
            var loaiHinhID = $('#cach-to-chuc-search').val();
            var phanKhucID = $('#phan-khuc-search').val();
            var phuongTienID = $('#phuong-tien-search').val();
            var trangThai = $('#trang-thai-search').val();
            var maNgonNgu = $('#ngon-ngu-search').val();
            return JSON.stringify({
                tuKhoa: $('#tu-khoa-search').val() || null,
                toChucID: toChucID == "-1" ? null : toChucID,
                theLoaiID: theLoaiID == "-1" ? null : theLoaiID,
                loaiHinhID: loaiHinhID == "-1" ? null : loaiHinhID,
                phanKhucID: phanKhucID == "-1" ? null : phanKhucID,
                phuongTienID: phuongTienID == "-1" ? null : phuongTienID,
                trangThai: trangThai == "-1" ? null : trangThai=="1",
                maNgonNgu: maNgonNgu == "-1" ? null : maNgonNgu
            });
        },
        contentType: 'application/json; charset=utf-8',
        dataSrc: function (data) {
            if (data && data.isSuccess && data.value.length > 0) {
                data.value.forEach((item, index) => {
                    item.stt = index + 1;

                    if (item.ngayKhoiHanh) {
                        const date = new Date(item.ngayKhoiHanh);
                        item.ngayKhoiHanhFormatted = date.toLocaleString('vi-VN', {
                            year: 'numeric',
                            month: '2-digit',
                            day: '2-digit',
                            hour: '2-digit',
                            minute: '2-digit',
                            hour12: false
                        });
                    }
                    if (item.ngayKetThuc) {
                        const date = new Date(item.ngayKetThuc);
                        item.ngayKetThucFormatted = date.toLocaleString('vi-VN', {
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
            targets: 1, // Cột tên cơ quan báo chí
            render: function (data, type, row, meta) {
                return `<div class="group-info">
                    <div class="info-main">
                        <a href="${baseUrl}/AdminTool/TourDuLich/Details?id=${row.tourID}" class="text-primary text-decoration-none">
                            ${row.tenTour || ''}
                        </a>
                    </div>
                    <div class="info-sub">Mã định danh: ${row.maDinhDanh || ''}</div>
                    <div class="info-sub">Thời gian: ${row.thoiGian || ''}</div>
                  </div>`;
            }
        },
        {
            targets: 2, // Thời gian, địa điểm
            render: function (data, type, row, meta) {
                return `<div class="group-info">
                            <div class="info-main">
                                   Khởi hành: ${row.tenDiemDi || ''}
                            </div>
                            <div class="info-sub">${row.ngayKhoiHanhFormatted || ''}</div>
                        </div>
                        <div class="group-info">
                            <div class="info-main">
                                   Kết Thúc: ${row.tenDiemDen || ''}
                            </div>
                            <div class="info-sub">${row.ngayKetThucFormatted || ''}</div>
                        </div>`
                    ;
            }
        },
        {
            targets: 3, // tổ chức
            render: function (data, type, row, meta) {
                return row.tenToChuc || '';
            }
        },
        {
            targets: 4,// Thể loại tour
            render: function (data, type, row, meta) {
                return row.tenTheLoai || "";
            }
        },
        //{
        //    targets: 5,// Cách tổ chức, triển khai
        //    render: function (data, type, row, meta) {
        //        return row.TenLoaiHinh || "";
        //    }
        //},
        {
            targets: 5,// Phân khúc, giá tour
            render: function (data, type, row, meta) {
                return `<div class="group-info">
                            <div class="info-main">
                                   ${row.tenPhanKhuc || ''}
                            </div>
                            <div class="info-sub">Giá tour: ${formatMoneyVND(row.giaTour)|| ''}</div>
                             <div class="info-sub">Giá Người(với tour ghép): ${formatMoneyVND(row.giaNguoi)|| ''}</div>
                        </div>`;
            }
        },
        {
            targets: 6, 
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
                    html += `<a href="${baseUrl}/AdminTool/TourDuLich/Edit?id=${row.tourID}" data-toggle="tooltip" title="Chỉnh sửa" class="text-yellow me-2">
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
        { "data": "", "width": "15%", "class": "left-align" },
        { "data": "", "width": "10%", "class": "left-align" },
        { "data": "", "width": "7%", "class": "left-align" },
        { "data": "", "width": "12%", "class": "left-align name-text" },
        { "data": "", "width": "8%", "class": "left-align name-text" },
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

        $('#idDelete').val(data.tourID);
        $('#nameDelete').text(`${data.tenTour}`);

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
        url: `${baseUrl}/api/TourDuLichApi/Xoa/${id}`,
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

function initToChuc(loaiToChucID, elementId = "#toChuc", placeholder = "Chọn tổ chức") {
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
function initTheLoai(elementId = "#the-loai-search", placeholder = "Chọn thể loại") {
    getDataWithApi('GET', '/api/TourDuLichApi/TheLoai/DanhSach?maNgonNgu=vi', null).then(response => {
        if (response && response.isSuccess && response.value) {
            $(elementId).empty()
                .append(`<option value="-1">Tất cả</option>`);

            response.value.forEach(item => {
                const displayText = item.tenTheLoai || item.noiDung || 'Không rõ tên';
                $(elementId).append(`<option value="${item.theLoaiID || item.id}">${displayText}</option>`);
            });
        } else {
            showNotification(0, "Không thể load danh sách thể loại");
        }
    }).catch(error => {
        console.error('Lỗi khi load thể loại:', error);
        showNotification(0, "Lỗi kết nối khi tải danh sách thể loại");
    });
}
function formatMoneyVND(amount) {
    if (amount == null || amount === '') return '';
    return Number(amount).toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
}