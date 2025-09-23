//using Dapper;
//using Domain.BaoChi;
//using Domain.Core;
//using Domain.DanhMuc;
//using Domain.VanHoa;
//using MediatR;
//using Microsoft.Data.SqlClient;
//using Microsoft.Extensions.Configuration;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Application.VanHoa.DiTich
//{
//    public class DanhSachBanDich
//    {
//        /// <summary>
//        /// Danh sách các bản dịch
//        /// </summary>
//        public class Query : IRequest<Result<List<DiTich_NoiDung>>>
//        {
//            public Guid? DiTichID { get; set; }
//            public string MaNgonNgu { get; set; }
//        }

//        public class Handler : IRequestHandler<Query, Result<List<DiTich_NoiDung>>>
//        {
//            private readonly IConfiguration _config;
//            public Handler(IConfiguration config)
//            {
//                _config = config;
//            }
//            public async Task<Result<List<DiTich_NoiDung>>> Handle(Query request, CancellationToken cancellationToken)
//            {
//                using (var connettion = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
//                {
//                    await connettion.OpenAsync();
//                    try
//                    {
//                        DynamicParameters parameters = new DynamicParameters();
//                        parameters.Add("@DiTichID", request.DiTichID);
//                        parameters.Add("@MaNgonNgu", request.MaNgonNgu);
//                        var queryResult = await connettion.QueryAsync<DiTich_NoiDung>("spu_VH_DiTich_NoiDung_GetFilter", parameters, commandType: System.Data.CommandType.StoredProcedure);

//                        return Result<List<DiTich_NoiDung>>.Success(queryResult?.ToList());
//                    }
//                    catch (Exception ex)
//                    {
//                        return Result<List<DiTich_NoiDung>>.Failure(ex.Message);
//                    }
//                }
//            }
//        }
//    }
//}
