namespace Domain.CaNhan
{
    public class CaNhan_VaiTroXaHoi
    {
        public Guid CaNhanID { get; set; }
        public int VaiTroID { get; set; }
    }
    public class CaNhan_VaiTroXaHoi_DTO
    {
        public Guid CaNhanID { get; set; }
        public int VaiTroID { get; set; }
        public string TenVaiTro { get; set; }
    }
}
