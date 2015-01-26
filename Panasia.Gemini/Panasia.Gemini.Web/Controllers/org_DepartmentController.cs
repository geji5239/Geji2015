using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Panasia.Gemini.Web.Models;
using Panasia.Gemini.Web.Models.DepartmentPageModel;

namespace Panasia.Gemini.Web.Controllers
{
    public class org_DepartmentController : Controller
    {
        private PADBContext db = new PADBContext();

        //
        // GET: /org_Department/

        public ActionResult Index()
        {
            DepartmentListModel pageModel = new DepartmentListModel();
            pageModel.DepartmentList = db.org_Departments.ToList();
            return View(pageModel);
        }

        //
        // GET: /org_Department/Details/5

        public ActionResult Details(int id = 0)
        {
            org_Department org_department = db.org_Departments.Find(id);
            if (org_department == null)
            {
                return HttpNotFound();
            }
            return View(org_department);
        }

        //
        // GET: /org_Department/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /org_Department/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(org_Department org_department)
        {
            if (ModelState.IsValid)
            {
                db.org_Departments.Add(org_department);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(org_department);
        }

        //
        // GET: /org_Department/Edit/5

        public ActionResult Edit(int id = 0)
        {
            org_Department org_department = db.org_Departments.Find(id);
            if (org_department == null)
            {
                return HttpNotFound();
            }
            return View(org_department);
        }

        //
        // POST: /org_Department/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(org_Department org_department)
        {
            if (ModelState.IsValid)
            {
                db.Entry(org_department).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(org_department);
        }

        //
        // GET: /org_Department/Delete/5

        public ActionResult Delete(int id = 0)
        {
            org_Department org_department = db.org_Departments.Find(id);
            if (org_department == null)
            {
                return HttpNotFound();
            }
            return View(org_department);
        }

        //
        // POST: /org_Department/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            org_Department org_department = db.org_Departments.Find(id);
            db.org_Departments.Remove(org_department);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }




    }
}