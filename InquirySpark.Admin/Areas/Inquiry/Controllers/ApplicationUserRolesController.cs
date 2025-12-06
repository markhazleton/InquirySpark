using System;
using System.Linq;
using InquirySpark.Repository.Database;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InquirySpark.Admin.Areas.Inquiry.Controllers
{
    [Area("Inquiry")]
    public class ApplicationUserRolesController : Controller
    {
        private readonly InquirySparkContext _context;

        public ApplicationUserRolesController(InquirySparkContext context)
        {
            _context = context;
        }

        // GET: Inquiry/ApplicationUserRoles
        public async Task<IActionResult> Index()
        {
            var inquirySparkContext = _context.ApplicationUserRoles.Include(a => a.Application).Include(a => a.ApplicationUser).Include(a => a.Role);
            return View(await inquirySparkContext.ToListAsync());
        }

        // GET: Inquiry/ApplicationUserRoles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUserRole = await _context.ApplicationUserRoles
                .Include(a => a.Application)
                .Include(a => a.ApplicationUser)
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.ApplicationUserRoleId == id);
            if (applicationUserRole == null)
            {
                return NotFound();
            }

            return View(applicationUserRole);
        }

        // GET: Inquiry/ApplicationUserRoles/Create
        public IActionResult Create()
        {
            ViewData["ApplicationId"] = new SelectList(_context.Applications, "ApplicationId", "ApplicationCd");
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "ApplicationUserId", "AccountNm");
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleCd");
            return View();
        }

        // POST: Inquiry/ApplicationUserRoles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ApplicationUserRoleId,ApplicationId,ApplicationUserId,RoleId,ModifiedId,ModifiedDt,IsDemo,StartUpDate,IsMonthlyPrice,Price,UserInRolled,IsUserAdmin")] ApplicationUserRole applicationUserRole)
        {
            if (ModelState.IsValid)
            {
                _context.Add(applicationUserRole);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationId"] = new SelectList(_context.Applications, "ApplicationId", "ApplicationCd", applicationUserRole.ApplicationId);
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "ApplicationUserId", "AccountNm", applicationUserRole.ApplicationUserId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleCd", applicationUserRole.RoleId);
            return View(applicationUserRole);
        }

        // GET: Inquiry/ApplicationUserRoles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUserRole = await _context.ApplicationUserRoles.FindAsync(id);
            if (applicationUserRole == null)
            {
                return NotFound();
            }
            ViewData["ApplicationId"] = new SelectList(_context.Applications, "ApplicationId", "ApplicationCd", applicationUserRole.ApplicationId);
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "ApplicationUserId", "AccountNm", applicationUserRole.ApplicationUserId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleCd", applicationUserRole.RoleId);
            return View(applicationUserRole);
        }

        // POST: Inquiry/ApplicationUserRoles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ApplicationUserRoleId,ApplicationId,ApplicationUserId,RoleId,ModifiedId,ModifiedDt,IsDemo,StartUpDate,IsMonthlyPrice,Price,UserInRolled,IsUserAdmin")] ApplicationUserRole applicationUserRole)
        {
            if (id != applicationUserRole.ApplicationUserRoleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(applicationUserRole);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationUserRoleExists(applicationUserRole.ApplicationUserRoleId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationId"] = new SelectList(_context.Applications, "ApplicationId", "ApplicationCd", applicationUserRole.ApplicationId);
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "ApplicationUserId", "AccountNm", applicationUserRole.ApplicationUserId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleCd", applicationUserRole.RoleId);
            return View(applicationUserRole);
        }

        // GET: Inquiry/ApplicationUserRoles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUserRole = await _context.ApplicationUserRoles
                .Include(a => a.Application)
                .Include(a => a.ApplicationUser)
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.ApplicationUserRoleId == id);
            if (applicationUserRole == null)
            {
                return NotFound();
            }

            return View(applicationUserRole);
        }

        // POST: Inquiry/ApplicationUserRoles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var applicationUserRole = await _context.ApplicationUserRoles.FindAsync(id);
            if (applicationUserRole != null)
            {
                _context.ApplicationUserRoles.Remove(applicationUserRole);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApplicationUserRoleExists(int id)
        {
            return _context.ApplicationUserRoles.Any(e => e.ApplicationUserRoleId == id);
        }
    }
}
