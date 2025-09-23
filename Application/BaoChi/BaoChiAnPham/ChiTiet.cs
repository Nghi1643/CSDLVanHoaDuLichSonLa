//using Dapper;
//using Domain.BaoChi;
//using Domain.Core;
//using Domain.DanhMuc;
//using MediatR;
//using Microsoft.Data.SqlClient;
//using Microsoft.Extensions.Configuration;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Application.BaoChiAnPham
//{
//    public class ChiTiet
//    {
//        public class Query : IRequest<Result<BaoChiAnPhamDTO>>
//        {
//            public Guid BaoChiAnPhamID { get; set; }
//        }

//        public class Handler : IRequestHandler<Query, Result<BaoChiAnPhamDTO>>
//        {
//            private readonly IConfiguration _config;
//            public Handler(IConfiguration config)
//            {
//                _config = config;
//            }
//            public async Task<Result<BaoChiAnPhamDTO>> Handle(Query request, CancellationToken cancellationToken)
//            {
//                using (var connettion = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
//                {
//                    await connettion.OpenAsync();
//                    try
//                    {
//                        DynamicParameters parameters = new DynamicParameters();
//                        parameters.Add("@BaoChiAnPhamID", request.BaoChiAnPhamID);
//                        var queryResult = await connettion.QueryFirstOrDefaultAsync<BaoChiAnPhamDTO>("spu_BC_BaoChiAnPham_Get", parameters, commandType: System.Data.CommandType.StoredProcedure);


//                        return Result<BaoChiAnPhamDTO>.Success(queryResult);
//                    }
//                    catch (Exception ex)
//                    {
//                        return Result<BaoChiAnPhamDTO>.Failure(ex.Message);
//                    }
//                }
//            }
//        }
//    }
//}
