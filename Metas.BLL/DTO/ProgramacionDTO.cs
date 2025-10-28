using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Metas.BLL.DTO
{
    public class ProgramacionDTO
    {
        // ========================================
        // DATOS GENERALES
        // ========================================
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("area")]
        public string Area { get; set; }
        [JsonPropertyName("departamento")]
        public string Departamento { get; set; }

        [JsonPropertyName("correoContacto")]
        public string CorreoContacto { get; set; }

        [JsonPropertyName("pp")]
        public string Pp { get; set; }

        [JsonPropertyName("componente")]
        public int Componente { get; set; }

        [JsonPropertyName("nComponente")]
        public string NComponente { get; set; }

        [JsonPropertyName("nActividad")]
        public int NActividad { get; set; }

        [JsonPropertyName("justificacion")]
        public string Justificacion { get; set; }

        [JsonPropertyName("descripcionDocumento")]
        public string DescripcionDocumento { get; set; }

        [JsonPropertyName("recursoFederal")]
        public string RecursoFederal { get; set; }

        [JsonPropertyName("recursoEstatal")]
        public string RecursoEstatal { get; set; }

        [JsonPropertyName("programaSocial")]
        public string ProgramaSocial { get; set; }

        [JsonPropertyName("descripcionActividad")]
        public string DescripcionActividad { get; set; }

        [JsonPropertyName("nombreIndicador")]
        public string NombreIndicador { get; set; }

        [JsonPropertyName("definicionIndicador")]
        public string DefinicionIndicador { get; set; }

        [JsonPropertyName("unidadMedida")]
        public string UnidadMedida { get; set; }

        [JsonPropertyName("mediosVerificacion")]
        public string MediosVerificacion { get; set; }

        [JsonPropertyName("serieInformacionDesde")]
        public string SerieInformacionDesde { get; set; }

        [JsonPropertyName("serieInformacionHasta")]
        public string SerieInformacionHasta { get; set; }

        [JsonPropertyName("fuenteInformacion")]
        public string FuenteInformacion { get; set; }

        [JsonPropertyName("intervienenDelegaciones")]
        public string IntervienenDelegaciones { get; set; }

        [JsonPropertyName("intervienenDelegacionesManera")]
        public string IntervienenDelegacionesManera { get; set; }
        [JsonPropertyName("selectAcumulable")]
        public string SelectAcumulable { get; set; }

        [JsonPropertyName("estatus")]
        public int Estatus { get; set; }

        [JsonPropertyName("beneficiarios")]
        public string Beneficiarios { get; set; }
        // ========================================
        // LÍNEA BASE
        // ========================================
        [JsonPropertyName("anoBase")]
        public int AnoBase { get; set; }

        [JsonPropertyName("porcentajeBase")]
        public decimal PorcentajeBase { get; set; }

        [JsonPropertyName("servicioBase")]
        public int ServicioBase { get; set; }

        [JsonPropertyName("personasBase")]
        public int PersonasBase { get; set; }

        // ========================================
        // META ANUAL
        // ========================================
        [JsonPropertyName("anoMeta")]
        public int AnoMeta { get; set; }

        [JsonPropertyName("porcentajeMeta")]
        public decimal PorcentajeMeta { get; set; }

        [JsonPropertyName("servicioMeta")]
        public int ServicioMeta { get; set; }

        [JsonPropertyName("personasMeta")]
        public decimal PersonasMeta { get; set; }

        // ========================================
        // TRIMESTRES SERVICIOS (Grupo 1)
        // ========================================
        [JsonPropertyName("primerServicio")]
        public int PrimerServicio { get; set; }

        [JsonPropertyName("segundoServicio")]
        public int SegundoServicio { get; set; }

        [JsonPropertyName("tercerServicio")]
        public int TercerServicio { get; set; }

        [JsonPropertyName("cuartoServicio")]
        public int CuartoServicio { get; set; }

        // ========================================
        // TRIMESTRES PERSONAS (Grupo 2)
        // ========================================
        [JsonPropertyName("primerPersona")]
        public int PrimerPersona { get; set; }

        [JsonPropertyName("segundoPersona")]
        public int SegundoPersona { get; set; }

        [JsonPropertyName("tercerPersona")]
        public int TercerPersona { get; set; }

        [JsonPropertyName("cuartoPersona")]
        public int CuartoPersona { get; set; }

        // ========================================
        // MESES SERVICIOS (Grupo 1) - 12 meses
        // ========================================
        [JsonPropertyName("mesesServicios")]
        public List<int> MesesServicios { get; set; } = new List<int>();
        [JsonPropertyName("totalAnos")]
        public int TotalAnos { get; set; }

        // ========================================
        // MESES PERSONAS (Grupo 2) - 12 meses
        // ========================================
        [JsonPropertyName("mesesPersonas")]
        public List<int> MesesPersonas { get; set; } = new List<int>();
        [JsonPropertyName("totalAnos2")]
        public decimal TotalAnos2 { get; set; }

        // ========================================
        // MUNICIPIOS SERVICIOS (Grupo 1)
        // ========================================
        [JsonPropertyName("municipiosServicios")]
        public List<DTOMunicipioAgregado> MunicipiosServicios { get; set; } = new List<DTOMunicipioAgregado>();

        // ========================================
        // MUNICIPIOS PERSONAS (Grupo 2)
        // ========================================
        [JsonPropertyName("municipiosPersonas")]
        public List<DTOMunicipioAgregado> MunicipiosPersonas { get; set; } = new List<DTOMunicipioAgregado>();

        // ========================================
        // ACCIONES (1-6)
        // ========================================
        [JsonPropertyName("acciones")]
        public List<DTOAccion> Acciones { get; set; } = new List<DTOAccion>();

        // ========================================
        // FIRMAS
        // ========================================
        [JsonPropertyName("elaboraNombre")]
        public string ElaboraNombre { get; set; }

        [JsonPropertyName("elaboroCargo")]
        public string ElaboroCargo { get; set; }

        [JsonPropertyName("revisionNombre")]
        public string RevisionNombre { get; set; }

        [JsonPropertyName("revisionCargo")]
        public string RevisionCargo { get; set; }

        [JsonPropertyName("autorizacionNombre")]
        public string AutorizacionNombre { get; set; }

        [JsonPropertyName("autorizacionCargo")]
        public string AutorizacionCargo { get; set; }
    }

    // DTO para municipios agregados en las tablas
    public class DTOMunicipioAgregado
    {
        [JsonPropertyName("idMunicipio")]
        public int IdMunicipio { get; set; }

        [JsonPropertyName("cantidad")]
        public int Cantidad { get; set; }
    }

    // DTO para acciones
    public class DTOAccion
    {
        [JsonPropertyName("descripcion")]
        public string Descripcion { get; set; }

        [JsonPropertyName("frecuencia")]
        public string Frecuencia { get; set; }

        [JsonPropertyName("fechaInicio")]
        public DateOnly? FechaInicio { get; set; }
    }
}