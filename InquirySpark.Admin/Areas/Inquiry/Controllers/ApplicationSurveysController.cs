using System;
using System.Linq;
using InquirySpark.Repository.Database;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InquirySpark.Admin.Areas.Inquiry.Controllers
{
    [Area("Inquiry")]
    public class ApplicationSurveysController : Controller
    {
        private readonly InquirySparkContext _context;

        public ApplicationSurveysController(InquirySparkContext context)
        {
            _context = context;
        }

        // GET: Inquiry/ApplicationSurveys
        public async Task<IActionResult> Index()
        {
            var inquirySparkContext = _context.ApplicationSurveys.Include(a => a.Application).Include(a => a.DefaultRole).Include(a => a.Survey);
            return View(await inquirySparkContext.ToListAsync());
        }

        // GET: Inquiry/ApplicationSurveys/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationSurvey = await _context.ApplicationSurveys
                .Include(a => a.Application)
                .Include(a => a.DefaultRole)
                .Include(a => a.Survey)
                .FirstOrDefaultAsync(m => m.ApplicationSurveyId == id);
            if (applicationSurvey == null)
            {
                return NotFound();
            }

            return View(applicationSurvey);
        }

        // GET: Inquiry/ApplicationSurveys/Create
        public IActionResult Create()
        {
            ViewData["ApplicationId"] = new SelectList(_context.Applications, "ApplicationId", "ApplicationCd");
            ViewData["DefaultRoleId"] = new SelectList(_context.Roles, "RoleId", "RoleCd");
            ViewData["SurveyId"] = new SelectList(_context.Surveys, "SurveyId", "CompletionMessage");
            return View();
        }

        // POST: Inquiry/ApplicationSurveys/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ApplicationSurveyId,ApplicationId,SurveyId,DefaultRoleId,ModifiedId,ModifiedDt")] ApplicationSurvey applicationSurvey)
        {
            if (ModelState.IsValid)
            {
                _context.Add(applicationSurvey);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationId"] = new SelectList(_context.Applications, "ApplicationId", "ApplicationCd", applicationSurvey.ApplicationId);
            ViewData["DefaultRoleId"] = new SelectList(_context.Roles, "RoleId", "RoleCd", applicationSurvey.DefaultRoleId);
            ViewData["SurveyId"] = new SelectList(_context.Surveys, "SurveyId", "CompletionMessage", applicationSurvey.SurveyId);
            return View(applicationSurvey);
        }

        // GET: Inquiry/ApplicationSurveys/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationSurvey = await _context.ApplicationSurveys.FindAsync(id);
            if (applicationSurvey == null)
            {
                return NotFound();
            }
            ViewData["ApplicationId"] = new SelectList(_context.Applications, "ApplicationId", "ApplicationCd", applicationSurvey.ApplicationId);
            ViewData["DefaultRoleId"] = new SelectList(_context.Roles, "RoleId", "RoleCd", applicationSurvey.DefaultRoleId);
            ViewData["SurveyId"] = new SelectList(_context.Surveys, "SurveyId", "CompletionMessage", applicationSurvey.SurveyId);
            return View(applicationSurvey);
        }

        // POST: Inquiry/ApplicationSurveys/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ApplicationSurveyId,ApplicationId,SurveyId,DefaultRoleId,ModifiedId,ModifiedDt")] ApplicationSurvey applicationSurvey)
        {
            if (id != applicationSurvey.ApplicationSurveyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(applicationSurvey);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationSurveyExists(applicationSurvey.ApplicationSurveyId))
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
            ViewData["ApplicationId"] = new SelectList(_context.Applications, "ApplicationId", "ApplicationCd", applicationSurvey.ApplicationId);
            ViewData["DefaultRoleId"] = new SelectList(_context.Roles, "RoleId", "RoleCd", applicationSurvey.DefaultRoleId);
            ViewData["SurveyId"] = new SelectList(_context.Surveys, "SurveyId", "CompletionMessage", applicationSurvey.SurveyId);
            return View(applicationSurvey);
        }

        // GET: Inquiry/ApplicationSurveys/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationSurvey = await _context.ApplicationSurveys
                .Include(a => a.Application)
                .Include(a => a.DefaultRole)
                .Include(a => a.Survey)
                .FirstOrDefaultAsync(m => m.ApplicationSurveyId == id);
            if (applicationSurvey == null)
            {
                return NotFound();
            }

            return View(applicationSurvey);
        }

        // POST: Inquiry/ApplicationSurveys/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var applicationSurvey = await _context.ApplicationSurveys.FindAsync(id);
            if (applicationSurvey != null)
            {
                _context.ApplicationSurveys.Remove(applicationSurvey);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApplicationSurveyExists(int id)
        {
            return _context.ApplicationSurveys.Any(e => e.ApplicationSurveyId == id);
        }
    }
}
