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
    public class XuatBanAnPhamApiController : BaseApiController
    {
        const string UploadPath = "uploads/baochi/";
        public XuatBanAnPhamApiController(IWebHostEnvironment hostingEnvironment, IConfiguration config) : base(hostingEnvironment, config)
        {
        }

        [HttpPost]
        [Route("DanhSach")]
        public async Task<IActionResult> DanhSach(XuatBanAnPhamRequest data)
        {
            try
            {
                var result = await Mediator.Send(new Application.BaoChi.XuatBanAnPham.DanhSach.Query { Data = data });
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý");
            }
        }

        [HttpPost]
        [Route("ThemMoi")]
        public async Task<IActionResult> ThemMoi([FromForm] XuatBanAnPhamAdd data)
        {
            try
            {
                if (data == null || string.IsNullOrEmpty(data.XuatBanAnPham) || string.IsNullOrEmpty(data.XuatBanAnPham_NoiDung))
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }

                XuatBanAnPham xbAnPham = JsonConvert.DeserializeObject<XuatBanAnPham>(data.XuatBanAnPham);
                List<XuatBanAnPham_NoiDung> banDich = JsonConvert.DeserializeObject<List<XuatBanAnPham_NoiDung>>(data.XuatBanAnPham_NoiDung);

                if (data.FileDinhKem != null)
                {
                    var file = data.FileDinhKem;

                    UploadFileResult ufile = await SaveFileUpload(file, UploadPath);
                    if (ufile.Success == false)
                    {
                        return BadRequest(ufile.Message);
                    }

                    xbAnPham.DuongDanHinhAnhBia = ufile.Url;
                }

                var result = await Mediator.Send(new Application.BaoChi.XuatBanAnPham.ThemMoiChinhSua.Command { XBAnPham = xbAnPham, NoiDungBanDich = banDich });
                return Ok(result);
            }
            catch (Exception)
            {
                //return BadRequest(ex.Message);
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý");
            }
        }

        [HttpPut]
        [Route("ChinhSua/{id}")]
        public async Task<IActionResult> ChinhSua(Guid id, [FromForm] XuatBanAnPhamAdd data)
        {
            try
            {
                if (id == Guid.Empty || data == null || string.IsNullOrEmpty(data.XuatBanAnPham) || string.IsNullOrEmpty(data.XuatBanAnPham_NoiDung))
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }

                XuatBanAnPham xbAnPham = JsonConvert.DeserializeObject<XuatBanAnPham>(data.XuatBanAnPham);
                List<XuatBanAnPham_NoiDung> banDich = JsonConvert.DeserializeObject<List<XuatBanAnPham_NoiDung>>(data.XuatBanAnPham_NoiDung);

                xbAnPham.XuatBanAnPhamID = id;
                
                if (data.FileDinhKem != null)
                {
                    var file = data.FileDinhKem;

                    UploadFileResult ufile = await SaveFileUpload(file, UploadPath);
                    if (ufile.Success == false)
                    {
                        return BadRequest(ufile.Message);
                    }

                    if (!string.IsNullOrEmpty(xbAnPham.DuongDanHinhAnhBia))
                    {
                        DeleteFileUpload(xbAnPham.DuongDanHinhAnhBia);
                    }

                    xbAnPham.DuongDanHinhAnhBia = ufile.Url;
                }

                var result = await Mediator.Send(new Application.BaoChi.XuatBanAnPham.ThemMoiChinhSua.Command { XBAnPham = xbAnPham, NoiDungBanDich = banDich });
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
                var result = await Mediator.Send(new Application.BaoChi.XuatBanAnPham.Xoa.Command { ID = id });
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý");
            }
        }

        [HttpGet]
        [Route("BanDich/{xuatBanAnPhamID}/{maNgonNgu?}")]
        public async Task<IActionResult> BanDich(Guid xuatBanAnPhamID, string maNgonNgu)
        {
            try
            {
                var result = await Mediator.Send(new Application.BaoChi.XuatBanAnPham.DanhSachBanDich.Query { XuatBanAnPhamID = xuatBanAnPhamID, MaNgonNgu = null});
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý");
            }
        }
    }
}
