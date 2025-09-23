namespace Domain.DanhMuc
{
    public class DanToc : NoiDungDaNgu
    {
        public string MaDanToc { get; set; }       // char(2)
        public byte ThuTu { get; set; }            // tinyint
        public bool TrangThai { get; set; }        // bit
        public Guid? DanTocID { get; set; }         // uniqueidentifier
    }
    public class DanTocBase
    {
        public string MaDanToc { get; set; }       // char(2)
        public byte ThuTu { get; set; }            // tinyint
        public bool TrangThai { get; set; }        // bit
        public Guid? DanTocID { get; set; }         // uniqueidentifier
    }
    public class DanTocTrinhDien
    {
        public string MaDanToc { get; set; }       // char(2)
        public byte ThuTu { get; set; }            // tinyint
        public bool TrangThai { get; set; }        // bit
        public Guid? DanTocID { get; set; }         // uniqueidentifier
        public string NoiDung { get; set; }
        public IEnumerable<NoiDungDaNgu> DaNgu { get; set; }
    }
}
