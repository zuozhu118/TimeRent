using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Come.Alipay;
using Newtonsoft.Json;
using YTTimeRentWeb.Models;
using YTTimeRentWeb.utils;
namespace YTTimeRentWeb.Controllers
{
    public class BasicMsgController : BaseController
    {
        TimeRentEntities entity = new TimeRentEntities();
        public static BasicMsgModel bmsgmodel = new BasicMsgModel();
        //BasicMsgModel bmsgmodel = new BasicMsgModel();
        public ActionResult Index(string OrderId)//string carid, string piccity, string retcity, string picshop, string retshop, string pickday, string retday, string pictime, string rettime
        {
            //======判断是否能获取到Session["IDS"]的值=========================
            string strid;
            object objstrid = Session["IDS"];//获取登录用户的的账户编号；
            if (objstrid == null)
            {
                return RedirectToAction("Logon", "Account");
            }
            else
            {
                strid = objstrid.ToString();
            }

            long AId = Convert.ToInt64(strid);
            var bal = entity.Balances.SingleOrDefault(b => b.Account_Id == AId);
            var money = bal.Balance;
            Session["MONEY"] = money;//将剩余金额存储在Session中；

            InitMsg(OrderId, strid);//, rentcartime, costhour


            ViewData.Model = bmsgmodel;//将值通过Model传递到网页端；
            return View();

        }
        public JsonResult OrderState()
        {
            string strid = Session["IDS"].ToString();//获取登录用户的的账户编号；
            long AId = Convert.ToInt64(strid);
            var orders = entity.Order.Where(o => o.Account_Id == AId && (o.OrderStatus == 1 || o.OrderStatus == 2)).ToList();//from O in entity.Order where O.Account_Id == AId select O;
            if (orders.Count == 0)
            {
                return Json("无订单");
            }
            else
            {
                var ooo = orders.OrderByDescending(o => o.Id).FirstOrDefault();
                int? orderstat = ooo.OrderStatus;
                int? paystate = ooo.PayStatus;
                string message = "没有旧订单";
                if (orderstat == 2)//进行中订单
                {
                    message = "已有进行中订单";
                }
                else if (orderstat == 1)
                {
                    if (paystate == 0)
                    {
                        message = "已有未支付订单";
                    }
                    else
                    {
                        message = "已有进行中订单";
                    }
                }
                var ordernum = ooo.OrderNum;
                var datas = new { msg = message, OrderNum = ordernum };
                return Json(datas);
            }
        }

