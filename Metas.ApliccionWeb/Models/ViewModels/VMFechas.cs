namespace Metas.AplicacionWeb.Models.ViewModels
{
    public class VMFechas
    {
        public int IdFechaCaptura { get; set; }
        public DateOnly FechaInicio { get; set; }
        public DateOnly FechaFin { get; set; }
        public int Ano { get; set; }
        public string Mes { get; set; }
    }
}
