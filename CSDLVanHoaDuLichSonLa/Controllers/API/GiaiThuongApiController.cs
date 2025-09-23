using Domain;
using Domain.BaoChi;
using Domain.DanhMuc;
using Domain.SuKienHoatDong;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSDLVanHoaDuLichSonLa.Controllers.API
{
    public class GiaiThuongApiController : BaseApiController
    {
        const string UploadPath = "uploads/giaithuong/";
        public GiaiThuongApiController(IWebHostEnvironment env, IConfiguration config) : base(env, config) { }

        public class UpsertDto
        {
            public string GiaiThuong { get; set; }
            public string NoiDungBanDich { get; set; }
            public List<IFormFile> FileDinhKem { get; set; }
        }

        [HttpPost]
        [Route("DanhSach")]
        public async Task<IActionResult> DanhSach([FromBody] GiaiThuongRequest data)
        {
            try
            {
                var result = await Mediator.Send(new Application.BaoChi.GiaiThuong.DanhSach.Query { Data = data });
                List<GiaiThuongDTO> lst = result.Value.ToList();
                List<GiaiThuongDTO> NewList = new List<GiaiThuongDTO>();
                foreach (var item in lst)
                {
                    VanBanTaiLieuRequest request = new VanBanTaiLieuRequest();
                    request.DoiTuongSoHuuID = item.GiaiThuongID;
                    var result2 = await Mediator.Send(new Application.VanBanTaiLieu.DanhSach.Query { Data = request });
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

        [HttpGet]
        [Route("BanDich")]
        public async Task<IActionResult> BanDich(Guid giaiThuongID, string? maNgonNgu = null)
        {
            try
            {
                var result = await Mediator.Send(new Application.BaoChi.GiaiThuong.DanhSachBanDich.Query { GiaiThuongID = giaiThuongID, MaNgonNgu = maNgonNgu });
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý");
            }
        }

        [HttpPost]
        [Route("ThemMoi")]
        public async Task<IActionResult> ThemMoi( UpsertDto data)
        {
            try
            {
                var validation = Validate(data, isEdit: false);
                if (validation != null) return BadRequest(validation);
                GiaiThuong giaithuong = JsonConvert.DeserializeObject<GiaiThuong>(data.GiaiThuong);
                List<GiaiThuong_NoiDung> banDich = JsonConvert.DeserializeObject<List<GiaiThuong_NoiDung>>(data.NoiDungBanDich);
                var result = await Mediator.Send(new Application.BaoChi.GiaiThuong.ThemMoiChinhSua.Command { GiaiThuong = giaithuong, NoiDungBanDich = banDich });
                
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
                        vb.DoiTuongSoHuuID = result.Value.GiaiThuongID;
                        vb.DuongDanFile = ufile.Url;
                        await Mediator.Send(new Application.VanBanTaiLieu.ThemMoiChinhSua.Command { vbtl = vb });
                    }

                }
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý");
            }
        }

        [HttpPut]
        [Route("ChinhSua/{id}")]
        public async Task<IActionResult> ChinhSua(Guid id,  UpsertDto data)
        {
            try
            {
                if (id == Guid.Empty) return BadRequest("ID không hợp lệ");

                GiaiThuong giaithuong = JsonConvert.DeserializeObject<GiaiThuong>(data.GiaiThuong);
                List<GiaiThuong_NoiDung> banDich = JsonConvert.DeserializeObject<List<GiaiThuong_NoiDung>>(data.NoiDungBanDich);
                giaithuong.GiaiThuongID = id;
                var validation = Validate(data, isEdit: true);
                if (validation != null) return BadRequest(validation);
                var result = await Mediator.Send(new Application.BaoChi.GiaiThuong.ThemMoiChinhSua.Command { GiaiThuong = giaithuong, NoiDungBanDich = banDich });
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý");
            }
        }

        [HttpDelete]
        [Route("Xoa/{id}")]
        public async Task<IActionResult> Xoa(Guid id)
        {
            try
            {
                var result = await Mediator.Send(new Application.BaoChi.GiaiThuong.Xoa.Command { ID = id });
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý");
            }
        }

        private string? Validate(UpsertDto dto, bool isEdit)
        {
            
            return null;
        }
    }
}