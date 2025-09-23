
using Application.DaPhuongTien;
using Application.VH_LeHoiServices;
using Domain;
using Domain.Core;
using Domain.DanhMuc;
using Domain.ToChuc;
using Domain.VH_LeHoiModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CSDLVanHoaDuLichSonLa.Controllers.API
{
    public class VH_LeHoiApiController : BaseApiController
    {
        const string UploadPath = "uploads/DaPhuongTien/LeHoi";
        public VH_LeHoiApiController(IWebHostEnvironment hostingEnvironment, IConfiguration config) : base(hostingEnvironment, config)
        {
        }

        [HttpGet]
        [Route("Gets")]
        public async Task<Result<IEnumerable<VH_LeHoiViewModel>>> Gets(string MaNgonNgu)
        {
            return await Mediator.Send(new Gets.Query { MaNgonNgu = MaNgonNgu });
        }

        [HttpGet]
        [Route("Get")]
        public async Task<Result<VH_LeHoiViewModel>> Get(string MaNgonNgu, Guid LeHoiID)
        {
            return await Mediator.Send(new Get.Query { MaNgonNgu = MaNgonNgu, LeHoiID = LeHoiID });
        }

        [HttpPost]
        [Route("Add")]
        public async Task<Result<VH_LeHoi>> Add([FromForm]VH_LeHoi_RequestMulti Entity)
        {
            //Defined instance obejct
            VH_LeHoi_RequestAdd EntityLeHoi = JsonConvert.DeserializeObject<VH_LeHoi_RequestAdd>(Entity.EntityLeHoi);

            //Check value input LeHoi
            if (Entity.EntityLeHoi == null)
            {
                return Result<VH_LeHoi>.Failure("Dữ liệu đầu vào không hợp lệ");
            }

            //Add LeHoi
            var result = await Mediator.Send(new Add.Command { RequestLeHoi = EntityLeHoi});

            //Check services add LeHoi
            if (result.IsSuccess == false && result.Value == null)
            {
                return Result<VH_LeHoi>.Failure("Thêm mới không thành công");
            }

            //Check value input DaPhuongTien
            if (Entity.EntityDaPhuongTien != null)
            {
                foreach(var item in Entity.EntityDaPhuongTien)
                {
                    DaPhuongTien daPhuongTien = JsonConvert.DeserializeObject<DaPhuongTien>(item.DaPhuongTien);
                    List<DaPhuongTien_NoiDung> banDich = JsonConvert.DeserializeObject<List<DaPhuongTien_NoiDung>>(item.DaPhuongTien_NoiDung);
                    if (item.File != null)
                    {
                        var file = item.File;

                        UploadFileResult ufile = await SaveFileUpload(file, UploadPath);
                        if (ufile.Success == false)
                        {
                            return Result<VH_LeHoi>.Failure(ufile.Message);
                        }

                        daPhuongTien.DuongDanFile = ufile.Url;
                        daPhuongTien.DoiTuongSoHuuID = result.Value.LeHoiID;
                    }

                    var resultDaPhuongTien = await Mediator.Send(new ThemMoiChinhSua.Command { Data = daPhuongTien, NoiDungBanDich = banDich });
                }
            }

          

           

          

            return result;

        }

        [HttpPost]
        [Route("HandleContent")]
        public async Task<Result<VH_LeHoi_NoiDung>> HandleContent(VH_LeHoi_NoiDung_Request Entity)
        {
            return await Mediator.Send(new HandleContent.Command { Entity = Entity });
        }

        [HttpPut]
        [Route("Update")]
        public async Task<Result<VH_LeHoi>> Update(VH_LeHoi_RequestUpdate Entity)
        {
            return await Mediator.Send(new Update.Command { Entity = Entity });
        }


        [HttpDelete]
        [Route("Delete")]
        public async Task<Result<int>> Delete(Guid LeHoiID)
        {
            return await Mediator.Send(new Delete.Command { LeHoiID = LeHoiID });
        }

        [HttpDelete]
        [Route("DeleteContent")]
        public async Task<Result<int>> DeleteContent(Guid LeHoiNoiDungID)
        {
            return await Mediator.Send(new DeleteContent.Command { LeHoiNoiDungID = LeHoiNoiDungID });
        }
    }
}
