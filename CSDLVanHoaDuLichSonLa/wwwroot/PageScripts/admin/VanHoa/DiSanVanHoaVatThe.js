const baseUrl = getRootLink();

$(document).ready(function () {
    initSelect2();
    initDatePicker();
    let translations = [];

    (async function () {
        await Promise.all([
            initNgonNgu(),
            initCapXepHang(),
            initLoaiHinhDiTich(),
            initDonViQuanLy(),
            initBaoTang(),
            initChatLieu(),
            initLoaiHienVat()
        ]);
        initTable();
        initTableHienVat()

    })();

    $('#tim-kiem').on('click', async function () {
        initTable()
    });
    $('#tim-kiem-hien-vat').on('click', async function () {
        initTableHienVat()
    });

    $('#tat-ca').on('click', async function () {
        $('#tu-khoa-search').val('');
        $('#loai-hinh-di-tich-search').val('-1').trigger('change');
        $('#don-vi-quan-ly-search').val('-1').trigger('change');
        $('#cap-xep-hang-search').val('-1').trigger('change');
        $('#trang-thai-search').val('-1').trigger('change');
        $('#tinh-trang-search').val('-1').trigger('change');
        initTable()
    });

    $('#tat-ca-hien-vat').on('click', async function () {
        // Reset các ô tìm kiếm
        $('#tu-khoa-hien-vat-search').val('');

        // Reset các select về -1
        $('#chat-lieu-hien-vat-search').val('-1').trigger('change');
        $('#bao-tang-hien-vat-search').val('-1').trigger('change');
        $('#phuong-thuc-hien-vat-search').val('-1').trigger('change');
        $('#loai-hien-vat-search').val('-1').trigger('change');
        $('#tinh-trang-hien-vat-search').val('-1').trigger('change');
        $('#trang-thai-hien-vat-search').val('-1').trigger('change');
        $('#ngon-ngu-hien-vat-search').val('-1').trigger('change');

        // Load lại bảng dữ liệu hiện vật
        initTableHienVat();
    });

    $("#formDelete").on("submit", function (e) {
        e.preventDefault();
        let type = $('#idTypeDelete').val();
        if (type == 1)
            dataDeleteDiTich();
        else if (type == 2) {
            dataDeleteHienVat();
        }
    });
    async function initTable() {
        const tableApi = {
            url: `${baseUrl}/api/DiTichApi/DanhSach`,
            type: "POST",
            data: function (d) {
                var suDung = $('#trang-thai-search').val()
                var loaiHinh = $('#loai-hinh-di-tich-search').val()
                var donViQuanLy = $('#don-vi-quan-ly-search').val()
                var capXepHang = $('#cap-xep-hang-search').val()
                var tinhTrang = $('#tinh-trang-search').val()
                return JSON.stringify({
                    tuKhoa: $('#tu-khoa-search').val() || null,
                    maNgonNgu: $('#ngon-ngu-search').val() || 'vi',
                    loaiHinhID: loaiHinh == "-1" ? null : loaiHinh,
                    donViQuanLyID: donViQuanLy == "-1" ? null : donViQuanLy,
                    capXepHangID: capXepHang == "-1" ? null : capXepHang,
                    trangThai: suDung == "-1" ? null : (suDung === "1" ? true : false),
                    tinhTrangID: tinhTrang == "-1" ? null : (tinhTrang === "1" ? true : false)
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
                    console.log(data, row)
                    return `<div class="group-info have-image detail-command-btn" id=n-"${meta.row}">
                        <div class="have-image">
                            <img src="${row.anhDaiDien || '/assets/images/vector/no-image.png'}" alt="${row.tenDiTich || ''}" onerror="this.onerror=null;this.src='/assets/images/vector/no-image.png';" />
                        </div>
                        <div class="group-info">
                        <div class="info-main"><div>${row.tenDiTich || ''}</div></div>
                        <div class="info-sub">${row.maDinhDanh || ''}</div></div>
                    </div>`;
                }
            },
            {
                targets: 5,
                render: function (data, type, row, meta) {
                    if (data == 1) {
                        return `Tốt`;
                    } else if (data == 2) {
                        return `Trung bình`;
                    } else if (data == 3) {
                        return `Xuống cấp`;
                    } else if (data == 4) {
                        return `Đã trùng tu`;
                    }
                    return ``;
                }
            },
            {
                targets: 6,
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
                targets: 7,
                render: function (data, type, row, meta) {
                    let html = ""
                    if (1) {
                        html += `<i data-toggle="tooltip" title="Chỉnh sửa" class="edit-command-btn text-yellow" id=n-"${meta.row}">
                                   <i class="hgi-icon hgi-edit"></i>
                                </i>`;
                    }
                    if (1) {
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
            { "data": "tenDiTich", "width": "", "class": "left-align" },
            { "data": "tenCapXepHang", "width": "220px", "class": "left-align" },
            { "data": "tenToChucQuanLy", "width": "220px", "class": "center-align" },
            { "data": "tenLoaiHinh", "width": "120px", "class": "center-align" },
            { "data": "trangThaiID", "width": "120px", "class": "center-align" },
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

            let hef = `/DiTich/ChinhSua?id=${data.diTichID}`;
            window.location.href += hef;
        });
        $('#dataGrid tbody').on('click', '.delete-command-btn', function () {
            var id = $(this).attr("ID").match(/\d+/)[0];
            var data = $('#dataGrid').DataTable().row(id).data();

            $('#idDelete').val(data.diTichID);
            $('#idTypeDelete').val(1);
            $('#nameDelete').text(`${data.tenDiTich}`);

            $('#modalDelete').modal('show');
        });

        $('#dataGrid tbody').on('click', '.detail-command-btn', function () {
            var id = $(this).attr("ID").match(/\d+/)[0];
            var data = $('#dataGrid').DataTable().row(id).data();

            $('#linhVucChuyenSauDetail').text(data.tenLinhVucChuyenSau || "");
            $('#tanSuatDetail').text(data.tenTanSuat || "");
            //$('#thuTuDetail').text(data.thuTu);
            $('#trangThaiDetail').text(data.trangThai == true ? "Đã phát hành" : "Chưa phát hành");

            $('#ngonNguDetail').text(data.tenNgonNgu || "");
            $('#tenAnPhamDetail').text(data.tenAnPham || "");
            $('#doiTuongDocGiaDetail').text(data.doiTuongDocGia || "");

            // Thông tin audit
            $('#ngayTaoDetail').text(formatDateTime(data.ngayCapNhat));
            $('#nguoiTaoDetail').text(data.tenNguoiCapNhat || "");
            $('#ngayCapNhatDetail').text(formatDateTime(data.ngayHieuChinh));
            $('#nguoiCapNhatDetail').text(data.tenNguoiHieuChinh || "");

            $('#modalDetail').modal('show');
        });

    }

    async function dataDeleteDiTich() {
        let id = $('#idDelete').val();

        try {
            const res = await fetch(`/api/DiTichApi/Xoa/${id}`, {
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


    async function initTableHienVat() {
        const tableApi = {
            url: `${baseUrl}/api/HienVatApi/DanhSach`,
            type: "POST",
            data: function (d) {
                //var suDung = $('#trang-thai-search').val()
                //var loaiHinh = $('#loai-hinh-di-tich-search').val()
                //var donViQuanLy = $('#don-vi-quan-ly-search').val()
                //var capXepHang = $('#cap-xep-hang-search').val()
                //var tinhTrang = $('#tinh-trang-search').val()
                return JSON.stringify({
                    tuKhoa: $('#tu-khoa-hien-vat-search').val()?.trim() || null,
                    maNgonNgu: $('#ngon-ngu-hien-vat-search').val() || 'vi',
                    loaiHienVatID: $('#loai-hien-vat-search').val() === "-1" ? null : $('#loai-hien-vat-search').val(),
                    phuongThucTangMuaID: $('#phuong-thuc-hien-vat-search').val() === "-1" ? null : $('#phuong-thuc-hien-vat-search').val(),
                    chatLieuID: $('#chat-lieu-hien-vat-search').val() === "-1" ? null : $('#chat-lieu-hien-vat-search').val(),
                    baoTangID: $('#bao-tang-hien-vat-search').val() === "-1" ? null : $('#bao-tang-hien-vat-search').val(),
                    trangThaiID: $('#tinh-trang-hien-vat-search').val() === "-1" ? null : $('#tinh-trang-hien-vat-search').val(),
                    suDung: $('#trang-thai-hien-vat-search').val() === "-1" ? null : $('#trang-thai-hien-vat-search').val()

                })
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
                targets: 1, // Cột tên di sản hiện vật
                render: function (data, type, row, meta) {
                    return `<div class="group-info have-image">
                        <div class="have-image">
                            <img src="${row.anhDaiDien || '/assets/images/vector/no-image.png'}" alt="${row.tenHienVat || ''}" onerror="this.onerror=null;this.src='/assets/images/vector/no-image.png';" />
                        </div>
                        <div class="group-info">
                        <div class="info-main"><div>${row.tenHienVat || ''}</div></div>
                        <div class="info-sub">${row.maHienVat || ''}</div></div>
                    </div>`;
                }
            },
            {
                targets: 4,
                render: function (data, type, row, meta) {
                    if (data == 1) {
                        return `Mua`;
                    } else if (data == 2) {
                        return `Hiến tặng`;
                    }
                    return ``;
                }
            },
            {
                targets: 7,
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
                targets: 8,
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
            { "data": "tenHienVat", "width": "25%", "class": "left-align" },
            { "data": "tenLoaiHienVat", "width": "120px", "class": "left-align" },
            { "data": "tenChatLieu", "width": "120px", "class": "left-align" },
            { "data": "phuongThucTangMuaID", "width": "120px", "class": "center-align" },
            { "data": "tenBaoTang", "width": "120px", "class": "center-align" },
            { "data": "soLuong", "width": "120px", "class": "center-align" },
            { "data": "suDung", "width": "120px", "class": "center-align" },
            { "data": "", "width": "120px", "class": "center-align group-icon-action" },
        ];

        if (!permitedEdit && !permitedDelete) {
            tableCols.pop();
            tableDefs.pop();
        }

        initDataTableConfigNoSearch('dataGridHienVat', tableApi, tableDefs, tableCols);

        $('#dataGridHienVat tbody').on('click', '.edit-command-btn', async function () {
            var id = $(this).attr("ID").match(/\d+/)[0];
            var data = $('#dataGridHienVat').DataTable().row(id).data();

            let hef = `/HienVat/ChinhSua?id=${data.hienVatID}`;
            window.location.href += hef;
        });
        $('#dataGridHienVat tbody').on('click', '.delete-command-btn', function () {
            var id = $(this).attr("ID").match(/\d+/)[0];
            var data = $('#dataGridHienVat').DataTable().row(id).data();

            $('#idDelete').val(data.hienVatID);
            $('#idTypeDelete').val(2);
            $('#nameDelete').text(`${data.tenHienVat}`);

            $('#modalDelete').modal('show');
        });

        //$('#dataGrid tbody').on('click', '.detail-command-btn', function () {
        //    var id = $(this).attr("ID").match(/\d+/)[0];
        //    var data = $('#dataGrid').DataTable().row(id).data();

        //    $('#linhVucChuyenSauDetail').text(data.tenLinhVucChuyenSau || "");
        //    $('#tanSuatDetail').text(data.tenTanSuat || "");
        //    //$('#thuTuDetail').text(data.thuTu);
        //    $('#trangThaiDetail').text(data.trangThai == true ? "Đã phát hành" : "Chưa phát hành");

        //    $('#ngonNguDetail').text(data.tenNgonNgu || "");
        //    $('#tenAnPhamDetail').text(data.tenAnPham || "");
        //    $('#doiTuongDocGiaDetail').text(data.doiTuongDocGia || "");

        //    // Thông tin audit
        //    $('#ngayTaoDetail').text(formatDateTime(data.ngayCapNhat));
        //    $('#nguoiTaoDetail').text(data.tenNguoiCapNhat || "");
        //    $('#ngayCapNhatDetail').text(formatDateTime(data.ngayHieuChinh));
        //    $('#nguoiCapNhatDetail').text(data.tenNguoiHieuChinh || "");

        //    $('#modalDetail').modal('show');
        //});

    }

    async function dataDeleteHienVat() {
        let id = $('#idDelete').val();

        try {
            const res = await fetch(`/api/HienVatApi/Xoa/${id}`, {
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
                $('#dataGridHienVat').DataTable().ajax.reload();
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

    async function initLoaiHinhDiTich() {
        try {
            const res = await fetch('/api/DanhMucChungApi/DanhSach', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    loaiDanhMucID: 18,
                    tuKhoa: null
                })
            })

            if (!res.ok) {
                var errText = await res.text();
                throw new Error(errText);
            }
            data = await res.json();

            if (data && data.isSuccess && data.value) {
                $("#loai-hinh-di-tich-search").empty();

                $("#loai-hinh-di-tich-search").append(`<option value="-1">Tất cả</option>`);
                data.value.forEach(el => {
                    $("#loai-hinh-di-tich-search").append(`<option value="${el.danhMucID}">${el.tenDanhMuc}</option>`);
                });
            } else {
                showNotification(0, data.error)
            }
        }
        catch (err) {
            console.log(err.message)
        }
    };

    async function initCapXepHang() {
        try {
            const res = await fetch('/api/DanhMucChungApi/DanhSach', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    loaiDanhMucID: 17,
                    tuKhoa: null
                })
            })

            if (!res.ok) {
                var errText = await res.text();
                throw new Error(errText);
            }
            data = await res.json();

            if (data && data.isSuccess && data.value) {
                $("#cap-xep-hang-search").append(`<option value="-1">Tất cả</option>`);
                data.value.forEach(el => {
                    $("#cap-xep-hang-search").append(`<option value="${el.danhMucID}">${el.tenDanhMuc}</option>`);
                });
            } else {
                showNotification(0, data.error)
            }
        }
        catch (err) {
            console.log(err.message)
        }
    };

    async function initDonViQuanLy() {
        try {
            const res = await fetch('/api/ToChucApi/DanhSach', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    loaiToChucID: 122
                })
            })

            if (!res.ok) {
                var errText = await res.text();
                throw new Error(errText);
            }
            data = await res.json();


            if (data && data.isSuccess && data.value) {
                $("#don-vi-quan-ly-search").empty();
                $("#don-vi-quan-ly-search").append(`<option value="-1">Tất cả<option>`);
                data.value.forEach(el => {
                    $("#don-vi-quan-ly-search").append(`<option value="${el.toChucID}">${el.tenToChuc}</option>`);
                });
            } else {
                showNotification(0, data.error)
            }
        }
        catch (err) {
            console.log(err.message)
        }
    };

    async function initChatLieu() {
        try {
            const res = await fetch('/api/DanhMucChungApi/DanhSach', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    loaiDanhMucID: 35,
                    tuKhoa: null
                })
            })

            if (!res.ok) {
                var errText = await res.text();
                throw new Error(errText);
            }
            data = await res.json();

            if (data && data.isSuccess && data.value) {
                data.value.forEach(el => {
                    $("#chat-lieu-hien-vat-search").append(`<option value="${el.danhMucID}">${el.tenDanhMuc}</option>`);
                });
            } else {
                showNotification(0, data.error)
            }
        }
        catch (err) {
            console.log(err.message)
        }
    };

    async function initBaoTang() {
        try {
            const res = await fetch('/api/ToChucApi/DanhSach', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    loaiToChucID: 95
                })
            })

            if (!res.ok) {
                var errText = await res.text();
                throw new Error(errText);
            }
            data = await res.json();


            if (data && data.isSuccess && data.value) {
                data.value.forEach(el => {
                    $("#bao-tang-hien-vat-search").append(`<option value="${el.toChucID}">${el.tenToChuc}</option>`);
                });
            } else {
                showNotification(0, data.error)
            }
        }
        catch (err) {
            console.log(err.message)
        }
    };

    async function initLoaiHienVat() {
        try {
            const res = await fetch('/api/DanhMucChungApi/DanhSach', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    loaiDanhMucID: 45,
                    tuKhoa: null
                })
            })

            if (!res.ok) {
                var errText = await res.text();
                throw new Error(errText);
            }
            data = await res.json();

            if (data && data.isSuccess && data.value) {
                $("#loai-hien-vat-search").empty();

                $("#loai-hien-vat-search").append(`<option value="-1">Tất cả</option>`);
                data.value.forEach(el => {
                    $("#loai-hien-vat-search").append(`<option value="${el.danhMucID}">${el.tenDanhMuc}</option>`);
                });
            } else {
                showNotification(0, data.error)
            }
        }
        catch (err) {
            console.log(err.message)
        }
    };
    //async function initLinhVuc() {
    //    try {
    //        const res = await fetch(`/api/LinhVucApi/Gets`, {
    //            method: 'Get'
    //        })
    //        if (!res.ok) {
    //            var errText = await res.text();
    //            throw new Error(errText);
    //        }
    //        var data = await res.json()

    //        for (let i = 0; i < data.value.length; i++) {
    //            $('#linh-vuc-hien-vat-search').append(`<option value="${data.value[i].linhVucID}">${data.value[i].ten}</option>`)
    //        }

    //        return data
    //    }
    //    catch (err) {
    //        showNotification(0, err.message)
    //    }
    //}
    //#endregion HÀM LOAD DỮ LIỆU CHO CÁC COMBOBOX
})

