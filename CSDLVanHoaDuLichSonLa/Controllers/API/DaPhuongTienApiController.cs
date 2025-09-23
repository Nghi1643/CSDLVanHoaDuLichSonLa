using Domain.VanHoa;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Domain.DanhMuc;
using Application.DaPhuongTien;

namespace CSDLVanHoaDuLichSonLa.Controllers.API
{
    public class DaPhuongTienApiController : BaseApiController
    {
        const string UploadPath = "uploads/DaPhuongTien";
        public DaPhuongTienApiController(IWebHostEnvironment hostingEnvironment, IConfiguration config) : base(hostingEnvironment, config)
        {
        }

        [HttpPost]
        [Route("DanhSach")]
        public async Task<IActionResult> DanhSach(DaPhuongTien data)
        {
            try
            {
                var result = await Mediator.Send(new DanhSach.Query { Data = data });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("ThemMoi")]
        public async Task<IActionResult> ThemMoi([FromForm] DaPhuongTienAdd data)
        {
            try
            {
                if (data == null || string.IsNullOrEmpty(data.DaPhuongTien) || string.IsNullOrEmpty(data.DaPhuongTien_NoiDung))
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }

                DaPhuongTien daPhuongTien = JsonConvert.DeserializeObject<DaPhuongTien>(data.DaPhuongTien);
                List<DaPhuongTien_NoiDung> banDich = JsonConvert.DeserializeObject<List<DaPhuongTien_NoiDung>>(data.DaPhuongTien_NoiDung);

                if (data.File != null)
                {
                    var file = data.File;

                    UploadFileResult ufile = await SaveFileUpload(file, UploadPath);
                    if (ufile.Success == false)
                    {
                        return BadRequest(ufile.Message);
                    }

                    daPhuongTien.DuongDanFile = ufile.Url;
                }

                var result = await Mediator.Send(new ThemMoiChinhSua.Command { Data = daPhuongTien, NoiDungBanDich = banDich });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("ChinhSua/{id}")]
        public async Task<IActionResult> ChinhSua(Guid id, [FromForm] DaPhuongTienAdd data)
        {
            try
            {
                if (data == null || string.IsNullOrEmpty(data.DaPhuongTien) || string.IsNullOrEmpty(data.DaPhuongTien_NoiDung))
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }

                DaPhuongTien daPhuongTien = JsonConvert.DeserializeObject<DaPhuongTien>(data.DaPhuongTien);
                List<DaPhuongTien_NoiDung> banDich = JsonConvert.DeserializeObject<List<DaPhuongTien_NoiDung>>(data.DaPhuongTien_NoiDung);

                if (data.File != null)
                {
                    var file = data.File;

                    UploadFileResult ufile = await SaveFileUpload(file, UploadPath);
                    if (ufile.Success == false)
                    {
                        return BadRequest(ufile.Message);
                    }

                    if (!string.IsNullOrEmpty(daPhuongTien.DuongDanFile))
                    {
                        DeleteFileUpload(daPhuongTien.DuongDanFile);
                    }

                    daPhuongTien.DuongDanFile = ufile.Url;
                }
                daPhuongTien.DaPhuongTienID = id;
                var result = await Mediator.Send(new ThemMoiChinhSua.Command { Data = daPhuongTien, NoiDungBanDich = banDich });
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
                var result = await Mediator.Send(new Xoa.Command { ID = id });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý" + ex.Message);
            }
        }

        [HttpGet]
        [Route("BanDich/{daPhuongTienID}/{maNgonNgu?}")]
        public async Task<IActionResult> BanDich(Guid daPhuongTienID, string maNgonNgu)
        {
            try
            {
                var result = await Mediator.Send(new DanhSachBanDich.Query { DaPhuongTienID = daPhuongTienID, MaNgonNgu = null });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý" + ex.Message);
            }
        }
    }
}
