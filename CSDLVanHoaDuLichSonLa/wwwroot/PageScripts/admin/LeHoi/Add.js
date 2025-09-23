const baseUrl = getRootLink();
const ckeditorQuiDinhAdd = CKEDITOR.replace('quyDinhAdd');
const ckeditorNoiDungAdd = CKEDITOR.replace('gioiThieuLeHoiAdd');
const ckeditordanhGiaAdd = CKEDITOR.replace('danhGiaAdd');
let tempDaPhuongTiens = [];
let RequestXuatBan = [];
let tempDaPhuongTienTranslations = [];
let currentEditingDaPhuongTienIndex = -1;

$(document).ready(function () {
    initDatePicker();
    initDanhMucChung_NoAll(37, "#theLoaiAdd", "Thể loại lễ hội")
    initDanhMucChung_NoAll(38, "#capDoAdd", "")
    initDanhMucChung_NoAll(39, "#capQuanLyAdd", "")
    initDanhMucChung_NoAll(6, "#tanSuatAdd", "")
    initToChuc_NoAll(122, "#donViToChucAdd", "")
    initDanhMucChung_NoAll(1, "#danTocAdd", "")
    initDanhMucChung_NoAll(9, "#linhVucAdd", "")
    initNgonNgu("#ngonNguDichAdd")
    initNgonNgu("#ngonNguDichDiaDiemAdd")
    initNgonNgu("#ngonNguDPT")
    initTinhThanhPho()
    initDiaDiem()
    initSelect2();
    previewPicture();
    switchMediaData();
    $("#ngonNguDichDiaDiemAdd").select2({
        width: $(this).data('width') ? $(this).data('width') : $(this).hasClass('w-100') ? '100%' : 'style',
        language: "vi",
        placeholder: $(this).attr('placeholder') || $(this).data('placeholder') || 'Chọn...',
        dropdownParent: $('.content-wrapper').length > 0 ? $('.content-wrapper') : $('body')
    });


    // Event handler cho thay đổi tỉnh/thành phố
    $(document).on('change', '#tinhThanhPhoAdd', function() {
        const tinhID = $(this).val();
        if (tinhID) {
            initPhuongXa(tinhID);
        } else {
            $("#phuongXaAdd").empty().append(`<option value="">Chọn</option>`);
        }
    });

    $("#btnSaveAdd").on("click", function () {
        dataAdd()
    })

    $("#btnModalAddDiaDiem").on("click", function () {
        resetDiaDiemModal();
    })

    $("#btnAddDiaDiem").on("click", function () {
        dataAddDiaDiem()
    })

    // Khi thay đổi ngôn ngữ dịch, reset các input trong phần đa ngữ
    $(document).on('change', '#ngonNguDichAdd', function() {
        $('#tenLeHoiDichAdd').val('');
        $('#diaChiAdd').val('');
        $('#donViPhoiHopAdd').val('');
        $('#nhanVatPhungThoAdd').val('');
        if (typeof ckeditorNoiDungAdd !== 'undefined') ckeditorNoiDungAdd.setData('');
        if (typeof ckeditorQuiDinhAdd !== 'undefined') ckeditorQuiDinhAdd.setData('');
        if (typeof ckeditordanhGiaAdd !== 'undefined') ckeditordanhGiaAdd.setData('');
    });

    $(document).on('change', '#DiaDiemCoSanAdd', function() {
        let listDiaDiem = $(this).val();
        initTableDiaDiem(listDiaDiem.map(item => item).join(','));
    });

    // Event handler cho nút thêm đa phương tiện
    $('#btnAddDaPhuongTien').on('click', function() {
        currentEditingDaPhuongTienIndex = -1;
        tempDaPhuongTienTranslations = [];
        resetDaPhuongTienModal();
        $('#modalAddDaPhuongTien').modal('show');
    });

    // Event handlers cho bảng tạm
    $(document).on('click', '.edit-temp-DaPhuongTien', function() {
        const index = $(this).data('index');
        editTempDaPhuongTien(index);
    });

    $(document).on('click', '.delete-temp-DaPhuongTien', function() {
        const index = $(this).data('index');
        tempDaPhuongTiens.splice(index, 1);
        refreshTempDaPhuongTienTable();
    });

    /* $("#ngonNguDichAdd").on("change", function () {
        let maNgonNgu = $(this).val();
        let exist = tempDaPhuongTienTranslations.find(t => t.maNgonNgu === maNgonNgu);
        if (exist) {
            $("#tenDaPhuongTienAdd").val(exist.tenDaPhuongTien);
            $("#doiTuongDocGiaAdd").val(exist.doiTuongDocGia);
        } else {
            $("#tenDaPhuongTienAdd").val("");
            $("#doiTuongDocGiaAdd").val("");
        }
    }); */

    /* $("#ngonNguDich").on("change", function () {
        let maNgonNgu = $(this).val();
        const coQuanId = $('#coQuanId').val();
        
        if (coQuanId && maNgonNgu) {
            console.log('Chuyển ngôn ngữ dịch cho cơ quan hiện tại:', maNgonNgu);
            // Load bản dịch cho ngôn ngữ được chọn
            loadToChucTranslation(coQuanId, maNgonNgu);
        } else {
            // Nếu chưa có cơ quan (tạo mới) hoặc chưa chọn ngôn ngữ, clear các trường dịch
            if (!coQuanId) {
                $('#tenCoQuanDich').val('');
                $('#diaChiDich').val('');
                $('#gioiThieuDich').val('');
            }
        }
    }); */

    // Submit modal đa phương tiện
    $("#formAddDaPhuongTien").on("submit", function (e) {
        e.preventDefault();
        /* saveTempDaPhuongTienTranslation(); */
        saveDaPhuongTienToTemp();
    });

    $("#modalAddDiaDiem, #modalAddDaPhuongTien").on('hidden.bs.modal', function () {
        translations = []
        $(this).find('input[type=text], textarea').val('');
        $(this).find('select').val('vi').trigger('change');
        $(this).find('input[type=radio][value=1], input[type=checkbox][value=1]').prop('checked', true);
    });
});

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

