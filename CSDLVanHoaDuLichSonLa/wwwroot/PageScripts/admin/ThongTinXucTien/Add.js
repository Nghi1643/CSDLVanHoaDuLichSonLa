const baseUrl = getRootLink();

$(document).ready(function () {
    initDatePicker();
    initLinhVuc();// dùng api gets lĩnh vực
    // hoặc initDanhMucChung_NoAll(9, "#linhVuc", "")// dùng danh mục chung lĩnh vực
    initToChuc_NoAll(1, "#toChuc","")//id test
    initDanhMucChung_NoAll(3, "#doiTuong", "") // test
    initDanhMucChung_NoAll(4, "#hinhThuc", "") // test
    initDanhMucChung_NoAll(5, "#truyenThong", "") // test
    initDanhMucChung_NoAll(6, "#trangThai", "") // test
    initNgonNgu("#ngonNguDich")
    initSelect2();

    // Khi thay đổi ngôn ngữ dịch, reset các input trong phần đa ngữ
    $(document).on('change', '#ngonNguDich', function () {
        $('#tieuDe').val('');
        $('#moTa').val('');
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
            // Validate trước khi submit
            //if (validateRating()) {
                dataAdd();
            //} else {
            //    submitBtn.prop('disabled', false);
            //    submitBtn.text(originalText);
            //}
        } catch (error) {
            //console.error('Lỗi khi xử lý form:', error);
            showNotification(0, "Có lỗi xảy ra khi xử lý dữ liệu");
            showNotification(0, error.message);
            submitBtn.prop('disabled', false);
            submitBtn.text(originalText);
        }
    });

    $("#ngonNguDich").on("change", function () {
        let maNgonNgu = $(this).val();
        const xucTienID = $('#coSoId').val();

        if (coSoId && maNgonNgu) {
            console.log('Chuyển ngôn ngữ dịch cho cơ sở hiện tại:', maNgonNgu);
            // Load bản dịch cho ngôn ngữ được chọn
            loadCoSoTranslation(xucTienID, maNgonNgu);
        } else {
            // Nếu chưa có cơ sở (tạo mới) hoặc chưa chọn ngôn ngữ, clear các trường dịch
            if (!coSoId) {
                $('#tieuDe').val('');
                $('#moTa').val('');
            }
        }
    });
});

function dataAdd() {
    const submitBtn = $('#formXucTien').find('button[type="submit"]');
    const originalText = submitBtn.data('original-text') || 'Lưu';

    // Thông tin cơ bản
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
    // Nếu đến đây thì validation đã pass, tiếp tục xử lý

    // Tạo đối tượng thông tin xúc tiến
    let thongTinXucTien = {
        "linhVucID": parseInt(linhVucID),
        "toChucID": toChucID ? toChucID : null,
        "doiTuongID": parseInt(doiTuongID),
        "hinhThucID": parseInt(hinhThucID),
        "truyenThongID": parseInt(truyenThongID),
        "trangThaiID": parseInt(trangThaiID),
        "ngayBatDau": ngayBatDau,
        "ngayKetThuc": ngayKetThuc,
    };
    console.log(thongTinXucTien);
    console.log(ngayBatDau +'  '+ ngayKetThuc);
    // Tạo đối tượng nội dung đa ngữ
    let noiDungBanDich = [{
        "MaNgonNgu": ngonNguDich || "vi",
        "TieuDe": tieuDe?.trim(),
        "MoTa": moTa?.trim() || null,
    }];

    let formData = new FormData();
    formData.append('ThongTinXucTien', JSON.stringify(thongTinXucTien));
    formData.append('ThongTinXucTien_NoiDung', JSON.stringify(noiDungBanDich));

    $.ajax({
        type: 'POST',
        url: `${baseUrl}/api/ThongTinXucTienApi/ThemMoi`,
        async: false,
        contentType: false,
        processData: false,
        data: formData,
        success: function (data) {
            if (data.isSuccess && data.value) {
                showNotification(1, "Thêm mới thông tin xúc tiến thành công");
                setTimeout(() => {
                    window.location.href = `${baseUrl}/AdminTool/ThongTinXucTien`;
                }, 1500);
            } else {
                showNotification(0, data.message || "Thêm mới thông tin xúc tiến không thành công");
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
function initLinhVuc() {
    //const requestData = {
    //    MaNgonNgu: "vi",
    //    TrangThai: true
    //};

    $.ajax({
        type: 'GET',
        url: `${baseUrl}/api/LinhVucApi/Gets`,
        contentType: 'application/json',
        //data: JSON.stringify(requestData),
        success: function (response) {
            if (response && response.isSuccess && response.value) {
                $("#linhVuc").empty().append(`<option value="">Chọn lĩnh vực</option>`);

                response.value.forEach(item => {
                    //const displayText = item.tenDiaDiem || 'Không rõ tên';
                    //const addressText = item.diaChi ? ` - ${item.diaChi}` : '';
                    //const fullText = displayText + addressText;
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


