using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace CSDLVanHoaDuLichSonLa.Services
{
    public class ConnectDatabase
    {
        private readonly IConfiguration _configuration;
        public ConnectDatabase(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SqlConnection IConnectData()
        {
            try
            {
                var conn = new SqlConnection
                {
                    ConnectionString = _configuration.GetConnectionString("DefaultConnection")
                };

                return conn;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
