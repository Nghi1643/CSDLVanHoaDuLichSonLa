const baseUrl = getRootLink();
let translations = [];
let translationsDaPhuongTien = [];
let translationsDiaDiem = [];
console.log(SuKienId)

$(document).ready(function () {

    //#region Xử lý các function chung
    initSelect2()
    initValidation()
    initDatePicker()
    initDanhMucChung_NoAll(39, "#loaiDoiTuong", "") // Loại đối tượng - Cấp độ
    initDanhMucChung_NoAll(42, "#trangThaiSuKien", "") // Trạng thái sự kiện
    initDanhMucChung_NoAll(25, "#theLoaiAdd", "") // Thể loại - Lĩnh vực
    initDanhMucChung_NoAll(9, "#linhVucAdd", "") // Lĩnh vực
    initNgonNgu("#ngonNguDichAdd")
    initNgonNgu("#ngonNguDPT")
    initNgonNgu("#ngonNguDichDiaDiemAdd")
    initTinhThanhPho()
    initDiaDiem()
    previewPicture()
    previewPictureSuKien()
    switchMediaData();
    CheckDateMinMax("#ngayBatDauAdd", "#ngayKetThucAdd")
    function initValidation() {
        CheckLengthEach('#tenSuKien', 255, null);
        CheckLengthEach('#tenDiaDiem', 255, null);
        CheckLengthEach('#diaChi', 255, null);
        CheckLengthEach('#noiDungSuKien', 1000, null);
    }

    $('#tinhThanhPhoAdd').on('change', function () {
        let tinhID = $(this).val();
        initPhuongXa(tinhID)
    })
    //#endregion Xử lý các function chung


    //#region Xử lý thông tin sự kiện
    setTimeout(() => {
        loadData(SuKienId)
    }, 100);

    // Khi thay đổi ngôn ngữ dịch, reset các input trong phần đa ngữ
    $(document).on('change', '#ngonNguDichAdd', function () {
        let maNgonNgu = $(this).val();
        
        if (SuKienId && maNgonNgu) {
            console.log('Chuyển ngôn ngữ dịch cho sự kiện hiện tại:', maNgonNgu);
            // Load bản dịch cho ngôn ngữ được chọn
            loadTranslation(SuKienId, maNgonNgu);
        } else {
            // Nếu chưa có cơ quan (tạo mới) hoặc chưa chọn ngôn ngữ, clear các trường dịch
            if (!SuKienId || !maNgonNgu) {
                $("#tenSuKienDichAdd").val('');
                $("#ketQuaAdd").val('');
                $("#moTaAdd").val('');
            }
        }
    });

    $("#btnSaveAdd").on("click", function () {
        dataAdd()
    })
    //#endregion Xử lý thông tin sự kiện


    //#region Xử lý tọa độ, vị trí bản đồ
    $("#modalAddDiaDiem").on('hidden.bs.modal', function () {
        translationsDiaDiem = []
        resetModal(this)
    })

    $(document).on('change', '#DiaDiemCoSanAdd', function () {
        let listDiaDiem = $(this).val();
        initTableDiaDiem(listDiaDiem.map(item => item).join(','));
    });

    $("#btnAddDiaDiem").on("click", function () {
        dataAddDiaDiem()
    })
    //#endregion Xử lý tọa độ, vị trí bản đồ

    
    //#region Xử lý Đa phương tiện
    $("#modalAddDaPhuongTien").on('hidden.bs.modal', function () {
        translationsDaPhuongTien = []
        resetModal(this)
    })

    $('#btnAddDaPhuongTien').on('click', function () {
        $('#modalAddDaPhuongTien').modal('show');
        resetModal(this)
    });

    $("#formAddDaPhuongTien").on("submit", function (e) {
        e.preventDefault();
        dataAddDaPhuongTien();
    });

    $("#ngonNguDPT").on("change", function () {
        let maNgonNgu = $(this).val();
        let exist = translationsDaPhuongTien.find(t => t.maNgonNgu === maNgonNgu);

        if (exist) {
            $("#tieuDeDPT").val(exist.tieuDe);
            $("#moTaDPT").val(exist.moTa);
        } else {
            // reset nếu chưa có dữ liệu
            $("#tieuDeDPT").val("");
            $("#moTaDPT").val("");
        }
        console.log("Sau khi đổi ngôn ngữ:", translationsDaPhuongTien);
    });

    $(document).on('click', '.edit-DaPhuongTien', function () {
        var id = $(this).attr("ID").match(/\d+/)[0];
        var data = $('#dataGridDaPhuongTien').DataTable().row(id).data();
        loadDataDaPhuongTien_FromTable(data);

        $("#modalAddDaPhuongTien").modal("show");
    });

    $(document).on('click', '.delete-DaPhuongTien', function () {
        var id = $(this).attr("ID").match(/\d+/)[0];
        var data = $('#dataGridDaPhuongTien').DataTable().row(id).data();

        $('#idDelete').val(data.daPhuongTienID);
        $('#nameDelete').text(`${data.tieuDe}`);

        $('#modalDelete').modal('show');
    });

    $("#formDelete").on("submit", function (e) {
        e.preventDefault();
        deleteDaPhuongTien();
    });
    //#endregion Xử lý Đa phương tiện
});

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
//#endregion Function Xử lý chung

