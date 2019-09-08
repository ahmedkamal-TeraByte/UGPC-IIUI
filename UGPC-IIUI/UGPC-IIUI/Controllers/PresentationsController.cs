using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UGPC_IIUI.Models;
using UGPC_IIUI.ViewModels;

namespace UGPC_IIUI.Controllers
{
    [Authorize]
    public class PresentationsController : Controller
    {
        private ApplicationDbContext _context = new ApplicationDbContext();

        // GET: Presentations
        [AllowAnonymous]
        public ActionResult Index()
        {
            var presentations = _context.Presentations.Include(p => p.Project).Where(p=>p.Status=="Scheduled").ToList();
            var count = 0;
            count = _context.Projects.Count(p => p.Status == "Proposal Submitted");
            ViewBag.Count = count;
            return View(presentations.ToList());
        }

       
        [Authorize(Roles = "Admin, Committee Incharge, Committee Member")]
        // GET: Presentations/Create
        public ActionResult Create()
        {

           // ViewBag.ProjectId = new SelectList(_context.Projects.Where(p=>p.Status=="Proposal Submitted"), "ProjectId", "Title");
            return View();
        }

        // POST: Presentations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin, Committee Incharge, Committee Member")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreatePresentationsViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var num = viewModel.NumberOfPresentations;
                var presentations = _context.Presentations.Where(p => p.Status == "NotScheduled").Take(num).ToList();
                if (presentations != null)
                {
                    foreach (var p in presentations)
                    {
                        p.Date = viewModel.Date;
                        p.Time = viewModel.Time;
                        p.Status = "Scheduled";
                    }

                    _context.SaveChanges();
                }
                return RedirectToAction("Index");
            }

//            ViewBag.ProjectId = new SelectList(_context.Projects.Where(p => p.Status == "Proposal Submitted"), "ProjectId", "Title", presentation.ProjectId);
            return View(viewModel);
        }

        [Authorize(Roles = "Admin, Committee Incharge, Committee Member")]
        // GET: Presentations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var presentation = _context.Presentations.Include(p=>p.Project).SingleOrDefault(p=>p.PresentationId==id);
            if (presentation == null)
            {
                return HttpNotFound();
            }
            IEnumerable<SelectListItem> status = new List<SelectListItem>
            {
                new SelectListItem() { Value = "Scheduled", Text = "Scheduled" },
                new SelectListItem() { Value = "Done", Text = "Done" },
                new SelectListItem() { Value = "ReSchedule", Text = "ReSchedule" }
            };
            ViewBag.StatusList = status;

            return View(presentation);
        }

        // POST: Presentations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin, Committee Incharge, Committee Member")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PresentationId,Date,Time,Status,ProjectId")] Presentation presentation)
        {
            if (ModelState.IsValid)
            {
                var presentationInDb = _context.Presentations.Find(presentation.PresentationId);
                if (presentation.Status == "Done")
                {
                    presentationInDb.Status = presentation.Status;
                }
                else if (presentation.Status == "ReSchedule")
                {
                    _context.Presentations.Remove(presentationInDb);
                    var newPresentation = new Presentation
                    {
                        Status = "NotScheduled",
                        ProjectId = presentation.ProjectId
                    };
                    _context.Presentations.Add(newPresentation);
                }
                else
                {
                    presentationInDb = presentation;
                }

                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            IEnumerable<SelectListItem> status = new List<SelectListItem>
            {
                new SelectListItem() { Value = "Scheduled", Text = "Scheduled" },
                new SelectListItem() { Value = "Done", Text = "Done" },
                new SelectListItem() { Value = "ReSchedule", Text = "ReSchedule" }
            };
            ViewBag.StatusList = status;
            return View(presentation);
        }

//        // GET: Presentations/Delete/5
//        public ActionResult Delete(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            Presentation presentation = _context.Presentations.Find(id);
//            if (presentation == null)
//            {
//                return HttpNotFound();
//            }
//            return View(presentation);
//        }
//
//        // POST: Presentations/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public ActionResult DeleteConfirmed(int id)
//        {
//            Presentation presentation = _context.Presentations.Find(id);
//            _context.Presentations.Remove(presentation);
//            _context.SaveChanges();
//            return RedirectToAction("Index");
//        }

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