        public void InitMsg(string orderid, string aid)
        {
            long OrderId = Convert.ToInt64(orderid);
            long AId = Convert.ToInt64(aid);
            var order = entity.Order.SingleOrDefault(o => o.Id == OrderId);
            var account = entity.Account.SingleOrDefault(a => a.Id == AId);
            var Car_Class = from C in entity.Car_Class join O in entity.Order on C.Id equals O.CarClass_Id where O.Id == order.Id select C;
            var carclass_list = Car_Class.ToList();
            var picshop = entity.Shops.SingleOrDefault(s => s.Id == order.PickupShop_Id);
            var retshop = entity.Shops.SingleOrDefault(s => s.Id == order.ReturnShop_Id);
            var pic_country = entity.Provinces.SingleOrDefault(p => p.Id == picshop.Country);
            long cityId = Convert.ToInt64(pic_country.parentno);
            var pic_city = entity.Provinces.SingleOrDefault(p => p.Id == cityId);
            string Carname = carclass_list[0].CarName;
            string CarImg = carclass_list[0].CarPic;
            

            //string carimg = CarImg.ToList()[0].ToString();
            //string url = Request.Url.AbsoluteUri;
            //int index = url.IndexOf("BasicMsg");
            //url = url.Substring(0, index - 1);
            CarImg = CarImg.Replace("\\", "/");
            CarImg = "http://121.40.210.7:8043/" + CarImg;
            //carimg = url + "/" + carimg;


            bmsgmodel.Carname = Carname;
            bmsgmodel.carimg = CarImg;
            bmsgmodel.piccity = pic_city.areaname;
            bmsgmodel.retcity = pic_city.areaname;
            bmsgmodel.picshop = picshop.Name;
            bmsgmodel.retshop = retshop.Name;
            

            //根据取车还车门店,查出对应的取车还车地址,以及取车还车门店的经纬度坐标；

            bmsgmodel.picadress = picshop.Address;//取车地址；
            bmsgmodel.retadress = retshop.Address;//还车地址；
            bmsgmodel.picLongitude = picshop.Longitude;//取车经度；
            bmsgmodel.picLatitude = picshop.Latitude;//取车纬度；
            bmsgmodel.retLongitude = retshop.Longitude;//还车经度；
            bmsgmodel.retLatitude = retshop.Latitude;//还车纬度；

            DateTime picktime = order.StartTime;
            DateTime returntime = order.EndTime;
            bmsgmodel.PickTime = picktime;
            bmsgmodel.RetTime = returntime;

            var time = TimeCount(picktime, returntime);

            ComputeFees cf = new ComputeFees(picktime, returntime, carclass_list[0].Code);

            double costTotal = Math.Round(cf.getActualRentCarFee(), 2);
            double cost_hour = Math.Round(cf.getSinglePrice(), 2);
            double insurance = Math.Round(cf.getInsuranceFee(), 2);

            bmsgmodel.OrderNum = order.OrderNum;
            bmsgmodel.RENTMAN = account.AccountName;
            bmsgmodel.RentDeposit = order.OrderDeposit;

            bmsgmodel.costTotal = costTotal;
            bmsgmodel.cost_hour = cost_hour;
            bmsgmodel.hourcounts = time;


            bmsgmodel.insurance = insurance;//保险
            bmsgmodel.OrderTotalPrice = costTotal + insurance + (double)order.OrderDeposit; //暂定将押金定为1000元（2016/10/11)
        }


        //判断用户是否需要免押金：NeedDeposit;
        public bool NeedDeposit(long AId)
        {
            #region 28天的验证：作废
            ////28天的验证：作废----------------------
            //var PersonalDepositUsage = entity.PersonalDepositUsage.Where(p => p.AccountId == AId).ToList();
            //var singleperde = PersonalDepositUsage.OrderByDescending(o => o.Id).FirstOrDefault();
            ////查出用户最近一次订单的完成时间：
            //var orders = entity.Order.Where(c => c.Account_Id == AId && c.OrderStatus == 3).ToList();
            //var recentordertime = orders.OrderByDescending(e => e.Id).FirstOrDefault().CreatTime;
            ////比较订单完成时间是否超过28天：
            //TimeSpan ts = DateTime.Now.Subtract(recentordertime);

            //int requireddepost = 1;
            //if (singleperde != null)
            //{
            //    requireddepost = singleperde.RequiredDepost;//表示下次租车是否需要交押金，0（不需要），1（需要）
            //}

            //int? score = 0;
            //var uzmxy = entity.UserZMXY.SingleOrDefault(a => a.AccountId == AId);
            //if (uzmxy != null)
            //{
            //    score = uzmxy.Score;
            //    if (score >= 600)
            //    {
            //        return false;//不需要押金
            //    }
            //    else//芝麻信用积分小于600
            //    {
            //        if (ts.Days> 28)//超过28天
            //        {
            //            return true; //需要交押金
            //        }
            //        else
            //        {
            //            if (requireddepost == 0)
            //            {
            //                return false;//不需要押金
            //            }
            //            return true; //需要交押金
            //        }
            //    }
            //}
            //else
            //{
            //    if (ts.Days > 28)//超过28天
            //    {
            //        return true; //需要交押金
            //    }
            //    else
            //    {
            //        if (requireddepost == 0)
            //        {
            //            return false;//不需要押金
            //        }
            //        return true; //需要交押金
            //    }
            //}

            #endregion
            var uzmxy = entity.UserZMXY.SingleOrDefault(a => a.AccountId == AId);
            var userdeposit = entity.UserDeposit.SingleOrDefault(d => d.AccountId == AId);
            if (uzmxy == null || uzmxy.Score < 650)
            {
                if (userdeposit == null || userdeposit.Deposit < 1000)
                {
                    return true;//需要交押金
                }
                else
                {
                    return false;//不需要押金
                }
            }
            else//芝麻信用大于650
            {
                return false;

            }
        }

