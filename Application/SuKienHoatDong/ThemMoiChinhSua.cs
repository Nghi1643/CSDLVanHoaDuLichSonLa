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

namespace Application.SuKienHoatDong
{
    public class ThemMoiChinhSua
    {
        public class Command : IRequest<Result<Domain.SuKienHoatDong.DiaDiem>>
        {
            public Domain.SuKienHoatDong.DiaDiem SuKien { get; set; }
            public List<DiaDiem_noiDung> NoiDungBanDich { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Domain.SuKienHoatDong.DiaDiem>>
        {
            private readonly IConfiguration _config;
            private readonly IMediator _mediator;


            public Handler(IConfiguration config, IMediator mediator)
            {
                _config = config;
                _mediator = mediator;
            }

            public async Task<Result<Domain.SuKienHoatDong.DiaDiem>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync(cancellationToken);
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var parameters = new DynamicParameters();
                           parameters.Add("@MaSuKien", request.SuKien.MaSuKien);
                           parameters.Add("@TenSuKien", request.SuKien.TenSuKien  );
                           parameters.Add("@LinhVucID", request.SuKien.LinhVucID  );
                           parameters.Add("@LoaiDoiTuong", request.SuKien.LoaiDoiTuong  );
                           parameters.Add("@NgayBatDau", request.SuKien.NgayBatDau  );
                           parameters.Add("@NgayKetThuc", request.SuKien.NgayKetThuc  );
                           parameters.Add("@AnhDaiDien", request.SuKien.AnhDaiDien  );
                           parameters.Add("@DonViToChucID", request.SuKien.DonViToChucID  );
                           parameters.Add("@SoKhachThamGia", request.SuKien.SoKhachThamGia  );
                           parameters.Add("@NgayCapNhat", request.SuKien.NgayCapNhat);
                           parameters.Add("@NgayHieuChinh", request.SuKien.NgayHieuChinh  );
                           parameters.Add("@NguoiCapNhat", request.SuKien.NguoiCapNhat  );
                           parameters.Add("@NguoiHieuChinh", request.SuKien.NguoiHieuChinh );
                           parameters.Add("@TrangThai", request.SuKien.TrangThai );

                            var result = await connection.QueryFirstOrDefaultAsync<Domain.SuKienHoatDong.DiaDiem>(
                                "spu_DM_SuKienHoatDong_AddEdit",
                                parameters,
                                commandType: CommandType.StoredProcedure,
                                transaction: transaction 
                            );
                            if (result != null)
                            {
                                foreach (var noiDung in request.NoiDungBanDich)
                                {
                                    var parametersNoiDung = new DynamicParameters();
                                    parametersNoiDung.Add("@SuKienID", result.SuKienID);
                                    parametersNoiDung.Add("@MaNgonNgu", noiDung.MaNgonNgu );
                                    parametersNoiDung.Add("@TenSuKien", noiDung.TenSuKien );
                                    parametersNoiDung.Add("@MoTa", noiDung.MoTa );
                                    parametersNoiDung.Add("@NoiDung", noiDung.NoiDung );

                                    var resultNoiDung = await connection.QueryFirstOrDefaultAsync<Domain.SuKienHoatDong.DiaDiem>(
                                        "spu_DM_SuKienHoatDong_NoiDung_AddEdit",
                                        parametersNoiDung,
                                        commandType: CommandType.StoredProcedure,
                                        transaction: transaction
                                    );
                                }
                            }
                            transaction.Commit(); // ok
                            return Result<Domain.SuKienHoatDong.DiaDiem>.Success(result);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback(); // kó
                            return Result<Domain.SuKienHoatDong.DiaDiem>.Failure(ex.Message);
                        }
                    }
                }
            }
        }
    }
}
