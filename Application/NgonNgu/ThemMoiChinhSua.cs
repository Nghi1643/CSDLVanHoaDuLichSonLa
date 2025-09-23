using Dapper;
using Domain;
using Domain.Core;
using Domain.DanhMuc;
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

namespace Application.NgonNgu
{
    public class ThemMoiChinhSua
    {
        public class Command : IRequest<Result<Domain.DanhMuc.NgonNgu>>
        {
            public Domain.DanhMuc.NgonNguRequest Data { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Domain.DanhMuc.NgonNgu>>
        {
            private readonly IConfiguration _config;
            private readonly IMediator _mediator;


            public Handler(IConfiguration config, IMediator mediator)
            {
                _config = config;
                _mediator = mediator;
            }

            public async Task<Result<Domain.DanhMuc.NgonNgu>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync(cancellationToken);
                    try
                    {
                        var oldData = new Result<Domain.DanhMuc.NgonNgu>();
                        try
                        {
                            if (request.Data.NgonNguID != null)
                            {
                                oldData = await _mediator.Send(new Application.NgonNgu.ChiTiet.Query { NgonNguID = (Guid)request.Data.NgonNguID });
                            }
                        }
                        catch (Exception)
                        {
                            // Không trả lại lỗi
                        }

                        var parameters = new DynamicParameters();
                        parameters.Add("@NgonNguID", request.Data.NgonNguID);
                        parameters.Add("@MaNgonNgu", request.Data.MaNgonNgu);
                        parameters.Add("@TenNgonNgu", request.Data.TenNgonNgu);
                        parameters.Add("@ThuTu", request.Data.ThuTu);
                        parameters.Add("@TrangThai", request.Data.TrangThai);

                        var result = await connection.QueryFirstOrDefaultAsync<Domain.DanhMuc.NgonNgu>(
                            "spu_DM_NgonNgu_AddEdit",
                            parameters,
                            commandType: CommandType.StoredProcedure
                        );

                        // Ghi log
                        if(result != null)
                        {
                            try
                            {
                                var log = await _mediator.Send(new Application.Audit.GhiLog.Command
                                {
                                    Data = new CSDL_Log()
                                    {
                                        ObjectID = request.Data.NgonNguID != null ? oldData.Value.NgonNguID.ToString() : result.NgonNguID.ToString(),
                                        TableName = "DM_NgonNgu",
                                        Action = request.Data.NgonNguID != null ? (byte)EnumAction.Sua : (byte)EnumAction.Them,
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

                        return Result<Domain.DanhMuc.NgonNgu>.Success(result);
                    }
                    catch (Exception ex)
                    {
                        return Result<Domain.DanhMuc.NgonNgu>.Failure(ex.Message);
                    }
                }
            }
        }
    }
}
