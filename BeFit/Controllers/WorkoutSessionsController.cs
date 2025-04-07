using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class WorkoutSessionsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public WorkoutSessionsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        return View(await _context.WorkoutSessions.Where(w => w.UserId == userId).ToListAsync());
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(WorkoutSession session)
    {
        ModelState.Remove("UserId");
        ModelState.Remove("PerformedExercises"); // <-- dodano to!

        Console.WriteLine("📥 Dane przesłane z formularza:");
        Console.WriteLine("StartTime: " + session.StartTime);
        Console.WriteLine("EndTime: " + session.EndTime);
        Console.WriteLine("UserId (przed przypisaniem): " + session.UserId);

        if (!ModelState.IsValid)
        {
            Console.WriteLine("🔴 ModelState NIE jest prawidłowy!");
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine("Błąd: " + error.ErrorMessage);
            }
            return View(session);
        }

        Console.WriteLine("✅ ModelState jest prawidłowy – zapisuję dane");
        session.UserId = _userManager.GetUserId(User);
        _context.Add(session);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var session = await _context.WorkoutSessions.FindAsync(id);
        if (session == null || session.UserId != _userManager.GetUserId(User)) return Unauthorized();
        return View(session);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, WorkoutSession session)
    {
        if (id != session.Id || session.UserId != _userManager.GetUserId(User)) return Unauthorized();

        ModelState.Remove("UserId");
        ModelState.Remove("PerformedExercises");

        if (!ModelState.IsValid) return View(session);
        _context.Update(session);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var session = await _context.WorkoutSessions.FindAsync(id);
        if (session == null || session.UserId != _userManager.GetUserId(User)) return Unauthorized();
        return View(session);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var session = await _context.WorkoutSessions.FindAsync(id);
        if (session == null || session.UserId != _userManager.GetUserId(User)) return Unauthorized();
        _context.WorkoutSessions.Remove(session);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}

