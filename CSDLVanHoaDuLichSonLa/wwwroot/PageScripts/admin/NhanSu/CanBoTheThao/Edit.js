const baseUrl = getRootLink();
const ckeditormoTaDichEdit = CKEDITOR.replace('moTaDichEdit');
const ckeditorthanhTichDichEdit = CKEDITOR.replace('thanhTichDichEdit');
const ckeditorghiChuDichEdit = CKEDITOR.replace('ghiChuDichEdit');

let tempTheThao = [];
let listTheThao = [];
let allSports = []; // chứa tất cả môn từ API

console.log(NhanSuId)

$(document).ready(function () {
    //#region Xử lý các function chung
    initDatePicker();
    initDanhMucChung_NoAll(1, "#danTocEdit", "")
    initDanhMucChung_NoAll(5, "#gioiTinhEdit", "")
    initNgonNgu("#ngonNguDichEdit")
    initNgonNgu("#ngonNguDichDiaDiemEdit")
    initNgonNgu("#ngonNguDPT")
    initTinhThanhPho()
    initSelect2();
    previewPicture();
    initTableTheThao(NhanSuId);
    listTheThao = initListTheThao();
    console.log('Danh sách môn thể thao từ API:', listTheThao);

    $('#tinhThanhPhoEdit').on('change', function () {
        let tinhID = $(this).val();
        initPhuongXa(tinhID)
    })
    //#endregion Xử lý các function chung

    //#region Xử lý lưu thông tin cán bộ thể thao
    setTimeout(() => {
        loadData(NhanSuId)
    }, 300);

    $("#formNhanSuEdit").on("submit", function (e) {
        e.preventDefault(); // Ngăn chặn hành vi mặc định của form
        dataEdit();
    });

    // Khi thay đổi ngôn ngữ dịch, reset các input trong phần đa ngữ
    $(document).on('change', '#ngonNguDichEdit', function () {
        let maNgonNgu = $(this).val();
        
        if (NhanSuId && maNgonNgu) {
            console.log('Chuyển ngôn ngữ dịch cho lễ hội hiện tại:', maNgonNgu);
            // Load bản dịch cho ngôn ngữ được chọn
            loadTranslation(NhanSuId, maNgonNgu);
        } else {
            // Nếu chưa có cơ quan (tạo mới) hoặc chưa chọn ngôn ngữ, clear các trường dịch
            if (!NhanSuId || !maNgonNgu) {
                $("#ngonNguDichEdit").val('');
                $("#hoVaTenEdit").val('');
                $("#diaChiEdit").val('');
                $("#noiLamViecEdit").val('');
                $("#theChatDichEdit").val('');
                if (typeof ckeditormoTaDichEdit !== 'undefined') ckeditormoTaDichEdit.setData('');
                if (typeof ckeditorthanhTichDichEdit !== 'undefined') ckeditorthanhTichDichEdit.setData('');
                if (typeof ckeditorghiChuDichEdit !== 'undefined') ckeditorghiChuDichEdit.setData('');
            }
        }
    });
    //#endregion Xử lý lưu thông tin cán bộ thể thao
})

//#region Function Xử lý chung
function initTinhThanhPho() {
    getDataWithApi('GET', '/api/CapTinhApi/Gets').then(data => {
        if (data && data.isSuccessed && data.resultObj) {
            $("#tinhThanhPhoEdit").empty().append(`<option value="">Chọn</option>`);
            data.resultObj.forEach(item => {
                const displayText = item.noiDung || item.tenTinh || 'Không rõ tên';
                $("#tinhThanhPhoEdit").append(`<option value="${item.tinhID}">${displayText}</option>`);
            });
        }
    })
}

function initPhuongXa(tinhID = null) {
    getDataWithApi('GET', `/api/CapXaApi/GetByMaTinh?MaNgonNgu=vi&MaTinh=${tinhID ? `${tinhID}` : ''}`).then(data => {
        $("#phuongXaEdit").empty().append(`<option value="">Chọn</option>`);
        if (data && data.isSuccess && data.value) {
            data.value.forEach(item => {
                const displayText = item.noiDung || item.tenXa || 'Không rõ tên';
                $("#phuongXaEdit").append(`<option value="${item.xaID}">${displayText}</option>`);
            });
        }
    })
}

