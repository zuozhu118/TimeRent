using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CKExpr;
using CKRule;

namespace TimeRent.Common
{
    public class FeeClass
    {
        public double RentFee { get; set; }
        public double InsuranceFee { get; set; }
        public double SinglePrice { get; set; }
        DateTime StartTime;
        DateTime EndTime;

        public FeeClass(DateTime STime, DateTime ETime)
        {
            StartTime = STime;
            EndTime = ETime;
            CKRuleClass ck = new CKRuleClass();
            ck.StartTime = StartTime;
            ck.EndTime = EndTime;
            ck = new RuleFacade().Exec("价格规则配置2", ck);
            RentFee = ck.RentFee;
            InsuranceFee = Math.Round(ck.InsuranceFee, 1);
            SinglePrice = Math.Round(ck.SinglePrice, 1);
        }
    }

    [Serializable]
    public class CKRuleClass
    {
        public DateTime StartTime { get; set; }//租车开始时间，输入
        public DateTime EndTime { get; set; }//租车结束时间,输入

        public double RentFee { get; set; }//租车费用，输出
        public double InsuranceFee { get; set; }//租车保险，输出
        public string RentDateType { get; set; }//租车日期类型，如节假日，工作日，双休日，输出
        public double SinglePrice { get; set; }

        public double StartElectronic { get; set; }//取车电量百分比，输入
        public double EndElectronic { get; set; }//还车电量百分比，输入
        public double FullElectronicFee { get; set; }//车型满电电量电价，输入

        public double ElectronicFee { get; set; }//租车用电费用，输出
    }
}