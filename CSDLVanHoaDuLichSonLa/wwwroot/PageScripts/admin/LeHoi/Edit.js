const baseUrl = getRootLink();
let translations = [];
let translationsDaPhuongTien = [];
let translationsDiaDiem = [];
console.log(LeHoiId)
const ckeditorQuiDinhEdit = CKEDITOR.replace('quyDinhEdit');
const ckeditorNoiDungEdit = CKEDITOR.replace('gioiThieuLeHoiEdit');
const ckeditordanhGiaEdit = CKEDITOR.replace('danhGiaEdit');

$(document).ready(function () {

    //#region Xử lý các function chung
    initDatePicker();
    initDanhMucChung_NoAll(37, "#theLoaiEdit", "Thể loại lễ hội")
    initDanhMucChung_NoAll(38, "#capDoEdit", "")
    initDanhMucChung_NoAll(39, "#capQuanLyEdit", "")
    initDanhMucChung_NoAll(6, "#tanSuatEdit", "")
    initToChuc_NoAll(122, "#donViToChucEdit", "")
    initDanhMucChung_NoAll(1, "#danTocEdit", "")
    initDanhMucChung_NoAll(9, "#linhVucEdit", "")
    initNgonNgu("#ngonNguDichEdit")
    initNgonNgu("#ngonNguDichDiaDiemEdit")
    initNgonNgu("#ngonNguDPT")
    initTinhThanhPho()
    initDiaDiem()
    initSelect2();
    previewPicture();
    switchMediaData();
    $("#ngonNguDichDiaDiemEdit").select2({
        width: $(this).data('width') ? $(this).data('width') : $(this).hasClass('w-100') ? '100%' : 'style',
        language: "vi",
        placeholder: $(this).attr('placeholder') || $(this).data('placeholder') || 'Chọn...',
        dropdownParent: $('.content-wrapper').length > 0 ? $('.content-wrapper') : $('body')
    });

    $('#tinhThanhPhoEdit').on('change', function () {
        let tinhID = $(this).val();
        initPhuongXa(tinhID)
    })
    //#endregion Xử lý các function chung

    //#region Xử lý thông tin lễ hội
    setTimeout(() => {
        loadData(LeHoiId)
    }, 100);

    // Khi thay đổi ngôn ngữ dịch, reset các input trong phần đa ngữ
    $(document).on('change', '#ngonNguDichEdit', function () {
        let maNgonNgu = $(this).val();
        
        if (LeHoiId && maNgonNgu) {
            console.log('Chuyển ngôn ngữ dịch cho lễ hội hiện tại:', maNgonNgu);
            // Load bản dịch cho ngôn ngữ được chọn
            loadTranslation(LeHoiId, maNgonNgu);
        } else {
            // Nếu chưa có cơ quan (tạo mới) hoặc chưa chọn ngôn ngữ, clear các trường dịch
            if (!LeHoiId || !maNgonNgu) {
                $("#tenLeHoiDichEdit").val('');
                $("#diaChiEdit").val('');
                $("#donViPhoiHopEdit").val('');
                $("#nhanVatPhungThoEdit").val('');
            }
        }
    });

    $("#btnSaveEdit").on("click", function () {
        dataEdit()
    })
    //#endregion Xử lý thông tin lễ hội


    //#region Xử lý tọa độ, vị trí bản đồ
    $("#modalEditDiaDiem").on('hidden.bs.modal', function () {
        translationsDiaDiem = []
        resetModal(this)
    })

    $(document).on('change', '#DiaDiemCoSanEdit', function () {
        let listDiaDiem = $(this).val();
        initTableDiaDiem(listDiaDiem.map(item => item).join(','));
    });

    $("#btnEditDiaDiem").on("click", function () {
        dataEditDiaDiem()
    })
    //#endregion Xử lý tọa độ, vị trí bản đồ

    
    //#region Xử lý Đa phương tiện
    

    $("#modalEditDaPhuongTien").on('hidden.bs.modal', function () {
        translationsDaPhuongTien = []
        resetModal(this)
    })

    $('#btnEditDaPhuongTien').on('click', function () {
        $('#modalEditDaPhuongTien').modal('show');
        resetModal(this)
    });

    $("#formEditDaPhuongTien").on("submit", function (e) {
        e.preventDefault();
        dataEditDaPhuongTien();
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

        $("#modalEditDaPhuongTien").modal("show");
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
//#endregion Function Xử lý chung

//#region Function Xử lý thông tin lễ hội
function loadData(LeHoiID) {
    getDataWithApi('GET', `/api/VH_LeHoiApi/Get?MaNgonNgu=vi&LeHoiID=${LeHoiID}`).then(data => {
        console.log(data)
        if (data && data.isSuccess && data.value) {
            const lh = data.value;
            $("#tenLeHoiEdit").val(lh.tenLeHoi);
            $("#maDinhDanhEdit").val(lh.maDinhDanhLeHoi);
            $("#theLoaiEdit").val(lh.theLoaiID).trigger('change');
            $("#ngayBatDauEdit").val(formatDateWithoutTime(lh.ngayBatDau));
            $("#ngayKetThucEdit").val(formatDateWithoutTime(lh.ngayKetThuc));
            $("#tanSuatEdit").val(lh.tanSuatID).trigger('change');
            $("#capDoEdit").val(lh.capDoID).trigger('change');
            $("#capQuanLyEdit").val(lh.capQuanLyID).trigger('change');
            $("#soLuongThamGiaEdit").val(lh.soLuongThamGia);
            $("#thoiGianDienRaEdit").val(lh.thoiGianDienRa);
            $("#donViToChucEdit").val(lh.donViToChucID).trigger('change');
            $("#soNgayDienRaEdit").val(lh.soNgayDienRa);
            $("#namVHPHTQGEdit").val(lh.namVHPHTQG);
            $("#danTocEdit").val(lh.danTocID).trigger('change');
            $("#laTieuBieuEdit").prop('checked', lh.laTieuBieu);
            $("#laCungDinhEdit").prop('checked', lh.laCungDinh);
            $("input[name='trangThaiToChucEdit'][value='" + (lh.trangThaiToChuc ? 1 : 0) + "']").prop('checked', true);
            $("input[name='trangThaiEdit'][value='" + (lh.trangThai ? 1 : 0) + "']").prop('checked', true);
            if (lh.listIDDiaDiem) {
                console.log(lh.listIDDiaDiem, "Danh sách địa điểm đã chọn")
                let listDiaDiem = lh.listIDDiaDiem.split(',').map(x => x.trim().toLowerCase());
                $("#DiaDiemCoSanEdit").val(listDiaDiem.map(item => item)).trigger('change');
                initTableDiaDiem(listDiaDiem.map(item => item).join(','));
            }
            //Nội dung đa ngữ
            $("#ngonNguDichEdit").val(lh.maNgonNgu).trigger('change');
            $("#tenLeHoiDichEdit").val(lh.tenLeHoiChild);
            $("#diaChiEdit").val(lh.diaDiemChiTiet);
            $("#donViPhoiHopEdit").val(lh.donViPhoiHop);
            $("#nhanVatPhungThoEdit").val(lh.nhanVatPhungTho);
            if (typeof ckeditorNoiDungEdit !== 'undefined') ckeditorNoiDungEdit.setData(lh.noiDung || '');
            if (typeof ckeditorQuiDinhEdit !== 'undefined') ckeditorQuiDinhEdit.setData(lh.quiDinh || '');
            if (typeof ckeditordanhGiaEdit !== 'undefined') ckeditordanhGiaEdit.setData(lh.danhGia || '');
            initDaPhuongTienTable(LeHoiID);
        }
    })

}

function dataEdit() { 
    //Thông tin chung
    const id = LeHoiId;
    const TenLeHoi = $("#tenLeHoiEdit").val() || null;
    const MaDinhDanhLeHoi = $("#maDinhDanhEdit").val() || null;
    const TheLoaiID = $("#theLoaiEdit").val() || null;
    const NgayBatDau = formatDateToSaveAsString($("#ngayBatDauEdit").val()) || null;
    const NgayKetThuc = formatDateToSaveAsString($("#ngayKetThucEdit").val()) || null;
    const TanSuatID = $("#tanSuatEdit").val() || null;
    const CapDoID = $("#capDoEdit").val() || null;
    const CapQuanLyID = $("#capQuanLyEdit").val() || null;
    const SoLuongThamGia = $("#soLuongThamGiaEdit").val() || null;
    const ThoiGianDienRa = $("#thoiGianDienRaEdit").val() || null;
    const DonViToChucID = $("#donViToChucEdit").val() || null;
    const SoNgayDienRa = $("#soNgayDienRaEdit").val() || null;
    const NamVHPHTQG = $("#namVHPHTQGEdit").val() || null;
    const DanTocID = $("#danTocEdit").val() || null;
    const LaTieuBieu = $("#laTieuBieuEdit").is(":checked");
    const LaCungDinh = $("#laCungDinhEdit").is(":checked");
    const TrangThaiToChuc = $("input[name='trangThaiToChucEdit']:checked").val();
    const TrangThai = $("input[name='trangThaiEdit']:checked").val();

    //Nội dung đa ngữ
    const MaNgonNgu = $("#ngonNguDichEdit").val();
    const TenLeHoiChild = $("#tenLeHoiDichEdit").val() || null;
    const DiaDiemChiTiet = $("#diaChiEdit").val() || null;
    const DonViPhoiHop = $("#donViPhoiHopEdit").val() || null;
    const NhanVatPhungTho = $("#nhanVatPhungThoEdit").val() || null;
    const NoiDung = ckeditorNoiDungEdit.getData();
    const QuiDinh = ckeditorQuiDinhEdit.getData();
    const DanhGia = ckeditordanhGiaEdit.getData();

    console.log({
        TenLeHoi, MaDinhDanhLeHoi, TheLoaiID, NgayBatDau, NgayKetThuc, TanSuatID, CapDoID, CapQuanLyID,
        SoLuongThamGia, DonViToChucID, SoNgayDienRa, NamVHPHTQG, DanTocID, LaTieuBieu, LaCungDinh,
        TrangThaiToChuc, TrangThai,
        MaNgonNgu, TenLeHoiChild, DiaDiemChiTiet, DonViPhoiHop, NhanVatPhungTho, NoiDung, QuiDinh, DanhGia
    })

    let dtChung = {
        LeHoiID: id,
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
        ListID: $("#DiaDiemCoSanEdit").val().map(item => item).join(','),

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

    $.ajax({
        type: 'PUT',
        url: `${baseUrl}/api/VH_LeHoiApi/Update`,
        //async: false,
        contentType: "application/json, charset=utf-8",
        //processData: false,
        data: JSON.stringify(dtChung),
        success: function (data) {
            if (data.isSuccess && data.value) {
                showNotification(1, "Chỉnh sửa lễ hội thành công");
                setTimeout(() => {
                    window.location.href = `${baseUrl}/AdminTool/LeHoi/LeHoi`;
                }, 1500);
            } else {
                showNotification(0, "Chỉnh sửa lễ hội không thành công");
                console.log(data.error)
            }
        },
        error: function (err) {
            console.log(err)
        }
    })
}

function loadTranslation(toChucID, maNgonNgu) {
    getDataWithApi('GET', `/api/VH_LeHoiApi/Get?LeHoiID=${toChucID}&MaNgonNgu=${maNgonNgu}`).then(data => {
        console.log(data, "Load bản dịch lễ hội")
        if (data && data.isSuccess && data.value) {
            translations = data.value;
            // Populate các trường dịch với dữ liệu đa ngữ
            $("#tenSuKienDichEdit").val(translations.tenSuKien);
            $("#ketQuaEdit").val(translations.ketQua);
            $("#moTaEdit").val(translations.moTa);
        } else {
            // Nếu không có bản dịch, clear các trường dịch
            $("#tenSuKienDichEdit").val('');
            $("#ketQuaEdit").val('');
            $("#moTaEdit").val('');
        }
    })
}
//#endregion Function Xử lý thông tin lễ hội

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
            $("#DiaDiemCoSanEdit").empty().append(`<option value="">Chọn</option>`);
            data.value.forEach(item => {
                const displayText = item.tenDiaDiem || item.noiDung || 'Không rõ tên';
                $("#DiaDiemCoSanEdit").append(`<option value="${item.diaDiemID || item.id}">${displayText}</option>`);
            });
        }
    })
}

function dataEditDiaDiem() {
    //Thông tin địa điểm
    const DiaDiemID = null; //Chưa có trường địa điểm trong form thêm lễ hội
    const DiaDiemCapChaID = null //Chưa có trường địa điểm trong form thêm lễ hội
    const LinhVucID = $("#linhVucEdit").val();
    const XaID = $("#phuongXaEdit").val();
    const TinhID = $("#tinhThanhPhoEdit").val();
    const KinhDo = $("#kinhDoEdit").val();
    const ViDo = $("#viDoEdit").val();
    const CaoDo = $("#caoDoEdit").val();
    const BanDo = null; //Chưa có trường địa điểm trong form thêm lễ hội
    const HinhAnh = null; //Chưa có trường địa điểm trong form thêm lễ hội
    const NguoiKhuyetTat = $("#nguoiKhuyetTatEdit").is(":checked");
    const NhaVeSinh = $("#nhaVeSinhEdit").is(":checked");
    const BaiDoXe = $("#baiDoXeEdit").is(":checked");

    //Thông tin địa điểm dịch
    const MaNgonNguDiaDiem = $("#ngonNguDichDiaDiemEdit").val();
    const ThoiGianHoatDong = $("#thoiGianHoatDongDichEdit").val();
    const TenDiaDiem = $("#tenDiaDiemDichEdit").val();
    const SucChua = $("#sucChuaDichEdit").val();
    const HeToaDo = null; //Chưa có trường địa điểm trong form thêm lễ hội
    const DiaChiDiaDiem = $("#diaChiDichEdit").val();
    const MoTaDiaDiem = $("#moTaDichEdit").val();

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
                var prevDiaDiem = $('#DiaDiemCoSanEdit').val();
                console.log("Giá trị đã chọn trước đó:", prevDiaDiem);
                let dt = JSON.stringify({
                    "maNgonNgu": "vi",
                    "trangThai": true
                })
                console.log(dt)
                getDataWithApi('POST', '/api/DiaDiemApi/DanhSach', dt).then(data => {
                    console.log(data)
                    if (data && data.isSuccess && data.value) {
                        var $select = $('#DiaDiemCoSanEdit');
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
                $('#modalEditDiaDiem').modal('hide');
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
                    <div class="info-sub">${row.tacGia || ''}</div></div>
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

function dataEditDaPhuongTien() {
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
        doiTuongSoHuuID: LeHoiId,
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
                    $('#modalEditDaPhuongTien').modal('hide');
                    initDaPhuongTienTable(LeHoiId);
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
                    $('#modalEditDaPhuongTien').modal('hide');
                    initDaPhuongTienTable(LeHoiId);
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
        $("#showhinhAnhEdit").attr("src", data.duongDanFile);
    } else {
        $("#showFileEdit").text("");
    }
    $("#tacGiaDPT").val(data.tacGia);
    $("#thuTuDPT").val(data.thuTuHienThi);
    $(`input[name='trangThaiEdit'][value='${data.suDung ? 1 : 0}']`).prop('checked', true);

    // Radio trạng thái
    $(`input[name='trangThaiEdit'][value='${data.suDung ? 1 : 0}']`).prop('checked', true);

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

function deleteDaPhuongTien() {
    let id = $('#idDelete').val();
    $.ajax({
        type: 'DELETE',
        url: `${baseUrl}/api/DaPhuongTienApi/Xoa/${id}`,
        async: false,
        contentType: "application/json, charset=utf-8",
        success: function (data) {
            if (data.isSuccess && data.value) {
                showNotification(1, "Xoá đa phương tiện thành công");
                $('#modalDelete').modal('hide');
                $('#dataGridDaPhuongTien').DataTable().ajax.reload();
            } else {
                showNotification(0, "Xoá đa phương tiện không thành công");
                console.log(data.error)
            }
        },
        error: function (err) {
            console.log(err)
        }
    })
}
//#endregion Function Xử lý đa phương tiện