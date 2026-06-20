namespace eVote360.WebApp.ViewModels.Auth
{
    public class AccesoDenegadoViewModel
    {
        public string Titulo { get; set; } = "Acceso denegado";
        public string Mensaje { get; set; } = "No cuenta con los permisos necesarios para acceder a este recurso.";
        public string TextoBoton { get; set; } = "Volver al inicio";
        public string ControladorDestino { get; set; } = "Elector";
        public string AccionDestino { get; set; } = "Index";
    }
}