using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using UGPC_IIUI.Models;

namespace UGPC_IIUI.Controllers
{
    public class ProjectsController : Controller
    {
        private ApplicationDbContext _context = new ApplicationDbContext();

        // GET: Projects
        public ActionResult Index()
        {
            var projects = _context.Projects.Include(p => p.Group);
            return View(projects.ToList());
        }

        public ActionResult MyIndex()
        {
            var id = User.Identity.GetUserId();
            var groupId = _context.Groups.Single(g => g.Student1Id == id || g.Student2Id == id).Id;
            var project = _context.Projects.Single(p => p.GroupId == groupId);
            return View("Index", project);
        }

        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = _context.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // GET: Projects/Create
        public ActionResult Create()
        {
            ViewBag.GroupId = new SelectList(_context.Groups, "Id", "Student1Id");
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProjectId,Title,SubmissionDate,Status,ProjectType,GroupId")] Project project)
        {
            if (ModelState.IsValid)
            {
                _context.Projects.Add(project);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.GroupId = new SelectList(_context.Groups, "Id", "Student1Id", project.GroupId);
            return View(project);
        }

        // GET: Projects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = _context.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            ViewBag.GroupId = new SelectList(_context.Groups, "Id", "Student1Id", project.GroupId);
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProjectId,Title,SubmissionDate,Status,ProjectType,GroupId")] Project project)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(project).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.GroupId = new SelectList(_context.Groups, "Id", "Student1Id", project.GroupId);
            return View(project);
        }

        // GET: Projects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = _context.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = _context.Projects.Find(id);
            _context.Projects.Remove(project);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
