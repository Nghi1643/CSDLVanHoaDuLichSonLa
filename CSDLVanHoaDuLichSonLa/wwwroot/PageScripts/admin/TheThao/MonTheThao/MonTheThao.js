const baseUrl = getRootLink();

$(document).ready(function () {
    initSelect2();
    initDatePicker();
    let translations = [];

    (async function () {
        await Promise.all([
            initDanhMucChung(33, "#cach-thi-dau-search, #cachThiDauIDAdd", "Cách thi đấu"),
            initDanhMucChung(36, "#trang-thai-search", "Trạng thái"),
            initNgonNgu(),
        ]);
        initTable();
    })();

    $('#tim-kiem').on('click', async function () {
        initTable();
    });

    $('#tat-ca').on('click', async function () {
        $('#tu-khoa-search').val("");
        $('#cach-thi-dau-search').val("-1").trigger('change');
        $('#ngon-ngu-search').val("-1").trigger('change');
        $('#trang-thai-search').val("-1").trigger('change');
        initTable();
    });

    async function initTable() {
        const tableApi = {
            url: `${baseUrl}/api/TT_MonTheThaoApi/Gets?MaNgonNgu=vi`,
            type: "GET",
            contentType: 'application/json; charset=utf-8',
            dataSrc: function (data) {
                if (data && data.isSuccess && data.value && data.value.length > 0) {
                    console.log(data.value);
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
                targets: 2, // Cột trạng thái
                render: function (data, type, row, meta) {
                    return `<div class="details-form" id="n-${meta.row}">${data}</div>`;
                }
            },
            {
                targets: 5, // Cột trạng thái
                render: function (data, type, row, meta) {
                    if (row.trangThaiID == 1) {
                        return `<span class="TrangThai green-text">Hoạt động</span>`;
                    } else {
                        return `<span class="TrangThai red-text">Không hoạt động</span>`;
                    }
                }
            },
            {
                targets: 6, // Cột chức năng
                render: function (data, type, row, meta) {
                    let html = "";
                    if (typeof permitedEdit !== 'undefined' && permitedEdit) {
                        html += `<a data-toggle="tooltip" title="Chỉnh sửa" class="text-yellow edit-form" id="n-${meta.row}">
                                    <i class="hgi-icon hgi-edit"></i>
                                </a>`;
                    }
                    if (typeof permitedDelete !== 'undefined' && permitedDelete) {
                        html += `<a data-toggle="tooltip" title="Xóa" class="delete-command-btn text-red cursor-pointer" id="delete-${meta.row}">
                                    <i class="hgi-icon hgi-delete"></i>
                                </a>`;
                    }
                    if ((!permitedEdit && !permitedDelete) || (typeof permitedEdit === 'undefined' && typeof permitedDelete === 'undefined')) {
                        html = `<span class="text-muted">Chỉ xem</span>`;
                    }
                    return html;
                }
            }
        ];

        const tableCols = [
            { "data": "stt", "width": "40px", "class": "center-align" },
            { "data": "maDinhDanh", "class": "left-align" },
            { "data": "tenMon", "class": "left-align name-text" },
            { "data": "cachThiDau", "class": "left-align" },
            { "data": "tepKemTheo", "class": "left-align" },
            { "data": "trangThaiID", "class": "left-align" },
            { "data": "", "class": "center-align group-icon-action" }
        ];

        initDataTableConfigNoSearch('dataGrid', tableApi, tableDefs, tableCols);
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
                $("#maNgonNguAdd").empty();
                //$("#ngon-ngu-search").append(`<option value="">Tất cả</option>`);
                data.value.forEach(lang => {
                    $("#ngon-ngu-search").append(`<option value="${lang.maNgonNgu?.toLowerCase() }">${lang.tenNgonNgu}</option>`);
                    $("#maNgonNguAdd").append(`<option value="${lang.maNgonNgu?.toLowerCase()}">${lang.tenNgonNgu}</option>`);
                });
                $("#ngon-ngu-search").val('vi').trigger('change');
                $("#maNgonNguAdd").val('vi').trigger('change');
            } else {
                showNotification(0, data.error)
            }
        }
        catch (err) {
            showNotification(0, err.message)
        }
    };

     // hàm lưu tạm cho 1 ngôn ngữ
    function saveTempTranslation() {
        let maNgonNgu = $('#maNgonNguAdd').val();
        let tenMon = $('#tenMonAdd').val();
        let luatChoi = $('#luatChoiAdd').val();
        let moTa = $('#moTaAdd').val();

        if (!maNgonNgu || !tenMon) return; // chưa chọn ngôn ngữ hoặc không nhập tên thì bỏ qua

        // kiểm tra đã có ngôn ngữ này trong mảng chưa
        let exist = translations.find(t => t.maNgonNgu === maNgonNgu);
        if (exist) {
            exist.tenMon = tenMon?.trim();
            exist.luatChoi = luatChoi?.trim();
            exist.moTa = moTa?.trim();
        } else {
            translations.push({
                maNgonNgu: maNgonNgu,
                tenMon: tenMon.trim(),
                luatChoi: luatChoi.trim(),
                moTa: moTa.trim(),
            });
        }
        console.log(translations)
    }

    $(document).on('click', '.edit-form', function () {
        var id = $(this).attr("ID").match(/\d+/)[0];
        var data = $('#dataGrid').DataTable().row(id).data();
        translations = []
        if (data) {
            translations = data
        } else {
            console.log(data.error)
        }
        console.log(translations)
        $('#monTheThaoIDAdd').val(data.monTheThaoID || "");
        $('#maDinhDanhAdd').val(data.maDinhDanh || "");
        $('#cachThiDauIDAdd').val(data.cachThiDauID || "").trigger('change');
        $('#tenAdd').val(data.ten || "");
        $('#tenMonAdd').val(data.tenMon || "");

        // Trạng thái: giả sử 1 là hoạt động, 139 là không hoạt động
        if (data.trangThai === "Hoạt động" || data.trangThaiID == 1) {
            $('input[name="trangThaiAdd"][value="1"]').prop('checked', true);
        } else {
            $('input[name="trangThaiAdd"][value="139"]').prop('checked', true);
        }
        // File đính kèm: chỉ hiển thị tên file nếu cần
        if (data.tepKemTheo) {
            $('#tepKemTheoDetail').val(data.tepKemTheo);
        }

        $('#maNgonNguAdd').val('vi').trigger('change');
        $('#tenMonAdd').val(translations.tenMon);
        $('#luatChoiAdd').val(translations.luatChoi);
        $('#moTaAdd').val(translations.moTa);

        $('#modalAdd').modal('show');
    });

    $(document).on('click', '.details-form', function () {
        var id = $(this).attr("ID").match(/\d+/)[0];
        var data = $('#dataGrid').DataTable().row(id).data();

        console.log(data, "Chi tiết")

        $("#modalDetailTitle").text(`Chi tiết môn ${data.ten || ""}`);

        $('#monTheThaoIDDetail').text(data.monTheThaoID || "");
        $('#maDinhDanhDetail').text(data.maDinhDanh || "");
        $('#cachThiDauIDDetail').text(data.cachThiDauID || "").trigger('change');
        $('#tenDetail').text(data.ten || "");
        $('#tenMonDetail').text(data.tenMon || "");

        $('#tenMonDetail').text(data.tenMon);
        $('#luatChoiDetail').text(data.luatChoi);
        $('#moTaDetail').text(data.moTa);

        // Trạng thái: giả sử 1 là hoạt động, 139 là không hoạt động
        if (data.trangThai === "Hoạt động" || data.trangThaiID == 1) {
            $('input[name="trangThaiDetail"][value="1"]').prop('checked', true);
        } else {
            $('input[name="trangThaiDetail"][value="139"]').prop('checked', true);
        }
        // File đính kèm: chỉ hiển thị tên file nếu cần
        if (data.tepKemTheo) {
            $('#tepKemTheoDetail').val(data.tepKemTheo);
        }

        $('#maNgonNguDetail').val('vi').trigger('change');
        

        $('#modalDetail').modal('show');
    });

    // Xử lý submit thêm/sửa môn thể thao
    $("#formAdd").on("submit", async function (e) {
        e.preventDefault();
        const submitBtn = $(this).find('button[type="submit"]');
        if (submitBtn.prop('disabled')) return;
        submitBtn.prop('disabled', true);
        const originalText = submitBtn.text();
        submitBtn.text('Đang xử lý...');

        // Lấy dữ liệu từ form
        const monTheThaoID = $('#monTheThaoIDAdd').val();
        const maDinhDanh = $('#maDinhDanhAdd').val();
        const ten = $('#tenAdd').val();
        const cachThiDauID = $('#cachThiDauIDAdd').val();
        //const trangThaiID = $('input[name="trangThaiAdd"]:checked').val();
        const trangThaiID = 139;
        const tepKemTheoDetail = $('#tepKemTheoDetail').val();
        const tepKemTheoInput = document.getElementById('tepKemTheoAdd');
        const tepKemTheo = tepKemTheoInput && tepKemTheoInput.files.length > 0 ? tepKemTheoInput.files[0] : null;
        console.log(tepKemTheo);
        // Đa ngữ
        const maNgonNgu = $('#maNgonNguAdd').val();
        const tenMon = $('#tenMonAdd').val();
        const luatChoi = $('#luatChoiAdd').val();
        const moTa = $('#moTaAdd').val();

        // Build object theo mẫu
        const dataObj = {
            MaDinhDanh: maDinhDanh,
            CachThiDauID: cachThiDauID,
            TrangThaiID: trangThaiID,
            ten: ten,
            MaNgonNgu: maNgonNgu,
            MonTheThaoID: monTheThaoID || "00000000-0000-0000-0000-000000000000",
            tenMon: tenMon,
            LuatChoi: luatChoi,
            MoTa: moTa,
            tepKemTheoDetail: tepKemTheoDetail || null,
        };

        // Validate bắt buộc
        if (!maDinhDanh || !ten || !cachThiDauID || !maNgonNgu || !tenMon) {
            showNotification(0, 'Vui lòng nhập đầy đủ thông tin bắt buộc!');
            submitBtn.prop('disabled', false).text(originalText);
            return;
        }

        // Tạo form data
        let formData = new FormData();
        if (tepKemTheo) {
            formData.append('File', tepKemTheo);
        }
        formData.append('Data', JSON.stringify(dataObj));

        if (monTheThaoID && monTheThaoID !== "00000000-0000-0000-0000-000000000000") {
            try {
                $.ajax({
                    type: "PUT",
                    async: false,
                    url: `${baseUrl}/api/TT_MonTheThaoApi/Update`,
                    contentType: false,
                    processData: false,
                    data: formData,
                    success: function(data) {
                        console.log(data);
                        if (data && data.isSuccess) {
                            showNotification(1, monTheThaoID ? 'Cập nhật thành công!' : 'Thêm mới thành công!');
                            $('#modalAdd').modal('hide');
                            initTable();
                        } else {
                            showNotification(0, data.error || 'Lưu dữ liệu thất bại!');
                        }
                    },
                    error: function(err) {
                        console.log(err);
                    }
                })
            } catch (err) {
                showNotification(0, err.message || 'Lưu dữ liệu thất bại!');
            } finally {
                submitBtn.prop('disabled', false).text(originalText);
            }
        } else {
            try {
                $.ajax({
                    type: "POST",
                    async: false,
                    url: `${baseUrl}/api/TT_MonTheThaoApi/Add`,
                    contentType: false,
                    processData: false,
                    data: formData,
                    success: function(data) {
                        console.log(data);
                        if (data && data.isSuccess) {
                            showNotification(1, monTheThaoID ? 'Cập nhật thành công!' : 'Thêm mới thành công!');
                            $('#modalAdd').modal('hide');
                            initTable();
                        } else {
                            showNotification(0, data.error || 'Lưu dữ liệu thất bại!');
                        }
                    },
                    error: function(err) {
                        console.log(err);
                    }
                })
            } catch (err) {
                showNotification(0, err.message || 'Lưu dữ liệu thất bại!');
            } finally {
                submitBtn.prop('disabled', false).text(originalText);
            }
        }

    });

    $("#tenMonAdd, #luatChoiAdd, #moTaAdd").on("blur", function () {
        saveTempTranslation();
    });

    // Xử lý xóa môn thể thao
    $("#formDelete").on("submit", function (e) {
        e.preventDefault();
        dataDelete();
    });

    async function dataDelete() {
        const monTheThaoID = $('#monTheThaoIDDelete').val();
        if (!monTheThaoID) {
            showNotification(0, 'Không xác định được môn thể thao cần xóa!');
            return;
        }
        try {
            const res = await fetch(`${baseUrl}/api/TT_MonTheThaoApi/Delete?MonTheThaoID=${encodeURIComponent(monTheThaoID)}`, {
                method: 'DELETE'
            });
            if (!res.ok) {
                const errText = await res.text();
                showNotification(0, errText || 'Xóa thất bại!');
                return;
            }
            const result = await res.json();
            if (result && result.isSuccess) {
                showNotification(1, 'Xóa thành công!');
                $('#modalDelete').modal('hide');
                initTable();
            } else {
                showNotification(0, result.error || 'Xóa thất bại!');
            }
        } catch (err) {
            showNotification(0, err.message || 'Xóa thất bại!');
        }
    }

    // reset modal
    $("#modalAdd").on('hidden.bs.modal', function () {
        translations = []
        $(this).find('input[type=text], textarea').val('');
        $(this).find('select').val('vi').trigger('change');
        $(this).find('input[type=radio][value=1], input[type=checkbox][value=1]').prop('checked', true);
    });

    $("#maNgonNguAdd").on("change", function () {
        // nếu đã có dữ liệu cho ngôn ngữ vừa chọn thì bind lại vào form
        let maNgonNgu = $(this).val();
        console.log(maNgonNgu)
        let exist = translations;
        if (exist) {
            $("#tenMonAdd").val(exist.tenMon);
            $("#luatChoiAdd").val(exist.luatChoi);
            $("#moTaAdd").val(exist.moTa);
        } else {
            // reset nếu chưa nhập gì cho ngôn ngữ đó
            $("#tenMonAdd").val("");
            $("#luatChoiAdd").val("");
            $("#moTaAdd").val("");
        }
        console.log(translations)
    });
});