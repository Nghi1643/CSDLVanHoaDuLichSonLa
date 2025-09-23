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
    public class DiSanPhiVatTheApiController : BaseApiController
    {
        const string UploadPath = "uploads/VanHoa/DiSanPhiVatThe";
        public DiSanPhiVatTheApiController(IWebHostEnvironment hostingEnvironment, IConfiguration config) : base(hostingEnvironment, config)
        {
        }

        [HttpPost]
        [Route("DanhSach")]
        public async Task<IActionResult> DanhSach(DiSanPhiVatTheRequest data)
        {
            try
            {
                var result = await Mediator.Send( new Application.VanHoa.PhiVatThe.DanhSach.Query { Data = data });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("ThemMoi")]
        public async Task<IActionResult> ThemMoi([FromForm] DiSanPhiVatTheJson data)
        {
            try
            {
                if (data == null || string.IsNullOrEmpty(data.PhiVatThe) || string.IsNullOrEmpty(data.PhiVatThe_NoiDung))
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }

                DiSanPhiVatTheAdd phiVatThe = JsonConvert.DeserializeObject<DiSanPhiVatTheAdd>(data.PhiVatThe);
                List<DiSanPhiVatThe_NoiDungAdd> banDich = JsonConvert.DeserializeObject<List<DiSanPhiVatThe_NoiDungAdd>>(data.PhiVatThe_NoiDung);

                if (data.AnhDaiDien != null)
                {
                    var file = data.AnhDaiDien;

                    UploadFileResult ufile = await SaveFileUpload(file, UploadPath);
                    if (ufile.Success == false)
                    {
                        return BadRequest(ufile.Message);
                    }

                    phiVatThe.AnhDaiDien = ufile.Url;
                }

                var result = await Mediator.Send(new Application.VanHoa.PhiVatThe.ThemMoiChinhSua.Command { Data = phiVatThe, NoiDungBanDich = banDich });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("ChinhSua/{id}")]
        public async Task<IActionResult> ChinhSua(Guid id, [FromForm] DiSanPhiVatTheJson data)
        {
            try
            {
                if (id == Guid.Empty || data == null || string.IsNullOrEmpty(data.PhiVatThe) || string.IsNullOrEmpty(data.PhiVatThe_NoiDung))
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }

                DiSanPhiVatTheAdd phiVatThe = JsonConvert.DeserializeObject<DiSanPhiVatTheAdd>(data.PhiVatThe);
                List<DiSanPhiVatThe_NoiDungAdd> banDich = JsonConvert.DeserializeObject<List<DiSanPhiVatThe_NoiDungAdd>>(data.PhiVatThe_NoiDung);

                if (data.AnhDaiDien != null)
                {
                    var file = data.AnhDaiDien;

                    UploadFileResult ufile = await SaveFileUpload(file, UploadPath);
                    if (ufile.Success == false)
                    {
                        return BadRequest(ufile.Message);
                    }

                    if (!string.IsNullOrEmpty(phiVatThe.AnhDaiDien))
                    {
                        DeleteFileUpload(phiVatThe.AnhDaiDien);
                    }

                    phiVatThe.AnhDaiDien = ufile.Url;
                }
                phiVatThe.DiSanID = id;
                var result = await Mediator.Send(new Application.VanHoa.PhiVatThe.ThemMoiChinhSua.Command { Data = phiVatThe, NoiDungBanDich = banDich });
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
                var result = await Mediator.Send(new Application.VanHoa.PhiVatThe.Xoa.Command { ID = id });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý" + ex.Message);
            }
        }

        [HttpGet]
        [Route("ChiTiet/{diSanID}")]
        public async Task<IActionResult> ChiTiet(Guid diSanID)
        {
            try
            {
                var result = await Mediator.Send(new Application.VanHoa.PhiVatThe.ChiTiet.Query { DiSanID = diSanID });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý" + ex.Message);
            }
        }
    }
}
