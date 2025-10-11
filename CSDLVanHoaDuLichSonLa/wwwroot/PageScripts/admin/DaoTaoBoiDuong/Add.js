const baseUrl = getRootLink();

$(document).ready(function () {
    initDatePicker();
    initLinhVuc_NoAll("#linhVuc", "Chọn lĩnh vực");
    initToChuc_NoAll(1, "#toChuc", "");
    initDiaDiem_NoAll("#diaDiem", "Chọn địa điểm");
    initDanhMucChung_NoAll(6, "#trangThai", "Chọn trạng thái");
    initNgonNgu("#ngonNguDich");
    initSelect2();

  
    $(document).on('change', '#ngonNguDich', function () {
        $('#noiDungDaoTao').val('');
        $('#donVi').val('');
    });

    $("#formDaoTao").on("submit", function (e) {
        e.preventDefault();

        const submitBtn = $(this).find('button[type="submit"]');
        if (submitBtn.prop('disabled')) {
            return;
        }

        submitBtn.prop('disabled', true);
        const originalText = submitBtn.text();
        submitBtn.data('original-text', originalText);
        submitBtn.text('Đang xử lý...');

        try {
            dataAdd();
        } catch (error) {
            showNotification(0, "Có lỗi xảy ra khi xử lý dữ liệu");
            showNotification(0, error.message);
            submitBtn.prop('disabled', false);
            submitBtn.text(originalText);
        }
    });
});

function dataAdd() {
    const submitBtn = $('#formDaoTao').find('button[type="submit"]');
    const originalText = submitBtn.data('original-text') || 'Lưu';

  
    let linhVucID = $('#linhVuc').val();
    let toChucID = $('#toChuc').val();
    let diaDiemID = $('#diaDiem').val();
    let trangThaiID = $('#trangThai').val();
    let batDau = $('#batDau').val();
    let ketThuc = $('#ketThuc').val();

    
    let ngonNguDich = $('#ngonNguDich').val();
    let noiDungDaoTao = $('#noiDungDaoTao').val();
    let donVi = $('#donVi').val();

    function resetButton() {
        submitBtn.prop('disabled', false);
        submitBtn.text(originalText);
    }

   
    if (!linhVucID?.trim()) {
        showNotification(0, "Vui lòng chọn lĩnh vực đào tạo");
        $('#linhVuc').focus();
        resetButton();
        return;
    }

    if (!diaDiemID?.trim()) {
        showNotification(0, "Vui lòng chọn địa điểm tổ chức");
        $('#diaDiem').focus();
        resetButton();
        return;
    }

    if (!trangThaiID?.trim()) {
        showNotification(0, "Vui lòng chọn trạng thái");
        $('#trangThai').focus();
        resetButton();
        return;
    }

    if (!batDau?.trim()) {
        showNotification(0, "Vui lòng nhập ngày bắt đầu");
        $('#batDau').focus();
        resetButton();
        return;
    }

    if (!ketThuc?.trim()) {
        showNotification(0, "Vui lòng nhập ngày kết thúc");
        $('#ketThuc').focus();
        resetButton();
        return;
    }

   
    if (ketThuc?.trim() && batDau?.trim()) {
        const startDate = new Date(batDau);
        const endDate = new Date(ketThuc);

        if (isNaN(endDate.getTime())) {
            showNotification(0, "Ngày kết thúc không hợp lệ");
            $('#ketThuc').focus();
            resetButton();
            return;
        }

        if (endDate <= startDate) {
            showNotification(0, "Ngày kết thúc phải sau ngày bắt đầu");
            $('#ketThuc').focus();
            resetButton();
            return;
        }
    }

    if (!noiDungDaoTao?.trim()) {
        showNotification(0, "Vui lòng nhập nội dung đào tạo");
        $('#noiDungDaoTao').focus();
        resetButton();
        return;
    }


    let daoTaoBoiDuong = {
        "linhVucID": parseInt(linhVucID),
        "toChucID": toChucID ? toChucID : null,
        "diaDiemID": diaDiemID,
        "trangThaiID": parseInt(trangThaiID),
        "batDau": new Date(batDau).toISOString(), 
        "ketThuc": new Date(ketThuc).toISOString() 
    };

   
    let noiDungBanDich = [{
        "MaNgonNgu": ngonNguDich || "vi",
        "NoiDungDaoTao": noiDungDaoTao?.trim(),
        "DonVi": donVi?.trim() || ""
    }];

    let formData = new FormData();
    formData.append('DaoTaoBoiDuong', JSON.stringify(daoTaoBoiDuong));
    formData.append('DaoTaoBoiDuong_NoiDung', JSON.stringify(noiDungBanDich));

    $.ajax({
        type: 'POST',
        url: `${baseUrl}/api/DaoTaoBoiDuongApi/ThemMoi`,
        async: false,
        contentType: false,
        processData: false,
        data: formData,
        success: function (data) {
            if (data.isSuccess && data.value) {
                showNotification(1, "Thêm mới đào tạo bồi dưỡng thành công");
                setTimeout(() => {
                    window.location.href = `${baseUrl}/AdminTool/DaoTaoBoiDuong`;
                }, 1500);
            } else {
                showNotification(0, data.message || "Thêm mới đào tạo bồi dưỡng không thành công");
                resetButton();
            }
        },
        error: function (xhr, status, error) {
            showNotification(0, "Có lỗi xảy ra khi kết nối đến server");
            resetButton();
        }
    });
}

function initLinhVuc_NoAll(elementId = "#linhVuc", placeholder = "Chọn lĩnh vực") {
    getDataWithApi('GET', '/api/LinhVucApi/Gets').then(response => {
        if (response && response.isSuccess && response.value) {
            $(elementId).empty().append(`<option value="">${placeholder}</option>`);
            response.value.forEach(item => {
                $(elementId).append(`<option value="${item.linhVucID}">${item.ten}</option>`);
            });
        } else {
            showNotification(0, "Không thể load danh sách lĩnh vực");
        }
    }).catch(error => {
        showNotification(0, "Lỗi kết nối khi tải danh sách lĩnh vực");
    });
}

function initDiaDiem_NoAll(elementId = "#diaDiem", placeholder = "Chọn địa điểm") {
    const requestData = JSON.stringify({
        MaNgonNgu: "vi",
        TrangThai: true
    });

    getDataWithApi('POST', '/api/DiaDiemApi/DanhSach', requestData).then(response => {
        if (response && response.isSuccess && response.value) {
            $(elementId).empty().append(`<option value="">${placeholder}</option>`);

            response.value.forEach(item => {
                const displayText = item.tenDiaDiem || item.noiDung || 'Không rõ tên';
                const addressText = item.diaChi ? ` - ${item.diaChi}` : '';
                const fullText = displayText + addressText;

                $(elementId).append(`<option value="${item.diaDiemID || item.id}">${fullText}</option>`);
            });
        } else {
            showNotification(0, "Không thể load danh sách địa điểm");
        }
    }).catch(error => {
        showNotification(0, "Lỗi kết nối khi tải danh sách địa điểm");
    });
}
