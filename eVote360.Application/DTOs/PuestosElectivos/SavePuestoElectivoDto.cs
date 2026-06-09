namespace eVote360.Core.Application.DTOs.PuestosElectivos
{
    public class SavePuestoElectivoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
    }
}