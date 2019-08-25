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
    [Authorize]
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
            var group = _context.Groups.SingleOrDefault(g => g.Student1Id == id || g.Student2Id == id);
            if (group == null)
                return RedirectToAction("MyIndex", "Groups");
            var groupId = group.Id;
            var projects = _context.Projects.Include(s => s.Group.Student1).Include(s => s.Group.Student2).Where(p => p.GroupId == groupId).ToList();
            var check = _context.Users.Include(u => u.Student).Single(u => u.Id == id).Student.CanSubmitProposal;
            ViewBag.CanSubmit = check;
            return View("Index", projects);
        }

        // GET: Projects/Details/5
        [Authorize(Roles = "Admin,Committee Member , Committee Incharge,Student")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var project = _context.Projects.Include(p => p.Group.Student1).Include(p => p.Group.Student2).Include(s => s.Supervisor).Single(p => p.ProjectId == id);
            if (project == null)
            {
                return HttpNotFound();
            }

            if (!(User.IsInRole("Student")) || (User.Identity.GetUserId() == project.Group.Student1Id ||
                User.Identity.GetUserId() == project.Group.Student2Id))
            {
                var changes = _context.Changes.Single(c => c.ProjectId == project.ProjectId);

                var files = _context.ProjectFiles.Where(f => f.ProjectId == id).ToList();

                var viewModel = new ProjectViewModel
                {
                    ProjectId = project.ProjectId,
                    Title = project.Title,
                    Student1 = project.Group.Student1,
                    Student2 = project.Group.Student2,
                    SubmissionDate = project.SubmissionDate,
                    ProjectType = project.ProjectType,
                    Status = project.Status,
                    Changes = changes.Changes,
                    Supervisor = project.Supervisor,
                    ProjectFiles = files
                };

                return View(viewModel);
            }
            else
                return RedirectToAction("Index");
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
            var group = _context.Groups.SingleOrDefault(g => g.Student1Id == id || g.Student2Id == id);
            if (group == null)
            {
                return RedirectToAction("MyIndex", "Groups");
            }

            var groupId = group.Id;
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
        [Authorize(Roles = "Admin,Student")]
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

                var group = _context.Groups.Include(g => g.Student1.Student).Include(g => g.Student2.Student).Single(g => g.Id == viewModel.GroupId);
                group.Student1.Student.CanSubmitProposal = false;
                group.Student2.Student.CanSubmitProposal = false;


                _context.Projects.Add(project);
                _context.ProjectFiles.Add(projectFile);
                _context.SaveChanges();
                var changes = new Change
                {
                    ProjectId = project.ProjectId
                };
                _context.Changes.Add(changes);

                _context.SaveChanges();
                var presentation = new Presentation
                {
                    ProjectId = project.ProjectId,
                    Status = "NotScheduled"
                };
                _context.Presentations.Add(presentation);

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
            var project = _context.Projects.Include(s => s.Group.Student1).Include(p => p.Group.Student2).Include(s => s.Supervisor).Single(p => p.ProjectId == id);


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
                Status = project.Status,
                // SupervisorId = project.Supervisor.Id
            };
            if (project.SupervisorId == null)
                viewModel.SupervisorId = null;
            else
            {
                viewModel.SupervisorName = project.Supervisor.Name;
                viewModel.SupervisorId = project.SupervisorId;
            }

            var change = _context.Changes.SingleOrDefault(c => c.ProjectId == id);
            if (change != null)
                viewModel.Changes = change.Changes;
            IEnumerable<SelectListItem> Status = new List<SelectListItem>
            {
                new SelectListItem() { Value = "Proposal Submitted", Text = "Proposal Submitted" },
                new SelectListItem() { Value = "Proposal Accepted", Text = "Proposal Accepted" },
                new SelectListItem() { Value = "Proposal Accepted With Changes", Text = "Proposal Accepted With Changes" },
                new SelectListItem() { Value = "In Progress", Text = "In Progress" },
                new SelectListItem() { Value = "Ready For Internal Evaluation", Text = "Ready For Internal Evaluation" },
                new SelectListItem() { Value = "Ready For External Evaluation", Text = "Ready For External Evaluation" },
                new SelectListItem() { Value = "Completed", Text = "Completed" },
                new SelectListItem() { Value = "Rejected", Text = "Rejected" }

            };
            ViewBag.StatusList = Status;

            viewModel.Supervisors = _context.Users.Where(u => u.StudentId == null && u.ProfessorId != null).ToList();

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
                    return HttpNotFound("Project was Deleted");
                project.Status = viewModel.Status;
                project.SupervisorId = viewModel.SupervisorId;
                if (viewModel.Status == "Rejected")
                {
                    var group = _context.Groups.Include(g => g.Student1.Student).Include(g => g.Student2.Student)
                        .Single(g => g.Id == project.GroupId);
                    group.Student1.Student.CanSubmitProposal = true;
                    group.Student2.Student.CanSubmitProposal = true;

                }

                var change = _context.Changes.SingleOrDefault(c => c.ProjectId == viewModel.ProjectId);
                if (change != null)
                {
                    change.Changes = viewModel.Changes;
                }
                else
                {
                    var changes = new Change
                    {
                        Changes = viewModel.Changes,
                        ProjectId = project.ProjectId
                    };

                    _context.Changes.Add(changes);
                }

                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            IEnumerable<SelectListItem> Status = new List<SelectListItem>
            {
                new SelectListItem() { Value = "Proposal Submitted", Text = "Proposal Submitted" },
                new SelectListItem() { Value = "Proposal Accepted", Text = "Proposal Accepted" },
                new SelectListItem() { Value = "Proposal Accepted With Changes", Text = "Proposal Accepted With Changes" },
                new SelectListItem() { Value = "In Progress", Text = "In Progress" },
                new SelectListItem() { Value = "Ready For Internal Evaluation", Text = "Ready For Internal Evaluation" },
                new SelectListItem() { Value = "Ready For External Evaluation", Text = "Ready For External Evaluation" },
                new SelectListItem() { Value = "Completed", Text = "Completed" },
                new SelectListItem() { Value = "Rejected", Text = "Rejected" }
            };
            ViewBag.Status = Status;
            viewModel.Supervisors = _context.Users.Where(u => u.StudentId == null && u.ProfessorId != null).ToList();


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
            var project = _context.Projects.Include(p => p.Group.Student1).Include(p => p.Group.Student2).Single(p => p.ProjectId == id);
            if (project == null)
            {
                return HttpNotFound();
            }
            var changes = _context.Changes.Single(c => c.ProjectId == project.ProjectId);
            var viewModel = new ProjectViewModel
            {
                ProjectId = project.ProjectId,
                Title = project.Title,
                Student1 = project.Group.Student1,
                Student2 = project.Group.Student2,
                SubmissionDate = project.SubmissionDate,
                ProjectType = project.ProjectType,
                Status = project.Status,
                Changes = changes.Changes
            };
            return View(viewModel);
        }

        // POST: Projects/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = _context.Projects.Find(id);

            var group = _context.Groups.Include(g => g.Student1.Student).Include(g => g.Student2.Student).Single(g => g.Id == project.GroupId);
            group.Student1.Student.CanSubmitProposal = true;
            group.Student2.Student.CanSubmitProposal = true;
            var changes = _context.Changes.Single(c => c.ProjectId == project.ProjectId);
            var presentations = _context.Presentations.Where(p => p.ProjectId == project.ProjectId);
            if (presentations != null)
            {
                foreach (var p in presentations)
                    _context.Presentations.Remove(p);
            }
            _context.Projects.Remove(project);
            _context.Changes.Remove(changes);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult AddNewFile(int id)
        {
            var userId = User.Identity.GetUserId();
            var project = _context.Projects.Include(s => s.Group).Single(p => p.ProjectId == id);
            if (!(userId == project.Group.Student1Id || userId == project.Group.Student2Id))
                return RedirectToAction("Index");
            var viewModel = new FileViewModel
            {
                ProjectId = id
            };
            IEnumerable<SelectListItem> fileTypes = new List<SelectListItem>
            {
                new SelectListItem() { Value = "Proposal", Text = "Proposal" },
                new SelectListItem() { Value = "Presentation", Text = "Presentation" },
                new SelectListItem() { Value = "Others", Text = "Others" }
            };
            ViewBag.FileTypes = fileTypes;
            return View(viewModel);
        }


        [HttpPost, ActionName("AddNewFile")]
        [Authorize(Roles = "Student")]
        [ValidateAntiForgeryToken]
        public ActionResult AddNewFile(FileViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var path = Server.MapPath("~/App_Data/Files");
                var fileName = Path.GetFileName(viewModel.ProjectFile.FileName);
                fileName = DateTime.Now.ToString("yyMMddHHmmss") + fileName;

                var fullPath = Path.Combine(path, fileName);

                //saving the file in the server at "full path"
                viewModel.ProjectFile.SaveAs(fullPath);

                var projectFile = new ProjectFile
                {
                    FileName = fileName,
                    ProjectId = viewModel.ProjectId,
                    FilePath = fullPath,
                    FileType = viewModel.FileType
                };
                _context.ProjectFiles.Add(projectFile);
                _context.SaveChanges();

                return RedirectToAction("Details", new { id = viewModel.ProjectId });
            }

            IEnumerable<SelectListItem> fileTypes = new List<SelectListItem>
                {
                    new SelectListItem() { Value = "Proposal", Text = "Proposal" },
                    new SelectListItem() { Value = "Presentation", Text = "Presentation" },
                    new SelectListItem() { Value = "Others", Text = "Others" }
                };
            ViewBag.FileTypes = fileTypes;
            return View(viewModel);
        }

        public ActionResult DownloadFile(int id)
        {
            var file = _context.ProjectFiles.SingleOrDefault(f => f.ProjectFileId == id);
            if (file == null)
                return HttpNotFound("File not found");
            var fullPath = file.FilePath;
            var fileName = Path.GetFileName(fullPath);
            var contentType = "application/pdf";
            return File(fullPath, contentType, fileName);
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
            var viewModel = new ProjectViewModel();
            viewModel.Status = "Proposal Accepted With Changes";
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

        public ActionResult Testing(ProjectViewModel viewModel)
        {
            var status = viewModel.Changes;
            return Content(status);
        }

        public void AddFile()
        {
            throw new NotImplementedException();
        }
    }
}
