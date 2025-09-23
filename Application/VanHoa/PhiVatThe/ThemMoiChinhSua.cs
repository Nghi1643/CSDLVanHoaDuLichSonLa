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

namespace Application.VanHoa.PhiVatThe
{
    public class ThemMoiChinhSua
    {
        public class Command : IRequest<Result<DiSanPhiVatThe>>
        {
            public DiSanPhiVatTheAdd Data { get; set; }
            public List<DiSanPhiVatThe_NoiDungAdd> NoiDungBanDich{ get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<DiSanPhiVatThe>>
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

            public async Task<Result<DiSanPhiVatThe>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync(cancellationToken);
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            //Lấy dữ liệu cũ nếu là chỉnh sửa
                            var oldData = new Result<DiSanPhiVatTheDTO>();
                            if (request.Data.DiSanID != null && request.Data.DiSanID != Guid.Empty)
                            {
                                try
                                {
                                    oldData = await _mediator.Send(new ChiTiet.Query { DiSanID = (Guid)request.Data.DiSanID });
                                }
                                catch (Exception)
                                {
                                    // Không trả lại lỗi
                                }
                            }

                            // lấy userId
                            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                            var parameters = new DynamicParameters();
                            parameters.Add("@DiSanID", request.Data.DiSanID);
                            parameters.Add("@LoaiHinhID", request.Data.LoaiHinhID);
                            parameters.Add("@TinhTrangID", request.Data.TinhTrangID);
                            parameters.Add("@MaDinhDanh", request.Data.MaDinhDanh);
                            parameters.Add("@AnhDaiDien", request.Data.AnhDaiDien);
                            parameters.Add("@ThuTu", request.Data.ThuTu);
                            parameters.Add("@NguoiCapNhat", userId);
                            parameters.Add("@NguoiHieuChinh", userId);
                            parameters.Add("@SuDung", request.Data.SuDung);

                            var result = await connection.QueryFirstOrDefaultAsync<DiSanPhiVatThe>(
                                "spu_VH_DiSanPhiVatThe_AddEdit",
                                parameters,
                                commandType: CommandType.StoredProcedure,
                                transaction: transaction
                            );

                            if (result != null)
                            {
                                foreach (var noiDung in request.NoiDungBanDich)
                                {
                                    var parametersNoiDung = new DynamicParameters();
                                    parametersNoiDung.Add("@DiSanID", result.DiSanID);
                                    parametersNoiDung.Add("@MaNgonNgu", noiDung.MaNgonNgu);
                                    parametersNoiDung.Add("@CongDong", noiDung.CongDong);
                                    parametersNoiDung.Add("@MoTa", noiDung.MoTa);
                                    parametersNoiDung.Add("@TinhTrang", noiDung.TinhTrang);
                                    parametersNoiDung.Add("@TenDiSan", noiDung.TenDiSan);

                                    var resultNoiDung = await connection.QueryAsync<DiSanPhiVatThe_NoiDung>(
                                        "spu_VH_DiSanPhiVatThe_NoiDung_AddEdit",
                                        parametersNoiDung,
                                        commandType: CommandType.StoredProcedure,
                                        transaction: transaction
                                    );
                                }
                            }
                            else
                            {
                                transaction.Rollback();
                                return Result<DiSanPhiVatThe>.Failure("Mã định danh đã tồn tại");
                            }

                            // Ghi log
                            try
                            {
                                var log = await _mediator.Send(new GhiLog.Command
                                {
                                    Data = new CSDL_Log()
                                    {
                                        ObjectID = request.Data.DiSanID != null ? oldData.Value.DiSanID.ToString() : result.DiSanID.ToString(),
                                        TableName = "DM_DiSanPhiVatThe",
                                        Action = request.Data.DiSanID != null ? (byte)EnumAction.Sua : (byte)EnumAction.Them,
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
                            return Result<DiSanPhiVatThe>.Success(result);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback(); // err
                            return Result<DiSanPhiVatThe>.Failure(ex.Message);
                        }
                    }
                }
            }
        }
    }
}
