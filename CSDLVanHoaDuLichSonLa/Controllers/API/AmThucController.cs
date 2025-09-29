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
    public class AmThucApiController : BaseApiController
    {
        const string UploadPath = "uploads/VanHoa/AmThuc";
        public AmThucApiController(IWebHostEnvironment hostingEnvironment, IConfiguration config) : base(hostingEnvironment, config)
        {
        }

        [HttpPost]
        [Route("DanhSach")]
        public async Task<IActionResult> DanhSach(AmThucRequest data)
        {
            try
            {
                var result = await Mediator.Send(new Application.VanHoa.VHAmThuc.DanhSach.Query { Data = data });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("ThemMoi")]
        public async Task<IActionResult> ThemMoi([FromForm] AmThucJson data)
        {
            try
            {
                if (data == null || string.IsNullOrEmpty(data.AmThuc) || string.IsNullOrEmpty(data.AmThuc_NoiDung))
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }

                AmThucAdd amThuc = JsonConvert.DeserializeObject<AmThucAdd>(data.AmThuc);
                List<AmThuc_NoiDungAdd> banDich = JsonConvert.DeserializeObject<List<AmThuc_NoiDungAdd>>(data.AmThuc_NoiDung);

                if (data.AnhDaiDien != null)
                {
                    var file = data.AnhDaiDien;

                    UploadFileResult ufile = await SaveFileUpload(file, UploadPath);
                    if (ufile.Success == false)
                    {
                        return BadRequest(ufile.Message);
                    }

                    amThuc.AnhDaiDien = ufile.Url;
                }

                var result = await Mediator.Send(new Application.VanHoa.VHAmThuc.ThemMoiChinhSua.Command { Data = amThuc, NoiDungBanDich = banDich });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("ChinhSua/{id}")]
        public async Task<IActionResult> ChinhSua(Guid id, [FromForm] AmThucJson data)
        {
            try
            {
                if (id == Guid.Empty || data == null || string.IsNullOrEmpty(data.AmThuc) || string.IsNullOrEmpty(data.AmThuc_NoiDung))
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }

                AmThucAdd amThuc = JsonConvert.DeserializeObject<AmThucAdd>(data.AmThuc);
                List<AmThuc_NoiDungAdd> banDich = JsonConvert.DeserializeObject<List<AmThuc_NoiDungAdd>>(data.AmThuc_NoiDung);

                //if (data.AnhDaiDien != null)
                //{
                //    var file = data.AnhDaiDien;

                //    UploadFileResult ufile = await SaveFileUpload(file, UploadPath);
                //    if (ufile.Success == false)
                //    {
                //        return BadRequest(ufile.Message);
                //    }

                //    if (!string.IsNullOrEmpty(phiVatThe.AnhDaiDien))
                //    {
                //        DeleteFileUpload(phiVatThe.AnhDaiDien);
                //    }

                //    phiVatThe.AnhDaiDien = ufile.Url;
                //}

                amThuc.MonAnUongID = id;
                var result = await Mediator.Send(new Application.VanHoa.VHAmThuc.ThemMoiChinhSua.Command { Data = amThuc, NoiDungBanDich = banDich });
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
                var result = await Mediator.Send(new Application.VanHoa.VHAmThuc.Xoa.Command { ID = id });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý" + ex.Message);
            }
        }

        [HttpGet]
        [Route("ChiTiet/{monAnUongID}")]
        public async Task<IActionResult> ChiTiet(Guid monAnUongID)
        {
            try
            {
                var result = await Mediator.Send(new Application.VanHoa.VHAmThuc.ChiTiet.Query { MonAnUongID = monAnUongID });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý" + ex.Message);
            }
        }
    }
}
