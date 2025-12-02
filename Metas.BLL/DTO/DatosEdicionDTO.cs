using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metas.BLL.DTO
{
    public class DatosEdicionDTO
    {
        // Identificador
        public int IdProceso { get; set; }
        public int AnoFiscal { get; set; }
        public string Departamento { get; set; }

        // Campos de metadatos (Pueden ser de solo lectura o para referencia)
        public int? Componente { get; set; }
        public int? Actividad { get; set; }
        public int? PP { get; set; }
        public string ProgramaSocial { get; set; }
        public string UnidadMedida { get; set; }

        // Campo principal editable
        public string DescripcionActividad { get; set; }

        // Metas Programadas
        public int? TotalEnero { get; set; }
        public int? TotalFebrero { get; set; }
        public int? TotalMarzo { get; set; }
        public int? TotalAbril { get; set; }
        public int? TotalMayo { get; set; }
        public int? TotalJunio { get; set; }
        public int? TotalJulio { get; set; }
        public int? TotalAgosto { get; set; }
        public int? TotalSeptiembre { get; set; }
        public int? TotalOctubre { get; set; }
        public int? TotalNoviembre { get; set; }
        public int? TotalDiciembre { get; set; }

        // Personas Programadas
        public int? EneroPersona { get; set; }
        public int? FebreroPersona { get; set; }
        public int? MarzoPersona { get; set; }
        public int? AbrilPersona { get; set; }
        public int? MayoPersona { get; set; }
        public int? JunioPersona { get; set; }
        public int? JulioPersona { get; set; }
        public int? AgostoPersona { get; set; }
        public int? SeptiembrePersona { get; set; }
        public int? OctubrePersona { get; set; }
        public int? NoviembrePersona { get; set; }
        public int? DiciembrePersona { get; set; }

        public List<SelectListItem> ListaMedidas { get; set; } = new List<SelectListItem>();
    }
}
