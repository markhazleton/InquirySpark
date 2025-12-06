using System;
using System.Linq;
using InquirySpark.Repository.Database;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InquirySpark.Admin.Areas.Inquiry.Controllers
{
    [Area("Inquiry")]
    public class QuestionAnswersController : Controller
    {
        private readonly InquirySparkContext _context;

        public QuestionAnswersController(InquirySparkContext context)
        {
            _context = context;
        }

        // GET: Inquiry/QuestionAnswers
        public async Task<IActionResult> Index()
        {
            var inquirySparkContext = _context.QuestionAnswers.Include(q => q.Question);
            return View(await inquirySparkContext.ToListAsync());
        }

        // GET: Inquiry/QuestionAnswers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questionAnswer = await _context.QuestionAnswers
                .Include(q => q.Question)
                .FirstOrDefaultAsync(m => m.QuestionAnswerId == id);
            if (questionAnswer == null)
            {
                return NotFound();
            }

            return View(questionAnswer);
        }

        // GET: Inquiry/QuestionAnswers/Create
        public IActionResult Create()
        {
            ViewData["QuestionId"] = new SelectList(_context.Questions, "QuestionId", "QuestionDs");
            return View();
        }

        // POST: Inquiry/QuestionAnswers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("QuestionAnswerId,QuestionId,QuestionAnswerSort,QuestionAnswerShortNm,QuestionAnswerNm,QuestionAnswerValue,QuestionAnswerDs,CommentFl,ActiveFl,ModifiedId,ModifiedDt")] QuestionAnswer questionAnswer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(questionAnswer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["QuestionId"] = new SelectList(_context.Questions, "QuestionId", "QuestionDs", questionAnswer.QuestionId);
            return View(questionAnswer);
        }

        // GET: Inquiry/QuestionAnswers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questionAnswer = await _context.QuestionAnswers.FindAsync(id);
            if (questionAnswer == null)
            {
                return NotFound();
            }
            ViewData["QuestionId"] = new SelectList(_context.Questions, "QuestionId", "QuestionDs", questionAnswer.QuestionId);
            return View(questionAnswer);
        }

        // POST: Inquiry/QuestionAnswers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("QuestionAnswerId,QuestionId,QuestionAnswerSort,QuestionAnswerShortNm,QuestionAnswerNm,QuestionAnswerValue,QuestionAnswerDs,CommentFl,ActiveFl,ModifiedId,ModifiedDt")] QuestionAnswer questionAnswer)
        {
            if (id != questionAnswer.QuestionAnswerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(questionAnswer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionAnswerExists(questionAnswer.QuestionAnswerId))
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
            ViewData["QuestionId"] = new SelectList(_context.Questions, "QuestionId", "QuestionDs", questionAnswer.QuestionId);
            return View(questionAnswer);
        }

        // GET: Inquiry/QuestionAnswers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questionAnswer = await _context.QuestionAnswers
                .Include(q => q.Question)
                .FirstOrDefaultAsync(m => m.QuestionAnswerId == id);
            if (questionAnswer == null)
            {
                return NotFound();
            }

            return View(questionAnswer);
        }

        // POST: Inquiry/QuestionAnswers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var questionAnswer = await _context.QuestionAnswers.FindAsync(id);
            if (questionAnswer != null)
            {
                _context.QuestionAnswers.Remove(questionAnswer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuestionAnswerExists(int id)
        {
            return _context.QuestionAnswers.Any(e => e.QuestionAnswerId == id);
        }
    }
}
