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

namespace Application.VanHoa.HienVat
{
    public class DanhSach
    {
        public class Query : IRequest<Result<List<HienVatDTO>>>
        {
            public HienVatRequest Data { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<HienVatDTO>>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config)
            {
                _config = config;
            }
            public async Task<Result<List<HienVatDTO>>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var connettion = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connettion.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@TuKhoa", request.Data.TuKhoa);
                        parameters.Add("@PhuongThucTangMuaID", request.Data.PhuongThucTangMuaID);
                        parameters.Add("@LoaiHienVatID", request.Data.LoaiHienVatID);
                        parameters.Add("@BaoTangID", request.Data.BaoTangID);
                        parameters.Add("@TrangThaiID", request.Data.TrangThaiID);
                        parameters.Add("@ChatLieuID", request.Data.ChatLieuID);
                        parameters.Add("@MaNgonNgu", request.Data.MaNgonNgu);
                        parameters.Add("@SuDung", request.Data.SuDung);

                        var queryResult = await connettion.QueryAsync<HienVatDTO>("spu_VH_HienVat_GetFilter", parameters, commandType: System.Data.CommandType.StoredProcedure);


                        return Result<List<HienVatDTO>>.Success(queryResult?.ToList());
                    }
                    catch (Exception ex)
                    {
                        return Result<List<HienVatDTO>>.Failure(ex.Message);
                    }
                }
            }
        }
    }
    
}
