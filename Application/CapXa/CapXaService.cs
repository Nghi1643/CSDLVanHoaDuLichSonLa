using Azure.Core;
using Dapper;
using Domain;
using Domain.DanhMuc;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Data;
using Microsoft.Data.SqlClient;

namespace CSDLVanHoaDuLichSonLa.Services
{
    /// <summary>
    /// Interface quản lý Cấp Xã
    /// </summary>
    public interface ICapXaService
    {
        Task<IEnumerable<CapXaTrinhDien>> Gets();
        Task<CapXaTrinhDien> GetById(Guid? id);
        Task<CapXa> Create(CapXa request);
        Task<CapXa> Update(CapXa request);
        Task<int> Delete(Guid id);
    }

    /// <summary>
    /// Service quản lý Cấp Xã
    /// </summary>
    public class CapXaService : ConnectDatabase, ICapXaService
    {
        private readonly IMediator _mediator;
        private readonly INoiDungDaNguServices _noidung;

        public CapXaService(IConfiguration configuration, IMediator mediator, INoiDungDaNguServices noidung) : base(configuration)
        {
            _mediator = mediator;
            _noidung = noidung;
        }

        /// <summary>
        /// Lấy danh sách tất cả Cấp Xã
        /// </summary>
        public async Task<IEnumerable<CapXaTrinhDien>> Gets()
        {
            using (SqlConnection conn = IConnectData())
            {
                try
                {
                    await conn.OpenAsync();
                    IEnumerable<CapXa> data = await conn.QueryAsync<CapXa>(
                        "SP_CapXa_GetAll",
                        commandType: CommandType.StoredProcedure
                    );
                    List<CapXaTrinhDien> dsCapXa = new List<CapXaTrinhDien>();
                    foreach (CapXa item in data)
                    {
                        CapXaTrinhDien iCapXa = new CapXaTrinhDien();
                        iCapXa.TinhID = item.TinhID;
                        iCapXa.XaID = item.XaID;
                        iCapXa.TrangThai = item.TrangThai;
                        iCapXa.SapXep = item.SapXep;
                        iCapXa.NoiDung = item.NoiDung;
                        iCapXa.MaXa = item.MaXa;
                        iCapXa.Cap = item.Cap;
                        iCapXa.TenTinh = item.TenTinh;
                        iCapXa.DaNgu = await _noidung.Gets(item.XaID, item.LoaiDM);

                        dsCapXa.Add(iCapXa);
                    }
                    return dsCapXa;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi lấy danh sách Cấp Xã: {ex.Message}");
                }
                finally { conn?.Close(); }
            }
        }

        /// <summary>
        /// Lấy thông tin 1 Cấp Xã theo ID
        /// </summary>
        public async Task<CapXa> GetByIdLog(Guid? id)
        {
            using (SqlConnection conn = IConnectData())
            {
                try
                {
                    await conn.OpenAsync();
                    var param = new DynamicParameters();
                    param.Add("@XaID", id);

                    return await conn.QueryFirstOrDefaultAsync<CapXa>(
                        "SP_CapXa_GetById",
                        param,
                        commandType: CommandType.StoredProcedure
                    );
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi lấy thông tin Cấp Xã: {ex.Message}");
                }
                finally { conn?.Close(); }
            }
        }
        public async Task<CapXaTrinhDien> GetById(Guid? id)
        {
            using (SqlConnection conn = IConnectData())
            {
                try
                {
                    await conn.OpenAsync();
                    var param = new DynamicParameters();
                    param.Add("@XaID", id);

                    CapXa data = await conn.QueryFirstOrDefaultAsync<CapXa>(
                        "SP_CapXa_GetById",
                        param,
                        commandType: CommandType.StoredProcedure
                    );
                    CapXaTrinhDien iCapXa = new CapXaTrinhDien();
                    iCapXa.TinhID = data.TinhID;
                    iCapXa.XaID = data.XaID;
                    iCapXa.TrangThai = data.TrangThai;
                    iCapXa.SapXep = data.SapXep;
                    iCapXa.NoiDung = data.NoiDung;
                    iCapXa.MaXa = data.MaXa;
                    iCapXa.Cap = data.Cap;
                    iCapXa.TenTinh = data.TenTinh;
                    iCapXa.DaNgu = await _noidung.Gets(data.XaID, data.LoaiDM);
                    return iCapXa;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi lấy thông tin Cấp Xã: {ex.Message}");
                }
                finally { conn?.Close(); }
            }
        }

