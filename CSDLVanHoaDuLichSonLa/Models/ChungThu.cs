using System.Runtime.Serialization;

namespace CSDLVanHoaDuLichSonLa.Models
{
    public class ChungThu
    {
        #region Tóm lược keyword tài khoản
        /// <summary>
        /// Mã định danh duy nhất của tài khoản
        /// </summary>
        [DataMember]
        public Guid IDTaiKhoan { get; set; }
        /// <summary>
        /// Tên tài khoản sử dụng
        /// </summary>
        [DataMember]
        public string TenTaiKhoan { get; set; }
        /// <summary>
        /// loại tài khoản cá nhân/đơn vị
        /// </summary>
        [DataMember]
        public int LoaiTaiKhoan { get; set; }
        /// <summary>
        /// Vai trò tài khoản hiện hành
        /// </summary>
        [DataMember]
        public int VaiTro { get; set; }
        /// <summary>
        /// Là công chức 
        /// </summary>
        [DataMember]
        public bool LaCongVienChuc { get; set; }
        /// <summary>
        /// Mật khẩu người sử dụng
        /// </summary>
        [DataMember]
        public string MaGuid { get; set; }
        #endregion

        #region Tóm lược thông tin cá nhân
        /// <summary>
        /// email sử dụng
        /// </summary>
        [DataMember]
        public string EmailCaNhan { get; set; }
        /// <summary>
        /// Mã cá nhân dùng làm khóa chính để truy lục thông tin cá nhân
        /// </summary>
        //[DataMember]
        //public Guid IDCaNhan { get; set; }
        /// <summary>
        /// Mã cá nhân 
        /// </summary>
        [DataMember]
        public string MaCaNhan { get; set; }
        /// <summary>
        /// Họ tên đầy đủ
        /// </summary>
        [DataMember]
        public string HoVaTen { get; set; }
        [DataMember]
        public string DienThoaiCaNhan { get; set; }
        [DataMember]
        public int LoaiGiayToCaNhan { get; set; }
        [DataMember]
        public string MaSoCaNhan { get; set; }
        //[DataMember]
        //public DateTime NgayCapGiayToCaNhan { get; set; }
        [DataMember]
        public bool GioiTinh { get; set; }
        [DataMember]
        public string DiaChiThuongTru { get; set; }
        //[DataMember]
        //public string ThuongTruTinhThanh { get; set; }
        //[DataMember]
        //public string ThuongTruQuanHuyen { get; set; }
        [DataMember]
        public string ThuongTruXaPhuong { get; set; }
        #endregion

        #region Tóm lược thông tin đơn vị
        /// <summary>
        /// Mã đơn vị dùng làm khóa để truy lục thông tin đơn vị
        /// </summary>
        //[DataMember]
        //public Guid IDDonVi { get; set; }
        /// <summary>
        /// Mã cơ quan, tổ chức (PKID).
        /// </summary>
        [DataMember]
        public string MaDonVi { get; set; }
        /// <summary>
        /// Tên đơn vị
        /// </summary>
        [DataMember]
        public string TenDonVi { get; set; }
        [DataMember]
        public string EmailDonVi { get; set; }
        [DataMember]
        public string DienThoaiDonVi { get; set; }
        [DataMember]
        public string DiaChiDonVi { get; set; }
        [DataMember]
        public string MaSoThueDonVi { get; set; }
        //[DataMember]
        //public string SoThanhLapDonVi { get; set; }
        #endregion

        #region Tóm lược thông tin chứng thư gốc
        //[DataMember]
        //public WebLog WebLog { get; set; }

        /// <summary>
        /// ID chứng thư hiện hành
        /// </summary>
        [DataMember]
        public Guid IDchungThu { get; set; }

        /// <summary>
        /// Mã chứng thư được cấp phiên làm việc hiện hành 
        /// </summary>
        [DataMember]
        public string MaChungThu { get; set; }
        #endregion
    }
}
