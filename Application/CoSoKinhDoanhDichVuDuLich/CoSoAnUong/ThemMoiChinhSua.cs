using Dapper;
using Domain.Core;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Application.CoSoKinhDoanhDichVuDuLich.CoSoAnUong
{
    public class ThemMoiChinhSua
    {
        public class Command : IRequest<Result<Domain.CoSoKinhDoanhDichVuDuLich.CoSoAnUong>>
        {
            public Domain.CoSoKinhDoanhDichVuDuLich.CoSoAnUong CoSoAnUong { get; set; }
            public List<Domain.CoSoKinhDoanhDichVuDuLich.CoSoAnUong_NoiDung> NoiDungBanDich { get; set; }
        }
        public class Handler : IRequestHandler<Command, Result<Domain.CoSoKinhDoanhDichVuDuLich.CoSoAnUong>>
        {
            private readonly IConfiguration _config;
            private readonly IMediator _mediator;
            private readonly IHttpContextAccessor _httpContextAccessor;
            public Handler(IConfiguration config, IMediator mediator, IHttpContextAccessor httpContextAccessor)
            {
                _config = config;
                _mediator = mediator;
                _httpContextAccessor = httpContextAccessor;
            }
            public async Task<Result<Domain.CoSoKinhDoanhDichVuDuLich.CoSoAnUong>> Handle(Command request, CancellationToken cancellationToken)
            {

                await using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync(cancellationToken);
                await using var transaction = connection.BeginTransaction();
                try
                {
                    var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var parameters = new DynamicParameters();
                    parameters.Add("@CoSoAnUongID", request.CoSoAnUong.CoSoAnUongID);
                    parameters.Add("@DiaDiemID", request.CoSoAnUong.DiaDiemID);
                    parameters.Add("@LoaiDichVuAnUongID", request.CoSoAnUong.LoaiDichVuAnUongID);
                    parameters.Add("@SoLuotDanhGia", request.CoSoAnUong.SoLuotDanhGia);
                    parameters.Add("@DiemDanhGia", request.CoSoAnUong.DiemDanhGia);
                    parameters.Add("@SucChua", request.CoSoAnUong.SucChua);
                    parameters.Add("@TiecGiaDinh", request.CoSoAnUong.TiecGiaDinh);
                    parameters.Add("@TiecDongNguoi", request.CoSoAnUong.TiecDongNguoi);
                    parameters.Add("@CoPhongRieng", request.CoSoAnUong.CoPhongRieng);
                    parameters.Add("@CoVuiChoiTreEm", request.CoSoAnUong.CoVuiChoiTreEm);
                    parameters.Add("@ChuanDuLich", request.CoSoAnUong.ChuanDuLich);
                    parameters.Add("@DienThoai", request.CoSoAnUong.DienThoai);
                    parameters.Add("@HopThu", request.CoSoAnUong.HopThu);
                    parameters.Add("@LienKet", request.CoSoAnUong.LienKet);
                    parameters.Add("@NgayNghiHangTuan", request.CoSoAnUong.NgayNghiHangTuan);
                    parameters.Add("@GioMoCua", request.CoSoAnUong.GioMoCua);
                    parameters.Add("@GioDongCua", request.CoSoAnUong.GioDongCua);
                    parameters.Add("@TrangThai", request.CoSoAnUong.TrangThai);
                    parameters.Add("@NguoiTao", userId);

                    var result = await connection.QueryFirstOrDefaultAsync<Domain.CoSoKinhDoanhDichVuDuLich.CoSoAnUong>(
                        "spu_DL_CoSoAnUong_AddEdit",
                        parameters,
                        transaction: transaction,
                        commandType: CommandType.StoredProcedure);
                    
                    if (result != null)
                    {
                        if (request.NoiDungBanDich != null && request.NoiDungBanDich.Count > 0)
                        {
                            foreach (var item in request.NoiDungBanDich)
                            {
                                var parametersNoiDung = new DynamicParameters();
                                parametersNoiDung.Add("@CoSoAnUongID", result.CoSoAnUongID);
                                parametersNoiDung.Add("@MaNgonNgu", item.MaNgonNgu);
                                parametersNoiDung.Add("@TenCoSo", item.TenCoSo);
                                parametersNoiDung.Add("@MoTa", item.MoTa);
                                parametersNoiDung.Add("@DiaChi", item.DiaChi);
                                parametersNoiDung.Add("@NguoiDaiDien", item.NguoiDaiDien);
                                var resultNoiDung = await connection.QueryFirstOrDefaultAsync<Domain.CoSoKinhDoanhDichVuDuLich.CoSoAnUong_NoiDung>(
                                    "spu_DL_CoSoAnUong_NoiDung_AddEdit",
                                    parametersNoiDung,
                                    transaction: transaction,
                                    commandType: CommandType.StoredProcedure);
                            }
                        }
                    }
                    transaction.Commit();
                    return Result<Domain.CoSoKinhDoanhDichVuDuLich.CoSoAnUong>.Success(result);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Result<Domain.CoSoKinhDoanhDichVuDuLich.CoSoAnUong>.Failure(ex.Message );
                }
            }
        }
    }
}