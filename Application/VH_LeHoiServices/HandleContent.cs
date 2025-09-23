using Dapper;
using Domain.Core;
using Domain.VH_LeHoiModel;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.VH_LeHoiServices
{
    public class HandleContent
    {
        public class Command : IRequest<Result<VH_LeHoi_NoiDung>>
        {
            public VH_LeHoi_NoiDung_Request Entity;
        }

        public class Handler : IRequestHandler<Command, Result<VH_LeHoi_NoiDung>>
        {
            private readonly IConfiguration _configuration;

            public Handler(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public async Task<Result<VH_LeHoi_NoiDung>> Handle(Command request, CancellationToken cancellationToken)
            {
                using(SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();
                    try
                    {
                        DynamicParameters parametersContent = new DynamicParameters();
                        parametersContent.Add("@LeHoiID", request.Entity.LeHoiID);
                        parametersContent.Add("@MaNgonNgu", request.Entity.MaNgonNgu);
                        parametersContent.Add("@TenLeHoi", request.Entity.TenLeHoiChild);
                        parametersContent.Add("@NhanVatPhungTho", request.Entity.NhanVatPhungTho);
                        parametersContent.Add("@NoiDung", request.Entity.NoiDung);
                        parametersContent.Add("@QuiDinh", request.Entity.QuiDinh);
                        parametersContent.Add("@DanhGia", request.Entity.DanhGia);
                        parametersContent.Add("@DonViPhoiHop", request.Entity.DonViPhoiHop);
                        parametersContent.Add("@DiaDiemChiTiet", request.Entity.DiaDiemChiTiet);
                        var resultContent = await connection.QueryFirstOrDefaultAsync<VH_LeHoi_NoiDung>("spu_VH_LeHoi_NoiDung_HandleInfo", parametersContent, commandType: CommandType.StoredProcedure);
                        return Result<VH_LeHoi_NoiDung>.Success(resultContent);
                    }
                    catch (Exception ex) {
                        return Result<VH_LeHoi_NoiDung>.Failure(ex.Message);
                    }
                    finally
                    {
                        await connection.CloseAsync();
                    }

                     
                }
            }
        }
    }
}
