using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

[Authorize]
public class PerformedExercisesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public PerformedExercisesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var exercises = await _context.PerformedExercises
            .Include(p => p.ExerciseType)
            .Include(p => p.WorkoutSession)
            .Where(p => p.UserId == userId)
            .ToListAsync();

        return View(exercises);
    }

    public IActionResult Create(int? workoutSessionId)
    {
        ViewData["ExerciseTypeId"] = new SelectList(_context.ExerciseTypes, "Id", "Name");
        ViewData["WorkoutSessionId"] = new SelectList(_context.WorkoutSessions, "Id", "StartTime");

        var model = new PerformedExercise();
        if (workoutSessionId.HasValue)
            model.WorkoutSessionId = workoutSessionId.Value;

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PerformedExercise performedExercise)
    {
        ModelState.Remove("UserId");
        ModelState.Remove("ExerciseType");
        ModelState.Remove("WorkoutSession");

        if (!ModelState.IsValid)
        {
            Console.WriteLine("🔴 Błąd walidacji:");
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                Console.WriteLine("❌ " + error.ErrorMessage);

            ViewData["ExerciseTypeId"] = new SelectList(_context.ExerciseTypes, "Id", "Name", performedExercise.ExerciseTypeId);
            ViewData["WorkoutSessionId"] = new SelectList(_context.WorkoutSessions, "Id", "StartTime", performedExercise.WorkoutSessionId);
            return View(performedExercise);
        }

        performedExercise.UserId = _userManager.GetUserId(User);
        _context.Add(performedExercise);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var exercise = await _context.PerformedExercises.FindAsync(id);
        if (exercise == null || exercise.UserId != _userManager.GetUserId(User)) return Unauthorized();

        ViewData["ExerciseTypeId"] = new SelectList(_context.ExerciseTypes, "Id", "Name", exercise.ExerciseTypeId);
        ViewData["WorkoutSessionId"] = new SelectList(_context.WorkoutSessions, "Id", "StartTime", exercise.WorkoutSessionId);
        return View(exercise);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PerformedExercise performedExercise)
    {
        if (id != performedExercise.Id || performedExercise.UserId != _userManager.GetUserId(User))
            return Unauthorized();

        ModelState.Remove("UserId");
        ModelState.Remove("ExerciseType");
        ModelState.Remove("WorkoutSession");

        if (!ModelState.IsValid)
        {
            ViewData["ExerciseTypeId"] = new SelectList(_context.ExerciseTypes, "Id", "Name", performedExercise.ExerciseTypeId);
            ViewData["WorkoutSessionId"] = new SelectList(_context.WorkoutSessions, "Id", "StartTime", performedExercise.WorkoutSessionId);
            return View(performedExercise);
        }

        _context.Update(performedExercise);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var exercise = await _context.PerformedExercises
            .Include(p => p.ExerciseType)
            .Include(p => p.WorkoutSession)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (exercise == null || exercise.UserId != _userManager.GetUserId(User)) return Unauthorized();
        return View(exercise);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var exercise = await _context.PerformedExercises.FindAsync(id);
        if (exercise == null || exercise.UserId != _userManager.GetUserId(User)) return Unauthorized();

        _context.PerformedExercises.Remove(exercise);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Statistics()
    {
        var userId = _userManager.GetUserId(User);
        var monthAgo = DateTime.Now.AddDays(-28);

        var stats = await _context.PerformedExercises
            .Include(p => p.ExerciseType)
            .Include(p => p.WorkoutSession)
            .Where(p => p.UserId == userId && p.WorkoutSession.StartTime >= monthAgo)
            .GroupBy(p => p.ExerciseType.Name)
            .Select(g => new
            {
                ExerciseName = g.Key,
                Count = g.Count(),
                TotalReps = g.Sum(x => x.Repetitions * x.Sets),
                AvgWeight = g.Average(x => x.Weight),
                MaxWeight = g.Max(x => x.Weight)
            })
            .ToListAsync();

        return View(stats);
    }
}
