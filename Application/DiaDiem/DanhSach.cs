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
using Domain.ToChuc;
using Domain.SuKienHoatDong;
using Domain.DiaDiem;

namespace Application.DiaDiem
{
    public class DanhSach
    {
        /// <summary>
        /// Danh sách danh mục chung(list địa điểm, list chức danh,...) theo loại danh mục (địa điểm, chức danh,...)
        /// </summary>
        public class Query : IRequest<Result<List<DiaDiemDTO>>>
        {
            public DiaDiemRequest Data { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<DiaDiemDTO>>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config)
            {
                _config = config;
            }
            public async Task<Result<List<DiaDiemDTO>>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var connettion = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connettion.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@DiaDiemID", request.Data.DiaDiemID);
                        parameters.Add("@MaNgonNgu", request.Data.MaNgonNgu);
                        parameters.Add("@TrangThai", request.Data.TrangThai);
                        parameters.Add("@TuKhoa", request.Data.TuKhoa);
                        parameters.Add("@DiaDiemCapChaID", request.Data.DiaDiemCapChaID);
                        parameters.Add("@LinhVucID", request.Data.LinhVucID);
                        parameters.Add("@XaID", request.Data.XaID);
                        parameters.Add("@TinhID", request.Data.TinhID);

                        var queryResult = await connettion.QueryAsync<DiaDiemDTO>("spu_DM_DiaDiem_GetFilter", parameters, commandType: System.Data.CommandType.StoredProcedure);

                        return Result<List<DiaDiemDTO>>.Success(queryResult?.ToList());
                    }
                    catch (Exception ex)
                    {
                        return Result<List<DiaDiemDTO>>.Failure(ex.Message);
                    }
                }
            }
        }
    }
}
