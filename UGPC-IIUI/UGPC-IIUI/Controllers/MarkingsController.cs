using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using UGPC_IIUI.Models;

namespace UGPC_IIUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MarkingsController : Controller
    {
        private ApplicationDbContext _context = new ApplicationDbContext();

        // GET: Markings
        public ActionResult Index()
        {
            var markings = _context.Markings.Include(m => m.Project.Group.Student1).Include(m => m.Project.Group.Student1.Student).Include(m => m.Project.Group.Student2).Include(m => m.Project.Group.Student2.Student).Include(m => m.Project);
            return View(markings.ToList());
        }



        // GET: Markings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Marking marking = _context.Markings.Include(m => m.Project).Single(m => m.MarkingId == id);
            if (marking == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProjectId = new SelectList(_context.Projects, "ProjectId", "Title", marking.ProjectId);
            return View(marking);
        }

        // POST: Markings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MarkingId,ProjectId,PresentationMarks,SupervisorMarks,InternalMarks,ExternalMarks")] Marking marking)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(marking).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProjectId = new SelectList(_context.Projects, "ProjectId", "Title", marking.ProjectId);
            return View(marking);
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
