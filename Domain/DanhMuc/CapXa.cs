namespace Domain.DanhMuc
{
    public class CapXa : NoiDungDaNgu
    {
        public Guid? XaID { get; set; }               // tinyint (Identity)
        public byte SapXep { get; set; }           // tinyint
        public bool TrangThai { get; set; }        // bit
        public Guid? TinhID { get; set; }
        public string TenTinh { get; set; }
        public int Cap { get; set; } //1= phuong ; 2=xa ; 3= dac khu
        public int MaXa { get; set; }
    }
    public class CapXaBase
    {
        public Guid? XaID { get; set; }               // tinyint (Identity)
        public byte SapXep { get; set; }           // tinyint
        public bool TrangThai { get; set; }        // bit
        public Guid? TinhID { get; set; }
        public int Cap { get; set; } //1= phuong ; 2=xa ; 3= dac khu
        public int MaXa { get; set; }
    }
    public class CapXaTrinhDien
    {
        public Guid? XaID { get; set; }               // tinyint (Identity)
        public byte SapXep { get; set; }           // tinyint
        public bool TrangThai { get; set; }        // bit
        public Guid? TinhID { get; set; }
        public string NoiDung { get; set; }
        public IEnumerable<NoiDungDaNgu> DaNgu { get; set; }
        public int Cap { get; set; } //1= phuong ; 2=xa ; 3= dac khu
        public int MaXa { get; set; }
        public string TenTinh { get; set; }
    }
}
