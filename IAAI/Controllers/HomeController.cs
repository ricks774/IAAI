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
            return View(db.Abouts.ToList());
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

        public ActionResult Contact()
        {
            // 獲取用戶輸入的圖形文字
            string userCaptcha = Request.Form["captcha"];

            // 從會話中獲取生成的圖形文字
            string generatedCaptcha = Session["ValidatePictureCode"] as string;

            // 驗證圖形文字是否匹配
            if (userCaptcha == generatedCaptcha)
            {
                // 驗證通過，執行相應的操作
                ViewBag.SuccessMessage = "驗證碼正確";
            }
            else
            {
                // 驗證失敗，顯示錯誤信息或採取其他措施
                ViewBag.SuccessMessage = "驗證碼錯了";
            }

            return View();
        }
    }
}