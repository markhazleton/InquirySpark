using InquirySpark.Repository.Database;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InquirySpark.Admin.Areas.Inquiry.Controllers
{
    [Area("Inquiry")]
    public class ApplicationsController : Controller
    {
        private readonly InquirySparkContext _context;

        public ApplicationsController(InquirySparkContext context)
        {
            _context = context;
        }

        // GET: Inquiry/Applications
        public async Task<IActionResult> Index()
        {
            var inquirySparkContext = _context.Applications.Include(a => a.ApplicationType).Include(a => a.Company);
            return View(await inquirySparkContext.ToListAsync());
        }

        // GET: Inquiry/Applications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var application = await _context.Applications
                .Include(a => a.ApplicationType)
                .Include(a => a.Company)
                .FirstOrDefaultAsync(m => m.ApplicationId == id);
            if (application == null)
            {
                return NotFound();
            }

            return View(application);
        }

        // GET: Inquiry/Applications/Create
        public IActionResult Create()
        {
            ViewData["ApplicationTypeId"] = new SelectList(_context.LuApplicationTypes, "ApplicationTypeId", "ApplicationTypeNm");
            ViewData["CompanyId"] = new SelectList(_context.Companies, "CompanyId", "Address1");
            return View();
        }

        // POST: Inquiry/Applications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ApplicationId,ApplicationNm,ApplicationCd,ApplicationShortNm,ApplicationTypeId,ApplicationDs,MenuOrder,ApplicationFolder,DefaultPageId,CompanyId,ModifiedId,ModifiedDt")] Application application)
        {
            if (ModelState.IsValid)
            {
                _context.Add(application);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationTypeId"] = new SelectList(_context.LuApplicationTypes, "ApplicationTypeId", "ApplicationTypeNm", application.ApplicationTypeId);
            ViewData["CompanyId"] = new SelectList(_context.Companies, "CompanyId", "Address1", application.CompanyId);
            return View(application);
        }

        // GET: Inquiry/Applications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var application = await _context.Applications.FindAsync(id);
            if (application == null)
            {
                return NotFound();
            }
            ViewData["ApplicationTypeId"] = new SelectList(_context.LuApplicationTypes, "ApplicationTypeId", "ApplicationTypeNm", application.ApplicationTypeId);
            ViewData["CompanyId"] = new SelectList(_context.Companies, "CompanyId", "Address1", application.CompanyId);
            return View(application);
        }

        // POST: Inquiry/Applications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ApplicationId,ApplicationNm,ApplicationCd,ApplicationShortNm,ApplicationTypeId,ApplicationDs,MenuOrder,ApplicationFolder,DefaultPageId,CompanyId,ModifiedId,ModifiedDt")] Application application)
        {
            if (id != application.ApplicationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(application);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationExists(application.ApplicationId))
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
            ViewData["ApplicationTypeId"] = new SelectList(_context.LuApplicationTypes, "ApplicationTypeId", "ApplicationTypeNm", application.ApplicationTypeId);
            ViewData["CompanyId"] = new SelectList(_context.Companies, "CompanyId", "Address1", application.CompanyId);
            return View(application);
        }

        // GET: Inquiry/Applications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var application = await _context.Applications
                .Include(a => a.ApplicationType)
                .Include(a => a.Company)
                .FirstOrDefaultAsync(m => m.ApplicationId == id);
            if (application == null)
            {
                return NotFound();
            }

            return View(application);
        }

        // POST: Inquiry/Applications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var application = await _context.Applications.FindAsync(id);
            if (application != null)
            {
                _context.Applications.Remove(application);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApplicationExists(int id)
        {
            return _context.Applications.Any(e => e.ApplicationId == id);
        }
    }
}
