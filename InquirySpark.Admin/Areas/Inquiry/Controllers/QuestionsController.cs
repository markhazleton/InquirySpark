using System;
using System.Linq;
using InquirySpark.Repository.Database;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InquirySpark.Admin.Areas.Inquiry.Controllers
{
    [Area("Inquiry")]
    public class QuestionsController : Controller
    {
        private readonly InquirySparkContext _context;

        public QuestionsController(InquirySparkContext context)
        {
            _context = context;
        }

        // GET: Inquiry/Questions
        public async Task<IActionResult> Index()
        {
            var inquirySparkContext = _context.Questions.Include(q => q.QuestionType).Include(q => q.SurveyType).Include(q => q.UnitOfMeasure);
            return View(await inquirySparkContext.ToListAsync());
        }

        // GET: Inquiry/Questions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _context.Questions
                .Include(q => q.QuestionType)
                .Include(q => q.SurveyType)
                .Include(q => q.UnitOfMeasure)
                .FirstOrDefaultAsync(m => m.QuestionId == id);
            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }

        // GET: Inquiry/Questions/Create
        public IActionResult Create()
        {
            ViewData["QuestionTypeId"] = new SelectList(_context.LuQuestionTypes, "QuestionTypeId", "AnswerDataType");
            ViewData["SurveyTypeId"] = new SelectList(_context.LuSurveyTypes, "SurveyTypeId", "SurveyTypeNm");
            ViewData["UnitOfMeasureId"] = new SelectList(_context.LuUnitOfMeasures, "UnitOfMeasureId", "UnitOfMeasureNm");
            return View();
        }

        // POST: Inquiry/Questions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("QuestionId,SurveyTypeId,QuestionShortNm,QuestionNm,QuestionDs,Keywords,QuestionSort,ReviewRoleLevel,QuestionTypeId,CommentFl,QuestionValue,UnitOfMeasureId,ModifiedId,ModifiedDt,FileData")] Question question)
        {
            if (ModelState.IsValid)
            {
                _context.Add(question);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["QuestionTypeId"] = new SelectList(_context.LuQuestionTypes, "QuestionTypeId", "AnswerDataType", question.QuestionTypeId);
            ViewData["SurveyTypeId"] = new SelectList(_context.LuSurveyTypes, "SurveyTypeId", "SurveyTypeNm", question.SurveyTypeId);
            ViewData["UnitOfMeasureId"] = new SelectList(_context.LuUnitOfMeasures, "UnitOfMeasureId", "UnitOfMeasureNm", question.UnitOfMeasureId);
            return View(question);
        }

        // GET: Inquiry/Questions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _context.Questions.FindAsync(id);
            if (question == null)
            {
                return NotFound();
            }
            ViewData["QuestionTypeId"] = new SelectList(_context.LuQuestionTypes, "QuestionTypeId", "AnswerDataType", question.QuestionTypeId);
            ViewData["SurveyTypeId"] = new SelectList(_context.LuSurveyTypes, "SurveyTypeId", "SurveyTypeNm", question.SurveyTypeId);
            ViewData["UnitOfMeasureId"] = new SelectList(_context.LuUnitOfMeasures, "UnitOfMeasureId", "UnitOfMeasureNm", question.UnitOfMeasureId);
            return View(question);
        }

        // POST: Inquiry/Questions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("QuestionId,SurveyTypeId,QuestionShortNm,QuestionNm,QuestionDs,Keywords,QuestionSort,ReviewRoleLevel,QuestionTypeId,CommentFl,QuestionValue,UnitOfMeasureId,ModifiedId,ModifiedDt,FileData")] Question question)
        {
            if (id != question.QuestionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(question);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionExists(question.QuestionId))
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
            ViewData["QuestionTypeId"] = new SelectList(_context.LuQuestionTypes, "QuestionTypeId", "AnswerDataType", question.QuestionTypeId);
            ViewData["SurveyTypeId"] = new SelectList(_context.LuSurveyTypes, "SurveyTypeId", "SurveyTypeNm", question.SurveyTypeId);
            ViewData["UnitOfMeasureId"] = new SelectList(_context.LuUnitOfMeasures, "UnitOfMeasureId", "UnitOfMeasureNm", question.UnitOfMeasureId);
            return View(question);
        }

        // GET: Inquiry/Questions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _context.Questions
                .Include(q => q.QuestionType)
                .Include(q => q.SurveyType)
                .Include(q => q.UnitOfMeasure)
                .FirstOrDefaultAsync(m => m.QuestionId == id);
            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }

        // POST: Inquiry/Questions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question != null)
            {
                _context.Questions.Remove(question);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuestionExists(int id)
        {
            return _context.Questions.Any(e => e.QuestionId == id);
        }
    }
}
