using System;
using System.Linq;
using InquirySpark.Repository.Database;
using Microsoft.EntityFrameworkCore;

namespace InquirySpark.Admin.Areas.Inquiry.Controllers
{
    [Area("Inquiry")]
    public class LuQuestionTypesController : Controller
    {
        private readonly InquirySparkContext _context;

        public LuQuestionTypesController(InquirySparkContext context)
        {
            _context = context;
        }

        // GET: Inquiry/LuQuestionTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.LuQuestionTypes.ToListAsync());
        }

        // GET: Inquiry/LuQuestionTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var luQuestionType = await _context.LuQuestionTypes
                .FirstOrDefaultAsync(m => m.QuestionTypeId == id);
            if (luQuestionType == null)
            {
                return NotFound();
            }

            return View(luQuestionType);
        }

        // GET: Inquiry/LuQuestionTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Inquiry/LuQuestionTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("QuestionTypeId,QuestionTypeCd,QuestionTypeDs,ControlName,AnswerDataType,ModifiedId,ModifiedDt")] LuQuestionType luQuestionType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(luQuestionType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(luQuestionType);
        }

        // GET: Inquiry/LuQuestionTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var luQuestionType = await _context.LuQuestionTypes.FindAsync(id);
            if (luQuestionType == null)
            {
                return NotFound();
            }
            return View(luQuestionType);
        }

        // POST: Inquiry/LuQuestionTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("QuestionTypeId,QuestionTypeCd,QuestionTypeDs,ControlName,AnswerDataType,ModifiedId,ModifiedDt")] LuQuestionType luQuestionType)
        {
            if (id != luQuestionType.QuestionTypeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(luQuestionType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LuQuestionTypeExists(luQuestionType.QuestionTypeId))
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
            return View(luQuestionType);
        }

        // GET: Inquiry/LuQuestionTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var luQuestionType = await _context.LuQuestionTypes
                .FirstOrDefaultAsync(m => m.QuestionTypeId == id);
            if (luQuestionType == null)
            {
                return NotFound();
            }

            return View(luQuestionType);
        }

        // POST: Inquiry/LuQuestionTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var luQuestionType = await _context.LuQuestionTypes.FindAsync(id);
            if (luQuestionType != null)
            {
                _context.LuQuestionTypes.Remove(luQuestionType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LuQuestionTypeExists(int id)
        {
            return _context.LuQuestionTypes.Any(e => e.QuestionTypeId == id);
        }
    }
}
