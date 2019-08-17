using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using UGPC_IIUI.Models;
using UGPC_IIUI.ViewModels;

namespace UGPC_IIUI.Controllers
{
    public class ProjectsController : Controller
    {
        private ApplicationDbContext _context = new ApplicationDbContext();

        [Authorize]
        // GET: Projects
        public ActionResult Index()
        {
            if (User.IsInRole("Student"))
                return RedirectToAction("MyIndex");
            var projects = _context.Projects.Include(p => p.Group.Student1).Include(p => p.Group.Student2);
            return View(projects.ToList());
        }

        public ActionResult MyIndex()
        {
            var id = User.Identity.GetUserId();
            var groupId = _context.Groups.Single(g => g.Student1Id == id || g.Student2Id == id).Id;
            var projects = _context.Projects.Include(s => s.Group.Student1).Include(s => s.Group.Student2).Where(p => p.GroupId == groupId).ToList();
            return View("Index", projects);
        }

        // GET: Projects/Details/5
        [Authorize(Roles = "Admin,Committee Member , Committee Incharge")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Project project = _context.Projects.Include(p => p.Group.Student1).Include(p => p.Group.Student2).Single(p => p.ProjectId == id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // GET: Projects/Create
        [Authorize(Roles = "Admin,Student")]
        public ActionResult Create()
        {
            IEnumerable<SelectListItem> ProjectTypes = new List<SelectListItem>
            {
                new SelectListItem() { Value = "Research", Text = "Research" },
                new SelectListItem() { Value = "Development", Text = "Development" }

            };
            ViewBag.ProjectType = ProjectTypes;
            var id = User.Identity.GetUserId();
            var groupId = _context.Groups.Single(g => g.Student1Id == id || g.Student2Id == id).Id;

            var viewModel = new NewProjectViewModel
            {
                GroupId = groupId,
                SubmissionDate = DateTime.Now.Date
            };
            return View(viewModel);
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Committee Member , Committee Incharge")]
        public ActionResult Create(NewProjectViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var path = Server.MapPath("~/App_Data/Files");
                var fileName = Path.GetFileName(viewModel.ProjectFile.FileName);
                fileName = DateTime.Now.ToString("yyMMddHHmmss") + fileName;

                var fullPath = Path.Combine(path, fileName);

                //saving the file in the server at "full path"
                viewModel.ProjectFile.SaveAs(fullPath);

                //now we need to save the details to the database
                var project = new Project
                {
                    GroupId = viewModel.GroupId,
                    Title = viewModel.Title,
                    SubmissionDate = viewModel.SubmissionDate,
                    ProjectType = viewModel.ProjectType,
                    Status = "Proposal Submitted"
                };

                var projectFile = new ProjectFile
                {
                    FileName = fileName,
                    ProjectId = project.ProjectId,
                    FilePath = fullPath,
                    FileType = "Proposal"
                };

                _context.Projects.Add(project);
                _context.ProjectFiles.Add(projectFile);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            IEnumerable<SelectListItem> ProjectTypes = new List<SelectListItem>
            {
                new SelectListItem() { Value = "Research", Text = "Research" },
                new SelectListItem() { Value = "Development", Text = "Development" }

            };
            ViewBag.ProjectType = ProjectTypes;

            return View(viewModel);
        }

        // GET: Projects/Edit/5

        [Authorize(Roles = "Admin, Committee Member , Committee Incharge")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var project = _context.Projects.Include(s => s.Group.Student1).Include(p => p.Group.Student2).Single(p => p.ProjectId == id);

            if (project == null)
            {
                return HttpNotFound();
            }

            var viewModel = new ProjectViewModel
            {
                ProjectId = project.ProjectId,
                Title = project.Title,
                SubmissionDate = project.SubmissionDate,
                ProjectType = project.ProjectType,
                Student1 = project.Group.Student1,
                Student2 = project.Group.Student2,
                Status = project.Status
            };
            IEnumerable<SelectListItem> Status = new List<SelectListItem>
            {
                new SelectListItem() { Value = "Proposal Submitted", Text = "Proposal Submitted" },
                new SelectListItem() { Value = "Proposal Accepted", Text = "Proposal Accepted" },
                new SelectListItem() { Value = "Proposal Accepted With Changes", Text = "Proposal Accepted With Changes" },
                new SelectListItem() { Value = "In Progress", Text = "In Progress" },
                new SelectListItem() { Value = "Completed", Text = "Completed" },
                new SelectListItem() { Value = "Rejected", Text = "Rejected" }
            };
            ViewBag.Status = Status;

            //ViewBag.GroupId = new SelectList(_context.Groups, "Id", "Student1Id", project.GroupId);
            return View(viewModel);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Committee Member , Committee Incharge")]
        public ActionResult Edit(ProjectViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var project = _context.Projects.Find(viewModel.ProjectId);
                if (project == null)
                    return Content("NULL");
                project.Status = viewModel.Status;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            IEnumerable<SelectListItem> Status = new List<SelectListItem>
            {
                new SelectListItem() { Value = "Proposal Submitted", Text = "Proposal Submitted" },
                new SelectListItem() { Value = "Proposal Accepted", Text = "Proposal Accepted" },
                new SelectListItem() { Value = "Proposal Accepted With Changes", Text = "Proposal Accepted With Changes" },
                new SelectListItem() { Value = "In Progress", Text = "In Progress" },
                new SelectListItem() { Value = "Completed", Text = "Completed" },
                new SelectListItem() { Value = "Rejected", Text = "Rejected" }
            };
            ViewBag.Status = Status;

            return View(viewModel);
        }

        // GET: Projects/Delete/5
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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



        public ActionResult test()
        {
            var viewModel= new ProjectViewModel();
            viewModel.Status = "Proposal Submitted";
            IEnumerable<SelectListItem> Status = new List<SelectListItem>
            {
                new SelectListItem() { Value = "Proposal Submitted", Text = "Proposal Submitted" },
                new SelectListItem() { Value = "Proposal Accepted", Text = "Proposal Accepted" },
                new SelectListItem() { Value = "Proposal Accepted With Changes", Text = "Proposal Accepted With Changes" },
                new SelectListItem() { Value = "In Progress", Text = "In Progress" },
                new SelectListItem() { Value = "Completed", Text = "Completed" },
                new SelectListItem() { Value = "Rejected", Text = "Rejected" }
            };
            ViewBag.Status1 = Status;
            return View(viewModel);
        }

    }
}
