using Dapper;
using Domain.Core;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.TourDuLich.LichTrinh
{
    public class Xoa
    {
        public class Command : IRequest<Result<bool>>
        {
            public int ID { get; set; }
        }
        public class Handler : IRequestHandler<Command, Result<bool>>
        {
            private readonly IConfiguration _config;
            private readonly IMediator _mediator;
            public Handler(IConfiguration config, IMediator mediator)
            {
                _config = config;
                _mediator = mediator;
            }
            public Handler()
            {
            }
            public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
            {
                await using var connection = new Microsoft.Data.SqlClient.SqlConnection(_config.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync(cancellationToken);
                try
                {
                    var affectedRows = await connection.ExecuteAsync(
                        "spu_DL_TourDuLich_LichTrinh_Delete",
                        new { LichTrinhID = request.ID },
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
