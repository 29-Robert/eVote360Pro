using System.ComponentModel.DataAnnotations;

namespace eVote360.WebApp.ViewModels.Usuarios
{
    public class SaveUsuarioViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        public string Apellido { get; set; } = null!;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Debe ingresar un correo electrónico válido.")]
        public string Correo { get; set; } = null!;

        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        public string NombreUsuario { get; set; } = null!;

        [DataType(DataType.Password)]
        public string? Contrasena { get; set; }

        [DataType(DataType.Password)]
        [Compare("Contrasena", ErrorMessage = "La contraseña y la confirmación no coinciden.")]
        public string? ConfirmarContrasena { get; set; }

        [Required(ErrorMessage = "El rol es obligatorio.")]
        public string Rol { get; set; } = null!;

        public bool Estado { get; set; } = true;
    }
}