using System;
using System.Linq;
using InquirySpark.Repository.Database;
using Microsoft.EntityFrameworkCore;

namespace InquirySpark.Admin.Areas.Inquiry.Controllers
{
    [Area("Inquiry")]
    public class ChartSettingsController : Controller
    {
        private readonly InquirySparkContext _context;

        public ChartSettingsController(InquirySparkContext context)
        {
            _context = context;
        }

        // GET: Inquiry/ChartSettings
        public async Task<IActionResult> Index()
        {
            return View(await _context.ChartSettings.ToListAsync());
        }

        // GET: Inquiry/ChartSettings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chartSetting = await _context.ChartSettings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chartSetting == null)
            {
                return NotFound();
            }

            return View(chartSetting);
        }

        // GET: Inquiry/ChartSettings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Inquiry/ChartSettings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SiteUserId,SiteAppId,SettingType,SettingName,SettingValue,SettingValueEnhanced,DateCreated,LastUpdated")] ChartSetting chartSetting)
        {
            if (ModelState.IsValid)
            {
                _context.Add(chartSetting);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(chartSetting);
        }

        // GET: Inquiry/ChartSettings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chartSetting = await _context.ChartSettings.FindAsync(id);
            if (chartSetting == null)
            {
                return NotFound();
            }
            return View(chartSetting);
        }

        // POST: Inquiry/ChartSettings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SiteUserId,SiteAppId,SettingType,SettingName,SettingValue,SettingValueEnhanced,DateCreated,LastUpdated")] ChartSetting chartSetting)
        {
            if (id != chartSetting.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chartSetting);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChartSettingExists(chartSetting.Id))
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
            return View(chartSetting);
        }

        // GET: Inquiry/ChartSettings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chartSetting = await _context.ChartSettings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chartSetting == null)
            {
                return NotFound();
            }

            return View(chartSetting);
        }

        // POST: Inquiry/ChartSettings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chartSetting = await _context.ChartSettings.FindAsync(id);
            if (chartSetting != null)
            {
                _context.ChartSettings.Remove(chartSetting);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChartSettingExists(int id)
        {
            return _context.ChartSettings.Any(e => e.Id == id);
        }
    }
}
