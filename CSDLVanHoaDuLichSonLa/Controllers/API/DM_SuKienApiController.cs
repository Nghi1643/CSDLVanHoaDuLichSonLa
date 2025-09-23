
using Application.DaPhuongTien;
using Application.DM_SuKienServices;
using Domain;
using Domain.Core;
using Domain.DanhMuc;
using Domain.DM_SuKienModel;
using Domain.VH_LeHoiModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CSDLVanHoaDuLichSonLa.Controllers.API
{
    public class DM_SuKienApiController : BaseApiController
    {
        const string FilePath = "uploads/DaPhuongTien/SuKien";
        const string FilePathAvatar = "uploads/SuKien";
        public DM_SuKienApiController(IWebHostEnvironment hostingEnvironment, IConfiguration config) : base(hostingEnvironment, config)
        {
        }

        [HttpGet]
        [Route("Gets")]
        public async Task<Result<IEnumerable<DM_SuKienViewModel>>> Gets(string MaNgonNgu, int LinhVucID)
        {
            return await Mediator.Send(new Gets.Query { MaNgonNgu = MaNgonNgu, LinhVucID = LinhVucID });
        }

        [HttpGet]
        [Route("Get")]
        public async Task<Result<DM_SuKienViewModel>> Get(string MaNgonNgu, Guid SuKienID)
        {
            return await Mediator.Send(new Get.Query { MaNgonNgu = MaNgonNgu, SuKienID = SuKienID });
        }

        [HttpGet]
        [Route("Filter")]
        public async Task<Result<IEnumerable<DM_SuKienViewModel>>> Filter(string TuKhoa, int? CapDoID, DateTime? NgayBatDau, DateTime? NgayKetThuc, bool? TrangThai, string MaNgonNgu, int LinhVucID)
        {
            return await Mediator.Send(new Filter.Query { TuKhoa = TuKhoa, CapDoID = CapDoID, NgayBatDau = NgayBatDau, NgayKetThuc = NgayKetThuc, TrangThai = TrangThai, MaNgonNgu = MaNgonNgu, LinhVucID = LinhVucID });
        }

        [HttpPost]
        [Route("Add")]
        public async Task<Result<DM_SuKien>> Add([FromForm] DM_SuKien_RequestAdd _request)
        {
            if (_request.EntitySuKien == null)
            {
                return Result<DM_SuKien>.Failure("Dữ liệu đầu vào không hợp lệ");
            }

            DM_SuKienModalAdd Entity = JsonConvert.DeserializeObject<DM_SuKienModalAdd>(_request.EntitySuKien);

            string AnhDaiDien = null;

            if (_request.File != null)
            {
                UploadFileResult ufile = await SaveFileUpload(_request.File, FilePathAvatar);
                if (ufile.Success == false)
                {
                    return Result<DM_SuKien>.Failure(ufile.Message);
                }

                AnhDaiDien = ufile.Url;
            }

            Entity.AnhDaiDien = AnhDaiDien;

            var result = await Mediator.Send(new Add.Command { Entity = Entity });

            if (result.IsSuccess == false && result.Value == null)
            {
                return Result<DM_SuKien>.Failure("Thêm mới sự kiện không thành công");
            }

            //Check value input DaPhuongTien
            if (_request.EntityDaPhuongTien != null)
            {
                foreach (var item in _request.EntityDaPhuongTien)
                {
                    DaPhuongTien daPhuongTien = JsonConvert.DeserializeObject<DaPhuongTien>(item.DaPhuongTien);
                    List<DaPhuongTien_NoiDung> banDich = JsonConvert.DeserializeObject<List<DaPhuongTien_NoiDung>>(item.DaPhuongTien_NoiDung);
                    if (item.File != null)
                    {
                        var file = item.File;

                        UploadFileResult ufile = await SaveFileUpload(file, FilePath);
                        if (ufile.Success == false)
                        {
                            return Result<DM_SuKien>.Failure(ufile.Message);
                        }

                        daPhuongTien.DuongDanFile = ufile.Url;
                        daPhuongTien.DoiTuongSoHuuID = result.Value.SuKienID;
                    }

                    var resultDaPhuongTien = await Mediator.Send(new ThemMoiChinhSua.Command { Data = daPhuongTien, NoiDungBanDich = banDich });
                }
            }

            return result;

        }

        [HttpPut]
        [Route("Update")]
        public async Task<Result<DM_SuKien>> Update([FromForm] DM_SuKien_RequestUpdate _request)
        {
            if (_request.EntitySuKien == null)
            {
                return Result<DM_SuKien>.Failure("Dữ liệu đầu vào không hợp lệ");
            }


            DM_SuKienModalUpdate Entity = JsonConvert.DeserializeObject<DM_SuKienModalUpdate>(_request.EntitySuKien);

            string AnhDaiDien = Entity.AnhDaiDienDetail;

            if (_request.File != null)
            {
                if (Entity.AnhDaiDienDetail != null)
                {
                    DeleteFileObject(Entity.AnhDaiDienDetail);
                }

                UploadFileResult UploadFile = await SaveFileUpload(_request.File, FilePath);
                if (UploadFile.Success && UploadFile.Url != null)
                {
                    Entity.AnhDaiDien = UploadFile.Url;
                }

            }
            else
            {
                Entity.AnhDaiDien = AnhDaiDien;
            }

                var result = await Mediator.Send(new Update.Command { Entity = Entity });
            return result;
        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<Result<int>> Delete (Guid SuKienID)
        {
            return await Mediator.Send(new Delete.Command { SuKienID = SuKienID });
        }

    }
}
