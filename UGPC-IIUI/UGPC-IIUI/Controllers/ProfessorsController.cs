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
    public class ProfessorsController : Controller


    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext _context = new ApplicationDbContext();

        public ProfessorsController()
        {

        }
        public ProfessorsController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get { return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>(); }
            private set { _signInManager = value; }
        }

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }

        // GET: Professors
        public ActionResult Index()
        {
            IEnumerable<ApplicationUser> users = _context.Users.Where(u => u.ProfessorId != null && u.StudentId == null)
                .Include(u => u.Professor).ToList();

            return View(users);
        }

        // GET: Professors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ApplicationUser professor = _context.Users.Where(u => u.ProfessorId == id).Include(d => d.Department).SingleOrDefault();
            if (professor == null)
            {
                return HttpNotFound();
            }

            var role = UserManager.GetRoles(professor.Id);
            var prof = new Professor
            {
                ProfessorId = (int)id
            };

            var viewModel = new UserViewModel
            {
                Name = professor.Name,
                Email = professor.Email,
                UserName = professor.UserName,
                Department = professor.Department,
                Role = role[0],
                Professor = prof
            };

            return View(viewModel);
        }

        // GET: Professors/Create
        public ActionResult Create()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var roles = _context.Roles.Where(u => !u.Name.Contains("Student")).ToList();

            foreach (var role in roles)
                list.Add(new SelectListItem() { Value = role.Name, Text = role.Name });
            ViewBag.Roles = list;

            var departments = _context.Departments.ToList();
            var viewModel = new NewUserViewModel
            {
                Departments = departments
            };
            ViewBag.Title = "Create";
            return View("NewProfessorForm", viewModel);
        }

        // POST: Professors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(NewUserViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var professor = new Professor();

                var user = new ApplicationUser
                {
                    UserName = viewModel.UserName,
                    Email = viewModel.Email,
                    Name = viewModel.Name,
                    DepartmentId = viewModel.DepartmentId,
                    Professor = professor

                };
                var result = await UserManager.CreateAsync(user, viewModel.Password);
                if (result.Succeeded)
                {

                    // ** use Role Name in "" for user role in place of model.role
                    await UserManager.AddToRoleAsync(user.Id, viewModel.Role);
                    return RedirectToAction("Index");
                }
            }
            List<SelectListItem> list = new List<SelectListItem>();
            var roles = _context.Roles.Where(u => !u.Name.Contains("Student")).ToList();

            foreach (var role in roles)
                list.Add(new SelectListItem() { Value = role.Name, Text = role.Name });
            ViewBag.Roles = list;

            var departments = _context.Departments.ToList();
            viewModel.Departments = departments;

            return View("NewProfessorForm", viewModel);
        }

        // GET: Professors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser professor = _context.Users.Where(u => u.ProfessorId == id).Include(d => d.Department).SingleOrDefault();

            if (professor == null)
            {
                return HttpNotFound();
            }
            List<SelectListItem> list = new List<SelectListItem>();
            var roles = _context.Roles.Where(u => !u.Name.Contains("Student")).ToList();

            foreach (var r in roles)
                list.Add(new SelectListItem() { Value = r.Name, Text = r.Name });
            ViewBag.Roles = list;

            var role = UserManager.GetRoles(professor.Id);
            var prof = new Professor
            {
                ProfessorId = (int)id
            };

            var viewModel = new UserViewModel
            {
                userId = professor.Id,
                Name = professor.Name,
                Email = professor.Email,
                UserName = professor.UserName,
                Department = professor.Department,
                DepartmentId = professor.DepartmentId,
                Role = role[0],
                Professor = prof,
                Departments = _context.Departments.ToList()
            };

            ViewBag.Title = "Edit";
            return View("ProfessorForm", viewModel);
        }

        // POST: Professors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var userInDb = _context.Users.Single(u => u.Id == viewModel.userId);

                userInDb.Id = viewModel.userId;
                userInDb.Email = viewModel.Email;
                userInDb.Name = viewModel.Name;
                userInDb.UserName = viewModel.UserName;
                userInDb.DepartmentId = viewModel.DepartmentId;

                _context.SaveChanges();


                return RedirectToAction("Index");
            }

            //            HttpNotFound("MODEL STATE IS NOT VALID");
            List<SelectListItem> list = new List<SelectListItem>();
            var roles = _context.Roles.Where(u => !u.Name.Contains("Student")).ToList();

            foreach (var role in roles)
                list.Add(new SelectListItem() { Value = role.Name, Text = role.Name });
            ViewBag.Roles = list;

            var departments = _context.Departments.ToList();
            viewModel.Departments = departments;

            return View("ProfessorForm", viewModel);
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
