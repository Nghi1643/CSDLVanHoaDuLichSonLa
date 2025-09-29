const baseUrl = getRootLink();

$(document).ready(function () {
    initSelect2();
    initDatePicker();

    (async function () {
        await Promise.all([
            initNgonNgu(),
            initKieuMon(),
            initCheDoAn()
        ]);
        initTable();
    })();

    $('#tim-kiem').on('click', async function () {
        initTable()
    });

    $('#tat-ca').on('click', async function () {
        $('#tu-khoa-search').val('');
        $('#kieu-mon-search').val('-1').trigger('change');
        $('#che-do-an-search').val('-1').trigger('change');
        $('#dac-san-search').val('-1').trigger('change');
        $('#trang-thai-search').val('-1').trigger('change');
        $('#ngon-ngu-search').val('vi').trigger('change');
        initTable()
    });


    $("#formDelete").on("submit", function (e) {
        e.preventDefault();
        dataDeleteAmThuc();
    });
    async function initTable() {
        const tableApi = {
            url: `${baseUrl}/api/AmThucApi/DanhSach`,
            type: "POST",
            data: function (d) {
                var dacSan = $('#dac-san-search').val()
                var kieuMon = $('#kieu-mon-search').val()
                var cheDoAn = $('#che-do-an-search').val()
                var suDung = $('#trang-thai-search').val()
                return JSON.stringify({
                    tuKhoa: $('#tu-khoa-search').val() || null,
                    maNgonNgu: $('#ngon-ngu-search').val() || 'vi',
                    kieuMonID: kieuMon == "-1" ? null : kieuMon,
                    dacSan: dacSan == "-1" ? null : (dacSan === "1" ? true : false),
                    cheDoAnID: cheDoAn == "-1" ? null : cheDoAn,
                    suDung: suDung == "-1" ? null : (suDung === "1" ? true : false)
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
                    return `<div class="group-info have-image detail-command-btn" id=n-"${meta.row}">
                        <div class="have-image">
                            <img src="${row.anhDaiDien || '/assets/images/vector/no-image.png'}" alt="${row.tenMon || ''}" onerror="this.onerror=null;this.src='/assets/images/vector/no-image.png';" />
                        </div>
                        <div class="group-info">
                        <div class="info-main"><div>${row.tenMon || ''}</div></div>
                        <div class="info-sub">${row.kieuMon || ''}</div></div>
                    </div>`;
                }
            },
            {
                targets: [4,5],
                render: function (data, type, row, meta) {
                    if (data) {
                        return `<i class="hgi-icon hgi-check"></i>`;
                    }
                    else {
                        return `<i class="hgi-icon hgi-cancel"></i>`;
                    }
                }
            },
            {
                targets: 6,
                render: function (data, type, row, meta) {
                    let html = ""
                    if (permitedEdit) {
                        html += `<i data-toggle="tooltip" title="Chỉnh sửa" class="edit-command-btn text-yellow" id=n-"${meta.row}">
                                   <i class="hgi-icon hgi-edit"></i>
                                </i>`;
                    }
                    if (permitedDelete) {
                        html += `<i data-toggle="tooltip" title="Xoá" class="delete-command-btn text-red" id=n-"${meta.row}">
                                   <i class="hgi-icon hgi-delete"></i>
                                </i>`
                    }
                    return html
                }
            }
        ];

        const tableCols = [
            { "data": "stt", "width": "40px", "class": "center-align" },
            { "data": "tenMon", "width": "", "class": "left-align" },
            { "data": "tenKieuMon", "width": "220px", "class": "left-align" },
            { "data": "tenCheDoAn", "width": "220px", "class": "center-align" },
            { "data": "dacSan", "width": "120px", "class": "center-align" },
            { "data": "suDung", "width": "120px", "class": "center-align" },
            { "data": "", "width": "120px", "class": "center-align group-icon-action" },
        ];

        //if (!permitedEdit && !permitedDelete) {
        //    tableCols.pop();
        //    tableDefs.pop();
        //}

        initDataTableConfigNoSearch('dataGrid', tableApi, tableDefs, tableCols);

        $('#dataGrid tbody').on('click', '.edit-command-btn', async function () {
            var id = $(this).attr("ID").match(/\d+/)[0];
            var data = $('#dataGrid').DataTable().row(id).data();

            let hef = `/ChinhSua?id=${data.monAnUongID}`;   
            window.location.href += hef;
        });
        $('#dataGrid tbody').on('click', '.delete-command-btn', function () {
            var id = $(this).attr("ID").match(/\d+/)[0];
            var data = $('#dataGrid').DataTable().row(id).data();

            $('#idDelete').val(data.monAnUongID);
            $('#nameDelete').text(`${data.tenMon}`);

            $('#modalDelete').modal('show');
        });

        //Lấy chi tiết
        //$('#dataGrid tbody').on('click', '.detail-command-btn', function () {
        //    var id = $(this).attr("ID").match(/\d+/)[0];
        //    var data1 = $('#dataGrid').DataTable().row(id).data();
        //    $.ajax({
        //        url: `${baseUrl}/api/AmThucApi/ChiTiet/${data1.monAnUongID}`,
        //        type: "GET",
        //        success: function (res) {
        //            if (res.isSuccess && res.value) {
        //                var data = res.value;

        //                // Lấy mã ngôn ngữ hiện tại từ select
        //                var currentLang = $("#ngon-ngu-search").val();

        //                // Thông tin cơ bản
        //                $("#anhDaiDienDetail").attr("src", data1.anhDaiDien || "/images/no-image.png");
        //                $("#kieuMonIDDetail").text(data1.tenKieuMon || "");
        //                $("#cheDoAnIDDetail").text(data1.tenCheDoAn || "");
        //                $("#dacSanDetail").text(data1.dacSan ? "Món đặc sản" : "Món thông thường");
        //                $("#thuTuDetail").text(data1.thuTu || "");
        //                $("#suDungDetail").text(data1.suDung ? "Sử dụng" : "Không sử dụng");

        //                // Thông tin đa ngữ (theo mã ngôn ngữ đã chọn)
        //                var banDich = data.banDich.find(x => x.maNgonNgu === currentLang);

        //                $("#tenMon").text(banDich?.tenMon || data.tenMon || "");
        //                $("#nguyenLieuDetail").text(banDich?.nguyenLieu || "");
        //                $("#traiNghiemDetail").text(banDich?.traiNghiem || "");
        //                $("#moTaDetail").text(banDich?.moTa || "");

        //                $('#modalDetail').modal('show');
        //            } else {
        //                alert("Không lấy được dữ liệu chi tiết.");
        //            }
        //        },
        //        error: function () {
        //            alert("Có lỗi xảy ra khi gọi API.");
        //        }
        //    });
        //});
        $('#dataGrid tbody').on('click', '.detail-command-btn', function () {
            var id = $(this).attr("ID").match(/\d+/)[0];
            var data = $('#dataGrid').DataTable().row(id).data();

            console.log(data)

            // Thông tin cơ bản
            $("#anhDaiDienDetail").attr("src", data.anhDaiDien || "/images/no-image.png");
            $("#kieuMonIDDetail").text(data.tenKieuMon || "");
            $("#cheDoAnIDDetail").text(data.tenCheDoAn || "");
            $("#thuTuDetail").text(data.thuTu || "");
            $("#suDungDetail").text(data.suDung ? "Sử dụng" : "Không sử dụng");
            $("#dacSanDetail").text(data.dacSan ? "Món đặc sản" : "Món thông thường");

            // Thông tin đa ngữ (hiển thị theo ngôn ngữ hiện tại)
            $("#tenMonDetail").text(data.tenMon || "");
            $("#nguyenLieuDetail").text(data.nguyenLieu || "");
            $("#traiNghiemDetail").text(data.traiNghiem || "");
            $("#moTaDetail").text(data.moTa || "");

            $('#modalDetail').modal('show');
        });
    }

    async function dataDeleteAmThuc() {
        let id = $('#idDelete').val();

        try {
            const res = await fetch(`/api/AmThucApi/Xoa/${id}`, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json'
                }
            })

            if (!res.ok) {
                var errText = await res.text();
                throw new Error(errText);
            }

            data = await res.json();

            if (data && data.isSuccess && data.value) {
                showNotification(1, 'Xoá thành công')
                $('#modalDelete').modal('hide');
                $('#dataGrid').DataTable().ajax.reload();
            } else {
                console.log(data.error)
            }

        }
        catch (err) {
            showNotification(0, err.message)
        }
    }

    //#region HÀM LOAD DỮ LIỆU CHO CÁC COMBOBOX
    async function initNgonNgu() {
        try {
            const res = await fetch('/api/NgonNguApi/DanhSach', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    trangThai: null,
                    tuKhoa: null
                })
            })

            if (!res.ok) {
                var errText = await res.text();
                throw new Error(errText);
            }
            data = await res.json();

            if (data && data.isSuccess && data.value) {
                $("#ngon-ngu-search").empty();
                $("#ngonNguAdd").empty();
                $("#ngon-ngu-hien-vat-search").empty();
                //$("#ngon-ngu-search").append(`<option value="">Tất cả</option>`);
                data.value.forEach(lang => {
                    $("#ngon-ngu-search").append(`<option value="${lang.maNgonNgu?.toLowerCase()}">${lang.tenNgonNgu}</option>`);
                    $("#ngonNguAdd").append(`<option value="${lang.maNgonNgu?.toLowerCase()}">${lang.tenNgonNgu}</option>`);
                    $("#ngon-ngu-hien-vat-search").append(`<option value="${lang.maNgonNgu?.toLowerCase()}">${lang.tenNgonNgu}</option>`);
                });
                $("#ngon-ngu-search").val('vi').trigger('change');
                $("#ngonNguAdd").val('vi').trigger('change');
                $("#ngon-ngu-hien-vat-search").val('vi').trigger('change');
            } else {
                showNotification(0, data.error)
            }
        }
        catch (err) {
            showNotification(0, err.message)
        }
    };

    async function initKieuMon() {
        try {
            const res = await fetch('/api/DanhMucChungApi/DanhSach', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    loaiDanhMucID: 16,
                    tuKhoa: null
                })
            })

            if (!res.ok) {
                var errText = await res.text();
                throw new Error(errText);
            }
            data = await res.json();

            if (data && data.isSuccess && data.value) {

                $("#kieu-mon-search").append(`<option value="-1">Tất cả</option>`);
                data.value.forEach(el => {
                    $("#kieu-mon-search").append(`<option value="${el.danhMucID}">${el.tenDanhMuc}</option>`);
                });
            } else {
                showNotification(0, data.error)
            }
        }
        catch (err) {
            console.log(err.message)
        }
    };

    async function initCheDoAn() {
        try {
            const res = await fetch('/api/DanhMucChungApi/DanhSach', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    loaiDanhMucID: 16,
                    tuKhoa: null
                })
            })

            if (!res.ok) {
                var errText = await res.text();
                throw new Error(errText);
            }
            data = await res.json();

            if (data && data.isSuccess && data.value) {
                $("#che-do-an-search").append(`<option value="-1">Tất cả</option>`);
                data.value.forEach(el => {
                    $("#che-do-an-search").append(`<option value="${el.danhMucID}">${el.tenDanhMuc}</option>`);
                });
            } else {
                showNotification(0, data.error)
            }
        }
        catch (err) {
            console.log(err.message)
        }
    };

    //#endregion HÀM LOAD DỮ LIỆU CHO CÁC COMBOBOX
})

