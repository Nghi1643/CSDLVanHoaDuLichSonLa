using Dapper;
using Domain.DanhMuc;
using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using MediatR;
using Domain.Core;
using Domain.Enums;
using Domain;
using Azure.Core;

namespace CSDLVanHoaDuLichSonLa.Services
{
    /// <summary>
    /// Interface quản lý Cấp Tỉnh
    /// </summary>
    public interface ICapTinhService
    {
        Task<IEnumerable<CapTinhTrinhDien>> Gets();
        Task<CapTinhTrinhDien> GetById(Guid? id);
        Task<CapTinh> Create(CapTinh request);
        Task<CapTinh> Update(CapTinh request);
        Task<int> Delete(Guid id);
    }
    public class CapTinhService : ConnectDatabase, ICapTinhService
    {
        private readonly IMediator _mediator;
        private readonly INoiDungDaNguServices _noidung;

        public CapTinhService(IConfiguration configuration, IMediator mediator, INoiDungDaNguServices noidung) : base(configuration) {
            _mediator= mediator;
            _noidung = noidung;
        }

        public async Task<IEnumerable<CapTinhTrinhDien>> Gets()
        {
            using (SqlConnection conn = IConnectData())
            {
                try
                {
                    await conn.OpenAsync();
                    IEnumerable<CapTinh> data = await conn.QueryAsync<CapTinh>(
                        "SP_CapTinh_GetAll",
                        commandType: CommandType.StoredProcedure
                    );
                    List<CapTinhTrinhDien> dsCapTinh =new List<CapTinhTrinhDien>();
                    foreach(CapTinh item in data)
                    {
                        CapTinhTrinhDien iCapTinh = new CapTinhTrinhDien();
                        iCapTinh.MaTinh = item.MaTinh;
                        iCapTinh.TinhID = item.TinhID;
                        iCapTinh.TrangThai = item.TrangThai;
                        iCapTinh.SapXep = item.SapXep;
                        iCapTinh.NoiDung = item.NoiDung;
                        iCapTinh.DaNgu =await _noidung.Gets(item.TinhID, item.LoaiDM);
                        dsCapTinh.Add(iCapTinh);
                    }
                    return dsCapTinh;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi lấy danh sách cấp tỉnh: {ex.Message}");
                }
                finally
                {
                    conn?.Close();
                }
            }
        }

        public async Task<CapTinh> GetByIdLog(Guid? id)
        {
            using (SqlConnection conn = IConnectData())
            {
                try
                {
                    await conn.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@TinhID", id);

                    CapTinh data =  await conn.QueryFirstOrDefaultAsync<CapTinh>(
                        "SP_CapTinh_GetById",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );
                    return data;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi lấy thông tin cấp tỉnh: {ex.Message}");
                }
                finally
                {
                    conn?.Close();
                }
            }
        }
        public async Task<CapTinhTrinhDien> GetById(Guid? id)
        {
            using (SqlConnection conn = IConnectData())
            {
                try
                {
                    await conn.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@TinhID", id);

                    CapTinh data = await conn.QueryFirstOrDefaultAsync<CapTinh>(
                        "SP_CapTinh_GetById",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );
                    CapTinhTrinhDien iCapTinh = new CapTinhTrinhDien();
                    iCapTinh.MaTinh = data.MaTinh;
                    iCapTinh.TinhID = data.TinhID;
                    iCapTinh.TrangThai = data.TrangThai;
                    iCapTinh.SapXep = data.SapXep;
                    iCapTinh.NoiDung = data.NoiDung;
                    iCapTinh.DaNgu = await _noidung.Gets(data.TinhID, data.LoaiDM);
                    return iCapTinh;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi lấy thông tin cấp tỉnh: {ex.Message}");
                }
                finally
                {
                    conn?.Close();
                }
            }
        }
        public async Task<CapTinh> Create(CapTinh request)
        {
            using (SqlConnection conn = IConnectData())
            {
                try
                {
                    await conn.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@NoiDungID", Guid.NewGuid());
                    parameters.Add("@LoaiDM", request.LoaiDM);
                    parameters.Add("@NoiDung", request.NoiDung);
                    parameters.Add("@NgonNguID", request.NgonNguID);
                    parameters.Add("@SapXep", request.SapXep);
                    parameters.Add("@TrangThai", request.TrangThai);
                    parameters.Add("@MaTinh", request.MaTinh);

                    CapTinh data =  await conn.QueryFirstOrDefaultAsync<CapTinh>(
                        "SP_CapTinh_Insert",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );
                    //Ghi Log
                    if (data.TinhID != null)
                    {
                        
                        string json = JsonConvert.SerializeObject(data);
                        CapTinhBase baseModel = JsonConvert.DeserializeObject<CapTinhBase>(json);
                        NoiDungDaNgu noidung = JsonConvert.DeserializeObject<NoiDungDaNgu>(json);
                        await _mediator.Send(new Application.Audit.GhiLog.Command
                        {
                            Data = new CSDL_Log()
                            {
                                ObjectID = data.TinhID.ToString(),
                                TableName = "DM_DiaPhuongCapTinh",
                                Action = (byte)EnumAction.Them,
                                NewData = JsonConvert.SerializeObject(baseModel)
                            }
                        });
                        await _mediator.Send(new Application.Audit.GhiLog.Command
                        {
                            Data = new CSDL_Log()
                            {
                                ObjectID = data.TinhID.ToString(),
                                TableName = "DM_NoiDungDaNgu",
                                Action = (byte)EnumAction.Them,
                                NewData = JsonConvert.SerializeObject(noidung)
                            }
                        });
                        return data;
                    } else
                    {
                        throw new Exception("Trùng mã cấp tỉnh");
                    }
                    
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi thêm cấp tỉnh: {ex.Message}");
                }
                finally
                {
                    conn?.Close();
                }
            }
        }

