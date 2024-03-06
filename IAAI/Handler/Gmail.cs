using MimeKit;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace IAAI.Handler
{
    public class Gmail
    {
        // 使用 MimeKit 套件來發信
        public static void SendGmail(string content)
        {
            //宣告使用 MimeMessage
            var message = new MimeMessage();
            //設定發信地址 ("發信人", "發信 email")
            message.From.Add(new MailboxAddress("ricks774", "ricks774@gmail.com"));
            //設定收信地址 ("收信人", "收信 email")
            message.To.Add(new MailboxAddress("ricks774", "ricks774@gmail.com"));
            //寄件副本email
            //message.Cc.Add(new MailboxAddress("收信人名稱", "XXXXXXX@gmail.com"));
            //設定優先權
            //message.Priority = MessagePriority.Normal;

            //信件標題
            message.Subject = "國際縱火調查人員協會臺灣分會";
            //建立 html 郵件格式
            BodyBuilder bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = content;
            //設定郵件內容
            message.Body = bodyBuilder.ToMessageBody(); //轉成郵件內容格式

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                //有開防毒時需設定 false 關閉檢查
                client.CheckCertificateRevocation = false;
                //設定連線 gmail ("smtp Server", Port, SSL加密)
                client.Connect("smtp.gmail.com", 587, false); // localhost 測試使用加密需先關閉
                //  SMTP 伺服器的認證
                var acc = ConfigurationManager.AppSettings["gmailAcc"];
                var auth = ConfigurationManager.AppSettings["gmailAuth"];
                client.Authenticate(acc, auth);
                //發信
                client.Send(message);
                //結束連線
                client.Disconnect(true);
            }
        }
    }
}