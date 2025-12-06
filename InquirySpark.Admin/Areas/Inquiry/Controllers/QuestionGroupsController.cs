using System;
using System.Linq;
using InquirySpark.Repository.Database;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InquirySpark.Admin.Areas.Inquiry.Controllers
{
    [Area("Inquiry")]
    public class QuestionGroupsController : Controller
    {
        private readonly InquirySparkContext _context;

        public QuestionGroupsController(InquirySparkContext context)
        {
            _context = context;
        }

        // GET: Inquiry/QuestionGroups
        public async Task<IActionResult> Index()
        {
            var inquirySparkContext = _context.QuestionGroups.Include(q => q.Survey);
            return View(await inquirySparkContext.ToListAsync());
        }

        // GET: Inquiry/QuestionGroups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questionGroup = await _context.QuestionGroups
                .Include(q => q.Survey)
                .FirstOrDefaultAsync(m => m.QuestionGroupId == id);
            if (questionGroup == null)
            {
                return NotFound();
            }

            return View(questionGroup);
        }

        // GET: Inquiry/QuestionGroups/Create
        public IActionResult Create()
        {
            ViewData["SurveyId"] = new SelectList(_context.Surveys, "SurveyId", "CompletionMessage");
            return View();
        }

        // POST: Inquiry/QuestionGroups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("QuestionGroupId,SurveyId,GroupOrder,QuestionGroupShortNm,QuestionGroupNm,QuestionGroupDs,QuestionGroupWeight,GroupHeader,GroupFooter,ModifiedId,ModifiedDt,DependentQuestionGroupId,DependentMinScore,DependentMaxScore")] QuestionGroup questionGroup)
        {
            if (ModelState.IsValid)
            {
                _context.Add(questionGroup);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SurveyId"] = new SelectList(_context.Surveys, "SurveyId", "CompletionMessage", questionGroup.SurveyId);
            return View(questionGroup);
        }

        // GET: Inquiry/QuestionGroups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questionGroup = await _context.QuestionGroups.FindAsync(id);
            if (questionGroup == null)
            {
                return NotFound();
            }
            ViewData["SurveyId"] = new SelectList(_context.Surveys, "SurveyId", "CompletionMessage", questionGroup.SurveyId);
            return View(questionGroup);
        }

        // POST: Inquiry/QuestionGroups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("QuestionGroupId,SurveyId,GroupOrder,QuestionGroupShortNm,QuestionGroupNm,QuestionGroupDs,QuestionGroupWeight,GroupHeader,GroupFooter,ModifiedId,ModifiedDt,DependentQuestionGroupId,DependentMinScore,DependentMaxScore")] QuestionGroup questionGroup)
        {
            if (id != questionGroup.QuestionGroupId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(questionGroup);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionGroupExists(questionGroup.QuestionGroupId))
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
            ViewData["SurveyId"] = new SelectList(_context.Surveys, "SurveyId", "CompletionMessage", questionGroup.SurveyId);
            return View(questionGroup);
        }

        // GET: Inquiry/QuestionGroups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questionGroup = await _context.QuestionGroups
                .Include(q => q.Survey)
                .FirstOrDefaultAsync(m => m.QuestionGroupId == id);
            if (questionGroup == null)
            {
                return NotFound();
            }

            return View(questionGroup);
        }

        // POST: Inquiry/QuestionGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var questionGroup = await _context.QuestionGroups.FindAsync(id);
            if (questionGroup != null)
            {
                _context.QuestionGroups.Remove(questionGroup);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuestionGroupExists(int id)
        {
            return _context.QuestionGroups.Any(e => e.QuestionGroupId == id);
        }
    }
}
