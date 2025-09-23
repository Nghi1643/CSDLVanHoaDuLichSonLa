
using Application.DM_CaNhan_VanDongVienServices;
using Domain;
using Domain.Core;
using Domain.DM_CaNhan_NgheSiModel;
using Domain.DM_CaNhan_VanDongVienModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static Dapper.SqlMapper;

namespace CSDLVanHoaDuLichSonLa.Controllers.API
{
    public class DM_CaNhan_VanDongVienApiController : BaseApiController
    {
        const string FilePath = "uploads/VanDongVien";
        public DM_CaNhan_VanDongVienApiController(IWebHostEnvironment hostingEnvironment, IConfiguration config) : base(hostingEnvironment, config)
        {
        }

        [HttpGet]
        [Route("Gets")]
        public async Task<Result<IEnumerable<DM_CaNhan_VanDongVienViewModel>>> Gets (string MaNgonNgu)
        {
            return await Mediator.Send(new Gets.Query { MaNgonNgu = MaNgonNgu });
        }

        [HttpGet]
        [Route("Get")]
        public async Task<Result<DM_CaNhan_VanDongVienViewModel>> Get(string MaNgonNgu, Guid CaNhanID)
        {
            return await Mediator.Send(new Get.Query { MaNgonNgu = MaNgonNgu, CaNhanID = CaNhanID });
        }

        [HttpGet]
        [Route("Filter")]
        public async Task<Result<IEnumerable<DM_CaNhan_VanDongVienViewModel>>> Filter(string TuKhoa = null, Guid? ToChucID = null, int? GioiTinhID = null, bool? TrangThai = null, string MaNgonNgu = "vi")
        {
            return await Mediator.Send(new Filter.Query { TuKhoa = TuKhoa, ToChucID = ToChucID, GioiTinhID = GioiTinhID, TrangThai = TrangThai, MaNgonNgu = MaNgonNgu });
        }

        [HttpGet]
        [Route("GetListMon")]
        public async Task<Result<IEnumerable<DM_CaNhan_MonTheThaoList>>> GetListMon(Guid CaNhanID, string MaNgonNgu)
        {
            return await Mediator.Send(new GetListMonByCaNhanID.Query { CaNhanID = CaNhanID, MaNgonNgu = MaNgonNgu });
        }

        [HttpPost]
        [Route("Add")]
        public async Task<Result<DM_CaNhan>> Add([FromForm] DM_CaNhanVDV_AddRequest _request)
        {
            if(_request.RequestVanDongVien == null)
            {
                return Result<DM_CaNhan>.Failure("Dữ liệu đầu vào không hợp lệ");
            }

            DM_CaNhanRequestHandleInfo EntityVanDongVien = JsonConvert.DeserializeObject<DM_CaNhanRequestHandleInfo>(_request.RequestVanDongVien);
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
                    EntityVanDongVien.AnhChanDung = UploadFile.Url;
                }
            }

            return await Mediator.Send(new Add.Command { Entity = EntityVanDongVien, EntityTheThao = EntityTheThao });
        }


        [HttpPut]
        [Route("Update")]
        public async Task<Result<DM_CaNhan>> Update([FromForm] DM_CaNhanVDV_UpdateRequest _request)
        {
            if (_request.RequestVanDongVien == null)
            {
                return Result<DM_CaNhan>.Failure("Dữ liệu đầu vào không hợp lệ");
            }

            DM_CaNhanRequestHandleInfo EntityVanDongVien = JsonConvert.DeserializeObject<DM_CaNhanRequestHandleInfo>(_request.RequestVanDongVien);

            if (_request.File != null)
            {
                if(EntityVanDongVien.AnhChanDungDetail != null)
                {
                    DeleteFileObject(EntityVanDongVien.AnhChanDungDetail);
                }

                UploadFileResult UploadFile = await SaveFileUpload(_request.File, FilePath);
                if (UploadFile.Success && UploadFile.Url != null)
                {
                    EntityVanDongVien.AnhChanDung = UploadFile.Url;
                }
            }
            else
            {
                EntityVanDongVien.AnhChanDung = EntityVanDongVien.AnhChanDungDetail;
            }

                return await Mediator.Send(new Update.Command { Entity = EntityVanDongVien });
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
            return await Mediator.Send(new Delete.Command { CaNhanID = CaNhanID });
        }
    }
}
