using CSDLVanHoaDuLichSonLa.Models;
using CSDLVanHoaDuLichSonLa.Services;
using Domain;
using Domain.BaoChi;
using Domain.CaNhan;
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
    public class CaNhan_BaoChiApiController : BaseApiController
    {
        const string UploadPath = "uploads/canhan/";
        public CaNhan_BaoChiApiController(IWebHostEnvironment hostingEnvironment, IConfiguration config) : base(hostingEnvironment, config)
        {

        }

        [HttpPost]
        [Route("DanhSach")]
        public async Task<IActionResult> DanhSach(CaNhanRequest data)
        {
            data.MaNgonNgu = "vi";
            var result = await Mediator.Send(new Application.CaNhan.CaNhan.DanhSach.Query { Data = data });
            return Ok(result);
        }

        [HttpPost]
        [Route("ThemMoi")]
        public async Task<IActionResult> ThemMoi(CaNhan_BaoChi_Add data)
        {
            try
            {
                if (data == null 
                   || string.IsNullOrEmpty(data.CaNhan) 
                   || string.IsNullOrEmpty(data.CaNhan_NoiDung)
                   || string.IsNullOrEmpty(data.CaNhan_BaoChi)
                   || string.IsNullOrEmpty(data.CaNhan_BaoChi_NoiDung)
                   )
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }
                VanBan caNhan = JsonConvert.DeserializeObject<VanBan>(data.CaNhan);
                List<CaNhan_NoiDung> caNhan_NoiDung = JsonConvert.DeserializeObject<List<CaNhan_NoiDung>>(data.CaNhan_NoiDung);
                CaNhan_BaoChi caNhan_BaoChi = JsonConvert.DeserializeObject<CaNhan_BaoChi>(data.CaNhan_BaoChi);
                List<CaNhan_BaoChi_NoiDung> caNhan_BaoChi_NoiDung = JsonConvert.DeserializeObject<List<CaNhan_BaoChi_NoiDung>>(data.CaNhan_BaoChi_NoiDung);
                if (data.FileDinhKem != null)
                {
                    var file = data.FileDinhKem;

                    UploadFileResult ufile = await SaveFileUpload(file, UploadPath);
                    if (ufile.Success == false)
                    {
                        return BadRequest(ufile.Message);
                    }

                    caNhan.AnhChanDung = ufile.Url;
                }
                var result_cn = await Mediator.Send(new Application.CaNhan.CaNhan.ThemMoiChinhSua.Command { cn = caNhan, NoiDungBanDich = caNhan_NoiDung });
                caNhan_BaoChi.CaNhanID = result_cn.Value.CaNhanID;
                var result_cnbc = await Mediator.Send(new Application.CaNhan.CaNhan_BaoChi.ThemMoiChinhSua.Command { cnbc = caNhan_BaoChi, NoiDungBanDich = caNhan_BaoChi_NoiDung });
                return Ok(result_cn);
            }
            catch (Exception)
            {
                //return BadRequest(ex.Message);
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý");
            }
        }
       

        [HttpPut]
        [Route("ChinhSua/{caNhanID}")]
        public async Task<IActionResult> ChinhSua(Guid caNhanID, [FromBody] CaNhan_BaoChi_Add data)
        {

            try
            {
                if (data == null
                  || string.IsNullOrEmpty(data.CaNhan)
                  || string.IsNullOrEmpty(data.CaNhan_NoiDung)
                  || string.IsNullOrEmpty(data.CaNhan_BaoChi)
                  || string.IsNullOrEmpty(data.CaNhan_BaoChi_NoiDung)
                  )
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }
                VanBan caNhan = JsonConvert.DeserializeObject<VanBan>(data.CaNhan);
                List<CaNhan_NoiDung> caNhan_NoiDung = JsonConvert.DeserializeObject<List<CaNhan_NoiDung>>(data.CaNhan_NoiDung);
                CaNhan_BaoChi caNhan_BaoChi = JsonConvert.DeserializeObject<CaNhan_BaoChi>(data.CaNhan_BaoChi);
                List<CaNhan_BaoChi_NoiDung> caNhan_BaoChi_NoiDung = JsonConvert.DeserializeObject<List<CaNhan_BaoChi_NoiDung>>(data.CaNhan_BaoChi_NoiDung);

                caNhan.CaNhanID = caNhanID;

                if (data.FileDinhKem != null)
                {
                    var file = data.FileDinhKem;

                    UploadFileResult ufile = await SaveFileUpload(file, UploadPath);
                    if (ufile.Success == false)
                    {
                        return BadRequest(ufile.Message);
                    }

                    if (!string.IsNullOrEmpty(caNhan.AnhChanDung))
                    {
                        DeleteFileUpload(caNhan.AnhChanDung);
                    }

                    caNhan.AnhChanDung = ufile.Url;
                }

                var result_cn = await Mediator.Send(new Application.CaNhan.CaNhan.ThemMoiChinhSua.Command { cn = caNhan, NoiDungBanDich = caNhan_NoiDung });
                caNhan_BaoChi.CaNhanID = result_cn.Value.CaNhanID;
                var result_cnbc = await Mediator.Send(new Application.CaNhan.CaNhan_BaoChi.ThemMoiChinhSua.Command { cnbc = caNhan_BaoChi, NoiDungBanDich = caNhan_BaoChi_NoiDung });
                return Ok(result_cn);
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
            var result = await Mediator.Send(new Application.CaNhan.CaNhan.Xoa.Command { ID = id });

            return Ok(result);
        }
    }
}
