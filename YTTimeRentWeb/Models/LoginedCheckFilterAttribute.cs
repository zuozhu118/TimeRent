using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YTTimeRentWeb.Models
{
    //LoginedCheckFilterAttribute过滤器，在Action方法执行之前先校验用户是否已登录；
    public class LoginedCheckFilterAttribute:ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (filterContext.HttpContext.Session["ACCOUNTNAME"] == null)
            {
                filterContext.HttpContext.Response.Redirect("/Account/Logon");
            }
        }

    }
}