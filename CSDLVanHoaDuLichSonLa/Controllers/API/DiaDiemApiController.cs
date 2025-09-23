using Application.DiaDiem;
using CSDLVanHoaDuLichSonLa.Models;
using CSDLVanHoaDuLichSonLa.Services;
using Domain;
using Domain.BaoChi;
using Domain.Core;
using Domain.DanhMuc;
using Domain.DiaDiem;
using Domain.SuKienHoatDong;
using Domain.ToChuc;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CSDLVanHoaDuLichSonLa.Controllers.API
{
    public class DiaDiemApiController : BaseApiController
    {
        public DiaDiemApiController(IWebHostEnvironment hostingEnvironment, IConfiguration config) : base(hostingEnvironment, config)
        {

        }

        [HttpPost]
        [Route("DanhSach")]
        public async Task<IActionResult> DanhSach(DiaDiemRequest data)
        {
            data.MaNgonNgu = "vi";
            var result = await Mediator.Send(new Application.DiaDiem.DanhSach.Query { Data = data });
            return Ok(result);
        }

        [HttpGet]
        [Route("GetMulti")]
        public async Task<Result<IEnumerable<DiaDiemDTO>>> GetMulti(string ListID, string MaNgonNgu)
        {
            return await Mediator.Send(new GetMulti.Query { ListID = ListID, MaNgonNgu = MaNgonNgu });
        }

        [HttpPost]
        [Route("ThemMoi")]
        public async Task<IActionResult> ThemMoi([FromForm]DiaDiemAdd data)
        {
            try
            {
                if (data == null || string.IsNullOrEmpty(data.DiaDiem) || string.IsNullOrEmpty(data.DiaDiem_NoiDung))
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }

                Domain.DiaDiem.DiaDiem diadiem = JsonConvert.DeserializeObject<Domain.DiaDiem.DiaDiem>(data.DiaDiem);
                List<DiaDiem_NoiDung> banDich = JsonConvert.DeserializeObject<List<DiaDiem_NoiDung>>(data.DiaDiem_NoiDung);
                var result = await Mediator.Send(new Application.DiaDiem.ThemMoiChinhSua.Command { DiaDiem = diadiem, NoiDungBanDich = banDich });
                return Ok(result);
            }
            catch (Exception)
            {
                //return BadRequest(ex.Message);
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý");
            }
        }

        [HttpPut]
        [Route("ChinhSua/{diaDiemID}")]
        public async Task<IActionResult> ChinhSua(Guid diaDiemID,  DiaDiemAdd data)
        {

            try
            {
                if (data == null || string.IsNullOrEmpty(data.DiaDiem) || string.IsNullOrEmpty(data.DiaDiem_NoiDung))
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }
                Domain.DiaDiem.DiaDiem diadiem = JsonConvert.DeserializeObject<Domain.DiaDiem.DiaDiem>(data.DiaDiem);
                List<DiaDiem_NoiDung> banDich = JsonConvert.DeserializeObject<List<DiaDiem_NoiDung>>(data.DiaDiem_NoiDung);
                diadiem.DiaDiemID = diaDiemID;
                var result = await Mediator.Send(new Application.DiaDiem.ThemMoiChinhSua.Command { DiaDiem = diadiem, NoiDungBanDich = banDich });
                return Ok(result);
            }
            catch (Exception)
            {
                //return BadRequest(ex.Message);
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý");
            }
        }

        [HttpDelete]
        [Route("Xoa/{id}")]
        public async Task<IActionResult> Xoa(Guid id)
        {
            var result = await Mediator.Send(new Application.DiaDiem.Xoa.Command { ID = id });

            return Ok(result);
        }
    }
}
