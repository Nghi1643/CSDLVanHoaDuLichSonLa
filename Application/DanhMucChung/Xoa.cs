using Dapper;
using Domain;
using Domain.Core;
using Domain.Enums;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DanhMucChung
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

            public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync(cancellationToken);
                    try
                    {
                        //var oldData = new Result<Domain.ThietLapNgonNgu>();
                        //try
                        //{
                        //    oldData = await _mediator.Send(new Application.DanhMucChung.ChiTiet.Query { ID = request.ID });
                        //}
                        //catch (Exception)
                        //{
                        //    // Không trả lại lỗi
                        //}

                        var parameters = new DynamicParameters();
                        parameters.Add("@DanhMucID", request.ID);

                        var affectedRows = await connection.ExecuteAsync(
                            "spu_DM_DanhMucChung_Delete",
                            parameters,
                            commandType: CommandType.StoredProcedure
                        );

                        // Ghi log
                        //if (affectedRows > 0)
                        //{
                        //    try
                        //    {
                        //        var log = await _mediator.Send(new Application.Audit.GhiLog.Command
                        //        {
                        //            Data = new CSDL_Log()
                        //            {
                        //                ObjectID = oldData.Value.ID.ToString(),
                        //                TableName = "DM_ThietLapNgonNgu",
                        //                Action = (byte)EnumAction.Xoa,
                        //                OldData = JsonConvert.SerializeObject(oldData.Value)
                        //            }
                        //        });
                        //    }
                        //    catch (Exception)
                        //    {
                        //        // Không trả lại lỗi
                        //    }
                        //}

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
