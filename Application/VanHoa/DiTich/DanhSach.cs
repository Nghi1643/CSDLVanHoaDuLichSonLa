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

namespace Application.VanHoa.DiTich
{
    /// <summary>
    /// Danh sách di tích văn hoá
    /// </summary>
    public class DanhSach
    {
        public class Query : IRequest<Result<List<DiTichDTO>>>
        {
            public DiTichRequest Data { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<DiTichDTO>>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config)
            {
                _config = config;
            }
            public async Task<Result<List<DiTichDTO>>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var connettion = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connettion.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@TuKhoa", request.Data.TuKhoa);
                        parameters.Add("@CapXepHangID", request.Data.CapXepHangID);
                        parameters.Add("@LoaiHinhID", request.Data.LoaiHinhID);
                        parameters.Add("@TinhTrangID", request.Data.TrangThaiID);
                        parameters.Add("@DonViQuanLyID", request.Data.ToChucQuanLyID);
                        parameters.Add("@MaNgonNgu", request.Data.MaNgonNgu);
                        parameters.Add("@TrangThai", request.Data.SuDung);

                        var queryResult = await connettion.QueryAsync<DiTichDTO>("spu_VH_DiTich_GetFilter", parameters, commandType: System.Data.CommandType.StoredProcedure);


                        return Result<List<DiTichDTO>>.Success(queryResult?.ToList());
                    }
                    catch (Exception ex)
                    {
                        return Result<List<DiTichDTO>>.Failure(ex.Message);
                    }
                }
            }
        }
    }
    
}