function previewPicture() {
    // Xử lý preview ảnh bìa
    $('.group_image_input #fileAnhDaiDienPrev').on('change', function(e) {
        const file = e.target.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = function(ev) {
                $('#showAnhDaiDienEdit').attr('src', ev.target.result);
            };
            reader.readAsDataURL(file);
        } else {
            $('#showAnhDaiDienEdit').attr('src', '/assets/images/vector/AddImage.png');
        }
    });
}
//#endregion Function Xử lý chung

//#region Function Thông tin cán bộ thể thao
function loadData(id) {
    getDataWithApi('GET', `/api/DM_CaNhan_CanBoTheThaoApi/Get?CaNhanID=${id}&maNgonNgu=vi`).then(data => {
        if (data && data.isSuccess && data.value) {
            const item = data.value;
            console.log('Dữ liệu cán bộ thể thao:', item);
            $("#maDinhDanhEdit").val(item.maDinhDanh || '');
            $("#showAnhDaiDienEdit").attr('src', item.anhChanDung || '/assets/images/vector/no-image.png');
            $("#showAnhDaiDienDetail").val(item.anhChanDung || '');
            $("#gioiTinhEdit").val(item.gioiTinhID || '').trigger('change');
            $("#ngaySinhEdit").val(formatDateWithoutTime(item.ngaySinh) || '');
            $("#danTocEdit").val(item.danTocID || '').trigger('change');
            $("#soDienThoaiEdit").val(item.dienThoai || '');
            $("#hopThuEdit").val(item.hopThu || '');
            $("#tinhThanhPhoEdit").val(item.tinhID || '').trigger('change');
            setTimeout(() => {
                $("#phuongXaEdit").val(item.xaID || '').trigger('change');
            }, 300);
            //$("#soTheEdit").val(item.soThe || '');
            //$("#ngayCapEdit").val(formatDateWithoutTime(item.ngayCap) || '');
            //$("#coQuanCapEdit").val(item.coQuanCap || '');
            $("#thuTuEdit").val(item.thuTu || '');
            if (item.trangThai) {
                $("input[name='trangThai'][value='1']").prop('checked', true);
            } else {
                $("input[name='trangThai'][value='0']").prop('checked', true);
            }
            $("#ngonNguDichEdit").val(item.maNgonNgu || '').trigger('change');
            $("#hoVaTenEdit").val(item.hoTen || '');
            $("#diaChiEdit").val(item.diaChi || '');
            $("#noiLamViecEdit").val(item.noiLamViec || '');
            $("#theChatDichEdit").val(item.theChat || '');
            if (typeof ckeditormoTaDichEdit !== 'undefined') ckeditormoTaDichEdit.setData(item.moTa || '');
            if (typeof ckeditorthanhTichDichEdit !== 'undefined') ckeditorthanhTichDichEdit.setData(item.thanhTich || '');
            if (typeof ckeditorghiChuDichEdit !== 'undefined') ckeditorghiChuDichEdit.setData(item.ghiChu || '');
        }
    })
}

