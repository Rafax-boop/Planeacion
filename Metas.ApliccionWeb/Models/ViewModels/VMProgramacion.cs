using Microsoft.AspNetCore.Mvc.Rendering;

namespace Metas.AplicacionWeb.Models.ViewModels
{
    public class VMProgramacion
    {
        public int AnoFiscal { get; set; }
        public int DepartamentoId { get; set; }
        public string AreaNombre { get; set; }
        public string DepartamentoNombre { get; set; }
        public string CorreoContacto { get; set; }
        public List<SelectListItem> ListaProgramas { get; set; } = new();
        public List<SelectListItem> ListaComponentes { get; set; } = new();
        public List<SelectListItem> ListaMedidas { get; set; } = new();
        public List<SelectListItem> ListaMunicipios { get; set; } = new();

    }
    public class ComentarioViewModel
    {
        public int ComentarioId { get; set; } // 1-27
        public int Campo { get; set; } // 4-30
        public string Texto { get; set; }
        public string FechaHora { get; set; }
        public string Descripcion { get; set; }
    }
}
