using System;
using System.Linq;
using InquirySpark.Repository.Database;
using Microsoft.EntityFrameworkCore;

namespace InquirySpark.Admin.Areas.Inquiry.Controllers
{
    [Area("Inquiry")]
    public class ImportHistoriesController : Controller
    {
        private readonly InquirySparkContext _context;

        public ImportHistoriesController(InquirySparkContext context)
        {
            _context = context;
        }

        // GET: Inquiry/ImportHistories
        public async Task<IActionResult> Index()
        {
            return View(await _context.ImportHistories.ToListAsync());
        }

        // GET: Inquiry/ImportHistories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var importHistory = await _context.ImportHistories
                .FirstOrDefaultAsync(m => m.ImportHistoryId == id);
            if (importHistory == null)
            {
                return NotFound();
            }

            return View(importHistory);
        }

        // GET: Inquiry/ImportHistories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Inquiry/ImportHistories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ImportHistoryId,FileName,ImportType,NumberOfRows,ImportLog,ModifiedId,ModifiedDt")] ImportHistory importHistory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(importHistory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(importHistory);
        }

        // GET: Inquiry/ImportHistories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var importHistory = await _context.ImportHistories.FindAsync(id);
            if (importHistory == null)
            {
                return NotFound();
            }
            return View(importHistory);
        }

        // POST: Inquiry/ImportHistories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ImportHistoryId,FileName,ImportType,NumberOfRows,ImportLog,ModifiedId,ModifiedDt")] ImportHistory importHistory)
        {
            if (id != importHistory.ImportHistoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(importHistory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ImportHistoryExists(importHistory.ImportHistoryId))
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
            return View(importHistory);
        }

        // GET: Inquiry/ImportHistories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var importHistory = await _context.ImportHistories
                .FirstOrDefaultAsync(m => m.ImportHistoryId == id);
            if (importHistory == null)
            {
                return NotFound();
            }

            return View(importHistory);
        }

        // POST: Inquiry/ImportHistories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var importHistory = await _context.ImportHistories.FindAsync(id);
            if (importHistory != null)
            {
                _context.ImportHistories.Remove(importHistory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ImportHistoryExists(int id)
        {
            return _context.ImportHistories.Any(e => e.ImportHistoryId == id);
        }
    }
}
