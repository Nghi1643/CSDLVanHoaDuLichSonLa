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
using Domain.SuKienHoatDong;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Domain.BaoChi;

namespace Application.DiaDiem
{
    public class ThemMoiChinhSua
    {
        public class Command : IRequest<Result<Domain.DiaDiem.DiaDiem>>
        {
            public Domain.DiaDiem.DiaDiem DiaDiem { get; set; }
            public List<DiaDiem_NoiDung> NoiDungBanDich { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Domain.DiaDiem.DiaDiem>>
        {
            private readonly IConfiguration _config;
            private readonly IMediator _mediator;


            public Handler(IConfiguration config, IMediator mediator)
            {
                _config = config;
                _mediator = mediator;
            }

            public async Task<Result<Domain.DiaDiem.DiaDiem>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync(cancellationToken);
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("@DiaDiemID", request.DiaDiem.DiaDiemID);
                            parameters.Add("@DiaDiemCapChaID", request.DiaDiem.DiaDiemCapChaID);
                            parameters.Add("@LinhVucID", request.DiaDiem.LinhVucID);
                            parameters.Add("@XaID", request.DiaDiem.XaID);
                            parameters.Add("@TinhID", request.DiaDiem.TinhID);
                            parameters.Add("@KinhDo", request.DiaDiem.KinhDo);
                            parameters.Add("@ViDo", request.DiaDiem.ViDo);
                            parameters.Add("@CaoDo", request.DiaDiem.CaoDo);
                            parameters.Add("@BanDo", request.DiaDiem.BanDo);
                            parameters.Add("@HinhAnh", request.DiaDiem.HinhAnh);
                            parameters.Add("@NguoiKhuyetTat", request.DiaDiem.NguoiKhuyetTat);
                            parameters.Add("@NhaVeSinh", request.DiaDiem.NhaVeSinh);
                            parameters.Add("@BaiDoXe", request.DiaDiem.BaiDoXe);

                            var result = await connection.QueryFirstOrDefaultAsync<Domain.DiaDiem.DiaDiem>(
                                "spu_DM_DiaDiem_AddEdit",
                                parameters,
                                commandType: CommandType.StoredProcedure,
                                transaction: transaction 
                            );
                            if (result != null)
                            {
                                foreach (var noiDung in request.NoiDungBanDich)
                                {
                                    var parametersNoiDung = new DynamicParameters();
                                    parametersNoiDung.Add("@DiaDiemID", result.DiaDiemID);
                                    parametersNoiDung.Add("@MaNgonNgu", noiDung.MaNgonNgu);
                                    parametersNoiDung.Add("@TenDiaDiem", noiDung.TenDiaDiem);
                                    parametersNoiDung.Add("@MoTa", noiDung.MoTa);
                                    parametersNoiDung.Add("@DiaChi", noiDung.DiaChi);
                                    parametersNoiDung.Add("@HeToaDo", noiDung.HeToaDo);
                                    parametersNoiDung.Add("@ThoiGianHoatDong", noiDung.ThoiGianHoatDong);
                                    parametersNoiDung.Add("@SucChua", noiDung.SucChua);

                                    var resultNoiDung = await connection.QueryFirstOrDefaultAsync<Domain.DiaDiem.DiaDiem>(
                                        "spu_DM_DiaDiem_NoiDung_AddEdit",
                                        parametersNoiDung,
                                        commandType: CommandType.StoredProcedure,
                                        transaction: transaction
                                    );
                                }
                            }
                            transaction.Commit(); // ok
                            return Result<Domain.DiaDiem.DiaDiem>.Success(result);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback(); // kó
                            return Result<Domain.DiaDiem.DiaDiem>.Failure(ex.Message);
                        }
                    }
                }
            }
        }
    }
}
