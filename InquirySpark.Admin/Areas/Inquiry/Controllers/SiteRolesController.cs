using System;
using System.Linq;
using InquirySpark.Repository.Database;
using Microsoft.EntityFrameworkCore;

namespace InquirySpark.Admin.Areas.Inquiry.Controllers
{
    [Area("Inquiry")]
    public class SiteRolesController : Controller
    {
        private readonly InquirySparkContext _context;

        public SiteRolesController(InquirySparkContext context)
        {
            _context = context;
        }

        // GET: Inquiry/SiteRoles
        public async Task<IActionResult> Index()
        {
            return View(await _context.SiteRoles.ToListAsync());
        }

        // GET: Inquiry/SiteRoles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var siteRole = await _context.SiteRoles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (siteRole == null)
            {
                return NotFound();
            }

            return View(siteRole);
        }

        // GET: Inquiry/SiteRoles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Inquiry/SiteRoles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RoleName,Active")] SiteRole siteRole)
        {
            if (ModelState.IsValid)
            {
                _context.Add(siteRole);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(siteRole);
        }

        // GET: Inquiry/SiteRoles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var siteRole = await _context.SiteRoles.FindAsync(id);
            if (siteRole == null)
            {
                return NotFound();
            }
            return View(siteRole);
        }

        // POST: Inquiry/SiteRoles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RoleName,Active")] SiteRole siteRole)
        {
            if (id != siteRole.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(siteRole);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SiteRoleExists(siteRole.Id))
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
            return View(siteRole);
        }

        // GET: Inquiry/SiteRoles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var siteRole = await _context.SiteRoles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (siteRole == null)
            {
                return NotFound();
            }

            return View(siteRole);
        }

        // POST: Inquiry/SiteRoles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var siteRole = await _context.SiteRoles.FindAsync(id);
            if (siteRole != null)
            {
                _context.SiteRoles.Remove(siteRole);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SiteRoleExists(int id)
        {
            return _context.SiteRoles.Any(e => e.Id == id);
        }
    }
}
