using System;
using System.Linq;
using InquirySpark.Repository.Database;
using Microsoft.EntityFrameworkCore;

namespace InquirySpark.Admin.Areas.Inquiry.Controllers
{
    [Area("Inquiry")]
    public class LuReviewStatusController : Controller
    {
        private readonly InquirySparkContext _context;

        public LuReviewStatusController(InquirySparkContext context)
        {
            _context = context;
        }

        // GET: Inquiry/LuReviewStatus
        public async Task<IActionResult> Index()
        {
            return View(await _context.LuReviewStatuses.ToListAsync());
        }

        // GET: Inquiry/LuReviewStatus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var luReviewStatus = await _context.LuReviewStatuses
                .FirstOrDefaultAsync(m => m.ReviewStatusId == id);
            if (luReviewStatus == null)
            {
                return NotFound();
            }

            return View(luReviewStatus);
        }

        // GET: Inquiry/LuReviewStatus/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Inquiry/LuReviewStatus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReviewStatusId,ReviewStatusNm,ReviewStatusDs,ApprovedFl,CommentFl,ModifiedId,ModifiedDt")] LuReviewStatus luReviewStatus)
        {
            if (ModelState.IsValid)
            {
                _context.Add(luReviewStatus);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(luReviewStatus);
        }

        // GET: Inquiry/LuReviewStatus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var luReviewStatus = await _context.LuReviewStatuses.FindAsync(id);
            if (luReviewStatus == null)
            {
                return NotFound();
            }
            return View(luReviewStatus);
        }

        // POST: Inquiry/LuReviewStatus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReviewStatusId,ReviewStatusNm,ReviewStatusDs,ApprovedFl,CommentFl,ModifiedId,ModifiedDt")] LuReviewStatus luReviewStatus)
        {
            if (id != luReviewStatus.ReviewStatusId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(luReviewStatus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LuReviewStatusExists(luReviewStatus.ReviewStatusId))
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
            return View(luReviewStatus);
        }

        // GET: Inquiry/LuReviewStatus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var luReviewStatus = await _context.LuReviewStatuses
                .FirstOrDefaultAsync(m => m.ReviewStatusId == id);
            if (luReviewStatus == null)
            {
                return NotFound();
            }

            return View(luReviewStatus);
        }

        // POST: Inquiry/LuReviewStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var luReviewStatus = await _context.LuReviewStatuses.FindAsync(id);
            if (luReviewStatus != null)
            {
                _context.LuReviewStatuses.Remove(luReviewStatus);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LuReviewStatusExists(int id)
        {
            return _context.LuReviewStatuses.Any(e => e.ReviewStatusId == id);
        }
    }
}