        //根据用户是否需要交押金，计算出用户需要交的押金的数目：UdepositCount;
        public double UdepositCount(long AId)
        {
            double tempdeposit;
            //查询出用户押金表中的剩余押金数目：
            var udeposit = entity.UserDeposit.SingleOrDefault(u => u.AccountId == AId);
            bool need = NeedDeposit(AId);
            if (need)//用户需要交押金时，判断用户需要交多少押金：
            {
                if (udeposit == null)
                {
                    tempdeposit = 1000;
                }
                else
                {
                    tempdeposit = 1000 - udeposit.Deposit;
                }
            }
            else
            {
                tempdeposit = 0;
            }
            return tempdeposit;
        }

        //获取Account_Id:
        public long GetAId()
        {
            string strid = "";
            object objstrid = Session["IDS"];//获取登录用户的的AccountId；
            try
            {
                strid = objstrid.ToString();
                long AId = Convert.ToInt64(strid);
                return AId;
            }
            catch (Exception)
            {
                return 0;

            }
        }

        public JsonResult GetAllProvinces()
        {
            List<Object> province = new List<Object>();
            var provinceSet = entity.Provinces.Where(d => d.parentno.Equals("0"));
            if (provinceSet != null)
            {
                var items = provinceSet.ToList();
                if (items.Count > 0)
                {
                    foreach (var i in items)
                    {
                        var info = new
                        {
                            id = i.Id.ToString(),//省ID
                            name = i.areaname,//省名
                        };
                        province.Add(info);
                    }
                }
            }
            string result = JsonConvert.SerializeObject(province);
            return Json(new { list = result });
        }

        public JsonResult GetCityByProvince(string id)
        {
            if (id == null)
            {
                return Json(null);
            }
            List<Object> city = new List<Object>();
            string result;
            if (id == "110000" || id == "120000" || id == "500000" || id == "310000")
            {
                long ccid = long.Parse(id);
                var centerCity = entity.Provinces.SingleOrDefault(d => d.Id == ccid);
                if (centerCity != null)
                {
                    var info = new
                    {
                        id = centerCity.Id.ToString(),
                        name = centerCity.areaname,
                    };
                    city.Add(info);
                }
                result = JsonConvert.SerializeObject(city);
                return Json(new { list = result });
            }

            long pid = long.Parse(id);
            var Province = entity.Provinces.SingleOrDefault(d => d.Id == pid);
            if (Province == null)
            {
                return Json(null);
            }
            string parentid = Province.Id.ToString();
            var citySet = entity.Provinces.Where(d => d.parentno.Equals(parentid));
            if (citySet != null)
            {
                var items = citySet.ToList();
                if (items.Count > 0)
                {
                    foreach (var i in items)
                    {
                        var info = new
                        {
                            id = i.Id.ToString(),
                            name = i.areaname,
                        };
                        city.Add(info);
                    }
                }
            }
            result = JsonConvert.SerializeObject(city);
            return Json(new { list = result });
        }

        public JsonResult GetCountry(string id)
        {
            if (id == null)
            {
                return Json(null);
            }
            List<Object> country = new List<Object>();
            long cid = long.Parse(id);
            var city = entity.Provinces.SingleOrDefault(d => d.Id == cid);
            if (city != null)
            {
                string cityid = city.Id.ToString();
                var countrySet = entity.Provinces.Where(d => d.parentno.Equals(cityid));
                if (countrySet != null)
                {
                    var items = countrySet.ToList();
                    if (items.Count > 0)
                    {
                        foreach (var i in items)
                        {
                            var info = new
                            {
                                id = i.Id.ToString(),
                                name = i.areaname,
                            };
                            country.Add(info);
                        }
                    }
                }
            }
            string result = JsonConvert.SerializeObject(country);
            return Json(new { list = result });
        }

