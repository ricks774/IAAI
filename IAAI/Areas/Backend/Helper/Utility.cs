﻿using IAAI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MVC0917.Models
{
    public class Utility
    {
        #region "密碼加密"

        public const int DefaultSaltSize = 5;

        /// <summary>
        /// 產生Salt
        /// </summary>
        /// <returns>Salt</returns>
        public static string CreateSalt()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buffer = new byte[DefaultSaltSize];
            rng.GetBytes(buffer);
            return Convert.ToBase64String(buffer);
        }

        public static string GenerateHashWithSalt(string password, string salt)
        {
            // merge password and salt together
            string sHashWithSalt = password + salt;
            // convert this merged value to a byte array
            byte[] saltedHashBytes = Encoding.UTF8.GetBytes(sHashWithSalt);
            // use hash algorithm to compute the hash
            HashAlgorithm algorithm = new SHA256Managed();
            // convert merged bytes to a hash as byte array
            byte[] hash = algorithm.ComputeHash(saltedHashBytes);
            // return the has as a base 64 encoded string
            return Convert.ToBase64String(hash);
        }

        #endregion "密碼加密"

        #region "將使用者資料寫入cookie,產生AuthenTicket"

        /// <summary>
        /// 將使用者資料寫入cookie,產生AuthenTicket
        /// </summary>
        /// <param name="userData">使用者資料</param>
        /// <param name="userId">UserAccount</param>
        ///

        public static void SetAuthenTicket(string userData, string userId)
        {
            //宣告一個驗證票
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, userId, DateTime.Now, DateTime.Now.AddHours(3), false, userData);
            //加密驗證票
            string encryptedTicket = FormsAuthentication.Encrypt(ticket);
            //建立Cookie
            HttpCookie authenticationcookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            //將Cookie寫入回應

            HttpContext.Current.Response.Cookies.Add(authenticationcookie);
        }

        //public static void SetAuthenTicket(string userData, string userId, string customData)
        //{
        //    // 將自定義資料附加到 userData 中
        //    string userDataWithCustomData = $"{userData}|{customData}";

        //    //宣告一個驗證票
        //    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, userId, DateTime.Now, DateTime.Now.AddHours(3), false, userDataWithCustomData);
        //    //加密驗證票
        //    string encryptedTicket = FormsAuthentication.Encrypt(ticket);
        //    //建立Cookie
        //    HttpCookie authenticationcookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
        //    //將Cookie寫入回應
        //    HttpContext.Current.Response.Cookies.Add(authenticationcookie);
        //}

        #endregion "將使用者資料寫入cookie,產生AuthenTicket"

        #region "不透過Argon2加密密碼"

        public static (string salt, string hashPassword) PasswordHash(string password)
        {
            // 將密碼使用 SHA256 雜湊運算(不可逆)
            //string salt = email.Substring(0, 1).ToLower(); //使用帳號前一碼當作密碼鹽

            // 生成隨機的密碼鹽
            byte[] saltBytes = new byte[16];
            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(saltBytes);
            }

            string salt = Convert.ToBase64String(saltBytes);

            SHA256 sha256 = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(salt + password); //將密碼鹽及原密碼組合
            byte[] hash = sha256.ComputeHash(bytes);
            StringBuilder newPassword = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                newPassword.Append(hash[i].ToString("X2"));
            }
            string hashPassword = newPassword.ToString(); // 雜湊運算後密碼
            return (salt, hashPassword);
        }

        #endregion "不透過Argon2加密密碼"

        #region "驗證密碼"

        public static bool VerifyPassword(string userInputPassword, string storedSalt, string storedHashPassword)
        {
            // 將輸入密碼和儲存的鹽組合
            //byte[] saltBytes = Convert.FromBase64String(storedSalt);  // 用途不明?
            byte[] inputBytes = Encoding.UTF8.GetBytes(storedSalt + userInputPassword);

            // 使用 SHA-256 哈希函數對組合的密碼進行雜湊
            SHA256 sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(inputBytes);
            StringBuilder newPassword = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                newPassword.Append(hash[i].ToString("X2"));
            }
            string hashedInputPassword = newPassword.ToString();

            // 比較雜湊後的密碼是否與儲存的密碼相符
            return hashedInputPassword == storedHashPassword;
        }

        #endregion "驗證密碼"

        #region"取得票證資料"

        public static (string userId, string userName) GetAuthenData(System.Security.Principal.IIdentity identity)
        {
            string userId = identity.Name; // 會取得使用者Id
            string userName = ((FormsIdentity)identity).Ticket.UserData;   // 取得使用者名稱
            return (userId, userName);
        }

        //public static (string userId, string userName, string permissions) GetAuthenData(System.Security.Principal.IIdentity identity)
        //{
        //    string userId = identity.Name; // 取得使用者Id
        //    string userData = ((FormsIdentity)identity).Ticket.UserData;   // 取得使用者資料

        //    // 解析資料，使用者資料格式為 "userName | permissions"
        //    string[] userDataParts = userData.Split('|');

        //    string userName = userDataParts.Length > 0 ? userDataParts[0] : string.Empty;
        //    string permissions = userDataParts.Length > 1 ? userDataParts[1] : string.Empty;

        //    return (userId, userName, permissions);
        //}

        #endregion

        #region "取得 SideBar"

        public static string GetSideBar(int userId)
        {
            IAAIDBContent db = new IAAIDBContent();

            // 設定TreeView
            var userData = db.Users.Where(u => u.Id == userId).FirstOrDefault();
            List<Permissions> permissions = db.Permissions.ToList();
            StringBuilder sb = new StringBuilder();
            string[] userPermiss = userData.Permissions.Split(',');
            var permissIdData = db.Permissions
                    .Where(p => userPermiss.Contains(p.Value))
                    .Select(p => p.ParentId)
                    .Distinct()
                    .ToList();
            List<Permissions> root = permissions.Where(p => permissIdData.Contains(p.Id)).ToList();   // 找出主要節點

            GetTree(root, sb, userPermiss);
            string permission = sb.ToString();
            return permission;
        }

        #region "取得側邊權限樹狀圖"

        private static void GetTree(List<Permissions> permissionsList, StringBuilder sb, string[] userPermiss)
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

        private static void GetSubNode(List<Permissions> permissionsList, StringBuilder sb, string[] userPermiss)
        {
            foreach (Permissions Permission in permissionsList)
            {
                sb.Append($"<a class=\"mdl-navigation__link\" href=\"{Permission.Page}\">");
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

        #endregion
    }
}