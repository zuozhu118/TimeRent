using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YTTimeRentWeb.Models
{
    public class BasicMsgModel
    {
        public string Carname { get; set; }//车名
        public string carimg { get; set; }//照片
        public string piccity { get; set; }//取车城市
        public string retcity { get; set; }//还车城市
        public string picshop { get; set; }//取车门店
        public string retshop { get; set; }//还车门店
        //public string PicDAY { get; set; }//取车日期
        //public string RetDAY { get; set; }//还车日期
        //public string Pichourmins { get; set; }//取车时刻
        //public string Rethourmins { get; set; }//还车时刻
        public DateTime PickTime { get; set; }
        public DateTime RetTime { get; set; }
        public string picadress { get; set; }//取车地址；
        public string retadress { get; set; }//还车地址；
        public decimal? picLongitude { get; set; }//取车经度
        public decimal? picLatitude { get; set; }//取车纬度
        public decimal? retLongitude { get; set; }//还车经度
        public decimal? retLatitude { get; set; }//还车纬度
        //public string RENTCARTIME { get; set; }//租车时间
        public double costTotal { get; set; }//车辆租金；
        public double cost_hour { get; set; }//单价；
        public string hourcounts { get; set; }//租车时间；

        public double PeccancyDepositPeccancyDeposit { get; set; }//违章押金
        public double CarDeposit { get; set; }//车辆押金

        public double? RentDeposit { get; set; }//租车押金
       
        public double insurance { get; set; }//保险
        public double OrderTotalPrice { get; set; }//总价
        public string OrderNum { get; set; }//租车订单
        public string RENTMAN { get; set; }//租车人

    }
}