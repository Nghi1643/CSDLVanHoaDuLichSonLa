
using Application.DM_CaNhan_VanDongVienServices;
using Domain;
using Domain.Core;
using Domain.DM_CaNhan_BaoChiModel;
using Domain.DM_CaNhan_VanDongVienModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CSDLVanHoaDuLichSonLa.Controllers.API
{
    public class DM_CaNhan_CanBoTheThaoApiController : BaseApiController
    {
        const string FilePath = "uploads/CanBoTheThao";
        public DM_CaNhan_CanBoTheThaoApiController(IWebHostEnvironment hostingEnvironment, IConfiguration config) : base(hostingEnvironment, config)
        {
        }

        [HttpGet]
        [Route("Gets")]
        public async Task<Result<IEnumerable<DM_CaNhan_CanBoTheThaoViewModel>>> Gets(string MaNgonNgu)
        {
            return await Mediator.Send(new Application.DM_CaNhan_CanBoTheThaoServices.Gets.Query { MaNgonNgu = MaNgonNgu });
        }

        [HttpGet]
        [Route("Get")]
        public async Task<Result<DM_CaNhan_CanBoTheThaoViewModel>> Get(string MaNgonNgu, Guid CaNhanID)
        {
            return await Mediator.Send(new Application.DM_CaNhan_CanBoTheThaoServices.Get.Query { MaNgonNgu = MaNgonNgu, CaNhanID = CaNhanID });
        }

        [HttpGet]
        [Route("Filter")]
        public async Task<Result<IEnumerable<DM_CaNhan_CanBoTheThaoViewModel>>> Filter(string TuKhoa = null, Guid? ToChucID = null, int? GioiTinhID = null, bool? TrangThai = null, string MaNgonNgu = "vi")
        {
            return await Mediator.Send(new Application.DM_CaNhan_CanBoTheThaoServices.Filter.Query { TuKhoa = TuKhoa, ToChucID = ToChucID, GioiTinhID = GioiTinhID, TrangThai = TrangThai, MaNgonNgu = MaNgonNgu });
        }

        [HttpPost]
        [Route("Add")]
        public async Task<Result<DM_CaNhan>> Add(DM_CaNhanCanBoTheThao_Request _request)
        {
            if (_request == null)
            {
                return Result<DM_CaNhan>.Failure("Dữ liệu đầu vào không hợp lệ");
            }

            DM_CaNhan_CanBoTheTheoRequestHandleInfo EntityCanBoTheThao = JsonConvert.DeserializeObject<DM_CaNhan_CanBoTheTheoRequestHandleInfo>(_request.RequestCanBoTheThao);
            

            if (_request.File != null)
            {
                UploadFileResult UploadFile = await SaveFileUpload(_request.File, FilePath);
                if (UploadFile.Success && UploadFile.Url != null)
                {
                    EntityCanBoTheThao.AnhChanDung = UploadFile.Url;
                }
            }

            return await Mediator.Send(new Application.DM_CaNhan_CanBoTheThaoServices.Add.Command { Entity = EntityCanBoTheThao });
        }

        [HttpPut]
        [Route("Update")]
        public async Task<Result<DM_CaNhan>> Update([FromForm] DM_CaNhanCanBoTheThao_Request _request)
        {
            if (_request.RequestCanBoTheThao == null)
            {
                return Result<DM_CaNhan>.Failure("Dữ liệu đầu vào không hợp lệ");
            }

            DM_CaNhan_CanBoTheTheoRequestHandleInfo EntityCanBoTheThao = JsonConvert.DeserializeObject<DM_CaNhan_CanBoTheTheoRequestHandleInfo>(_request.RequestCanBoTheThao);

            if (_request.File != null)
            {
                if (EntityCanBoTheThao.AnhChanDungDetail != null)
                {
                    DeleteFileObject(EntityCanBoTheThao.AnhChanDungDetail);
                }

                UploadFileResult UploadFile = await SaveFileUpload(_request.File, FilePath);
                if (UploadFile.Success && UploadFile.Url != null)
                {
                    EntityCanBoTheThao.AnhChanDung = UploadFile.Url;
                }
            }
            else
            {
                EntityCanBoTheThao.AnhChanDung = EntityCanBoTheThao.AnhChanDungDetail;
            }

            return await Mediator.Send(new Application.DM_CaNhan_CanBoTheThaoServices.Update.Command { Entity = EntityCanBoTheThao });
        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<Result<int>> Delete(Guid CaNhanID)
        {
            return await Mediator.Send(new Application.DM_CaNhan_CanBoTheThaoServices.Delete.Command { CaNhanID = CaNhanID });
        }
    }
}
