
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameRecommendationSystem.Models;
using GameRecommendationSystem.Data;

public class PublishersController : Controller
{
    private readonly ApplicationDbContext _context;

    public PublishersController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: PUBLISHERS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Publishers.ToListAsync());
    }

    // GET: PUBLISHERS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var publisher = await _context.Publishers
            .FirstOrDefaultAsync(m => m.Id == id);
        if (publisher == null)
        {
            return NotFound();
        }

        return View(publisher);
    }

    // GET: PUBLISHERS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: PUBLISHERS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Description,Website,Games")] Publisher publisher)
    {
        if (ModelState.IsValid)
        {
            _context.Add(publisher);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(publisher);
    }

    // GET: PUBLISHERS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var publisher = await _context.Publishers.FindAsync(id);
        if (publisher == null)
        {
            return NotFound();
        }
        return View(publisher);
    }

    // POST: PUBLISHERS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Name,Description,Website,Games")] Publisher publisher)
    {
        if (id != publisher.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(publisher);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PublisherExists(publisher.Id))
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
        return View(publisher);
    }

    // GET: PUBLISHERS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var publisher = await _context.Publishers
            .FirstOrDefaultAsync(m => m.Id == id);
        if (publisher == null)
        {
            return NotFound();
        }

        return View(publisher);
    }

    // POST: PUBLISHERS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var publisher = await _context.Publishers.FindAsync(id);
        if (publisher != null)
        {
            _context.Publishers.Remove(publisher);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool PublisherExists(int? id)
    {
        return _context.Publishers.Any(e => e.Id == id);
    }
}
