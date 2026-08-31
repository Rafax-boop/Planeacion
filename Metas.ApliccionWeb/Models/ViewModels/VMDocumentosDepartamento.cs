using Metas.BLL.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Metas.AplicacionWeb.Models.ViewModels
{
    public class VMDocumentosDepartamento
    {
        public List<SelectListItem> ListaDepartamentos { get; set; } = new();
        public int DepartamentoSeleccionado { get; set; }
        public bool EsAdministrador { get; set; }

        public string ReglasOperacion { get; set; }
        public string LineamientosOperacion { get; set; }
        public string ArbolProblemasObjetivos { get; set; }
        public string MetodologiaPadron { get; set; }
        public string Diagnostico { get; set; }
        public string Justificacion { get; set; }

        public bool TieneReglas => !string.IsNullOrEmpty(ReglasOperacion);
        public bool TieneLineamientos => !string.IsNullOrEmpty(LineamientosOperacion);
        public bool TieneArbol => !string.IsNullOrEmpty(ArbolProblemasObjetivos);
        public bool TieneMetodologia => !string.IsNullOrEmpty(MetodologiaPadron);
        public bool TieneDiagnostico => !string.IsNullOrEmpty(Diagnostico);
    }
}
