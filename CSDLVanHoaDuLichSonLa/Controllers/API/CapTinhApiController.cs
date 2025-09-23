using CSDLVanHoaDuLichSonLa.Models;
using CSDLVanHoaDuLichSonLa.Services;
using Domain.DanhMuc;
using Microsoft.AspNetCore.Mvc;

namespace CSDLVanHoaDuLichSonLa.Controllers.API
{
    [Area("CMS")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CapTinhApiController : ControllerBase
    {
        private readonly ICapTinhService _capTinhService;

        public CapTinhApiController(ICapTinhService capTinhService)
        {
            _capTinhService = capTinhService;
        }

        /// <summary>
        /// Lấy danh sách tất cả Cấp Tỉnh
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Gets()
        {
            try
            {
                IEnumerable<CapTinhTrinhDien> list = await _capTinhService.Gets();
                return Ok(new ApiSuccessResult<IEnumerable<CapTinhTrinhDien>>(list, "Lấy danh sách Cấp Tỉnh thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiErrorResult<IEnumerable<CapTinhTrinhDien>>($"Lỗi: [{ex.Message}]"));
            }
        }

        /// <summary>
        /// Lấy thông tin Cấp Tỉnh theo ID
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                CapTinhTrinhDien data = await _capTinhService.GetById(id);
                if (data == null)
                    return NotFound(new ApiErrorResult<CapTinhTrinhDien>("Không tìm thấy thông tin Cấp Tỉnh!"));

                return Ok(new ApiSuccessResult<CapTinhTrinhDien>(data, "Lấy thông tin Cấp Tỉnh thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiErrorResult<CapTinh>($"Lỗi: [{ex.Message}]"));
            }
        }

        /// <summary>
        /// Thêm mới Cấp Tỉnh
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CapTinh request)
        {
            try
            {
                CapTinh result = await _capTinhService.Create(request);
                return Ok(new ApiSuccessResult<CapTinh>(result, "Thêm mới Cấp Tỉnh thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiErrorResult<CapTinh>($"Lỗi: [{ex.Message}]"));
            }
        }

        /// <summary>
        /// Cập nhật thông tin Cấp Tỉnh
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] CapTinh request)
        {
            try
            {
                CapTinh result = await _capTinhService.Update(request);
                return Ok(new ApiSuccessResult<CapTinh>(result, "Cập nhật Cấp Tỉnh thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiErrorResult<CapTinh>($"Lỗi: [{ex.Message}]"));
            }
        }

        /// <summary>
        /// Xóa Cấp Tỉnh theo ID
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                int rowsAffected = await _capTinhService.Delete(id);
                if (rowsAffected <= 0)
                    return NotFound(new ApiErrorResult<bool>("Không tìm thấy dữ liệu để xóa!"));

                return Ok(new ApiSuccessResult<bool>(true, "Xóa Cấp Tỉnh thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiErrorResult<bool>($"Lỗi: [{ex.Message}]"));
            }
        }
    }
}
