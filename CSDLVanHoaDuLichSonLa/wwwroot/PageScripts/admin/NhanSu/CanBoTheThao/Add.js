const baseUrl = getRootLink();
const ckeditormoTaDichAdd = CKEDITOR.replace('moTaDichAdd');
const ckeditorthanhTichDichAdd = CKEDITOR.replace('thanhTichDichAdd');
const ckeditorghiChuDichAdd = CKEDITOR.replace('ghiChuDichAdd');

let tempTheThao = [];
let listTheThao = [];

$(document).ready(function () {
    //#region Xử lý các function chung
    initDatePicker();
    initDanhMucChung_NoAll(1, "#danTocAdd", "")
    initDanhMucChung_NoAll(5, "#gioiTinhAdd", "")
    initNgonNgu("#ngonNguDichAdd")
    initNgonNgu("#ngonNguDichDiaDiemAdd")
    initNgonNgu("#ngonNguDPT")
    initTinhThanhPho()
    initSelect2();
    previewPicture();
    initTableTheThao();
    listTheThao = initListTheThao();
    console.log('Danh sách môn thể thao từ API:', listTheThao);

    $('#tinhThanhPhoAdd').on('change', function () {
        let tinhID = $(this).val();
        initPhuongXa(tinhID)
    })
    //#endregion Xử lý các function chung

    //#region Xử lý lưu thông tin cán bộ thể thao
    $("#formNhanSuAdd").on("submit", function (e) {
        e.preventDefault(); // Ngăn chặn hành vi mặc định của form
        dataAdd();
    });

    $(document).on('change', '#ngonNguDichAdd', function() {
        $("#ngonNguDichAdd").val('');
        $("#hoVaTenAdd").val('');
        $("#diaChiAdd").val('');
        $("#noiLamViecAdd").val('');
        $("#theChatDichAdd").val('');
        if (typeof ckeditormoTaDichAdd !== 'undefined') ckeditormoTaDichAdd.setData('');
        if (typeof ckeditorthanhTichDichAdd !== 'undefined') ckeditorthanhTichDichAdd.setData('');
        if (typeof ckeditorghiChuDichAdd !== 'undefined') ckeditorghiChuDichAdd.setData('');
    });
    //#endregion Xử lý lưu thông tin cán bộ thể thao
})

//#region Function Xử lý chung
function initTinhThanhPho() {
    getDataWithApi('GET', '/api/CapTinhApi/Gets').then(data => {
        if (data && data.isSuccessed && data.resultObj) {
            $("#tinhThanhPhoAdd").empty().append(`<option value="">Chọn</option>`);
            data.resultObj.forEach(item => {
                const displayText = item.noiDung || item.tenTinh || 'Không rõ tên';
                $("#tinhThanhPhoAdd").append(`<option value="${item.tinhID}">${displayText}</option>`);
            });
        }
    })
}

