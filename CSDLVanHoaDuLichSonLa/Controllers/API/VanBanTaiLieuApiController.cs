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
    public class VanBanTaiLieuApiController : BaseApiController
    {
        const string UploadPath = "uploads/VanHoa/DiTich";
        public VanBanTaiLieuApiController(IWebHostEnvironment hostingEnvironment, IConfiguration config) : base(hostingEnvironment, config)
        {
        }

        [HttpPost]
        [Route("DanhSach")]
        public async Task<IActionResult> DanhSach(VanBanTaiLieuRequest data)
        {
            try
            {
                var result = await Mediator.Send(new Application.VanBanTaiLieu.DanhSach.Query { Data = data });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Đang chỉ thực hiện thêm 1 file duy nhất
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ThemMoi")]
        public async Task<IActionResult> ThemMoi([FromForm] VanBanTaiLieuAdd data)
        {
            try
            {
                if (data == null || data.File == null || !data.File.Any())
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }

                if (data.File != null && data.File.Any())
                {
                    var fFile = data.File.FirstOrDefault();

                    UploadFileResult ufile = await SaveFileUpload(fFile, UploadPath);
                    if (ufile.Success == false)
                    {
                        return BadRequest(ufile.Message);
                    }
                    data.DuongDanFile = ufile.Url;
                }
                var result = await Mediator.Send(new Application.VanBanTaiLieu.ThemMoiChinhSua.Command { vbtl = data });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// Đang chỉ thêm 1 file duy nhất
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("ChinhSua/{id}")]
        public async Task<IActionResult> ChinhSua(Guid id, [FromForm] VanBanTaiLieuAdd data)
        {
            try
            {
                if (data == null)
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }

                if(data.File != null && data.File.Any())
                {
                    var fFile = data.File.FirstOrDefault();

                    UploadFileResult ufile = await SaveFileUpload(fFile, UploadPath);
                    if (ufile.Success == false)
                    {
                        return BadRequest(ufile.Message);
                    }
                    data.DuongDanFile = ufile.Url;
                   
                }
                var result = await Mediator.Send(new Application.VanBanTaiLieu.ThemMoiChinhSua.Command { vbtl = data });
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
                var result = await Mediator.Send(new Application.VanBanTaiLieu.Xoa.Command { ID = id });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý" + ex.Message);
            }
        }
    }
}
