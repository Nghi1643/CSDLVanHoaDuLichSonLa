using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class CSDL_Log
    {
        [Key]
        public int ID { get; set; } 
        public string ObjectID { get; set; } // khóa chính của đối tượng bị thay đổi
        public string TableName { get; set; } // tên bảng của đối tượng bị thay đổi
        public byte? Action { get; set; } // EnumAction
        public string OldData { get; set; } // dữ liệu cũ (trước khi thay đổi)
        public string NewData { get; set; } // dữ liệu mới (sau khi thay đổi)
        public DateTime? CreatedOnDate { get; set; } // thời gian thay đổi
        public int? CreatedByUserID { get; set; } // người thay đổi
        public string IpAddress { get; set; } // địa chỉ IP của người thay đổi
        public string UserAgent { get; set; } // thông tin trình duyệt của người thay đổi
    }
}