function initPhuongXa(tinhID = null) {
    getDataWithApi('GET', `/api/CapXaApi/GetByMaTinh?MaNgonNgu=vi&MaTinh=${tinhID ? `${tinhID}` : ''}`).then(data => {
        $("#phuongXaAdd").empty().append(`<option value="">Chọn</option>`);
        if (data && data.isSuccess && data.value) {
            data.value.forEach(item => {
                const displayText = item.noiDung || item.tenXa || 'Không rõ tên';
                $("#phuongXaAdd").append(`<option value="${item.xaID}">${displayText}</option>`);
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
                $('#showAnhDaiDienAdd').attr('src', ev.target.result);
            };
            reader.readAsDataURL(file);
        } else {
            $('#showAnhDaiDienAdd').attr('src', '/assets/images/vector/addImage.png');
        }
    });
}
//#endregion Function Xử lý chung

//#region Function Thông tin cán bộ thể thao
function dataAdd() {
    const fileAnhDaiDien = $("#fileAnhDaiDienPrev")[0].files[0] || null;
    const maDinhDanh = $("#maDinhDanhAdd").val();
    const gioiTinh = $("#gioiTinhAdd").val();
    const ngaySinh = $("#ngaySinhAdd").val();
    const danToc = $("#danTocAdd").val();
    const soDienThoai = $("#soDienThoaiAdd").val();
    const hopThu = $("#hopThuAdd").val();
    const tinhThanhPho = $("#tinhThanhPhoAdd").val();
    const phuongXa = $("#phuongXaAdd").val();
    //const soThe = $("#soTheAdd").val();
    //const ngayCap = $("#ngayCapAdd").val();
    //const coQuanCap = $("#coQuanCapAdd").val();
    //const thuTu = $("#thuTuAdd").val();
    const trangThai = $("input[name='trangThai']:checked").val() == 1 ? true : false;

    // Đa ngữ
    const ngonNguDich = $("#ngonNguDichAdd").val();
    const hoVaTen = $("#hoVaTenAdd").val();
    const diaChi = $("#diaChiAdd").val();
    const noiLamViec = $("#noiLamViecAdd").val();
    const theChat = $("#theChatDichAdd").val();
    const moTa = ckeditormoTaDichAdd.getData();
    const thanhTich = ckeditorthanhTichDichAdd.getData();
    const ghiChu = ckeditorghiChuDichAdd.getData();

    // Kết quả trả về dạng object
    const dtChung = {
        MaDinhDanh: maDinhDanh,
        GioiTinh: gioiTinh,
        NgaySinh: ngaySinh,
        DanTocID: danToc,
        DienThoai: soDienThoai,
        HopThu: hopThu,
        TinhID: tinhThanhPho,
        XaID: phuongXa,
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

    let formData = new FormData()
    formData.append('RequestCanBoTheThao', JSON.stringify(dtChung))
    formData.append("File", fileAnhDaiDien)
    formData.append("RequestMonTheThao", JSON.stringify(tempTheThao))

    $.ajax({
        type: 'POST',
        url: `${baseUrl}/api/DM_CaNhan_CanBoTheThaoApi/Add`,
        async: false,
        contentType: false,
        processData: false,
        data: formData,
        success: function (data) {
            if (data.isSuccess && data.value) {
                showNotification(1, "Thêm mới cán bộ thể thao thành công");
                console.log(data)
                setTimeout(() => {
                    window.location.href = `${baseUrl}/AdminTool/NhanSu/CanBoTheThao`;
                }, 1500);
            } else {
                showNotification(0, "Thêm mới cán bộ thể thao không thành công");
                console.log(data.error)
            }
        },
        error: function (err) {
            console.log(err)
        }
    })

    console.log('Dữ liệu cán bộ thể thao:', dtChung);
}
//#endregion Function Thông tin cán bộ thể thao


//#region Function Xử lý bảng thể chất
function initTableTheThao() {
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
        { data: "monTheThao", class: "left-align" },
        { data: "MonTheThaoChinh", width: "180px", class: "left-align" },
        { data: null, width: "120px", class: "center-align group-icon-action" }
    ];

    const table = $('#tableTheThao').DataTable({
        data: tempTheThao,
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

    // Hàm sinh ra select (loại bỏ môn đã chọn rồi)
    function createSelectSport(selectedId = "") {
        let options = `<option value="">Chọn</option>`;
        listTheThao.forEach(s => {
            options += `<option value="${s.monTheThaoID}" ${s.monTheThaoID === selectedId ? "selected" : ""}>${s.tenMon}</option>`;
        });
        return `<select id="selectMonTheThao" class="form-control">${options}</select>`;
    }

    // === Thêm dòng mới ===
    let newRow = null;
    let originalRowData = null;
    $('#btnAddRow').on('click', function () {
        if (newRow) return;

        const rowData = {
            stt: table.rows().count() + 1,
            monTheThao: createSelectSport(),
            MonTheThaoChinh: `<input type="checkbox" id="monChinhAdd" name="monChinh" ${hasMainSport() ? "disabled" : ""}>`,
            _isNew: true
        };

        newRow = table.row.add(rowData).draw(false).node();
        $(newRow).addClass('adding');
        initSelect2();
    });

    // === Lưu dòng mới hoặc chỉnh sửa ===
    $('#tableTheThao').on('click', '.btnSave', function () {
        let monTheThaoID = $('#selectMonTheThao').val();
        let monTheThaoText = $('#selectMonTheThao option:selected').text();
        let MonTheThaoChinh = $('#monChinhAdd').is(':checked');

        if (!monTheThaoID) {
            showNotification(0, "Vui lòng chọn môn thể thao!");
            return;
        }

        // Cập nhật listTheThao (loại bỏ môn đã chọn)
        listTheThao = listTheThao.filter(s => s.monTheThaoID !== monTheThaoID);

        const rowData = {
            monTheThao: monTheThaoText,
            MonTheThaoChinh: MonTheThaoChinh ? "Có" : "Không",
            _isNew: false
        };

        const Data = {
            monTheThaoID: monTheThaoID,
            monTheThao: monTheThaoText,
            MonTheThaoChinh: MonTheThaoChinh
        };

        table.row('.adding').data(rowData).draw(false);

        // Nếu là edit thì replace, nếu là add thì push
        let idx = tempTheThao.findIndex(x => x.monTheThaoID === monTheThaoID);
        if (idx > -1) tempTheThao[idx] = Data;
        else tempTheThao.push(Data);

        // Đảm bảo chỉ có một MonTheThaoChinh = true
        if (MonTheThaoChinh) {
            tempTheThao.forEach(item => {
                if (item.monTheThaoID !== monTheThaoID) {
                    item.MonTheThaoChinh = false;
                }
            });
        }

        console.log('Dữ liệu tạm thời:', tempTheThao);

        table.clear().rows.add(tempTheThao).draw();
        newRow = null;
    });

    // === Hủy thêm dòng ===
    $('#tableTheThao').on('click', '.btnCancel', function () {
        if (originalRowData) {
            // Nếu là edit, restore dữ liệu gốc
            table.row(newRow).data(originalRowData).draw(false);
            $(newRow).removeClass('adding');
            originalRowData = null;
        } else {
            // Nếu là add mới, remove dòng
            table.row('.adding').remove().draw(false);
        }
        newRow = null;
    });

    // === Chỉnh sửa dòng ===
    $('#tableTheThao').on('click', '.edit-monTheThao', function () {
        if (newRow) return; // đang có row mới thì không cho edit

        const rowNode = $(this).closest('tr');
        const rowData = table.row(rowNode).data();

        // Lưu lại dữ liệu gốc để restore khi cancel
        originalRowData = { ...rowData };

        // Add lại môn cũ vào list để chọn lại
        listTheThao.push({ monTheThaoID: rowData.monTheThaoID, tenMon: rowData.monTheThao });

        const editRowData = {
            monTheThao: createSelectSport(rowData.monTheThaoID),
            MonTheThaoChinh: `<input type="checkbox" id="monChinhAdd" name="monChinh" ${(!rowData.MonTheThaoChinh && hasMainSport()) ? "disabled" : ""} ${rowData.MonTheThaoChinh ? "checked" : ""}>`,
            _isNew: true
        };

        table.row(rowNode).data(editRowData).draw(false);
        $(rowNode).addClass('adding');
        initSelect2();
        newRow = rowNode;
    });

    // === Xử lý checkbox MonTheThaoChinh ===
    $('#tableTheThao').on('change', 'input[name="monChinh"]', function () {
        if ($(this).is(':checked')) {
            // Uncheck tất cả checkbox khác
            $('input[name="monChinh"]').not(this).prop('checked', false);
        }
    });

    // === Xóa dòng ===
    $('#tableTheThao').on('click', '.delete-monTheThao', function () {
        const index = $(this).data('index') - 1; // stt bắt đầu từ 1
        const rowData = tempTheThao[index];

        // Add lại môn vào listTheThao
        listTheThao.push({ monTheThaoID: rowData.monTheThaoID, tenMon: rowData.monTheThao });

        // Remove từ tempTheThao
        tempTheThao.splice(index, 1);

        // Re-render table
        table.clear().rows.add(tempTheThao).draw();
    });

    // === Xóa dòng existing (nếu có) ===
    $('#tableTheThao').on('click', '.delete-existing-monTheThao', function () {
        const id = $(this).data('id');
        // Giả sử gọi API xóa, nhưng vì đây là add, có lẽ không cần
        // Hoặc remove local nếu cần
        showNotification(0, "Chức năng xóa existing chưa được implement!");
    });

    // Hàm kiểm tra xem đã có môn chính chưa
    function hasMainSport() {
        return tempTheThao.some(x => x.MonTheThaoChinh === true);
    }
}

function initListTheThao() {
    getDataWithApi('GET', '/api/TT_MonTheThaoApi/Gets?MaNgonNgu=vi').then(data => {
        if (data && data.isSuccess && data.value.length > 0) {
            listTheThao.push(...data.value);
        }
    })
    return listTheThao;
}
//#endregion Function Xử lý bảng thể chất