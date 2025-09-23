using Dapper;
using Domain.Core;
using Domain;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Domain.BaoChi;
using Domain.VanHoa;

namespace Application.VanHoa.TrungTuDiTich
{
    public class DanhSach
    {
        public class Query : IRequest<Result<List<DiTich_TrungTu>>>
        {
            public DiTich_TrungTu Data { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<DiTich_TrungTu>>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config)
            {
                _config = config;
            }
            public async Task<Result<List<DiTich_TrungTu>>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var connettion = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connettion.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@DiTichID", request.Data.DiTichID);
                        parameters.Add("@MaNgonNgu", request.Data.MaNgonNgu);

                        var queryResult = await connettion.QueryAsync<DiTich_TrungTu>("spu_VH_DiTich_TrungTu_GetFilter", parameters, commandType: System.Data.CommandType.StoredProcedure);


                        return Result<List<DiTich_TrungTu>>.Success(queryResult?.ToList());
                    }
                    catch (Exception ex)
                    {
                        return Result<List<DiTich_TrungTu>>.Failure(ex.Message);
                    }
                }
            }
        }
    }
    
}
