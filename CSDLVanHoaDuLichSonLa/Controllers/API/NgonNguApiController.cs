using Azure.Core;
using Domain.DanhMuc;
using Microsoft.AspNetCore.Mvc;

namespace CSDLVanHoaDuLichSonLa.Controllers.API
{
    public class NgonNguApiController : BaseApiController
    {
        public NgonNguApiController(IWebHostEnvironment hostingEnvironment, IConfiguration config) : base(hostingEnvironment, config)
        {
        }

        [HttpPost]
        [Route("DanhSach")]
        public async Task<IActionResult> DanhSach(NgonNguRequest data)
        {
            var result = await Mediator.Send(new Application.NgonNgu.DanhSach.Query { Data = data});
            return Ok(result);
        }

        [HttpPost]
        [Route("ThemMoi")]
        public async Task<IActionResult> ThemMoi(NgonNguRequest data)
        {
            var result = await Mediator.Send(new Application.NgonNgu.ThemMoiChinhSua.Command { Data = data });
            return Ok(result);
        }

        [HttpPut]
        [Route("ChinhSua/{id}")]
        public async Task<IActionResult> ChinhSua(Guid id, [FromBody] NgonNguRequest data)
        {
            data.NgonNguID = id;
            var result = await Mediator.Send(new Application.NgonNgu.ThemMoiChinhSua.Command { Data = data });
            return Ok(result);
        }

        [HttpDelete]
        [Route("Xoa/{id}")]
        public async Task<IActionResult> Xoa(Guid id)
        {
            var result = await Mediator.Send(new Application.NgonNgu.Xoa.Command { ID = id });

            return Ok(result);
        }
    }
}
