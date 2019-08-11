using Microsoft.AspNet.Identity;
using System.Collections.Generic;
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
		[Authorize(Roles = "Admin")]
		public ActionResult Index()
		{
			var groups = _context.Groups.Include(a => a.Student1).Include(a => a.Student2).ToList();
			return View(groups);
		}


		// GET: Groups/Create
		public ActionResult Create()
		{
			
			var users = _context.Users.Where(u => u.StudentId != null && u.ProfessorId == null && u.Joined==false).ToList();

			var viewModel = new GroupViewModel
			{
				Users = users,
			};
			if (User.IsInRole("Student"))
			{
				var id = User.Identity.GetUserId();
				var student = _context.Users.Single(s => s.Id == id);
				viewModel.Student1 = student;
			}
				

			return View(viewModel);
		}

		// POST: Groups/Create
		[HttpPost]
		public ActionResult Create(FormCollection collection)
		{
			try
			{
				// TODO: Add insert logic here

				return RedirectToAction("Index");
			}
			catch
			{
				return View();
			}
		}

		// GET: Groups/Edit/5
		public ActionResult Edit(int id)
		{
			return View();
		}

		// POST: Groups/Edit/5
		[HttpPost]
		public ActionResult Edit(int id, FormCollection collection)
		{
			try
			{
				// TODO: Add update logic here

				return RedirectToAction("Index");
			}
			catch
			{
				return View();
			}
		}

		// GET: Groups/Delete/5
		public ActionResult Delete(int id)
		{
			return View();
		}

		// POST: Groups/Delete/5
		[HttpPost]
		public ActionResult Delete(int id, FormCollection collection)
		{
			try
			{
				// TODO: Add delete logic here

				return RedirectToAction("Index");
			}
			catch
			{
				return View();
			}
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
