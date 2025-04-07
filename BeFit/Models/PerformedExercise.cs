using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class PerformedExercise
{
    public int Id { get; set; }

    [Display(Name = "Rodzaj ćwiczenia")]
    public int ExerciseTypeId { get; set; }

    [Display(Name = "Sesja treningowa")]
    public int WorkoutSessionId { get; set; }

    [Display(Name = "Obciążenie (kg)")]
    [Range(0, 1000, ErrorMessage = "Podaj wartość od 0 do 1000")]
    public double Weight { get; set; }

    [Display(Name = "Liczba serii")]
    [Range(1, 20, ErrorMessage = "Podaj liczbę serii od 1 do 20")]
    public int Sets { get; set; }

    [Display(Name = "Liczba powtórzeń")]
    [Range(1, 100, ErrorMessage = "Podaj liczbę powtórzeń od 1 do 100")]
    public int Repetitions { get; set; }

    public string UserId { get; set; }

    [ValidateNever]
    public virtual ExerciseType ExerciseType { get; set; }

    [ValidateNever]
    public virtual WorkoutSession WorkoutSession { get; set; }
}