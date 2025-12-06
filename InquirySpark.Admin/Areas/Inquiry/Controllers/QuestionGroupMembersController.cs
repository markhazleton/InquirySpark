using System;
using System.Linq;
using InquirySpark.Repository.Database;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InquirySpark.Admin.Areas.Inquiry.Controllers
{
    [Area("Inquiry")]
    public class QuestionGroupMembersController : Controller
    {
        private readonly InquirySparkContext _context;

        public QuestionGroupMembersController(InquirySparkContext context)
        {
            _context = context;
        }

        // GET: Inquiry/QuestionGroupMembers
        public async Task<IActionResult> Index()
        {
            var inquirySparkContext = _context.QuestionGroupMembers.Include(q => q.Question).Include(q => q.QuestionGroup);
            return View(await inquirySparkContext.ToListAsync());
        }

        // GET: Inquiry/QuestionGroupMembers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questionGroupMember = await _context.QuestionGroupMembers
                .Include(q => q.Question)
                .Include(q => q.QuestionGroup)
                .FirstOrDefaultAsync(m => m.QuestionGroupMemberId == id);
            if (questionGroupMember == null)
            {
                return NotFound();
            }

            return View(questionGroupMember);
        }

        // GET: Inquiry/QuestionGroupMembers/Create
        public IActionResult Create()
        {
            ViewData["QuestionId"] = new SelectList(_context.Questions, "QuestionId", "QuestionDs");
            ViewData["QuestionGroupId"] = new SelectList(_context.QuestionGroups, "QuestionGroupId", "QuestionGroupNm");
            return View();
        }

        // POST: Inquiry/QuestionGroupMembers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("QuestionGroupMemberId,QuestionGroupId,QuestionId,QuestionWeight,DisplayOrder,ModifiedId,ModifiedDt")] QuestionGroupMember questionGroupMember)
        {
            if (ModelState.IsValid)
            {
                _context.Add(questionGroupMember);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["QuestionId"] = new SelectList(_context.Questions, "QuestionId", "QuestionDs", questionGroupMember.QuestionId);
            ViewData["QuestionGroupId"] = new SelectList(_context.QuestionGroups, "QuestionGroupId", "QuestionGroupNm", questionGroupMember.QuestionGroupId);
            return View(questionGroupMember);
        }

        // GET: Inquiry/QuestionGroupMembers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questionGroupMember = await _context.QuestionGroupMembers.FindAsync(id);
            if (questionGroupMember == null)
            {
                return NotFound();
            }
            ViewData["QuestionId"] = new SelectList(_context.Questions, "QuestionId", "QuestionDs", questionGroupMember.QuestionId);
            ViewData["QuestionGroupId"] = new SelectList(_context.QuestionGroups, "QuestionGroupId", "QuestionGroupNm", questionGroupMember.QuestionGroupId);
            return View(questionGroupMember);
        }

        // POST: Inquiry/QuestionGroupMembers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("QuestionGroupMemberId,QuestionGroupId,QuestionId,QuestionWeight,DisplayOrder,ModifiedId,ModifiedDt")] QuestionGroupMember questionGroupMember)
        {
            if (id != questionGroupMember.QuestionGroupMemberId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(questionGroupMember);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionGroupMemberExists(questionGroupMember.QuestionGroupMemberId))
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
            ViewData["QuestionId"] = new SelectList(_context.Questions, "QuestionId", "QuestionDs", questionGroupMember.QuestionId);
            ViewData["QuestionGroupId"] = new SelectList(_context.QuestionGroups, "QuestionGroupId", "QuestionGroupNm", questionGroupMember.QuestionGroupId);
            return View(questionGroupMember);
        }

        // GET: Inquiry/QuestionGroupMembers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questionGroupMember = await _context.QuestionGroupMembers
                .Include(q => q.Question)
                .Include(q => q.QuestionGroup)
                .FirstOrDefaultAsync(m => m.QuestionGroupMemberId == id);
            if (questionGroupMember == null)
            {
                return NotFound();
            }

            return View(questionGroupMember);
        }

        // POST: Inquiry/QuestionGroupMembers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var questionGroupMember = await _context.QuestionGroupMembers.FindAsync(id);
            if (questionGroupMember != null)
            {
                _context.QuestionGroupMembers.Remove(questionGroupMember);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuestionGroupMemberExists(int id)
        {
            return _context.QuestionGroupMembers.Any(e => e.QuestionGroupMemberId == id);
        }
    }
}
