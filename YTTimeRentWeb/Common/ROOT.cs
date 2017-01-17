using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YTTimeRentWeb.Common
{
    public class ROOT
    {
    }
    public class ExtraFeeslist
    {
        /// <summary>
        /// 保险
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 972
        /// </summary>
        public string price { get; set; }
        /// <summary>
        /// 保险必买
        /// </summary>
        public string commit { get; set; }
    }

    public class Saleslist
    {
        /// <summary>
        /// 保险
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 972
        /// </summary>
        public string price { get; set; }
        /// <summary>
        /// 保险必买
        /// </summary>
        public string commit { get; set; }
    }

    public class List
    {
        /// <summary>
        /// Cost
        /// </summary>
        public int Cost { get; set; }
        /// <summary>
        /// Insurance
        /// </summary>
        public int Insurance { get; set; }
        /// <summary>
        /// 0
        /// </summary>
        public string Discount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Score { get; set; }
        /// <summary>
        /// ExtraFeeslist
        /// </summary>
        public List<ExtraFeeslist> ExtraFeeslist { get; set; }
        /// <summary>
        /// Saleslist
        /// </summary>
        public List<Saleslist> Saleslist { get; set; }
        /// <summary>
        /// 1008
        /// </summary>
        public string TotalFee { get; set; }
        /// <summary>
        /// 1000
        /// </summary>
        public string RentalDeposit { get; set; }
        /// <summary>
        /// 0
        /// </summary>
        public string DepositDiscount { get; set; }
    }

    public class Root
    {
        /// <summary>
        /// true
        /// </summary>
        public string isSuccess { get; set; }
        /// <summary>
        /// 租车预算费用获取成功
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// List
        /// </summary>
        public List list { get; set; }
    }

}