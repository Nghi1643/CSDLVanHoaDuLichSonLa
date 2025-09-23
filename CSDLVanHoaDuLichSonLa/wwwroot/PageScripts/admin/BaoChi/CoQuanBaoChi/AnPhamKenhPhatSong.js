const baseUrl = getRootLink();

$(document).ready(function () {
    initSelect2();
    let translations = [];

    console.log(coQuanBaoChiID);
    (async function () {
        await initThongTinToChuc(coQuanBaoChiID);
        await initNgonNgu();
        await initTanSuat();
        await initLinhVucChuyenSau();
        await initCoQuanBaoChi()
        initTable(coQuanBaoChiID);

    })();

    $('#tim-kiem').on('click', async function () {
        initTable(coQuanBaoChiID)
    });

    $('#tat-ca').on('click', async function () {
        $('#tu-khoa-search').val('');
        $('#tan-suat-search').val('-1').trigger('change');
        $('#linh-vuc-chuyen-sau-search').val('-1').trigger('change');
        $('#co-quan-bao-chi-search').val('-1').trigger('change');
        $('#trang-thai-search').val('-1').trigger('change');
        initTable(coQuanBaoChiID)
    });

    document.getElementById("tu-khoa-search").addEventListener("keypress", function (event) {
        if (event.key === "Enter") {
            event.preventDefault();
            initTable(coQuanBaoChiID)
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

    async function initTable(coQuanBaoChiID) {
        const tableApi = {
            url: `${baseUrl}/api/BaoChiAnPhamApi/DanhSach`,
            type: "POST",
            data: function (d) {
                var suDung = $('#trang-thai-search').val()
                var tanSuat = $('#tan-suat-search').val()
                var linhVuc = $('#linh-vuc-chuyen-sau-search').val()
                return JSON.stringify({
                    tuKhoa: $('#tu-khoa-search').val() || null,
                    maNgonNgu: $('#ngon-ngu-search').val() || 'vi',
                    tanSuatID: tanSuat == "-1" ? null : tanSuat,
                    toChucID: coQuanBaoChiID ? coQuanBaoChiID : null,
                    linhVucChuyenSauID: linhVuc == "-1" ? null : linhVuc,
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
                targets: 1,
                render: function (data, type, row, meta) {
                    return `<span class="detail-command-btn" id=n-"${meta.row}">${data}</span>`;
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
            { "data": "tenAnPham", "width": "", "class": "left-align name-text" },
            { "data": "tenCoQuanBaoChi", "width": "15%", "class": "left-align" },
            { "data": "tenTanSuat", "width": "10%", "class": "left-align" },
            { "data": "tenLinhVucChuyenSau", "width": "12%", "class": "left-align" },
            { "data": "soLuong", "width": "150px", "class": "center-align" },
            { "data": "trangThai", "width": "120px", "class": "left-align" },
            { "data": "", "width": "120px", "class": "center-align group-icon-action" },
        ];

        if (!permitedEdit && !permitedDelete) {
            tableCols.pop();
            tableDefs.pop();
        }

        initDataTableConfigNoSearch('dataGrid', tableApi, tableDefs, tableCols);

        $('#dataGrid tbody').on('click', '.edit-command-btn', async function () {
            var id = $(this).attr("ID").match(/\d+/)[0];
            var data = $('#dataGrid').DataTable().row(id).data();
            $('#idAnPhamAdd').val(data.baoChiAnPhamID);

            // Các input text
            $('#tanSuatAdd').val(data.tanSuatID).trigger('change');
            $('#linhVucChuyenSauAdd').val(data.linhVucChuyenSauID).trigger('change');
            // Radio trạng thái
            $(`input[name='trangThaiAdd'][value='${data.trangThai == true ? "1" : "0"}'`).prop('checked', true);

            translations = []
            const res = await fetch(`/api/BaoChiAnPhamApi/BanDich/${data.baoChiAnPhamID}`, {
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
            $('#doiTuongDocGiaAdd').val(banDich_vi.doiTuongDocGia);


            $('#modalAdd').modal('show');
        });
        $('#dataGrid tbody').on('click', '.delete-command-btn', function () {
            var id = $(this).attr("ID").match(/\d+/)[0];
            var data = $('#dataGrid').DataTable().row(id).data();

            $('#idDelete').val(data.baoChiAnPhamID);
            $('#nameDelete').text(`${data.tenAnPham}`);

            $('#modalDelete').modal('show');
        });

        $('#dataGrid tbody').on('click', '.detail-command-btn', function () {
            var id = $(this).attr("ID").match(/\d+/)[0];
            var data = $('#dataGrid').DataTable().row(id).data();

            $('#modalDetailTitle').text(data.tenAnPham || "Chi tiết ấn phẩm/kênh phát sóng")

            $('#linhVucChuyenSauDetail').text(data.tenLinhVucChuyenSau || "");
            $('#tanSuatDetail').text(data.tenTanSuat || "");
            //$('#thuTuDetail').text(data.thuTu);
            $('#trangThaiDetail').html(data.trangThai == true ? `<div class="TrangThai green-text">Đã phát hành</div>` : `<div class="TrangThai yellow-text">Chưa phát hành</div>`);

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

    // hàm lưu tạm cho 1 ngôn ngữ
    function saveTempTranslation() {
        let maNgonNgu = $("#ngonNguAdd").val();
        let tenAnPham = $("#tenAnPhamAdd").val().trim();
        let doiTuongDocGia = $("#doiTuongDocGiaAdd").val().trim();

        if (!maNgonNgu || !tenAnPham) return; // chưa chọn ngôn ngữ hoặc không nhập tên thì bỏ qua

        // kiểm tra đã có ngôn ngữ này trong mảng chưa
        let exist = translations.find(t => t.maNgonNgu === maNgonNgu);
        if (exist) {
            exist.tenAnPham = tenAnPham?.trim();
            exist.doiTuongDocGia = doiTuongDocGia?.trim();
        } else {
            translations.push({
                maNgonNgu: maNgonNgu,
                tenAnPham: tenAnPham.trim(),
                doiTuongDocGia: doiTuongDocGia.trim(),
            });
        }
        console.log(translations)
    }

    // sự kiện blur trên input
    $("#tenAnPhamAdd, #doiTuongDocGiaAdd").on("blur", function () {
        saveTempTranslation();
    });

    // sự kiện đổi ngôn ngữ → blind lại dữ liệu
    $("#ngonNguAdd").on("change", function () {
        // nếu đã có dữ liệu cho ngôn ngữ vừa chọn thì bind lại vào form
        let maNgonNgu = $(this).val();
        let exist = translations.find(t => t.maNgonNgu === maNgonNgu);
        if (exist) {
            $("#tenAnPhamAdd").val(exist.tenAnPham);
            $("#doiTuongDocGiaAdd").val(exist.doiTuongDocGia);
        } else {
            // reset nếu chưa nhập gì cho ngôn ngữ đó
            $("#tenAnPhamAdd").val("");
            $("#doiTuongDocGiaAdd").val("");
            $("#moTaAdd").val("");
        }
        console.log(translations)
    });

    $("#formAdd").on("submit", function (e) {
        e.preventDefault();

        const submitBtn = $(this).find('button[type="submit"]');
        if (submitBtn.prop('disabled')) {
            return;
        }
        
        submitBtn.prop('disabled', true);
        const originalText = submitBtn.find('#submitText').text();
        submitBtn.find('#submitText').text('Đang xử lý...')

        saveTempTranslation();
        dataAdd(coQuanBaoChiID);
    });

    $("#formDelete").on("submit", function (e) {
        e.preventDefault();
        dataDelete();
    });

    async function dataAdd(coQuanBaoChiID) {
        let idAnPham = $('#idAnPhamAdd').val();
        let linhVucChuyenSau = $('#linhVucChuyenSauAdd').val() || null;
        let tanSuat = $('#tanSuatAdd').val();
        let trangThai = $("input[name='trangThaiAdd']:checked").val();

        var tenTiengViet = translations.find(el => el.maNgonNgu === 'vi')?.tenAnPham || null
        if (translations?.length <= 0 || tenTiengViet == null || checkEmptyBlank(linhVucChuyenSau) || checkEmptyBlank(tanSuat)) {
            showNotification(0, 'Vui lòng nhập đầy đủ thông tin bắt buộc (*), bắt buộc có bản dịch tiếng Việt')
            return;
        }

        let thongTinChung = {
            "BaoChiAnPhamID": idAnPham || "00000000-0000-0000-0000-000000000000",
            "LinhVucChuyenSauID": linhVucChuyenSau,
            "TanSuatID": tanSuat,
            "ToChucID": coQuanBaoChiID,
            "trangThai": trangThai == "1" ? true : false,
        };

        let formData = new FormData()
        formData.append("BaoChiAnPham", JSON.stringify(thongTinChung));
        formData.append("BaoChiAnPham_NoiDung", JSON.stringify(translations));
      
        if (checkEmptyBlank(idAnPham)) {
            try {
                const res = await fetch('/api/BaoChiAnPhamApi/ThemMoi', {
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
                const res = await fetch(`/api/BaoChiAnPhamApi/ChinhSua/${idAnPham}`, {
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
    });

    async function dataDelete() {
        let id = $('#idDelete').val();

        try {
            const res = await fetch(`/api/BaoChiAnPhamApi/Xoa/${id}`, {
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
                $("#ngon-ngu-search").empty();
                $("#ngonNguAdd").empty();
                //$("#ngon-ngu-search").append(`<option value="">Tất cả</option>`);
                data.value.forEach(lang => {
                    $("#ngon-ngu-search").append(`<option value="${lang.maNgonNgu?.toLowerCase() }">${lang.tenNgonNgu}</option>`);
                    $("#ngonNguAdd").append(`<option value="${lang.maNgonNgu?.toLowerCase()}">${lang.tenNgonNgu}</option>`);
                });
                $("#ngon-ngu-search").val('vi').trigger('change');
                $("#ngonNguAdd").val('vi').trigger('change');
            } else {
                showNotification(0, data.error)
            }
        }
        catch (err) {
            showNotification(0, err.message)
        }
    };

    async function initLinhVucChuyenSau() {
        try {
            const res = await fetch('/api/DanhMucChungApi/DanhSach', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    loaiDanhMucID: 9,
                    tuKhoa: null
                })
            })

            if (!res.ok) {
                var errText = await res.text();
                throw new Error(errText);
            }
            data = await res.json();

            if (data && data.isSuccess && data.value) {
                $("#linh-vuc-chuyen-sau-search").empty();

                $("#linh-vuc-chuyen-sau-search").append(`<option value="-1">Tất cả</option>`);
                data.value.forEach(el => {
                    $("#linhVucChuyenSauAdd").append(`<option value="${el.danhMucID}">${el.tenDanhMuc}</option>`);
                    $("#linh-vuc-chuyen-sau-search").append(`<option value="${el.danhMucID}">${el.tenDanhMuc}</option>`);
                });
            } else {
                showNotification(0, data.error)
            }
        }
        catch (err) {
            console.log(err.message)
        }
    };

    async function initTanSuat() {
        try {
            const res = await fetch('/api/DanhMucChungApi/DanhSach', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    loaiDanhMucID: 6,
                    tuKhoa: null
                })
            })

            if (!res.ok) {
                var errText = await res.text();
                throw new Error(errText);
            }
            data = await res.json();

            if (data && data.isSuccess && data.value) {
                $("#tan-suat-search").append(`<option value="-1">Tất cả</option>`);
                data.value.forEach(el => {
                    $("#tanSuatAdd").append(`<option value="${el.danhMucID}">${el.tenDanhMuc}</option>`);
                    $("#tan-suat-search").append(`<option value="${el.danhMucID}">${el.tenDanhMuc}</option>`);
                });
            } else {
                showNotification(0, data.error)
            }
        }
        catch (err) {
            console.log(err.message)
        }
    };

    async function initCoQuanBaoChi() {
        try {
            const res = await fetch('/api/ToChucApi/DanhSach', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    loaiToChucID: 97
                })
            })

            if (!res.ok) {
                var errText = await res.text();
                throw new Error(errText);
            }
            data = await res.json();

           
            if (data && data.isSuccess && data.value) {
                $("#co-quan-bao-chi-search").empty();
                $("#co-quan-bao-chi-search").append(`<option value="-1">Tất cả<option>`);
                data.value.forEach(el => {
                    $("#co-quan-bao-chi-search").append(`<option value="${el.toChucID}">${el.tenToChuc}</option>`);
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