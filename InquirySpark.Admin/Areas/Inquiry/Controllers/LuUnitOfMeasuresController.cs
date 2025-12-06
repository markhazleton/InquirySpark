using System;
using System.Linq;
using InquirySpark.Repository.Database;
using Microsoft.EntityFrameworkCore;

namespace InquirySpark.Admin.Areas.Inquiry.Controllers
{
    [Area("Inquiry")]
    public class LuUnitOfMeasuresController : Controller
    {
        private readonly InquirySparkContext _context;

        public LuUnitOfMeasuresController(InquirySparkContext context)
        {
            _context = context;
        }

        // GET: Inquiry/LuUnitOfMeasures
        public async Task<IActionResult> Index()
        {
            return View(await _context.LuUnitOfMeasures.ToListAsync());
        }

        // GET: Inquiry/LuUnitOfMeasures/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var luUnitOfMeasure = await _context.LuUnitOfMeasures
                .FirstOrDefaultAsync(m => m.UnitOfMeasureId == id);
            if (luUnitOfMeasure == null)
            {
                return NotFound();
            }

            return View(luUnitOfMeasure);
        }

        // GET: Inquiry/LuUnitOfMeasures/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Inquiry/LuUnitOfMeasures/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UnitOfMeasureId,UnitOfMeasureNm,UnitOfMeasureDs,ModifiedId,ModifiedDt")] LuUnitOfMeasure luUnitOfMeasure)
        {
            if (ModelState.IsValid)
            {
                _context.Add(luUnitOfMeasure);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(luUnitOfMeasure);
        }

        // GET: Inquiry/LuUnitOfMeasures/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var luUnitOfMeasure = await _context.LuUnitOfMeasures.FindAsync(id);
            if (luUnitOfMeasure == null)
            {
                return NotFound();
            }
            return View(luUnitOfMeasure);
        }

        // POST: Inquiry/LuUnitOfMeasures/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UnitOfMeasureId,UnitOfMeasureNm,UnitOfMeasureDs,ModifiedId,ModifiedDt")] LuUnitOfMeasure luUnitOfMeasure)
        {
            if (id != luUnitOfMeasure.UnitOfMeasureId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(luUnitOfMeasure);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LuUnitOfMeasureExists(luUnitOfMeasure.UnitOfMeasureId))
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
            return View(luUnitOfMeasure);
        }

        // GET: Inquiry/LuUnitOfMeasures/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var luUnitOfMeasure = await _context.LuUnitOfMeasures
                .FirstOrDefaultAsync(m => m.UnitOfMeasureId == id);
            if (luUnitOfMeasure == null)
            {
                return NotFound();
            }

            return View(luUnitOfMeasure);
        }

        // POST: Inquiry/LuUnitOfMeasures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var luUnitOfMeasure = await _context.LuUnitOfMeasures.FindAsync(id);
            if (luUnitOfMeasure != null)
            {
                _context.LuUnitOfMeasures.Remove(luUnitOfMeasure);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LuUnitOfMeasureExists(int id)
        {
            return _context.LuUnitOfMeasures.Any(e => e.UnitOfMeasureId == id);
        }
    }
}
