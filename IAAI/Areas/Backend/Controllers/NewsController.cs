using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using IAAI.Models;
using MVC0917.Models;

namespace IAAI.Areas.Backend.Controllers
{
    public class NewsController : Controller
    {
        private IAAIDBContent db = new IAAIDBContent();

        // GET: Backend/News
        [Authorize]
        public ActionResult Index()
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

            return View(db.News.ToList());
        }

        // GET: Backend/News/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        // GET: Backend/News/Create
        public ActionResult Create()
        {
            News news = new News(); // 初始化模型對象
            return View(news);
        }

        // POST: Backend/News/Create
        // 若要免於大量指派 (overposting) 攻擊，請啟用您要繫結的特定屬性，
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Content,PubDate,CreateDate")] News news)
        {
            if (ModelState.IsValid)
            {
                db.News.Add(news);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(news);
        }

        // GET: Backend/News/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        // POST: Backend/News/Edit/5
        // 若要免於大量指派 (overposting) 攻擊，請啟用您要繫結的特定屬性，
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Content,PubDate,CreateDate")] News news)
        {
            if (ModelState.IsValid)
            {
                db.Entry(news).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(news);
        }

        // GET: Backend/News/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        // POST: Backend/News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            News news = db.News.Find(id);
            db.News.Remove(news);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

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
    }
}