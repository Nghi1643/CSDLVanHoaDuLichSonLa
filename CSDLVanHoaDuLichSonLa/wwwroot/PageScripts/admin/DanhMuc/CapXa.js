const baseUrl = getRootLink();

$(document).ready(function () {
    initSelect2();
    initTable();
    loadNgonNguData(); // Load dữ liệu ngôn ngữ
    loadCapTinhData(); // Load dữ liệu cấp tỉnh

    // Áp dụng validation cho các trường input
    initValidation();

    function initValidation() {
        // Validation cho form Add (cũng dùng cho Edit)
        CheckLengthEach('#maSoAdd, #maSoEdit', 50, null); // Giới hạn 50 ký tự cho mã số
        CheckLengthEach('#tenDonViAdd, #tenDonViEdit', 255, null); // Giới hạn 255 ký tự cho tên đơn vị
        CheckLengthEach('#thuTuAdd, #thuTuEdit', 10, null); // Giới hạn 10 ký tự cho thứ tự
    }

    // Function để format mã xã với số 0 đằng trước thành 5 chữ số
    function formatMaXa(value) {
        // Chỉ xử lý nếu là số
        const num = parseInt(value);
        if (!isNaN(num) && num >= 0) {
            // Pad với số 0 đằng trước để có 5 chữ số
            return num.toString().padStart(5, '0');
        }
        return value;
    }

    // Event listener cho việc format mã xã
    $(document).on('blur', '#maSoAdd, #maSoEdit', function() {
        const currentValue = $(this).val().trim();
        if (currentValue) {
            const formattedValue = formatMaXa(currentValue);
            $(this).val(formattedValue);
        }
    });

    async function initTable() {
        const tableApi = {
            url: `${baseUrl}/api/CapXaApi/Gets`,
            type: "GET",
            contentType: 'application/json; charset=utf-8',
            dataSrc: function (data) {
                if (data && data.isSuccessed && data.resultObj && data.resultObj.length > 0) {
                    // Sử dụng dữ liệu cấp 1, giữ nguyên mảng daNgu
                    let processedData = data.resultObj.map(item => ({
                        id: item.xaID,
                        maXa: item.maXa,
                        noiDung: item.noiDung,
                        sapXep: item.sapXep,
                        trangThai: item.trangThai,
                        tinhID: item.tinhID,
                        tenTinh: item.tenTinh,
                        cap: item.cap,
                        daNgu: item.daNgu // Giữ nguyên mảng daNgu để sử dụng sau
                    }));
                    console.log(processedData)
                    
                    // Sắp xếp dữ liệu theo trường thứ tự (sapXep) trước, nếu trùng thì theo Mã xã
                    processedData.sort((a, b) => {
                        const orderA = a.sapXep || 999999; // Nếu không có sapXep thì đặt cuối
                        const orderB = b.sapXep || 999999;
                        
                        // So sánh theo thứ tự trước
                        if (orderA !== orderB) {
                            return orderA - orderB; // Sắp xếp tăng dần theo thứ tự
                        }
                        
                        // Nếu thứ tự trùng nhau, sắp xếp theo Mã xã (maXa)
                        const codeA = (a.maXa || '').toString().toLowerCase();
                        const codeB = (b.maXa || '').toString().toLowerCase();
                        return codeA.localeCompare(codeB, 'vi', { numeric: true, sensitivity: 'base' });
                    });
                    
                    // Thêm STT sau khi đã sắp xếp
                    processedData.forEach((item, index) => {
                        item.stt = index + 1;
                    });
                    
                    return processedData;
                }
                return [];
            },
        };

        const tableDefs = [
            {
                targets: 1, // Cột mã xã (index 1)
                render: function (data, type, row, meta) {
                    // Format mã xã với 5 chữ số
                    const num = parseInt(data);
                    if (!isNaN(num) && num >= 0) {
                        return num.toString().padStart(5, '0');
                    }
                    return data;
                }
            },
            {
                targets: 3, // Cột cấp
                render: function (data, type, row, meta) {
                    switch(data) {
                        case 1: return 'Phường';
                        case 2: return 'Xã';
                        case 3: return 'Đặc khu';
                        default: return data;
                    }
                }
            },
            {
                targets: 6,
                render: function (data, type, row, meta) {
                    if (row.trangThai == false || row.trangThai == 0) {
                        return `<i class="hgi-icon hgi-cancel"></i>`;
                    }
                    else if (row.trangThai == true || row.trangThai == 1) {
                        return `<i class="hgi-icon hgi-check"></i>`; 
                    }
                }
            },
            {
                targets: 7,
                render: function (data, type, row, meta) {
                    let html = ""
                    if (permitedEdit) {
                        html += `<i data-toggle="tooltip" title="Chỉnh sửa" class="edit-command-btn text-yellow" id=n-"${meta.row}">
                                    <i class="hgi-icon hgi-edit"></i>
                                </i>`;
                    }
                    if (permitedDelete) {
                        html += `<i data-toggle="tooltip" title="Xóa" class="delete-command-btn text-red" id=n-"${meta.row}">
                                    <i class="hgi-icon hgi-delete"></i>
                                </i>`
                    }
                    return html
                }
            }
        ];

        const tableCols = [
            { "data": "stt", "width": "40px", "class": "center-align" },
            { "data": "maXa", "width": "10%", "class": "left-align" },
            { "data": "noiDung", "class": "left-align" },
            { "data": "cap", "width": "120px", "class": "left-align" },
            { "data": "tenTinh", "width": "15%", "class": "left-align" },
            { "data": "sapXep", "width": "120px", "class": "center-align" },
            { "data": "trangThai", "width": "120px", "class": "center-align" },
            { "data": "", "width": "120px", "class": "center-align group-icon-action" },
        ];

        if (!permitedEdit && !permitedDelete) {
            tableCols.pop();
            tableDefs.pop();
        }

        initDataTableConfig('dataGrid', tableApi, tableDefs, tableCols);

        $(".New_Table .dataTables_filter input").prependTo(".New_Table .group-button").addClass("search-icon").attr("style", "width: 350px;")
        $(".New_Table .dataTables_filter label").remove()

        $('#dataGrid tbody').on('click', '.edit-command-btn', function () {
            var id = $(this).attr("ID").match(/\d+/)[0];
            var data = $('#dataGrid').DataTable().row(id).data();

            // Fill dữ liệu cơ bản
            $('#idCapXaEdit').val(data.id);
            $("#maSoEdit").val(data.maXa);
            $('#thuTuEdit').val(data.sapXep || 1);
            $(`input[name="trangThaiEdit"][value="${data.trangThai ? 'true' : 'false'}"]`).prop('checked', true);
            
            // Set cấp
            $(`input[name="capXaEdit"][value="${data.cap}"]`).prop('checked', true);
            
            // Set tỉnh/thành phố
            $('#select_tinhEdit').val(data.tinhID).trigger('change');
            
            // Reset các trường đa ngữ trước
            $('#tenDonViEdit').val('');
            $('#ngonNguEdit').val('').trigger('change');
            
            // Lưu dữ liệu daNgu để xử lý khi thay đổi ngôn ngữ
            $('#modalEdit').data('daNguArray', data.daNgu || []);
            
            // Nếu có dữ liệu daNgu, set ngôn ngữ đầu tiên làm mặc định
            if (data.daNgu && data.daNgu.length > 0) {
                const firstDaNgu = data.daNgu[0];
                $('#ngonNguEdit').val(firstDaNgu.ngonNguID).trigger('change');
                $('#tenDonViEdit').val(firstDaNgu.noiDung);
            } else {
                // Nếu không có daNgu, sử dụng dữ liệu gốc
                $('#tenDonViEdit').val(data.noiDung);
            }

            $('#modalEdit').modal('show');
        });
        
        $('#dataGrid tbody').on('click', '.delete-command-btn', function () {
            var id = $(this).attr("ID").match(/\d+/)[0];
            var data = $('#dataGrid').DataTable().row(id).data();

            $('#idDelete').val(data.id);
            $('#nameDelete').text(`${data.noiDung}`);

            $('#modalDelete').modal('show');
        });  
    }

    $("#formAdd").on("submit", async function (e) {
        e.preventDefault();
        
        // Disable submit button để tránh spam
        const submitBtn = $(this).find('button[type="submit"]');
        if (submitBtn.prop('disabled')) {
            return; // Nếu button đã disabled, không làm gì cả
        }
        
        submitBtn.prop('disabled', true);
        const originalText = submitBtn.text();
        submitBtn.text('Đang xử lý...');
        
        try {
            // Gọi function xử lý
            await dataAdd();
        } catch (error) {
            console.error('Lỗi khi xử lý form:', error);
        } finally {
            // Enable lại button sau 1 giây
            setTimeout(() => {
                submitBtn.prop('disabled', false);
                submitBtn.text(originalText);
            }, 1000);
        }
    });

    $("#formDelete").on("submit", async function (e) {
        e.preventDefault();
        
        // Disable submit button để tránh spam
        const submitBtn = $(this).find('button[type="submit"]');
        if (submitBtn.prop('disabled')) {
            return; // Nếu button đã disabled, không làm gì cả
        }
        
        submitBtn.prop('disabled', true);
        const originalText = submitBtn.text();
        submitBtn.text('Đang xóa...');
        
        try {
            // Gọi function xử lý
            await dataDelete();
        } catch (error) {
            console.error('Lỗi khi xử lý form:', error);
        } finally {
            // Enable lại button sau 1 giây
            setTimeout(() => {
                submitBtn.prop('disabled', false);
                submitBtn.text(originalText);
            }, 1000);
        }
    });

    $("#formEdit").on("submit", async function (e) {
        e.preventDefault();
        
        // Disable submit button để tránh spam
        const submitBtn = $(this).find('button[type="submit"]');
        if (submitBtn.prop('disabled')) {
            return; // Nếu button đã disabled, không làm gì cả
        }
        
        submitBtn.prop('disabled', true);
        const originalText = submitBtn.text();
        submitBtn.text('Đang cập nhật...');
        
        try {
            // Gọi function xử lý
            await dataEdit();
        } catch (error) {
            console.error('Lỗi khi xử lý form:', error);
        } finally {
            // Enable lại button sau 1 giây
            setTimeout(() => {
                submitBtn.prop('disabled', false);
                submitBtn.text(originalText);
            }, 1000);
        }
    });

    // Kiểm tra trùng lặp mã xã và ngôn ngữ bằng API Gets - CHỈ DÀNH CHO MODAL ADD
    async function checkDuplicate(maXa, ngonNguID, tinhID) {
        try {
            const res = await fetch(`${baseUrl}/api/CapXaApi/Gets`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            if (!res.ok) {
                console.error('Lỗi khi kiểm tra trùng lặp');
                return false; // Nếu API lỗi, cho phép tiếp tục
            }

            const data = await res.json();
            
            if (data && data.isSuccessed && data.resultObj) {
                const tableData = data.resultObj;
                
                // Kiểm tra trùng lặp cho thêm mới
                for (let i = 0; i < tableData.length; i++) {
                    const item = tableData[i];
                    
                    // Kiểm tra trùng mã xã và cùng tỉnh
                    if (item.maXa == maXa && item.tinhID == tinhID) {
                        // Nếu cả hai đều không có ngôn ngữ (null/empty)
                        if ((!ngonNguID || ngonNguID === '') && (!item.ngonNguID || item.ngonNguID === '')) {
                            return true;
                        }
                        // Nếu cả hai đều có ngôn ngữ và trùng nhau
                        if (ngonNguID && item.ngonNguID && ngonNguID === item.ngonNguID) {
                            return true;
                        }
                    }
                }
            }
            
            return false;
            
        } catch (err) {
            console.error('Lỗi khi kiểm tra trùng lặp:', err);
            return false; // Nếu có lỗi, cho phép tiếp tục
        }
    }

    // Kiểm tra trùng lặp mã xã và ngôn ngữ cho EDIT - loại trừ record hiện tại
    async function checkDuplicateForEdit(maXa, ngonNguID, tinhID, currentId) {
        try {
            const res = await fetch(`${baseUrl}/api/CapXaApi/Gets`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            if (!res.ok) {
                console.error('Lỗi khi kiểm tra trùng lặp');
                return false; // Nếu API lỗi, cho phép tiếp tục
            }

            const data = await res.json();
            
            if (data && data.isSuccessed && data.resultObj) {
                const tableData = data.resultObj;
                
                // Kiểm tra trùng lặp cho chỉnh sửa (loại trừ record hiện tại)
                for (let i = 0; i < tableData.length; i++) {
                    const item = tableData[i];
                    
                    // Bỏ qua record hiện tại khi edit
                    if (currentId && item.id == currentId) {
                        continue;
                    }
                    
                    // Kiểm tra trùng mã xã và cùng tỉnh
                    if (item.maXa == maXa && item.tinhID == tinhID) {
                        // Nếu cả hai đều không có ngôn ngữ (null/empty)
                        if ((!ngonNguID || ngonNguID === '') && (!item.ngonNguID || item.ngonNguID === '')) {
                            return true;
                        }
                        // Nếu cả hai đều có ngôn ngữ và trùng nhau
                        if (ngonNguID && item.ngonNguID && ngonNguID === item.ngonNguID) {
                            return true;
                        }
                    }
                }
            }
            
            return false;
            
        } catch (err) {
            console.error('Lỗi khi kiểm tra trùng lặp cho edit:', err);
            return false; // Nếu có lỗi, cho phép tiếp tục
        }
    }

    // Function cho thêm mới
    async function dataAdd() {
        let maSo = $('#maSoAdd').val();
        let tenDonVi = $('#tenDonViAdd').val();
        let trangThai = $("input[name='trangThai']:checked").val();
        let ngonNguID = $('#ngonNguAdd').val();
        let thuTu = $('#thuTuAdd').val();
        let cap = $("input[name='capXa']:checked").val();
        let tinhID = $('#select_tinhAdd').val();

        // Sử dụng checkEmptyBlankV2 để validate tất cả các trường bắt buộc
        const requiredFields = ['#maSoAdd', '#tenDonViAdd'];
        if (checkEmptyBlankV2(requiredFields)) {
            showNotification(0, 'Dữ liệu sai quy định, vui lòng kiểm tra lại!')
            return; // Dừng nếu có trường trống
        }

        // Kiểm tra có lỗi validation từ CheckLengthEach hay không
        if (checkFalseInput(requiredFields)) {
            showNotification(0, 'Dữ liệu sai quy định, vui lòng kiểm tra lại!')
            return; // Dừng nếu có lỗi validation
        }

        if (checkEmptyBlank(trangThai)) {
            showNotification(0, 'Vui lòng chọn trạng thái!')
            return;
        }

        if (checkEmptyBlank(cap)) {
            showNotification(0, 'Vui lòng chọn cấp phường/xã/đặc khu!')
            return;
        }

        if (checkEmptyBlank(tinhID)) {
            showNotification(0, 'Vui lòng chọn tỉnh/thành phố!')
            return;
        }

        // Kiểm tra trùng lặp mã xã và ngôn ngữ (không có id khi thêm mới)
        const isDuplicate = await checkDuplicate(maSo, ngonNguID, tinhID);
        if (isDuplicate) {
            showNotification(0, 'Xã đã tồn tại');
            return;
        }

        let dt = {
            "loaiDM": 3,
            "maXa": maSo?.trim(),
            "noiDung": tenDonVi?.trim(),
            "trangThai": trangThai === "true",
            "sapXep": !checkEmptyBlank(thuTu) ? parseInt(thuTu) : 1,
            "tinhID": tinhID,
            "cap": parseInt(cap)
        };

        // Thêm ngonNguID nếu có chọn
        if (!checkEmptyBlank(ngonNguID)) {
            dt.ngonNguID = ngonNguID;
        }

        try {
            const res = await fetch('/api/CapXaApi/Add', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(dt)
            })

            if (!res.ok) {
                var errText = await res.text();
                throw new Error(errText);
            }
            data = await res.json();

            if (data && data.isSuccessed && data.resultObj) {
                showNotification(1, 'Thêm mới thành công')
                $('#dataGrid').DataTable().ajax.reload(null, false);
                $('#modalAdd').modal('hide');
            } else {
                showNotification(0, data.message || 'Có lỗi xảy ra')
            }
        }
        catch (err) {
            showNotification(0, err.message)
        }
    }

    // Function cho chỉnh sửa
    async function dataEdit() {
        let id = $('#idCapXaEdit').val();
        let maSo = $('#maSoEdit').val();
        let tenDonVi = $('#tenDonViEdit').val();
        let trangThai = $("input[name='trangThaiEdit']:checked").val();
        let ngonNguID = $('#ngonNguEdit').val();
        let thuTu = $('#thuTuEdit').val();
        let cap = $("input[name='capXaEdit']:checked").val();
        let tinhID = $('#select_tinhEdit').val();

        // Sử dụng checkEmptyBlankV2 để validate tất cả các trường bắt buộc
        const requiredFields = ['#maSoEdit', '#tenDonViEdit'];
        if (checkEmptyBlankV2(requiredFields)) {
            showNotification(0, 'Dữ liệu sai quy định, vui lòng kiểm tra lại!')
            return; // Dừng nếu có trường trống
        }

        // Kiểm tra có lỗi validation từ CheckLengthEach hay không
        if (checkFalseInput(requiredFields)) {
            showNotification(0, 'Dữ liệu sai quy định, vui lòng kiểm tra lại!')
            return; // Dừng nếu có lỗi validation
        }

        if (checkEmptyBlank(trangThai)) {
            showNotification(0, 'Vui lòng chọn trạng thái!')
            return;
        }

        if (checkEmptyBlank(cap)) {
            showNotification(0, 'Vui lòng chọn cấp phường/xã/đặc khu!')
            return;
        }

        if (checkEmptyBlank(tinhID)) {
            showNotification(0, 'Vui lòng chọn tỉnh/thành phố!')
            return;
        }

        if (checkEmptyBlank(id)) {
            showNotification(0, 'Không tìm thấy ID để chỉnh sửa!')
            return;
        }

        // Kiểm tra ngôn ngữ có tồn tại trong daNgu hay không
        const daNguArray = $('#modalEdit').data('daNguArray') || [];
        const existingDaNgu = daNguArray.find(item => item.ngonNguID === ngonNguID);
        const isNewLanguage = ngonNguID && !existingDaNgu;

        // Kiểm tra trùng lặp mã xã và ngôn ngữ (loại trừ record hiện tại)
        const isDuplicate = await checkDuplicateForEdit(maSo, ngonNguID, tinhID, id);
        if (isDuplicate) {
            showNotification(0, 'Mã xã này đã tồn tại trong ngôn ngữ đã chọn!');
            return;
        }

        let dt = {
            "XaID": id,
            "loaiDM": 3,
            "maXa": maSo?.trim(),
            "noiDung": tenDonVi?.trim(),
            "trangThai": trangThai === "true",
            "sapXep": !checkEmptyBlank(thuTu) ? parseInt(thuTu) : 1,
            "tinhID": tinhID,
            "cap": parseInt(cap)
        };

        // Thêm ngonNguID nếu có chọn
        if (!checkEmptyBlank(ngonNguID)) {
            dt.ngonNguID = ngonNguID;
        }

        // Nếu là ngôn ngữ mới (không có trong daNgu), thêm noiDungID từ existingDaNgu nếu có
        if (existingDaNgu && existingDaNgu.noiDungID) {
            dt.noiDungID = existingDaNgu.noiDungID;
        }

        try {
            // Chọn API dựa vào việc ngôn ngữ có tồn tại trong daNgu hay không
            const apiEndpoint = isNewLanguage ? '/api/CapXaApi/Add' : '/api/CapXaApi/Edit';
            const successMessage = isNewLanguage ? 'Thêm ngôn ngữ mới thành công' : 'Chỉnh sửa thành công';
            const notificationType = isNewLanguage ? 1 : 2; // 1 = success add, 2 = success edit

            console.log(`Sử dụng API: ${apiEndpoint}, Ngôn ngữ mới: ${isNewLanguage}`);
            console.log(dt);

            const res = await fetch(apiEndpoint, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(dt)
            })

            if (!res.ok) {
                var errText = await res.text();
                throw new Error(errText);
            }

            data = await res.json();

            if (data && data.isSuccessed) {
                showNotification(notificationType, successMessage)
                $('#dataGrid').DataTable().ajax.reload(null, false);
                $('#modalEdit').modal('hide');
            } else {
                showNotification(0, data.message || 'Có lỗi xảy ra')
            }
        }
        catch (err) {
            showNotification(0, err.message)
        }
    }

    // Load dữ liệu ngôn ngữ cho select
    // Load dữ liệu ngôn ngữ cho cả modalAdd và modalEdit
    async function loadNgonNguData() {
        try {
            const res = await fetch(`${baseUrl}/api/NgonNguApi/DanhSach`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ "trangThai": true })
            });

            if (!res.ok) {
                throw new Error('Không thể tải dữ liệu ngôn ngữ');
            }

            const data = await res.json();
            
            if (data.isSuccess) {
                // Clear existing options cho modalAdd
                $('#ngonNguAdd').empty();
                // Clear existing options cho modalEdit  
                $('#ngonNguEdit').empty();
                
                // Add new options cho modalAdd
                data.value.forEach(item => {
                    $('#ngonNguAdd').append(`<option value="${item.ngonNguID}" data-symbol="${item.maNgonNgu}">${item.tenNgonNgu}</option>`);
                });
                
                // Add new options cho modalEdit
                data.value.forEach(item => {
                    $('#ngonNguEdit').append(`<option value="${item.ngonNguID}" data-symbol="${item.maNgonNgu}">${item.tenNgonNgu}</option>`);
                });

                // Trigger Select2 refresh if it's initialized
                if ($('#ngonNguAdd').hasClass('select2-hidden-accessible')) {
                    $('#ngonNguAdd').trigger('change');
                }
                if ($('#ngonNguEdit').hasClass('select2-hidden-accessible')) {
                    $('#ngonNguEdit').trigger('change');
                }
            }
        } catch (err) {
            console.error('Lỗi khi load dữ liệu ngôn ngữ:', err);
            showNotification(0, 'Không thể tải danh sách ngôn ngữ');
        }
    }

    // Load dữ liệu cấp tỉnh cho select
    async function loadCapTinhData() {
        try {
            const res = await fetch(`${baseUrl}/api/CapTinhApi/Gets`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            if (!res.ok) {
                throw new Error('Không thể tải dữ liệu cấp tỉnh');
            }

            const data = await res.json();
            
            if (data && data.isSuccessed && data.resultObj) {
                // Clear existing options cho modalAdd
                $('#select_tinhAdd').empty();
                $('#select_tinhAdd').append('<option value="">Chọn tỉnh/thành phố...</option>');
                
                // Clear existing options cho modalEdit  
                $('#select_tinhEdit').empty();
                $('#select_tinhEdit').append('<option value="">Chọn tỉnh/thành phố...</option>');
                
                // Add new options cho modalAdd
                data.resultObj.forEach(item => {
                    const displayText = item.noiDung || item.tenTinh || 'Không rõ tên';
                    $('#select_tinhAdd').append(`<option value="${item.tinhID}">${displayText}</option>`);
                });
                
                // Add new options cho modalEdit
                data.resultObj.forEach(item => {
                    const displayText = item.noiDung || item.tenTinh || 'Không rõ tên';
                    $('#select_tinhEdit').append(`<option value="${item.tinhID}">${displayText}</option>`);
                });

                // Trigger Select2 refresh if it's initialized
                if ($('#select_tinhAdd').hasClass('select2-hidden-accessible')) {
                    $('#select_tinhAdd').trigger('change');
                }
                if ($('#select_tinhEdit').hasClass('select2-hidden-accessible')) {
                    $('#select_tinhEdit').trigger('change');
                }
            }
        } catch (err) {
            console.error('Lỗi khi load dữ liệu cấp tỉnh:', err);
            showNotification(0, 'Không thể tải danh sách tỉnh/thành phố');
        }
    }

    // Event listener cho việc thay đổi ngôn ngữ trong modal Edit
    $(document).on('change', '#ngonNguEdit', function() {
        const selectedNgonNguID = $(this).val();
        const daNguArray = $('#modalEdit').data('daNguArray') || [];
        
        if (selectedNgonNguID && daNguArray.length > 0) {
            // Tìm dữ liệu tương ứng với ngôn ngữ đã chọn
            const selectedDaNgu = daNguArray.find(item => item.ngonNguID === selectedNgonNguID);
            
            if (selectedDaNgu) {
                // Cập nhật nội dung theo ngôn ngữ đã chọn
                $('#tenDonViEdit').val(selectedDaNgu.noiDung);
            } else {
                // Nếu không tìm thấy dữ liệu cho ngôn ngữ này, xóa nội dung
                $('#tenDonViEdit').val('');
            }
        } else {
            // Nếu không chọn ngôn ngữ, xóa nội dung
            $('#tenDonViEdit').val('');
        }
    });

    // reset modal
    // reset modal Add
    $("#modalAdd").on('hide.bs.modal', function () {
        $(this).find('input[type=text], textarea').val('');
        $(this).find('input[type=number]').val('');
        $(this).find('select').val('').trigger('change');
        $(this).find('input[type=radio][value="1"], input[type=checkbox][value="1"]').prop('checked', true);
        $('input[name="trangThai"][value="true"]').prop('checked', true);
        $('input[name="capXa"][value="1"]').prop('checked', true);
        $('#thuTuAdd').val('1'); // Reset thứ tự về 1
        
        // Lấy value của option có data-symbol="vi"
        var viValue = $('#ngonNguAdd option[data-symbol="vi"]').val();
        // Set select và trigger change
        $('#ngonNguAdd').val(viValue).trigger('change');
        $('#select_tinhAdd').val('').trigger('change');
        
        // Xóa các thông báo lỗi validation
        RemoveFalseInput(document.getElementById('maSoAdd'));
        RemoveFalseInput(document.getElementById('tenDonViAdd'));
        RemoveFalseInput(document.getElementById('thuTuAdd'));
    });

    // reset modal Edit
    $("#modalEdit").on('hide.bs.modal', function () {
        $(this).find('input[type=text], textarea').val('');
        $(this).find('input[type=number]').val('');
        $(this).find('select').val('').trigger('change');
        $(this).find('input[type=radio][value="1"], input[type=checkbox][value="1"]').prop('checked', true);
        $('input[name="trangThaiEdit"][value="true"]').prop('checked', true);
        $('input[name="capXaEdit"][value="1"]').prop('checked', true);
        $('#idCapXaEdit').val('');
        $('#thuTuEdit').val('1'); // Reset thứ tự về 1
        
        // Reset select ngôn ngữ về placeholder
        $('#ngonNguEdit').val('').trigger('change');
        $('#select_tinhEdit').val('').trigger('change');
        
        // Xóa các thông báo lỗi validation
        RemoveFalseInput(document.getElementById('maSoEdit'));
        RemoveFalseInput(document.getElementById('tenDonViEdit'));
        RemoveFalseInput(document.getElementById('thuTuEdit'));
    });

    async function dataDelete() {
        let id = $('#idDelete').val();

        try {
            const res = await fetch(`/api/CapXaApi/Delete?id=${id}`, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json'
                }
            })

            if (!res.ok) {
                var errText = await res.text();
                throw new Error(errText);
            }

            data = await res.json();

            if (data && data.isSuccessed && data.resultObj) {
                showNotification(1, 'Xóa thành công')
                $('#modalDelete').modal('hide');
                $('#dataGrid').DataTable().ajax.reload();
            } else {
                showNotification(0, data.message || 'Có lỗi xảy ra')
            }
        }
        catch (err) {
            showNotification(0, err.message)
        }
    }
})