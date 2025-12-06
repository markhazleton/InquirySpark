using System;
using System.Linq;
using InquirySpark.Repository.Database;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InquirySpark.Admin.Areas.Inquiry.Controllers
{
    [Area("Inquiry")]
    public class AppPropertiesController : Controller
    {
        private readonly InquirySparkContext _context;

        public AppPropertiesController(InquirySparkContext context)
        {
            _context = context;
        }

        // GET: Inquiry/AppProperties
        public async Task<IActionResult> Index()
        {
            var inquirySparkContext = _context.AppProperties.Include(a => a.SiteApp);
            return View(await inquirySparkContext.ToListAsync());
        }

        // GET: Inquiry/AppProperties/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appProperty = await _context.AppProperties
                .Include(a => a.SiteApp)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appProperty == null)
            {
                return NotFound();
            }

            return View(appProperty);
        }

        // GET: Inquiry/AppProperties/Create
        public IActionResult Create()
        {
            ViewData["SiteAppId"] = new SelectList(_context.Applications, "ApplicationId", "ApplicationCd");
            return View();
        }

        // POST: Inquiry/AppProperties/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SiteAppId,Key,Value")] AppProperty appProperty)
        {
            if (ModelState.IsValid)
            {
                _context.Add(appProperty);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SiteAppId"] = new SelectList(_context.Applications, "ApplicationId", "ApplicationCd", appProperty.SiteAppId);
            return View(appProperty);
        }

        // GET: Inquiry/AppProperties/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appProperty = await _context.AppProperties.FindAsync(id);
            if (appProperty == null)
            {
                return NotFound();
            }
            ViewData["SiteAppId"] = new SelectList(_context.Applications, "ApplicationId", "ApplicationCd", appProperty.SiteAppId);
            return View(appProperty);
        }

        // POST: Inquiry/AppProperties/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SiteAppId,Key,Value")] AppProperty appProperty)
        {
            if (id != appProperty.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appProperty);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppPropertyExists(appProperty.Id))
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
            ViewData["SiteAppId"] = new SelectList(_context.Applications, "ApplicationId", "ApplicationCd", appProperty.SiteAppId);
            return View(appProperty);
        }

        // GET: Inquiry/AppProperties/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appProperty = await _context.AppProperties
                .Include(a => a.SiteApp)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appProperty == null)
            {
                return NotFound();
            }

            return View(appProperty);
        }

        // POST: Inquiry/AppProperties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appProperty = await _context.AppProperties.FindAsync(id);
            if (appProperty != null)
            {
                _context.AppProperties.Remove(appProperty);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppPropertyExists(int id)
        {
            return _context.AppProperties.Any(e => e.Id == id);
        }
    }
}
