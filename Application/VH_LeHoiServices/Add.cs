using Dapper;
using Domain.Core;
using Domain.VH_LeHoiModel;
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

namespace Application.VH_LeHoiServices
{
    public class Add
    {
        public class Command : IRequest<Result<VH_LeHoi>>
        {
            public VH_LeHoi_RequestAdd RequestLeHoi;
            public List<VH_DiaDiemRequest> RequestDiaDiem;
        }

        public class Handler : IRequestHandler<Command, Result<VH_LeHoi>>
        {
            private readonly IConfiguration _configuration;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
            {
                _configuration = configuration;
                _httpContextAccessor = httpContextAccessor;
            }
            public async Task<Result<VH_LeHoi>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();
                    try
                    {
                        VH_LeHoi result = new VH_LeHoi();
                        string ListIDDiaDiem = request.RequestLeHoi.ListIDDiaDiem;
                        var ids = new List<string>();

                        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                        //Thêm mới lễ hội
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@TenLeHoi", request.RequestLeHoi.TenLeHoi);
                        parameters.Add("@MaDinhDanhLeHoi", request.RequestLeHoi.MaDinhDanhLeHoi);
                        parameters.Add("@TheLoaiID", request.RequestLeHoi.TheLoaiID);
                        parameters.Add("@NgayBatDau", request.RequestLeHoi.NgayBatDau);
                        parameters.Add("@NgayKetThuc", request.RequestLeHoi.NgayKetThuc);
                        parameters.Add("@TanSuatID", request.RequestLeHoi.TanSuatID);
                        parameters.Add("@CapDoID", request.RequestLeHoi.CapDoID);
                        parameters.Add("@CapQuanLyID", request.RequestLeHoi.CapQuanLyID);
                        parameters.Add("@SoLuongThamGia", request.RequestLeHoi.SoLuongThamGia);
                        parameters.Add("@SoNgayDienRa", request.RequestLeHoi.SoNgayDienRa);
                        parameters.Add("@DonViToChucID", request.RequestLeHoi.DonViToChucID);
                        parameters.Add("@NamVHPVTQG", request.RequestLeHoi.NamVHPHTQG);
                        parameters.Add("@DanTocID", request.RequestLeHoi.DanTocID);
                        parameters.Add("@LaTieuBieu", request.RequestLeHoi.LaTieuBieu);
                        parameters.Add("@LaCungDinh", request.RequestLeHoi.LaCungDinh);
                        parameters.Add("@TrangThaiToChuc", request.RequestLeHoi.TrangThaiToChuc);
                        parameters.Add("@TrangThai", request.RequestLeHoi.TrangThai);
                        parameters.Add("@NguoiCapNhat", userId);
                        parameters.Add("@ThoiGianDienRa", request.RequestLeHoi.ThoiGianDienRa);
                        result = await connection.QueryFirstOrDefaultAsync<VH_LeHoi>("spu_VH_LeHoi_Add", parameters, commandType: CommandType.StoredProcedure);

                        //Thêm mới nội dung lễ hội
                        if (result != null)
                        {
                            DynamicParameters parametersContent = new DynamicParameters();
                            parametersContent.Add("@LeHoiID", result.LeHoiID);
                            parametersContent.Add("@MaNgonNgu", request.RequestLeHoi.MaNgonNgu);
                            parametersContent.Add("@TenLeHoi", request.RequestLeHoi.TenLeHoiChild);
                            parametersContent.Add("@NhanVatPhungTho", request.RequestLeHoi.NhanVatPhungTho);
                            parametersContent.Add("@NoiDung", request.RequestLeHoi.NoiDung);
                            parametersContent.Add("@QuiDinh", request.RequestLeHoi.QuiDinh);
                            parametersContent.Add("@DanhGia", request.RequestLeHoi.DanhGia);
                            parametersContent.Add("@DonViPhoiHop", request.RequestLeHoi.DonViPhoiHop);
                            parametersContent.Add("@DiaDiemChiTiet", request.RequestLeHoi.DiaDiemChiTiet);
                            var resultContent = await connection.QueryFirstOrDefaultAsync<VH_LeHoi_NoiDung>("spu_VH_LeHoi_NoiDung_HandleInfo", parametersContent, commandType: CommandType.StoredProcedure);
                            if (resultContent == null)
                            {
                                return Result<VH_LeHoi>.Failure("Thêm mới nội dung lễ hội không thành công");
                            }
                        }

                        else
                        {
                            return Result<VH_LeHoi>.Failure("Thêm mới lễ hội không thành công");
                        }

                        if (result != null && ListIDDiaDiem != null && ListIDDiaDiem != "") {
                            try
                            {
                                //Thêm mới relation
                                DynamicParameters parametersLH_DiaDiem = new DynamicParameters();
                                parametersLH_DiaDiem.Add("@LeHoiID", result.LeHoiID);
                                parametersLH_DiaDiem.Add("@ListDiaDiem", ListIDDiaDiem);
                                var resultLH_DiaDiem = await connection.QueryFirstOrDefaultAsync<VH_LeHoi_DiaDiem>("spu_VH_LeHoi_DiaDiem_Add", parametersLH_DiaDiem, commandType: CommandType.StoredProcedure);
                                if (resultLH_DiaDiem == null)
                                {
                                    return Result<VH_LeHoi>.Failure("Thêm mới relation không thành công");
                                }
                            }
                            catch (Exception ex) {

                                return Result<VH_LeHoi>.Failure(ex.Message);
                            }
                           
                        }

                        return Result<VH_LeHoi>.Success(result);
                    }
                    catch (Exception ex)
                    {
                        return Result<VH_LeHoi>.Failure(ex.Message);
                    }
                    finally
                    {
                        await connection.CloseAsync();
                    }
                }
            }

        }
    }
}
