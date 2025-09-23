using CSDLVanHoaDuLichSonLa.Models;
using CSDLVanHoaDuLichSonLa.Services;
using Domain.Core;
using Domain.DanhMuc;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CSDLVanHoaDuLichSonLa.Controllers.API
{
    public class DanhMucChungApiController : BaseApiController
    {
        public DanhMucChungApiController(IWebHostEnvironment hostingEnvironment, IConfiguration config) : base(hostingEnvironment, config)
        {

        }

        [HttpPost]
        [Route("DanhSach")]
        public async Task<IActionResult> DanhSach(DanhMucChungRequest data)
        {
            data.MaNgonNgu = "vi";
            var result = await Mediator.Send(new Application.DanhMucChung.DanhSachTheoLoaiDanhMuc.Query { Data = data });
            return Ok(result);
        }

        [HttpPost]
        [Route("ThemMoi")]
        public async Task<IActionResult> ThemMoi(DanhMucChungAdd data)
        {
            if (data?.DanhMucChung_NoiDungs?.Any() == true)
            {
                var result = await Mediator.Send(new Application.DanhMucChung.ThemMoiChinhSua.Command { Data = data });
                return Ok(result);
            }
            return BadRequest("Tham số không hợp lệ");
        }

        [HttpPut]
        [Route("ChinhSua/{danhMucID}")]
        public async Task<IActionResult> ChinhSua(int danhMucID, [FromBody] DanhMucChungAdd data)
        {
            data.DanhMucID = danhMucID;
            if(data.DanhMucID == null)
            {
                return BadRequest(Result<DanhMucChung>.Failure("Tham số truyền vào không hợp lệ"));
            }
            var result = await Mediator.Send(new Application.DanhMucChung.ThemMoiChinhSua.Command { Data = data });
            return Ok(result);
        }

        [HttpGet]
        [Route("BanDich/{danhMucID}")]
        public async Task<IActionResult> BanDich(int danhMucID,[FromQuery] string maNgonNgu)
        {
            if (danhMucID != 0)
            {
                var result = await Mediator.Send(new Application.DanhMucChung.DanhSachBanDich.Query { DanhMucID = danhMucID , MaNgonNgu = maNgonNgu});
                return Ok(result);
            }
            return BadRequest("Tham số không hợp lệ");
        }

        [HttpDelete]
        [Route("Xoa/{id}")]
        public async Task<IActionResult> Xoa(int id)
        {
            var result = await Mediator.Send(new Application.DanhMucChung.Xoa.Command { ID = id });

            return Ok(result);
        }
    }
}
