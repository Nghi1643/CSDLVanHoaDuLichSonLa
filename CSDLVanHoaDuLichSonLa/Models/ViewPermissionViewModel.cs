namespace CSDLVanHoaDuLichSonLa.Models
{
    public class ViewPermissionViewModel
    {
        public int PermitedView { get; set; }
        public int PermitedEdit { get; set; }
        public int PermitedDelete { get; set; }
        public int PermitedApprove { get; set; }
        public int PermitedCreate { get; set; }
    }

    public class DataPermissionViewModel
    {
        public bool IsAdmin { get; set; }
        public string UniqueCode { get; set; }
        public string LinhVuc { get; set; }
    }
}
