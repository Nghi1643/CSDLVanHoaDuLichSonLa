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

namespace Application.VanHoa.VHAmThuc
{
    public class DanhSach
    {
        public class Query : IRequest<Result<List<AmThucDTO>>>
        {
            public AmThucRequest Data { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<AmThucDTO>>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config)
            {
                _config = config;
            }
            public async Task<Result<List<AmThucDTO>>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var connettion = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connettion.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@TuKhoa", request.Data.TuKhoa);
                        parameters.Add("@KieuMonID", request.Data.KieuMonID);
                        parameters.Add("@CheDoAnID", request.Data.CheDoAnID);
                        parameters.Add("@DacSan", request.Data.DacSan);
                        parameters.Add("@MaNgonNgu", request.Data.MaNgonNgu);
                        parameters.Add("@SuDung", request.Data.SuDung);

                        var queryResult = await connettion.QueryAsync<AmThucDTO>("spu_VH_AmThuc_GetFilter", parameters, commandType: System.Data.CommandType.StoredProcedure);


                        return Result<List<AmThucDTO>>.Success(queryResult?.ToList());
                    }
                    catch (Exception ex)
                    {
                        return Result<List<AmThucDTO>>.Failure(ex.Message);
                    }
                }
            }
        }
    }

}
