using Dapper;
using Domain.Core;
using Domain.DM_CaNhan_BaoChiModel;
using Domain.DM_CaNhan_HuongDanVienModel;
using Domain.DM_CaNhan_VanDongVienModel;
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

namespace Application.DM_CaNhan_HuongDanVienServices
{
    public class Add
    {
        public class Command : IRequest<Result<DM_CaNhan_HuongDanVien>>
        {
            public DM_CaNhan_HuongDanVien_RequestInfo Entity;
        }

        public class Handler : IRequestHandler<Command, Result<DM_CaNhan_HuongDanVien>>
        {
            private readonly IConfiguration _configuration;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
            {
                _configuration = configuration;
                _httpContextAccessor = httpContextAccessor;
            }
            public async Task<Result<DM_CaNhan_HuongDanVien>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();
                    using (var transaction = connection.BeginTransaction())
                    {
                        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                        //Add cá nhân
                        try
                        {
                            DynamicParameters parameters = new DynamicParameters();
                            parameters.Add("@MaDinhDanh", request.Entity.MaDinhDanh);
                            parameters.Add("@NgaySinh", request.Entity.NgaySinh);
                            parameters.Add("@GioiTinhID", request.Entity.GioiTinhID);
                            parameters.Add("@DanTocID", request.Entity.DanTocID);
                            parameters.Add("@TinhID", request.Entity.TinhID);
                            parameters.Add("@XaID", request.Entity.XaID);
                            parameters.Add("@DienThoai", request.Entity.DienThoai);
                            parameters.Add("@HopThu", request.Entity.HopThu);
                            parameters.Add("@AnhChanDung", request.Entity.AnhChanDung);
                            parameters.Add("@TrangThaiID", request.Entity.TrangThaiID);
                            var result = await connection.QueryFirstOrDefaultAsync<DM_CaNhan_HuongDanVien>("spu_DM_CaNhan_Add", parameters, commandType: CommandType.StoredProcedure, transaction: transaction);

                            if (result != null)
                            {
                                //Add content cá nhan
                                try
                                {
                                    DynamicParameters parameterContentCaNhan = new DynamicParameters();
                                    parameterContentCaNhan.Add("@CaNhanID", result.CaNhanID);
                                    parameterContentCaNhan.Add("@MaNgonNgu", request.Entity.MaNgonNgu);
                                    parameterContentCaNhan.Add("@HoTen", request.Entity.HoTen);
                                    parameterContentCaNhan.Add("@DiaChi", request.Entity.DiaChi);
                                    parameterContentCaNhan.Add("@NoiLamViec", request.Entity.NoiLamViec);
                                    parameterContentCaNhan.Add("@MoTa", request.Entity.MoTa);
                                    parameterContentCaNhan.Add("@TrangThai", request.Entity.TrangThai);
                                    parameterContentCaNhan.Add("@NguoiCapNhat", userId);
                                    parameterContentCaNhan.Add("@GhiChu", request.Entity.GhiChu);
                                    var resultContentCaNhan = await connection.QueryFirstOrDefaultAsync<DM_CaNhan_HuongDanVien_NoiDung>("spu_DM_CaNhan_HandleInfo", parameterContentCaNhan, commandType: CommandType.StoredProcedure, transaction: transaction);
                                }
                                catch (Exception ex)
                                {
                                    transaction.Rollback();
                                    return Result<DM_CaNhan_HuongDanVien>.Failure(ex.Message);
                                }

                               
                                //Add content báo chí
                                try
                                {
                                    DynamicParameters parametersBaoChi = new DynamicParameters();
                                    parametersBaoChi.Add("@CaNhanID", result.CaNhanID);
                                    parametersBaoChi.Add("@MaNgonNgu", request.Entity.MaNgonNgu);
                                    parametersBaoChi.Add("@SoThe", request.Entity.SoThe);
                                    parametersBaoChi.Add("@NgayHetHan", request.Entity.NgayHetHan);
                                    parametersBaoChi.Add("@LoaiTheID", request.Entity.LoaiTheID);
                                    parametersBaoChi.Add("@NamKinhNghiem", request.Entity.NamKinhNghiem);
                                    parametersBaoChi.Add("@NoiCapID", request.Entity.NoiCapID);
                                   
                                    var resultVDV = await connection.QueryFirstOrDefaultAsync<DM_CaNhan_NoiDungHuongDanVien>("spu_DM_CaNhan_HuongDanVien_HandleInfo", parametersBaoChi, commandType: CommandType.StoredProcedure, transaction: transaction);
                                }
                                catch (Exception ex)
                                {
                                    transaction.Rollback();
                                    return Result<DM_CaNhan_HuongDanVien>.Failure(ex.Message);

                                }

                                //Add relation CaNhan <=> ToChuc

                                if (request.Entity.ToChucID != null)
                                {

                                    try
                                    {
                                        DynamicParameters parametersCaNhanToChuc = new DynamicParameters();
                                        parametersCaNhanToChuc.Add("@CaNhanID", result.CaNhanID);
                                        parametersCaNhanToChuc.Add("@ToChucID", request.Entity.ToChucID);
                                        var resultCaNhanToChuc = await connection.QueryFirstOrDefaultAsync<DM_CaNhan_ToChuc>("spu_DM_CaNhan_ToChuc_Add", parameters, commandType: CommandType.StoredProcedure, transaction: transaction);

                                    }
                                    catch (Exception ex)
                                    {
                                        transaction.Rollback();
                                        return Result<DM_CaNhan_HuongDanVien>.Failure(ex.Message);
                                    }
                                }
                            }

                            transaction.Commit();
                            return Result<DM_CaNhan_HuongDanVien>.Success(result);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Result<DM_CaNhan_HuongDanVien>.Failure(ex.Message);
                        }
                    }
                }
            }
        }
    }
}
