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
        public string NombreEstatus { get; set; }
        public bool? Enero { get; set; }
        public bool? Febrero { get; set; }
        public bool? Marzo { get; set; }
        public bool? Abril { get; set; }
        public bool? Mayo { get; set; }
        public bool? Junio { get; set; }
        public bool? Julio { get; set; }
        public bool? Agosto { get; set; }
        public bool? Septiembre { get; set; }
        public bool? Octubre { get; set; }
        public bool? Noviembre { get; set; }
        public bool? Diciembre { get; set; }
        public int? Total { get; set; }
        public string? UnidadMedida { get; set; }
        public string Mes { get; set; }
        public DateOnly? FechaFin { get; set; }
        public Dictionary<int, RangoDeFechas> FechasCaptura { get; set; }
    }

    public struct RangoDeFechas
    {
        public DateOnly? FechaInicio { get; set; }
        public DateOnly? FechaFin { get; set; }
    }
}
