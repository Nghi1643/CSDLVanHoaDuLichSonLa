
using Application.TT_MonTheThaoServices;
using Domain;
using Domain.Core;
using Domain.TT_MonTheThaoModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CSDLVanHoaDuLichSonLa.Controllers.API
{
    public class TT_MonTheThaoApiController : BaseApiController
    {
        string FilePath = "uploads/MonTheThao";
        public TT_MonTheThaoApiController(IWebHostEnvironment hostingEnvironment, IConfiguration config) : base(hostingEnvironment, config)
        {
        }

        [HttpGet]
        [Route("Gets")]
        public async Task<Result<IEnumerable<TT_MonTheThaoViewModel>>> Gets(string MaNgonNgu)
        {
            return await Mediator.Send(new Gets.Query { MaNgonNgu = MaNgonNgu });
        }

        [HttpGet]
        [Route("Get")]
        public async Task<Result<TT_MonTheThao>> Get(string MaNgonNgu, Guid MonTheThaoID)
        {
            return await Mediator.Send(new Get.Query { MaNgonNgu = MaNgonNgu, MonTheThaoID = MonTheThaoID });
        }

        [HttpGet]
        [Route("GetListForCaNhan")]
        public async Task<Result<IEnumerable<TT_MonTheThaoViewModel>>> GetListForCaNhan(Guid CaNhanID, string MaNgonNgu)
        {
            return await Mediator.Send(new GetListForCaNhan.Query { CaNhanID = CaNhanID, MaNgonNgu = MaNgonNgu });
        }

        [HttpPost]
        [Route("Add")]
        public async Task<Result<TT_MonTheThao>> Add([FromForm] TT_MonTheThaoRequestAddFile _request)
        {
            TT_MonTheThao_AddRequest Entity = JsonConvert.DeserializeObject<TT_MonTheThao_AddRequest>(_request.Data);
            if (_request.File != null)
            {
                UploadFileResult UploadFile = await SaveFileUpload(_request.File, FilePath);
                if (UploadFile.Success && UploadFile.Url != null)
                {
                    Entity.TepKemTheo = UploadFile.Url;
                }
            }
            return await Mediator.Send(new Add.Command { Entity = Entity });
        }

        [HttpPut]
        [Route("Update")]
        public async Task<Result<TT_MonTheThao>> Update([FromForm] TT_MonTheThaoRequestAddFile _request)
        {
            TT_MonTheThao_UpdateRequest Entity = JsonConvert.DeserializeObject<TT_MonTheThao_UpdateRequest>(_request.Data);
            if (_request.File != null)
            {
                if (Entity.TepKemTheoDetail != null)
                {
                    DeleteFileObject(Entity.TepKemTheoDetail);
                }

                UploadFileResult UploadFile = await SaveFileUpload(_request.File, FilePath);
                if (UploadFile.Success && UploadFile.Url != null)
                {
                    Entity.TepKemTheo = UploadFile.Url;
                }
            }
            else
            {
                Entity.TepKemTheo = Entity.TepKemTheoDetail;
            }
            return await Mediator.Send(new Update.Command { Entity = Entity });
        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<Result<int>> Delete(Guid MonTheThaoID)
        {
            return await Mediator.Send(new Delete.Command { MonTheThaoID = MonTheThaoID });
        }
    }
}
