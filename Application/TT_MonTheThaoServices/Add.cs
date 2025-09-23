using Dapper;
using Domain.Core;
using Domain.ToChuc;
using Domain.TT_MonTheThaoModel;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.TT_MonTheThaoServices
{
    public class Add
    {
        public class Command : IRequest<Result<TT_MonTheThao>>
        {
            public TT_MonTheThao_AddRequest Entity;
        }

        public class Handler : IRequestHandler<Command, Result<TT_MonTheThao>>
        {
            private readonly IConfiguration _configuration;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
            {
                _configuration = configuration;
                _httpContextAccessor = httpContextAccessor;
            }
            public async Task<Result<TT_MonTheThao>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();
                    try
                    {
                        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@MaDinhDanh", request.Entity.MaDinhDanh);
                        parameters.Add("@CachThiDau", request.Entity.CachThiDauID);
                        parameters.Add("@TepKemTheo", request.Entity.TepKemTheo);
                        parameters.Add("@TrangThaiID", request.Entity.TrangThaiID);
                        parameters.Add("@NguoiCapNhat", userId);
                        parameters.Add("@TrangThai", request.Entity.TrangThai);
                        var result = await connection.QueryFirstOrDefaultAsync<TT_MonTheThao>("spu_TT_MonTheThao_Add", parameters, commandType: CommandType.StoredProcedure);
                        if (result != null)
                        {
                            DynamicParameters parameterContent = new DynamicParameters();
                            parameterContent.Add("@MonTheThaoID", result.MonTheThaoID);
                            parameterContent.Add("@MaNgonNgu", request.Entity.MaNgonNgu);
                            parameterContent.Add("@TenMon", request.Entity.TenMon);
                            parameterContent.Add("@LuatChoi", request.Entity.LuatChoi);
                            parameterContent.Add("@MoTa", request.Entity.MoTa);
                            var resultContent = await connection.QueryFirstOrDefaultAsync<TT_MonTheThao_NoiDung>("spu_TT_MonTheThao_NoiDung_Add", parameterContent, commandType: CommandType.StoredProcedure);
                            if (resultContent == null)
                            {
                                return Result<TT_MonTheThao>.Failure("Thêm mới nội dung không thành công");
                            }

                            return Result<TT_MonTheThao>.Success(result);
                        }

                        return Result<TT_MonTheThao>.Failure("Thêm mới môn thể thao không thành công");
                    }
                    catch (Exception ex) {
                        return Result<TT_MonTheThao>.Failure(ex.Message);
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
