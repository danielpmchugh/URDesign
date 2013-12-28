using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using URDesign.Models;
using PagedList;
using PagedList.Mvc;
using System.IO;


namespace URDesign.Controllers
{
    public class FurnitureController : Controller
    {
        private FurnitureDBContext db = new FurnitureDBContext();

        // GET: /Furniture/
        public ActionResult Index(int? page)
        {

            var products = db.FindAllProducts(); //returns IQueryable<Product> representing an unknown number of products. a thousand maybe?

            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var onePageOfProducts = products.ToPagedList(pageNumber, 10); // will only contain 25 products max because of the pageSize

            ViewBag.OnePageOfProducts = onePageOfProducts;

            return View();
        }

        // GET: /Furniture/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Furniture furniture = db.Furniture.Find(id);
            if (furniture == null)
            {
                return HttpNotFound();
            }
            return View(furniture);
        }

        // GET: /Furniture/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Furniture/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="IdFurniture,Name,Vendor,URL,Price")] Furniture furniture)
        {
            if (ModelState.IsValid)
            {
                db.Furniture.Add(furniture);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(furniture);
        }

        // GET: /Furniture/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Furniture furniture = db.Furniture.Find(id);
            if (furniture == null)
            {
                return HttpNotFound();
            }
            return View(furniture);
        }

        // POST: /Furniture/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdFurniture,Name,Vendor,URL,Price")] Furniture furniture, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null && file.ContentLength > 0)
                {
                    furniture.DrawingFile = file.FileName;
                    using (Stream inputStream = file.InputStream)
                    {
                        MemoryStream memoryStream = inputStream as MemoryStream;
                        if (memoryStream == null)
                        {
                            memoryStream = new MemoryStream();
                            inputStream.CopyTo(memoryStream);
                        }
                        furniture.DrawingFileData = memoryStream.ToArray();
                    }
                }

                db.Entry(furniture).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(furniture);
        }


        public FileResult Download(int? id)
        {
            Furniture furniture = db.Furniture.Find(id);
            return File(furniture.DrawingFileData, "application/octet-stream", furniture.DrawingFile);
        }

        // GET: /Furniture/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Furniture furniture = db.Furniture.Find(id);
            if (furniture == null)
            {
                return HttpNotFound();
            }
            return View(furniture);
        }

        // POST: /Furniture/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Furniture furniture = db.Furniture.Find(id);
            db.Furniture.Remove(furniture);
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