function dataAdd() {
    //Thông tin chung
    const TenLeHoi = $("#tenLeHoiAdd").val();
    const MaDinhDanhLeHoi = $("#maDinhDanhAdd").val();
    const TheLoaiID = $("#theLoaiAdd").val();
    const NgayBatDau = formatDateToSaveAsString($("#ngayBatDauAdd").val());
    const NgayKetThuc = formatDateToSaveAsString($("#ngayKetThucAdd").val());
    const TanSuatID = $("#tanSuatAdd").val();
    const CapDoID = $("#capDoAdd").val();
    const CapQuanLyID = $("#capQuanLyAdd").val();
    const SoLuongThamGia = $("#soLuongThamGiaAdd").val();
    const ThoiGianDienRa = $("#thoiGianDienRaAdd").val();
    const DonViToChucID = $("#donViToChucAdd").val();
    const SoNgayDienRa = $("#soNgayDienRaAdd").val();
    const NamVHPHTQG = $("#namVHPHTQGAdd").val();
    const DanTocID = $("#danTocAdd").val();
    const LaTieuBieu = $("#laTieuBieuAdd").is(":checked");
    const LaCungDinh = $("#laCungDinhAdd").is(":checked");
    const TrangThaiToChuc = $("input[name='trangThaiToChucAdd']:checked").val();
    const TrangThai = $("input[name='trangThaiAdd']:checked").val();

    //Nội dung đa ngữ
    const MaNgonNgu = $("#ngonNguDichAdd").val();
    const TenLeHoiChild = $("#tenLeHoiDichAdd").val();
    const DiaDiemChiTiet = $("#diaChiAdd").val();
    const DonViPhoiHop = $("#donViPhoiHopAdd").val();
    const NhanVatPhungTho = $("#nhanVatPhungThoAdd").val();
    const NoiDung = ckeditorNoiDungAdd.getData();
    const QuiDinh = ckeditorQuiDinhAdd.getData();
    const DanhGia = ckeditordanhGiaAdd.getData();

    console.log({
        TenLeHoi, MaDinhDanhLeHoi, TheLoaiID, NgayBatDau, NgayKetThuc, TanSuatID, CapDoID, CapQuanLyID,
        SoLuongThamGia, DonViToChucID, SoNgayDienRa, NamVHPHTQG, DanTocID, LaTieuBieu, LaCungDinh,
        TrangThaiToChuc, TrangThai,
        MaNgonNgu, TenLeHoiChild, DiaDiemChiTiet, DonViPhoiHop, NhanVatPhungTho, NoiDung, QuiDinh, DanhGia
    })

    let dtChung = {
        TenLeHoi: TenLeHoi,
        MaDinhDanhLeHoi: MaDinhDanhLeHoi,
        TheLoaiID: TheLoaiID,
        NgayBatDau: null,
        NgayKetThuc: null,
        TanSuatID: TanSuatID,
        CapDoID: CapDoID,
        CapQuanLyID: CapQuanLyID,
        SoLuongThamGia: SoLuongThamGia,
        DonViToChucID: DonViToChucID,
        SoNgayDienRa: SoNgayDienRa,
        ThoiGianDienRa: ThoiGianDienRa,
        NamVHPHTQG: NamVHPHTQG,
        DanTocID: DanTocID,
        LaTieuBieu: LaTieuBieu,
        LaCungDinh: LaCungDinh,
        TrangThaiToChuc: TrangThaiToChuc == 1 ? true : false,
        TrangThai: TrangThai == 1 ? true : false,
        ListID: $("#DiaDiemCoSanAdd").val().map(item => item).join(','),

        //Nội dung đa ngữ
        MaNgonNgu: MaNgonNgu,
        TenLeHoiChild: TenLeHoiChild,
        DiaDiemChiTiet: DiaDiemChiTiet,
        DonViPhoiHop: DonViPhoiHop,
        NhanVatPhungTho: NhanVatPhungTho,
        NoiDung: NoiDung,
        QuiDinh: QuiDinh,
        DanhGia: DanhGia,
    }

    let listDaPhuongTien = createTempDaPhuongTiens();
    let requestDaPhuongTien = createRequestDaPhuongTiens();

    let formData = new FormData()
    formData.append('EntityLeHoi', JSON.stringify(dtChung))
    for (let i = 0; i < requestDaPhuongTien.length; i++) {
        formData.append(`EntityDaPhuongTien[${i}].DaPhuongTien`, JSON.stringify(requestDaPhuongTien[i].DaPhuongTien));
        formData.append(`EntityDaPhuongTien[${i}].DaPhuongTien_NoiDung`, JSON.stringify(requestDaPhuongTien[i].DaPhuongTien_NoiDung));  
        formData.append(`EntityDaPhuongTien[${i}].File`, requestDaPhuongTien[i].FileDinhKem[0]);  
    }

    $.ajax({
        type: 'POST',
        url: `${baseUrl}/api/VH_LeHoiApi/Add`,
        async: false,
        contentType: false,
        processData: false,
        data: formData,
        success: function (data) {
            if (data.isSuccess && data.value) {
                showNotification(1, "Thêm mới lễ hội thành công");
                console.log(data)
                setTimeout(() => {
                    window.location.href = `${baseUrl}/AdminTool/Lehoi/Lehoi`;
                }, 1500);
            } else {
                showNotification(0, "Thêm mới lễ hội không thành công");
                console.log(data.error)
            }
        },
        error: function (err) {
            console.log(err)
        }
    })
}

