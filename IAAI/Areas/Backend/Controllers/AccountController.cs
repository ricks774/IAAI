using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using IAAI.Models;
using MVC0917.Models;

namespace IAAI.Areas.Backend.Controllers
{
    public class AccountController : BaseController
    {
        private IAAIDBContent db = new IAAIDBContent();

        // GET: Backend/Account
        [Authorize]
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        // GET: Backend/Account/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Backend/Account/Create
        public ActionResult Create()
        {
            initTree();
            return View();
        }

        // POST: Backend/Account/Create
        // 若要免於大量指派 (overposting) 攻擊，請啟用您要繫結的特定屬性，
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Account,Password,PasswordSalt,Name,Guid,Permissions")] User user)
        {
            // 判斷帳號是否有重複
            bool isLegal = db.Users.Any(u => u.Account == user.Account);
            if (isLegal)
            {
                ModelState.AddModelError("Account", "帳號重複");
                initTree();
                return View();
            }
            else
            {
                ModelState.Remove("Guid");
                if (ModelState.IsValid)
                {
                    var getHash = Utility.PasswordHash(user.Password);
                    user.Guid = Guid.NewGuid();
                    //user.PasswordSalt = Utility.CreateSalt();
                    //user.Password = Utility.GenerateHashWithSalt(user.Password, user.PasswordSalt);
                    user.Password = getHash.hashPassword;
                    user.PasswordSalt = getHash.salt;
                    db.Users.Add(user);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            return View(user);
        }

        // GET: Backend/Account/Edit/5
        public ActionResult Edit(int? id)
        {
            List<Permissions> permissions = db.Permissions.ToList();
            StringBuilder sb = new StringBuilder("[");

            List<Permissions> root = permissions.Where(p => p.ParentId == null).ToList();   // 找出主要節點
            GetTree(root, sb);  // 執行遞迴
            sb.Append("]");
            ViewBag.treeView = sb.ToString();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            GetPermission(id);    // 取得使用者的權限

            return View(user);
        }

        // POST: Backend/Account/Edit/5
        // 若要免於大量指派 (overposting) 攻擊，請啟用您要繫結的特定屬性，
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Account,Password,PasswordSalt,Name,Guid,Permissions")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // 錯誤時重新渲染
            List<Permissions> permissions = db.Permissions.ToList();
            StringBuilder sb = new StringBuilder("[");
            List<Permissions> root = permissions.Where(p => p.ParentId == null).ToList();   // 找出主要節點
            GetTree(root, sb);  // 執行遞迴
            sb.Append("]");
            ViewBag.treeView = sb.ToString();

            return View(user);
        }

        // GET: Backend/Account/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Backend/Account/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
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

        #region "顯示權限樹狀圖"

        private void initTree()
        {
            List<Permissions> permissions = db.Permissions.ToList();
            StringBuilder sb = new StringBuilder("[");

            List<Permissions> root = permissions.Where(p => p.ParentId == null).ToList();   // 找出主要節點
            GetTree(root, sb);  // 執行遞迴

            sb.Append("]");

            ViewBag.treeView = sb.ToString();
        }

        #endregion "顯示權限樹狀圖"

        #region "取得權限樹狀圖"

        private void GetTree(List<Permissions> permissionsList, StringBuilder sb)
        {
            foreach (Permissions Permission in permissionsList)
            {
                sb.Append("{ 'id':'" + Permission.Value + "', 'text':'" + Permission.Subject + "'");
                if (Permission.Children.Count > 0)
                {
                    sb.Append(",'children': [");
                    GetTree(Permission.Children, sb);
                    sb.Append("]");
                }
                sb.Append("},");
            }
        }

        #endregion "取得權限樹狀圖"

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