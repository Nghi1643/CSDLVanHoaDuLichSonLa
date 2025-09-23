using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum EnumMenuGroup
    {
        /// <summary>
        /// Nhóm menu chính
        /// </summary>
        [Description("CHÍNH")]
        Nhom1 = 1,

        /// <summary>
        /// Nhóm menu văn hoá
        /// </summary>
        [Description("VĂN HOÁ")]
        Nhom2 = 2,

        /// <summary>
        /// Nhóm menu thể thao
        /// </summary>
        [Description("THỂ THAO")]
        Nhom3 = 3,

        /// <summary>
        /// Nhóm menu du lịch
        /// </summary>
        [Description("DU LỊCH")]
        Nhom4 = 4,

        /// <summary>
        /// Nhóm menu báo chí - xuất bản
        /// </summary>
        [Description("BÁO CHÍ - XUẤT BẢN")]
        Nhom5 = 5,

        /// <summary>
        /// Nhóm menu thống kê - báo cáo
        /// </summary>
        [Description("THỐNG KÊ - BÁO CÁO")]
        Nhom6 = 6,

        /// <summary>
        /// Nhóm menu hệ thống
        /// </summary>
        [Description("HỆ THỐNG")]
        Nhom7 = 7
    }
}
