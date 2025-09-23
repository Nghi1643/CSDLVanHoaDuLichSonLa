using Dapper;
using Domain.Core;
using Domain.Enums;
using Domain;
using MediatR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Domain.DanhMuc;

namespace Application.DanhMucChung
{
    public class ThemMoiChinhSua
    {
        public class Command : IRequest<Result<Domain.DanhMuc.DanhMucChung>>
        {
            public Domain.DanhMuc.DanhMucChungAdd Data { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Domain.DanhMuc.DanhMucChung>>
        {
            private readonly IConfiguration _config;
            private readonly IMediator _mediator;


            public Handler(IConfiguration config, IMediator mediator)
            {
                _config = config;
                _mediator = mediator;
            }

            public async Task<Result<Domain.DanhMuc.DanhMucChung>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync(cancellationToken);
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("@DanhMucID", request.Data.DanhMucID);
                            parameters.Add("@LoaiDanhMucID", request.Data.LoaiDanhMucID);
                            parameters.Add("@MaSo", request.Data.MaSo);
                            parameters.Add("@ThuTu", request.Data.ThuTu);
                            parameters.Add("@TrangThai", request.Data.TrangThai);
                            parameters.Add("@Ten", request.Data.Ten);

                            var result = await connection.QueryFirstOrDefaultAsync<Domain.DanhMuc.DanhMucChung>(
                                "spu_DM_DanhMucChung_AddEdit",
                                parameters,
                                commandType: CommandType.StoredProcedure,
                                transaction: transaction 
                            );

                            if (result != null)
                            {
                                foreach (var noiDung in request.Data.DanhMucChung_NoiDungs)
                                {
                                    var parametersNoiDung = new DynamicParameters();
                                    parametersNoiDung.Add("@DanhMucID", result.DanhMucID);
                                    parametersNoiDung.Add("@MaNgonNgu", noiDung.MaNgonNgu);
                                    parametersNoiDung.Add("@Ten", noiDung.Ten);
                                    parametersNoiDung.Add("@MoTa", noiDung.MoTa);

                                    var resultNoiDung = await connection.QueryFirstOrDefaultAsync<Domain.DanhMuc.DanhMucChung_NoiDung>(
                                        "spu_DM_DanhMucChung_NoiDung_AddEdit",
                                        parametersNoiDung,
                                        commandType: CommandType.StoredProcedure,
                                        transaction: transaction
                                    );
                                }
                            }

                            transaction.Commit(); // ok
                            return Result<Domain.DanhMuc.DanhMucChung>.Success(result);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback(); // kó
                            return Result<Domain.DanhMuc.DanhMucChung>.Failure(ex.Message);
                        }
                    }
                }
            }
        }
    }
}