//#region Function Xử lý thông tin sự kiện
function loadData(SuKienId) {
    getDataWithApi('GET', `/api/DM_SuKienApi/Get?MaNgonNgu=vi&SuKienID=${SuKienId}`).then(data => {
        console.log(data)
        if (data && data.isSuccess && data.value) {
            let suKien = data.value;
            $("#maSuKien").val(suKien.maDinhDanh);
            $("#loaiDoiTuong").val(suKien.capDoID).trigger('change');
            $("#trangThaiSuKien").val(suKien.trangThaiID).trigger('change');
            $("#ngayBatDauAdd").val(formatDateWithoutTime(suKien.batDau));
            $("#ngayKetThucAdd").val(formatDateWithoutTime(suKien.batDau));
            $("#thuTu").val(suKien.thuTu);
            $("input[name='trangThai'][value='" + (suKien.trangThai ? 1 : 0) + "']").prop("checked", true);
            if (suKien.anhDaiDien) {
                $("#showAnhDaiDienAdd").attr("src", suKien.anhDaiDien);
                $("#showAnhDaiDienDetail").val(suKien.anhDaiDien);
            }
            if (suKien.listIDDiaDiem) {
                console.log(suKien.listIDDiaDiem, "Danh sách địa điểm đã chọn")
                let listDiaDiem = suKien.listIDDiaDiem.split(',').map(x => x.trim().toLowerCase());
                $("#DiaDiemCoSanAdd").val(listDiaDiem.map(item => item)).trigger('change');
                initTableDiaDiem(listDiaDiem.map(item => item).join(','));
            }
            //Đa ngữ
            $("#ngonNguDichAdd").val(suKien.maNgonNgu).trigger('change');
            $("#tenSuKienDichAdd").val(suKien.tenSuKien);
            $("#ketQuaAdd").val(suKien.ketQua);
            $("#moTaAdd").val(suKien.moTa);
            initDaPhuongTienTable(suKien.suKienID);
        }
    })

}

function dataAdd() { 
    //Thông tin chung
    const IDSuKien = SuKienId;
    const MaSuKien = $("#maSuKien").val();
    const CapDoID = $("#loaiDoiTuong").val();
    const TrangThaiSuKien = $("#trangThaiSuKien").val();
    const NgayBatDau = formatDateToSaveAsString($("#ngayBatDauAdd").val());
    const NgayKetThuc = formatDateToSaveAsString($("#ngayKetThucAdd").val());
    const ThuTu = $("#thuTu").val();
    const TrangThai = $("input[name='trangThai']:checked").val();

    //Nội dung đa ngữ
    const MaNgonNgu = $("#ngonNguDichAdd").val();
    const TenSuKien = $("#tenSuKienDichAdd").val();
    const KetQua = $("#ketQuaAdd").val();
    const MoTa = $("#moTaAdd").val();
    const AnhDaiDienDetail =  $("#showAnhDaiDienDetail").val();
    const AnhDaiDien = $("#fileAnhDaiDienPrev")[0].files[0] || null;

    console.log(AnhDaiDien, "anhDaiDien")

    let dtChung = {
        suKienID: IDSuKien,
        TenSuKien: TenSuKien,
        MaDinhDanh: MaSuKien,
        CapDoID: CapDoID,
        BatDau: NgayBatDau,
        KetThuc: NgayKetThuc,
        TrangThaiID: TrangThaiSuKien,
        ThuTu: ThuTu,
        AnhDaiDienDetail: AnhDaiDienDetail,
        TrangThai: TrangThai == 1 ? true : false,
        ListID: $("#DiaDiemCoSanAdd").val().map(item => item).join(','),
        LinhVucID: 2,

        //Nội dung đa ngữ
        MaNgonNgu: MaNgonNgu,
        KetQua: KetQua,
        MoTa: MoTa,
    }

    console.log(dtChung)

    let formData = new FormData()
    formData.append('EntitySuKien', JSON.stringify(dtChung))
    formData.append("File", AnhDaiDien)

    $.ajax({
        type: 'PUT',
        url: `${baseUrl}/api/DM_SuKienApi/Update`,
        async: false,
        contentType: false,
        processData: false,
        data: formData,
        success: function (data) {
            if (data.isSuccess && data.value) {
                showNotification(1, "Chỉnh sửa sự kiện thành công");
                setTimeout(() => {
                    window.location.href = `${baseUrl}/AdminTool/SuKienVanHoa/SuKien`;
                }, 1500);
            } else {
                showNotification(0, "Chỉnh sửa sự kiện không thành công");
                console.log(data.error)
            }
        },
        error: function (err) {
            console.log(err)
        }
    })
}