        /// <summary>
        /// Thêm mới 1 Cấp Xã
        /// </summary>
        public async Task<CapXa> Create(CapXa request)
        {
            using (SqlConnection conn = IConnectData())
            {
                try
                {
                    await conn.OpenAsync();
                    var param = new DynamicParameters();
                    // Thuộc tính từ NoiDungDaNgu
                    param.Add("@NoiDungID", Guid.NewGuid());
                    param.Add("@LoaiDM", request.LoaiDM);
                    param.Add("@NoiDung", request.NoiDung);
                    param.Add("@NgonNguID", request.NgonNguID);

                    // Thuộc tính riêng của CapXa
                    param.Add("@SapXep", request.SapXep);
                    param.Add("@TrangThai", request.TrangThai);
                    param.Add("@TinhID", request.TinhID);
                    param.Add("@Cap", request.Cap);
                    param.Add("@MaXa", request.MaXa);

                    CapXa data =  await conn.QueryFirstOrDefaultAsync<CapXa>(
                        "SP_CapXa_Insert",
                        param,
                        commandType: CommandType.StoredProcedure
                    );
                    if (data.XaID != null)
                    {

                        string json = JsonConvert.SerializeObject(data);
                        CapXaBase baseModel = JsonConvert.DeserializeObject<CapXaBase>(json);
                        NoiDungDaNgu noidung = JsonConvert.DeserializeObject<NoiDungDaNgu>(json);
                        await _mediator.Send(new Application.Audit.GhiLog.Command
                        {
                            Data = new CSDL_Log()
                            {
                                ObjectID = data.XaID.ToString(),
                                TableName = "DM_DiaPhuongCapXa",
                                Action = (byte)EnumAction.Them,
                                NewData = JsonConvert.SerializeObject(baseModel)
                            }
                        });
                        await _mediator.Send(new Application.Audit.GhiLog.Command
                        {
                            Data = new CSDL_Log()
                            {
                                ObjectID = data.XaID.ToString(),
                                TableName = "DM_NoiDungDaNgu",
                                Action = (byte)EnumAction.Them,
                                NewData = JsonConvert.SerializeObject(noidung)
                            }
                        });
                       
                    }
                    return data;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi thêm Cấp Xã: {ex.Message}");
                }
                finally { conn?.Close(); }
            }
        }
       

        /// <summary>
        /// Cập nhật thông tin Cấp Xã
        /// </summary>
        public async Task<CapXa> Update(CapXa request)
        {
            using (SqlConnection conn = IConnectData())
            {
                try
                {
                    //Old Data
                    CapXa ct = await GetByIdLog(request.XaID);
                    string Oldjson = JsonConvert.SerializeObject(ct);
                    CapXaBase OldbaseModel = JsonConvert.DeserializeObject<CapXaBase>(Oldjson);
                    NoiDungDaNgu Oldnoidung = JsonConvert.DeserializeObject<NoiDungDaNgu>(Oldjson);

                    await conn.OpenAsync();
                    var param = new DynamicParameters();
                    // Khóa chính
                    param.Add("@XaID", request.XaID);

                    // Thuộc tính từ NoiDungDaNgu
                    param.Add("@NoiDungID", request.NoiDungID);
                    param.Add("@LoaiDM", request.LoaiDM);
                    param.Add("@NoiDung", request.NoiDung);
                    param.Add("@NgonNguID", request.NgonNguID);

                    // Thuộc tính riêng của CapXa
                    param.Add("@SapXep", request.SapXep);
                    param.Add("@TrangThai", request.TrangThai);
                    param.Add("@TinhID", request.TinhID);
                    param.Add("@Cap", request.Cap);
                    param.Add("@MaXa", request.MaXa);

                    CapXa data = await conn.QueryFirstOrDefaultAsync<CapXa>(
                        "SP_CapXa_Update",
                        param,
                        commandType: CommandType.StoredProcedure
                    );
                    //Ghi Log
                    if (data.XaID != null)
                    {
                        //New Data
                        string json = JsonConvert.SerializeObject(data);
                        CapXaBase baseModel = JsonConvert.DeserializeObject<CapXaBase>(json);
                        NoiDungDaNgu noidung = JsonConvert.DeserializeObject<NoiDungDaNgu>(json);
                        await _mediator.Send(new Application.Audit.GhiLog.Command
                        {
                            Data = new CSDL_Log()
                            {
                                ObjectID = data.TinhID.ToString(),
                                TableName = "DM_DiaPhuongCapXa",
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
                    throw new Exception($"Lỗi cập nhật Cấp Xã: {ex.Message}");
                }
                finally { conn?.Close(); }
            }
        }

        /// <summary>
        /// Xóa Cấp Xã
        /// </summary>
        public async Task<int> Delete(Guid id)
        {
            using (SqlConnection conn = IConnectData())
            {
                try
                {
                    //Old Data
                    CapXa ct = await GetByIdLog(id);
                    string Oldjson = JsonConvert.SerializeObject(ct);
                    CapXaBase OldbaseModel = JsonConvert.DeserializeObject<CapXaBase>(Oldjson);
                    NoiDungDaNgu Oldnoidung = JsonConvert.DeserializeObject<NoiDungDaNgu>(Oldjson);

                    await conn.OpenAsync();
                    var param = new DynamicParameters();
                    param.Add("@XaID", id);

                    int data = await conn.ExecuteAsync(
                        "SP_CapXa_Delete",
                        param,
                        commandType: CommandType.StoredProcedure
                    );
                    if (data != 0)
                    {
                        await _mediator.Send(new Application.Audit.GhiLog.Command
                        {
                            Data = new CSDL_Log()
                            {
                                ObjectID = id.ToString(),
                                TableName = "DM_DiaPhuongCapXa",
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
                    throw new Exception($"Lỗi xóa Cấp Xã: {ex.Message}");
                }
                finally { conn?.Close(); }
            }
        }
    }
}
