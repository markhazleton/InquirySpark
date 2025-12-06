using System;
using System.Linq;
using InquirySpark.Repository.Database;
using Microsoft.EntityFrameworkCore;

namespace InquirySpark.Admin.Areas.Inquiry.Controllers
{
    [Area("Inquiry")]
    public class LuSurveyTypesController : Controller
    {
        private readonly InquirySparkContext _context;

        public LuSurveyTypesController(InquirySparkContext context)
        {
            _context = context;
        }

        // GET: Inquiry/LuSurveyTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.LuSurveyTypes.ToListAsync());
        }

        // GET: Inquiry/LuSurveyTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var luSurveyType = await _context.LuSurveyTypes
                .FirstOrDefaultAsync(m => m.SurveyTypeId == id);
            if (luSurveyType == null)
            {
                return NotFound();
            }

            return View(luSurveyType);
        }

        // GET: Inquiry/LuSurveyTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Inquiry/LuSurveyTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SurveyTypeId,SurveyTypeShortNm,SurveyTypeNm,SurveyTypeDs,SurveyTypeComment,ApplicationTypeId,ParentSurveyTypeId,MutiSequenceFl,ModifiedId,ModifiedDt")] LuSurveyType luSurveyType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(luSurveyType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(luSurveyType);
        }

        // GET: Inquiry/LuSurveyTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var luSurveyType = await _context.LuSurveyTypes.FindAsync(id);
            if (luSurveyType == null)
            {
                return NotFound();
            }
            return View(luSurveyType);
        }

        // POST: Inquiry/LuSurveyTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SurveyTypeId,SurveyTypeShortNm,SurveyTypeNm,SurveyTypeDs,SurveyTypeComment,ApplicationTypeId,ParentSurveyTypeId,MutiSequenceFl,ModifiedId,ModifiedDt")] LuSurveyType luSurveyType)
        {
            if (id != luSurveyType.SurveyTypeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(luSurveyType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LuSurveyTypeExists(luSurveyType.SurveyTypeId))
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
            return View(luSurveyType);
        }

        // GET: Inquiry/LuSurveyTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var luSurveyType = await _context.LuSurveyTypes
                .FirstOrDefaultAsync(m => m.SurveyTypeId == id);
            if (luSurveyType == null)
            {
                return NotFound();
            }

            return View(luSurveyType);
        }

        // POST: Inquiry/LuSurveyTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var luSurveyType = await _context.LuSurveyTypes.FindAsync(id);
            if (luSurveyType != null)
            {
                _context.LuSurveyTypes.Remove(luSurveyType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LuSurveyTypeExists(int id)
        {
            return _context.LuSurveyTypes.Any(e => e.SurveyTypeId == id);
        }
    }
}
