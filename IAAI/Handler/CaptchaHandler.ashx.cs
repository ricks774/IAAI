using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace IAAI.Handler
{
    /// <summary>
    /// CaptchaHandler 的摘要描述
    /// </summary>
    public class CaptchaHandler : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            int DigitCount = 6;
            string str_rnd_code = GetRandomCode(DigitCount);
            context.Session["ValidatePictureCode"] = str_rnd_code;

            System.Drawing.Image image = CreateCaptchaImage(str_rnd_code);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

            context.Response.Clear();
            context.Response.ContentType = "image/jpeg";
            context.Response.BinaryWrite(ms.ToArray());
            ms.Close();
        }

        private string GetRandomCode(int digit_count)
        {
            string str_code_result = "";
            Random rand = new Random(Guid.NewGuid().GetHashCode());

            List<string> lsChrSet = new List<string>();
            List<string> lsNeedAvoid = new List<string>(new[] { "O", "o", "l", "I" });//保留數字0、1排除容易混淆的大寫O小寫o跟大寫I小寫L
            for (int i = 0; i <= 35; i++)
            {
                if (i < 10)
                {
                    int ascii_code = 48 + i;//0~9 : 48 ~ 57
                    string chr_digit_result = ((char)(ascii_code)).ToString();
                    //Console.WriteLine(string.Format("Ascii code (digit):{0}->{1}", ascii_code, chr_digit_result));
                    lsChrSet.Add(chr_digit_result);
                }
                else
                {
                    int new_idx = i - 10;
                    int ascii_code = 97 + new_idx;//a~z : 97 ~ 122
                    string chr_result_lowercase = ((char)(ascii_code)).ToString();
                    //Console.WriteLine(string.Format("Ascii code (lowercase):{0}->{1}", ascii_code, chr_result_lowercase));
                    if (!lsNeedAvoid.Contains(chr_result_lowercase))
                        lsChrSet.Add(chr_result_lowercase);
                    string chr_result_uppercase = ((char)(ascii_code - 32)).ToString();//A~Z : 65 ~ 90
                    if (!lsNeedAvoid.Contains(chr_result_uppercase))
                        lsChrSet.Add(chr_result_uppercase);
                }
            }

            for (int idx_digit = 0; idx_digit < digit_count; idx_digit++)
            {
                int rndIdx = rand.Next(lsChrSet.Count);
                str_code_result += lsChrSet[rndIdx];
            }
            return str_code_result;
        }

        private System.Drawing.Image CreateCaptchaImage(string str_rnd_code)
        {
            System.Drawing.Bitmap image = new System.Drawing.Bitmap((str_rnd_code.Length * 14), 28);
            System.Drawing.Graphics g = Graphics.FromImage(image);
            Random random = new Random(Guid.NewGuid().GetHashCode());
            int int_Red = 255;
            int int_Green = 255;
            int int_Blue = 255;
            int int_bkack = random.Next(240, 255);
            g.Clear(Color.FromArgb(int_Red, int_Green, int_Blue));
            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
            Brush brushBack = new LinearGradientBrush(rect, Color.FromArgb(int_bkack, int_bkack, int_bkack), Color.FromArgb(255, 255, 255), 255);// 新增黑白漸層
            g.FillRectangle(brushBack, rect);
            for (int i = 0; i < str_rnd_code.Length; i++)
            {
                Color wc = Color.FromArgb(0, 0, 0);
                int y = random.Next(0, 6);
                Font font = new System.Drawing.Font("Tahoma", 12 + y, System.Drawing.FontStyle.Italic);
                System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), wc, wc, 1.2F, true);
                g.DrawString(str_rnd_code.Substring(i, 1), font, brush, 4 + i * 11, 2 + random.Next(0, 6 - y));
            }
            for (int i = 0; i <= 300; i++)//image noise
            {
                int x = random.Next(image.Width);
                int y = random.Next(image.Height);
                image.SetPixel(x, y, Color.FromArgb(random.Next()));
            }

            return image;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}