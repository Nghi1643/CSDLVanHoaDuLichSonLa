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
using Domain.CaNhan;

namespace Application.CaNhan.CaNhan
{
    public class ThemMoiChinhSua
    {
        public class Command : IRequest<Result<Domain.CaNhan.VanBan>>
        {
            public Domain.CaNhan.VanBan cn { get; set; }
            public List<CaNhan_NoiDung> NoiDungBanDich { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Domain.CaNhan.VanBan>>
        {
            private readonly IConfiguration _config;
            private readonly IMediator _mediator;


            public Handler(IConfiguration config, IMediator mediator)
            {
                _config = config;
                _mediator = mediator;
            }

            public async Task<Result<Domain.CaNhan.VanBan>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync(cancellationToken);
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("@CaNhanID", request.cn.CaNhanID);
                            parameters.Add("@MaDinhDanh", request.cn.MaDinhDanh);
                            parameters.Add("@NgaySinh", request.cn.NgaySinh);
                            parameters.Add("@GioiTinhID", request.cn.GioiTinhID);
                            parameters.Add("@DanTocID", request.cn.DanTocID);
                            parameters.Add("@TinhID", request.cn.TinhID);
                            parameters.Add("@XaID", request.cn.XaID);
                            parameters.Add("@DienThoai", request.cn.DienThoai);
                            parameters.Add("@HopThu", request.cn.HopThu);
                            parameters.Add("@AnhChanDung", request.cn.AnhChanDung);
                            parameters.Add("@TrangThaiID", request.cn.TrangThaiID);
                            parameters.Add("@IsDeleted", request.cn.IsDeleted);

                            var result = await connection.QueryFirstOrDefaultAsync<Domain.CaNhan.VanBan>(
                                "spu_DM_CaNhan_AddEdit",
                                parameters,
                                commandType: CommandType.StoredProcedure,
                                transaction: transaction
                            );
                            if (result != null)
                            {
                                foreach (var noiDung in request.NoiDungBanDich)
                                {
                                    var parametersNoiDung = new DynamicParameters();
                                    parametersNoiDung.Add("@CaNhanID", result.CaNhanID);
                                    parametersNoiDung.Add("@MaNgonNgu", noiDung.MaNgonNgu);
                                    parametersNoiDung.Add("@HoTen", noiDung.HoTen);
                                    parametersNoiDung.Add("@DiaChi", noiDung.DiaChi);
                                    parametersNoiDung.Add("@NoiLamViec", noiDung.NoiLamViec);
                                    parametersNoiDung.Add("@MoTa", noiDung.MoTa);
                                    parametersNoiDung.Add("@TrangThai", noiDung.TrangThai);
                                    parametersNoiDung.Add("@NgayCapNhat", noiDung.NgayCapNhat);
                                    parametersNoiDung.Add("@NgayHieuChinh", noiDung.NgayHieuChinh);
                                    parametersNoiDung.Add("@NguoiCapNhat", noiDung.NguoiCapNhat);
                                    parametersNoiDung.Add("@NguoiHieuChinh", noiDung.NguoiHieuChinh);
                                    parametersNoiDung.Add("@IsDeleted", noiDung.IsDeleted);

                                    var resultNoiDung = await connection.QueryFirstOrDefaultAsync<Domain.CaNhan.VanBan>(
                                        "spu_DM_CaNhan_NoiDung_AddEdit",
                                        parametersNoiDung,
                                        commandType: CommandType.StoredProcedure,
                                        transaction: transaction
                                    );
                                }
                            }
                            transaction.Commit(); // ok
                            return Result<Domain.CaNhan.VanBan>.Success(result);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback(); // kó
                            return Result<Domain.CaNhan.VanBan>.Failure(ex.Message);
                        }
                    }
                }
            }
        }
    }
}
