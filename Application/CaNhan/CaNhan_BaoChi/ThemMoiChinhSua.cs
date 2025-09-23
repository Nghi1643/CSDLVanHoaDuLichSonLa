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
using Domain.ToChuc;
using Domain.SuKienHoatDong;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Domain.BaoChi;
using Domain.CaNhan;

namespace Application.CaNhan.CaNhan_BaoChi
{
    public class ThemMoiChinhSua
    {
        public class Command : IRequest<Result<Domain.CaNhan.CaNhan_BaoChi>>
        {
            public Domain.CaNhan.CaNhan_BaoChi cnbc { get; set; }
            public List<CaNhan_BaoChi_NoiDung> NoiDungBanDich { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Domain.CaNhan.CaNhan_BaoChi>>
        {
            private readonly IConfiguration _config;
            private readonly IMediator _mediator;


            public Handler(IConfiguration config, IMediator mediator)
            {
                _config = config;
                _mediator = mediator;
            }

            public async Task<Result<Domain.CaNhan.CaNhan_BaoChi>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync(cancellationToken);
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("@CaNhanID", request.cnbc.CaNhanID);
                            parameters.Add("@VaiTroID", request.cnbc.VaiTroID);
                            parameters.Add("@SoThe", request.cnbc.SoThe);
                            parameters.Add("@ChucDanhID", request.cnbc.ChucDanhID);

                            var result = await connection.QueryFirstOrDefaultAsync<Domain.CaNhan.CaNhan_BaoChi>(
                                "spu_DM_CaNhan_BaoChi_AddEdit",
                                parameters,
                                commandType: CommandType.StoredProcedure,
                                transaction: transaction
                            );
                            if (result != null)
                            {
                                foreach (var noiDung in request.NoiDungBanDich)
                                {
                                    var parametersNoiDung = new DynamicParameters();
                                    parametersNoiDung.Add("@CaNhanID", result.CaNhanID);
                                    parametersNoiDung.Add("@VaiTroID", noiDung.VaiTroID);
                                    parametersNoiDung.Add("@MaNgonNgu", noiDung.MaNgonNgu);
                                    parametersNoiDung.Add("@NoiCap", noiDung.NoiCap);
                                    parametersNoiDung.Add("@TacPhamNoiBat", noiDung.TacPhamNoiBat);

                                    var resultNoiDung = await connection.QueryFirstOrDefaultAsync<Domain.CaNhan.CaNhan_BaoChi>(
                                        "spu_DM_CaNhan_BaoChi_NoiDung_AddEdit",
                                        parametersNoiDung,
                                        commandType: CommandType.StoredProcedure,
                                        transaction: transaction
                                    );
                                }
                            }
                            transaction.Commit(); // ok
                            return Result<Domain.CaNhan.CaNhan_BaoChi>.Success(result);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback(); // kó
                            return Result<Domain.CaNhan.CaNhan_BaoChi>.Failure(ex.Message);
                        }
                    }
                }
            }
        }
    }
}
