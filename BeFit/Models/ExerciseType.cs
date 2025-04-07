using System.ComponentModel.DataAnnotations;

public class ExerciseType
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Nazwa ćwiczenia jest wymagana")]
    [StringLength(100, ErrorMessage = "Nazwa ćwiczenia może mieć maksymalnie 100 znaków")]
    [Display(Name = "Nazwa ćwiczenia")]
    public string Name { get; set; }
}