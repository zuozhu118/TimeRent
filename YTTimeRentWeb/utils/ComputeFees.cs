using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CKExpr;
using CKRule;
using YTTimeRentWeb.Models;

namespace YTTimeRentWeb.utils
{
    public class ComputeFees
    {
        RentCarFee rcf = new RentCarFee();
        string classcode;

        /// <summary>
        /// 用于计算预算价格
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="CarClassCode">车型编码，CarClass中的Code，与配置文件名相同</param>
        public ComputeFees(DateTime StartTime, DateTime EndTime, string CarClassCode)
        {
            rcf.StartTime = StartTime;
            rcf.EndTime = EndTime;
            rcf.CarClassCode = CarClassCode.Replace("/","");
            rcf.Type = 1;
            classcode = CarClassCode.Replace("/", "");
        }

        /// <summary>
        /// 用户计算实际价格
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="ActualStartTime"></param>
        /// <param name="ActualEndTime"></param>
        /// <param name="CarClassCode">车型编码，CarClass中的Code，与配置文件名相同</param>
        public ComputeFees(DateTime StartTime, DateTime EndTime, DateTime ActualStartTime, DateTime ActualEndTime, string CarClassCode)
        {
            if (ActualStartTime == DateTime.MinValue && EndTime == DateTime.MinValue)
            {
                rcf.Type = 3;

            }
            else
            {
                rcf.EndTime = EndTime;
                rcf.ActualStartTime = ActualStartTime;
                rcf.Type = 2;
            }
            rcf.StartTime = StartTime;
            rcf.ActualEndTime = ActualEndTime;
            rcf.CarClassCode = CarClassCode.Replace("/", "");
            classcode = CarClassCode.Replace("/", "");
        }

        /// <summary>
        /// 用户计算取消价格
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="ActualStartTime"></param>
        /// <param name="ActualEndTime"></param>
        /// <param name="CarClassCode"></param>
        public ComputeFees(DateTime StartTime, DateTime EndTime, DateTime ActualStartTime, DateTime ActualEndTime, string CarClassCode, float Electricity, bool IsBilling)
        {
            rcf.EndTime = EndTime;
            rcf.ActualStartTime = ActualStartTime;

            rcf.StartTime = StartTime;
            rcf.ActualEndTime = ActualEndTime;
            rcf.Type = 3;
            rcf.CarClassCode = CarClassCode.Replace("/", "");
            rcf.Electricity = Electricity;
            rcf.IsBilling = IsBilling;

            classcode = CarClassCode.Replace("/", "");
        }

        public double getRentCarFee()
        {
            try
            {
                rcf = new RuleFacade().Exec(rcf.CarClassCode, rcf);
                rcf.CarClassCode = classcode;
                return Math.Round(rcf.RentFee, 1);
            }
            catch (Exception ex)
            {
                return 0.0;
            }
        }
        public double getInsuranceFee()
        {
            try
            {
                rcf = new RuleFacade().Exec(rcf.CarClassCode, rcf);
                rcf.CarClassCode = classcode;
                return Math.Round(rcf.InsuranceFee, 1);
            }
            catch (Exception ex)
            {
                return 0.0;
            }
        }
        public double getSinglePrice()
        {
            try
            {
                rcf = new RuleFacade().Exec(rcf.CarClassCode, rcf);
                rcf.CarClassCode = classcode;
                return Math.Round(rcf.SinglePrice, 1);
            }
            catch (Exception ex)
            {
                return 0.0;
            }
        }
        public double getActualRentCarFee()
        {
            try
            {
                rcf = new RuleFacade().Exec(rcf.CarClassCode, rcf);
                rcf.CarClassCode = classcode;
                return Math.Round(rcf.ActualRentFee, 1);
            }
            catch (Exception ex)
            {
                return 0.0;
            }
        }
        public double getActualInsuranceFee()
        {
            try
            {
                rcf = new RuleFacade().Exec(rcf.CarClassCode, rcf);
                rcf.CarClassCode = classcode;
                return Math.Round(rcf.ActualInsuranceFee, 1);
            }
            catch (Exception ex)
            {
                return 0.0;
            }
        }
    }
}