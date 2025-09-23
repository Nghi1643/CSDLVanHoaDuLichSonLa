namespace Domain.DanhMuc
{
    public class QuocTich : NoiDungDaNgu
    {
        public string MaQuocTich { get; set; }      // nvarchar(20)
        public int ThuTu { get; set; }             // int
        public Guid? QuocTichID { get; set; }       // uniqueidentifier
    }
    public class QuocTichBase
    {
        public string MaQuocTich { get; set; }      // nvarchar(20)
        public int ThuTu { get; set; }             // int
        public Guid? QuocTichID { get; set; }       // uniqueidentifier
    }
    public class QuocTichTrinhDien
    {
        public string MaQuocTich { get; set; }      // nvarchar(20)
        public int ThuTu { get; set; }             // int
        public Guid? QuocTichID { get; set; }       // uniqueidentifier
        public IEnumerable<NoiDungDaNgu> DaNgu { get; set; }
    }
}
