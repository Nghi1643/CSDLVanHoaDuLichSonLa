const baseUrl = getRootLink();
console.log(nhaXuatBanID)
$(document).ready(function () {
    initSelect2();
    let translations = [];
    
    (async function () {
        initThongTinToChuc(nhaXuatBanID)
        initNgonNgu();
        await initTheLoaiAnPham();
        initNhaXuatBan()
        initTable(nhaXuatBanID);
    })();

    $('#tim-kiem').on('click', async function () {
        initTable(nhaXuatBanID)
    });

    $('#tat-ca').on('click', async function () {
        $('#tu-khoa-search').val('');
        $('#the-loai-search').val('-1').trigger('change');
        $('#nha-xuat-ban-search').val('-1').trigger('change');
        $('#trang-thai-search').val('-1').trigger('change');
        initTable(nhaXuatBanID)
    });

    document.getElementById("tu-khoa-search").addEventListener("keypress", function (event) {
        if (event.key === "Enter") {
            event.preventDefault();
            initTable(nhaXuatBanID)
        }
    });

    async function initThongTinToChuc(coQuanBaoChiID) {
        try {
            const res = await fetch(`${baseUrl}/api/ToChucApi/BanDich/${coQuanBaoChiID}?maNgonNgu=vi`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            if (res.ok) {
                const data = await res.json();
                if (data && data.isSuccess && data.value) {
                    const translation = data.value[0];
                    $('.TieuDe h2').text(`Quản lý ấn phẩm xuất bản ${translation.tenToChuc || ''}`);
                }
            } else {
                console.log('Không tìm thấy bản dịch cho ngôn ngữ:', maNgonNgu);
            }
        } catch (err) {
            console.log('Lỗi khi tải bản dịch:', err.message);
        }
    }

    async function initTable(nhaXuatBanID) {
        const tableApi = {
            url: `${baseUrl}/api/XuatBanAnPhamApi/DanhSach`,
            type: "POST",
            data: function (d) {
                var suDung = $('#trang-thai-search').val()
                var nhaXuatBan = $('#nha-xuat-ban-search').val()
                var theLoai = $('#the-loai-search').val()
                return JSON.stringify({
                    tuKhoa: $('#tu-khoa-search').val() || null,
                    maNgonNgu: 'vi',
                    theLoaiID: theLoai == "-1" ? null : theLoai,
                    nhaXuatBanID: nhaXuatBanID ? nhaXuatBanID : null,
                    trangThai: suDung == "-1" ? null : (suDung === "1" ? true : false)    
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
                targets: 1, // Cột tên cơ quan báo chí
                render: function (data, type, row, meta) {
                    return `<div class="group-info have-image">
                        <div class="have-image">
                            <img src="${row.duongDanHinhAnhBia || '/assets/images/vector/no-image.png'}" alt="${row.tenAnPham || ''}" onerror="this.onerror=null;this.src='/assets/images/vector/no-image.png';" />
                        </div>
                        <div class="group-info">
                        <div class="info-main"><div>${row.tenAnPham || ''}</div></div>
                        <div class="info-sub">${row.maDinhDanhXBAP || ''}</div></div>
                    </div>`;
                }
            },
            {
                targets: 3, // Năm xuất bản
                render: function (data, type, row, meta) {
                    return data || '-';
                }
            },
            {
                targets: 5, // Giá bìa
                render: function (data, type, row, meta) {
                    if (data && data > 0) {
                        return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(data);
                    }
                    return '-';
                }
            },
            {
                targets: 6,
                render: function (data, type, row, meta) {
                    if (row.trangThai) {
                        return `<span class="TrangThai green-text">Đã phát hành</span>`;
                    }
                    else {
                        return `<span class="TrangThai yellow-text">Chưa phát hành</span>`;
                    }
                }
            },
            {
                targets: 7,
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
            { "data": "tenAnPham", "width": "25%", "class": "left-align" },
            { "data": "tenTheLoai", "width": "120px", "class": "left-align" },
            { "data": "namXuatBan", "width": "100px", "class": "left-align" },
            { "data": null, "width": "100px", "class": "left-align", "defaultContent": "-" },
            { "data": "giaBia", "width": "100px", "class": "left-align" },
            { "data": "trangThai", "width": "120px", "class": "left-align" },
            { "data": null, "width": "120px", "class": "center-align group-icon-action" }
        ];

        if (!permitedEdit && !permitedDelete) {
            tableCols.pop();
            tableDefs.pop();
        }

        initDataTableConfigNoSearch('dataGrid', tableApi, tableDefs, tableCols);

        $('#dataGrid tbody').on('click', '.edit-command-btn', async function () {
            var id = $(this).attr("ID").match(/\d+/)[0];
            var data = $('#dataGrid').DataTable().row(id).data();
            $('#idAnPhamAdd').val(data.xuatBanAnPhamID);
            if (data.duongDanHinhAnhBia) {
                // Nếu muốn preview ảnh bìa đã có sẵn từ server
                $('#showAnhBiaAdd').attr('src', data.duongDanHinhAnhBia);
            }
                
            // Các input text
            $('#maDinhDanhAdd').val(data.maDinhDanhXBAP || '');
            $('#maQuocTeAdd').val(data.maQuocTe || '');
            $('#nhaXuatBanAdd').val(data.nhaXuatBanID).trigger('change');
            $('#namXuatBanAdd').val(data.namXuatBan || '');
            $('#soLanTaiBanAdd').val(data.soLanTaiBan || '');
            $('#theLoaiAdd').val(data.theLoaiID).trigger('change');
            $('#soLuongInAdd').val(data.soLuongIn || '');
            $('#giaBiaAdd').val(data.giaBia || '');

            // Radio trạng thái
            $(`input[name='trangThaiAdd'][value='${data.trangThai == true ? "1" : "0"}'`).prop('checked', true);

            translations = []
            const res = await fetch(`/api/XuatBanAnPhamApi/BanDich/${data.xuatBanAnPhamID}`, {
                method: 'GET',
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
                translations = data.value
            } else {
                console.log(data.error)
            }

            var banDich_vi = translations.find(el => el.maNgonNgu === 'vi')
            $('#ngonNguAdd').val('vi').trigger('change');
            $('#tenAnPhamAdd').val(banDich_vi.tenAnPham);
            $('#tacGiaAdd').val(banDich_vi.tacGia);
            $('#moTaAdd').val(banDich_vi.moTa);

         
            $('#modalAdd').modal('show');
        });
        $('#dataGrid tbody').on('click', '.delete-command-btn', function () {
            var id = $(this).attr("ID").match(/\d+/)[0];
            var data = $('#dataGrid').DataTable().row(id).data();

            $('#idDelete').val(data.xuatBanAnPhamID);
            $('#nameDelete').text(`${data.tenAnPham}`);

            $('#modalDelete').modal('show');
        });

    }

   

    $('#openModalAdd').on("click", function () {
        $('#labelTenDanhMuc').html('Tên quốc tịch <span class="text-red">*</span>')
        $("#tenDanhMucAdd").attr("placeholder", "Nhập tên quốc tịch");
        $('#modalAddTitle').text("Thêm mới tên quốc tịch")
        $('#maSoHide').removeClass('d-none')

        //$("#modalAdd").find('select').val('vi').trigger('change');
        //$("#modalAdd").find('input[type=radio][value=1], input[type=checkbox][value=1]').prop('checked', true);

        $('#modalAdd').modal('show')
    });

    // hàm lưu tạm cho 1 ngôn ngữ
    function saveTempTranslation() {
        let maNgonNgu = $("#ngonNguAdd").val();
        let tenAnPham = $("#tenAnPhamAdd").val().trim();
        let tacGia = $("#tacGiaAdd").val().trim();
        let moTa = $("#moTaAdd").val().trim();

        if (!maNgonNgu || !tenAnPham) return; // chưa chọn ngôn ngữ hoặc không nhập tên thì bỏ qua

        // kiểm tra đã có ngôn ngữ này trong mảng chưa
        let exist = translations.find(t => t.maNgonNgu === maNgonNgu);
        if (exist) {
            exist.tenAnPham = tenAnPham?.trim();
            exist.tacGia = tacGia?.trim();
            exist.moTa = moTa?.trim();
        } else {
            translations.push({
                maNgonNgu: maNgonNgu,
                tenAnPham: tenAnPham.trim(),
                tacGia: tacGia.trim(),
                moTa: moTa.trim(),
            });
        }
    }

    // sự kiện blur trên input
    $("#tenDanhMucAdd, #tacGia, #moTaAdd").on("blur", function () {
        saveTempTranslation();
    });

    // sự kiện đổi ngôn ngữ → blind lại dữ liệu
    $("#ngonNguAdd").on("change", function () {
        // nếu đã có dữ liệu cho ngôn ngữ vừa chọn thì bind lại vào form
        let maNgonNgu = $(this).val();
        let exist = translations.find(t => t.maNgonNgu === maNgonNgu);
        if (exist) {
            $("#tenAnPhamAdd").val(exist.tenAnPham);
            $("#tacGiaAdd").val(exist.tacGia);
            $("#moTaAdd").val(exist.moTa);
        } else {
            // reset nếu chưa nhập gì cho ngôn ngữ đó
            $("#tenAnPhamAdd").val("");
            $("#tacGiaAdd").val("");
            $("#moTaAdd").val("");
        }
    });

    $("#formAdd").on("submit", function (e) {
        e.preventDefault();
        saveTempTranslation();

        dataAdd();
    });

    $("#formDelete").on("submit", function (e) {
        e.preventDefault();
        dataDelete();
    });

    async function dataAdd() {
        let idAnPham = $('#idAnPhamAdd').val();
        let anhBia = $('#anhBiaAdd')[0]?.files[0] || null;
        let maDinhDanh = $('#maDinhDanhAdd').val()?.trim() || null;
        let maQuocTe = $('#maQuocTeAdd').val()?.trim() || null;
        let nhaXuatBan = $('#nhaXuatBanAdd').val() || null;
        let namXuatBan = $('#namXuatBanAdd').val();
        let soLanTaiBan = $('#soLanTaiBanAdd').val();
        let theLoai = $('#theLoaiAdd').val() || null;
        let soLuongIn = $('#soLuongInAdd').val();
        let giaBia = $('#giaBiaAdd').val();
        let trangThai = $("input[name='trangThaiAdd']:checked").val();

        var tenTiengViet = translations.find(el => el.maNgonNgu === 'vi')?.tenAnPham || null
        if (translations?.length <= 0 || tenTiengViet == null || checkEmptyBlank(maDinhDanh)) {
            showNotification(0, 'Vui lòng nhập đầy đủ thông tin bắt buộc (*), bắt buộc có bản dịch tiếng Việt')
            return;
        }

        let thongTinChung = {
            "XuatBanAnPhamID": idAnPham || null,
            "MaDinhDanhXBAP": maDinhDanh,
            "MaQuocTe": maQuocTe,
            "NhaXuatBanID": nhaXuatBan,
            "NamXuatBan": namXuatBan !== "" ? Number(namXuatBan) : null,
            "SoLanTaiBan": soLanTaiBan !== "" ? Number(soLanTaiBan) : 0,
            "TheLoaiID": theLoai,
            "SoLuongIn": soLuongIn !== "" ? Number(soLuongIn) : 0,
            "GiaBia": giaBia !== "" ? Number(giaBia) : 0,
            "TrangThai": trangThai === "1",
            "tenAnPham": tenTiengViet,
        };

        let formData = new FormData()
        formData.append("XuatBanAnPham", JSON.stringify(thongTinChung));
        formData.append("XuatBanAnPham_NoiDung", JSON.stringify(translations));
        if (anhBia != null) { 
            formData.append("FileDinhKem", anhBia);
        }

        if (checkEmptyBlank(idAnPham)) {
            try {
                const res = await fetch('/api/XuatBanAnPhamApi/ThemMoi', {
                    method: 'POST',
                    body: formData
                })

                if (!res.ok) {
                    var errText = await res.text();
                    throw new Error(errText);
                }
                data = await res.json();

                if (data && data.isSuccess && data.value) {
                    showNotification(1, 'Thêm mới thành công')
                    translations = []

                    $('#dataGrid').DataTable().ajax.reload(null, false);
                    $('#modalAdd').modal('hide');
                } else {
                    showNotification(0, data.error)
                }
            }
            catch (err) {
                console.log(err.message)
            }
        } else {
            try {
                const res = await fetch(`/api/XuatBanAnPhamApi/ChinhSua/${idAnPham}`, {
                    method: 'PUT',
                    body: formData
                })

                if (!res.ok) {
                    var errText = await res.text();
                    throw new Error(errText);
                }

                data = await res.json();

                if (data && data.isSuccess && data.value) {
                    showNotification(1, 'Chỉnh sửa thành công')
                    $('#dataGrid').DataTable().ajax.reload(null, false);
                    $('#modalAdd').modal('hide');
                } else {
                    showNotification(0, data.error)
                }
            }
            catch (err) {
                console.log(err.message)
            }
        }
    }

    // reset modal
    $("#modalAdd").on('hidden.bs.modal', function () {
        translations = []
        $(this).find('input[type=text], textarea').val('');
        $(this).find('select').val('vi').trigger('change');
        $(this).find('input[type=radio][value=1], input[type=checkbox][value=1]').prop('checked', true);
        $('#showAnhBiaAdd').attr('src', '/assets/images/vector/addImage.png');
    });

    // Xử lý preview ảnh bìa khi chọn file mới
    $('#anhBiaAdd').on('change', function(e) {
        const file = e.target.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = function(ev) {
                $('#showAnhBiaAdd').attr('src', ev.target.result);
            };
            reader.readAsDataURL(file);
        } else {
            $('#showAnhBiaAdd').attr('src', '/assets/images/vector/addImage.png');
        }
    });

    async function dataDelete() {
        let id = $('#idDelete').val();

        try {
            const res = await fetch(`/api/XuatBanAnPhamApi/Xoa/${id}`, {
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
                //$("#ngon-ngu-search").empty();
                $("#ngonNguAdd").empty();
                //$("#ngon-ngu-search").append(`<option value="">Tất cả</option>`);
                data.value.forEach(lang => {
                    //$("#ngon-ngu-search").append(`<option value="${lang.maNgonNgu?.toLowerCase() }">${lang.tenNgonNgu}</option>`);
                    $("#ngonNguAdd").append(`<option value="${lang.maNgonNgu?.toLowerCase()}">${lang.tenNgonNgu}</option>`);
                });
                //$("#ngon-ngu-search").val('vi').trigger('change');
                $("#ngonNguAdd").val('vi').trigger('change');
            } else {
                showNotification(0, data.error)
            }
        }
        catch (err) {
            showNotification(0, err.message)
        }
    };

    async function initTheLoaiAnPham() {
        try {
            const res = await fetch('/api/DanhMucChungApi/DanhSach', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    loaiDanhMucID: 25,
                    tuKhoa: null
                })
            })

            if (!res.ok) {
                var errText = await res.text();
                throw new Error(errText);
            }
            data = await res.json();

            if (data && data.isSuccess && data.value) {
                $("#theLoaiAdd").empty();
                $("#the-loai-search").empty();

                $("#the-loai-search").append(`<option value="-1">Tất cả</option>`);
                data.value.forEach(el => {
                    $("#theLoaiAdd").append(`<option value="${el.danhMucID}">${el.tenDanhMuc}</option>`);
                    $("#the-loai-search").append(`<option value="${el.danhMucID}">${el.tenDanhMuc}</option>`);
                });
            } else {
                showNotification(0, data.error)
            }
        }
        catch (err) {
            console.log(err.message)
        }
    };

    async function initNhaXuatBan() {
        try {
            const res = await fetch('/api/ToChucApi/DanhSach', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    loaiToChucID: 98
                })
            })

            if (!res.ok) {
                var errText = await res.text();
                throw new Error(errText);
            }
            data = await res.json();

           
            if (data && data.isSuccess && data.value) {
                $("#nhaXuatBanAdd").empty();
                $("#nha-xuat-ban-search").empty();

                $("#nha-xuat-ban-search").append(`<option value="-1">Tất cả<option>`);
                data.value.forEach(el => {
                    $("#nhaXuatBanAdd").append(`<option value="${el.toChucID}">${el.tenToChuc}</option>`);
                    $("#nha-xuat-ban-search").append(`<option value="${el.toChucID}">${el.tenToChuc}</option>`);
                });
                //data.value.forEach(el => {
                //    $("#theLoaiAdd").append(`<option value="${el.danhMucID}">${el.tenDanhMuc}</option>`);
                //});
            } else {
                showNotification(0, data.error)
            }
        }
        catch (err) {
            console.log(err.message)
        }
    };

    
})