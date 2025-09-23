using Dapper;
using Domain.BaoChi;
using Domain.Core;
using Domain.VanHoa;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.VanHoa.HienVat
{
    public class ThemMoiChinhSua
    {
        public class Command : IRequest<Result<Domain.VanHoa.HienVat>>
        {
            public HienVatAdd Data { get; set; }
            public List<HienVat_NoiDungAdd> NoiDungBanDich{ get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Domain.VanHoa.HienVat>>
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

            public async Task<Result<Domain.VanHoa.HienVat>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync(cancellationToken);
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // lấy userId
                            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                            var parameters = new DynamicParameters();
                            parameters.Add("@HienVatID", request.Data.HienVatID);
                            parameters.Add("@LoaiHienVatID", request.Data.LoaiHienVatID);
                            parameters.Add("@TenHienVat", request.Data.TenHienVat);
                            parameters.Add("@MaHienVat", request.Data.MaHienVat);
                            parameters.Add("@ChatLieuID", request.Data.ChatLieuID);
                            parameters.Add("@PhuongThucTangMuaID", request.Data.PhuongThucTangMuaID);
                            parameters.Add("@BaoTangID", request.Data.BaoTangID);
                            parameters.Add("@TrangThaiID", request.Data.TrangThaiID);
                            parameters.Add("@NguoiCapNhat", userId);
                            parameters.Add("@NguoiHieuChinh", userId);
                            parameters.Add("@SuDung", request.Data.SuDung);
                            parameters.Add("@AnhDaiDien", request.Data.AnhDaiDien);
                            parameters.Add("@ThuTu", request.Data.ThuTu);

                            var result = await connection.QueryFirstOrDefaultAsync<Domain.VanHoa.HienVat>(
                                "spu_VH_HienVat_AddEdit",
                                parameters,
                                commandType: CommandType.StoredProcedure,
                                transaction: transaction
                            );

                            if (result != null)
                            {
                                foreach (var noiDung in request.NoiDungBanDich)
                                {
                                    var parametersNoiDung = new DynamicParameters();
                                    parametersNoiDung.Add("@HienVatID", result.HienVatID);
                                    parametersNoiDung.Add("@MaNgonNgu", noiDung.MaNgonNgu);
                                    parametersNoiDung.Add("@TenHienVat", noiDung.TenHienVat);
                                    parametersNoiDung.Add("@TenGoiKhac", noiDung.TenGoiKhac);
                                    parametersNoiDung.Add("@SoLuong", noiDung.SoLuong);
                                    parametersNoiDung.Add("@MauSac", noiDung.MauSac);
                                    parametersNoiDung.Add("@KichThuoc", noiDung.KichThuoc);
                                    parametersNoiDung.Add("@NienDai", noiDung.NienDai);
                                    parametersNoiDung.Add("@NguonGoc", noiDung.NguonGoc);
                                    parametersNoiDung.Add("@KhaoTa", noiDung.KhaoTa);
                                    parametersNoiDung.Add("@TinhTrangSuuTam", noiDung.TinhTrangSuuTam);
                                    parametersNoiDung.Add("@HoanCanhSuuTam", noiDung.HoanCanhSuuTam);
                                    parametersNoiDung.Add("@ChuHienVat", noiDung.ChuHienVat);
                                    parametersNoiDung.Add("@NguoiSuuTam", noiDung.NguoiSuuTam);

                                    var resultNoiDung = await connection.QueryFirstOrDefaultAsync<HienVat_NoiDung>(
                                        "spu_VH_HienVat_NoiDung_AddEdit",
                                        parametersNoiDung,
                                        commandType: CommandType.StoredProcedure,
                                        transaction: transaction
                                    );
                                }
                            }
                            else
                            {
                                transaction.Rollback();
                                return Result<Domain.VanHoa.HienVat>.Failure("Mã định danh đã tồn tại");
                            }

                                transaction.Commit(); // ok
                            return Result<Domain.VanHoa.HienVat>.Success(result);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback(); // err
                            return Result<Domain.VanHoa.HienVat>.Failure(ex.Message);
                        }
                    }
                }
            }
        }
    }
}
