using Application.TacPham;
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
    public class TacPhamApiController : BaseApiController
    {
        const string UploadPath = "uploads/TacPham";
        public TacPhamApiController(IWebHostEnvironment hostingEnvironment, IConfiguration config) : base(hostingEnvironment, config)
        {
        }

        #region TÁC PHẨM
        [HttpPost]
        [Route("DanhSach")]
        public async Task<IActionResult> DanhSach(TacPhamRequest data)
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
        public async Task<IActionResult> ThemMoi([FromForm] TacPhamJson data)
        {
            try
            {
                if (data == null || string.IsNullOrEmpty(data.TacPham) || string.IsNullOrEmpty(data.TacPham_NoiDung))
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }

                TacPhamAdd tacPham = JsonConvert.DeserializeObject<TacPhamAdd>(data.TacPham);
                List<TacPham_NoiDungAdd> banDich = JsonConvert.DeserializeObject<List<TacPham_NoiDungAdd>>(data.TacPham_NoiDung);

                if (data.HinhAnh != null)
                {
                    var file = data.HinhAnh;

                    UploadFileResult ufile = await SaveFileUpload(file, UploadPath);
                    if (ufile.Success == false)
                    {
                        return BadRequest(ufile.Message);
                    }

                    tacPham.HinhAnh = ufile.Url;
                }

                var result = await Mediator.Send(new ThemMoiChinhSua.Command { Data = tacPham, BanDich = banDich });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("ChinhSua/{id}")]
        public async Task<IActionResult> ChinhSua(Guid id, [FromForm] TacPhamJson data)
        {
            try
            {
                if (id == Guid.Empty || data == null || string.IsNullOrEmpty(data.TacPham) || string.IsNullOrEmpty(data.TacPham_NoiDung))
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }

                TacPhamAdd tacPhamNgheThuat = JsonConvert.DeserializeObject<TacPhamAdd>(data.TacPham);
                List<TacPham_NoiDungAdd> banDich = JsonConvert.DeserializeObject<List<TacPham_NoiDungAdd>>(data.TacPham_NoiDung);

                if (data.HinhAnh != null)
                {
                    var file = data.HinhAnh;

                    UploadFileResult ufile = await SaveFileUpload(file, UploadPath);
                    if (ufile.Success == false)
                    {
                        return BadRequest(ufile.Message);
                    }

                    if (!string.IsNullOrEmpty(tacPhamNgheThuat.HinhAnh))
                    {
                        DeleteFileUpload(tacPhamNgheThuat.HinhAnh);
                    }

                    tacPhamNgheThuat.HinhAnh = ufile.Url;
                }

                tacPhamNgheThuat.TacPhamID = id;
                var result = await Mediator.Send(new ThemMoiChinhSua.Command { Data = tacPhamNgheThuat, BanDich = banDich });
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
        [Route("ChiTiet/{tacPhamID}")]
        public async Task<IActionResult> ChiTiet(Guid tacPhamID)
        {
            try
            {
                var result = await Mediator.Send(new ChiTiet.Query { TacPhamID = tacPhamID });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý" + ex.Message);
            }
        }

        #endregion
    }
}
