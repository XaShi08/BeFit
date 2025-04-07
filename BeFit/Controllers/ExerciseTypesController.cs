using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[AllowAnonymous] // domyślnie dla przeglądania
public class ExerciseTypesController : Controller
{
    private readonly ApplicationDbContext _context;

    public ExerciseTypesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index() => View(await _context.ExerciseTypes.ToListAsync());

    [Authorize(Roles = "Administrator")]
    public IActionResult Create() => View();

    [HttpPost, Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Create(ExerciseType exerciseType)
    {
        if (!ModelState.IsValid) return View(exerciseType);
        _context.Add(exerciseType);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var et = await _context.ExerciseTypes.FindAsync(id);
        return et == null ? NotFound() : View(et);
    }

    [HttpPost, Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Edit(int id, ExerciseType exerciseType)
    {
        if (id != exerciseType.Id) return NotFound();
        if (!ModelState.IsValid) return View(exerciseType);
        _context.Update(exerciseType);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var et = await _context.ExerciseTypes.FindAsync(id);
        return et == null ? NotFound() : View(et);
    }

    [HttpPost, ActionName("Delete"), Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var et = await _context.ExerciseTypes.FindAsync(id);
        if (et != null) _context.ExerciseTypes.Remove(et);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}