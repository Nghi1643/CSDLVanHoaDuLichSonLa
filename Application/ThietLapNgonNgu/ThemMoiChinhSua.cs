using Dapper;
using Domain;
using Domain.Core;
using Domain.Enums;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ThietLapNgonNgu
{
    public class ThemMoiChinhSua
    {
        public class Command : IRequest<Result<Domain.ThietLapNgonNgu>>
        {
            public ThietLapNgonNguRequest Data { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Domain.ThietLapNgonNgu>>
        {
            private readonly IConfiguration _config;
            private readonly IMediator _mediator;

            public Handler(IConfiguration config, IMediator mediator)
            {
                _config = config;
                _mediator = mediator;
            }

            public async Task<Result<Domain.ThietLapNgonNgu>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync(cancellationToken);
                    try
                    {
                        var oldData = new Result<Domain.ThietLapNgonNgu>();
                        if (request.Data.ID != null)
                        {
                            try
                            {
                                oldData = await _mediator.Send(new Application.ThietLapNgonNgu.ChiTiet.Query { ID = (int)request.Data.ID });
                            }
                            catch (Exception)
                            {
                                // Không trả lại lỗi
                            }
                        }
                        var parameters = new DynamicParameters();
                        parameters.Add("@ID", request.Data.ID);
                        parameters.Add("@Khoa", request.Data.Khoa);
                        parameters.Add("@GiaTriDich", request.Data.GiaTriDich);
                        parameters.Add("@MoTa", request.Data.MoTa);
                        parameters.Add("@MaNgonNgu", request.Data.MaNgonNgu);

                        var result = await connection.QueryFirstOrDefaultAsync<Domain.ThietLapNgonNgu>(
                            "spu_CSDL_ThietLapNgonNgu_AddEdit",
                            parameters,
                            commandType: CommandType.StoredProcedure
                        );

                        // Ghi log
                        if (result != null)
                        {
                            try
                            {
                                var log = await _mediator.Send(new Application.Audit.GhiLog.Command
                                {
                                    Data = new CSDL_Log()
                                    {
                                        ObjectID = request.Data.ID != null ? oldData.Value.ID.ToString() : result.ID.ToString(),
                                        TableName = "DM_ThietLapNgonNgu",
                                        Action = request.Data.ID != null ? (byte)EnumAction.Sua : (byte)EnumAction.Them,
                                        OldData = oldData.Value != null ? JsonConvert.SerializeObject(oldData.Value) : null,
                                        NewData = JsonConvert.SerializeObject(result)
                                    }
                                });
                            }
                            catch (Exception)
                            {
                                // Không trả lại lỗi
                            }
                        }
                        return Result<Domain.ThietLapNgonNgu>.Success(result);
                    }
                    catch (Exception ex)
                    {
                        return Result<Domain.ThietLapNgonNgu>.Failure(ex.Message);
                    }
                }
            }
        }
    }
}
