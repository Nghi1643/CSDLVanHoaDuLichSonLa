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
using Domain.DanhMuc;

namespace Application.TacPham
{
    public class DanhSach
    {
        public class Query : IRequest<Result<List<TacPhamDTO>>>
        {
            public TacPhamRequest Data { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<TacPhamDTO>>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config)
            {
                _config = config;
            }
            public async Task<Result<List<TacPhamDTO>>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var connettion = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connettion.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@TuKhoa", request.Data.TuKhoa);
                        parameters.Add("@TheLoaiID", request.Data.TheLoaiID);
                        parameters.Add("@LinhVucID", request.Data.LinhVucID);
                        parameters.Add("@MaNgonNgu", request.Data.MaNgonNgu);
                        parameters.Add("@SuDung", request.Data.SuDung);

                        var queryResult = await connettion.QueryAsync<TacPhamDTO>("spu_DM_TacPham_GetFilter", parameters, commandType: System.Data.CommandType.StoredProcedure);


                        return Result<List<TacPhamDTO>>.Success(queryResult?.ToList());
                    }
                    catch (Exception ex)
                    {
                        return Result<List<TacPhamDTO>>.Failure(ex.Message);
                    }
                }
            }
        }
    }
    
}
