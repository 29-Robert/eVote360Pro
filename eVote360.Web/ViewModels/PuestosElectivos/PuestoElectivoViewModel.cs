namespace eVote360.WebApp.ViewModels.PuestosElectivos
{
    public class PuestoElectivoViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public bool Estado { get; set; }
    }
}