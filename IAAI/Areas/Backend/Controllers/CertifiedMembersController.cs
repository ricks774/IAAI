using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IAAI.Models;
using MVC0917.Models;
using ImageResizer;

namespace IAAI.Areas.Backend.Controllers
{
    public class CertifiedMembersController : BaseController
    {
        private IAAIDBContent db = new IAAIDBContent();

        // GET: Backend/CertifiedMembers
        public ActionResult Index()
        {
            return View(db.CertifiedMembers.ToList());
        }

        // GET: Backend/CertifiedMembers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CertifiedMember certifiedMember = db.CertifiedMembers.Find(id);
            if (certifiedMember == null)
            {
                return HttpNotFound();
            }
            return View(certifiedMember);
        }

        // GET: Backend/CertifiedMembers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Backend/CertifiedMembers/Create
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Picture,FirstName,LastName,Country,Title,Company,CreateDate")] CertifiedMember certifiedMember, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    // 查詢取得資料庫中最後一筆記錄的 Id
                    int lastId = db.CertifiedMembers.OrderByDescending(x => x.Id).Select(x => x.Id).FirstOrDefault();

                    string fileName = Path.GetFileName(ImageFile.FileName);
                    // 取得圖片的副檔名
                    string extension = Path.GetExtension(ImageFile.FileName);
                    // 組合新的檔名：id+現在時間（年月日時分秒毫秒） + 副檔名
                    fileName = (lastId + 1) + "_" + DateTime.Now.ToString("yyMMddHHmmssfff") + extension;
                    // 組合完整的檔案路徑 (絕對路徑_實際儲存檔案)
                    string saveAsPath = Path.Combine(Server.MapPath("~/Uploads/CertifiedMember"), fileName);
                    // 儲存到資料庫的路徑 (相對路徑)
                    string sqlPath = $"/Uploads/CertifiedMember/{fileName}";
                    certifiedMember.Picture = sqlPath;

                    // 使用 ImageResizer 套件來調整圖片大小並保存
                    ImageBuilder.Current.Build(ImageFile.InputStream, saveAsPath, new ResizeSettings("width=200&height=200&mode=max"));
                }
                db.CertifiedMembers.Add(certifiedMember);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(certifiedMember);
        }

        // GET: Backend/CertifiedMembers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CertifiedMember certifiedMember = db.CertifiedMembers.Find(id);
            if (certifiedMember == null)
            {
                return HttpNotFound();
            }
            return View(certifiedMember);
        }

        // POST: Backend/CertifiedMembers/Edit/5
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Picture,FirstName,LastName,Country,Title,Company,CreateDate")] CertifiedMember certifiedMember, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    // 刪除舊的圖片
                    string filePath = Server.MapPath(certifiedMember.Picture);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }

                    // 取得目前檔案的名稱
                    string fileName = Path.GetFileName(certifiedMember.Picture);
                    // 組合完整的檔案路徑 (絕對路徑_實際儲存檔案)
                    string saveAsPath = Path.Combine(Server.MapPath("~/Uploads/CertifiedMember"), fileName);

                    // 使用 ImageResizer 套件來調整圖片大小並保存
                    ImageBuilder.Current.Build(ImageFile.InputStream, saveAsPath, new ResizeSettings("width=200&height=200&mode=max"));
                }

                db.Entry(certifiedMember).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(certifiedMember);
        }

        // GET: Backend/CertifiedMembers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CertifiedMember certifiedMember = db.CertifiedMembers.Find(id);
            if (certifiedMember == null)
            {
                return HttpNotFound();
            }
            return View(certifiedMember);
        }

        // POST: Backend/CertifiedMembers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CertifiedMember certifiedMember = db.CertifiedMembers.Find(id);

            // 刪除圖片
            string filePath = Server.MapPath(certifiedMember.Picture);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            db.CertifiedMembers.Remove(certifiedMember);
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
    }
}