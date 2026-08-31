using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metas.BLL.DTO
{
    public class DepartamentoDocumentosDTO
    {
        public int IdDepartamento { get; set; }

        // =====================================
        // SECCIÓN ARCHIVOS (Documentos)
        // Los 5 documentos que pueden cargarse por departamento
        // =====================================
        public IFormFile InputReglasOperacion { get; set; }
        public IFormFile InputLineamientosOperacion { get; set; }
        public IFormFile InputArbolProblemasObjetivos { get; set; }
        public IFormFile InputMetodologiaPadron { get; set; }
        public IFormFile InputDiagnostico { get; set; }

        // =====================================
        // RUTAS EXISTENTES (para conservar/reemplazar)
        // =====================================
        public string RutaReglasOperacion { get; set; }
        public string RutaLineamientosOperacion { get; set; }
        public string RutaArbolProblemasObjetivos { get; set; }
        public string RutaMetodologiaPadron { get; set; }
        public string RutaDiagnostico { get; set; }

        // =====================================
        // JUSTIFICACIÓN (requerida si faltan obligatorios)
        // =====================================
        public string Justificacion { get; set; }
    }
}
