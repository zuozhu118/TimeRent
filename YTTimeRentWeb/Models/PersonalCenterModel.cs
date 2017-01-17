using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YTTimeRentWeb.Models
{
    public class PersonalCenterModel
    {
        public long Id { get; set; }//订单编号
        public long? Shop_Id { get; set; }//店铺编号id
        public string AccountName { get; set; }//姓名
        public string CarName { get; set; }//车名
        public string StartTime { get; set; }//预约开始用车时间
        public string EndTime { get; set; }//预约结束用车时间
        public string ActualPickTime { get; set; }
        public string ActualReturnTime { get; set; }
        public string PickupShop_Name { get; set; }//取车门店
        public string ReturnShop_Name { get; set; }//还车门店
        public double? OrderDeposit { get; set; }//订单押金
        public string OrderStatus { get; set; }//订单状态
        public int? PayStatus { get; set; }//支付状态
        public int? OrderStatusint { get; set; }
        public string PayStatusstr { get; set; }
        public string OrderNum { get; set; }//订单流水号
        public double? OrderMoney { get; set; }//实际金额、
        public string PicAddress { get; set; }//取车门店地址；
        public string RetAddress { get; set; }//还车门店地址；
        public string PICCity { get; set; }//取车门店所在的市；
        public string RETCity { get; set; }//还车门店市；
        public DateTime CreatTime { get; set; }
        public double? BudgetCost { get; set; }//预算费用;
        public string carimg { get; set; }//照片
        public double? CarDeposit { get; set; }//车辆使用押金
        public double? PeccancyDeposit { get; set; }//车辆违章押金；

        public double? RentDeposit { get; set; }//押金

        public string TotalPrice { get; set; }//订单总价；
        public double? Insurance { get; set; }//车辆保险；
        public string Rentcounttime { get; set; }//租车时间数；
    }
}