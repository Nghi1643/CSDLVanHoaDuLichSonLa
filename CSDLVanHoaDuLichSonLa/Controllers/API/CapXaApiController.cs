using Application.CapXa;
using CSDLVanHoaDuLichSonLa.Models;
using CSDLVanHoaDuLichSonLa.Services;
using Domain.Core;
using Domain.DanhMuc;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CSDLVanHoaDuLichSonLa.Controllers.API
{
    [Area("CMS")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CapXaApiController : ControllerBase
    {
        private readonly ICapXaService _capXaService;
        private readonly IMediator _mediator;

        public CapXaApiController(ICapXaService capXaService, IMediator mediator)
        {
            _capXaService = capXaService;
            _mediator = mediator;
        }

        /// <summary>
        /// Lấy danh sách tất cả Cấp Xã
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Gets()
        {
            try
            {
                IEnumerable<CapXaTrinhDien> list = await _capXaService.Gets();
                return Ok(new ApiSuccessResult<IEnumerable<CapXaTrinhDien>>(list, "Lấy danh sách Cấp Xã thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiErrorResult<IEnumerable<CapXa>>($"Lỗi: [{ex.Message}]"));
            }
        }

        [HttpGet]
        public async Task<Result<IEnumerable<CapXaTrinhDien>>> GetByMaTinh(string MaNgonNgu, Guid MaTinh)
        {
            return await _mediator.Send(new GetByMaTInh.Query { MaNgonNgu = MaNgonNgu, MaTinh = MaTinh });
        }

        /// <summary>
        /// Lấy thông tin Cấp Xã theo ID
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                CapXaTrinhDien data = await _capXaService.GetById(id);
                if (data == null)
                    return NotFound(new ApiErrorResult<CapXa>("Không tìm thấy thông tin Cấp Xã!"));

                return Ok(new ApiSuccessResult<CapXaTrinhDien>(data, "Lấy thông tin Cấp Xã thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiErrorResult<CapXa>($"Lỗi: [{ex.Message}]"));
            }
        }

        /// <summary>
        /// Thêm mới Cấp Xã
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CapXa request)
        {
            try
            {
                CapXa result = await _capXaService.Create(request);
                return Ok(new ApiSuccessResult<CapXa>(result, "Thêm mới Cấp Xã thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiErrorResult<CapXa>($"Lỗi: [{ex.Message}]"));
            }
        }

        /// <summary>
        /// Cập nhật thông tin Cấp Xã
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] CapXa request)
        {
            try
            {
                CapXa result = await _capXaService.Update(request);
                return Ok(new ApiSuccessResult<CapXa>(result, "Cập nhật Cấp Xã thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiErrorResult<CapXa>($"Lỗi: [{ex.Message}]"));
            }
        }

        /// <summary>
        /// Xóa Cấp Xã theo ID
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                int rowsAffected = await _capXaService.Delete(id);
                if (rowsAffected <= 0)
                    return NotFound(new ApiErrorResult<bool>("Không tìm thấy dữ liệu để xóa!"));

                return Ok(new ApiSuccessResult<bool>(true, "Xóa Cấp Xã thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiErrorResult<bool>($"Lỗi: [{ex.Message}]"));
            }
        }
    }
}
