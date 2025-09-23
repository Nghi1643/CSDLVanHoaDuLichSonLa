
using Application.DM_CaNhan_VanDongVienServices;
using Domain;
using Domain.Core;
using Domain.DM_CaNhan_NgheSiModel;
using Domain.DM_CaNhan_VanDongVienModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CSDLVanHoaDuLichSonLa.Controllers.API
{
    public class DM_CaNhan_TrongTaiApiController : BaseApiController
    {
        const string FilePath = "uploads/TrongTai";
        public DM_CaNhan_TrongTaiApiController(IWebHostEnvironment hostingEnvironment, IConfiguration config) : base(hostingEnvironment, config)
        {
        }

        [HttpGet]
        [Route("Gets")]
        public async Task<Result<IEnumerable<DM_CaNhan_TrongTaiViewModel>>> Gets (string MaNgonNgu)
        {
            return await Mediator.Send(new Application.DM_CaNhan_TrongTaiServices.Gets.Query { MaNgonNgu = MaNgonNgu });
        }

        [HttpGet]
        [Route("Get")]
        public async Task<Result<DM_CaNhan_TrongTaiViewModel>> Get(string MaNgonNgu, Guid CaNhanID)
        {
            return await Mediator.Send(new Application.DM_CaNhan_TrongTaiServices.Get.Query { MaNgonNgu = MaNgonNgu, CaNhanID = CaNhanID });
        }

        [HttpGet]
        [Route("Filter")]
        public async Task<Result<IEnumerable<DM_CaNhan_TrongTaiViewModel>>> Filter(string TuKhoa = null, Guid? ToChucID = null, int? GioiTinhID = null, bool? TrangThai = null, string MaNgonNgu = "vi")
        {
            return await Mediator.Send(new Application.DM_CaNhan_TrongTaiServices.Filter.Query { TuKhoa = TuKhoa, ToChucID = ToChucID, GioiTinhID = GioiTinhID, TrangThai = TrangThai, MaNgonNgu = MaNgonNgu });
        }

        [HttpGet]
        [Route("GetListMon")]
        public async Task<Result<IEnumerable<DM_CaNhan_MonTheThaoList>>> GetListMon(Guid CaNhanID, string MaNgonNgu)
        {
            return await Mediator.Send(new GetListMonByCaNhanID.Query { CaNhanID = CaNhanID, MaNgonNgu = MaNgonNgu });
        }

        [HttpPost]
        [Route("Add")]
        public async Task<Result<DM_CaNhan>> Add([FromForm] DM_CaNhanTrongTai_AddRequest _request)
        {
            if(_request.RequestTrongTai == null)
            {
                return Result<DM_CaNhan>.Failure("Dữ liệu đầu vào không hợp lệ");
            }

            DM_CaNhan_TrongTaiRequestHandleInfo EntityTrongTai = JsonConvert.DeserializeObject<DM_CaNhan_TrongTaiRequestHandleInfo>(_request.RequestTrongTai);
            List<DM_CaNhan_MonTheThaoAddRequest> EntityTheThao = new List<DM_CaNhan_MonTheThaoAddRequest>();
            if(_request.RequestMonTheThao != null)
            {
                EntityTheThao = JsonConvert.DeserializeObject<List<DM_CaNhan_MonTheThaoAddRequest>>(_request.RequestMonTheThao);
            }

            if (_request.File != null)
            {
                UploadFileResult UploadFile = await SaveFileUpload(_request.File, FilePath);
                if (UploadFile.Success && UploadFile.Url != null)
                {
                    EntityTrongTai.AnhChanDung = UploadFile.Url;
                }
            }

            return await Mediator.Send(new Application.DM_CaNhan_TrongTaiServices.Add.Command { Entity = EntityTrongTai, EntityTheThao = EntityTheThao });
        }


        [HttpPut]
        [Route("Update")]
        public async Task<Result<DM_CaNhan>> Update([FromForm] DM_CaNhan_TrongTai_UpdateRequest _request)
        {
            if (_request.RequestTrongTai == null)
            {
                return Result<DM_CaNhan>.Failure("Dữ liệu đầu vào không hợp lệ");
            }

            DM_CaNhan_TrongTaiRequestHandleInfo EntityTrongTai = JsonConvert.DeserializeObject<DM_CaNhan_TrongTaiRequestHandleInfo>(_request.RequestTrongTai);

            if (_request.File != null)
            {
                if(EntityTrongTai.AnhChanDungDetail != null)
                {
                    DeleteFileObject(EntityTrongTai.AnhChanDungDetail);
                }

                UploadFileResult UploadFile = await SaveFileUpload(_request.File, FilePath);
                if (UploadFile.Success && UploadFile.Url != null)
                {
                    EntityTrongTai.AnhChanDung = UploadFile.Url;
                }
            }
            else
            {
                EntityTrongTai.AnhChanDung = EntityTrongTai.AnhChanDungDetail;
            }

                return await Mediator.Send(new Application.DM_CaNhan_TrongTaiServices.Update.Command { Entity = EntityTrongTai });
        }

        [HttpPost]
        [Route("AddRelation")]
        public async Task<Result<DM_CaNhan_MonTheThao>> Add(DM_CaNhan_MonTheThao Entity)
        {
            return await Mediator.Send(new AddRelation.Command { Entity = Entity });
        }

        [HttpPut]
        [Route("UpdateRelation")]
        public async Task<Result<DM_CaNhan_MonTheThao>> UpdateRelation(DM_CaNhan_MonTheThao Entity)
        {
            return await Mediator.Send(new UpdateRelation.Command { Entity = Entity });
        }

        [HttpDelete]
        [Route("DeleteRelation")]
        public async Task<Result<int>> DeleteRelation(Guid CaNhanID, Guid MonTheThaoID)
        {
            return await Mediator.Send(new DeleteRelation.Command { CaNhanID = CaNhanID, MonTheThaoID = MonTheThaoID });
        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<Result<int>> Delete(Guid CaNhanID)
        {
            return await Mediator.Send(new Application.DM_CaNhan_TrongTaiServices.Delete.Command { CaNhanID = CaNhanID });
        }
    }
}