function dataAddDiaDiem() {
    //Thông tin địa điểm
    const DiaDiemID = null; //Chưa có trường địa điểm trong form thêm lễ hội
    const DiaDiemCapChaID = null //Chưa có trường địa điểm trong form thêm lễ hội
    const LinhVucID = $("#linhVucAdd").val();
    const XaID = $("#phuongXaAdd").val();
    const TinhID = $("#tinhThanhPhoAdd").val();
    const KinhDo = $("#kinhDoAdd").val();
    const ViDo = $("#viDoAdd").val();
    const CaoDo = $("#caoDoAdd").val();
    const BanDo = null; //Chưa có trường địa điểm trong form thêm lễ hội
    const HinhAnh = null; //Chưa có trường địa điểm trong form thêm lễ hội
    const NguoiKhuyetTat = $("#nguoiKhuyetTatAdd").is(":checked");
    const NhaVeSinh = $("#nhaVeSinhAdd").is(":checked");
    const BaiDoXe = $("#baiDoXeAdd").is(":checked");

    //Thông tin địa điểm dịch
    const MaNgonNguDiaDiem = $("#ngonNguDichDiaDiemAdd").val();
    const ThoiGianHoatDong = $("#thoiGianHoatDongDichAdd").val();
    const TenDiaDiem = $("#tenDiaDiemDichAdd").val();
    const SucChua = $("#sucChuaDichAdd").val();
    const HeToaDo = null; //Chưa có trường địa điểm trong form thêm lễ hội
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


    let diaDiem_NoiDung =[{
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
                        data.value.forEach(function(item) {
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

// Khởi tạo bảng tạm cho đa phương tiện
function initTempDaPhuongTienTable() {
    // Khởi tạo DataTable cho bảng đa phương tiện tạm
    console.log(tempDaPhuongTiens)
    const tableApi = {
        data: tempDaPhuongTiens,
        dataSrc: function(data) {
            console.log(data)
            return data.map((item, index) => ({
                ...item,
                stt: index + 1
            }));
        }
    };

    const tableDefs = [
        {
            targets: 1,
            render: function (data, type, row, meta) {
                return `<div class="group-info have-image">
                    <div class="have-image">
                        <img src="${row.duongDanHinhAnhBia || '/assets/images/vector/no-image.png'}" alt="${row.tenDaPhuongTien || ''}" onerror="this.onerror=null;this.src='/assets/images/vector/no-image.png';" />
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
                if (row.LoaiMedia == 1) {
                    return `Hình ảnh`;
                } else if (row.LoaiMedia == 2) {
                    return `Video`;
                } else if (row.LoaiMedia == 3) {
                    return `Audio`;
                } else if (row.LoaiMedia == 4) {
                    return `File khác`;
                }
                return ``;
            }
        },
        {
            targets: 4, // Cột chức năng
            render: function (data, type, row, meta) {
                const editIcon = `<i data-toggle="tooltip" title="Chỉnh sửa" class="edit-temp-DaPhuongTien text-yellow cursor-pointer me-2" data-index="${meta.row}">
                    <i class="hgi-icon hgi-edit"></i>
                </i>`;
                
                const deleteIcon = row.isExisting 
                    ? `<i data-toggle="tooltip" title="Xóa khỏi hệ thống" class="delete-existing-DaPhuongTien text-red cursor-pointer" data-index="${meta.row}" data-id="${row.DaPhuongTienID}">
                        <i class="hgi-icon hgi-delete"></i>
                    </i>`
                    : `<i data-toggle="tooltip" title="Xóa khỏi danh sách" class="delete-temp-DaPhuongTien text-red cursor-pointer" data-index="${meta.row}">
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
        { "data": "TacGia", "width": "15%", "class": "left-align" },
        { "data": "", "width": "120px", "class": "center-align group-icon-action" }
    ];

    // Destroy existing DataTable if exists
    if ($.fn.DataTable.isDataTable('#dataGridDaPhuongTien')) {
        $('#dataGridDaPhuongTien').DataTable().destroy();
    }

    $('#dataGridDaPhuongTien').DataTable({
        data: tempDaPhuongTiens.map((item, index) => ({ ...item, stt: index + 1 })),
        columns: tableCols,
        columnDefs: tableDefs,
        paging: false,
        searching: false,
        info: false,
        ordering: false,
        autoWidth: false,
        responsive: true,
        language: {
            emptyTable: "Chưa có đa phương tiện nào"
        }
    });
}

function refreshTempDaPhuongTienTable() {
    // Cập nhật dữ liệu cho DataTable
    if ($.fn.DataTable.isDataTable('#dataGridDaPhuongTien')) {
        const table = $('#dataGridDaPhuongTien').DataTable();
        const dataWithStt = tempDaPhuongTiens.map((item, index) => ({ ...item, stt: index + 1 }));
        table.clear().rows.add(dataWithStt).draw();
    } else {
        initTempDaPhuongTienTable();
    }
}

function editTempDaPhuongTien(index) {
    currentEditingDaPhuongTienIndex = index;
    const item = tempDaPhuongTiens[index];
    
    // Populate form
    $('input[name="loaiMediaDPT"][value="' + item.loaiMedia + '"]').prop('checked', true);
    if (item.loaiMedia == 1) {
        $("#fileHinhAnh").removeClass("d-none");
        $("#fileVideo, #fileAmThanh, #fileKhac").addClass("d-none");
        $("#fileDPT").attr("accept", ".jpg,.jpeg,.png,.gif,.webp,.bmp");
    } else if (item.loaiMedia == 2) {
        $("#fileVideo").removeClass("d-none");
        $("#fileHinhAnh, #fileAmThanh, #fileKhac").addClass("d-none");
        $("#fileDPT").attr("accept", ".mp4,.mov,.avi,.mkv,.webm");
    } else if (item.loaiMedia == 3) {
        $("#fileAmThanh").removeClass("d-none");
        $("#fileHinhAnh, #fileVideo, #fileKhac").addClass("d-none");
        $("#fileDPT").attr("accept", ".mp3,.wav,.ogg,.flac");
    } else if (item.loaiMedia == 4) {
        $("#fileKhac").removeClass("d-none");
        $("#fileHinhAnh, #fileVideo, #fileAmThanh").addClass("d-none");
        $("#fileDPT").attr("accept", "*/*");
    }
    $("#fileDPT").val(''); // Reset input file

    $("#tacGiaDPT").val(item.TacGia);
    $("#thuTuDPT").val(item.ThuTuHienThi);
    $('input[name="trangThaiDPT"][value="' + (item.suDung ? "1" : "0") + '"]').prop('checked', true);

    // Hiển thị ảnh bìa nếu có
    if (item.duongDanHinhAnhBia) {
        $('#showAnhDaiDienAdd').attr('src', item.duongDanHinhAnhBia);
    }

    //Nội dung đa ngữ
    $('#ngonNguDPT').val('vi').trigger('change');
    $('#tieuDeDPT').val(item.tieuDe);
    $('#moTaDPT').val(item.moTa);
    
    $('#modalAdd').modal('show');
}

function resetDaPhuongTienModal() {
    $("#hinhAnhAdd").val('');
    $('#showAnhDaiDienAdd').attr('src', '/assets/images/vector/addImage.png');
    $("#tacGiaAdd").val('');
    $("#thuTuAdd").val('');
    $('input[name="trangThaiAdd"][value="1"]').prop('checked', true);
    $('#ngonNguAdd').val('vi').trigger('change');
    $('#tenFileAdd').val('');
    $('#moTaFileAdd').val('');
}

function resetDiaDiemModal() {
    $("#linhVucAdd").val('').trigger('change');
    $("#phuongXaAdd").empty().append(`<option value="">Chọn</option>`).val('').trigger('change');
    $("#tinhThanhPhoAdd").val('').trigger('change');
    $("#kinhDoAdd").val('');
    $("#viDoAdd").val('');
    $("#caoDoAdd").val('');
    $("#nguoiKhuyetTatAdd").prop('checked', false);
    $("#nhaVeSinhAdd").prop('checked', false);
    $("#baiDoXeAdd").prop('checked', false);
    $('#ngonNguDichDiaDiemAdd').val('vi').trigger('change');
    $('#thoiGianHoatDongDichAdd').val('');
    $('#tenDiaDiemDichAdd').val('');
    $('#sucChuaDichAdd').val('');
    $('#diaChiDichAdd').val('');
    $('#moTaDichAdd').val('');
}

function saveDaPhuongTienToTemp() {
    let loaiMedia = $("input[name='loaiMediaDPT']:checked").val();
    let fileDPT = $("#fileDPT")[0].files[0] || null;
    let tacGia = $("#tacGiaDPT").val();
    let thuTu = $("#thuTuDPT").val();
    let trangThai = $("input[name='trangThaiDPT']:checked").val();
    let duongDanFile = $("#duongDanFileDPT").val();
    let previewAnhBiaSrc = $('#showAnhDaiDienAdd').attr('src');

    let maNgonNgu = $("#ngonNguDPT").val();
    let tieuDe = $("#tieuDeDPT").val().trim();
    let moTa = $("#moTaDPT").val().trim();

    const DaPhuongTienData = {
        LoaiMedia: Number(loaiMedia),
        DuongDanFile: duongDanFile,
        duongDanHinhAnhBia: previewAnhBiaSrc !== '/assets/images/vector/addImage.png' ? previewAnhBiaSrc : null,
        TacGia: tacGia,
        ThuTuHienThi: thuTu || 1,
        SuDung: trangThai == "1" ? true : false,
        maNgonNgu: maNgonNgu,
        tieuDe: tieuDe,
        moTa: moTa
    };

    const DaPhuongTien = {
        LoaiMedia: Number(loaiMedia),
        LoaiDoiTuong: "VH_LeHoi",
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

    try {
        if (currentEditingDaPhuongTienIndex >= 0) {
            // Nếu là đa phương tiện đã tồn tại và được chỉnh sửa, cập nhật ngay lập tức
            if (tempDaPhuongTiens[currentEditingDaPhuongTienIndex].isExisting) {
                updateExistingDaPhuongTien(DaPhuongTienData);
            }
            tempDaPhuongTiens[currentEditingDaPhuongTienIndex] = DaPhuongTienData;
        } else {
            tempDaPhuongTiens.push(DaPhuongTienData);
            RequestXuatBan.push({
                DaPhuongTien: DaPhuongTien,
                DaPhuongTien_NoiDung: DaPhuongTien_NoiDung,
                FileDinhKem: FileDinhKem
            });
        }

        refreshTempDaPhuongTienTable();
        $('#modalAddDaPhuongTien').modal('hide');
        showNotification(1, currentEditingDaPhuongTienIndex >= 0 ? 'Cập nhật thành công' : 'Thêm thành công');
    } catch (error) {
        // Lỗi đã được hiển thị trong updateExistingDaPhuongTien
        console.error('Lỗi khi lưu đa phương tiện:', error);
    }
}

function createTempDaPhuongTiens() {
    const newDaPhuongTiens = tempDaPhuongTiens.filter(ap => !ap.isExisting);
    const listDaPhuongTien = [];
    for (let i = 0; i < newDaPhuongTiens.length; i++) {
        console.log(newDaPhuongTiens[i])
        listDaPhuongTien.push(newDaPhuongTiens[i]);
    }
    return listDaPhuongTien;
}

function createRequestDaPhuongTiens() {
    const requestDaPhuongTiens = RequestXuatBan.filter(ap => !ap.isExisting);
    const listDaPhuongTien = [];
    for (let i = 0; i < requestDaPhuongTiens.length; i++) {
        console.log(requestDaPhuongTiens[i])
        listDaPhuongTien.push(requestDaPhuongTiens[i]);
    }
    return listDaPhuongTien;
}