function dataEdit() {
    const fileAnhDaiDien = $("#fileAnhDaiDienPrev")[0].files[0] || null;
    const maDinhDanh = $("#maDinhDanhEdit").val();
    const gioiTinh = $("#gioiTinhEdit").val();
    const ngaySinh = $("#ngaySinhEdit").val();
    const danToc = $("#danTocEdit").val();
    const soDienThoai = $("#soDienThoaiEdit").val();
    const hopThu = $("#hopThuEdit").val();
    const tinhThanhPho = $("#tinhThanhPhoEdit").val();
    const phuongXa = $("#phuongXaEdit").val();
    const anhChanDungDetail = $("#showAnhDaiDienDetail").val() || '';
    //const soThe = $("#soTheEdit").val();
    //const ngayCap = $("#ngayCapEdit").val();
    //const coQuanCap = $("#coQuanCapEdit").val();
    //const thuTu = $("#thuTuEdit").val();
    const trangThai = $("input[name='trangThai']:checked").val() == 1 ? true : false;

    // Đa ngữ
    const ngonNguDich = $("#ngonNguDichEdit").val();
    const hoVaTen = $("#hoVaTenEdit").val();
    const diaChi = $("#diaChiEdit").val();
    const noiLamViec = $("#noiLamViecEdit").val();
    const theChat = $("#theChatDichEdit").val();
    const moTa = ckeditormoTaDichEdit.getData();
    const thanhTich = ckeditorthanhTichDichEdit.getData();
    const ghiChu = ckeditorghiChuDichEdit.getData();

    // Kết quả trả về dạng object
    const dtChung = {
        MaDinhDanh: maDinhDanh,
        gioiTinh: gioiTinh,
        NgaySinh: ngaySinh,
        DanTocID: danToc,
        DienThoai: soDienThoai,
        HopThu: hopThu,
        TinhID: tinhThanhPho,
        XaID: phuongXa,
        anhChanDungDetail: anhChanDungDetail,
        CaNhanID: NhanSuId,
        //ngayCap,
        //coQuanCap,
        //thuTu,
        TrangThai: trangThai,
        MaNgonNgu: ngonNguDich,
        HoTen: hoVaTen,
        DiaChi: diaChi,
        NoiLamViec: noiLamViec,
        TheChat: theChat,
        MoTa: moTa,
        ThanhTich: thanhTich,
        GhiChu: ghiChu
    };

    console.log(dtChung)

    let formData = new FormData()
    formData.append('RequestCanBoTheThao', JSON.stringify(dtChung))
    formData.append("File", fileAnhDaiDien)

    $.ajax({
        type: 'PUT',
        url: `${baseUrl}/api/DM_CaNhan_CanBoTheThaoApi/Update`,
        async: false,
        contentType: false,
        processData: false,
        data: formData,
        success: function (data) {
            if (data.isSuccess && data.value) {
                showNotification(1, "Chỉnh sửa cán bộ thể thao thành công");
                console.log(data)
                setTimeout(() => {
                    window.location.href = `${baseUrl}/AdminTool/NhanSu/CanBoTheThao`;
                }, 1500);
            } else {
                showNotification(0, "Chỉnh sửa cán bộ thể thao không thành công");
                console.log(data.error)
            }
        },
        error: function (err) {
            console.log(err)
        }
    })

    console.log('Dữ liệu cán bộ thể thao:', dtChung);
}

function loadTranslation(nhanSuId, maNgonNgu) {
    getDataWithApi('GET', `/api/DM_CaNhan_CanBoTheThaoApi/Get?CaNhanID=${nhanSuId}&maNgonNgu=${maNgonNgu}`).then(data => {
        console.log(data, "Load bản dịch lễ hội")
        if (data && data.isSuccess && data.value) {
            translations = data.value;
            // Populate các trường dịch với dữ liệu đa ngữ
            $("#ngonNguDichEdit").val(translations.maNgonNgu || '');
            $("#hoVaTenEdit").val(translations.hoTen || '');
            $("#diaChiEdit").val(translations.diaChi || '');
            $("#noiLamViecEdit").val(translations.noiLamViec || '');
            $("#theChatDichEdit").val(translations.theChat || '');
            if (typeof ckeditormoTaDichEdit !== 'undefined') ckeditormoTaDichEdit.setData(translations.moTa || '');
            if (typeof ckeditorthanhTichDichEdit !== 'undefined') ckeditorthanhTichDichEdit.setData(translations.thanhTich || '');
            if (typeof ckeditorghiChuDichEdit !== 'undefined') ckeditorghiChuDichEdit.setData(translations.ghiChu || '');
        } else {
            // Nếu không có bản dịch, clear các trường dịch
            $("#ngonNguDichEdit").val('');
            $("#hoVaTenEdit").val('');
            $("#diaChiEdit").val('');
            $("#noiLamViecEdit").val('');
            $("#theChatDichEdit").val('');
            if (typeof ckeditormoTaDichEdit !== 'undefined') ckeditormoTaDichEdit.setData('');
            if (typeof ckeditorthanhTichDichEdit !== 'undefined') ckeditorthanhTichDichEdit.setData('');
            if (typeof ckeditorghiChuDichEdit !== 'undefined') ckeditorghiChuDichEdit.setData('');
        }
    })
}
//#endregion Function Thông tin cán bộ thể thao


