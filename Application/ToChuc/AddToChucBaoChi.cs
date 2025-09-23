using Dapper;
using Domain.Core;
using Domain.ToChuc;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.ToChuc
{
    public class AddToChucBaoChi
    {
        public class Command : IRequest<Result<Domain.ToChuc.ToChuc>>
        {
            public DM_ToChucBaseRequest EntityToChuc;
            public List<BC_AnPhamBaseRequest> EntityAnPham;
        }

        public class Handler : IRequestHandler<Command, Result<Domain.ToChuc.ToChuc>>
        {
            private readonly IConfiguration _configuration;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
            {
                _configuration = configuration;
                _httpContextAccessor = httpContextAccessor;
            }
            public async Task<Result<Domain.ToChuc.ToChuc>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            DynamicParameters parameters = new DynamicParameters();
                            parameters.Add("@ToChucID", request.EntityToChuc.ToChucID);
                            parameters.Add("@LoaiToChucID", request.EntityToChuc.LoaiToChucID);
                            parameters.Add("@LinhVucID", request.EntityToChuc.LinhVucID);
                            parameters.Add("@MaDinhDanh", request.EntityToChuc.MaDinhDanh);
                            parameters.Add("@TrangThaiID", request.EntityToChuc.TrangThaiID);
                            parameters.Add("@HopThu", request.EntityToChuc.HopThu);
                            parameters.Add("@DienThoai", request.EntityToChuc.DienThoai);
                            parameters.Add("@Website", request.EntityToChuc.Website);
                            parameters.Add("@NgayThanhLap", request.EntityToChuc.NgayThanhLap);
                            parameters.Add("@SoGiayPhepHoatDong", request.EntityToChuc.SoGiayPhepHoatDong);
                            parameters.Add("@CoQuanChuQuanID", request.EntityToChuc.CoQuanChuQuanID);
                            parameters.Add("@PhamViHoatDongID", request.EntityToChuc.PhamViHoatDongID);
                            parameters.Add("@ToaDoX", request.EntityToChuc.ToaDoX);
                            parameters.Add("@ToaDoY", request.EntityToChuc.ToaDoY);
                            parameters.Add("@LoaiHinhID", request.EntityToChuc.LoaiHinhID);
                            parameters.Add("@TinhID", request.EntityToChuc.TinhID);
                            parameters.Add("@XaID", request.EntityToChuc.XaID);
                            parameters.Add("@QuyMo", request.EntityToChuc.QuyMo);
                            var result = await connection.QueryFirstOrDefaultAsync<Domain.ToChuc.ToChuc>(
                               "spu_DM_ToChuc_AddEdit",
                               parameters,
                               commandType: CommandType.StoredProcedure,
                               transaction: transaction
                            );

                            if(result != null)
                            {
                                var parametersNoiDung = new DynamicParameters();
                                parametersNoiDung.Add("@ID", request.EntityToChuc.ID);
                                parametersNoiDung.Add("@ToChucID", result.ToChucID);
                                parametersNoiDung.Add("@MaNgonNgu", request.EntityToChuc.MaNgonNgu);
                                parametersNoiDung.Add("@TenToChuc", request.EntityToChuc.TenToChuc);
                                parametersNoiDung.Add("@DiaChi", request.EntityToChuc.DiaChi);
                                parametersNoiDung.Add("@GioiThieu", request.EntityToChuc.GioiThieu);
                                parametersNoiDung.Add("@GhiChu", request.EntityToChuc.GhiChu);

                                var resultNoiDung = await connection.QueryFirstOrDefaultAsync<ToChuc_NoiDung>(
                                    "spu_DM_ToChuc_NoiDung_AddEdit",
                                    parametersNoiDung,
                                    commandType: CommandType.StoredProcedure,
                                    transaction: transaction
                                );

                                if (request.EntityAnPham.Count() > 0)
                                {
                                    foreach (var item in request.EntityAnPham) 
                                    {
                                        // lấy userId
                                        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                                        var parametersAnPham = new DynamicParameters();
                                        parametersAnPham.Add("@BaoChiAnPhamID",item.BaoChiAnPhamID);
                                        parametersAnPham.Add("@TenAnPham", item.TenAnPham);
                                        parametersAnPham.Add("@ToChucID", result.ToChucID);
                                        parametersAnPham.Add("@TanSuatID", item.TanSuatID);
                                        parametersAnPham.Add("@LinhVucChuyenSauID", item.LinhVucChuyenSauID);
                                        parametersAnPham.Add("@SoLuong", item.SoLuong);
                                        parametersAnPham.Add("@NguoiCapNhat", userId);
                                        parametersAnPham.Add("@NguoiHieuChinh", userId);
                                        parametersAnPham.Add("@TrangThai", item.TrangThai);

                                        var resultAnPham = await connection.QueryFirstOrDefaultAsync<Domain.BaoChi.BaoChiAnPham>(
                                            "spu_BC_BaoChiAnPham_AddEdit",
                                            parametersAnPham,
                                            commandType: CommandType.StoredProcedure,
                                            transaction: transaction
                                        );

                                        if(resultAnPham != null)
                                        {
                                            var parametersNoiDungAnPham = new DynamicParameters();
                                            parametersNoiDungAnPham.Add("@BaoChiAnPhamID", resultAnPham.BaoChiAnPhamID);
                                            parametersNoiDungAnPham.Add("@MaNgonNgu", item.MaNgonNgu);
                                            parametersNoiDungAnPham.Add("@TenAnPham", item.TenAnPham);
                                            parametersNoiDungAnPham.Add("@DoiTuongDocGia", item.DoiTuongDocGia);
                                            parametersNoiDungAnPham.Add("@LinhVucChuyenSau", item.LinhVucChuyenSau);

                                            var resultNoiDungAnPham = await connection.QueryFirstOrDefaultAsync<Domain.BaoChi.BaoChiAnPham_NoiDung>(
                                                "spu_BC_BaoChiAnPham_NoiDung_AddEdit",
                                                parametersNoiDungAnPham,
                                                commandType: CommandType.StoredProcedure,
                                                transaction: transaction
                                            );
                                        }

                                    }
                                }
                            }

                            transaction.Commit(); // ok
                            return Result<Domain.ToChuc.ToChuc>.Success(result);

                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback(); // kó
                            return Result<Domain.ToChuc.ToChuc>.Failure(ex.Message);
                        }
                        finally
                        {
                            await connection.CloseAsync();
                        }
                    }
                }
            }
        }
    }
}
