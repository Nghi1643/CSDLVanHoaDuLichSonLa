using Dapper;
using Domain.Core;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Application.TourDuLich.TheLoai
{
    public class ThemMoiChinhSua
    {
        public class Command : IRequest<Result<Domain.TourDuLich.TourDuLich_TheLoai>>
        {
            public Domain.TourDuLich.TourDuLich_TheLoai TourDuLich_TheLoai { get; set; }
            public List<Domain.TourDuLich.TourDuLich_TheLoai_NoiDung> NoiDung { get; set; }
            //public Domain.TourDuLich.TourDuLich_TheLoai_NoiDung NoiDung { get; set; }
            //public List<short> DsLinhVucID { get; set; }
            public string ListLinhVucID { get; set; }
        }
        public class Handler : IRequestHandler<Command, Result<Domain.TourDuLich.TourDuLich_TheLoai>>
        {
            private readonly IConfiguration _config;
            private readonly IMediator _mediator;
            private readonly IHttpContextAccessor _httpContextAccessor;
            public Handler(IConfiguration config, IMediator mediator, IHttpContextAccessor httpContextAccessor)
            {
                _config = config;
                _mediator = mediator;
                _httpContextAccessor = httpContextAccessor;
            }
            public async Task<Result<Domain.TourDuLich.TourDuLich_TheLoai>> Handle(Command request, CancellationToken cancellationToken)
            {
                
                    await using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                    await connection.OpenAsync(cancellationToken);
                    await using var transaction = connection.BeginTransaction();
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@TheLoaiID", request.TourDuLich_TheLoai.TheLoaiID);
                    parameters.Add("@TrangThai", request.TourDuLich_TheLoai.TrangThai);

                    var result = await connection.QueryFirstOrDefaultAsync<Domain.TourDuLich.TourDuLich_TheLoai>(
                        "spu_DL_TourDuLich_TheLoai_AddEdit",
                        parameters,
                        commandType: System.Data.CommandType.StoredProcedure,
                         transaction: transaction);
                    if (result != null)
                    {
                        foreach (var noiDung in request.NoiDung)
                        {
                            var paramNoiDung = new DynamicParameters();
                            paramNoiDung.Add("@TheLoaiID", result.TheLoaiID);
                            paramNoiDung.Add("@TenTheLoai", noiDung.TenTheLoai);
                            paramNoiDung.Add("@MoTa", noiDung.MoTa);
                            await connection.QueryFirstOrDefaultAsync(
                                "spu_DL_TourDuLich_TheLoai_NoiDung_AddEdit",
                                paramNoiDung,
                                commandType: System.Data.CommandType.StoredProcedure,
                                 transaction: transaction);
                        }
                        // thêm quan hệ giữa thể loại và lĩnh vực
                        if (!string.IsNullOrEmpty(request.ListLinhVucID))
                        {
                            var paramLinhVuc = new DynamicParameters();
                            paramLinhVuc.Add("@TheLoaiID", result.TheLoaiID);
                            paramLinhVuc.Add("@ListLinhVucID", request.ListLinhVucID); 
                            await connection.QueryFirstOrDefaultAsync(
                                "spu_DL_TourDuLich_TheLoai_LinhVuc_Add",
                                paramLinhVuc,
                                commandType: System.Data.CommandType.StoredProcedure,
                                transaction: transaction);
                        }
                    }
                    transaction.Commit();
                    return Result<Domain.TourDuLich.TourDuLich_TheLoai>.Success(result);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Result<Domain.TourDuLich.TourDuLich_TheLoai>.Failure(ex.Message);
                }
            }
        }
    }
}
