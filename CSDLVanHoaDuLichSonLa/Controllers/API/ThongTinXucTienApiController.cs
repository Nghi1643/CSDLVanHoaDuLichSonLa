using Domain.CoSoKinhDoanhDichVuDuLich;
using Domain.ThongTinXucTien;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CSDLVanHoaDuLichSonLa.Controllers.API
{

    public class ThongTinXucTienApiController : BaseApiController
    {
        public ThongTinXucTienApiController(IWebHostEnvironment hostingEnvironment, IConfiguration config) : base(hostingEnvironment, config)
        {
        }
        [HttpPost("DanhSach")]
        public async Task<IActionResult> DanhSach([FromBody] ThongTinXucTienRequest data)
        {
            try
            {
                data.MaNgonNgu = "vi";
                var result = await Mediator.Send(new Application.ThongTinXucTien.DanhSach.Query { Data = data });
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
                var result = await Mediator.Send(new Application.ThongTinXucTien.ChiTiet.Query { XucTienID = id });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("ThemMoi")]
        public async Task<IActionResult> ThemMoi([FromForm] ThongTinXucTienAdd data)
        {
            try
            {
                if (data == null || string.IsNullOrEmpty(data.ThongTinXucTien) || string.IsNullOrEmpty(data.ThongTinXucTien_NoiDung))
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }

                var thongTinXucTien= JsonConvert.DeserializeObject<ThongTinXucTien>(data.ThongTinXucTien);
                List<ThongTinXucTien_NoiDung> noiDung = JsonConvert.DeserializeObject<List<ThongTinXucTien_NoiDung>>(data.ThongTinXucTien_NoiDung);


                var result = await Mediator.Send(new Application.ThongTinXucTien.ThemMoiChinhSua.Command { ThongTinXucTien = thongTinXucTien, NoiDung = noiDung });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("ChinhSua/{id}")]
        public async Task<IActionResult> ChinhSua(Guid id, [FromForm] ThongTinXucTienAdd data)
        {
            try
            {
                if (id == Guid.Empty || data == null || string.IsNullOrEmpty(data.ThongTinXucTien) || string.IsNullOrEmpty(data.ThongTinXucTien_NoiDung))
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }

                var thongTinXucTien = JsonConvert.DeserializeObject<ThongTinXucTien>(data.ThongTinXucTien);
                List<ThongTinXucTien_NoiDung> noiDung = JsonConvert.DeserializeObject<List<ThongTinXucTien_NoiDung>>(data.ThongTinXucTien_NoiDung);
                thongTinXucTien.XucTienID = id;

                var result = await Mediator.Send(new Application.ThongTinXucTien.ThemMoiChinhSua.Command { ThongTinXucTien = thongTinXucTien, NoiDung = noiDung });
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
                var result = await Mediator.Send(new Application.ThongTinXucTien.Xoa.Command { ID = id });
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý");
            }
        }
        [HttpGet]
        [Route("BanDich/{xucTienID}/{maNgonNgu?}")]
        public async Task<IActionResult> BanDich(Guid xucTienID, string maNgonNgu = null)
        {
            try
            {
                if (xucTienID == Guid.Empty)
                {
                    return BadRequest("ID không hợp lệ");
                }

                var result = await Mediator.Send(new Application.ThongTinXucTien.DanhSachBanDich.Query
                {
                    XucTienID = xucTienID,
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
