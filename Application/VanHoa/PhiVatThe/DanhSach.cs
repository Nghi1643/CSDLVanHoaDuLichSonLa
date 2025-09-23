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

namespace Application.VanHoa.PhiVatThe
{
    public class DanhSach
    {
        public class Query : IRequest<Result<List<DiSanPhiVatTheDTO>>>
        {
            public DiSanPhiVatTheRequest Data { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<DiSanPhiVatTheDTO>>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config)
            {
                _config = config;
            }
            public async Task<Result<List<DiSanPhiVatTheDTO>>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var connettion = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connettion.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@TuKhoa", request.Data.TuKhoa);
                        parameters.Add("@LoaiHinhID", request.Data.LoaiHinhID);
                        parameters.Add("@TinhTrangID", request.Data.TinhTrangID);
                        parameters.Add("@SuDung", request.Data.SuDung);
                        parameters.Add("@MaNgonNgu", request.Data.MaNgonNgu);

                        var queryResult = await connettion.QueryAsync<DiSanPhiVatTheDTO>("spu_VH_DiSanPhiVatThe_GetFilter", parameters, commandType: System.Data.CommandType.StoredProcedure);


                        return Result<List<DiSanPhiVatTheDTO>>.Success(queryResult?.ToList());
                    }
                    catch (Exception ex)
                    {
                        return Result<List<DiSanPhiVatTheDTO>>.Failure(ex.Message);
                    }
                }
            }
        }
    }
    
}
