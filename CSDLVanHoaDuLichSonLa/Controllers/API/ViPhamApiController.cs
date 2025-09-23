using Azure.Core;
using Domain;
using Domain.BaoChi;
using Domain.Core;
using Domain.DanhMuc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CSDLVanHoaDuLichSonLa.Controllers.API
{
    public class ViPhamApiController : BaseApiController
    {
        const string UploadPath = "uploads/vipham/";
        public ViPhamApiController(IWebHostEnvironment hostingEnvironment, IConfiguration config) : base(hostingEnvironment, config)
        {
        }

        [HttpPost]
        [Route("DanhSach")]
        public async Task<IActionResult> DanhSach(ViPhamRequest data)
        {
            try
            {
                var result = await Mediator.Send(new Application.BaoChi.ViPham.DanhSach.Query { Data = data });
                List<ViPhamDTO> lst = result.Value.ToList();
                List<ViPhamDTO> NewList = new List<ViPhamDTO>();
                foreach (var item in lst)
                {
                    VanBanTaiLieuRequest request = new VanBanTaiLieuRequest();
                    request.DoiTuongSoHuuID = item.MaViPham;
                    var result2 = await Mediator.Send(new Application.VanBanTaiLieu.DanhSach.Query { Data = request });

                    // Map VanBanTaiLieuDTO to VanBanTaiLieu
                    item.VanBanTaiLieus = result2.Value;
                    NewList.Add(item);
                }
                return Ok(NewList);
            }
            catch (Exception)
            {
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý");
            }
        }

        [HttpPost]
        [Route("ThemMoi")]
        public async Task<IActionResult> ThemMoi( ViPhamAdd data)
        {
            try
            {
                if (data == null || string.IsNullOrEmpty(data.ViPham) || string.IsNullOrEmpty(data.ViPham_NoiDung))
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }

                ViPham xbAnPham = JsonConvert.DeserializeObject<ViPham>(data.ViPham);
                List<ViPham_NoiDung> banDich = JsonConvert.DeserializeObject<List<ViPham_NoiDung>>(data.ViPham_NoiDung);
                var result = await Mediator.Send(new Application.BaoChi.ViPham.ThemMoiChinhSua.Command { ViPham = xbAnPham, NoiDungBanDich = banDich });

                if (data.FileDinhKem != null)
                {

                    foreach(var item in data.FileDinhKem)
                    {
                        var file = item;
                        UploadFileResult ufile = await SaveFileUpload(file, UploadPath);
                        if (ufile.Success == false)
                        {
                            return BadRequest(ufile.Message);
                        }
                        VanBanTaiLieuAdd vb = new VanBanTaiLieuAdd();
                        vb.DoiTuongSoHuuID = result.Value.MaViPham;
                        vb.DuongDanFile = ufile.Url;
                        await Mediator.Send(new Application.VanBanTaiLieu.ThemMoiChinhSua.Command { vbtl=vb });
                    }
                    
                }

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
        public async Task<IActionResult> ChinhSua(Guid id, [FromForm] ViPhamAdd data)
        {
            try
            {
                if (id == Guid.Empty || data == null || string.IsNullOrEmpty(data.ViPham) || string.IsNullOrEmpty(data.ViPham_NoiDung))
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }

                ViPham xbAnPham = JsonConvert.DeserializeObject<ViPham>(data.ViPham);
                List<ViPham_NoiDung> banDich = JsonConvert.DeserializeObject<List<ViPham_NoiDung>>(data.ViPham_NoiDung);

                xbAnPham.MaViPham = id;
                
                if (data.FileDinhKem != null)
                {
                    foreach (var item in data.FileDinhKem)
                    {
                        var file = item;
                        UploadFileResult ufile = await SaveFileUpload(file, UploadPath);
                        if (ufile.Success == false)
                        {
                            return BadRequest(ufile.Message);
                        }
                        VanBanTaiLieuAdd vb = new VanBanTaiLieuAdd();
                        vb.DoiTuongSoHuuID = id; 
                        vb.DuongDanFile = ufile.Url;
                        await Mediator.Send(new Application.VanBanTaiLieu.ThemMoiChinhSua.Command { vbtl = vb });
                    }
                }

                var result = await Mediator.Send(new Application.BaoChi.ViPham.ThemMoiChinhSua.Command { ViPham = xbAnPham, NoiDungBanDich = banDich });
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
                var result = await Mediator.Send(new Application.BaoChi.ViPham.Xoa.Command { ID = id });
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý");
            }
        }

        [HttpGet]
        [Route("BanDich/{maViPham}/{maNgonNgu?}")]
        public async Task<IActionResult> BanDich(Guid maViPham, string maNgonNgu)
        {
            try
            {
                var result = await Mediator.Send(new Application.BaoChi.ViPham.DanhSachBanDich.Query { MaViPhamID = maViPham, MaNgonNgu = null});
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý");
            }
        }
    }
}
