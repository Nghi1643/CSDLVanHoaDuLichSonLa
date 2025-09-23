using Dapper;
using Domain.BaoChi;
using Domain.Core;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Application.BaoChi.GiaiThuong
{
    /// <summary>
    /// Thêm mới / chỉnh sửa giải thưởng
    /// SP chính: spu_BC_GiaiThuong_AddEdit
    /// SP nội dung: spu_BC_GiaiThuong_NoiDung_AddEdit
    /// </summary>
    public class ThemMoiChinhSua
    {
        public class Command : IRequest<Result<Domain.BaoChi.GiaiThuong>>
        {
            public Domain.BaoChi.GiaiThuong GiaiThuong { get; set; }
            public List<GiaiThuong_NoiDung> NoiDungBanDich { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Domain.BaoChi.GiaiThuong>>
        {
            private readonly IConfiguration _config;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IConfiguration config, IHttpContextAccessor httpContextAccessor)
            {
                _config = config;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<Result<Domain.BaoChi.GiaiThuong>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync(cancellationToken);
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                            var parameters = new DynamicParameters();
                            parameters.Add("@GiaiThuongID", request.GiaiThuong.GiaiThuongID);
                            parameters.Add("@NamTraoGiai", request.GiaiThuong.NamTraoGiai);
                            parameters.Add("@LinhVucID", request.GiaiThuong.LinhVucID);
                            parameters.Add("@TenGiaiThuong", request.GiaiThuong.TenGiaiThuong);
                            parameters.Add("@TrangThai", request.GiaiThuong.TrangThai);
                            parameters.Add("@NgayCapNhat", request.GiaiThuong.NgayCapNhat);
                            parameters.Add("@NgayHieuChinh", request.GiaiThuong.NgayHieuChinh);
                            parameters.Add("@NguoiCapNhat", request.GiaiThuong.NguoiCapNhat);
                            parameters.Add("@NguoiHieuChinh", request.GiaiThuong.NguoiHieuChinh);

                            var result = await connection.QueryFirstOrDefaultAsync<Domain.BaoChi.GiaiThuong>(
                                "spu_BC_GiaiThuong_AddEdit",
                                parameters,
                                commandType: CommandType.StoredProcedure,
                                transaction: transaction
                            );

                            if (result != null && request.NoiDungBanDich != null)
                            {
                                foreach (var noiDung in request.NoiDungBanDich)
                                {
                                    var paramNoiDung = new DynamicParameters();
                                    paramNoiDung.Add("@GiaiThuongNoiDungID", noiDung.GiaiThuongNoiDungID);
                                    paramNoiDung.Add("@GiaiThuongID", result.GiaiThuongID);
                                    paramNoiDung.Add("@MaNgonNgu", noiDung.MaNgonNgu);
                                    paramNoiDung.Add("@TenGiaiThuong", noiDung.TenGiaiThuong);
                                    paramNoiDung.Add("@HangMucGiaiThuong", noiDung.HangMucGiaiThuong);
                                    paramNoiDung.Add("@MoTa", noiDung.MoTa);
                                    paramNoiDung.Add("@TrangThai", noiDung.TrangThai);
                                    paramNoiDung.Add("@NgayCapNhat", noiDung.NgayCapNhat);
                                    paramNoiDung.Add("@NgayHieuChinh", noiDung.NgayHieuChinh);
                                    paramNoiDung.Add("@NguoiCapNhat", noiDung.NguoiCapNhat);
                                    paramNoiDung.Add("@NguoiHieuChinh", noiDung.NguoiHieuChinh);

                                    await connection.QueryFirstOrDefaultAsync<GiaiThuong_NoiDung>(
                                        "spu_BC_GiaiThuong_NoiDung_AddEdit",
                                        paramNoiDung,
                                        commandType: CommandType.StoredProcedure,
                                        transaction: transaction
                                    );
                                }
                            }

                            transaction.Commit();
                            return Result<Domain.BaoChi.GiaiThuong>.Success(result);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Result<Domain.BaoChi.GiaiThuong>.Failure(ex.Message);
                        }
                    }
                }
            }
        }
    }
}
