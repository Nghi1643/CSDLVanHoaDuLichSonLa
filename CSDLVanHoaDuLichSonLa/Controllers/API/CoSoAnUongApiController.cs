using Domain.BaoChi;
using Domain.CoSoKinhDoanhDichVuDuLich;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CSDLVanHoaDuLichSonLa.Controllers.API
{
    public class CoSoAnUongApiController : BaseApiController
    {
        public CoSoAnUongApiController(IWebHostEnvironment hostingEnvironment, IConfiguration config) : base(hostingEnvironment, config)
        {
        }

        [HttpPost("DanhSach")]
        public async Task<IActionResult> DanhSach([FromBody] CoSoAnUongRequest data)
        {
            try
            {
                data.MaNgonNgu = "vi";
                var result = await Mediator.Send(new Application.CoSoKinhDoanhDichVuDuLich.CoSoAnUong.DanhSach.Query { Data = data });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("ThemMoi")]
        public async Task<IActionResult> ThemMoi([FromForm] CoSoAnUongAdd data)
        {
            try
            {
                if (data == null || string.IsNullOrEmpty(data.CoSoAnUong) || string.IsNullOrEmpty(data.CoSoAnUong_NoiDung))
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }

                CoSoAnUong coSoAnUong = JsonConvert.DeserializeObject<CoSoAnUong>(data.CoSoAnUong);
                List<CoSoAnUong_NoiDung> banDich = JsonConvert.DeserializeObject<List<CoSoAnUong_NoiDung>>(data.CoSoAnUong_NoiDung);


                var result = await Mediator.Send(new Application.CoSoKinhDoanhDichVuDuLich.CoSoAnUong.ThemMoiChinhSua.Command { CoSoAnUong = coSoAnUong, NoiDungBanDich = banDich });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("ChinhSua/{id}")]
        public async Task<IActionResult> ChinhSua(Guid id, [FromForm] CoSoAnUongAdd data)
        {
            try
            {
                if (id==Guid.Empty || data == null || string.IsNullOrEmpty(data.CoSoAnUong) || string.IsNullOrEmpty(data.CoSoAnUong_NoiDung))
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }

                CoSoAnUong coSoAnUong = JsonConvert.DeserializeObject<CoSoAnUong>(data.CoSoAnUong);
                List<CoSoAnUong_NoiDung> banDich = JsonConvert.DeserializeObject<List<CoSoAnUong_NoiDung>>(data.CoSoAnUong_NoiDung);

                coSoAnUong.CoSoAnUongID = id;

                var result = await Mediator.Send(new Application.CoSoKinhDoanhDichVuDuLich.CoSoAnUong.ThemMoiChinhSua.Command { CoSoAnUong = coSoAnUong, NoiDungBanDich = banDich });
                return Ok(result);
            }
            catch (Exception)
            {
                //return BadRequest(ex.Message);
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý");
            }
        }
        [HttpGet]
        [Route("ChiTiet/{tacPhamID}")]
        public async Task<IActionResult> ChiTiet(Guid tacPhamID)
        {
            try
            {
                if (tacPhamID == Guid.Empty)
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }
                var result = await Mediator.Send(new Application.CoSoKinhDoanhDichVuDuLich.CoSoAnUong.ChiTiet.Query { CoSoAnUongID = tacPhamID });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete]
        [Route("Xoa/{id}")]
        public async Task<IActionResult> Xoa(Guid id)
        {
            try
            {
                var result = await Mediator.Send(new Application.CoSoKinhDoanhDichVuDuLich.CoSoAnUong.Xoa.Command { ID = id });
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý");
            }
        }

    }
}

