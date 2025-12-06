using System;
using System.Linq;
using InquirySpark.Repository.Database;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InquirySpark.Admin.Areas.Inquiry.Controllers
{
    [Area("Inquiry")]
    public class SurveyStatusController : Controller
    {
        private readonly InquirySparkContext _context;

        public SurveyStatusController(InquirySparkContext context)
        {
            _context = context;
        }

        // GET: Inquiry/SurveyStatus
        public async Task<IActionResult> Index()
        {
            var inquirySparkContext = _context.SurveyStatuses.Include(s => s.Survey);
            return View(await inquirySparkContext.ToListAsync());
        }

        // GET: Inquiry/SurveyStatus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surveyStatus = await _context.SurveyStatuses
                .Include(s => s.Survey)
                .FirstOrDefaultAsync(m => m.SurveyStatusId == id);
            if (surveyStatus == null)
            {
                return NotFound();
            }

            return View(surveyStatus);
        }

        // GET: Inquiry/SurveyStatus/Create
        public IActionResult Create()
        {
            ViewData["SurveyId"] = new SelectList(_context.Surveys, "SurveyId", "CompletionMessage");
            return View();
        }

        // POST: Inquiry/SurveyStatus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SurveyStatusId,SurveyId,StatusId,StatusNm,StatusDs,EmailTemplate,EmailSubjectTemplate,PreviousStatusId,NextStatusId,ModifiedId,ModifiedDt")] SurveyStatus surveyStatus)
        {
            if (ModelState.IsValid)
            {
                _context.Add(surveyStatus);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SurveyId"] = new SelectList(_context.Surveys, "SurveyId", "CompletionMessage", surveyStatus.SurveyId);
            return View(surveyStatus);
        }

        // GET: Inquiry/SurveyStatus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surveyStatus = await _context.SurveyStatuses.FindAsync(id);
            if (surveyStatus == null)
            {
                return NotFound();
            }
            ViewData["SurveyId"] = new SelectList(_context.Surveys, "SurveyId", "CompletionMessage", surveyStatus.SurveyId);
            return View(surveyStatus);
        }

        // POST: Inquiry/SurveyStatus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SurveyStatusId,SurveyId,StatusId,StatusNm,StatusDs,EmailTemplate,EmailSubjectTemplate,PreviousStatusId,NextStatusId,ModifiedId,ModifiedDt")] SurveyStatus surveyStatus)
        {
            if (id != surveyStatus.SurveyStatusId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(surveyStatus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SurveyStatusExists(surveyStatus.SurveyStatusId))
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
            ViewData["SurveyId"] = new SelectList(_context.Surveys, "SurveyId", "CompletionMessage", surveyStatus.SurveyId);
            return View(surveyStatus);
        }

        // GET: Inquiry/SurveyStatus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surveyStatus = await _context.SurveyStatuses
                .Include(s => s.Survey)
                .FirstOrDefaultAsync(m => m.SurveyStatusId == id);
            if (surveyStatus == null)
            {
                return NotFound();
            }

            return View(surveyStatus);
        }

        // POST: Inquiry/SurveyStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var surveyStatus = await _context.SurveyStatuses.FindAsync(id);
            if (surveyStatus != null)
            {
                _context.SurveyStatuses.Remove(surveyStatus);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SurveyStatusExists(int id)
        {
            return _context.SurveyStatuses.Any(e => e.SurveyStatusId == id);
        }
    }
}
