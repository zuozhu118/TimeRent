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
    
    public partial class UnitTimePriceRule
    {
        public long Id { get; set; }
        public long CarClass_Id { get; set; }
        public double Weekday { get; set; }
        public double Weekend { get; set; }
        public double Holiday { get; set; }
        public double Overtime { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<double> Deposit { get; set; }
    }
}
