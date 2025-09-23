const baseUrl = getRootLink();

$(document).ready(function () {
    initSelect2();
    initValidation();
    initDatePicker();

    // Load dữ liệu cho các dropdown
    (async function () {
        await Promise.all([
            initTacPham(),
            initNgonNguDich()
        ]);
    })();

    function initValidation() {
        CheckLengthEach('#tenGiaiThuong', 255, null);
        CheckLengthEach('#hangMucGiaiThuong', 255, null);
        CheckLengthEach('#moTa', 1000, null);
    }

    async function initTacPham() {
        try {
            const res = await fetch(`${baseUrl}/api/TacPhamApi/DanhSach`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ trangThai: true })
            });
            if (!res.ok) throw new Error(await res.text());
            const data = await res.json();
            if (data && data.isSuccess && data.value) {
                $("#tacPham").empty().append(`<option value="">Chọn tác phẩm</option>`);
                data.value.forEach(item => {
                    $("#tacPham").append(`<option value="${item.tacPhamID}">${item.tenTacPham}</option>`);
                });
            }
        } catch (err) { console.log('Lỗi khi tải tác phẩm:', err.message); }
    }

    async function initNgonNguDich() {
        try {
            const res = await fetch(`${baseUrl}/api/NgonNguApi/DanhSach`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ trangThai: true, tuKhoa: null })
            });
            if (!res.ok) throw new Error(await res.text());
            const data = await res.json();
            if (data && data.isSuccess && data.value) {
                $("#ngonNguDich").empty().append(`<option value="">Chọn ngôn ngữ</option>`);
                data.value.forEach(lang => {
                    $("#ngonNguDich").append(`<option value="${lang.maNgonNgu?.toLowerCase()}">${lang.tenNgonNgu}</option>`);
                });
            }
        } catch (err) { console.log('Lỗi khi tải danh sách ngôn ngữ:', err.message); }
    }

    // Xử lý đa ngữ cho giải thưởng (nếu cần)
    $("#ngonNguDich").on("change", function () {
        // Có thể load bản dịch nếu cần
    });

    // Xử lý submit form
    $("#formGiaiThuong").on("submit", async function (e) {
        e.preventDefault();

        // Thu thập dữ liệu từ form
        let tacPham = $('#tacPham').val();
        let namTraoGiai = $('#namTraoGiai').val();
        let thuTu = $('#thuTu').val();
        let trangThai = $("input[name='trangThai']:checked").val();
        let ngonNguDich = $('#ngonNguDich').val();
        let tenGiaiThuong = $('#tenGiaiThuong').val();
        let hangMucGiaiThuong = $('#hangMucGiaiThuong').val();
        let moTa = $('#moTa').val();

        // Validate các trường bắt buộc
        const requiredFields = ['#tacPham', '#namTraoGiai', '#tenGiaiThuong', '#hangMucGiaiThuong'];
        if (checkEmptyBlankV2(requiredFields)) {
            showNotification(0, 'Vui lòng nhập đầy đủ thông tin bắt buộc (*)');
            return;
        }

        // Gom dữ liệu ngoài đa ngữ
        let giaiThuong = {
            tacPham,
            namTraoGiai,
            thuTu,
            trangThai
        };

        // Gom dữ liệu đa ngữ
        let giaiThuongNoiDungArr = [];
        if (ngonNguDich && tenGiaiThuong && hangMucGiaiThuong) {
            giaiThuongNoiDungArr.push({
                ngonNgu: ngonNguDich,
                tenGiaiThuong: tenGiaiThuong,
                hangMucGiaiThuong: hangMucGiaiThuong,
                moTa: moTa
            });
        }

        // Chuẩn bị form data
        let formData = new FormData();
        formData.append("GiaiThuong", JSON.stringify(giaiThuong));
        formData.append("GiaiThuong_NoiDung", JSON.stringify(giaiThuongNoiDungArr));

        // TODO: Xử lý danh sách tổ chức/cá nhân đạt giải nếu có
        // formData.append("ToChucDatGiai", JSON.stringify(toChucArr));
        // formData.append("CaNhanDatGiai", JSON.stringify(caNhanArr));

        try {
            const res = await fetch(`${baseUrl}/api/GiaiThuongApi/ThemMoi`, {
                method: 'POST',
                body: formData
            });
            if (!res.ok) throw new Error(await res.text());
            const result = await res.json();
            if (result && result.isSuccess) {
                showNotification(1, 'Thêm mới thành công');
                setTimeout(() => {
                    window.location.href = `${baseUrl}/AdminTool/GiaiThuong/GiaiThuong`;
                }, 1200);
            } else {
                showNotification(0, result.error || 'Lỗi không xác định');
            }
        } catch (err) {
            showNotification(0, `Thao tác bị hủy: ${err.message}`);
        }
    });

    // Xử lý thêm tổ chức đạt giải
    $('#btnThemToChuc').on('click', function() {
        // Hiển thị modal thêm tổ chức đạt giải (cần bổ sung modal và logic)
    });

    // Xử lý thêm cá nhân đạt giải
    $('#btnThemCaNhan').on('click', function() {
        // Hiển thị modal thêm cá nhân đạt giải (cần bổ sung modal và logic)
    });

    // TODO: Xử lý render bảng tổ chức/cá nhân đạt giải, xóa, sửa, ...
});