function loadTranslation(toChucID, maNgonNgu) {
    getDataWithApi('GET', `/api/DM_SuKienApi/Get?SuKienID=${toChucID}&MaNgonNgu=${maNgonNgu}`).then(data => {
        console.log(data, "Load bản dịch sự kiện")
        if (data && data.isSuccess && data.value) {
            translations = data.value;
            // Populate các trường dịch với dữ liệu đa ngữ
            $("#tenSuKienDichAdd").val(translations.tenSuKien);
            $("#ketQuaAdd").val(translations.ketQua);
            $("#moTaAdd").val(translations.moTa);
        } else {
            // Nếu không có bản dịch, clear các trường dịch
            $("#tenSuKienDichAdd").val('');
            $("#ketQuaAdd").val('');
            $("#moTaAdd").val('');
        }
    })
}
//#endregion Function Xử lý thông tin sự kiện

//#region Function Xử lý thông tin tọa độ, vị trí bản đồ
function initDiaDiem() {
    let dt = JSON.stringify({
        "maNgonNgu": "vi",
        "trangThai": true
    })

    getDataWithApi('POST', '/api/DiaDiemApi/DanhSach', dt).then(data => {
        console.log(data)
        if (data && data.isSuccess && data.value) {
            console.log(data)
            $("#DiaDiemCoSanAdd").empty().append(`<option value="">Chọn</option>`);
            data.value.forEach(item => {
                const displayText = item.tenDiaDiem || item.noiDung || 'Không rõ tên';
                $("#DiaDiemCoSanAdd").append(`<option value="${item.diaDiemID || item.id}">${displayText}</option>`);
            });
        }
    })
}

