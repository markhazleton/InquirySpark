using System;
using System.Linq;
using InquirySpark.Repository.Database;
using Microsoft.EntityFrameworkCore;

namespace InquirySpark.Admin.Areas.Inquiry.Controllers
{
    [Area("Inquiry")]
    public class LuSurveyResponseStatusController : Controller
    {
        private readonly InquirySparkContext _context;

        public LuSurveyResponseStatusController(InquirySparkContext context)
        {
            _context = context;
        }

        // GET: Inquiry/LuSurveyResponseStatus
        public async Task<IActionResult> Index()
        {
            return View(await _context.LuSurveyResponseStatuses.ToListAsync());
        }

        // GET: Inquiry/LuSurveyResponseStatus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var luSurveyResponseStatus = await _context.LuSurveyResponseStatuses
                .FirstOrDefaultAsync(m => m.StatusId == id);
            if (luSurveyResponseStatus == null)
            {
                return NotFound();
            }

            return View(luSurveyResponseStatus);
        }

        // GET: Inquiry/LuSurveyResponseStatus/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Inquiry/LuSurveyResponseStatus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StatusId,StatusNm,StatusDs,EmailTemplate,PreviousStatusId,NextStatusId,ModifiedId,ModifiedDt")] LuSurveyResponseStatus luSurveyResponseStatus)
        {
            if (ModelState.IsValid)
            {
                _context.Add(luSurveyResponseStatus);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(luSurveyResponseStatus);
        }

        // GET: Inquiry/LuSurveyResponseStatus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var luSurveyResponseStatus = await _context.LuSurveyResponseStatuses.FindAsync(id);
            if (luSurveyResponseStatus == null)
            {
                return NotFound();
            }
            return View(luSurveyResponseStatus);
        }

        // POST: Inquiry/LuSurveyResponseStatus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StatusId,StatusNm,StatusDs,EmailTemplate,PreviousStatusId,NextStatusId,ModifiedId,ModifiedDt")] LuSurveyResponseStatus luSurveyResponseStatus)
        {
            if (id != luSurveyResponseStatus.StatusId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(luSurveyResponseStatus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LuSurveyResponseStatusExists(luSurveyResponseStatus.StatusId))
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
            return View(luSurveyResponseStatus);
        }

        // GET: Inquiry/LuSurveyResponseStatus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var luSurveyResponseStatus = await _context.LuSurveyResponseStatuses
                .FirstOrDefaultAsync(m => m.StatusId == id);
            if (luSurveyResponseStatus == null)
            {
                return NotFound();
            }

            return View(luSurveyResponseStatus);
        }

        // POST: Inquiry/LuSurveyResponseStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var luSurveyResponseStatus = await _context.LuSurveyResponseStatuses.FindAsync(id);
            if (luSurveyResponseStatus != null)
            {
                _context.LuSurveyResponseStatuses.Remove(luSurveyResponseStatus);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LuSurveyResponseStatusExists(int id)
        {
            return _context.LuSurveyResponseStatuses.Any(e => e.StatusId == id);
        }
    }
}
