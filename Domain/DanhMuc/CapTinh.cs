namespace Domain.DanhMuc
{
    public class CapTinh : NoiDungDaNgu
    {
        public byte SapXep { get; set; }           // tinyint
        public bool TrangThai { get; set; }        // bit
        public Guid? TinhID { get; set; }
        public int MaTinh { get; set; }
    }   
    public class CapTinhTrinhDien
    {
        public byte SapXep { get; set; }           // tinyint
        public bool TrangThai { get; set; }        // bit
        public Guid? TinhID { get; set; }
        public int MaTinh { get; set; }
        public string NoiDung { get; set; }
        public IEnumerable<NoiDungDaNgu> DaNgu { get; set; }
    }
    public class CapTinhBase
    {
        public byte SapXep { get; set; }           // tinyint
        public bool TrangThai { get; set; }        // bit
        public Guid? TinhID { get; set; }
        public int MaTinh { get; set; }
    }
   
}
