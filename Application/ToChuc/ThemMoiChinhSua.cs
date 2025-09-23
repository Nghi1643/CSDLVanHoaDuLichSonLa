using Dapper;
using Domain.Core;
using Domain.Enums;
using Domain;
using MediatR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Domain.ToChuc;
using Domain.DanhMuc;

namespace Application.ToChuc
{
    public class ThemMoiChinhSua
    {
        public class Command : IRequest<Result<Domain.ToChuc.ToChuc>>
        {
            public ToChucAdd Data { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Domain.ToChuc.ToChuc>>
        {
            private readonly IConfiguration _config;
            private readonly IMediator _mediator;


            public Handler(IConfiguration config, IMediator mediator)
            {
                _config = config;
                _mediator = mediator;
            }

            public async Task<Result<Domain.ToChuc.ToChuc>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync(cancellationToken);
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("@ToChucID", request.Data.ToChucID);
                            parameters.Add("@LoaiToChucID", request.Data.LoaiToChucID);
                            parameters.Add("@LinhVucID", request.Data.LinhVucID);
                            parameters.Add("@MaDinhDanh", request.Data.MaDinhDanh);
                            parameters.Add("@TrangThaiID", request.Data.TrangThaiID);
                            parameters.Add("@HopThu", request.Data.HopThu);
                            parameters.Add("@DienThoai", request.Data.DienThoai);
                            parameters.Add("@Website", request.Data.Website);
                            parameters.Add("@NgayThanhLap", request.Data.NgayThanhLap);
                            parameters.Add("@SoGiayPhepHoatDong", request.Data.SoGiayPhepHoatDong);
                            parameters.Add("@CoQuanChuQuanID", request.Data.CoQuanChuQuanID);
                            parameters.Add("@PhamViHoatDongID", request.Data.PhamViHoatDongID);
                            parameters.Add("@ToaDoX", request.Data.ToaDoX);
                            parameters.Add("@ToaDoY", request.Data.ToaDoY);
                            parameters.Add("@LoaiHinhID", request.Data.LoaiHinhID);
                            parameters.Add("@TinhID", request.Data.TinhID);
                            parameters.Add("@XaID", request.Data.XaID);
                            parameters.Add("@QuyMo", request.Data.QuyMo);

                            var result = await connection.QueryFirstOrDefaultAsync<Domain.ToChuc.ToChuc>(
                                "spu_DM_ToChuc_AddEdit",
                                parameters,
                                commandType: CommandType.StoredProcedure,
                                transaction: transaction 
                            );

                            if (result != null)
                            {
                                foreach (var noiDung in request.Data.ToChuc_NoiDungs)
                                {
                                    var parametersNoiDung = new DynamicParameters();
                                    parametersNoiDung.Add("@ID", noiDung.ID);
                                    parametersNoiDung.Add("@ToChucID", result.ToChucID);
                                    parametersNoiDung.Add("@MaNgonNgu", noiDung.MaNgonNgu);
                                    parametersNoiDung.Add("@TenToChuc", noiDung.TenToChuc);
                                    parametersNoiDung.Add("@DiaChi", noiDung.DiaChi);
                                    parametersNoiDung.Add("@GioiThieu", noiDung.GioiThieu);
                                    parametersNoiDung.Add("@GhiChu", noiDung.GhiChu);

                                    var resultNoiDung = await connection.QueryFirstOrDefaultAsync<ToChuc_NoiDung>(
                                        "spu_DM_ToChuc_NoiDung_AddEdit",
                                        parametersNoiDung,
                                        commandType: CommandType.StoredProcedure,
                                        transaction: transaction
                                    );
                                }
                            }

                            transaction.Commit(); // ok
                            return Result<Domain.ToChuc.ToChuc>.Success(result);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback(); // kó
                            return Result<Domain.ToChuc.ToChuc>.Failure(ex.Message);
                        }
                    }
                }
            }
        }
    }
}
