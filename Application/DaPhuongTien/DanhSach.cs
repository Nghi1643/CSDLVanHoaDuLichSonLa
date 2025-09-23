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

namespace Application.DaPhuongTien
{
    public class DanhSach
    {
        public class Query : IRequest<Result<List<Domain.DanhMuc.DaPhuongTienDTO>>>
        {
            public Domain.DanhMuc.DaPhuongTien Data { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<Domain.DanhMuc.DaPhuongTienDTO>>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config)
            {
                _config = config;
            }
            public async Task<Result<List<Domain.DanhMuc.DaPhuongTienDTO>>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var connettion = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connettion.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@DoiTuongSoHuuID", request.Data.DoiTuongSoHuuID);
                        parameters.Add("@TheLoaiID", request.Data.TheLoaiID);
                        parameters.Add("@LoaiMedia", request.Data.LoaiMedia);

                        var queryResult = await connettion.QueryAsync<Domain.DanhMuc.DaPhuongTienDTO>("spu_DM_DaPhuongTien_GetFilter", parameters, commandType: System.Data.CommandType.StoredProcedure);


                        return Result<List<Domain.DanhMuc.DaPhuongTienDTO>>.Success(queryResult?.ToList());
                    }
                    catch (Exception ex)
                    {
                        return Result<List<Domain.DanhMuc.DaPhuongTienDTO>>.Failure(ex.Message);
                    }
                }
            }
        }
    }
    
}
