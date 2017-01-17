using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YTTimeRentWeb.Models
{
    [Serializable]
    public class RentCarFee
    {
        public DateTime StartTime { get; set; }//预约租车开始时间，输入
        public DateTime EndTime { get; set; }//预约租车结束时间,输入
        public DateTime ActualStartTime { get; set; }//实际租车结束时间,输入
        public DateTime ActualEndTime { get; set; }//实际租车结束时间,输入
        public string CarClassCode { get; set; }//车型名称，如江铃E200，输入
        public int Type { get; set; }//预约租车费用，输入
        public double Electricity { get; set; }//车辆电量,输入
        public bool IsBilling { get; set; }//是否已经开始计费

        public double SinglePrice { get; set; }//单价,输出
        public double RentFee { get; set; }//预约租车费用，输出
        public double InsuranceFee { get; set; }//租车保险，输出
        public double ActualRentFee { get; set; }//预约租车费用，输出
        public double ActualInsuranceFee { get; set; }//预约租车费用，输出

    }
}