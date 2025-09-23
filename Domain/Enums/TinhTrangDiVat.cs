using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum EnumTinhTrangDiTich : byte
    {
        /// <summary>
        /// Tốt
        /// </summary>
        [Description("Tốt")]
        Tot = 1,

        /// <summary>
        /// Trung bình
        /// </summary>
        [Description("Trung bình")]
        TrungBinh = 2,

        /// <summary>
        /// Xuống cấp
        /// </summary>
        [Description("Xuống cấp")]
        XuongCap = 3,

        /// <summary>
        /// Đã trùng tu
        /// </summary>
        [Description("Đã trùng tu")]
        DaTrungTu = 4
    }
}
