using Dapper;
using Domain.Core;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.BaoChi.GiaiThuong
{
    /// <summary>
    /// Xóa giải thưởng
    /// SP: spu_BC_GiaiThuong_Delete
    /// </summary>
    public class Xoa
    {
        public class Command : IRequest<Result<bool>>
        {
            public Guid ID { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<bool>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config)
            {
                _config = config;
            }

            public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync(cancellationToken);
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("@GiaiThuongID", request.ID);

                        var affectedRows = await connection.ExecuteAsync(
                            "spu_BC_GiaiThuong_Delete",
                            parameters,
                            commandType: System.Data.CommandType.StoredProcedure
                        );

                        return Result<bool>.Success(affectedRows > 0);
                    }
                    catch (Exception ex)
                    {
                        return Result<bool>.Failure(ex.Message);
                    }
                }
            }
        }
    }
}
