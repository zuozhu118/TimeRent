using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using WebDemo.Common;
using YTTimeRentWeb.Models;

namespace YTTimeRentWeb.Controllers
{
    public class PersonalCenterController : BaseController
    {
        TimeRentEntities entity = new TimeRentEntities();
        public ActionResult Index()
        {
            //查询出是当前登录进来的用户并且订单状态不为0的订单总数；
            string strid = Session["IDS"].ToString();//获取登录用户的的账户编号；
            long AId = Convert.ToInt64(strid);
           
            var bal = entity.Balances.SingleOrDefault(b => b.Account_Id == AId);
            var money = bal.Balance;
            Session["MONEY"] = money;//将剩余金额存储在Session中；

            ViewData.Model = PersonalList("0");
            return View();
        }
        public JsonResult ProcessPager()
        {
            string number = Request["Number"] ?? "0";

            object sessiontime = Session["SearchTIME"];
            DateTime[] search_time={};
            if (sessiontime!=null)
            {
                search_time = sessiontime as DateTime[];
            }

            PersonalPagerModel pagemodel = PersonalList(number, search_time);
            return Json(pagemodel);
        }

        /// <summary>
        /// PersonalList方法查询出当前页的数据，返回所有的分页链接标签和PersonalCenterModel集合
        /// </summary>
        /// <param name="number">number的值代表要查询的不同状态的订单："0"表示所有的订单，"1"表示‘进行中’，"2"表示用户点击的是‘已完成’，"3"表示‘已取消’，"4"表示预订单</param>
        /// <returns></returns>
        public PersonalPagerModel PersonalList(string number, params DateTime[] search_time)
        {
            List<PersonalCenterModel> personallist = new List<PersonalCenterModel>();
            string strid = Session["IDS"].ToString();//获取登录用户的的账户编号；
            long AId = Convert.ToInt64(strid);
            string strIndex = HttpContext.Request["pageIndex"] ?? "1";
            int pageindex = Convert.ToInt32(strIndex);
            string ssPagesize = HttpContext.Request["pageSize"] ?? "3";
            int pagesize = Convert.ToInt32(ssPagesize);
            //string number = HttpContext.Request.QueryString["Number"] ?? "0";
            //int num = Convert.ToInt32(numbers);
            int orderstatu;
            if (number == "1")//number=="1"表示用户点击的是‘进行中’；
            {
                orderstatu = 2;
            }
            else if (number == "2")//number=="2"表示用户点击的是‘已完成’；
            {
                orderstatu = 3;
            }
            else if (number == "3")//‘已取消’
            {
                orderstatu = 0;
            }
            else if (number == "4")//预订单
            {
                orderstatu = 1;
            }
            else///所有订单
            {
                orderstatu = -1;
            }

            //查询出当前页的数据；order_all_pager；
            var order_all = entity.Personal_Order.Where(s => s.Account_Id == AId);
            if (search_time.Length>0)
            {
                DateTime pictime=search_time[0];
                DateTime rettime=search_time[1];
                order_all = order_all.Where(t =>t.CreatTime >= pictime && t.CreatTime <= rettime);
            }
            if (orderstatu != -1)//表示传入了订单状态这个参数来查询：
            {
                order_all = order_all.Where(t => t.OrderStatus == orderstatu);
            }

            List<Personal_Order> order_all_pager = order_all.OrderByDescending(o => o.Id).Skip(pagesize * (pageindex - 1)).Take(pagesize).AsEnumerable().ToList();
            foreach (var t in order_all_pager)
            {
                PersonalCenterModel per = AddProperty(t);//给per的属性赋值
                personallist.Add(per);
            }
            #region
            ////var picparentno = from O in entity.Order.Where(d => d.Account_Id == AId) join S in entity.Shops on O.PickupShop_Id equals S.Id join P in entity.Provinces on S.Country equals P.Id select P.parentno;
            ////var picparentnolist = picparentno.ToList();
            ////var retparentno = from O in entity.Order.Where(d => d.Account_Id == AId) join S in entity.Shops on O.ReturnShop_Id equals S.Id join P in entity.Provinces on S.Country equals P.Id select P.parentno;
            ////var retparentnolist = retparentno.ToList();
            #endregion

            int total = order_all.Count();//总记录条数；
            Session["Numbers"] = number;
            Session["ORDERCOUNT"] = total;//将查询出的订单总数存储在Session中；
            //根据用户传入的一页多少条，当前第几页，总共的数据条数，生成所有的分页超链接标签；
            string alink = "";
            if (total != 0)
            {
                alink = LaomaPager.ShowPageNavigate(pagesize, pageindex, total, number);
            }
            var datas = new PersonalPagerModel() { Personallist = personallist, Pagerlink = alink };
            return datas;
        }