function dataAddDiaDiem() {
    //Thông tin địa điểm
    const DiaDiemID = null; //Chưa có trường địa điểm trong form thêm sự kiện
    const DiaDiemCapChaID = null //Chưa có trường địa điểm trong form thêm sự kiện
    const LinhVucID = $("#linhVucAdd").val();
    const XaID = $("#phuongXaAdd").val();
    const TinhID = $("#tinhThanhPhoAdd").val();
    const KinhDo = $("#kinhDoAdd").val();
    const ViDo = $("#viDoAdd").val();
    const CaoDo = $("#caoDoAdd").val();
    const BanDo = null; //Chưa có trường địa điểm trong form thêm sự kiện
    const HinhAnh = null; //Chưa có trường địa điểm trong form thêm sự kiện
    const NguoiKhuyetTat = $("#nguoiKhuyetTatAdd").is(":checked");
    const NhaVeSinh = $("#nhaVeSinhAdd").is(":checked");
    const BaiDoXe = $("#baiDoXeAdd").is(":checked");

    //Thông tin địa điểm dịch
    const MaNgonNguDiaDiem = $("#ngonNguDichDiaDiemAdd").val();
    const ThoiGianHoatDong = $("#thoiGianHoatDongDichAdd").val();
    const TenDiaDiem = $("#tenDiaDiemDichAdd").val();
    const SucChua = $("#sucChuaDichAdd").val();
    const HeToaDo = null; //Chưa có trường địa điểm trong form thêm sự kiện
    const DiaChiDiaDiem = $("#diaChiDichAdd").val();
    const MoTaDiaDiem = $("#moTaDichAdd").val();

    let diaDiem = {
        DiaDiemID: DiaDiemID,
        DiaDiemCapChaID: DiaDiemCapChaID,
        LinhVucID: LinhVucID,
        XaID: XaID,
        TinhID: TinhID,
        KinhDo: KinhDo,
        ViDo: ViDo,
        CaoDo: CaoDo,
        BanDo: BanDo,
        HinhAnh: HinhAnh,
        NguoiKhuyetTat: NguoiKhuyetTat,
        NhaVeSinh: NhaVeSinh,
        BaiDoXe: BaiDoXe,
    }


    let diaDiem_NoiDung = [{
        //Nội dung đa ngữ
        DiaDiemID: DiaDiemID,
        MaNgonNgu: MaNgonNguDiaDiem,
        ThoiGianHoatDong: ThoiGianHoatDong,
        TenDiaDiem: TenDiaDiem,
        SucChua: SucChua,
        HeToaDo: HeToaDo,
        DiaChi: DiaChiDiaDiem,
        MoTa: MoTaDiaDiem
    }]

    let formData = new FormData()
    formData.append("DiaDiem", JSON.stringify(diaDiem))
    formData.append("DiaDiem_NoiDung", JSON.stringify(diaDiem_NoiDung))

    $.ajax({
        type: 'POST',
        url: `${baseUrl}/api/DiaDiemApi/ThemMoi`,
        async: false,
        contentType: false,
        processData: false,
        data: formData,
        success: function (data) {
            console.log(data)
            if (data.isSuccess && data.value) {
                // Lưu lại id địa điểm vừa thêm mới
                var newDiaDiemId = data.value.diaDiemID || data.value.id || data.value;
                console.log("Địa điểm vừa thêm mới có ID:", newDiaDiemId);
                // Lưu lại các giá trị select đã chọn trước đó
                var prevDiaDiem = $('#DiaDiemCoSanAdd').val();
                console.log("Giá trị đã chọn trước đó:", prevDiaDiem);
                let dt = JSON.stringify({
                    "maNgonNgu": "vi",
                    "trangThai": true
                })
                console.log(dt)
                getDataWithApi('POST', '/api/DiaDiemApi/DanhSach', dt).then(data => {
                    console.log(data)
                    if (data && data.isSuccess && data.value) {
                        var $select = $('#DiaDiemCoSanAdd');
                        var prevValues = Array.isArray(prevDiaDiem) ? prevDiaDiem : [prevDiaDiem];
                        $select.empty();
                        $select.append('<option value="">Chọn</option>');
                        data.value.forEach(function (item) {
                            console.log(item, "Danh sách địa điểm")
                            var selected = '';
                            if (prevValues.includes(item.diaDiemID?.toString()) || prevValues.includes(item.id?.toString())) selected = 'selected';
                            if (item.diaDiemID == newDiaDiemId || item.id == newDiaDiemId) selected = 'selected';
                            $select.append(`<option value="${item.diaDiemID || item.id}" ${selected}>${item.tenDiaDiem || item.noiDung || 'Không rõ tên'}</option>`);
                        });
                        prevValues.push(newDiaDiemId)
                        $select.trigger('change.select2');
                        initTableDiaDiem(prevValues.map(item => item).join(','));
                    }
                })
                $('#modalAddDiaDiem').modal('hide');
            }
        },
        error: function (err) {
            console.log(err)
        }
    })
}

function initTableDiaDiem(list) {
    const tableApi = {
        url: `${baseUrl}/api/DiaDiemApi/GetMulti?ListID=${list}&MaNgonNgu=vi`,
        type: "GET",
        contentType: 'application/json; charset=utf-8',
        dataSrc: function (data) {
            console.log(data)
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
            targets: 3, // Cột số lượng cán bộ
            render: function (data, type, row, meta) {
                return `<div class="group-info">
                            <div class="">Kinh độ: ${row.kinhDo ? row.kinhDo : "-"}</div>
                            <div class="">Vĩ độ: ${row.viDo ? row.viDo : "-"}</div>
                        </div>`;
            }
        }
    ];

    const tableCols = [
        { "data": "stt", "width": "40px", "class": "center-align" },
        { "data": "tenDiaDiem", "class": "left-align name-text" },
        { "data": "diaChi", "width": "30%", "class": "left-align" },
        { "data": "", "width": "30%", "class": "left-align" }
    ];

    initDataTableConfigNoSearch('dataGridDiaDiem', tableApi, tableDefs, tableCols);
}

