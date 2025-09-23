using Azure.Core;
using Domain;
using Domain.BaoChi;
using Domain.Core;
using Domain.DanhMuc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace CSDLVanHoaDuLichSonLa.Controllers.API
{
    public class BaoChiAnPhamApiController : BaseApiController
    {
        public BaoChiAnPhamApiController(IWebHostEnvironment hostingEnvironment, IConfiguration config) : base(hostingEnvironment, config)
        {
        }

        [HttpPost]
        [Route("DanhSach")]
        public async Task<IActionResult> DanhSach(BaoChiAnPhamRequest data)
        {
            try
            {
                var result = await Mediator.Send(new Application.BaoChi.BaoChiAnPham.DanhSach.Query { Data = data });
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý");
            }
        }

        [HttpPost]
        [Route("ThemMoi")]
        public async Task<IActionResult> ThemMoi([FromForm] BaoChiAnPhamAdd data)
        {
            try
            {
                if (data == null || string.IsNullOrEmpty(data.BaoChiAnPham) || string.IsNullOrEmpty(data.BaoChiAnPham_NoiDung))
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }

                BaoChiAnPham bcAnPham = JsonConvert.DeserializeObject<BaoChiAnPham>(data.BaoChiAnPham);
                List<BaoChiAnPham_NoiDung> banDich = JsonConvert.DeserializeObject<List<BaoChiAnPham_NoiDung>>(data.BaoChiAnPham_NoiDung);


                var result = await Mediator.Send(new Application.BaoChi.BaoChiAnPham.ThemMoiChinhSua.Command { BCAnPham = bcAnPham, NoiDungBanDich = banDich });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("ChinhSua/{id}")]
        public async Task<IActionResult> ChinhSua(Guid id, [FromForm] BaoChiAnPhamAdd data)
        {
            try
            {
                if (id == Guid.Empty || data == null || string.IsNullOrEmpty(data.BaoChiAnPham) || string.IsNullOrEmpty(data.BaoChiAnPham_NoiDung))
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }

                BaoChiAnPham bcAnPham = JsonConvert.DeserializeObject<BaoChiAnPham>(data.BaoChiAnPham);
                List<BaoChiAnPham_NoiDung> banDich = JsonConvert.DeserializeObject<List<BaoChiAnPham_NoiDung>>(data.BaoChiAnPham_NoiDung);

                bcAnPham.BaoChiAnPhamID = id;
                
                var result = await Mediator.Send(new Application.BaoChi.BaoChiAnPham.ThemMoiChinhSua.Command { BCAnPham = bcAnPham, NoiDungBanDich = banDich });
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
                var result = await Mediator.Send(new Application.BaoChi.BaoChiAnPham.Xoa.Command { ID = id });
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý");
            }
        }

        [HttpGet]
        [Route("BanDich/{baoChiAnPhamID}/{maNgonNgu?}")]
        public async Task<IActionResult> BanDich(Guid baoChiAnPhamID, string maNgonNgu)
        {
            try
            {
                var result = await Mediator.Send(new Application.BaoChi.BaoChiAnPham.DanhSachBanDich.Query { BaoChiAnPhamID = baoChiAnPhamID, MaNgonNgu = null});
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý");
            }
        }
    }
}
