using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using UGPC_IIUI.Models;
using UGPC_IIUI.ViewModels;

namespace UGPC_IIUI.Controllers
{
    public class GroupsController : Controller
    {
        public ApplicationDbContext _context = new ApplicationDbContext();
        // GET: Groups
        [Authorize(Roles = "Admin,Student")]
        public ActionResult Index()
        {
            if (User.IsInRole("Student"))
                return RedirectToAction("MyIndex");
            var groups = _context.Groups.Include(a => a.Student1).Include(a => a.Student2).ToList();
            return View(groups);
        }


        // GET: Groups/Create
        public ActionResult Create()
        {

            var users = _context.Users.Where(u => u.StudentId != null && u.ProfessorId == null && u.Joined == false).ToList();

            var viewModel = new GroupViewModel
            {
                Users = users,
            };
            if (User.IsInRole("Student"))
            {
                var id = User.Identity.GetUserId();
                var student = _context.Users.Single(s => s.Id == id);
                viewModel.Student1Id = id;
                viewModel.Student1 = student;
            }


            return View("GroupForm", viewModel);
        }

        // POST: Groups/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GroupViewModel viewModel)
        {
            try
            {
                var group = new Group
                {
                    Student1Id = viewModel.Student1Id,
                    Student2Id = viewModel.Student2Id
                };

                _context.Groups.Add(group);
                var s1 = _context.Users.Single(u => u.Id == viewModel.Student1Id);
                s1.Joined = true;
                var s2 = _context.Users.Single(u => u.Id == viewModel.Student2Id);
                s2.Joined = true;

                _context.SaveChanges();
                if(User.IsInRole("Student"))
                return RedirectToAction("MyIndex");
                return RedirectToAction("Index");
            }
            catch
            {
                return View("GroupForm",viewModel);
            }
        }


        // GET: Groups/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            var group = _context.Groups.Include(s => s.Student1).Include(s => s.Student2).Single(g => g.Id == id);
            return View(group);
        }

        // POST: Groups/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Group group)
        {
            Group groupInDb = _context.Groups.Find(group.Id);
            if (groupInDb != null)
            {
                _context.Groups.Remove(groupInDb);
                var s1 = (_context.Users.Single(u => u.Id == group.Student1Id));
                s1.Joined = false;
                var s2 = (_context.Users.Single(u => u.Id == group.Student2Id));
                s2.Joined = false;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return HttpNotFound("Object Not Found in DB");
        }
            
        

        [Authorize(Roles = "Student")]
        public ActionResult MyIndex()
        {
            var id = User.Identity.GetUserId();
            var groups = _context.Groups.Include(a => a.Student1).Include(a => a.Student2).Where(s => s.Student1Id == id || s.Student2Id == id).ToList();

            return View("Index", groups);
        }
    }
}
