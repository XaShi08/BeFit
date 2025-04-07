using System.ComponentModel.DataAnnotations;

public class WorkoutSession
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; }

    [Required(ErrorMessage = "Data rozpoczęcia jest wymagana")]
    [Display(Name = "Data rozpoczęcia")]
    public DateTime StartTime { get; set; }

    [Required(ErrorMessage = "Data zakończenia jest wymagana")]
    [Display(Name = "Data zakończenia")]
    public DateTime EndTime { get; set; }

    public virtual ICollection<PerformedExercise> PerformedExercises { get; set; }
}
