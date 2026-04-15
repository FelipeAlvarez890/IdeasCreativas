using System.ComponentModel.DataAnnotations;

namespace IdeasCreativas.Models.ViewModels
{
    public class TeamRegistrationViewModel
    {
        [Required(ErrorMessage = "El nombre del equipo es obligatorio.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "La confirmación de la contraseña es obligatoria.")]
        [Compare("Password", ErrorMessage = "los passwords no coinciden")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de integrantes es obligatorio.")]
        [Range(1, 2, ErrorMessage = "el numero de integrantes no esta permitido")]
        public int MemberCount { get; set; }

        [Required(ErrorMessage = "Debe ingresar el nombre del integrante 1.")]
        public string Member1 { get; set; } = string.Empty;

        public string? Member2 { get; set; }
    }
}