        public PersonalCenterModel AddProperty(Personal_Order t)
        {
            PersonalCenterModel per = new PersonalCenterModel();
            per.PICCity = t.PickupShopCity;
            per.RETCity = t.ReturnShopCity;
            per.PickupShop_Name = t.PickupShopName;
            per.ReturnShop_Name = t.ReturnShopName;
            per.PicAddress = t.PickupShopAddress;
            per.RetAddress = t.ReturnShopAddress;
            per.OrderNum = t.OrderNum;
            per.StartTime = t.StartTime.ToString("yyyy-MM-dd HH:mm:ss");
            per.EndTime = t.EndTime.ToString("yyyy-MM-dd HH:mm:ss");
            per.CarName = t.CarName;
            per.OrderDeposit = t.OrderDeposit;
            per.OrderStatusint = t.OrderStatus;
            per.PayStatus = t.PayStatus;//订单支付状态；
            per.CreatTime = t.CreatTime;

            per.carimg = "http://121.40.210.7:8043/" + t.CarPic;
            //string url = Request.Url.AbsoluteUri;
            //int index = url.IndexOf("PersonalCenter");
            //url = url.Substring(0, index - 1);
            //per.carimg = url + "/" + t.CarPic;

            per.AccountName = t.Name;
            per.Rentcounttime = BasicMsgController.TimeCount(t.StartTime, t.EndTime);

            per.BudgetCost = t.BudgetCost;
            per.RentDeposit = t.OrderDeposit;

            per.TotalPrice = (Convert.ToDouble(per.BudgetCost) + per.RentDeposit).ToString();//订单总价
            switch (t.OrderStatus)
            {
                case 0: per.OrderStatus = "已取消";
                    break;
                case 1:
                    if (per.PayStatus == 0)
                    {
                        per.OrderStatus = "待支付";
                        break;
                    }
                    else
                    {
                        per.OrderStatus = "已支付";
                        break;
                    }
                case 2: per.OrderStatus = "进行中";
                    break;
                case 3: per.OrderStatus = "已完成";
                    break;
            }
            return per;
        }

        /// <summary>
        /// 封装一个分页方法Pager
        /// </summary>
        /// <param name="personallist">查询到的所有记录条数</param>
        /// <param name="pageindex">要查询的当前页的索引</param>
        /// <param name="pagesizes">一页多少条数据</param>
        /// <returns></returns>
        //public PersonalPagerModel Pager(List<PersonalCenterModel> personallistSS, string numbers)  //List<PersonalCenterModel> personallistSS, int pageindex, int pagesizes, string number
        //{
        //    //===============分页===================
        //    //string strIndex = HttpContext.Request["pageIndex"] ?? "1";
        //    //int pageindex = Convert.ToInt32(strIndex);
        //    //string ssPagesize = HttpContext.Request["pageSize"] ?? "3";
        //    //int pagesize = Convert.ToInt32(ssPagesize);
        //    //string numbers = HttpContext.Request.QueryString["Number"] ?? "0";
        //    //int num = Convert.ToInt32(numbers);

