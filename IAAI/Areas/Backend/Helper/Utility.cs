using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
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
    }
}