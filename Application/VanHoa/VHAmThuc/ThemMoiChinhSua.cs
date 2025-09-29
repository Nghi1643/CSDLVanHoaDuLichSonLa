using Application.Audit;
using Dapper;
using Domain;
using Domain.BaoChi;
using Domain.Core;
using Domain.Enums;
using Domain.VanHoa;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Application.VanHoa.VHAmThuc
{
    public class ThemMoiChinhSua
    {
        public class Command : IRequest<Result<AmThuc>>
        {
            public AmThucAdd Data { get; set; }
            public List<AmThuc_NoiDungAdd> NoiDungBanDich { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<AmThuc>>
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

            public async Task<Result<AmThuc>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync(cancellationToken);
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            //Lấy dữ liệu cũ nếu là chỉnh sửa
                            var oldData = new Result<AmThucDTO>();
                            if (request.Data.MonAnUongID != null && request.Data.MonAnUongID != Guid.Empty)
                            {
                                try
                                {
                                    oldData = await _mediator.Send(new ChiTiet.Query { MonAnUongID = (Guid)request.Data.MonAnUongID });
                                }
                                catch (Exception)
                                {
                                    // Không trả lại lỗi
                                }
                            }

                            // lấy userId
                            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                            var parameters = new DynamicParameters();
                            parameters.Add("@MonAnUongID", request.Data.MonAnUongID);
                            parameters.Add("@KieuMonID", request.Data.KieuMonID);
                            parameters.Add("@CheDoAnID", request.Data.CheDoAnID);
                            parameters.Add("@DacSan", request.Data.DacSan);
                            parameters.Add("@AnhDaiDien", request.Data.AnhDaiDien);
                            parameters.Add("@ThuTu", request.Data.ThuTu);
                            parameters.Add("@NguoiCapNhat", userId);
                            parameters.Add("@NguoiHieuChinh", userId);
                            parameters.Add("@SuDung", request.Data.SuDung);

                            var result = await connection.QueryFirstOrDefaultAsync<AmThuc>(
                                "spu_VH_AmThuc_AddEdit",
                                parameters,
                                commandType: CommandType.StoredProcedure,
                                transaction: transaction
                            );

                            if (result != null)
                            {
                                foreach (var noiDung in request.NoiDungBanDich)
                                {
                                    var parametersNoiDung = new DynamicParameters();
                                    parametersNoiDung.Add("@MonAnUongID", result.MonAnUongID);
                                    parametersNoiDung.Add("@MaNgonNgu", noiDung.MaNgonNgu);
                                    parametersNoiDung.Add("@NguyenLieu", noiDung.NguyenLieu);
                                    parametersNoiDung.Add("@MoTa", noiDung.MoTa);
                                    parametersNoiDung.Add("@TraiNghiem", noiDung.TraiNghiem);
                                    parametersNoiDung.Add("@TenMon", noiDung.TenMon);

                                    var resultNoiDung = await connection.QueryAsync<AmThuc_NoiDung>(
                                        "spu_VH_AmThuc_NoiDung_AddEdit",
                                        parametersNoiDung,
                                        commandType: CommandType.StoredProcedure,
                                        transaction: transaction
                                    );
                                }
                            }
                            else
                            {
                                transaction.Rollback();
                                return Result<AmThuc>.Failure("Mã định danh đã tồn tại");
                            }

                            // Ghi log
                            try
                            {
                                var log = await _mediator.Send(new GhiLog.Command
                                {
                                    Data = new CSDL_Log()
                                    {
                                        ObjectID = request.Data.MonAnUongID != null ? oldData.Value.MonAnUongID.ToString() : result.MonAnUongID.ToString(),
                                        TableName = "DM_AmThuc",
                                        Action = request.Data.MonAnUongID != null ? (byte)EnumAction.Sua : (byte)EnumAction.Them,
                                        OldData = oldData.Value != null ? JsonConvert.SerializeObject(oldData.Value) : null,
                                        NewData = JsonConvert.SerializeObject(result)
                                    }
                                });
                            }
                            catch (Exception)
                            {
                                // Không trả lại lỗi
                            }

                            transaction.Commit(); // ok
                            return Result<AmThuc>.Success(result);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback(); // err
                            return Result<AmThuc>.Failure(ex.Message);
                        }
                    }
                }
            }
        }
    }
}