        //    ////查询出当前页的数据；pagerdata；
        //    //var pagerdata = personallistSS.OrderBy(u => u.Id).Skip(pagesize * (pageindex - 1)).Take(pagesize).AsEnumerable().ToList();
        //    //int total = personallistSS.Count;//总记录条数；
        //    ////根据用户传入的一页多少条，当前第几页，总共的数据条数，生成所有的分页超链接标签；
        //    //string alink = "";
        //    //if (total != 0)
        //    //{
        //    //    alink = LaomaPager.ShowPageNavigate(pagesize, pageindex, total, numbers);
        //    //}
        //    //var datas = new PersonalPagerModel() { Personallist = pagerdata, Pagerlink = alink };
        //    //return datas;
        //}

        public JsonResult OrderStatus(string number)
        {
            PersonalPagerModel pagemodel = PersonalList(number);
            return Json(pagemodel);
        }

        public JsonResult SelectOrder(string pictime, string rtime)
        {
            DateTime Ptime = DateTime.Parse(pictime);
            DateTime Rtime = DateTime.Parse(rtime).AddDays(1);
            DateTime[] search_time={Ptime,Rtime};
            string number = Session["Numbers"].ToString();
            Session["SearchTIME"] = search_time;
            PersonalPagerModel pagemodel = PersonalList(number, search_time);
            return Json(pagemodel);
        }


        //====================订单详情页=========================
        public ActionResult PersonalDetial(string ordernum)
        {
            //根据订单号，查询出这条订单的详情信息：
            var personal_order = entity.Personal_Order.SingleOrDefault(o => o.OrderNum == ordernum);
            PersonalCenterModel per = AddProperty(personal_order);
            ViewData.Model = per;
            return View();
        }