        public JsonResult IsPayed(string ordernum)
        {
            var order = entity.Order.SingleOrDefault(o => o.OrderNum == ordernum);//根据订单号查询出唯一的订单；
            if (order.OrderStatus == 1 && order.PayStatus == 0)//预订单，未支付，才让用户去支付；
            {
                return Json("预订单未支付");
            }
            else if (order.OrderStatus == 0 && order.PayStatus == 0)
            {
                return Json("当前订单已过期失效");
            }
            else if (order.OrderStatus == 1 && order.PayStatus == 1)
            {
                return Json("订单已支付");
            }
            else
            {
                return Json("有正在进行中订单");
            }
        }

        public ActionResult BtnAlipay_Click(string ordernum)
        {
            long AId = GetAId();
            logHelper.WriteLog(AId.ToString());
            if (AId == 0)
            {
                //return RedirectToAction("Logon", "Account");
                return Redirect("/Account/Logon");
            }
            Order ooo = entity.Order.SingleOrDefault(O => O.Account_Id == AId && O.OrderStatus == 1);
            ooo.PayWay = 1;//支付方式选择支付宝；

            //根据订单号计算出订单总金额：
            var order = entity.Personal_Order.SingleOrDefault(o => o.OrderNum == ordernum);

            //获取订单可以支付的支付时间：------------------------------
            DateTime createtime = order.CreatTime;
            DateTime timenow = DateTime.Now;
            int timespan = timenow.Subtract(createtime).Minutes;

            if (timespan >= 5)
            {
                return Content("订单超时,已取消");
            }

            var BudgetCost = order.BudgetCost;

           

            double tempdeposit = UdepositCount(AId);

            var TotalPrice = Convert.ToDouble(BudgetCost)  + tempdeposit;//订单总价


            //向充值流水表ChargeSeriel中插入一条数据：
            ChargeSeriel charge = new ChargeSeriel();
            charge.Account_Id = AId;
            charge.AccountClass = 1;
            charge.Money = TotalPrice;
            charge.Classify = 1;
            charge.Type = 2;
            charge.OutTradeNo = "";
            charge.TradeStatus = 2;//交易创建；
            charge.CreateTime = ooo.CreatTime;
            entity.ChargeSeriel.Add(charge);
            entity.SaveChanges();

            var outradeNo = GeneratingUniqueNumber("D", charge.Id.ToString());//传递给支付宝的交易号；
            charge.OutTradeNo = outradeNo;
            entity.SaveChanges();

            string out_trade_no = outradeNo;//订单流水号；
            string total_fee = TotalPrice.ToString();//支付金额；
            //string total_fee = "0.01";
            string subject = "众驾租车";//商品名称
            string body = "及时到账";//商品描述；


            //日志存储路径
            string LogURL = ConfigurationManager.AppSettings["LogURL"].ToString();
            Log.WriteLog("Log", LogURL, "订单名称：" + out_trade_no + "|" + subject + "|" + total_fee + "|可空" + body);
            ////////////////////////////////////////////////////////////////////////////////////////////////

            //把请求参数打包成数组
            SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();


            string minscount = (5 - timespan).ToString() + "m";
            sParaTemp.Add("it_b_pay", minscount);//设置支付宝支付超时时间（订单的生成时间开始的4分钟内有效）

            sParaTemp.Add("service", Config.service);
            sParaTemp.Add("partner", Config.partner);
            sParaTemp.Add("seller_id", Config.seller_id);
            sParaTemp.Add("_input_charset", Config.input_charset.ToLower());
            sParaTemp.Add("payment_type", Config.payment_type);
            sParaTemp.Add("notify_url", Config.notify_url);
            sParaTemp.Add("return_url", Config.return_url);
            sParaTemp.Add("anti_phishing_key", Config.anti_phishing_key);
            sParaTemp.Add("exter_invoke_ip", Config.exter_invoke_ip);
            sParaTemp.Add("out_trade_no", out_trade_no);
            sParaTemp.Add("subject", subject);
            sParaTemp.Add("total_fee", total_fee);
            sParaTemp.Add("body", body);
            //其他业务参数根据在线开发文档，添加参数.文档地址:https://doc.open.alipay.com/doc2/detail.htm?spm=a219a.7629140.0.0.O9yorI&treeId=62&articleId=103740&docType=1
            //如sParaTemp.Add("参数名","参数值");

            //建立请求
            string sHtmlText = Submit.BuildRequest(sParaTemp, "post", "确认");

            return Content(sHtmlText);

        }

