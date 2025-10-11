namespace Metas.AplicacionWeb.Models.ViewModels
{
    public class VMDatosInternos
    {
        public int IdProceso { get; set; }
        public string pp { get; set; }
        public int? Componente { get; set; }
        public int? Actividad { get; set; }
        public string DescripcionActividad { get; set; }
        public string Area { get; set; }
        public string Departamento { get; set; }
        public string ProgramaSocial { get; set; }
        public int? IdEstatus { get; set; }
    }
}
