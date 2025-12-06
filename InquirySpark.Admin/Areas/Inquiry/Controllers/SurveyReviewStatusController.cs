using System;
using System.Linq;
using InquirySpark.Repository.Database;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InquirySpark.Admin.Areas.Inquiry.Controllers
{
    [Area("Inquiry")]
    public class SurveyReviewStatusController : Controller
    {
        private readonly InquirySparkContext _context;

        public SurveyReviewStatusController(InquirySparkContext context)
        {
            _context = context;
        }

        // GET: Inquiry/SurveyReviewStatus
        public async Task<IActionResult> Index()
        {
            var inquirySparkContext = _context.SurveyReviewStatuses.Include(s => s.Survey);
            return View(await inquirySparkContext.ToListAsync());
        }

        // GET: Inquiry/SurveyReviewStatus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surveyReviewStatus = await _context.SurveyReviewStatuses
                .Include(s => s.Survey)
                .FirstOrDefaultAsync(m => m.SurveyReviewStatusId == id);
            if (surveyReviewStatus == null)
            {
                return NotFound();
            }

            return View(surveyReviewStatus);
        }

        // GET: Inquiry/SurveyReviewStatus/Create
        public IActionResult Create()
        {
            ViewData["SurveyId"] = new SelectList(_context.Surveys, "SurveyId", "CompletionMessage");
            return View();
        }

        // POST: Inquiry/SurveyReviewStatus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SurveyReviewStatusId,SurveyId,ReviewStatusId,ReviewStatusNm,ReviewStatusDs,ApprovedFl,CommentFl,ModifiedId,ModifiedDt")] SurveyReviewStatus surveyReviewStatus)
        {
            if (ModelState.IsValid)
            {
                _context.Add(surveyReviewStatus);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SurveyId"] = new SelectList(_context.Surveys, "SurveyId", "CompletionMessage", surveyReviewStatus.SurveyId);
            return View(surveyReviewStatus);
        }

        // GET: Inquiry/SurveyReviewStatus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surveyReviewStatus = await _context.SurveyReviewStatuses.FindAsync(id);
            if (surveyReviewStatus == null)
            {
                return NotFound();
            }
            ViewData["SurveyId"] = new SelectList(_context.Surveys, "SurveyId", "CompletionMessage", surveyReviewStatus.SurveyId);
            return View(surveyReviewStatus);
        }

        // POST: Inquiry/SurveyReviewStatus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SurveyReviewStatusId,SurveyId,ReviewStatusId,ReviewStatusNm,ReviewStatusDs,ApprovedFl,CommentFl,ModifiedId,ModifiedDt")] SurveyReviewStatus surveyReviewStatus)
        {
            if (id != surveyReviewStatus.SurveyReviewStatusId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(surveyReviewStatus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SurveyReviewStatusExists(surveyReviewStatus.SurveyReviewStatusId))
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
            ViewData["SurveyId"] = new SelectList(_context.Surveys, "SurveyId", "CompletionMessage", surveyReviewStatus.SurveyId);
            return View(surveyReviewStatus);
        }

        // GET: Inquiry/SurveyReviewStatus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surveyReviewStatus = await _context.SurveyReviewStatuses
                .Include(s => s.Survey)
                .FirstOrDefaultAsync(m => m.SurveyReviewStatusId == id);
            if (surveyReviewStatus == null)
            {
                return NotFound();
            }

            return View(surveyReviewStatus);
        }

        // POST: Inquiry/SurveyReviewStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var surveyReviewStatus = await _context.SurveyReviewStatuses.FindAsync(id);
            if (surveyReviewStatus != null)
            {
                _context.SurveyReviewStatuses.Remove(surveyReviewStatus);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SurveyReviewStatusExists(int id)
        {
            return _context.SurveyReviewStatuses.Any(e => e.SurveyReviewStatusId == id);
        }
    }
}
