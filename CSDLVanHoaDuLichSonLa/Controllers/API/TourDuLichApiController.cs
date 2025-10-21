using Domain.TourDuLich;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CSDLVanHoaDuLichSonLa.Controllers.API
{
    public class TourDuLichApiController : BaseApiController
    {
        public TourDuLichApiController(IWebHostEnvironment hostingEnvironment, IConfiguration config) : base(hostingEnvironment, config)
        {
        }

        #region TourDuLich APIs

        [HttpPost("DanhSach")]
        public async Task<IActionResult> DanhSach([FromBody] TourDuLichRequest data)
        {
            try
            {
                data.MaNgonNgu = "vi";
                var result = await Mediator.Send(new Application.TourDuLich.TourDuLich.DanhSach.Query { data = data });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("ChiTiet/{id}")]
        public async Task<IActionResult> ChiTiet(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }
                var result = await Mediator.Send(new Application.TourDuLich.TourDuLich.ChiTiet.Query { TourID = id });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("ThemMoi")]
        public async Task<IActionResult> ThemMoi([FromForm] TourDuLichAdd data)
        {
            try
            {
                if (data == null || string.IsNullOrEmpty(data.TourDuLich) || string.IsNullOrEmpty(data.TourDuLich_NoiDung))
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }

                var tourDuLich = JsonConvert.DeserializeObject<TourDuLich>(data.TourDuLich);
                List<TourDuLich_NoiDung> noiDung = JsonConvert.DeserializeObject<List<TourDuLich_NoiDung>>(data.TourDuLich_NoiDung);

                var result = await Mediator.Send(new Application.TourDuLich.TourDuLich.ThemMoiChinhSua.Command { TourDuLich = tourDuLich, NoiDung = noiDung });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("ChinhSua/{id}")]
        public async Task<IActionResult> ChinhSua(Guid id, [FromForm] TourDuLichAdd data)
        {
            try
            {
                if (id == Guid.Empty || data == null || string.IsNullOrEmpty(data.TourDuLich) || string.IsNullOrEmpty(data.TourDuLich_NoiDung))
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }

                var tourDuLich = JsonConvert.DeserializeObject<TourDuLich>(data.TourDuLich);
                List<TourDuLich_NoiDung> noiDung = JsonConvert.DeserializeObject<List<TourDuLich_NoiDung>>(data.TourDuLich_NoiDung);
                tourDuLich.TourID = id;

                var result = await Mediator.Send(new Application.TourDuLich.TourDuLich.ThemMoiChinhSua.Command { TourDuLich = tourDuLich, NoiDung = noiDung });
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
                var result = await Mediator.Send(new Application.TourDuLich.TourDuLich.Xoa.Command { ID = id });
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý");
            }
        }

        [HttpGet]
        [Route("BanDich/{tourID}/{maNgonNgu?}")]
        public async Task<IActionResult> BanDich(Guid tourID, string maNgonNgu = null)
        {
            try
            {
                if (tourID == Guid.Empty)
                {
                    return BadRequest("ID không hợp lệ");
                }

                var result = await Mediator.Send(new Application.TourDuLich.TourDuLich.DanhSachBanDich.Query
                {
                    TourID = tourID,
                    MaNgonNgu = maNgonNgu
                });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý: " + ex.Message);
            }
        }

        #endregion

        #region LichTrinh APIs

        [HttpGet]
        [Route("LichTrinh/DanhSach/{tourID}")]
        public async Task<IActionResult> LichTrinhDanhSach(Guid tourID, string maNgonNgu = "vi")
        {
            try
            {
                if (tourID == Guid.Empty)
                {
                    return BadRequest("ID không hợp lệ");
                }

                var result = await Mediator.Send(new Application.TourDuLich.LichTrinh.DanhSach.Query { TourID = tourID, MaNgonNgu = maNgonNgu });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("LichTrinh/ChiTiet/{lichTrinhID}")]
        public async Task<IActionResult> LichTrinhChiTiet(int lichTrinhID)
        {
            try
            {
                if (lichTrinhID <= 0)
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }
                var result = await Mediator.Send(new Application.TourDuLich.LichTrinh.ChiTiet.Query { LichTrinhID = lichTrinhID });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("LichTrinh/ThemMoi")]
        public async Task<IActionResult> LichTrinhThemMoi([FromForm] string DanhSachLichTrinh)
        {
            try
            {
                if (string.IsNullOrEmpty(DanhSachLichTrinh))
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }

                var lichTrinhs = JsonConvert.DeserializeObject<List<Application.TourDuLich.LichTrinh.ThemMoiChinhSua.LichTrinhItem>>(DanhSachLichTrinh);

                var result = await Mediator.Send(new Application.TourDuLich.LichTrinh.ThemMoiChinhSua.Command
                {
                    DanhSachLichTrinh = lichTrinhs
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("LichTrinh/ChinhSua/{id}")]
        public async Task<IActionResult> LichTrinhChinhSua(Guid id, [FromForm] string DanhSachLichTrinh)
        {
            try
            {
                if (id == Guid.Empty || string.IsNullOrEmpty(DanhSachLichTrinh))
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }

                var lichTrinhs = JsonConvert.DeserializeObject<List<Application.TourDuLich.LichTrinh.ThemMoiChinhSua.LichTrinhItem>>(DanhSachLichTrinh);
                var result = await Mediator.Send(new Application.TourDuLich.LichTrinh.ThemMoiChinhSua.Command
                {
                    DanhSachLichTrinh = lichTrinhs
                });
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete]
        [Route("LichTrinh/Xoa/{id}")]
        public async Task<IActionResult> LichTrinhXoa(int id)
        {
            try
            {
                var result = await Mediator.Send(new Application.TourDuLich.LichTrinh.Xoa.Command { ID = id });
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý");
            }
        }

        #endregion

        #region TheLoai APIs

        [HttpGet]
        [Route("TheLoai/DanhSach")]
        public async Task<IActionResult> TheLoaiDanhSach([FromQuery] string maNgonNgu = "vi")
        {
            try
            {
                var result = await Mediator.Send(new Application.TourDuLich.TheLoai.DanhSach.Query { MaNgonNgu = maNgonNgu });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("TheLoai/ChiTiet/{theLoaiID}")]
        public async Task<IActionResult> TheLoaiChiTiet(Guid theLoaiID, [FromQuery] string maNgonNgu = "vi")
        {
            try
            {
                if (theLoaiID == Guid.Empty)
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }
                var result = await Mediator.Send(new Application.TourDuLich.TheLoai.ChiTiet.Query { TheLoaiID = theLoaiID, MaNgonNgu = maNgonNgu });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("TheLoai/ThemMoi")]
        public async Task<IActionResult> TheLoaiThemMoi([FromForm] TourDuLich_TheLoaiAdd data)
        {
            try
            {
                if (data == null || string.IsNullOrEmpty(data.TourDuLich_TheLoai) || string.IsNullOrEmpty(data.TourDuLich_TheLoai_NoiDung))
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }

                var theLoai = JsonConvert.DeserializeObject<TourDuLich_TheLoai>(data.TourDuLich_TheLoai);
                List<TourDuLich_TheLoai_NoiDung> noiDung = JsonConvert.DeserializeObject<List<TourDuLich_TheLoai_NoiDung>>(data.TourDuLich_TheLoai_NoiDung);

                var result = await Mediator.Send(new Application.TourDuLich.TheLoai.ThemMoiChinhSua.Command
                {
                    TourDuLich_TheLoai = theLoai,
                    NoiDung = noiDung,
                    ListLinhVucID = data.ListLinhVucID
                });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("TheLoai/ChinhSua/{id}")]
        public async Task<IActionResult> TheLoaiChinhSua(Guid id, [FromForm] TourDuLich_TheLoaiAdd data)
        {
            try
            {
                if (id == Guid.Empty || data == null || string.IsNullOrEmpty(data.TourDuLich_TheLoai) || string.IsNullOrEmpty(data.TourDuLich_TheLoai_NoiDung))
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }

                var theLoai = JsonConvert.DeserializeObject<TourDuLich_TheLoai>(data.TourDuLich_TheLoai);
                List<TourDuLich_TheLoai_NoiDung> noiDung = JsonConvert.DeserializeObject<List<TourDuLich_TheLoai_NoiDung>>(data.TourDuLich_TheLoai_NoiDung);
                theLoai.TheLoaiID = id;

                var result = await Mediator.Send(new Application.TourDuLich.TheLoai.ThemMoiChinhSua.Command
                {
                    TourDuLich_TheLoai = theLoai,
                    NoiDung = noiDung,
                    ListLinhVucID = data.ListLinhVucID
                });
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý");
            }
        }

        [HttpDelete]
        [Route("TheLoai/Xoa/{id}")]
        public async Task<IActionResult> TheLoaiXoa(Guid id)
        {
            try
            {
                var result = await Mediator.Send(new Application.TourDuLich.TheLoai.Xoa.Command { ID = id });
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest("Đã xảy ra lỗi trong quá trình xử lý");
            }
        }

        #endregion
    }
}