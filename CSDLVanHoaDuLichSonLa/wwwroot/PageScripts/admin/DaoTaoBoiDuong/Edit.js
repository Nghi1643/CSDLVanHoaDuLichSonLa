const baseUrl = getRootLink();
let translations = [];

$(document).ready(function () {
    initDatePicker();
    initLinhVuc_NoAll("#linhVuc", "Chọn lĩnh vực");
    initToChuc_NoAll(1, "#toChuc", "");
    initDiaDiem_NoAll("#diaDiem", "Chọn địa điểm");
    initDanhMucChung_NoAll(6, "#trangThai", "Chọn trạng thái");
    initNgonNgu("#ngonNguDich");
    initSelect2();

    if (daoTaoID) {
        setTimeout(function () {
            loadDaoTaoBoiDuongData(daoTaoID);
        }, 500);
    } 

    // Khi thay đổi ngôn ngữ dịch, load bản dịch tương ứng
    $(document).on('change', '#ngonNguDich', function () {
        let maNgonNgu = $(this).val();
        const daoTaoIDCurrent = $('#daoTaoID').val();

        if (daoTaoIDCurrent && maNgonNgu) {
            console.log('Chuyển ngôn ngữ dịch cho đào tạo hiện tại:', maNgonNgu);
            // Load bản dịch cho ngôn ngữ được chọn
            loadDaoTaoTranslation(daoTaoIDCurrent, maNgonNgu);
        } else {
            if (!daoTaoIDCurrent) {
                $('#noiDungDaoTao').val('');
                $('#donVi').val('');
            }
        }
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
            dataEdit();
        } catch (error) {
            showNotification(0, "Có lỗi xảy ra khi xử lý dữ liệu");
            showNotification(0, error.message);
            submitBtn.prop('disabled', false);
            submitBtn.text(originalText);
        }
    });
});

function dataEdit() {
    const submitBtn = $('#formDaoTao').find('button[type="submit"]');
    const originalText = submitBtn.data('original-text') || (isEdit ? 'Cập nhật' : 'Lưu');

    // Thông tin cơ bản
    let id = $('#daoTaoID').val();
    let linhVucID = $('#linhVuc').val();
    let toChucID = $('#toChuc').val();
    let diaDiemID = $('#diaDiem').val();
    let trangThaiID = $('#trangThai').val();
    let batDau = $('#batDau').val();
    let ketThuc = $('#ketThuc').val();

    // Thông tin đa ngữ
    let ngonNguDich = $('#ngonNguDich').val();
    let noiDungDaoTao = $('#noiDungDaoTao').val();
    let donVi = $('#donVi').val();

    function resetButton() {
        submitBtn.prop('disabled', false);
        submitBtn.text(originalText);
    }

    // Validation 
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

    // Kiểm tra ngày kết thúc phải sau ngày bắt đầu
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

    // Tạo đối tượng đào tạo bồi dưỡng
    let daoTaoBoiDuong = {
        "daoTaoID": id,
        "linhVucID": parseInt(linhVucID),
        "toChucID": toChucID && toChucID !== "" ? toChucID : null,
        "diaDiemID": diaDiemID,
        "trangThaiID": parseInt(trangThaiID),
        "batDau": new Date(batDau).toISOString(),
        "ketThuc": new Date(ketThuc).toISOString()
    };

    // Tạo đối tượng nội dung đa ngữ
    let noiDungBanDich = [{
        "maNgonNgu": ngonNguDich || "vi",
        "noiDungDaoTao": noiDungDaoTao?.trim(),
        "donVi": donVi?.trim() || ""
    }];

    let formData = new FormData();
    formData.append('DaoTaoBoiDuong', JSON.stringify(daoTaoBoiDuong));
    formData.append('DaoTaoBoiDuong_NoiDung', JSON.stringify(noiDungBanDich));

    $.ajax({
        type: 'PUT',
        url: `${baseUrl}/api/DaoTaoBoiDuongApi/ChinhSua/${id}`,
        async: false,
        contentType: false,
        processData: false,
        data: formData,
        success: function (data) {
            if (data.isSuccess && data.value) {
                showNotification(1, "Chỉnh sửa đào tạo bồi dưỡng thành công");
                setTimeout(() => {
                    window.location.href = `${baseUrl}/AdminTool/DaoTaoBoiDuong`;
                }, 1500);
            } else {
                showNotification(0, data.message || data.error || "Chỉnh sửa đào tạo bồi dưỡng không thành công");
                console.log("Server response:", data);
                resetButton();
            }
        },
        error: function (xhr, status, error) {
            console.error('Lỗi AJAX:', error);
            console.error('Chi tiết lỗi:', xhr.responseText);
            showNotification(0, "Có lỗi xảy ra khi kết nối đến server");
            resetButton();
        }
    });
}

function loadDaoTaoBoiDuongData(id) {
    getDataWithApi('GET', `/api/DaoTaoBoiDuongApi/ChiTiet/${id}`).then(data => {
      
        if (data && data.isSuccess && data.value) {
            const item = data.value;

            $('#pageTitle').text(`Chỉnh sửa ${item.noiDungDaoTao || 'đào tạo bồi dưỡng'}`);
            $('#daoTaoID').val(item.daoTaoID);
            $('#linhVuc').val(item.linhVucID).trigger('change');
            $('#toChuc').val(item.toChucID).trigger('change');
            $('#diaDiem').val(item.diaDiemID).trigger('change');
            $('#trangThai').val(item.trangThaiID).trigger('change');

            if (item.batDau) {
                const startDate = new Date(item.batDau);
                const isoString = startDate.toISOString();
                $('#batDau').val(isoString.slice(0, 16)); 
            }

            if (item.ketThuc) {
                const endDate = new Date(item.ketThuc);
                const isoString = endDate.toISOString();
                $('#ketThuc').val(isoString.slice(0, 16)); 
            }

            
            if (item.noiDungDaoTao || item.donVi) {
                $('#noiDungDaoTao').val(item.noiDungDaoTao || '');
                $('#donVi').val(item.donVi || '');
                $('#ngonNguDich').val('vi');
            } else {
                
                $('#ngonNguDich').val('vi');
                loadDaoTaoTranslation(item.daoTaoID, 'vi');
            }
        } else {
            showNotification(0, 'Không tìm thấy đào tạo bồi dưỡng');
        }
    }).catch(error => {
        console.error('Lỗi khi load dữ liệu:', error);
        showNotification(0, 'Có lỗi xảy ra khi tải dữ liệu');
    });
}

function loadDaoTaoTranslation(daoTaoID, maNgonNgu) {
    console.log('Loading translation for:', daoTaoID, maNgonNgu); // Thêm log

    getDataWithApi('GET', `/api/DaoTaoBoiDuongApi/BanDich/${daoTaoID}/${maNgonNgu}`).then(data => {
        console.log('Translation data:', data); // Thêm log

        if (data && data.isSuccess && data.value && data.value.length > 0) {
            translations = data.value;
            const translation = translations.find(t => t.maNgonNgu === maNgonNgu) || translations[0];

            $('#noiDungDaoTao').val(translation.noiDungDaoTao || '');
            $('#donVi').val(translation.donVi || '');
        } else {
            console.log('No translation data found'); // Thêm log
            $('#noiDungDaoTao').val('');
            $('#donVi').val('');
        }
    }).catch(error => {
        console.error('Error loading translation:', error); // Thêm log
        $('#noiDungDaoTao').val('');
        $('#donVi').val('');
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
