using System.ComponentModel.DataAnnotations;

namespace IdeasCreativas.Models.ViewModels
{
    public class ProfessorLoginViewModel
    {
        [Required]
        public string User { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
