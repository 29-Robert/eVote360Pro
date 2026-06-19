using System.ComponentModel.DataAnnotations;

namespace eVote360.WebApp.ViewModels.Auth
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido.")]
        public string NombreUsuario { get; set; } = null!;

        [Required(ErrorMessage = "La contraseña es requerida.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}