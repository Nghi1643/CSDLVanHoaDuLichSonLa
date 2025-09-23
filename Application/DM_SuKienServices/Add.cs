using Dapper;
using Domain.Core;
using Domain.DM_SuKienModel;
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

namespace Application.DM_SuKienServices
{
    public class Add
    {
        public class Command : IRequest<Result<DM_SuKien>>
        {
            public DM_SuKienModalAdd Entity;
        }

        public class Handler : IRequestHandler<Command, Result<DM_SuKien>>
        {
            private readonly IConfiguration _configuration;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
            {
                _configuration = configuration;
                _httpContextAccessor = httpContextAccessor;
            }
            public async Task<Result<DM_SuKien>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync(cancellationToken);
                    using (var transaction = connection.BeginTransaction())
                    {
                        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        try
                        {
                            DynamicParameters parameters = new DynamicParameters();
                            parameters.Add("@LinhVucID", request.Entity.LinhVucID);
                            parameters.Add("@MaDinhDanh", request.Entity.MaDinhDanh);
                            parameters.Add("@AnhDaiDien", request.Entity.AnhDaiDien);
                            parameters.Add("@CapDoID", request.Entity.CapDoID);
                            parameters.Add("@BatDau", request.Entity.BatDau);
                            parameters.Add("@KetThuc", request.Entity.KetThuc);
                            parameters.Add("@TrangThaiID", request.Entity.TrangThaiID);
                            parameters.Add("@NguoiCapNhat", userId);
                            parameters.Add("@ThuTu", request.Entity.ThuTu);
                            parameters.Add("@TrangThai", request.Entity.TrangThai);
                            var result = await connection.QueryFirstOrDefaultAsync<DM_SuKien>("spu_DM_SuKien_Add", parameters, commandType: CommandType.StoredProcedure, transaction: transaction);
                            if(result != null)
                            {
                                try
                                {
                                    DynamicParameters parametersContent = new DynamicParameters();
                                    parametersContent.Add("@SuKienID", result.SuKienID);
                                    parametersContent.Add("@MaNgonNgu", request.Entity.MaNgonNgu);
                                    parametersContent.Add("@TenSuKien", request.Entity.TenSuKien);
                                    parametersContent.Add("@MoTa", request.Entity.MoTa);
                                    parametersContent.Add("@KetQua", request.Entity.KetQua);
                                    parametersContent.Add("@NoiDung", request.Entity.NoiDung);
                                    var resultContent = await connection.QueryFirstOrDefaultAsync<DM_SuKien_NoiDung>("spu_DM_SuKien_NoiDung_HandleInfo", parametersContent, commandType: CommandType.StoredProcedure, transaction: transaction);

                                }
                                catch (Exception ex)
                                {
                                    transaction.Rollback();
                                    return Result<DM_SuKien>.Failure(ex.Message);
                                }

                                if(request.Entity.ListID != null && request.Entity.ListID != "") {
                                    try
                                    {
                                        DynamicParameters paramterRelation = new DynamicParameters();
                                        paramterRelation.Add("@SuKienID", result.SuKienID);
                                        paramterRelation.Add("@ListDiaDiem", request.Entity.ListID);
                                        var resultRelation = await connection.QueryFirstOrDefaultAsync<DM_SuKien_DiaDiem>("spu_DM_SuKien_DiaDiem_Add",paramterRelation, commandType: CommandType.StoredProcedure, transaction: transaction);
                                    }
                                    catch (Exception ex)
                                    {
                                        transaction.Rollback();
                                        return Result<DM_SuKien>.Failure(ex.Message);
                                    }
                                }
                            }
                            transaction.Commit();
                            return Result<DM_SuKien>.Success(result);
                        }catch(Exception ex)
                        {
                            transaction.Rollback();
                            return Result<DM_SuKien>.Failure(ex.Message);
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
