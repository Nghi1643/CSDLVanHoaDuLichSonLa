using CSDLVanHoaDuLichSonLa.Models;
using CSDLVanHoaDuLichSonLa.Services;
using Domain;
using Domain.BaoChi;
using Domain.Core;
using Domain.DanhMuc;
using Domain.SuKienHoatDong;
using Domain.ToChuc;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CSDLVanHoaDuLichSonLa.Controllers.API
{
    public class SuKienHoatDongApiController : BaseApiController
    {
        const string UploadPath = "uploads/sukienhoatdong/";
        public SuKienHoatDongApiController(IWebHostEnvironment hostingEnvironment, IConfiguration config) : base(hostingEnvironment, config)
        {

        }

        [HttpPost]
        [Route("DanhSach")]
        public async Task<IActionResult> DanhSach(SuKienHoatDongRequest data)
        {
            data.MaNgonNgu = "vi";
            var result = await Mediator.Send(new Application.SuKienHoatDong.DanhSach.Query { Data = data });
            return Ok(result);
        }

        [HttpPost]
        [Route("ThemMoi")]
        public async Task<IActionResult> ThemMoi(SuKienHoatDongAdd data)
        {
            try
            {
                if (data == null || string.IsNullOrEmpty(data.SuKienHoatDong) || string.IsNullOrEmpty(data.SuKienHoatDong_NoiDung))
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }

                DiaDiem SuKien = JsonConvert.DeserializeObject<DiaDiem>(data.SuKienHoatDong);
                List<DiaDiem_noiDung> banDich = JsonConvert.DeserializeObject<List<DiaDiem_noiDung>>(data.SuKienHoatDong_NoiDung);

                if (data.FileDinhKem != null)
                {
                    var file = data.FileDinhKem;

                    UploadFileResult ufile = await SaveFileUpload(file, UploadPath);
                    if (ufile.Success == false)
                    {
                        return BadRequest(ufile.Message);
                    }

                    SuKien.AnhDaiDien = ufile.Url;
                }

                var result = await Mediator.Send(new Application.SuKienHoatDong.ThemMoiChinhSua.Command { SuKien = SuKien, NoiDungBanDich = banDich });
                return Ok(result);
            }
            catch (Exception)
            {
                //return BadRequest(ex.Message);
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý");
            }
        }
       

        [HttpPut]
        [Route("ChinhSua/{suKienID}")]
        public async Task<IActionResult> ChinhSua(Guid suKienID, [FromBody] SuKienHoatDongAdd data)
        {

            try
            {
                if (suKienID == Guid.Empty || data == null || string.IsNullOrEmpty(data.SuKienHoatDong) || string.IsNullOrEmpty(data.SuKienHoatDong_NoiDung))
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }

                DiaDiem SuKien = JsonConvert.DeserializeObject<DiaDiem>(data.SuKienHoatDong);
                List<DiaDiem_noiDung> banDich = JsonConvert.DeserializeObject<List<DiaDiem_noiDung>>(data.SuKienHoatDong_NoiDung);

                SuKien.SuKienID = suKienID;

                if (data.FileDinhKem != null)
                {
                    var file = data.FileDinhKem;

                    UploadFileResult ufile = await SaveFileUpload(file, UploadPath);
                    if (ufile.Success == false)
                    {
                        return BadRequest(ufile.Message);
                    }

                    if (!string.IsNullOrEmpty(SuKien.AnhDaiDien))
                    {
                        DeleteFileUpload(SuKien.AnhDaiDien);
                    }

                    SuKien.AnhDaiDien = ufile.Url;
                }

                var result = await Mediator.Send(new Application.SuKienHoatDong.ThemMoiChinhSua.Command { SuKien = SuKien, NoiDungBanDich = banDich });
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
            var result = await Mediator.Send(new Application.SuKienHoatDong.Xoa.Command { ID = id });

            return Ok(result);
        }
    }
}
