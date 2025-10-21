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
namespace Application.TourDuLich.TourDuLich
{
    public class ThemMoiChinhSua
    {
        public class Command : IRequest<Result<Domain.TourDuLich.TourDuLich>>
        {
            public Domain.TourDuLich.TourDuLich TourDuLich { get; set; }
            public List<Domain.TourDuLich.TourDuLich_NoiDung> NoiDung { get; set; }
        }
        public class Handler : IRequestHandler<Command, Result<Domain.TourDuLich.TourDuLich>>
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
            public async Task<Result<Domain.TourDuLich.TourDuLich>> Handle(Command request, CancellationToken cancellationToken)
            {
                
                    await using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                    await connection.OpenAsync(cancellationToken);
                    await using var transaction = connection.BeginTransaction();
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@TourID", request.TourDuLich.TourID);
                    parameters.Add("@ToChucID", request.TourDuLich.ToChucID);
                    parameters.Add("@MaDinhDanh", request.TourDuLich.MaDinhDanh);
                    parameters.Add("@TheLoaiID", request.TourDuLich.TheLoaiID);
                    parameters.Add("@LoaiHinhID", request.TourDuLich.LoaiHinhID);
                    parameters.Add("@PhanKhucID", request.TourDuLich.PhanKhucID);
                    parameters.Add("@DiemDiID", request.TourDuLich.DiemDiID);
                    parameters.Add("@DiemDenID", request.TourDuLich.DiemDenID);
                    parameters.Add("@NgayKhoiHanh", request.TourDuLich.NgayKhoiHanh);
                    parameters.Add("@NgayKetThuc", request.TourDuLich.NgayKetThuc);
                    parameters.Add("@SoKhachToiThieu", request.TourDuLich.SoKhachToiThieu);
                    parameters.Add("@SoKhachToiDa", request.TourDuLich.SoKhachToiDa);
                    parameters.Add("@GiaTour", request.TourDuLich.GiaTour);
                    parameters.Add("@GiaNguoi", request.TourDuLich.GiaNguoi);
                    parameters.Add("@MaNgonNgu", request.TourDuLich.MaNgonNgu);
                    parameters.Add("@PhuongTienID", request.TourDuLich.PhuongTienID);
                    parameters.Add("@SoLuotDanhGia", request.TourDuLich.SoLuotDanhGia);
                    parameters.Add("@DiemDanhGia", request.TourDuLich.DiemDanhGia);
                    parameters.Add("@TrangThai", request.TourDuLich.TrangThai);


                    var result = await connection.QueryFirstOrDefaultAsync<Domain.TourDuLich.TourDuLich>(
                        "spu_DL_TourDuLich_AddEdit",
                        parameters,
                        commandType: System.Data.CommandType.StoredProcedure,
                         transaction: transaction);
                    if (result != null)
                    {
                        foreach (var noiDung in request.NoiDung)
                        {
                            var paramNoiDung = new DynamicParameters();
                            paramNoiDung.Add("@TourID", result.TourID);
                            paramNoiDung.Add("@MaNgonNgu", noiDung.MaNgonNgu);
                            paramNoiDung.Add("@TenTour", noiDung.TenTour);
                            paramNoiDung.Add("@MoTa", noiDung.MoTa);
                            paramNoiDung.Add("@ThoiGian", noiDung.ThoiGian);
                            paramNoiDung.Add("@TraiNghiem", noiDung.TraiNghiem);
                            paramNoiDung.Add("@GoiY", noiDung.GoiY);
                            await connection.QueryFirstOrDefaultAsync(
                                "spu_DL_TourDuLich_NoiDung_AddEdit",
                                paramNoiDung,
                                commandType: System.Data.CommandType.StoredProcedure,
                                 transaction: transaction);
                        }
                    }
                    transaction.Commit();
                    return Result<Domain.TourDuLich.TourDuLich>.Success(result);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Result<Domain.TourDuLich.TourDuLich>.Failure(ex.Message);
                }
            }
        }
    }
}
