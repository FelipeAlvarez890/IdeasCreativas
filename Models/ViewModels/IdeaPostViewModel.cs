using System.ComponentModel.DataAnnotations;

namespace IdeasCreativas.Models.ViewModels
{
    public class IdeaPostViewModel
    {
        [Required(ErrorMessage = "El nombre del equipo es obligatorio")]
        public string TeamName { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string TeamPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "El texto de la idea es obligatorio")]
        [StringLength(512, ErrorMessage = "El texto de la idea no puede superar los 512 caracteres")]
        public string IdeaText { get; set; } = string.Empty;
    }
}
