using IAAI.Models;
using MVC0917.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
                string salt = userData.PasswordSalt;
                string hashPassword = userData.Password;
                // 判斷密碼跟密碼鹽是否正確
                var isPasswordCorrect = Utility.VerifyPassword(password, salt, hashPassword);

                if (isPasswordCorrect)
                {
                    // 密碼正確，設定身份驗證 Cookie
                    Utility.SetAuthenTicket(userData.Account, userData.Id.ToString());

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
    }
}