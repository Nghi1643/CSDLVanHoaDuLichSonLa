
using Application.DM_CaNhan_HuongDanVienServices;
using Domain;
using Domain.Core;
using Domain.DM_CaNhan_BaoChiModel;
using Domain.DM_CaNhan_HuongDanVienModel;
using Domain.DM_CaNhan_VanDongVienModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CSDLVanHoaDuLichSonLa.Controllers.API
{
    public class DM_CaNhan_HuongDanVienApiController : BaseApiController
    {
        const string FilePath = "uploads/HuongDanVien";
        public DM_CaNhan_HuongDanVienApiController(IWebHostEnvironment hostingEnvironment, IConfiguration config) : base(hostingEnvironment, config)
        {
        }

        [HttpGet]
        [Route("Gets")]
        public async Task<Result<IEnumerable<DM_CaNhan_HuongDanVienViewModel>>> Gets(string MaNgonNgu)
        {
            return await Mediator.Send(new Gets.Query { MaNgonNgu = MaNgonNgu });
        }

        [HttpGet]
        [Route("Get")]
        public async Task<Result<DM_CaNhan_HuongDanVienViewModel>> Get(string MaNgonNgu, Guid CaNhanID)
        {
            return await Mediator.Send(new Get.Query { MaNgonNgu = MaNgonNgu, CaNhanID = CaNhanID });
        }

        [HttpGet]
        [Route("Filter")]
        public async Task<Result<IEnumerable<DM_CaNhan_HuongDanVienViewModel>>> Filter(string TuKhoa = null, Guid? ToChucID = null, int? GioiTinhID = null, bool? TrangThai = null, string MaNgonNgu = "vi")
        {
            return await Mediator.Send(new Filter.Query { TuKhoa = TuKhoa, ToChucID = ToChucID, GioiTinhID = GioiTinhID, TrangThai = TrangThai, MaNgonNgu = MaNgonNgu });
        }

        [HttpPost]
        [Route("Add")]
        public async Task<Result<DM_CaNhan_HuongDanVien>> Add (DM_CaNhanHuongDanVienRequestInfo _request)
        {
            if(_request.RequestHuongDanVien == null)
            {
                return Result<DM_CaNhan_HuongDanVien>.Failure("Dữ liệu đầu vào không hợp lệ");
            }

            DM_CaNhan_HuongDanVien_RequestInfo EntityHuongDanVien = JsonConvert.DeserializeObject<DM_CaNhan_HuongDanVien_RequestInfo>(_request.RequestHuongDanVien);
            if (_request.File != null)
            {
                UploadFileResult UploadFile = await SaveFileUpload(_request.File, FilePath);
                if (UploadFile.Success && UploadFile.Url != null)
                {
                    EntityHuongDanVien.AnhChanDung = UploadFile.Url;
                }
            }

            return await Mediator.Send(new Add.Command { Entity = EntityHuongDanVien });
        }

        [HttpPut]
        [Route("Update")]
        public async Task<Result<DM_CaNhan_HuongDanVien>> Update([FromForm] DM_CaNhanHuongDanVienRequestInfo _request)
        {
            if (_request.RequestHuongDanVien == null)
            {
                return Result<DM_CaNhan_HuongDanVien>.Failure("Dữ liệu đầu vào không hợp lệ");
            }

            DM_CaNhan_HuongDanVien_RequestInfo EntityHuongDanVien = JsonConvert.DeserializeObject<DM_CaNhan_HuongDanVien_RequestInfo>(_request.RequestHuongDanVien);

            if (_request.File != null)
            {
                if (EntityHuongDanVien.AnhChanDungDetail != null)
                {
                    DeleteFileObject(EntityHuongDanVien.AnhChanDungDetail);
                }

                UploadFileResult UploadFile = await SaveFileUpload(_request.File, FilePath);
                if (UploadFile.Success && UploadFile.Url != null)
                {
                    EntityHuongDanVien.AnhChanDung = UploadFile.Url;
                }
            }
            else
            {
                EntityHuongDanVien.AnhChanDung = EntityHuongDanVien.AnhChanDungDetail;
            }

            return await Mediator.Send(new Update.Command { Entity = EntityHuongDanVien });
        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<Result<int>> Delete(Guid CaNhanID)
        {
            return await Mediator.Send(new Delete.Command { CaNhanID = CaNhanID });
        }
    }
}
