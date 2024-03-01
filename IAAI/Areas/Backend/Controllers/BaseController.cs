using MVC0917.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IAAI.Areas.Backend.Controllers
{
    public class BaseController : Controller
    {
        // OnActionExecuting 方法是一個適合於處理共用邏輯的地方，因為它可以在所有動作方法執行之前被呼叫，從而確保這些共用的邏輯在整個控制器中都得到應用
        // 載入左側側邊欄選單
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userInfo = Utility.GetAuthenData(User.Identity);
                ViewBag.UserName = userInfo.userName;

                // 取得 SideBar 的 TreeView
                int userId = int.Parse(userInfo.userId);
                string sideBar = Utility.GetSideBar(userId);
                ViewBag.SideBar = sideBar;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}