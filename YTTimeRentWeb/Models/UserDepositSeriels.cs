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
    
    public partial class UserDepositSeriels
    {
        public long Id { get; set; }
        public long UserDeposit_Id { get; set; }
        public Nullable<long> Order_Id { get; set; }
        public Nullable<double> DeducFee { get; set; }
        public Nullable<long> ChargeSerielId { get; set; }
        public int Classify { get; set; }
        public string PayName { get; set; }
        public string PayMentAccount { get; set; }
        public Nullable<long> PayMethod { get; set; }
        public System.DateTime CreatTime { get; set; }
        public string Remark { get; set; }
    }
}
