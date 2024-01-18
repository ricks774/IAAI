using IAAI.Models;
using MVC0917.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace IAAI.Areas.Backend.Controllers
{
    public class LoginController : Controller
    {
        private IAAIDBContent db = new IAAIDBContent();

        [HttpGet]
        public ActionResult Login()
        {
            // 判斷是否已經登入
            // 檢查是否存在身份驗證 Cookie
            // ASP.NET 使用的身份驗證 Cookie 的名稱是 .ASPXAUTH。這是在 FormsAuthentication 中的 FormsCookieName 屬性的默認值
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                return RedirectToAction("Index", "News");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Login(ViewUser viewUser)
        {
            if (ModelState.IsValid)
            {
                string account = viewUser.Account;
                string password = viewUser.Password;

                // 先判斷信箱是否存在，以免發生找不到Salt的錯誤
                bool isExistAccount = db.Users.Any(u => u.Account.Equals(account));
                if (!isExistAccount)
                {
                    //ModelState.AddModelError("Account", "帳號不存在");
                    ViewBag.AlertMessage = "帳號不存在";
                    return View();
                }

                // 取得使用者的資料
                var userData = db.Users.Where(u => u.Account == account).FirstOrDefault();
                int userId = userData.Id;
                string salt = userData.PasswordSalt;
                string hashPassword = userData.Password;
                // 判斷密碼跟密碼鹽是否正確
                var isPasswordCorrect = Utility.VerifyPassword(password, salt, hashPassword);

                if (isPasswordCorrect)
                {
                    // 設定TreeView
                    List<Permissions> permissions = db.Permissions.ToList();
                    StringBuilder sb = new StringBuilder();
                    string[] userPermiss = userData.Permissions.Split(',');
                    var permissIdData = db.Permissions
                            .Where(p => userPermiss.Contains(p.Value))
                            .Select(p => p.ParentId)
                            .Distinct()
                            .ToList();
                    List<Permissions> root = permissions.Where(p => permissIdData.Contains(p.Id)).ToList();   // 找出主要節點

                    GetSideBar(root, sb, userPermiss);
                    string permission = sb.ToString();

                    // 密碼正確，設定身份驗證 Cookie，儲存使用者名稱、Id、跟權限
                    Utility.SetAuthenTicket(userData.Name, userData.Id.ToString(), permission);

                    // 導向到指定的頁面
                    return RedirectToAction("Index", "News");
                }
                else
                {
                    // 密碼不正確，返回原始視圖
                    //ModelState.AddModelError("Password", "密碼不正確");
                    ViewBag.AlertMessage = "密碼錯誤";
                    return View();
                }
            }
            return View();
        }

        [Authorize]
        public ActionResult Logout()
        {
            // 登出相關的邏輯
            FormsAuthentication.SignOut();

            // 重定向到登出後的頁面，清除 ReturnUrl 參數
            return RedirectToAction("Login", "Login", new { area = "Backend" });
        }

        //#region "取得權限樹狀圖"

        //private void GetTree(List<Permissions> permissionsList, StringBuilder sb, string[] userPermiss)
        //{
        //    foreach (Permissions Permission in permissionsList)
        //    {
        //        sb.Append("{ 'id':'" + Permission.Value + "', 'text':'" + Permission.Subject + "'");

        //        // 判斷使用者有哪些子權限
        //        var filteredChildren = Permission.Children.Where(child => userPermiss.Contains(child.Value)).ToList();

        //        if (filteredChildren.Count > 0)
        //        {
        //            sb.Append(",'children': [");
        //            GetTree(filteredChildren, sb, userPermiss);
        //            sb.Append("]");
        //        }

        //        sb.Append("},");
        //    }
        //}

        //#endregion "取得權限樹狀圖"

        #region "取得側邊權限樹狀圖"

        private void GetSideBar(List<Permissions> permissionsList, StringBuilder sb, string[] userPermiss)
        {
            foreach (Permissions Permission in permissionsList)
            {
                sb.Append("<div class=\"sub-navigation\">");
                sb.Append("<a class=\"mdl-navigation__link\">");
                sb.Append("<i class=\"material-icons\">person</i>");
                sb.Append(Permission.Subject);
                sb.Append("<i class=\"material-icons\">keyboard_arrow_down</i>");
                sb.Append("</a>");

                // 判斷使用者有哪些子權限
                var filteredChildren = Permission.Children.Where(child => userPermiss.Contains(child.Value)).ToList();

                if (filteredChildren.Count > 0)
                {
                    sb.Append("<div class=\"mdl-navigation\">");
                    GetSubNode(filteredChildren, sb, userPermiss);
                    sb.Append("</div>");
                }

                sb.Append("</div>");
            }
        }

        #endregion "取得側邊權限樹狀圖"

        #region "取得子節點"

        private void GetSubNode(List<Permissions> permissionsList, StringBuilder sb, string[] userPermiss)
        {
            foreach (Permissions Permission in permissionsList)
            {
                sb.Append("<a class=\"mdl-navigation__link\" href=\"login.html\">");
                sb.Append(Permission.Subject);

                // 判斷使用者有哪些子權限
                var filteredChildren = Permission.Children.Where(child => userPermiss.Contains(child.Value)).ToList();

                if (filteredChildren.Count > 0)
                {
                    GetSubNode(filteredChildren, sb, userPermiss);
                }

                sb.Append("</a>");
            }
        }

        #endregion "取得子節點"

        #region "取得使用者的權限"

        private void GetPermission(int? id)
        {
            var permissionData = db.Users.FirstOrDefault(u => u.Id == id);
            string strpermission = permissionData.Permissions.ToString();
            var arrpermission = strpermission.Split(',');
            StringBuilder getpermission = new StringBuilder();

            for (int i = 0; i < arrpermission.Length; i++)
            {
                getpermission.Append($"'{arrpermission[i]}'");
                getpermission.Append(',');
            }
            ViewBag.permissionData = getpermission.ToString();
        }

        #endregion "取得使用者的權限"
    }
}