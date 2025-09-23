using Azure.Core;
using Domain;
using Domain.BaoChi;
using Domain.Core;
using Domain.DanhMuc;
using Domain.VanHoa;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace CSDLVanHoaDuLichSonLa.Controllers.API
{
    public class DiTichApiController : BaseApiController
    {
        const string UploadPath = "uploads/VanHoa/DiTich";
        public DiTichApiController(IWebHostEnvironment hostingEnvironment, IConfiguration config) : base(hostingEnvironment, config)
        {
        }

        [HttpPost]
        [Route("DanhSach")]
        public async Task<IActionResult> DanhSach(DiTichRequest data)
        {
            try
            {
                var result = await Mediator.Send(new Application.VanHoa.DiTich.DanhSach.Query { Data = data });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("ThemMoi")]
        public async Task<IActionResult> ThemMoi([FromForm] DiTichAdd data)
        {
            try
            {
                if (data == null || string.IsNullOrEmpty(data.DiTich) || string.IsNullOrEmpty(data.DiTich_NoiDung))
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }

                DiTich diTich = JsonConvert.DeserializeObject<DiTich>(data.DiTich);
                List<DiTich_NoiDung> banDich = JsonConvert.DeserializeObject<List<DiTich_NoiDung>>(data.DiTich_NoiDung);

                if (data.AnhDaiDien != null)
                {
                    var file = data.AnhDaiDien;

                    UploadFileResult ufile = await SaveFileUpload(file, UploadPath);
                    if (ufile.Success == false)
                    {
                        return BadRequest(ufile.Message);
                    }

                    diTich.AnhDaiDien = ufile.Url;
                }

                var result = await Mediator.Send(new Application.VanHoa.DiTich.ThemMoiChinhSua.Command { Data = diTich, NoiDungBanDich = banDich });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("ChinhSua/{id}")]
        public async Task<IActionResult> ChinhSua(Guid id, [FromForm] DiTichAdd data)
        {
            try
            {
                if (id == Guid.Empty || data == null || string.IsNullOrEmpty(data.DiTich) || string.IsNullOrEmpty(data.DiTich_NoiDung))
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }

                DiTich diTich = JsonConvert.DeserializeObject<DiTich>(data.DiTich);
                List<DiTich_NoiDung> banDich = JsonConvert.DeserializeObject<List<DiTich_NoiDung>>(data.DiTich_NoiDung);
              
                if (data.AnhDaiDien != null)
                {
                    var file = data.AnhDaiDien;

                    UploadFileResult ufile = await SaveFileUpload(file, UploadPath);
                    if (ufile.Success == false)
                    {
                        return BadRequest(ufile.Message);
                    }

                    if (!string.IsNullOrEmpty(diTich.AnhDaiDien))
                    {
                        DeleteFileUpload(diTich.AnhDaiDien);
                    }

                    diTich.AnhDaiDien = ufile.Url;
                }
                diTich.DiTichID = id;
                
                var result = await Mediator.Send(new Application.VanHoa.DiTich.ThemMoiChinhSua.Command { Data = diTich, NoiDungBanDich = banDich });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                //return BadRequest("Đã xảy ra lỗi trong quá trình xử lý");
            }
        }

        [HttpDelete]
        [Route("Xoa/{id}")]
        public async Task<IActionResult> Xoa(Guid id)
        {
            try
            {
                var result = await Mediator.Send(new Application.VanHoa.DiTich.Xoa.Command { ID = id });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý" + ex.Message);
            }
        }

        [HttpGet]
        [Route("BanDich/{diTichID}/{maNgonNgu?}")]
        public async Task<IActionResult> BanDich(Guid diTichID, string maNgonNgu)
        {
            try
            {
                var result = await Mediator.Send(new Application.VanHoa.DiTich.DanhSachBanDich.Query { DiTichID = diTichID, MaNgonNgu = null});
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý" + ex.Message);
            }
        }
        [HttpGet]
        [Route("ChiTiet/{diTichID}")]
        public async Task<IActionResult> ChiTiet(Guid diTichID)
        {
            try
            {

                var result = await Mediator.Send(new Application.VanHoa.DiTich.ChiTiet.Query { DiTichID = diTichID });
                if(result != null && result.IsSuccess == true)
                {
                    var banDich = await Mediator.Send(new Application.VanHoa.DiTich.DanhSachBanDich.Query { DiTichID = diTichID, MaNgonNgu = null });
                    if(banDich != null && banDich.IsSuccess == true)
                    {
                        result.Value.BanDich = banDich.Value;
                    }
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý" + ex.Message);
            }
        }
    }
}
