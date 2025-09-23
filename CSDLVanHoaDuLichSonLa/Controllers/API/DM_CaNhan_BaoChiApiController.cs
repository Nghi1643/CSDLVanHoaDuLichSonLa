using Domain;
using Domain.Core;
using Domain.DM_CaNhan_BaoChiModel;
using Domain.DM_CaNhan_VanDongVienModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CSDLVanHoaDuLichSonLa.Controllers.API
{
    public class DM_CaNhan_BaoChiApiController : BaseApiController
    {
        const string FilePath = "uploads/CanBoBaoChi";
        public DM_CaNhan_BaoChiApiController(IWebHostEnvironment hostingEnvironment, IConfiguration config) : base(hostingEnvironment, config)
        {
        }

        [HttpGet]
        [Route("Gets")]
        public async Task<Result<IEnumerable<DM_CaNhan_BaoChiViewModel>>> Gets(string MaNgonNgu)
        {
            return await Mediator.Send(new Application.DM_CaNhan_BaoChiServices.Gets.Query { MaNgonNgu = MaNgonNgu });
        }

        [HttpGet]
        [Route("Get")]
        public async Task<Result<DM_CaNhan_BaoChiViewModel>> Get(string MaNgonNgu, Guid CaNhanID)
        {
            return await Mediator.Send(new Application.DM_CaNhan_BaoChiServices.Get.Query { MaNgonNgu = MaNgonNgu, CaNhanID = CaNhanID });
        }

        [HttpGet]
        [Route("Filter")]
        public async Task<Result<IEnumerable<DM_CaNhan_BaoChiViewModel>>> Filter(string TuKhoa = null, Guid? ToChucID = null, int? GioiTinhID = null, bool? TrangThai = null, string MaNgonNgu = "vi"  )
        {
            return await Mediator.Send(new Application.DM_CaNhan_BaoChiServices.Filter.Query { TuKhoa = TuKhoa, ToChucID = ToChucID, GioiTinhID = GioiTinhID, TrangThai = TrangThai, MaNgonNgu = MaNgonNgu   });
        }

        [HttpPost]
        [Route("Add")]
        public async Task<Result<DM_CaNhan>> Add(DM_CaNhanRequestInfo _request)
        {
            if (_request == null)
            {
                return Result<DM_CaNhan>.Failure("Dữ liệu đầu vào không hợp lệ");
            }

            DM_CaNhan_BaoChi_RequestInfo EntityCaNhanBaoChi = JsonConvert.DeserializeObject<DM_CaNhan_BaoChi_RequestInfo>(_request.RequestCaNhanBaoChi);
            

            if (_request.File != null)
            {
                UploadFileResult UploadFile = await SaveFileUpload(_request.File, FilePath);
                if (UploadFile.Success && UploadFile.Url != null)
                {
                    EntityCaNhanBaoChi.AnhChanDung = UploadFile.Url;
                }
            }

            return await Mediator.Send(new Application.DM_CaNhan_BaoChiServices.Add.Command { Entity = EntityCaNhanBaoChi });
        }

        [HttpPut]
        [Route("Update")]
        public async Task<Result<DM_CaNhan>> Update([FromForm] DM_CaNhanRequestInfo _request)
        {
            if (_request.RequestCaNhanBaoChi == null)
            {
                return Result<DM_CaNhan>.Failure("Dữ liệu đầu vào không hợp lệ");
            }

            DM_CaNhan_BaoChi_RequestInfo EntityCaNhanBaoChi = JsonConvert.DeserializeObject<DM_CaNhan_BaoChi_RequestInfo>(_request.RequestCaNhanBaoChi);

            if (_request.File != null)
            {
                if (EntityCaNhanBaoChi.AnhChanDungDetail != null)
                {
                    DeleteFileObject(EntityCaNhanBaoChi.AnhChanDungDetail);
                }

                UploadFileResult UploadFile = await SaveFileUpload(_request.File, FilePath);
                if (UploadFile.Success && UploadFile.Url != null)
                {
                    EntityCaNhanBaoChi.AnhChanDung = UploadFile.Url;
                }
            }
            else
            {
                EntityCaNhanBaoChi.AnhChanDung = EntityCaNhanBaoChi.AnhChanDungDetail;
            }

            return await Mediator.Send(new Application.DM_CaNhan_BaoChiServices.Update.Command { Entity = EntityCaNhanBaoChi });
        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<Result<int>> Delete(Guid CaNhanID)
        {
            return await Mediator.Send(new Application.DM_CaNhan_BaoChiServices.Delete.Command { CaNhanID = CaNhanID });
        }
    }
}
