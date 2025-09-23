using Dapper;
using Domain.BaoChi;
using Domain.Core;
using Domain.VanHoa;
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

namespace Application.VanHoa.TrungTuDiTich
{
    public class ThemMoiChinhSua
    {
        public class Command : IRequest<Result<List<DiTich_TrungTu>>>
        {
            public List<DiTich_TrungTu> Data { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<List<DiTich_TrungTu>>>
        {
            private readonly IConfiguration _config;
            private readonly IMediator _mediator;
            private readonly IHttpContextAccessor _httpContextAccessor;


            public Handler(IConfiguration config, IMediator mediator, IHttpContextAccessor httpContextAccessor)
            {
                _config = config;
                _mediator = mediator;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<Result<List<DiTich_TrungTu>>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync(cancellationToken);
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var results = new List<DiTich_TrungTu>();

                            foreach (DiTich_TrungTu item in request.Data)
                            {
                                var parameters = new DynamicParameters();
                                parameters.Add("@DiTichID", item.DiTichID);
                                parameters.Add("@MaNgonNgu", item.MaNgonNgu);
                                parameters.Add("@ToChucThucHien", item.ToChucThucHien);
                                parameters.Add("@ThoiGian", item.ThoiGian);
                                parameters.Add("@CongViec", item.CongViec);
                                parameters.Add("@MucDich", item.MucDich);
                                parameters.Add("@LanTrungTu", item.LanTrungTu);

                                var result = await connection.QueryFirstOrDefaultAsync<DiTich_TrungTu>(
                                    "spu_VH_DiTich_TrungTu_AddEdit",
                                    parameters,
                                    commandType: CommandType.StoredProcedure,
                                    transaction: transaction
                                );

                                if (result != null)
                                    results.Add(result);
                            }

                            transaction.Commit();
                            return Result<List<DiTich_TrungTu>>.Success(results);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback(); // kó
                            return Result<List<DiTich_TrungTu>>.Failure(ex.Message);
                        }
                    }
                }
            }
        }
    }
}
