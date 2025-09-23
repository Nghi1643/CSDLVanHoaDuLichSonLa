using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Domain.Enums
{
    public enum EnumAction : byte
    {
        /// <summary>
        /// Xem
        /// </summary>
        [Description("Xem")]
        Xem = 1,

        /// <summary>
        /// Tải về
        /// </summary>
        [Description("Tải về")]
        TaiVe = 2,

        /// <summary>
        /// Thêm
        /// </summary>
        [Description("Thêm")]
        Them = 3,

        /// <summary>
        /// Sửa
        /// </summary>
        [Description("Sửa")]
        Sua = 4,

        /// <summary>
        /// Xoá
        /// </summary>
        [Description("Xoá")]
        Xoa = 5,

        /// <summary>
        /// Duyệt
        /// </summary>
        [Description("Duyệt")]
        Duyet = 6
    }
}