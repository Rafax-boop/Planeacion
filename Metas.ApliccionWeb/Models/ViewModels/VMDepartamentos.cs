using Microsoft.AspNetCore.Mvc.Rendering;

namespace Metas.AplicacionWeb.Models.ViewModels
{
    public class VMDepartamentos
    {
        public List<SelectListItem> ListaDepartamentos { get; set; } = new();
        public List<SelectListItem> ListaAreas { get; set; } = new();
    }
}
