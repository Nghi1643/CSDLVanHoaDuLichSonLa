using Domain;
using Microsoft.AspNetCore.Mvc;

namespace CSDLVanHoaDuLichSonLa.Controllers.API
{
    public class ThietLapNgonNguApiController : BaseApiController
    {
        public ThietLapNgonNguApiController(IWebHostEnvironment hostingEnvironment, IConfiguration config) : base(hostingEnvironment, config)
        {
        }

        [HttpPost]
        [Route("DanhSach")]
        public async Task<IActionResult> DanhSach(ThietLapNgonNguRequest data)
        {
            var result = await Mediator.Send(new Application.ThietLapNgonNgu.DanhSach.Query { Data = data});
            return Ok(result);
        }

        [HttpPost]
        [Route("ThemMoi")]
        public async Task<IActionResult> ThemMoi(ThietLapNgonNguRequest data)
        {
            var result = await Mediator.Send(new Application.ThietLapNgonNgu.ThemMoiChinhSua.Command { Data = data });
            return Ok(result);
        }

        [HttpPut]
        [Route("ChinhSua/{id}")]
        public async Task<IActionResult> ChinhSua(int id,[FromBody] ThietLapNgonNguRequest data)
        {
            data.ID = id;
            var result = await Mediator.Send(new Application.ThietLapNgonNgu.ThemMoiChinhSua.Command { Data = data });
            return Ok(result);
        }

        [HttpDelete]
        [Route("Xoa/{id}")]
        public async Task<IActionResult> Xoa(int id)
        {
            var result = await Mediator.Send(new Application.ThietLapNgonNgu.Xoa.Command { ID = id });
            return Ok(result);
        }
    }
}
