const baseUrl = getRootLink();
let translations = [];

$(document).ready(function () {
    initDatePicker();
    initLinhVuc();// dùng api gets lĩnh vực
    // hoặc initDanhMucChung_NoAll(9, "#linhVuc", "")// dùng danh mục chung lĩnh vực
    initToChuc_NoAll(1, "#toChuc", "")//id test
    initDanhMucChung_NoAll(3, "#doiTuong", "") // test
    initDanhMucChung_NoAll(4, "#hinhThuc", "") // test
    initDanhMucChung_NoAll(5, "#truyenThong", "") // test
    initDanhMucChung_NoAll(6, "#trangThai", "") // test
    initNgonNgu("#ngonNguDich")
    initSelect2();

    if (isEdit && xucTienID) {
        setTimeout(function () {
            loadThongTinXucTienData(xucTienID)
        }, 500);
    }

    // Khi thay đổi ngôn ngữ dịch, load bản dịch tương ứng
    $(document).on('change', '#ngonNguDich', function () {
        let maNgonNgu = $(this).val();
        const xucTienIDCurrent = $('#xucTienID').val();

        if (xucTienIDCurrent && maNgonNgu) {
            console.log('Chuyển ngôn ngữ dịch cho xúc tiến hiện tại:', maNgonNgu);
            // Load bản dịch cho ngôn ngữ được chọn
            loadXucTienTranslation(xucTienIDCurrent, maNgonNgu);
        } else {
        
            if (!xucTienIDCurrent) {
                $('#tieuDe').val('');
                $('#moTa').val('');
            }
        }
    });

    $("#formXucTien").on("submit", function (e) {
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
    const submitBtn = $('#formXucTien').find('button[type="submit"]');
    const originalText = submitBtn.data('original-text') || (isEdit ? 'Cập nhật' : 'Lưu');

    // Thông tin cơ bản
    let id = $('#xucTienID').val();
    let linhVucID = $('#linhVuc').val();
    let toChucID = $('#toChuc').val();
    let doiTuongID = $('#doiTuong').val();
    let hinhThucID = $('#hinhThuc').val();
    let truyenThongID = $('#truyenThong').val();
    let trangThaiID = $('#trangThai').val();
    let ngayBatDau = $('#ngayBatDau').val();
    let ngayKetThuc = $('#ngayKetThuc').val();

    // Thông tin đa ngữ
    let ngonNguDich = $('#ngonNguDich').val();
    let tieuDe = $('#tieuDe').val();
    let moTa = $('#moTa').val();

    function resetButton() {
        submitBtn.prop('disabled', false);
        submitBtn.text(originalText);
    }

    // Validation 
    if (!linhVucID?.trim()) {
        showNotification(0, "Vui lòng chọn lĩnh vực");
        $('#linhVuc').focus();
        resetButton();
        return;
    }

    if (!doiTuongID) {
        showNotification(0, "Vui lòng chọn đối tượng");
        $('#doiTuong').focus();
        resetButton();
        return;
    }

    if (!hinhThucID) {
        showNotification(0, "Vui lòng chọn hình thức xúc tiến");
        $('#hinhThuc').focus();
        resetButton();
        return;
    }

    if (!truyenThongID?.trim()) {
        showNotification(0, "Vui lòng chọn phương thức truyền thông");
        $('#truyenThong').focus();
        resetButton();
        return;
    }

    if (!trangThaiID?.trim()) {
        showNotification(0, "Vui lòng chọn trạng thái");
        $('#trangThai').focus();
        resetButton();
        return;
    }

    if (!ngayBatDau?.trim()) {
        showNotification(0, "Vui lòng nhập ngày bắt đầu");
        $('#ngayBatDau').focus();
        resetButton();
        return;
    }

    if (ngayKetThuc?.trim()) {
        const endDate = new Date(ngayKetThuc);

        if (isNaN(endDate.getTime())) {
            showNotification(0, "Ngày kết thúc không hợp lệ");
            $('#ngayKetThuc').focus();
            resetButton();
            return;
        }

        const startDate = new Date(ngayBatDau);
        if (endDate <= startDate) {
            showNotification(0, "Ngày kết thúc phải sau ngày bắt đầu");
            $('#ngayKetThuc').focus();
            resetButton();
            return;
        }
    }

    if (!tieuDe?.trim()) {
        showNotification(0, "Vui lòng nhập tiêu đề");
        $('#tieuDe').focus();
        resetButton();
        return;
    }

 
    let thongTinXucTien = {
        "xucTienID": id,
        "linhVucID": parseInt(linhVucID),
        "toChucID": toChucID ? toChucID : null,
        "doiTuongID": parseInt(doiTuongID),
        "hinhThucID": parseInt(hinhThucID),
        "truyenThongID": parseInt(truyenThongID),
        "trangThaiID": parseInt(trangThaiID),
        "ngayBatDau": ngayBatDau,
        "ngayKetThuc": ngayKetThuc || null,
    };


    let noiDungBanDich = [{
        "MaNgonNgu": ngonNguDich || "vi",
        "TieuDe": tieuDe?.trim(),
        "MoTa": moTa?.trim() || null,
    }];

    let formData = new FormData();
    formData.append('ThongTinXucTien', JSON.stringify(thongTinXucTien));
    formData.append('ThongTinXucTien_NoiDung', JSON.stringify(noiDungBanDich));

    $.ajax({
        type: 'PUT',
        url: `${baseUrl}/api/ThongTinXucTienApi/ChinhSua/${id}`,
        async: false,
        contentType: false,
        processData: false,
        data: formData,
        success: function (data) {
            if (data.isSuccess && data.value) {
                showNotification(1, "Chỉnh sửa thông tin xúc tiến thành công");
                setTimeout(() => {
                    window.location.href = `${baseUrl}/AdminTool/ThongTinXucTien`;
                }, 1500);
            } else {
                showNotification(0, data.message || "Chỉnh sửa thông tin xúc tiến không thành công");
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

function loadThongTinXucTienData(id) {
    console.log('Loading thông tin xúc tiến data:', id);

    getDataWithApi('GET', `/api/ThongTinXucTienApi/ChiTiet/${id}`).then(data => {
        console.log('Dữ liệu thông tin xúc tiến:', data);
        if (data && data.isSuccess && data.value) {
            const item = data.value;


            $('#pageTitle').text(`Chỉnh sửa ${item.tieuDe || 'thông tin xúc tiến'}`);
            $('#xucTienID').val(item.xucTienID);
            $('#linhVuc').val(item.linhVucID).trigger('change');
            $('#toChuc').val(item.toChucID).trigger('change');
            $('#doiTuong').val(item.doiTuongID).trigger('change');
            $('#hinhThuc').val(item.hinhThucID).trigger('change');
            $('#truyenThong').val(item.truyenThongID).trigger('change');
            $('#trangThai').val(item.trangThaiID).trigger('change');

            if (item.ngayBatDau) {
                const startDate = new Date(item.ngayBatDau);
                $('#ngayBatDau').val(startDate.toISOString().split('T')[0]);
            }

            if (item.ngayKetThuc) {
                const endDate = new Date(item.ngayKetThuc);
                $('#ngayKetThuc').val(endDate.toISOString().split('T')[0]);
            }

            $('#ngonNguDich').val('vi');

            loadXucTienTranslation(item.xucTienID, 'vi');
        } else {
            showNotification(0, 'Không tìm thấy thông tin xúc tiến');
        }
    }).catch(error => {
        console.error('Lỗi khi load dữ liệu:', error);
        showNotification(0, 'Có lỗi xảy ra khi tải dữ liệu');
    });
}

function loadXucTienTranslation(xucTienID, maNgonNgu) {
    getDataWithApi('GET', `/api/ThongTinXucTienApi/BanDich/${xucTienID}/${maNgonNgu}`).then(data => {
        if (data && data.isSuccess && data.value && data.value.length > 0) {
            translations = data.value;

            const translation = translations.find(t => t.maNgonNgu === maNgonNgu) || translations[0];

            $('#tieuDe').val(translation.tieuDe || '');
            $('#moTa').val(translation.moTa || '');
        } else {
            $('#tieuDe').val('');
            $('#moTa').val('');
        }
    }).catch(error => {
        $('#tieuDe').val('');
        $('#moTa').val('');
    });
}

function initLinhVuc() {
    $.ajax({
        type: 'GET',
        url: `${baseUrl}/api/LinhVucApi/Gets`,
        contentType: 'application/json',
        success: function (response) {
            if (response && response.isSuccess && response.value) {
                $("#linhVuc").empty().append(`<option value="">Chọn lĩnh vực</option>`);

                response.value.forEach(item => {
                    $("#linhVuc").append(`<option value="${item.linhVucID}">${item.ten}</option>`);
                });
            } else {
                showNotification(0, "Không thể load danh sách lĩnh vực");
            }
        },
        error: function (xhr, status, error) {
            showNotification(0, "Lỗi kết nối khi tải danh sách lĩnh vực");
        }
    });
}