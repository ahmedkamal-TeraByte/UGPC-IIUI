using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using UGPC_IIUI.Models;
using UGPC_IIUI.ViewModels;

namespace UGPC_IIUI.Controllers
{
    public class CommitteesController : Controller
    {
        private ApplicationDbContext _context = new ApplicationDbContext();
        private ApplicationUserManager _userManager;


        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }

        // GET: Committees

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View(_context.Committees.ToList());
        }

        // GET: Committees/Details/5
        [Authorize(Roles = "Admin,Supervisor")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Committee committee = _context.Committees.Find(id);
            if (committee == null)
            {
                return HttpNotFound();
            }

            var viewModel = new CommitteeViewModel();

            var members = _context.CommitteeMembers.Where(u => u.CommitteeId == id).Include(u => u.User).ToList();
            var user = _context.Users.Where(u => u.ProfessorId != null).ToList();
            var result = from
                    u in user
                         join
                             m in members
                             on
                             u.ProfessorId equals m.User.ProfessorId
                         select u;
            foreach (var u in result)
            {

                var role = UserManager.GetRoles(u.Id);

                foreach (var r in role)
                {

                    if (r.Equals("Committee Incharge"))
                    {
                        viewModel.CommitteeMembers.Add(u, "Incharge");
                    }
                    else if (r.Equals("Committee Member"))
                    {
                        viewModel.CommitteeMembers.Add(u, "Member");
                    }
                }
            }

            viewModel.Committee = committee;

            return View(viewModel);
        }

        // GET: Committees/Create

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Committees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CommitteeId,Name")] Committee committee)
        {
            if (ModelState.IsValid)
            {
                _context.Committees.Add(committee);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(committee);
        }

        [Authorize(Roles = "Admin")]
        // GET: Committees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Committee committee = _context.Committees.Find(id);
            if (committee == null)
            {
                return HttpNotFound();
            }

            return View(committee);
        }

        // POST: Committees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CommitteeId,Name")] Committee committee)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(committee).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(committee);
        }


        // GET: Committees/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Committee committee = _context.Committees.Find(id);
            if (committee == null)
            {
                return HttpNotFound();
            }

            return View(committee);
        }



        // POST: Committees/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Committee committee = _context.Committees.Find(id);
            _context.Committees.Remove(committee);
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


        [Authorize(Roles = "Admin,Committee Incharge")]
        public ActionResult EditMember(string id)
        {
            var profId = Convert.ToInt32(id);

            var user = _context.Users.Single(u => u.ProfessorId == profId);
            var name = user.Name;

            var membership = _context.CommitteeMembers.Single(c => c.UserId == user.Id);
            var comId = membership.CommitteeId;

            List<SelectListItem> list = new List<SelectListItem>();


            var roles = _context.Roles.Where(r =>
                r.Name.Equals("Committee Incharge") || r.Name.Equals("Committee Member"));

            foreach (var r in roles)
                list.Add(new SelectListItem() { Value = r.Name, Text = r.Name });
            ViewBag.Roles = list;
            var role = "";
            var memberRole = UserManager.GetRoles(user.Id);
            foreach (var r in memberRole)
            {
                if (r.Equals("Committee Member") || r.Equals("Committee Incharge"))
                    role = r;
            }

            var viewModel = new CommitteeViewModel
            {
                Member = user,
                Role = role,
                committeeId = comId
            };

            return View(viewModel);
        }


        [Authorize(Roles = "Admin, Committee Incharge")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMember(CommitteeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.Single(u => u.ProfessorId == viewModel.Member.ProfessorId);

                var currentRole = "";
                var memberRole = UserManager.GetRoles(user.Id);
                foreach (var r in memberRole)
                {
                    if (r.Equals("Committee Member") || r.Equals("Committee Incharge"))
                        currentRole = r;
                }

                var result = UserManager.RemoveFromRole(user.Id, currentRole);
                if (result == IdentityResult.Success)
                {
                    result = UserManager.AddToRole(user.Id, viewModel.Role);
                    if (result == IdentityResult.Success)
                        return RedirectToAction("Details", new { id = viewModel.committeeId });
                    else
                        return HttpNotFound("Can't add new Role");
                }
                else
                    return HttpNotFound("Can't Remove Existing Role");
            }

            return HttpNotFound("Model State is not Valid");

        }


        [Authorize(Roles = "Admin, Committee Incharge")]
        public ActionResult AddNewMember(int committeeId)
        {
            var users = _context.Users.Where(u => u.ProfessorId != null & u.StudentId == null).ToList();
            var committeeMembers = _context.CommitteeMembers.ToList();

            var result = from u in users
                         join m in committeeMembers
                             on u.Id equals m.UserId into subResult
                         from sr in subResult.DefaultIfEmpty()
                         select new
                         {
                             u,
                             cid = sr?.CommitteeId ?? null
                         };

            IList<ApplicationUser> names = new List<ApplicationUser>();
            foreach (var user in result)
            {
                if (user.cid == null)
                    names.Add(user.u);
            }

            ViewBag.MembersList = names;


            List<SelectListItem> list = new List<SelectListItem>();
            var roles = _context.Roles.Where(r =>
                r.Name.Equals("Committee Incharge") || r.Name.Equals("Committee Member"));
            foreach (var r in roles)
                list.Add(new SelectListItem() { Value = r.Name, Text = r.Name });
            ViewBag.Roles = list;

            var viewModel = new CommitteeMemberViewModel()
            {
                CommitteeId = committeeId
            };


            return View(viewModel);
        }


        [Authorize(Roles = "Admin, Committee Incharge")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddNewMember(CommitteeMemberViewModel viewModel)
        {
            var committeeMember = new CommitteeMember
            {
                CommitteeId = viewModel.CommitteeId,
                UserId = viewModel.MemberId
            };

             _context.CommitteeMembers.Add(committeeMember);
             _context.SaveChanges();
              var result = UserManager.AddToRole(viewModel.MemberId, viewModel.Role);
             if (result == IdentityResult.Success)
                 return RedirectToAction("Details", new { id = viewModel.CommitteeId });
             else
                 return HttpNotFound("Can't add new Role");
        }


        [Authorize(Roles = "Admin, Committee Incharge")]
        public ActionResult DeleteMember(int? id)
        {
            var user = _context.Users.Single(u => u.ProfessorId == id);

            var role = "";
            var memberRole = UserManager.GetRoles(user.Id);
            foreach (var r in memberRole)
            {
                if (r.Equals("Committee Member") || r.Equals("Committee Incharge"))
                    role = r;
            }

            var committeeId = (_context.CommitteeMembers.Single(m => m.UserId == user.Id)).CommitteeId;
            var viewModel = new CommitteeMemberViewModel
            {
                Member = user,
                MemberId = user.Id,
                Role = role,
                CommitteeId = committeeId
            };
            return View(viewModel);
        }


        [Authorize(Roles = "Admin, Committee Incharge")]
        [HttpPost, ActionName("DeleteMember")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteMemberConfirmed(CommitteeMemberViewModel viewModel)
        {


            var result = UserManager.RemoveFromRole(viewModel.MemberId, viewModel.Role);
            var membership = _context.CommitteeMembers.Single(m => m.UserId == viewModel.MemberId);
            if (membership != null)
            {
                _context.CommitteeMembers.Remove(membership);
                _context.SaveChanges();
            }
            else
                return HttpNotFound("Membership Not Found");
//            return Content(id.ToString());
            return RedirectToAction("Details",new {id=viewModel.CommitteeId});
        }


        [Authorize(Roles = "Supervisor")]
        public ActionResult MyIndex()
        {
            var userId = User.Identity.GetUserId();
            var committeeId = (_context.CommitteeMembers.Single(m => m.UserId == userId)).CommitteeId;
            return View(_context.Committees.Where(c=> c.CommitteeId==committeeId).ToList());
        }
    }
}
