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
    
    public partial class Personal_Order
    {
        public long Id { get; set; }
        public string PickupShopName { get; set; }
        public string CarName { get; set; }
        public string OrderNum { get; set; }
        public Nullable<int> OrderStatus { get; set; }
        public System.DateTime StartTime { get; set; }
        public System.DateTime EndTime { get; set; }
        public System.DateTime CreatTime { get; set; }
        public Nullable<double> OrderMoney { get; set; }
        public Nullable<double> BudgetCost { get; set; }
        public Nullable<double> OrderDeposit { get; set; }
        public long Account_Id { get; set; }
        public string CarPic { get; set; }
        public string Name { get; set; }
        public Nullable<int> PayStatus { get; set; }
        public Nullable<int> PayWay { get; set; }
        public string PickupShopCity { get; set; }
        public string ReturnShopCity { get; set; }
        public string ReturnShopName { get; set; }
        public string PickupShopAddress { get; set; }
        public Nullable<long> CarClass_Id { get; set; }
        public string ReturnShopAddress { get; set; }
    }
}