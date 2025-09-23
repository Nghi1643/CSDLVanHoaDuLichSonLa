using Application.ToChuc;
using CSDLVanHoaDuLichSonLa.Models;
using CSDLVanHoaDuLichSonLa.Services;
using Domain;
using Domain.BaoChi;
using Domain.Core;
using Domain.DanhMuc;
using Domain.ToChuc;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CSDLVanHoaDuLichSonLa.Controllers.API
{
    public class ToChucApiController : BaseApiController
    {
        const string UploadPath = "uploads/XuatBanAnPham";
        public ToChucApiController(IWebHostEnvironment hostingEnvironment, IConfiguration config) : base(hostingEnvironment, config)
        {

        }

        [HttpPost]
        [Route("DanhSach")]
        public async Task<IActionResult> DanhSach(ToChucRequest data)
        {
            data.MaNgonNgu = "vi";
            var result = await Mediator.Send(new Application.ToChuc.DanhSach.Query { Data = data });
            return Ok(result);
        }

        [HttpPost]
        [Route("ThemMoi")]
        public async Task<IActionResult> ThemMoi(ToChucAdd data)
        {
            if (data?.ToChuc_NoiDungs?.Any() == true)
            {
                var result = await Mediator.Send(new Application.ToChuc.ThemMoiChinhSua.Command { Data = data });
                return Ok(result);
            }
            return BadRequest("Tham số không hợp lệ");
        }

        [HttpPost]
        [Route("AddToChucBaoChi")]
        public async Task<Result<ToChuc>> AddToChucBaoChi ([FromForm]BC_ToChucBaoChiRequest _request)
        {
            DM_ToChucBaseRequest RequestToChuc = new DM_ToChucBaseRequest();
            List<BC_AnPhamBaseRequest> RequestAnPham = new List<BC_AnPhamBaseRequest>();
            if(_request.ReqeustToChuc == null)
            {
                return Result<ToChuc>.Failure("Dữ liệu đầu vào không hợp lệ");
            }

            RequestToChuc = JsonConvert.DeserializeObject<DM_ToChucBaseRequest>(_request.ReqeustToChuc);

            if(_request.RequestAnPham != null)
            {
                RequestAnPham = JsonConvert.DeserializeObject<List<BC_AnPhamBaseRequest>>(_request.RequestAnPham);
            }

            return await Mediator.Send(new AddToChucBaoChi.Command { EntityAnPham = RequestAnPham, EntityToChuc = RequestToChuc });


        }

        [HttpPost]
        [Route("AddNhaXuatBan")]
        public async Task<Result<ToChuc>> AddNhaXuatBan ([FromForm]BC_NhaXuatBanRequest _request)
        {
            DM_ToChucBaseRequest RequestToChuc = new DM_ToChucBaseRequest();
            List<BC_AnPhamBaseRequest> RequestAnPham = new List<BC_AnPhamBaseRequest>();
            if (_request.RequestToChuc == null)
            {
                return Result<ToChuc>.Failure("Dữ liệu đầu vào không hợp lệ");
            }

            RequestToChuc = JsonConvert.DeserializeObject<DM_ToChucBaseRequest>(_request.RequestToChuc);

            var result = await Mediator.Send(new AddToChucBaoChi.Command { EntityToChuc = RequestToChuc, EntityAnPham = RequestAnPham });

            if(!result.IsSuccess && result.Value == null)
            {
                return Result<ToChuc>.Failure("Thêm mới tổ chức không thành công");
            }

            if(_request.RequestXuatBan != null)
            {
                foreach(var item in _request.RequestXuatBan)
                {
                    var xbAnPham = JsonConvert.DeserializeObject<XuatBanAnPham>(item.XuatBanAnPham);
                    var banDich = JsonConvert.DeserializeObject<List<XuatBanAnPham_NoiDung>>(item.XuatBanAnPham_NoiDung);
                    if (item.FileDinhKem != null)
                    {
                        var file = item.FileDinhKem;

                        UploadFileResult ufile = await SaveFileUpload(file, UploadPath);
                        if (ufile.Success == false)
                        {
                            return Result<ToChuc>.Failure(ufile.Message);
                        }

                        xbAnPham.DuongDanHinhAnhBia = ufile.Url;
                    }

                    xbAnPham.NhaXuatBanID = result.Value.ToChucID;
                    var resultXuatBanAnPham = await Mediator.Send(new Application.BaoChi.XuatBanAnPham.ThemMoiChinhSua.Command { XBAnPham = xbAnPham, NoiDungBanDich = banDich });
                    if(!resultXuatBanAnPham.IsSuccess && resultXuatBanAnPham.Value == null)
                    {
                        return Result<ToChuc>.Failure("Thêm mới xuất bản ấn phẩm không thành công");
                    }
                }
            }

            return result;
        }




        [HttpPost]
        [Route("ThemMoi_ToChuc_AnPham")]
        public async Task<IActionResult> ThemMoiToChuc_AnPham([FromBody] ToChuc_AnPham_Add data)
        {
            if (data == null || string.IsNullOrEmpty(data.XuatBanAnPham) || string.IsNullOrEmpty(data.XuatBanAnPham_NoiDung) || string.IsNullOrEmpty(data.ToChucAdd))
            {
                return BadRequest("Dữ liệu không hợp lệ");
            }
            
            ToChucAdd tochuc = JsonConvert.DeserializeObject<ToChucAdd>(data.ToChucAdd);
            var result_ToChuc = await Mediator.Send(new Application.ToChuc.ThemMoiChinhSua.Command { Data = tochuc });
            XuatBanAnPham xbAnPham = JsonConvert.DeserializeObject<XuatBanAnPham>(data.XuatBanAnPham);
            xbAnPham.NhaXuatBanID = result_ToChuc.Value.ToChucID;
            List<XuatBanAnPham_NoiDung> banDich = JsonConvert.DeserializeObject<List<XuatBanAnPham_NoiDung>>(data.XuatBanAnPham_NoiDung);
            var result_AnPham = await Mediator.Send(new Application.BaoChi.XuatBanAnPham.ThemMoiChinhSua.Command { XBAnPham = xbAnPham, NoiDungBanDich = banDich });
            return Ok(result_ToChuc);
        }

        [HttpPut]
        [Route("ChinhSua/{toChucID}")]
        public async Task<IActionResult> ChinhSua(Guid? toChucID, [FromBody] ToChucAdd data)
        {

            data.ToChucID = toChucID;
            if (data.ToChucID== null)
            {
                return BadRequest(Result<DanhMucChung>.Failure("Tham số truyền vào không hợp lệ"));
            }

            var result = await Mediator.Send(new Application.ToChuc.ThemMoiChinhSua.Command { Data = data });
            return Ok(result);
        }

        [HttpGet]
        [Route("BanDich/{toChucID}")]
        public async Task<IActionResult> BanDich(Guid? toChucID, [FromQuery] string maNgonNgu)
        {
            if (toChucID != null)
            {
                var result = await Mediator.Send(new Application.ToChuc.DanhSachBanDich.Query { ToChucID = toChucID,  MaNgonNgu = maNgonNgu});
                return Ok(result);
            }
            return BadRequest("Tham số không hợp lệ");
        }

        [HttpDelete]
        [Route("Xoa/{id}")]
        public async Task<IActionResult> Xoa(Guid id)
        {
            var result = await Mediator.Send(new Application.ToChuc.Xoa.Command { ID = id });

            return Ok(result);
        }
    }
}
