//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace YTTimeRentWeb.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class DoorPassword
    {
        public int Id { get; set; }
        public long CarId { get; set; }
        public Nullable<System.DateTime> RequestTime { get; set; }
        public string DeviceNo { get; set; }
        public string CurrentPWD { get; set; }
        public string NextPWD { get; set; }
        public Nullable<long> Status { get; set; }
    }
}
