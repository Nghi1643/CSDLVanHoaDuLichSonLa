namespace Domain.DanhMuc
{
    public class NoiDungDaNgu
    {
        public Guid? NoiDungID { get; set; }        // uniqueidentifier
        public int LoaiDM { get; set; }            // int
        public string NoiDung { get; set; }        // nvarchar(500)
        public Guid NgonNguID { get; set; }        // uniqueidentifier
        public string TenNgonNgu { get; set; }
        public string MoTa { get; set; }
        public string TieuDe { get; set; }

    }
}
