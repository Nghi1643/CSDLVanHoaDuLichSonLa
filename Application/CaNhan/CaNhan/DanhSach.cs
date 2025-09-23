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
using Domain.CaNhan;

namespace Application.CaNhan.CaNhan
{
    public class DanhSach
    {
        /// <summary>
        /// Danh sách danh mục chung(list địa điểm, list chức danh,...) theo loại danh mục (địa điểm, chức danh,...)
        /// </summary>
        public class Query : IRequest<Result<List<CaNhanDTO>>>
        {
            public CaNhanRequest Data { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<CaNhanDTO>>>
        {
            private readonly IConfiguration _config;
            public Handler(IConfiguration config)
            {
                _config = config;
            }
            public async Task<Result<List<CaNhanDTO>>> Handle(Query request, CancellationToken cancellationToken)
            {
                using (var connettion = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connettion.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@CaNhanID", request.Data.CaNhanID);
                        parameters.Add("@VaiTroID", request.Data.VaiTroID);
                        parameters.Add("@TuKhoa", request.Data.TuKhoa);
                        parameters.Add("@IsDeleted", request.Data.IsDeleted);
                        parameters.Add("@TrangThaiID", request.Data.TrangThaiID);
                        parameters.Add("@MaNgonNgu", request.Data.MaNgonNgu);

                        var queryResult = await connettion.QueryAsync<CaNhanDTO>("spu_DM_CaNhan_VaiTro_GetFilter", parameters, commandType: System.Data.CommandType.StoredProcedure);

                        return Result<List<CaNhanDTO>>.Success(queryResult?.ToList());
                    }
                    catch (Exception ex)
                    {
                        return Result<List<CaNhanDTO>>.Failure(ex.Message);
                    }
                }
            }
        }
    }
}
