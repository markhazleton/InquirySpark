using System;
using System.Linq;
using InquirySpark.Repository.Database;
using Microsoft.EntityFrameworkCore;

namespace InquirySpark.Admin.Areas.Inquiry.Controllers
{
    [Area("Inquiry")]
    public class LuApplicationTypesController : Controller
    {
        private readonly InquirySparkContext _context;

        public LuApplicationTypesController(InquirySparkContext context)
        {
            _context = context;
        }

        // GET: Inquiry/LuApplicationTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.LuApplicationTypes.ToListAsync());
        }

        // GET: Inquiry/LuApplicationTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var luApplicationType = await _context.LuApplicationTypes
                .FirstOrDefaultAsync(m => m.ApplicationTypeId == id);
            if (luApplicationType == null)
            {
                return NotFound();
            }

            return View(luApplicationType);
        }

        // GET: Inquiry/LuApplicationTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Inquiry/LuApplicationTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ApplicationTypeId,ApplicationTypeNm,ApplicationTypeDs,ModifiedId,ModifiedDt")] LuApplicationType luApplicationType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(luApplicationType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(luApplicationType);
        }

        // GET: Inquiry/LuApplicationTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var luApplicationType = await _context.LuApplicationTypes.FindAsync(id);
            if (luApplicationType == null)
            {
                return NotFound();
            }
            return View(luApplicationType);
        }

        // POST: Inquiry/LuApplicationTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ApplicationTypeId,ApplicationTypeNm,ApplicationTypeDs,ModifiedId,ModifiedDt")] LuApplicationType luApplicationType)
        {
            if (id != luApplicationType.ApplicationTypeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(luApplicationType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LuApplicationTypeExists(luApplicationType.ApplicationTypeId))
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
            return View(luApplicationType);
        }

        // GET: Inquiry/LuApplicationTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var luApplicationType = await _context.LuApplicationTypes
                .FirstOrDefaultAsync(m => m.ApplicationTypeId == id);
            if (luApplicationType == null)
            {
                return NotFound();
            }

            return View(luApplicationType);
        }

        // POST: Inquiry/LuApplicationTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var luApplicationType = await _context.LuApplicationTypes.FindAsync(id);
            if (luApplicationType != null)
            {
                _context.LuApplicationTypes.Remove(luApplicationType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LuApplicationTypeExists(int id)
        {
            return _context.LuApplicationTypes.Any(e => e.ApplicationTypeId == id);
        }
    }
}
