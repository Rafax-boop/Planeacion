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
    }
}
