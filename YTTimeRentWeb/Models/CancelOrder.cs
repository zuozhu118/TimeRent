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
    
    public partial class CancelOrder
    {
        public long ID { get; set; }
        public Nullable<long> CID { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<long> FranchiseeID { get; set; }
        public Nullable<long> StoreID { get; set; }
        public Nullable<long> SiteID { get; set; }
        public Nullable<decimal> CounterFee { get; set; }
    }
}
