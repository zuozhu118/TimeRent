using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using TimeRent.utils;
using YTTimeRentWeb.Models;
using System.Net;
using System.Management;
using System.Data;
using System.Collections;

namespace YTTimeRentWeb.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/
        TimeRentEntities entity = new TimeRentEntities();

        public ActionResult Register()//Register注册页面；
        {
            return View();
        }
        public JsonResult UsernameExist(string username)//username表示用户在用户名文本框中输入的值；
        {
            var user = from U in entity.Account
                       where U.AccountName == username
                       select U;

            if (user.ToList().Count == 0)//如果用户输入的用户名在Account表中不存在；
            {
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }

        //=============================发送短信验证码==============
        public JsonResult Getverification(string phone, string ctime)
        {
            //var ph = from P in entity.User
            //         join S in entity.RegistTextMsg on P.Tel equals S.Tel
            //         where P.Tel == phone & S.Status == 2
            //         select P;
            var ph = from P in entity.RegistTextMsg where P.Tel == phone && P.Status == 2 select P;
            if (ph.ToList().Count != 0)//用户已经注册过
            {
                return Json(false);
            }
            Random r = new Random();
            int seed = r.Next(100000, 1000000);
            PhoneMessage p = new PhoneMessage();
            string[] str = { seed.ToString(), "1" };

            //=====================将短信验证码插入到数据库中：
            #region 将短信验证码插入到数据库中
            //添加手机注册短信验证码RegistTextMsg表中数据：
            RegistTextMsg reg = new RegistTextMsg();
            reg.Veri_Code = seed.ToString();
            reg.Tel = phone;
            reg.RequestTime = DateTime.Parse(ctime);
            reg.Status = 1;
            entity.RegistTextMsg.Add(reg);
            #endregion

            var veri_codeList = from S in entity.RegistTextMsg where (S.Tel == phone & S.Status == 1) select S;//状态 1:可用0:不可用2.已验证
            if (veri_codeList.Count() == 0)//第一次点击获取验证码；
            {
                entity.SaveChanges();
                p.GoMessage(phone, str);//给用户手机发送短信验证码；
                return Json("获取验证码成功");
            }
            else//用户手机上已经收到验证码啦
            {
                //后台限定一分钟内不能重复获取验证码：
                DateTime last_sms_time = (DateTime)veri_codeList.ToList()[0].RequestTime;
                int timeSpan = DateTime.Now.Subtract(last_sms_time).Minutes;

                if (timeSpan < 1)//1分钟内不允许连续发送验证码；
                {
                    //return Json(new { isSuccess = "false", Message = "1分钟之内不能重复申请发送验证码" });
                    return Json("1分钟内不能重复获取");
                }
                else//大于一分钟，继续发送验证码，并将之前的验证码状态改为0；
                {
                    veri_codeList.ToList()[0].Status = 0;

                    entity.SaveChanges();
                    p.GoMessage(phone, str);
                    return Json("获取验证码成功");
                }
            }
        }

        public JsonResult RegisterMsg(string phone, string admin, string pwd, string verification, string ctime)//点击注册按钮
        {
            var fication = entity.RegistTextMsg.SingleOrDefault(s => s.Tel == phone & s.Status == 1);
            if (fication == null)
            {
                return Json(false);
            }
            var code = fication.Veri_Code.ToString();
            if (code == verification)//验证码输入正确
            {

                DateTime last_sms_time = DateTime.Parse(ctime);
                int timeSpan = DateTime.Now.Subtract(last_sms_time).Minutes;
                //判断收到验证码的时间距离当前时间有么有超过5分钟：
                if (timeSpan < 5)//当前收到的验证码5分钟内有效
                {
                    Account acc = new Account();
                    acc.Password = pwd;
                    acc.AccountName = phone;
                    acc.RegisterTime = DateTime.Parse(ctime);
                    acc.Classify = 5;
                    entity.Account.Add(acc);
                    entity.SaveChanges();
                    //添加User表中数据：
                    User user = new User();
                    user.Account_Id = acc.Id;
                    user.Tel = phone;
                    user.Name = phone;
                    user.Status = 2;//账户状态（0:待审核1:通过审核2:未上传3：审核失败）

                    //注册成功要向Balances表中插入数据：
                    Balances ban = new Balances();
                    ban.Account_Id = acc.Id;
                    ban.Balance = 0;
                    ban.UpdateTime = acc.RegisterTime;

                    ////用户押金表 UserDeposit
                    UserDeposit udeposit = new UserDeposit();
                    udeposit.AccountId = acc.Id;
                    udeposit.CreateTime = DateTime.Now;
                    udeposit.Deposit = 0;
                    udeposit.UpdateTime = DateTime.Now;
                    udeposit.UnusableDay = 28;


                    //租车用户类别表CarRentalUsersClassify
                    CarRentalUsersClassify cruc = new CarRentalUsersClassify();
                    cruc.AccountId = acc.Id;
                    cruc.DictionaryDetail_Id = 42;
                    cruc.CreateTime = DateTime.Now;
                    cruc.Status = 1;


                    entity.User.Add(user);
                    entity.Balances.Add(ban);
                    entity.UserDeposit.Add(udeposit);
                    entity.CarRentalUsersClassify.Add(cruc);

                    fication.Account_Id = acc.Id;
                    fication.Status = 2;
                    entity.SaveChanges();
                    return Json(true);
                }
                else
                {
                    fication.Status = 0;
                    entity.SaveChanges();
                    return Json(false);
                }

            }
            else//验证码输入错误
            {
                return Json(false);
            }

        }

        public ActionResult Logon()//登录页面
        {
            string users;
            string pwd;
            string che;

            try
            {
                users = Request.Cookies["accountCookes"].Value; users = Request.Cookies["accountCookes"].Value;
            }
            catch (Exception)
            {
                users = "";
            }

            try
            {
                pwd = Request.Cookies["pwdCookes"].Value;
            }
            catch (Exception)
            {
                pwd = "";
            }
            try
            {
                //che = Session["CHECK"].ToString();
                che = Request.Cookies["CHECK"].Value;
            }
            catch (Exception)
            {

                che = "";
            }
            ViewData.Model = new UserPwd() { user = users, password = pwd, checke=che };
            return View();
        }
        public JsonResult LogonJudge(string accountname, string password, string check)//将用户输入的用户名accountname和密码password传递到后台判断；
        {
            try
            {
                //根据传递过来的用户名accountname判断是否能在数据库中查询出相等的用户名：
                var accountID = entity.Account.SingleOrDefault(a => a.AccountName == accountname).Id;
                //如果accountID存在，就表示用户的用户名输入正确，那么就可以根据用户输入的用户名在数据库中查询出对应的密码，然后和用户输入的密码进行比较；
                var pwd = entity.Account.SingleOrDefault(a => a.AccountName == accountname).Password;
                if (pwd == password)
                {
                    long Ids = Convert.ToInt64(accountID);

                    Response.Cookies["accountCookes"].Value = accountname;//将用户名写入到Cookies中；
                    Response.Cookies["accountCookes"].Expires = DateTime.Now.AddDays(7);//设置Cookies的过期时间7天；
                    if (check == "checked")//如果用户勾选中记住密码
                    {
                        Response.Cookies["pwdCookes"].Value = password;
                        Response.Cookies["pwdCookes"].Expires = DateTime.Now.AddDays(7);

                        //Session["CHECK"] = check;
                        Response.Cookies["CHECK"].Value = check;
                        Response.Cookies["CHECK"].Expires = DateTime.Now.AddDays(7);
                    }
                    else
                    {
                        //清除保存密码的Cookie
                        HttpCookie cookies = new HttpCookie("pwdCookes");
                        cookies.Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies.Add(cookies);

                        //清除勾选的Cookie
                        HttpCookie checkcookies = new HttpCookie("CHECK");
                        checkcookies.Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies.Add(checkcookies);
                    }

                    Session["ACCOUNTNAME"] = accountname;
                    Session["PASSWORD"] = password;
                    
                    Session["IDS"] = Ids;
                    Session.Timeout = 30;//设置Session过期时间；
                    return Json("密码正确");
                }
                else
                {
                    return Json("密码错误");
                }

            }
            catch (Exception)
            {

                return Json("用户名错误");
            }
        }

        public ActionResult Logout()
        {
            //当用户点击‘注销’，就移除用户存储在session中的值；
            Session.Remove("ACCOUNTNAME");
            return Redirect("/Account/Logon");//点击‘注销’跳转到登录页面；
        }
    }
}
