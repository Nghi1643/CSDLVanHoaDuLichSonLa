
using Application.DM_CaNhan_NgheSiServices;
using Domain;
using Domain.Core;
using Domain.DM_CaNhan_BaoChiModel;
using Domain.DM_CaNhan_HuongDanVienModel;
using Domain.DM_CaNhan_NgheSiModel;
using Domain.DM_CaNhan_VanDongVienModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CSDLVanHoaDuLichSonLa.Controllers.API
{
    
    public class DM_CaNhan_NgheSiApiController : BaseApiController
    {
        const string FilePath = "uploads/NgheSi";
        public DM_CaNhan_NgheSiApiController(IWebHostEnvironment hostingEnvironment, IConfiguration config) : base(hostingEnvironment, config)
        {
        }

        [HttpGet]
        [Route("Gets")]
        public async Task<Result<IEnumerable<DM_CaNhan_NgheSiViewModel>>> Gets(string MaNgonNgu)
        {
            return await Mediator.Send(new Gets.Query { MaNgonNgu = MaNgonNgu });
        }

        [HttpGet]
        [Route("Get")]
        public async Task<Result<DM_CaNhan_NgheSiViewModel>> Get(string MaNgonNgu, Guid CaNhanID)
        {
            return await Mediator.Send(new Get.Query { MaNgonNgu = MaNgonNgu, CaNhanID = CaNhanID});
        }


        [HttpGet]
        [Route("Filter")]
        public async Task<Result<IEnumerable<DM_CaNhan_NgheSiViewModel>>> Filter(string TuKhoa = null, Guid? ToChucID = null, int? GioiTinhID = null, bool? TrangThai = null, string MaNgonNgu = "vi")
        {
            return await Mediator.Send(new Filter.Query { TuKhoa = TuKhoa, ToChucID = ToChucID, GioiTinhID = GioiTinhID, TrangThai = TrangThai, MaNgonNgu = MaNgonNgu });
        }

        [HttpPost]
        [Route("Add")]
        public async Task<Result<DM_CaNhan_NgheSi>> Add(DM_NgheSiRequestInfo _request)
        {
            if (_request == null)
            {
                return Result<DM_CaNhan_NgheSi>.Failure("Dữ liệu đầu vào không hợp lệ");
            }

            DM_CaNhan_NgheSi_RequestInfo EntityNgheSi = JsonConvert.DeserializeObject<DM_CaNhan_NgheSi_RequestInfo>(_request.RequestNgheSi);


            if (_request.File != null)
            {
                UploadFileResult UploadFile = await SaveFileUpload(_request.File, FilePath);
                if (UploadFile.Success && UploadFile.Url != null)
                {
                    EntityNgheSi.AnhChanDung = UploadFile.Url;
                }
            }

            return await Mediator.Send(new Add.Command { Entity = EntityNgheSi });
        }

        [HttpPut]
        [Route("Update")]
        public async Task<Result<DM_CaNhan_NgheSi>> Update([FromForm] DM_NgheSiRequestInfo _request)
        {
            if (_request.RequestNgheSi == null)
            {
                return Result<DM_CaNhan_NgheSi>.Failure("Dữ liệu đầu vào không hợp lệ");
            }

            DM_CaNhan_NgheSi_RequestInfo EntityNgheSi = JsonConvert.DeserializeObject<DM_CaNhan_NgheSi_RequestInfo>(_request.RequestNgheSi);

            if (_request.File != null)
            {
                if (EntityNgheSi.AnhChanDungDetail != null)
                {
                    DeleteFileObject(EntityNgheSi.AnhChanDungDetail);
                }

                UploadFileResult UploadFile = await SaveFileUpload(_request.File, FilePath);
                if (UploadFile.Success && UploadFile.Url != null)
                {
                    EntityNgheSi.AnhChanDung = UploadFile.Url;
                }
            }
            else
            {
                EntityNgheSi.AnhChanDung = EntityNgheSi.AnhChanDungDetail;
            }

            return await Mediator.Send(new Update.Command { Entity = EntityNgheSi });
        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<Result<int>> Delete(Guid CaNhanID)
        {
            return await Mediator.Send(new Delete.Command { CaNhanID = CaNhanID });
        }
    }
}
