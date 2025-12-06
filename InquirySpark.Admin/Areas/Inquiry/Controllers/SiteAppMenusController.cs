using System;
using System.Linq;
using InquirySpark.Repository.Database;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InquirySpark.Admin.Areas.Inquiry.Controllers
{
    [Area("Inquiry")]
    public class SiteAppMenusController : Controller
    {
        private readonly InquirySparkContext _context;

        public SiteAppMenusController(InquirySparkContext context)
        {
            _context = context;
        }

        // GET: Inquiry/SiteAppMenus
        public async Task<IActionResult> Index()
        {
            var inquirySparkContext = _context.SiteAppMenus.Include(s => s.SiteApp);
            return View(await inquirySparkContext.ToListAsync());
        }

        // GET: Inquiry/SiteAppMenus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var siteAppMenu = await _context.SiteAppMenus
                .Include(s => s.SiteApp)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (siteAppMenu == null)
            {
                return NotFound();
            }

            return View(siteAppMenu);
        }

        // GET: Inquiry/SiteAppMenus/Create
        public IActionResult Create()
        {
            ViewData["SiteAppId"] = new SelectList(_context.Applications, "ApplicationId", "ApplicationCd");
            return View();
        }

        // POST: Inquiry/SiteAppMenus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SiteAppId,MenuText,TartgetPage,GlyphName,MenuOrder,SiteRoleId,ViewInMenu")] SiteAppMenu siteAppMenu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(siteAppMenu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SiteAppId"] = new SelectList(_context.Applications, "ApplicationId", "ApplicationCd", siteAppMenu.SiteAppId);
            return View(siteAppMenu);
        }

        // GET: Inquiry/SiteAppMenus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var siteAppMenu = await _context.SiteAppMenus.FindAsync(id);
            if (siteAppMenu == null)
            {
                return NotFound();
            }
            ViewData["SiteAppId"] = new SelectList(_context.Applications, "ApplicationId", "ApplicationCd", siteAppMenu.SiteAppId);
            return View(siteAppMenu);
        }

        // POST: Inquiry/SiteAppMenus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SiteAppId,MenuText,TartgetPage,GlyphName,MenuOrder,SiteRoleId,ViewInMenu")] SiteAppMenu siteAppMenu)
        {
            if (id != siteAppMenu.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(siteAppMenu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SiteAppMenuExists(siteAppMenu.Id))
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
            ViewData["SiteAppId"] = new SelectList(_context.Applications, "ApplicationId", "ApplicationCd", siteAppMenu.SiteAppId);
            return View(siteAppMenu);
        }

        // GET: Inquiry/SiteAppMenus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var siteAppMenu = await _context.SiteAppMenus
                .Include(s => s.SiteApp)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (siteAppMenu == null)
            {
                return NotFound();
            }

            return View(siteAppMenu);
        }

        // POST: Inquiry/SiteAppMenus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var siteAppMenu = await _context.SiteAppMenus.FindAsync(id);
            if (siteAppMenu != null)
            {
                _context.SiteAppMenus.Remove(siteAppMenu);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SiteAppMenuExists(int id)
        {
            return _context.SiteAppMenus.Any(e => e.Id == id);
        }
    }
}
