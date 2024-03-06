using IAAI.Handler;
using IAAI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IAAI.Controllers
{
    public class HomeController : Controller
    {
        private IAAIDBContent db = new IAAIDBContent();

        public ActionResult Index()
        {
            // 從資料庫中取得前四筆資料
            var newsList = db.News.Take(4).ToList();

            return View(newsList);
        }

        public ActionResult About()
        {
            return View(db.Abouts.ToList());
        }

        public ActionResult Organization()
        {
            return View(db.Abouts.ToList());
        }

        public ActionResult History()
        {
            return View(db.Abouts.ToList());
        }

        public ActionResult Certified()
        {
            return View(db.CertifiedMembers.ToList());
        }

        public ActionResult Expert()
        {
            return View(db.Abouts.ToList());
        }

        public ActionResult News()
        {
            return View(db.News.ToList());
        }

        public ActionResult NewsDetail(int id)
        {
            var news = db.News.FirstOrDefault(n => n.Id == id);
            return View(news);
        }

        public ActionResult Scope()
        {
            return View(db.Businesses.ToList());
        }

        public ActionResult Accreditation()
        {
            return View(db.Businesses.ToList());
        }

        public ActionResult Consult()
        {
            return View(db.Businesses.ToList());
        }

        public ActionResult Investigation()
        {
            return View(db.Businesses.ToList());
        }

        public ActionResult Knowledge()
        {
            return View(db.Knowledges.ToList());
        }

        public ActionResult KnowledgeDetail(int id)
        {
            var news = db.Knowledges.FirstOrDefault(n => n.Id == id);
            return View(news);
        }

        public ActionResult Calendar()
        {
            return View();
        }

        public ActionResult Contact(string captcha)
        {
            // 從會話中獲取生成的圖形文字
            string generatedCaptcha = Session["ValidatePictureCode"] as string;

            if (captcha != null)
            {
                captcha = captcha.ToLower();
            }

            // 驗證圖形文字是否匹配
            if (captcha != generatedCaptcha)
            {
                ViewBag.Message = "驗證碼錯誤";
            }
            else
            {
                string context = "<h3>感謝您的來信，我們會盡快與您聯絡</h3>";
                Gmail.SendGmail(context);
                ViewBag.Message = "送出成功";
            }
            return View();
        }
    }
}