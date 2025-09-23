using Dapper;
using Domain.Core;
using Domain.VH_LeHoiModel;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.VH_LeHoiServices
{
    public class Update
    {
        public class Command : IRequest<Result<VH_LeHoi>>
        {
            public VH_LeHoi_RequestUpdate Entity;
        }

        public class Handler : IRequestHandler<Command, Result<VH_LeHoi>>
        {
            private readonly IConfiguration _configuration;

            public Handler(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public async Task<Result<VH_LeHoi>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@LeHoiID", request.Entity.LeHoiID);
                        parameters.Add("@TenLeHoi", request.Entity.TenLeHoi);
                        parameters.Add("@MaDinhDanhLeHoi", request.Entity.MaDinhDanhLeHoi);
                        parameters.Add("@TheLoaiID", request.Entity.TheLoaiID);
                        parameters.Add("@NgayBatDau", request.Entity.NgayBatDau);
                        parameters.Add("@NgayKetThuc", request.Entity.NgayKetThuc);
                        parameters.Add("@TanSuatID", request.Entity.TanSuatID);
                        parameters.Add("@CapDoID", request.Entity.CapDoID);
                        parameters.Add("@CapQuanLyID", request.Entity.CapQuanLyID);
                        parameters.Add("@SoLuongThamGia", request.Entity.SoLuongThamGia);
                        parameters.Add("@SoNgayDienRa", request.Entity.SoNgayDienRa);
                        parameters.Add("@DonViToChucID", request.Entity.DonViToChucID);
                        parameters.Add("@NamVHPVTQG", request.Entity.NamVHPHTQG);
                        parameters.Add("@DanTocID", request.Entity.DanTocID);
                        parameters.Add("@LaTieuBieu", request.Entity.LaTieuBieu);
                        parameters.Add("@LaCungDinh", request.Entity.LaCungDinh);
                        parameters.Add("@TrangThaiToChuc", request.Entity.TrangThaiToChuc);
                        parameters.Add("@TrangThai", request.Entity.TrangThai);
                        parameters.Add("@NguoiHieuChinh", request.Entity.NguoiHieuChinh);
                        parameters.Add("@ThoiGianDienRa", request.Entity.ThoiGianDienRa);
                        var result = await connection.QueryFirstOrDefaultAsync<VH_LeHoi>("spu_VH_LeHoi_Update", parameters, commandType: CommandType.StoredProcedure);
                        if (result != null)
                        {
                            DynamicParameters parametersContent = new DynamicParameters();
                            parametersContent.Add("@LeHoiID", request.Entity.LeHoiID);
                            parametersContent.Add("@MaNgonNgu", request.Entity.MaNgonNgu);
                            parametersContent.Add("@TenLeHoi", request.Entity.TenLeHoiChild);
                            parametersContent.Add("@NhanVatPhungTho", request.Entity.NhanVatPhungTho);
                            parametersContent.Add("@NoiDung", request.Entity.NoiDung);
                            parametersContent.Add("@QuiDinh", request.Entity.QuiDinh);
                            parametersContent.Add("@DanhGia", request.Entity.DanhGia);
                            parametersContent.Add("@DonViPhoiHop", request.Entity.DonViPhoiHop);
                            parametersContent.Add("@DiaDiemChiTiet", request.Entity.DiaDiemChiTiet);
                            var resultContent = await connection.QueryFirstOrDefaultAsync<VH_LeHoi_NoiDung>("spu_VH_LeHoi_NoiDung_HandleInfo", parametersContent, commandType: CommandType.StoredProcedure);
                            if (resultContent == null)
                            {
                                return Result<VH_LeHoi>.Failure("Cập nhật nội dung lễ hội không thành công");
                            }
                        }
                        else
                        {
                            return Result<VH_LeHoi>.Failure("Cập nhật lễ hội không thành công");
                        }

                        if (result != null && request.Entity.ListIDDiaDiem != null && request.Entity.ListIDDiaDiem != "") {
                            try
                            {
                                //Thêm mới relation
                                DynamicParameters parametersLH_DiaDiem = new DynamicParameters();
                                parametersLH_DiaDiem.Add("@LeHoiID", result.LeHoiID);
                                parametersLH_DiaDiem.Add("@ListDiaDiem", request.Entity.ListIDDiaDiem);
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
