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
    }
}