function loadTranslationDiaDiem(diaDiemID, maNgonNgu) {
    getDataWithApi('GET', `/api/DiaDiemApi/GetByID?MaNgonNgu=${maNgonNgu}&DiaDiemID=${diaDiemID}`).then(data => {
        console.log(data, "Load bản dịch địa điểm")
        if (data && data.isSuccess && data.value) {
            translationsDiaDiem = data.value;
            // Populate các trường dịch với dữ liệu đa ngữ
            $("#thoiGianHoatDongDichEdit").val(translationsDiaDiem.thoiGianHoatDong);
            $("#tenDiaDiemDichEdit").val(translationsDiaDiem.tenDiaDiem);
            $("#sucChuaDichEdit").val(translationsDiaDiem.sucChua);
            $("#diaChiDichEdit").val(translationsDiaDiem.diaChi);
            $("#moTaDichEdit").val(translationsDiaDiem.moTa);
        } else {
            // Nếu không có bản dịch, clear các trường dịch
            $("#thoiGianHoatDongDichEdit").val('');
            $("#tenDiaDiemDichEdit").val('');
            $("#sucChuaDichEdit").val('');
            $("#diaChiDichEdit").val('');
            $("#moTaDichEdit").val('');
        }
    })
}
//#endregion Function Xử lý thông tin tọa độ, vị trí bản đồ

