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
    public class HienVatApiController : BaseApiController
    {
        const string UploadPath = "uploads/VanHoa/HienVat";
        public HienVatApiController(IWebHostEnvironment hostingEnvironment, IConfiguration config) : base(hostingEnvironment, config)
        {
        }

        [HttpPost]
        [Route("DanhSach")]
        public async Task<IActionResult> DanhSach(HienVatRequest data)
        {
            try
            {
                var result = await Mediator.Send(new Application.VanHoa.HienVat.DanhSach.Query { Data = data });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("ThemMoi")]
        public async Task<IActionResult> ThemMoi([FromForm] HienVatJson data)
        {
            try
            {
                if (data == null || string.IsNullOrEmpty(data.HienVat) || string.IsNullOrEmpty(data.HienVat_NoiDung))
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }

                HienVatAdd hienVat = JsonConvert.DeserializeObject<HienVatAdd>(data.HienVat);
                List<HienVat_NoiDungAdd> banDich = JsonConvert.DeserializeObject<List<HienVat_NoiDungAdd>>(data.HienVat_NoiDung);

                if (data.AnhDaiDien != null)
                {
                    var file = data.AnhDaiDien;

                    UploadFileResult ufile = await SaveFileUpload(file, UploadPath);
                    if (ufile.Success == false)
                    {
                        return BadRequest(ufile.Message);
                    }

                    hienVat.AnhDaiDien = ufile.Url;
                }

                var result = await Mediator.Send(new Application.VanHoa.HienVat.ThemMoiChinhSua.Command { Data = hienVat, NoiDungBanDich = banDich });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("ChinhSua/{id}")]
        public async Task<IActionResult> ChinhSua(Guid id, [FromForm] HienVatJson data)
        {
            try
            {
                if (id == Guid.Empty || data == null || string.IsNullOrEmpty(data.HienVat) || string.IsNullOrEmpty(data.HienVat_NoiDung))
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }

                HienVatAdd diTich = JsonConvert.DeserializeObject<HienVatAdd>(data.HienVat);
                List<HienVat_NoiDungAdd> banDich = JsonConvert.DeserializeObject<List<HienVat_NoiDungAdd>>(data.HienVat_NoiDung);
              
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
                diTich.HienVatID = id;
                
                var result = await Mediator.Send(new Application.VanHoa.HienVat.ThemMoiChinhSua.Command { Data = diTich, NoiDungBanDich = banDich });
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
                var result = await Mediator.Send(new Application.VanHoa.HienVat.Xoa.Command { ID = id });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý" + ex.Message);
            }
        }

        [HttpGet]
        [Route("BanDich/{hienVatID}/{maNgonNgu?}")]
        public async Task<IActionResult> BanDich(Guid hienVatID, string maNgonNgu)
        {
            try
            {
                var result = await Mediator.Send(new Application.VanHoa.HienVat.DanhSachBanDich.Query { HienVatID = hienVatID, MaNgonNgu = null});
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý" + ex.Message);
            }
        }
        [HttpGet]
        [Route("ChiTiet/{hienVatID}")]
        public async Task<IActionResult> ChiTiet(Guid hienVatID)
        {
            try
            {
                var result = await Mediator.Send(new Application.VanHoa.HienVat.ChiTiet.Query { HienVatID = hienVatID });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý" + ex.Message);
            }
        }
    }
}
