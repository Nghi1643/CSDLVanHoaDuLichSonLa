using Domain.VanHoa;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Domain.DanhMuc;

namespace CSDLVanHoaDuLichSonLa.Controllers.API
{
    public class TrungTuDiTichApiController : BaseApiController
    {
        public TrungTuDiTichApiController(IWebHostEnvironment hostingEnvironment, IConfiguration config) : base(hostingEnvironment, config)
        {
        }

        [HttpPost]
        [Route("DanhSach")]
        public async Task<IActionResult> DanhSach(DiTich_TrungTu data)
        {
            try
            {
                var result = await Mediator.Send(new Application.VanHoa.TrungTuDiTich.DanhSach.Query { Data = data });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("ThemMoi")]
        public async Task<IActionResult> ThemMoi([FromForm] string data)
        {
            try
            {
                if (data == null )
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }

                List<DiTich_TrungTu> banDich = JsonConvert.DeserializeObject<List<DiTich_TrungTu>>(data);

                var result = await Mediator.Send(new Application.VanHoa.TrungTuDiTich.ThemMoiChinhSua.Command { Data = banDich });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[HttpPut]
        //[Route("ChinhSua/{id}")]
        //public async Task<IActionResult> ChinhSua(Guid id, [FromBody] DiTich_TrungTu data)
        //{
        //    try
        //    {
        //        data.DiTichID = id;
        //        var result = await Mediator.Send(new Application.VanHoa.TrungTuDiTich.ThemMoiChinhSua.Command { Data = data });
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        [HttpDelete]
        [Route("Xoa/{lanTrungTu}/{diTichId}")]
        public async Task<IActionResult> Xoa(int lanTrungTu, Guid diTichId)
        {
            try
            {
                var result = await Mediator.Send(new Application.VanHoa.TrungTuDiTich.Xoa.Command {LanTrungTu = lanTrungTu , DiTichID = diTichId});
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý" + ex.Message);
            }
        }

        //[HttpGet]
        //[Route("BanDich/{daPhuongTienID}/{maNgonNgu?}")]
        //public async Task<IActionResult> BanDich(Guid daPhuongTienID, string maNgonNgu)
        //{
        //    try
        //    {
        //        var result = await Mediator.Send(new Application.DanhMuc.DaPhuongTien.DanhSachBanDich.Query { DaPhuongTienID = daPhuongTienID, MaNgonNgu = null });
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest("Đã xảy ra lỗi trong quá trình xử lý" + ex.Message);
        //    }
        //}
    }
}
