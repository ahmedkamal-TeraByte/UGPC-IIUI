using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using UGPC_IIUI.Models;
using UGPC_IIUI.ViewModels;

namespace UGPC_IIUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StudentsController : Controller
    {

        private ApplicationDbContext _context = new ApplicationDbContext();
        private ApplicationUserManager _userManager;


        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }


        // GET: Students
        public ActionResult Index()
        {
            IEnumerable<ApplicationUser> users = _context.Users.Where(u => u.ProfessorId == null && u.StudentId != null)
                .Include(u => u.Student).Include(d => d.Department).ToList();
            return View(users);
        }



        // GET: Students/Create
        public ActionResult Create()
        {
            var departments = _context.Departments.ToList();
            var viewModel = new NewUserViewModel
            {
                Departments = departments
            };
            ViewBag.Title = "Create";
            return View("NewStudentForm", viewModel);
        }

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(NewUserViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var student = new Student
                {
                    Batch = viewModel.Student.Batch,
                    RegNo = viewModel.Student.RegNo,
                    Section = viewModel.Student.Section
                };

                var user = new ApplicationUser
                {
                    UserName = viewModel.UserName,
                    Email = viewModel.Email,
                    Name = viewModel.Name,
                    DepartmentId = viewModel.DepartmentId,
                    Student = student,
                    Joined = false

                };
                var result = await UserManager.CreateAsync(user, viewModel.Password);
                if (result.Succeeded)
                {

                    // ** use Role Name in "" for user role in place of model.role
                    await UserManager.AddToRoleAsync(user.Id, "Student");
                    return RedirectToAction("Index");
                }
                AddErrors(result);

            }

            var departments = _context.Departments.ToList();
            viewModel.Departments = departments;
            return View("NewStudentForm", viewModel);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {

                ModelState.AddModelError("", error);
            }
        }

        // GET: Students/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser student = _context.Users.Where(u => u.StudentId == id).Include(s => s.Student).Include(d => d.Department).SingleOrDefault();

            if (student == null)
            {
                return HttpNotFound();
            }

            var viewModel = new UserViewModel
            {
                userId = student.Id,
                Name = student.Name,
                Email = student.Email,
                UserName = student.UserName,
                DepartmentId = student.DepartmentId,
                Student = student.Student,
                Departments = _context.Departments.ToList(),
                Role = "Student"
            };

            ViewBag.Title = "Edit";
            return View("StudentForm", viewModel);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var userInDb = _context.Users.Include(s => s.Student).Single(u => u.Id == viewModel.userId);

                userInDb.Email = viewModel.Email;
                userInDb.Name = viewModel.Name;
                userInDb.UserName = viewModel.UserName;
                userInDb.DepartmentId = viewModel.DepartmentId;
                userInDb.Student.RegNo = viewModel.Student.RegNo;
                userInDb.Student.Batch = viewModel.Student.Batch;
                userInDb.Student.Section = viewModel.Student.Section;


                _context.SaveChanges();


                return RedirectToAction("Index");
            }

            viewModel.Departments = _context.Departments.ToList();
            return View("StudentForm", viewModel);
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
