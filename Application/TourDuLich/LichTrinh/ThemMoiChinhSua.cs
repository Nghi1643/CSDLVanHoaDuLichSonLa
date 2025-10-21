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

namespace Application.TourDuLich.LichTrinh
{
    public class ThemMoiChinhSua
    {
        public class LichTrinhItem
        {
            public Domain.TourDuLich.TourDuLich_LichTrinh LichTrinh { get; set; }
            public List<Domain.TourDuLich.TourDuLich_LichTrinh_NoiDung> NoiDungs { get; set; }
        }

        public class Command : IRequest<Result<List<Domain.TourDuLich.TourDuLich_LichTrinh>>>
        {
            public List<LichTrinhItem> DanhSachLichTrinh { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<List<Domain.TourDuLich.TourDuLich_LichTrinh>>>
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

            public async Task<Result<List<Domain.TourDuLich.TourDuLich_LichTrinh>>> Handle(Command request, CancellationToken cancellationToken)
            {
                await using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync(cancellationToken);
                await using var transaction = connection.BeginTransaction();

                try
                {
                    var results = new List<Domain.TourDuLich.TourDuLich_LichTrinh>();

                    foreach (var lichTrinhItem in request.DanhSachLichTrinh)
                    {
                        // Thêm lộ trình chính
                        var parameters = new DynamicParameters();
                        parameters.Add("@LichTrinhID", lichTrinhItem.LichTrinh.LichTrinhID);
                        parameters.Add("@TourID", lichTrinhItem.LichTrinh.TourID);
                        parameters.Add("@DiaDiemID", lichTrinhItem.LichTrinh.DiaDiemID);
                        parameters.Add("@ThoiDiem", lichTrinhItem.LichTrinh.ThoiDiem);
                        parameters.Add("@ThuTu", lichTrinhItem.LichTrinh.ThuTu);

                        var result = await connection.QueryFirstOrDefaultAsync<Domain.TourDuLich.TourDuLich_LichTrinh>(
                            "spu_DL_TourDuLich_LichTrinh_AddEdit",
                            parameters,
                            commandType: System.Data.CommandType.StoredProcedure,
                            transaction: transaction);

                        if (result != null)
                        {
                            results.Add(result);

                            if (lichTrinhItem.NoiDungs != null && lichTrinhItem.NoiDungs.Any())
                            {
                                foreach (var noiDung in lichTrinhItem.NoiDungs)
                                {
                                    //if (string.IsNullOrWhiteSpace(noiDung.HoatDong))
                                    //    continue;

                                    var paramNoiDung = new DynamicParameters();
                                    paramNoiDung.Add("@LichTrinhID", result.LichTrinhID);
                                    paramNoiDung.Add("@HoatDong", noiDung.HoatDong?.Trim());
                                    paramNoiDung.Add("@SoGio", noiDung.SoGio?.Trim());

                                    await connection.QueryFirstOrDefaultAsync(
                                        "spu_DL_TourDuLich_LichTrinh_NoiDung_AddEdit",
                                        paramNoiDung,
                                        commandType: System.Data.CommandType.StoredProcedure,
                                        transaction: transaction);
                                }
                            }
                        }
                    }

                    transaction.Commit();
                    return Result<List<Domain.TourDuLich.TourDuLich_LichTrinh>>.Success(results);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Result<List<Domain.TourDuLich.TourDuLich_LichTrinh>>.Failure(ex.Message);
                }
            }
        }
    }
}