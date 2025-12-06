using System;
using System.Linq;
using InquirySpark.Repository.Database;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InquirySpark.Admin.Areas.Inquiry.Controllers
{
    [Area("Inquiry")]
    public class SurveyEmailTemplatesController : Controller
    {
        private readonly InquirySparkContext _context;

        public SurveyEmailTemplatesController(InquirySparkContext context)
        {
            _context = context;
        }

        // GET: Inquiry/SurveyEmailTemplates
        public async Task<IActionResult> Index()
        {
            var inquirySparkContext = _context.SurveyEmailTemplates.Include(s => s.Survey);
            return View(await inquirySparkContext.ToListAsync());
        }

        // GET: Inquiry/SurveyEmailTemplates/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surveyEmailTemplate = await _context.SurveyEmailTemplates
                .Include(s => s.Survey)
                .FirstOrDefaultAsync(m => m.SurveyEmailTemplateId == id);
            if (surveyEmailTemplate == null)
            {
                return NotFound();
            }

            return View(surveyEmailTemplate);
        }

        // GET: Inquiry/SurveyEmailTemplates/Create
        public IActionResult Create()
        {
            ViewData["SurveyId"] = new SelectList(_context.Surveys, "SurveyId", "CompletionMessage");
            return View();
        }

        // POST: Inquiry/SurveyEmailTemplates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SurveyEmailTemplateId,SurveyEmailTemplateNm,SurveyId,StatusId,SubjectTemplate,EmailTemplate,FromEmailAddress,FilterCriteria,StartDt,EndDt,Active,SendToSupervisor,ModifiedId,ModifiedDt")] SurveyEmailTemplate surveyEmailTemplate)
        {
            if (ModelState.IsValid)
            {
                _context.Add(surveyEmailTemplate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SurveyId"] = new SelectList(_context.Surveys, "SurveyId", "CompletionMessage", surveyEmailTemplate.SurveyId);
            return View(surveyEmailTemplate);
        }

        // GET: Inquiry/SurveyEmailTemplates/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surveyEmailTemplate = await _context.SurveyEmailTemplates.FindAsync(id);
            if (surveyEmailTemplate == null)
            {
                return NotFound();
            }
            ViewData["SurveyId"] = new SelectList(_context.Surveys, "SurveyId", "CompletionMessage", surveyEmailTemplate.SurveyId);
            return View(surveyEmailTemplate);
        }

        // POST: Inquiry/SurveyEmailTemplates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SurveyEmailTemplateId,SurveyEmailTemplateNm,SurveyId,StatusId,SubjectTemplate,EmailTemplate,FromEmailAddress,FilterCriteria,StartDt,EndDt,Active,SendToSupervisor,ModifiedId,ModifiedDt")] SurveyEmailTemplate surveyEmailTemplate)
        {
            if (id != surveyEmailTemplate.SurveyEmailTemplateId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(surveyEmailTemplate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SurveyEmailTemplateExists(surveyEmailTemplate.SurveyEmailTemplateId))
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
            ViewData["SurveyId"] = new SelectList(_context.Surveys, "SurveyId", "CompletionMessage", surveyEmailTemplate.SurveyId);
            return View(surveyEmailTemplate);
        }

        // GET: Inquiry/SurveyEmailTemplates/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surveyEmailTemplate = await _context.SurveyEmailTemplates
                .Include(s => s.Survey)
                .FirstOrDefaultAsync(m => m.SurveyEmailTemplateId == id);
            if (surveyEmailTemplate == null)
            {
                return NotFound();
            }

            return View(surveyEmailTemplate);
        }

        // POST: Inquiry/SurveyEmailTemplates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var surveyEmailTemplate = await _context.SurveyEmailTemplates.FindAsync(id);
            if (surveyEmailTemplate != null)
            {
                _context.SurveyEmailTemplates.Remove(surveyEmailTemplate);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SurveyEmailTemplateExists(int id)
        {
            return _context.SurveyEmailTemplates.Any(e => e.SurveyEmailTemplateId == id);
        }
    }
}
