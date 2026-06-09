namespace eVote360.Core.Application.DTOs.PuestosElectivos
{
    public class PuestoElectivoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public bool Estado { get; set; }
    }
}