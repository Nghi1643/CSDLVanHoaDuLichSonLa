using Domain.DaoTaoBoiDuong;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CSDLVanHoaDuLichSonLa.Controllers.API
{
    public class DaoTaoBoiDuongApiController : BaseApiController
    {
        public DaoTaoBoiDuongApiController(IWebHostEnvironment hostingEnvironment, IConfiguration config) : base(hostingEnvironment, config)
        {
        }
        [HttpPost("DanhSach")]
        public async Task<IActionResult> DanhSach([FromBody] DaoTaoBoiDuongRequest data)
        {
            try
            {
                data.MaNgonNgu = "vi";
                var result = await Mediator.Send(new Application.DaoTaoBoiDuong.DanhSach.Query { Data = data });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("ChiTiet/{id}")]
        public async Task<IActionResult> ChiTiet(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }
                var result = await Mediator.Send(new Application.DaoTaoBoiDuong.ChiTiet.Query { DaoTaoID = id });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("ThemMoi")]
        public async Task<IActionResult> ThemMoi([FromForm] DaoTaoBoiDuongAdd data)
        {
            try
            {
                if (data == null || string.IsNullOrEmpty(data.DaoTaoBoiDuong) || string.IsNullOrEmpty(data.DaoTaoBoiDuong_NoiDung))
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }

                var daoTaoBoiDuong = JsonConvert.DeserializeObject<DaoTaoBoiDuong>(data.DaoTaoBoiDuong);
                List<DaoTaoBoiDuong_NoiDung> noiDung = JsonConvert.DeserializeObject<List<DaoTaoBoiDuong_NoiDung>>(data.DaoTaoBoiDuong_NoiDung);


                var result = await Mediator.Send(new Application.DaoTaoBoiDuong.ThemMoiChinhSua.Command { DaoTaoBoiDuong = daoTaoBoiDuong, NoiDung = noiDung });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("ChinhSua/{id}")]
        public async Task<IActionResult> ChinhSua(Guid id, [FromForm] DaoTaoBoiDuongAdd data)
        {
            try
            {
                if (id == Guid.Empty || data == null || string.IsNullOrEmpty(data.DaoTaoBoiDuong) || string.IsNullOrEmpty(data.DaoTaoBoiDuong_NoiDung))
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }

                var daoTaoBoiDuong = JsonConvert.DeserializeObject<DaoTaoBoiDuong>(data.DaoTaoBoiDuong);
                List<DaoTaoBoiDuong_NoiDung> noiDung = JsonConvert.DeserializeObject<List<DaoTaoBoiDuong_NoiDung>>(data.DaoTaoBoiDuong_NoiDung);
                daoTaoBoiDuong.DaoTaoID = id;

                var result = await Mediator.Send(new Application.DaoTaoBoiDuong.ThemMoiChinhSua.Command { DaoTaoBoiDuong = daoTaoBoiDuong, NoiDung = noiDung });
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
            try
            {
                var result = await Mediator.Send(new Application.DaoTaoBoiDuong.Xoa.Command { ID = id });
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý");
            }
        }
        [HttpGet]
        [Route("BanDich/{daoTaoID}/{maNgonNgu?}")]
        public async Task<IActionResult> BanDich(Guid daoTaoID, string maNgonNgu = null)
        {
            try
            {
                if (daoTaoID == Guid.Empty)
                {
                    return BadRequest("ID không hợp lệ");
                }

                var result = await Mediator.Send(new Application.DaoTaoBoiDuong.DanhSachBanDich.Query
                {
                    DaoTaoID = daoTaoID,
                    MaNgonNgu = maNgonNgu
                });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý: " + ex.Message);
            }
        }
    }
}