        //==================图片上传================================
        //ImgUpLoad只做图片上传功能，不判定上传状态；
        public void ImgUpLoad()
        {
            var hidden = Request["card"];
            string url = Request.Url.AbsoluteUri;
            int index = url.IndexOf("PersonalCenter");
            url = url.Substring(0, index - 1);//获取http://121.40.210.7:8023/的虚拟路径；

            //将用户上传的图片路径写到User表：
            string strid = Session["IDS"].ToString();//获取登录用户的的账户编号；
            long AId = Convert.ToInt64(strid);
            var user = entity.User.SingleOrDefault(u => u.Account_Id == AId);

            if (hidden == "IDcard")//则提交过来的是身份证的图片
            {
                HttpPostedFileBase file1 = HttpContext.Request.Files["fileUp1"];
                //string path1 = @"D:\img\IDCardFront\";
                string path1 = ConfigurationManager.AppSettings["IDFilePath"] + user.Id + @"\";//读取config配置文件中存放身份证的根路径

                if (!Directory.Exists(path1))//如果当前没有文件夹，就创建
                {
                    Directory.CreateDirectory(path1);//创建文件夹

                }
                string imageUrl1 = path1 + Path.GetFileName(file1.FileName);
                file1.SaveAs(imageUrl1);
                int Urlindex = imageUrl1.IndexOf(@"\");
                string xdurl1 = imageUrl1.Substring(Urlindex + 1);//截取相对路径
                user.ID_CardFront = xdurl1;//修改User表身份证路径；
                user.IDPicStatus = 2;//身份证照片审核中；
                entity.SaveChanges();

                xdurl1 = xdurl1.Replace("\\", "/");
                imageUrl1 = url + "/" + xdurl1;

                var imgurl = imageUrl1 + "0";
                Response.Write("<script>window.parent.uploadSuccess('" + imgurl + "')</script>");//在路径尾部加0，区分是身份证路径还是驾驶证路径；
            }
            else if (hidden == "Driver")
            {
                HttpPostedFileBase file2 = HttpContext.Request.Files["fileUp2"];
                //驾驶证上传到的文件夹
                string path2 = ConfigurationManager.AppSettings["DriverFilePath"] + user.Id + @"\";

                if (!Directory.Exists(path2))
                {
                    Directory.CreateDirectory(path2);//创建文件夹

                }
                string imageUrl2 = path2 + Path.GetFileName(file2.FileName);
                file2.SaveAs(imageUrl2);//将图片保存到imageUrl2路径下；
                int Urlindex = imageUrl2.IndexOf(@"\");
                string xdurl2 = imageUrl2.Substring(Urlindex + 1);//截取相对路径

                user.Driver_License = xdurl2;//修改User表驾驶证路径；
                user.DriverPicStatus = 2;
                entity.SaveChanges();


                xdurl2 = xdurl2.Replace("\\", "/");
                imageUrl2 = url + "/" + xdurl2;//本地或者服务器的虚拟路径；

                var imgur2 = imageUrl2 + "1";

                Response.Write("<script>window.parent.uploadSuccess('" + imgur2 + "')</script>");
            }
            else
            {
                HttpPostedFileBase file3 = HttpContext.Request.Files["fileUp3"];
                //驾驶证上传到的文件夹
                string path3 = ConfigurationManager.AppSettings["HeadFilePath"] + user.Id + @"\";
                if (!Directory.Exists(path3))
                {
                    Directory.CreateDirectory(path3);//创建文件夹

                }
                string imageUrl3 = path3 + Path.GetFileName(file3.FileName);
                file3.SaveAs(imageUrl3);
                int Urlindex = imageUrl3.IndexOf(@"\");
                string xdurl3 = imageUrl3.Substring(Urlindex + 1);

                user.PortraitPic = xdurl3;
                user.PortraitStatus = 2;
                entity.SaveChanges();


                xdurl3 = xdurl3.Replace("\\", "/");
                imageUrl3 = url + "/" + xdurl3;//本地或者服务器的虚拟路径；

                imageUrl3 = imageUrl3 + "2";

                Response.Write("<script>window.parent.uploadSuccess('" + imageUrl3 + "')</script>");

            }
            SetUserStatus(user);
        }

        public JsonResult CardPicStatus()
        {
            string strid = Session["IDS"].ToString();//获取登录用户的的账户编号；
            long AId = Convert.ToInt64(strid);
            User users = entity.User.SingleOrDefault(u => u.Account_Id == AId);
            string IDcardmsg = "";
            string Drivermsg = "";
            string PortraitStatus = "";
            switch (users.IDPicStatus)//IDPicStatus身份证照片是否属实（0:不属实，1：属实，2：已上传未审核）
            {

                case null:
                    IDcardmsg = "2";   // IDcardmsg = "请上传身份证";
                    break;
                case 0: IDcardmsg = "3";// IDcardmsg = "身份证审核失败";
                    break;
                case 1: IDcardmsg = "1";//IDcardmsg = "身份证审核成功";
                    break;
                case 2: IDcardmsg = "0";//IDcardmsg = "身份证正在审核";
                    break;
            }

            switch (users.DriverPicStatus)
            {
                case null:
                    Drivermsg = "2";
                    break;
                case 0: Drivermsg = "3";
                    break;
                case 1: Drivermsg = "1";
                    break;
                case 2: Drivermsg = "0";
                    break;
            }

            switch (users.PortraitStatus)
            {
                case null:
                    PortraitStatus = "2";
                    break;
                case 0: PortraitStatus = "3";
                    break;
                case 1: PortraitStatus = "1";
                    break;
                case 2: PortraitStatus = "0";
                    break;
            }
            var Message = new { IDCardMsg = IDcardmsg, DriverCardMsg = Drivermsg, PorCardMsg = PortraitStatus };
            return Json(Message);
        }

        public void SetUserStatus(User users)
        {
            //设置Status的值：Status:账户状态（0:待审核1:通过审核2:未上传 3：审核失败）
            //IDPicStatus身份证照片是否属实（0:不属实，1：属实，2：已上传未审核）
            if (users.IDPicStatus == null || users.DriverPicStatus == null || users.PortraitStatus == null)
            {
                users.Status = 2;//未上传；
            }
            else if (users.IDPicStatus == 0 || users.DriverPicStatus == 0 || users.PortraitStatus == 0)
            {
                users.Status = 3;////审核失败;
            }
            else if (users.IDPicStatus == 1 && users.DriverPicStatus == 1 && users.PortraitStatus == 1)
            {
                users.Status = 1;//通过审核；
            }
            else
            {
                users.Status = 0;//待审核；
            }

            //if (users.IDPicStatus == 2)
            //{
            //    switch (users.DriverPicStatus)
            //    {
            //        case null:
            //            users.Status = 2;//未上传；
            //            break;
            //        case 0: users.Status = 3;//审核失败
            //            break;
            //        case 1: users.Status = 0;//待审核
            //            break;
            //        case 2: users.Status = 0;
            //            break;
            //    }

            //}
            //else if (users.DriverPicStatus == 2)
            //{
            //    switch (users.IDPicStatus)
            //    {
            //        case null:
            //            users.Status = 2;//未上传；
            //            break;
            //        case 0: users.Status = 3;//审核失败
            //            break;
            //        case 1: users.Status = 0;//待审核
            //            break;
            //        case 2: users.Status = 0;
            //            break;
            //    }
            //}

            entity.SaveChanges();

        }

        //--------------------------------------------以下保留，暂时不用-----------------
        public JsonResult VipManage()
        {
            string strid = Session["IDS"].ToString();//获取登录用户的的账户编号；
            long AId = Convert.ToInt64(strid);
            YTTimeRentWeb.Models.User usermodel = new User();
            var usertbl = entity.User.SingleOrDefault(a => a.Account_Id == AId);
            usermodel.Name = usertbl.Name;
            usermodel.ID_CardNum = usertbl.ID_CardNum;
            //usermodel.Driver_Experience = usertbl.Driver_Experience;
            usermodel.Tel = usertbl.Tel;
            usermodel.Email = usertbl.Email;
            return Json(usermodel);
        }
        public JsonResult VipModify(string name, string card, string driver, string ph, string email)
        {
            string strid = Session["IDS"].ToString();//获取登录用户的的账户编号；
            long AId = Convert.ToInt64(strid);
            long UId = entity.User.AsNoTracking().SingleOrDefault(u => u.Account_Id == AId).Id;
            YTTimeRentWeb.Models.User users = new User();
            users.Id = UId;//只修改用户名ID是Account_Id的这个用户信息；
            users.Account_Id = AId;
            users.Name = name;
            #region 验证电话邮箱身份证
            //string phregex = @"^(13|14|15|16|18|19)\d{9}$";//验证是否合法的电话号码；
            //string emailregex = @"\w+@\w+(\.\w+)+";//验证合法邮箱；
            //string cardregex = "^([1-9][0-9]{14}|[1-9][0-9]{16}[0-9xX])$";//验证身份证；
            //bool phistrue= Regex.IsMatch(ph,phregex);
            //bool emistrue = Regex.IsMatch(email, emailregex);
            //bool cardistrue = Regex.IsMatch(card, cardregex);
            #endregion
            users.Tel = ph;
            users.Email = email;
            users.ID_CardNum = card;
            //======判断用户在‘驾龄’文本框中输入是否为空：
            driver = string.IsNullOrEmpty(driver) ? "0" : driver;
            entity.Entry(users).State = EntityState.Modified;//修改；
            int n = entity.SaveChanges();
            if (n > 0)
            {
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }
        public JsonResult ValidateOldPwd(string oldpwd)
        {
            string strid = Session["IDS"].ToString();//获取登录用户的的账户编号；
            long AId = Convert.ToInt64(strid);
            //根据登录用户的Account_Id查询出用户的登录密码：
            var Apwd = entity.Account.SingleOrDefault(a => a.Id == AId).Password;
            if (oldpwd == Apwd)
            {
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }
        public JsonResult ModifyPWD(string newpwd)
        {
            string strid = Session["IDS"].ToString();//获取登录用户的的账户编号；
            long AId = Convert.ToInt64(strid);
            Account acc = new Account();
            //Account表中Id，AccountName，Password，RegisterTime这些字段不允许为空，所以必须要全部赋值；
            acc.Id = AId;
            acc.Password = newpwd;
            acc.AccountName = entity.Account.AsNoTracking().SingleOrDefault(a => a.Id == AId).AccountName;
            acc.RegisterTime = entity.Account.AsNoTracking().SingleOrDefault(a => a.Id == AId).RegisterTime;
            entity.Entry(acc).State = EntityState.Modified;

            int n = entity.SaveChanges();
            if (n > 0)
            {
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }
    }
}