//#region Function Xử lý bảng thể chất
function initTableTheThao(id) {
    const tableApi = {
        url: `${baseUrl}/api/DM_CaNhan_CanBoTheThaoApi/GetListMon?CaNhanID=${id}&maNgonNgu=vi`,
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
            targets: 0,
            render: function (data, type, row, meta) {
                return meta.row + 1;
            }
        },
        {
            targets: 2,
            render: function (data, type, row) {
                if (row._isNew) {
                    return `
                        <input type="checkbox" id="monChinhAdd" name="monChinh" ${hasMainSport() ? "disabled" : ""} ${row.MonTheThaoChinh ? "" : "checked"}>
                    `;
                } else {
                    return row.MonTheThaoChinh ? `<span class="TrangThai green-text">Có</span>` : `<span class="TrangThai red-text">Không</span>`;
                }
            }
        },
        {
            targets: 3,
            render: function (data, type, row, meta) {
                if (row._isNew) {
                    return `
                        <button type="button" class="btnSave hgi-icon hgi-check"></button>
                        <button type="button" class="btnCancel hgi-icon hgi-cancel"></button>
                    `;
                }

                const editIcon = `<i class="edit-monTheThao cursor-pointer me-2" 
                                    data-index="${meta.row + 1}">
                                    <i class="hgi-icon hgi-edit"></i></i>`;
                const deleteIcon = row.isExisting
                    ? `<i class="delete-existing-monTheThao cursor-pointer text-danger" 
                            data-id="${row.monTheThaoID}">
                            <i class="hgi-icon hgi-delete"></i></i>`
                    : `<i class="delete-monTheThao cursor-pointer text-danger" 
                            data-index="${meta.row + 1}">
                            <i class="hgi-icon hgi-delete"></i></i>`;
                return editIcon + deleteIcon;
            }
        }
    ];

    const tableCols = [
        { data: null, width: "40px", class: "center-align" },
        { data: "tenMon", class: "left-align" },
        { data: "MonTheThaoChinh", width: "180px", class: "left-align" },
        { data: null, width: "120px", class: "center-align group-icon-action" }
    ];

    const table = $('#tableTheThao').DataTable({
        ajax: tableApi,
        columns: tableCols,
        columnDefs: tableDefs,
        paging: false,
        searching: false,
        info: false,
        ordering: false,
        autoWidth: false,
        responsive: true,
        language: {
            emptyTable: "Chưa có môn thể thao nào"
        }
    });

    // Lấy danh sách tất cả môn ngay khi init
    initListTheThao();
    // Hàm sinh select, loại bỏ môn đã có trong bảng

    // === Thêm dòng mới ===
    let newRow = null;
    let originalRowData = null;

    $('#btnEditRow').on('click', function () {
        if (newRow) return;

        const rowData = {
            stt: table.rows().count() + 1,
            tenMon: createSelectSport(),
            MonTheThaoChinh: `<input type="checkbox" id="monChinhAdd" name="monChinh" ${hasMainSport() ? "disabled" : ""}>`,
            _isNew: true
        };

        newRow = table.row.add(rowData).draw(false).node();
        $(newRow).addClass('adding');
        initSelect2();
    });

    // === Hủy thêm hoặc chỉnh sửa dòng ===
    $('#tableTheThao').on('click', '.btnCancel', function () {
        if (originalRowData) {
            // Nếu đang edit, restore dữ liệu gốc
            table.row(newRow).data(originalRowData).draw(false);
            $(newRow).removeClass('adding');
            originalRowData = null;
        } else {
            // Nếu đang add mới, remove dòng "ảo"
            table.row('.adding').remove().draw(false);
            // hoặc reload để chắc chắn đồng bộ
            // table.ajax.reload();
        }
        newRow = null;
    });

    // === Chỉnh sửa dòng ===
    $('#tableTheThao').on('click', '.edit-monTheThao', function () {
        if (newRow) return; // đang có row thêm mới thì không cho edit

        const rowNode = $(this).closest('tr');
        const rowData = table.row(rowNode).data();

        // Lưu lại dữ liệu gốc để restore khi cancel
        originalRowData = { ...rowData };

        const editRowData = {
            stt: rowData.stt,
            tenMon: createSelectSport(rowData.monTheThaoID),
            MonTheThaoChinh: `
                <input type="checkbox" id="monChinhAdd" name="monChinh"
                    ${(!rowData.MonTheThaoChinh && hasMainSport()) ? "disabled" : ""}
                    ${rowData.MonTheThaoChinh ? "checked" : ""}>
            `,
            _isNew: true
        };

        table.row(rowNode).data(editRowData).draw(false);
        $(rowNode).addClass('adding');
        initSelect2();
        newRow = rowNode;
    });

    // === Lưu dòng mới hoặc chỉnh sửa (AddRelation / UpdateRelation) ===
    $('#tableTheThao').on('click', '.btnSave', function () {
        let monTheThaoID = $('#selectMonTheThao').val();
        let monTheThaoText = $('#selectMonTheThao option:selected').text();
        let MonTheThaoChinh = $('#monChinhAdd').is(':checked');

        if (!monTheThaoID) {
            showNotification(0, "Vui lòng chọn môn thể thao!");
            return;
        }

        const request = {
            caNhanID: id,   // id từ initTableTheThao(id)
            monTheThaoID: monTheThaoID,
            monTheThaoChinh: MonTheThaoChinh
        };

        // Nếu có originalRowData => đang chỉnh sửa => Update
        const isEdit = originalRowData !== null;
        const apiUrl = isEdit
            ? `${baseUrl}/api/DM_CaNhan_CanBoTheThaoApi/UpdateRelation`
            : `${baseUrl}/api/DM_CaNhan_CanBoTheThaoApi/AddRelation`;
        const method = isEdit ? "PUT" : "POST";

        $.ajax({
            url: apiUrl,
            type: method,
            data: JSON.stringify(request),
            contentType: "application/json",
            success: function (res) {
                if (res && res.isSuccess) {
                    showNotification(1, isEdit ? "Cập nhật môn thể thao thành công!" : "Thêm môn thể thao thành công!");
                    table.ajax.reload();
                } else {
                    showNotification(0, res.message || "Không thể lưu dữ liệu!");
                    // Restore nếu là edit và fail
                    if (isEdit && originalRowData && newRow) {
                        table.row(newRow).data(originalRowData).draw(false);
                    } else if (!isEdit) {
                        // Nếu thêm mới fail thì remove dòng "ảo"
                        table.row('.adding').remove().draw(false);
                    }
                }
            },
            error: function () {
                showNotification(0, "Có lỗi xảy ra khi gọi API!");
                if (isEdit && originalRowData && newRow) {
                    table.row(newRow).data(originalRowData).draw(false);
                } else if (!isEdit) {
                    table.row('.adding').remove().draw(false);
                }
            }
        });

        newRow = null;
        originalRowData = null;
    });

    // === Xử lý checkbox MonTheThaoChinh ===
    $('#tableTheThao').on('change', 'input[name="monChinh"]', function () {
        if ($(this).is(':checked')) {
            // Uncheck tất cả checkbox khác
            $('input[name="monChinh"]').not(this).prop('checked', false);
        }
    });

    // === Xóa dòng (dùng API DeleteRelation) ===
    $('#tableTheThao').on('click', '.delete-monTheThao', function () {
        const rowNode = $(this).closest('tr');
        const rowData = table.row(rowNode).data();

        console.log('Dữ liệu dòng cần xóa:', rowData);

        $('#idDelete').val(rowData.monTheThaoID);
        $('#nameDelete').text(`${rowData.tenMon}`);
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

    function hasMainSport() {
        return tempTheThao.some(x => x.MonTheThaoChinh === true);
    }

    function createSelectSport(selectedId = "") {
        let options = `<option value="">Chọn</option>`;

        // Lấy danh sách môn đã gắn cho VĐV
        const existingSports = $('#tableTheThao').DataTable().data().toArray()
            .map(r => r.monTheThaoID);

        allSports.forEach(s => {
            // Bỏ qua nếu môn đã có và không phải môn đang edit
            if (existingSports.includes(s.monTheThaoID) && s.monTheThaoID !== selectedId) {
                return;
            }
            options += `<option value="${s.monTheThaoID}" ${s.monTheThaoID === selectedId ? "selected" : ""}>${s.tenMon}</option>`;
        });

        return `<select id="selectMonTheThao" class="form-control">${options}</select>`;
    }

    function dataDelete() {
        let monTheThaoID = $('#idDelete').val();
        $.ajax({
            url: `${baseUrl}/api/DM_CaNhan_CanBoTheThaoApi/DeleteRelation?CaNhanID=${NhanSuId}&MonTheThaoID=${monTheThaoID}`,
            type: "DELETE",
            success: function (res) {
                if (res && res.isSuccess) {
                    showNotification(1, "Xóa môn thể thao thành công!");
                    table.ajax.reload();
                    $('#modalDelete').modal('hide');
                } else {
                    showNotification(0, res.message || "Không thể xóa môn thể thao!");
                }
            },
            error: function () {
                showNotification(0, "Có lỗi xảy ra khi gọi API xóa!");
            }
        });
    }
}

function initListTheThao() {
    return $.ajax({
        url: `${baseUrl}/api/TT_MonTheThaoApi/Gets?MaNgonNgu=vi`,
        type: "GET",
        contentType: "application/json",
        success: function (res) {
            if (res && res.isSuccess) {
                allSports = res.value || [];
            }
        }
    });
}
//#endregion Function Xử lý bảng thể chất