        public string GeneratingUniqueNumber(string flag, string number)
        {
            DateTime time = DateTime.Now;
            int len = number.Length;
            string reserved = String.Empty;
            if (len >= 5)
            {
                reserved = number.Substring(len - 5, 5);
            }
            else
            {
                int left = 5 - number.Length;
                reserved = number;
                //reserved = number.PadLeft(left, '0');
                for (int i = 0; i < left; i++)
                {
                    reserved = "0" + reserved;
                }
            }
            Random r = new Random();
            string PayNum = flag + time.ToString("yyyyMMddHHmmss") + reserved;
            //LogHelper.WriteLog("---flag:" + flag + "||number:" + number + "||reserved:" + reserved);
            return PayNum;
        }

        public JsonResult YuEPay(string ordernum)
        {
            var order = entity.Order.SingleOrDefault(o => o.OrderNum == ordernum);
            //余额支付，必须保证用户的押金账户满1000元押金，否则不能支付；
            long AId = GetAId();
            if (AId == 0)
            {
                return Json(false);
            }
            var Udeposit = entity.UserDeposit.SingleOrDefault(u => u.AccountId == AId);
            if (Udeposit == null || Udeposit.Deposit < 1000)
            {
                return Json("押金不足");
            }
            order.PayStatus = 1;
            order.PayWay = 0;//余额支付；
            int result = entity.SaveChanges();
            if (result > 0)
            {
                return Json("支付成功");
            }
            else
            {
                return Json("失败");
            }
        }

        //PersonalDepositUsage表作废
        //public int SetPersonalDepositUsageTab()
        //{
        //    //支付成功之后，设置RequiredDepost不再需要押金；
        //    long AId = GetAId();
        //    //是否需要交押金判定表 PersonalDepositUsage
        //    var persondeposit = entity.PersonalDepositUsage.SingleOrDefault(p => p.AccountId == AId);
        //    if (persondeposit == null)
        //    {
        //        //如果没有就往PersonalDepositUsage表添加记录
        //        PersonalDepositUsage pd = new PersonalDepositUsage();
        //        pd.AccountId = AId;
        //        pd.RequiredDepost = 0;
        //        pd.UpdateTime = DateTime.Now;
        //        entity.PersonalDepositUsage.Add(pd);
        //    }
        //    else
        //    {
        //        //修改记录
        //        persondeposit.RequiredDepost = 0;
        //    }
        //    return entity.SaveChanges();

        //}

        public static string TimeCount(DateTime PickTime, DateTime ReturnTime)
        {

            TimeSpan ts = ReturnTime.Subtract(PickTime);
            int hour = ts.Hours;
            int minute_to_hour = ts.Minutes;
            int day = ts.Days;

            int month = 0, year = 0;
            while (day >= 30)
            {
                month++;
                day -= 30;
            }
            while (month >= 12)
            {
                year++;
                month -= 12;
            }
            var time = year + "年" + month + "月" + day + "天" + hour + "小时" + minute_to_hour + "分钟";
            if (year == 0)
            {
                time = month + "月" + day + "天" + hour + "小时" + minute_to_hour + "分钟";
            }
            if (year == 0 && month == 0)
            {
                time = day + "天" + hour + "小时" + minute_to_hour + "分";
            }
            if (year == 0 && month == 0 && day == 0)
            {
                time = hour + "小时" + minute_to_hour + "分";

            }
            return time;

        }
    }
}
