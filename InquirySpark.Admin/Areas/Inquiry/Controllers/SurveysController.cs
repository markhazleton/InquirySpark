using System;
using System.Linq;
using InquirySpark.Repository.Database;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InquirySpark.Admin.Areas.Inquiry.Controllers
{
    [Area("Inquiry")]
    public class SurveysController : Controller
    {
        private readonly InquirySparkContext _context;

        public SurveysController(InquirySparkContext context)
        {
            _context = context;
        }

        // GET: Inquiry/Surveys
        public async Task<IActionResult> Index()
        {
            var inquirySparkContext = _context.Surveys.Include(s => s.SurveyType);
            return View(await inquirySparkContext.ToListAsync());
        }

        // GET: Inquiry/Surveys/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var survey = await _context.Surveys
                .Include(s => s.SurveyType)
                .FirstOrDefaultAsync(m => m.SurveyId == id);
            if (survey == null)
            {
                return NotFound();
            }

            return View(survey);
        }

        // GET: Inquiry/Surveys/Create
        public IActionResult Create()
        {
            ViewData["SurveyTypeId"] = new SelectList(_context.LuSurveyTypes, "SurveyTypeId", "SurveyTypeNm");
            return View();
        }

        // POST: Inquiry/Surveys/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SurveyId,SurveyTypeId,UseQuestionGroupsFl,SurveyNm,SurveyShortNm,SurveyDs,CompletionMessage,ResponseNmtemplate,ReviewerAccountNm,AutoAssignFilter,StartDt,EndDt,ParentSurveyId,ModifiedId,ModifiedDt")] Survey survey)
        {
            if (ModelState.IsValid)
            {
                _context.Add(survey);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SurveyTypeId"] = new SelectList(_context.LuSurveyTypes, "SurveyTypeId", "SurveyTypeNm", survey.SurveyTypeId);
            return View(survey);
        }

        // GET: Inquiry/Surveys/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var survey = await _context.Surveys.FindAsync(id);
            if (survey == null)
            {
                return NotFound();
            }
            ViewData["SurveyTypeId"] = new SelectList(_context.LuSurveyTypes, "SurveyTypeId", "SurveyTypeNm", survey.SurveyTypeId);
            return View(survey);
        }

        // POST: Inquiry/Surveys/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SurveyId,SurveyTypeId,UseQuestionGroupsFl,SurveyNm,SurveyShortNm,SurveyDs,CompletionMessage,ResponseNmtemplate,ReviewerAccountNm,AutoAssignFilter,StartDt,EndDt,ParentSurveyId,ModifiedId,ModifiedDt")] Survey survey)
        {
            if (id != survey.SurveyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(survey);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SurveyExists(survey.SurveyId))
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
            ViewData["SurveyTypeId"] = new SelectList(_context.LuSurveyTypes, "SurveyTypeId", "SurveyTypeNm", survey.SurveyTypeId);
            return View(survey);
        }

        // GET: Inquiry/Surveys/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var survey = await _context.Surveys
                .Include(s => s.SurveyType)
                .FirstOrDefaultAsync(m => m.SurveyId == id);
            if (survey == null)
            {
                return NotFound();
            }

            return View(survey);
        }

        // POST: Inquiry/Surveys/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var survey = await _context.Surveys.FindAsync(id);
            if (survey != null)
            {
                _context.Surveys.Remove(survey);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SurveyExists(int id)
        {
            return _context.Surveys.Any(e => e.SurveyId == id);
        }
    }
}
