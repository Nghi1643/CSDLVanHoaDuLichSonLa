using Dapper;
using Domain;
using Domain.Core;
using Domain.DanhMuc;
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

namespace Application.NgonNgu
{
    public class DanhSach
    {
        public class Query : IRequest<Result<List<Domain.DanhMuc.NgonNgu>>>
        {
            public NgonNguRequest Data{ get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<Domain.DanhMuc.NgonNgu>>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config)
            {
                _config = config;
            }
            public async Task<Result<List<Domain.DanhMuc.NgonNgu>>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var connettion = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connettion.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@TrangThai", request.Data.TrangThai);
                        parameters.Add("@TuKhoa", request.Data.TuKhoa);
                        var queryResult = await connettion.QueryAsync<Domain.DanhMuc.NgonNgu>("spu_DM_NgonNgu_GetFilter", parameters, commandType: System.Data.CommandType.StoredProcedure);

                        
                        return Result<List<Domain.DanhMuc.NgonNgu>>.Success(queryResult?.ToList());
                    }
                    catch (Exception ex)
                    {
                        return Result<List<Domain.DanhMuc.NgonNgu>>.Failure(ex.Message);
                    }
                }
            }
        }
    }
}
