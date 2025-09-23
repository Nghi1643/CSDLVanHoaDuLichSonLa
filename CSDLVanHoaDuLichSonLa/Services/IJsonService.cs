using Domain;
using CSDLVanHoaDuLichSonLa .Models;

namespace CSDLVanHoaDuLichSonLa.Services
{
    public interface IJsonService
    {
        void GetData();
        void UpdateData(AdminNavModel vm, AdminNavItemModel itemModel, string RoleName);
        void RecreateData();
    }
}
