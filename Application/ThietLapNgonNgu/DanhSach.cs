using Dapper;
using Domain;
using Domain.Core;
using Domain.ResponseDtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Persistence;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ThietLapNgonNgu
{
    public class DanhSach
    {
        public class Query : IRequest<Result<List<Domain.ThietLapNgonNgu>>>
        {
            public ThietLapNgonNguRequest Data{ get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<Domain.ThietLapNgonNgu>>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config)
            {
                _config = config;
            }
            public async Task<Result<List<Domain.ThietLapNgonNgu>>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var connettion = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connettion.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@ID", request.Data.ID);
                        parameters.Add("@MaNgonNgu", request.Data.MaNgonNgu);
                        parameters.Add("@TuKhoa", request.Data.TuKhoa);
                        var queryResult = await connettion.QueryAsync<Domain.ThietLapNgonNgu>("spu_CSDL_ThietLapNgonNgu_GetFilter", parameters, commandType: System.Data.CommandType.StoredProcedure);

                        
                        return Result<List<Domain.ThietLapNgonNgu>>.Success(queryResult?.ToList());
                    }
                    catch (Exception ex)
                    {
                        return Result<List<Domain.ThietLapNgonNgu>>.Failure(ex.Message);
                    }
                }
            }
        }
    }
}
