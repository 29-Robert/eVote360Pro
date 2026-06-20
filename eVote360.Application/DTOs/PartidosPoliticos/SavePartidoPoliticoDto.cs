namespace eVote360.Core.Application.DTOs.PartidosPoliticos
{
    public class SavePartidoPoliticoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public string Siglas { get; set; } = null!;
        public string LogoUrl { get; set; } = null!;
        public bool PuedeEditarCamposCriticos { get; set; } = true;
    }
}