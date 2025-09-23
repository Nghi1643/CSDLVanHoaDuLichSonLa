using Dapper;
using Domain.BaoChi;
using Domain.Core;
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

namespace Application.BaoChi.XuatBanAnPham
{
    public class ThemMoiChinhSua
    {
        public class Command : IRequest<Result<Domain.BaoChi.XuatBanAnPham>>
        {
            public Domain.BaoChi.XuatBanAnPham XBAnPham{ get; set; }
            public List<XuatBanAnPham_NoiDung> NoiDungBanDich{ get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Domain.BaoChi.XuatBanAnPham>>
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

            public async Task<Result<Domain.BaoChi.XuatBanAnPham>> Handle(Command request, CancellationToken cancellationToken)
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
                            parameters.Add("@XuatBanAnPhamID", request.XBAnPham.XuatBanAnPhamID);
                            parameters.Add("@MaDinhDanhXBAP", request.XBAnPham.MaDinhDanhXBAP);
                            parameters.Add("@TenAnPham", request.XBAnPham.TenAnPham);
                            parameters.Add("@MaQuocTe", request.XBAnPham.MaQuocTe);
                            parameters.Add("@NhaXuatBanID", request.XBAnPham.NhaXuatBanID);
                            parameters.Add("@NamXuatBan", request.XBAnPham.NamXuatBan);
                            parameters.Add("@TheLoaiID", request.XBAnPham.TheLoaiID);
                            parameters.Add("@SoLanTaiBan", request.XBAnPham.SoLanTaiBan);
                            parameters.Add("@SoLuongIn", request.XBAnPham.SoLuongIn);
                            parameters.Add("@GiaBia", request.XBAnPham.GiaBia);
                            parameters.Add("@DuongDanHinhAnhBia", request.XBAnPham.DuongDanHinhAnhBia);
                            parameters.Add("@NguoiCapNhat", userId);
                            parameters.Add("@NguoiHieuChinh", userId);
                            parameters.Add("@TrangThai", request.XBAnPham.TrangThai);

                            var result = await connection.QueryFirstOrDefaultAsync<Domain.BaoChi.XuatBanAnPham>(
                                "spu_BC_XuatBanAnPham_AddEdit",
                                parameters,
                                commandType: CommandType.StoredProcedure,
                                transaction: transaction
                            );

                            if (result != null)
                            {
                                foreach (var noiDung in request.NoiDungBanDich)
                                {
                                    var parametersNoiDung = new DynamicParameters();
                                    parametersNoiDung.Add("@XuatBanAnPhamID", result.XuatBanAnPhamID);
                                    parametersNoiDung.Add("@MaNgonNgu", noiDung.MaNgonNgu);
                                    parametersNoiDung.Add("@TenAnPham", noiDung.TenAnPham);
                                    parametersNoiDung.Add("@TacGia", noiDung.TacGia);
                                    parametersNoiDung.Add("@MoTa", noiDung.MoTa);

                                    var resultNoiDung = await connection.QueryFirstOrDefaultAsync<Domain.DanhMuc.DanhMucChung_NoiDung>(
                                        "spu_BC_XuatBanAnPham_NoiDung_AddEdit",
                                        parametersNoiDung,
                                        commandType: CommandType.StoredProcedure,
                                        transaction: transaction
                                    );
                                }
                            }

                            transaction.Commit(); // ok
                            return Result<Domain.BaoChi.XuatBanAnPham>.Success(result);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback(); // kó
                            return Result<Domain.BaoChi.XuatBanAnPham>.Failure(ex.Message);
                        }
                    }
                }
            }
        }
    }
}
