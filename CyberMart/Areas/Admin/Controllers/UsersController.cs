using System.Security.Claims;
using CyberMart.DataAccess.Contexts;
using CyberMart.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CyberMart.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(Roles = SD.AdminRole)]
    public class UsersController : Controller
    {
        private readonly CyberMartDbContext context;

        public UsersController(CyberMartDbContext _context)
        {
            context = _context;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim.Value;
            var Users = context.applicationUsers.Where(U => U.Id != userId).ToList(); 
            return View(Users);
        }

        public IActionResult LockUnlock(string? id)
        {
          var user = context.applicationUsers.FirstOrDefault(U => U.Id == id);
            if (user == null)
                return NotFound();

            if(user.LockoutEnd == null || user.LockoutEnd < DateTime.Now)
            {
                user.LockoutEnd = DateTime.Now.AddHours(4);
            }
            else
            {
                user.LockoutEnd = DateTime.Now;
            }
            context.SaveChanges();
            return RedirectToAction("Index", "Users", new {area = "Admin"});
        }

	}
}
