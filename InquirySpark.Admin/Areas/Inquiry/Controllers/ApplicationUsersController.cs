using InquirySpark.Repository.Database;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InquirySpark.Admin.Areas.Inquiry.Controllers
{
    [Area("Inquiry")]
    public class ApplicationUsersController : Controller
    {
        private readonly InquirySparkContext _context;

        public ApplicationUsersController(InquirySparkContext context)
        {
            _context = context;
        }

        // GET: Inquiry/ApplicationUsers
        public async Task<IActionResult> Index()
        {
            var inquirySparkContext = _context.ApplicationUsers.Include(a => a.Company).Include(a => a.Role);
            return View(await inquirySparkContext.ToListAsync());
        }

        // GET: Inquiry/ApplicationUsers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.ApplicationUsers
                .Include(a => a.Company)
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.ApplicationUserId == id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            return View(applicationUser);
        }

        // GET: Inquiry/ApplicationUsers/Create
        public IActionResult Create()
        {
            ViewData["CompanyId"] = new SelectList(_context.Companies, "CompanyId", "Address1");
            ViewData["RoleId"] = new SelectList(_context.SiteRoles, "Id", "RoleName");
            return View();
        }

        // POST: Inquiry/ApplicationUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ApplicationUserId,FirstNm,LastNm,EMailAddress,CommentDs,AccountNm,SupervisorAccountNm,LastLoginDt,LastLoginLocation,DisplayName,Password,RoleId,UserKey,UserLogin,EmailVerified,VerifyCode,CompanyId,ModifiedId,ModifiedDt")] ApplicationUser applicationUser)
        {
            if (ModelState.IsValid)
            {
                _context.Add(applicationUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "CompanyId", "Address1", applicationUser.CompanyId);
            ViewData["RoleId"] = new SelectList(_context.SiteRoles, "Id", "RoleName", applicationUser.RoleId);
            return View(applicationUser);
        }

        // GET: Inquiry/ApplicationUsers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.ApplicationUsers.FindAsync(id);
            if (applicationUser == null)
            {
                return NotFound();
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "CompanyId", "Address1", applicationUser.CompanyId);
            ViewData["RoleId"] = new SelectList(_context.SiteRoles, "Id", "RoleName", applicationUser.RoleId);
            return View(applicationUser);
        }

        // POST: Inquiry/ApplicationUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ApplicationUserId,FirstNm,LastNm,EMailAddress,CommentDs,AccountNm,SupervisorAccountNm,LastLoginDt,LastLoginLocation,DisplayName,Password,RoleId,UserKey,UserLogin,EmailVerified,VerifyCode,CompanyId,ModifiedId,ModifiedDt")] ApplicationUser applicationUser)
        {
            if (id != applicationUser.ApplicationUserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(applicationUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationUserExists(applicationUser.ApplicationUserId))
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
            ViewData["CompanyId"] = new SelectList(_context.Companies, "CompanyId", "Address1", applicationUser.CompanyId);
            ViewData["RoleId"] = new SelectList(_context.SiteRoles, "Id", "RoleName", applicationUser.RoleId);
            return View(applicationUser);
        }

        // GET: Inquiry/ApplicationUsers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.ApplicationUsers
                .Include(a => a.Company)
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.ApplicationUserId == id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            return View(applicationUser);
        }

        // POST: Inquiry/ApplicationUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var applicationUser = await _context.ApplicationUsers.FindAsync(id);
            if (applicationUser != null)
            {
                _context.ApplicationUsers.Remove(applicationUser);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApplicationUserExists(int id)
        {
            return _context.ApplicationUsers.Any(e => e.ApplicationUserId == id);
        }
    }
}
