using Dapper;
using Domain;
using Domain.Core;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Audit
{
    public class GhiLog
    {
        public class Command : IRequest<Result<Domain.CSDL_Log>>
        {
            public CSDL_Log Data { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Domain.CSDL_Log>>
        {
            private readonly IConfiguration _config;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IConfiguration config, IHttpContextAccessor httpContextAccessor)
            {
                _config = config;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<Result<Domain.CSDL_Log>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync(cancellationToken);
                    try
                    {
                        // lấy userId, ip, agent
                        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        var ipAddress = _httpContextAccessor.HttpContext?.Request?.Headers["X-Forwarded-For"].FirstOrDefault()
                                        ?? _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
                        var userAgent = _httpContextAccessor.HttpContext?.Request?.Headers["User-Agent"].ToString();

                        var parameters = new DynamicParameters();
                        parameters.Add("@ObjectID", request.Data.ObjectID);
                        parameters.Add("@TableName", request.Data.TableName);
                        parameters.Add("@Action", request.Data.Action);
                        parameters.Add("@OldData", request.Data.OldData);
                        parameters.Add("@NewData", request.Data.NewData);
                        parameters.Add("@CreatedByUserID", userId);
                        parameters.Add("@IpAddress", ipAddress);
                        parameters.Add("@UserAgent", userAgent);

                        var result = await connection.QueryFirstOrDefaultAsync<Domain.CSDL_Log>(
                            "spu_CSDL_Log_Add",
                            parameters,
                            commandType: CommandType.StoredProcedure
                        );

                        return Result<Domain.CSDL_Log>.Success(result);
                    }
                    catch (Exception ex)
                    {
                        return Result<Domain.CSDL_Log>.Failure(ex.Message);
                    }
                }
            }
        }
    }
}
