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
    
    public partial class UserDeposit
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public double Deposit { get; set; }
        public System.DateTime CreateTime { get; set; }
        public System.DateTime UpdateTime { get; set; }
        public Nullable<System.DateTime> ExpirationDate { get; set; }
        public int UnusableDay { get; set; }
        public Nullable<int> IdentityAuth { get; set; }
        public Nullable<int> ZmopAuth { get; set; }
        public Nullable<int> Status { get; set; }
    }
}
