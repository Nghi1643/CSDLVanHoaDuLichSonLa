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
    /// Interface quản lý Quốc Tịch
    /// </summary>
    public interface INoiDungDaNguServices
    {
        Task<IEnumerable<NoiDungDaNgu>> Gets(Guid? id,int LoaiDM);
        
        
    }

    /// <summary>
    /// Service quản lý Quốc Tịch
    /// </summary>s
    public class NoiDungDaNguServices : ConnectDatabase, INoiDungDaNguServices
    {

        public NoiDungDaNguServices(IConfiguration configuration) : base(configuration) {
           
        }



        /// <summary>
        /// Lấy thông tin Quốc Tịch theo ID
        /// </summary>
        public async Task<IEnumerable<NoiDungDaNgu>> Gets(Guid? id,int LoaiDM)
        {
            using (SqlConnection conn = IConnectData())
            {
                try
                {
                    await conn.OpenAsync();

                    var param = new DynamicParameters();
                    param.Add("@NoiDungID", id);
                    param.Add("@LoaiDM", LoaiDM);

                    IEnumerable<NoiDungDaNgu> data =  await conn.QueryAsync<NoiDungDaNgu>(
                        "SP_NoiDungDaNgu_Gets",
                        param,
                        commandType: CommandType.StoredProcedure
                    );
                    return data;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi lấy thông tin Quốc Tịch: {ex.Message}");
                }
                finally { conn?.Close(); }
            }
        }

    
    }
}
