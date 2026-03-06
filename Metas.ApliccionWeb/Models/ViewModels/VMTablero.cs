namespace Metas.AplicacionWeb.Models.ViewModels
{
    public class VMTablero
    {
        public int IdProceso { get; set; }
        public string DescripcionActividad { get; set; }
        public string ProgramaSocial { get; set; }
        public int? Componente { get; set; }
        public string UnidadMedida { get; set; }

        // Programado
        public int EneroProg { get; set; }
        public int FebreroProg { get; set; }
        // ... hasta DiciembreProg
        public int TotalProg { get; set; }

        // Realizado
        public int EneroReal { get; set; }
        public int FebreroReal { get; set; }
        // ... hasta DiciembreReal
        public int TotalReal { get; set; }
    }
}
