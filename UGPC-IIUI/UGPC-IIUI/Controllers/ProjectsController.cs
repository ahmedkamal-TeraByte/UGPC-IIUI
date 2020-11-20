using iTextSharp.text;
using iTextSharp.text.pdf;
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
        //        [Authorize(Roles = "Admin,Committee Incharge, Committee Member,Student")]
        public ActionResult Index()
        {
            if (User.IsInRole("Student"))
                return RedirectToAction("MyIndex");
            else if (User.IsInRole("Committee Incharge") || User.IsInRole("Committee Member") || User.IsInRole("Admin"))
            {
                var projects = _context.Projects.Include(p => p.Group.Student1).Include(p => p.Group.Student2);
                return View(projects.ToList());
            }
            else
            {
                return RedirectToAction("MyProjects");
            }
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

                var marking = new Marking
                {
                    ProjectId = project.ProjectId
                };
                _context.Markings.Add(marking);


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
            IList<ApplicationUser> supList = new List<ApplicationUser>();
            var sup = _context.Users.Include(u=>u.Professor).Where(u => u.StudentId == null && u.ProfessorId != null).ToList();
            foreach (var s in sup)
            {
                if(s.Professor.AssignedProjects<2)
                    supList.Add(s);
            }

            viewModel.Supervisors = supList;

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
                var supervisorId = viewModel.SupervisorId;
                project.SupervisorId =supervisorId;
                var prof = _context.Users.Include(u=>u.Professor).SingleOrDefault(u => u.Id == supervisorId);
                prof.Professor.AssignedProjects++;

                var marking = _context.Markings.SingleOrDefault(m => m.ProjectId == viewModel.ProjectId);
                switch (viewModel.Status)
                {
                    case "Rejected":
                        {
                            var group = _context.Groups.Include(g => g.Student1.Student).Include(g => g.Student2.Student)
                                .Single(g => g.Id == project.GroupId);
                            @group.Student1.Student.CanSubmitProposal = true;
                            @group.Student2.Student.CanSubmitProposal = true;
                            break;
                        }

                    case "Proposal Accepted":
                        {
                            if (marking == null)
                            {
                                marking = new Marking
                                {
                                    ProjectId = viewModel.ProjectId,
                                    PresentationMarks = viewModel.Marking
                                };
                                _context.Markings.Add(marking);
                            }
                            else
                            {
                                marking.PresentationMarks = viewModel.Marking;
                            }

                            break;
                        }

                    case "Ready For Internal Evaluation":
                        {
                            if (marking != null)
                                marking.SupervisorMarks = viewModel.Marking;
                            break;
                        }

                    case "Ready For External Evaluation":
                        {
                            if (marking != null)
                                marking.InternalMarks = viewModel.Marking;
                            break;
                        }

                    case "Completed":
                        {
                            if (marking != null)
                                marking.ExternalMarks = viewModel.Marking;
                            break;
                        }
                    default:
                        break;
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
            ViewBag.StatusList = Status;
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
            var presentations = _context.Presentations.Where(p => p.ProjectId == project.ProjectId).ToList();
            var marking = _context.Markings.SingleOrDefault(m => m.ProjectId == id);
            if (presentations.Count>0)
            {
                foreach (var p in presentations)
                    _context.Presentations.Remove(p);
            }
            _context.Projects.Remove(project);
            _context.Markings.Remove(marking);
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

        [Authorize(Roles = "Supervisor")]
        public ActionResult MyProjects()
        {
            var superId = User.Identity.GetUserId();

            var projects = _context.Projects.Include(p => p.Group.Student1).Include(p => p.Group.Student2).Where(p => p.SupervisorId == superId).ToList();

            return View("Index", projects);
        }

        public ActionResult GenerateLetter(int id)
        {
            PrepareReport(id);
            return RedirectToAction("Details", new { id = id });
        }


        private void PrepareReport(int id)
        {
            var project = _context.Projects
                .Include(pr => pr.Group.Student1)
                .Include(pr => pr.Group.Student1.Student)
                .Include(pr => pr.Group.Student1.Department)
                .Include(pr => pr.Group.Student2)
                .Include(pr => pr.Group.Student2.Student)
                .Include(pr => pr.Supervisor)
                .Single(pr => pr.ProjectId == id);
            if (project == null)
                return;

            var pdfDocument = new Document(PageSize.A4, 50f, 40f, 20f, 20f);


            #region creating file

            var path = Server.MapPath("~/App_Data/Files");
            var fileName = Path.GetFileName("Project Letter");
            fileName = DateTime.Now.ToString("yyMMddHHmmss-") + fileName + ".pdf";
            var fullPath = Path.Combine(path, fileName);
            PdfWriter.GetInstance(pdfDocument, new FileStream(fullPath, FileMode.Create));
            pdfDocument.Open();

            #endregion

            #region declarations

            //temp declarations
            var projectId = id;
            var title = project.Title;
            var student1Name = project.Group.Student1.Name;
            var student2Name = project.Group.Student2.Name;
            var professorName = project.Supervisor.Name;
            var regNo1 = project.Group.Student1.Student.RegNo;
            var regNo2 = project.Group.Student2.Student.RegNo;
            var batch = project.Group.Student1.Student.Batch;
            var single = regNo1 == regNo2;
            var programme = project.Group.Student1.Department.Name;
            if (programme == "Software Engineering")
                programme = "BSSE";
            else if (programme == "Computer Science")
                programme = "BSCS";
            else if (programme == "Information Technology")
                programme = "BSIT";
            var chairman = "Dr. Ayyaz Hussain";
            ///////////////

            var bfTimes = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            var boldFont = new Font(bfTimes, 13, Font.BOLD, iTextSharp.text.BaseColor.BLACK);
            var boldItalicFont = new Font(bfTimes, 13, Font.BOLDITALIC, iTextSharp.text.BaseColor.BLACK);
            var normalFont = new Font(bfTimes, 12, Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
            var italicFont = new Font(bfTimes, 12, Font.ITALIC, iTextSharp.text.BaseColor.BLACK);
            var boldUnderlineFont = new Font(bfTimes, 12, Font.BOLD | Font.UNDERLINE, iTextSharp.text.BaseColor.BLACK);


            var imgpath = Server.MapPath("~/App_Data/Images");
            var imgfileName = Path.GetFileName("logo.png");
            var imgfullPath = Path.Combine(imgpath, imgfileName);
            var img = Image.GetInstance(imgfullPath);
            img.ScaleAbsolute(60f, 60f);

            #endregion



            #region Header


            //////////////////////////
            //Header of Letter
            /////////////////////////


            //creating at table with 2 columns
            var headerTable = new PdfPTable(2);
            float[] width = { 70f, 525f };
            headerTable.SetWidthPercentage(width, PageSize.A4);

            //left column that will have the logo
            var leftCell = new PdfPCell();
            img.Alignment = Element.ALIGN_LEFT;
            leftCell.AddElement(img);
            leftCell.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER;

            //add column to table
            headerTable.AddCell(leftCell);


            //right column that will have header text
            var rightCell = new PdfPCell();
            var p = new Paragraph();
            p.Alignment = Element.ALIGN_CENTER;
            p.Add(new Phrase("INTERNATIONAL ISLAMIC UNIVERSITY, ISLAMABAD\n", boldFont));
            p.Add(new Phrase("FACULTY OF BASIC AND APPLIED SCIENCES\n", boldFont));
            p.Add(new Phrase("DEPARTMENT OF COMPUTER SCIENCE & SOFTWARE ENGINEERING", boldFont));

            //add text to column
            rightCell.AddElement(p);
            rightCell.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER;
            rightCell.PaddingLeft = 5f;

            //add column to table
            headerTable.AddCell(rightCell);

            //add header table to document
            pdfDocument.Add(headerTable);

            #endregion


            #region DateAndLetterNum

            var dateNumTable = new PdfPTable(2);

            dateNumTable.SetWidthPercentage(new float[] { 297f, 297f }, PageSize.A4);



            p.Clear();
            p.Add(new Phrase("No. IIU/FBAS/DCS&SE/" + DateTime.Now.Date.Year + "-" + projectId, normalFont));
            p.Alignment = Element.ALIGN_LEFT;
            var left = new PdfPCell();
            left.AddElement(p);
            left.Border = Rectangle.NO_BORDER;


            dateNumTable.AddCell(left);

            p.Clear();

            var date = DateTime.Now.ToString("dd-MM-yyyy");
            p.Add(new Phrase("Date:" + date, normalFont));
            p.Alignment = Element.ALIGN_RIGHT;
            var right = new PdfPCell();
            right.AddElement(p);
            right.Border = Rectangle.NO_BORDER;
            dateNumTable.AddCell(right);


            pdfDocument.Add(dateNumTable);



            #endregion

            p.Clear();
            p.Add(new Phrase("\n", normalFont));
            pdfDocument.Add(p);

            #region subject

            var subjectTable = new PdfPTable(2);

            subjectTable.SetWidthPercentage(new float[] { 60f, 535f }, PageSize.A4);

            p.Clear();
            p.Add(new Phrase("Subject:", normalFont));
            p.Alignment = Element.ALIGN_LEFT;
            var subjectTitle = new PdfPCell();
            subjectTitle.AddElement(p);
            subjectTitle.Border = Rectangle.NO_BORDER;


            subjectTable.AddCell(subjectTitle);

            var subjectData = new PdfPCell();
            p.Clear();
            p.Add(new Phrase("ALLOCATION OF PROVISIONAL SUPERVISION LETTER FOR " + programme + " PROJECT,\n", boldUnderlineFont));
            p.Add(new Phrase("\"" + title + "\"", normalFont));
            p.Alignment = Element.ALIGN_LEFT;
            subjectData.AddElement(p);
            subjectData.Border = Rectangle.NO_BORDER;
            subjectTable.AddCell(subjectData);


            pdfDocument.Add(subjectTable);

            #endregion

            #region Bodytext

            boldItalicFont.Size = 12;
            p.Clear();
            p.Add(new Phrase(@"
        The Department has allocated project titled above to ", normalFont));
            p.Add(new Phrase("Mr. " + student1Name + " Registration No. " + regNo1 + "-" + "FBAS/" + programme + "/" + batch, boldItalicFont));
            if (!single)
                p.Add(new Phrase(" and Mr. " + student2Name + " Registration No. " + regNo2 + "-" + "FBAS/" + programme + "/" + batch, boldItalicFont));

            p.Add(new Phrase(". " + professorName + ",", boldItalicFont));
            p.Add(new Phrase(
                @" from Department of Computer Science & Software Engineering, Faculty of Basic and Applied Sciences, International Islamic University, Islamabad will supervise the project. The work should be completed within one semester"
                , normalFont));
            p.Add(new Phrase(
                @"
        If the project is not completed within the prescribed period then you have to re-register in the next semester with only registration fee. Students failing to complete the project even in this additional duration will have to pay full fee for the subsequent semesters that will include the project fee plus registration fee."
                , italicFont));
            p.Add(new Phrase(
                @"
        Weekly progress report duly signed by the supervisor must also be submitted to the Program Coordinator. Project presentation within the concerned SIG after every three weeks is mandatory. Project will be evaluated as per the following criteria:-"
                , normalFont));

            p.Add(new Phrase("\n", normalFont));
            p.Alignment = Element.ALIGN_JUSTIFIED;
            pdfDocument.Add(p);

            var list = new List(List.UNORDERED);
            list.SetListSymbol("\u2022");
            list.IndentationLeft = 20f;
            list.Add(new ListItem("Scope", normalFont));
            list.Add(new ListItem("Project utility", normalFont));
            list.Add(new ListItem("Innovation", normalFont));
            list.Add(new ListItem("Selection of appropriate technology", normalFont));
            list.Add(new ListItem("Approach/ Implementation", normalFont));
            list.Add(new ListItem("Report write-up", normalFont));
            list.Add(new ListItem("Demo/ Presentation", normalFont));
            pdfDocument.Add(list);

            p.Clear();
            p.Add(new Phrase(
                @"
        The student must submit the copies of project report within three months after the Viva Voce Exam; otherwise whole process will be done again,"
                , normalFont));
            p.Alignment = Element.ALIGN_JUSTIFIED;

            pdfDocument.Add(p);
            #endregion

            #region Chairman Name

            var chairmanTable = new PdfPTable(2);
            chairmanTable.SetWidthPercentage(new float[] { 297f, 297f }, PageSize.A4);

            var chairmanLeftCell = new PdfPCell();
            chairmanLeftCell.Border = Rectangle.NO_BORDER;

            chairmanTable.AddCell(chairmanLeftCell);

            var chairmanRightCell = new PdfPCell();
            chairmanRightCell.Border = Rectangle.NO_BORDER;
            p.Clear();
            p.Add(new Phrase("\n\n", normalFont));
            pdfDocument.Add(p);
            p.Clear();

            boldFont.Size = 12;
            p.Add(new Phrase("(" + chairman + ")", boldFont));
            p.Add(new Phrase("\nChairman, DCS&SE, FBAS, IIUI", normalFont));
            p.Alignment = Element.ALIGN_CENTER;
            chairmanRightCell.AddElement(p);
            chairmanRightCell.HorizontalAlignment = Element.ALIGN_CENTER;
            chairmanTable.AddCell(chairmanRightCell);
            pdfDocument.Add(chairmanTable);


            p.Clear();
            p.Add(new Phrase("\n CC to:\n\n", normalFont));
            p.Alignment = Element.ALIGN_LEFT;
            pdfDocument.Add(p);

            var ccList = new List(List.UNORDERED);
            ccList.SetListSymbol("\u2022");
            ccList.IndentationLeft = 20f;
            ccList.Add(new ListItem("Supervisor of Student", normalFont));
            ccList.Add(new ListItem("Concerned Student", normalFont));
            ccList.Add(new ListItem("Program Office", normalFont));

            pdfDocument.Add(ccList);

            #endregion

            #region savefiletoDB

            var projectFile = new ProjectFile
            {
                FileName = fileName,
                ProjectId = id,
                FilePath = fullPath,
                FileType = "Project Letter"
            };
            _context.ProjectFiles.Add(projectFile);
            _context.SaveChanges();


            #endregion

            pdfDocument.Close();
        }

    }
}