        public async Task<CapTinh> Update(CapTinh request)
        {
            using (SqlConnection conn = IConnectData())
            {
                try
                {
                    //Old Data
                    CapTinh ct = await GetByIdLog(request.TinhID);
                    string Oldjson = JsonConvert.SerializeObject(ct);
                    CapTinhBase OldbaseModel = JsonConvert.DeserializeObject<CapTinhBase>(Oldjson);
                    NoiDungDaNgu Oldnoidung = JsonConvert.DeserializeObject<NoiDungDaNgu>(Oldjson);

                    await conn.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@TinhID", request.TinhID);
                    parameters.Add("@NoiDungID", request.TinhID);
                    parameters.Add("@LoaiDM", request.LoaiDM);
                    parameters.Add("@NoiDung", request.NoiDung);
                    parameters.Add("@NgonNguID", request.NgonNguID);
                    parameters.Add("@SapXep", request.SapXep);
                    parameters.Add("@TrangThai", request.TrangThai);
                    parameters.Add("@MaTinh", request.MaTinh);

                    CapTinh data = await conn.QueryFirstOrDefaultAsync<CapTinh>(
                        "SP_CapTinh_Update",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );
                    //Ghi Log
                    if (data != null)
                    {
                        //New Data
                        string json = JsonConvert.SerializeObject(data);
                        CapTinhBase baseModel = JsonConvert.DeserializeObject<CapTinhBase>(json);
                        NoiDungDaNgu noidung = JsonConvert.DeserializeObject<NoiDungDaNgu>(json);
                        await _mediator.Send(new Application.Audit.GhiLog.Command
                        {
                            Data = new CSDL_Log()
                            {
                                ObjectID = data.TinhID.ToString(),
                                TableName = "DM_DiaPhuongCapTinh",
                                Action = (byte)EnumAction.Sua,
                                NewData = JsonConvert.SerializeObject(baseModel),
                                OldData = JsonConvert.SerializeObject(OldbaseModel)
                            }
                        });
                        await _mediator.Send(new Application.Audit.GhiLog.Command
                        {
                            Data = new CSDL_Log()
                            {
                                ObjectID = data.TinhID.ToString(),
                                TableName = "DM_NoiDungDaNgu",
                                Action = (byte)EnumAction.Sua,
                                NewData = JsonConvert.SerializeObject(noidung),
                                OldData = JsonConvert.SerializeObject(Oldnoidung)
                            }
                        });
                    }
                    return data;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi cập nhật cấp tỉnh: {ex.Message}");
                }
                finally
                {
                    conn?.Close();
                }
            }
        }

        public async Task<int> Delete(Guid id)
        {
            using (SqlConnection conn = IConnectData())
            {
                try
                {
                    //Old Data
                    CapTinh ct = await GetByIdLog(id);
                    string Oldjson = JsonConvert.SerializeObject(ct);
                    CapTinhBase OldbaseModel = JsonConvert.DeserializeObject<CapTinhBase>(Oldjson);
                    NoiDungDaNgu Oldnoidung = JsonConvert.DeserializeObject<NoiDungDaNgu>(Oldjson);

                    await conn.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@TinhID", id);

                    int data =  await conn.ExecuteAsync(
                        "SP_CapTinh_Delete",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );
                    if(data != 0)
                    {
                        await _mediator.Send(new Application.Audit.GhiLog.Command
                        {
                            Data = new CSDL_Log()
                            {
                                ObjectID = id.ToString(),
                                TableName = "DM_DiaPhuongCapTinh",
                                Action = (byte)EnumAction.Xoa,
                                OldData = JsonConvert.SerializeObject(OldbaseModel)
                            }
                        });
                        await _mediator.Send(new Application.Audit.GhiLog.Command
                        {
                            Data = new CSDL_Log()
                            {
                                ObjectID = id.ToString(),
                                TableName = "DM_NoiDungDaNgu",
                                Action = (byte)EnumAction.Sua,
                                OldData = JsonConvert.SerializeObject(Oldnoidung)
                            }
                        });
                    }
                    return data;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi xóa cấp tỉnh: {ex.Message}");
                }
                finally
                {
                    conn?.Close();
                }
            }
        }
    }


}