//#region Function Xử lý đa phương tiện
function initDaPhuongTienTable(ID) {
    // Khởi tạo DataTable cho bảng đa phương tiện tạm
    const tableApi = {
        url: `${baseUrl}/api/DaPhuongTienApi/DanhSach`,
        type: "POST",
        data: function (d) {
            return JSON.stringify({
                "doiTuongSoHuuID": ID,
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
                return `<div class="group-info have-image">
                    <div class="have-image">
                        <img src="${row.duongDanFile || '/assets/images/vector/no-image.png'}" alt="${row.tenDaPhuongTien || ''}" onerror="this.onerror=null;this.src='/assets/images/vector/no-image.png';" />
                    </div>
                    <div class="group-info">
                    <div class="info-main"><div>${row.tieuDe || ''}</div></div>
                    <div class="info-sub">${row.TacGia || ''}</div></div>
                </div>`;
            }
        },
        {
            targets: 2,
            render: function (data, type, row, meta) {
                if (row.loaiMedia == 1) {
                    return `Hình ảnh`;
                } else if (row.loaiMedia == 2) {
                    return `Video`;
                } else if (row.loaiMedia == 3) {
                    return `Audio`;
                } else if (row.loaiMedia == 4) {
                    return `File khác`;
                }
                return ``;
            }
        },
        {
            targets: 4, // Cột chức năng
            render: function (data, type, row, meta) {
                const editIcon = `<i data-toggle="tooltip" title="Chỉnh sửa" class="edit-DaPhuongTien text-yellow cursor-pointer me-2" data-index="${meta.row}" id="${meta.row}">
                    <i class="hgi-icon hgi-edit"></i>
                </i>`;

                const deleteIcon = `<i data-toggle="tooltip" title="Xóa khỏi danh sách" class="delete-DaPhuongTien text-red cursor-pointer" data-index="${meta.row}" id="${meta.row}">
                        <i class="hgi-icon hgi-delete"></i>
                    </i>`;

                return editIcon + deleteIcon;;
            }
        }
    ];

    const tableCols = [
        { "data": "stt", "width": "40px", "class": "center-align" },
        { "data": "", "width": "", "class": "left-align" },
        { "data": "", "width": "15%", "class": "left-align" },
        { "data": "tacGia", "width": "15%", "class": "left-align" },
        { "data": "", "width": "120px", "class": "center-align group-icon-action" }
    ];

    // Destroy existing DataTable if exists
    if ($.fn.DataTable.isDataTable('#dataGridDaPhuongTien')) {
        $('#dataGridDaPhuongTien').DataTable().destroy();
    }

    initDataTableConfigNoSearch('dataGridDaPhuongTien', tableApi, tableDefs, tableCols);
}

function dataAddDaPhuongTien() {
    let daPhuongTienID = $('#idDPT').val() || null
    let loaiMedia = $("input[name='loaiMediaDPT']:checked").val();
    let fileDPT = $("#fileDPT")[0].files[0] || null;
    let tacGia = $("#tacGiaDPT").val();
    let thuTu = $("#thuTuDPT").val();
    let trangThai = $("input[name='trangThaiDPT']:checked").val();
    let duongDanFile = $("#duongDanFileDPT").val();

    let maNgonNgu = $("#ngonNguDPT").val();
    let tieuDe = $("#tieuDeDPT").val().trim();
    let moTa = $("#moTaDPT").val().trim();

    const DaPhuongTien = {
        LoaiMedia: Number(loaiMedia),
        LoaiDoiTuong: "VH_LeHoi",
        doiTuongSoHuuID: SuKienId,
        DuongDanFile: duongDanFile,
        TacGia: tacGia,
        ThuTuHienThi: thuTu || 1,
        SuDung: trangThai == "1" ? true : false
    };

    const DaPhuongTien_NoiDung = [{
        maNgonNgu: maNgonNgu,
        tieuDe: tieuDe,
        moTa: moTa
    }]

    const FileDinhKem = [];
    if (fileDPT) {
        FileDinhKem.push(fileDPT);
    }

    console.log(DaPhuongTien, DaPhuongTien_NoiDung, FileDinhKem)

    let formData = new FormData();
    formData.append('DaPhuongTien', JSON.stringify(DaPhuongTien));
    formData.append('DaPhuongTien_NoiDung', JSON.stringify(DaPhuongTien_NoiDung));
    if (fileDPT) formData.append('File', fileDPT);

    if (daPhuongTienID != null) {
        $.ajax({
            type: 'PUT',
            url: `${baseUrl}/api/DaPhuongTienApi/ChinhSua/${daPhuongTienID}`,
            async: false,
            contentType: false,
            processData: false,
            data: formData,
            success: function (data) {
                if (data.isSuccess && data.value) {
                    showNotification(1, "Chỉnh sửa đa phương tiện thành công");
                    $('#modalAddDaPhuongTien').modal('hide');
                    initDaPhuongTienTable(SuKienId);
                } else {
                    showNotification(0, "Chỉnh sửa đa phương tiện không thành công");
                    console.log(data.error) 
                }
            },
            error: function (err) {
                console.log(err)
            }
        })
    } else {
        $.ajax({
            type: 'POST',
            url: `${baseUrl}/api/DaPhuongTienApi/ThemMoi`,
            async: false,
            contentType: false,
            processData: false,
            data: formData,
            success: function (data) {
                if (data.isSuccess && data.value) {
                    showNotification(1, "Thêm mới đa phương tiện thành công");
                    $('#modalAddDaPhuongTien').modal('hide');
                    initDaPhuongTienTable(SuKienId);
                } else {
                    showNotification(0, "Thêm mới đa phương tiện không thành công");
                    console.log(data.error) 
                }
            },
            error: function (err) {
                console.log(err)
            }
        })
    }
}

function loadDataDaPhuongTien_FromTable(data) {
    console.log(data, "data đa phương tiện")
    $('#idDPT').val(data.daPhuongTienID);
    // Các input text
    $("input[name='loaiMediaDPT'][value='" + data.loaiMedia + "']").prop("checked", true);
    if (data.duongDanFile) {
        $("#showFileEdit").text(data.duongDanFile);
        $("#duongDanFileDPT").val(data.duongDanFile);
        $("#showhinhAnhAdd").attr("src", data.duongDanFile);
    } else {
        $("#showFileEdit").text("");
    }
    $("#tacGiaDPT").val(data.tacGia);
    $("#thuTuDPT").val(data.thuTuHienThi);
    $(`input[name='trangThaiAdd'][value='${data.suDung ? 1 : 0}']`).prop('checked', true);

    // Radio trạng thái
    $(`input[name='trangThaiAdd'][value='${data.suDung ? 1 : 0}']`).prop('checked', true);

    $('#ngonNguDPT').val('vi').trigger('change');
    loadTranslationDaPhuongTien(data.daPhuongTienID, 'vi');
}

function loadTranslationDaPhuongTien(daPhuongTienID, maNgonNgu) {
    getDataWithApi('GET', `/api/DaPhuongTienApi/BanDich/${daPhuongTienID}/${maNgonNgu}`).then(data => {
        console.log(data, "Load bản dịch đa phương tiện")
        if (data && data.isSuccess && data.value) {
            translationsDaPhuongTien = data.value;
            var banDich = translationsDaPhuongTien.find(el => el.maNgonNgu === maNgonNgu)
            // Populate các trường dịch với dữ liệu đa ngữ
            $('#tieuDeDPT').val(banDich.tieuDe);
            $('#moTaDPT').val(banDich.moTa);
        } else {
            // Nếu không có bản dịch, clear các trường dịch
            $("#tieuDeDPT").val('');
            $("#moTaDPT").val('');
        }  
    })
}
//#endregion Function Xử lý đa phương tiện