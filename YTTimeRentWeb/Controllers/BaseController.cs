using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YTTimeRentWeb.Controllers
{
    //BaseController是自定义的一个校验用户是否已经登录的基类;
    public class BaseController : Controller 
    {
        //控制器继承自IActionFilter类，本身有过滤器功能；
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            //如果用户未登陆，就不允许用户进入已登录的页面，而是先跳转到登录页面就行登录;
            if (filterContext.HttpContext.Session["ACCOUNTNAME"] == null)
            {
                filterContext.HttpContext.Response.Redirect("/Account/Logon");//跳转到登录页面;
            }
           
        }
    }
}