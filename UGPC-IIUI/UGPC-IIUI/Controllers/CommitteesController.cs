using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UGPC_IIUI.Models;
using UGPC_IIUI.ViewModels;

namespace UGPC_IIUI.Controllers
{
    [Authorize(Roles = "Admin")]
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
        public ActionResult Index()
        {
            return View(_context.Committees.ToList());
        }

        // GET: Committees/Details/5
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

            var members = _context.CommitteeMembers.Where(u => u.CommitteeId == id).ToList();
            var user = _context.Users.Where(u => u.ProfessorId != null).ToList();
            var result = from
                            u in user
                         join
                             m in members
                         on
                             u.ProfessorId equals m.ProfessorId
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
                        viewModel.CommitteeMembers.Add(u,"Member");
                    }
                }
            }

            viewModel.Committee = committee;

            return View(viewModel);
        }

        // GET: Committees/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Committees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
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

        public ActionResult EditMember(string id)
        {
            var profId = Convert.ToInt32(id);

            var user = _context.Users.Single(u => u.ProfessorId == profId);
            var name = user.Name;

            List<SelectListItem> list = new List<SelectListItem>();


            var roles = _context.Roles.Where(r => r.Name.Equals("Committee Incharge") || r.Name.Equals("Committee Member"));

            foreach (var r in roles)
                list.Add(new SelectListItem() { Value = r.Name, Text = r.Name });
            ViewBag.Roles = list;
            var role="";
            var memberRole = UserManager.GetRoles(user.Id);
            foreach (var r in memberRole)
            {
                if (r.Equals("Committee Member") || r.Equals("Committee InCharge"))
                    role= r;
            }

            var viewModel = new CommitteeViewModel
            {
                Member = user,
                Role =role
            };

            return View(viewModel);
        }

        public ActionResult Save()
        {
            throw new NotImplementedException();
        }
    }
}
