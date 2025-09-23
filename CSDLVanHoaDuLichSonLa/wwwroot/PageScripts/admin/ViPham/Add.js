const baseUrl = getRootLink();

$(document).ready(function () {
    initDatePicker();
    initDanhMucChung_NoAll(20, "#loaiHinh", "")
    initNgonNgu("#ngonNguDich")
    initSelect2();

    // Hiển thị khung nội dung phụ theo loại đơn vị
    $(document).on('change', '#loaiDonVi', function() {
        var val = $(this).val();
        if (val === '1') {
            $('.NoiDungPhu').show();
            $('#toChucViPhamSection').hide();
            $('#caNhanViPhamSection').show();
        } else if (val === '2') {
            $('.NoiDungPhu').show();
            $('#toChucViPhamSection').show();
            $('#caNhanViPhamSection').hide();
        } else if (val === '3') {
            $('.NoiDungPhu').show();
            $('#toChucViPhamSection').show();
            $('#caNhanViPhamSection').show();
        } else {
            $('.NoiDungPhu').hide();
            $('#toChucViPhamSection').hide();
            $('#caNhanViPhamSection').hide();
        }
    });

    // Gọi khi load trang để set đúng trạng thái ban đầu
    $('#loaiDonVi').trigger('change');
});
