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
    
    public partial class OrderMsg
    {
        public long Id { get; set; }
        public Nullable<long> OrderId { get; set; }
        public Nullable<int> Classify { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<System.DateTime> CreatTime { get; set; }
        public Nullable<System.DateTime> SendTime { get; set; }
    }
}
