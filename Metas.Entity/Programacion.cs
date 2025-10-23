using System;
using System.Collections.Generic;

namespace Metas.Entity;

public partial class Programacion
{
    public int IdRegistro { get; set; }

    public DateOnly? FechaSolicitud { get; set; }

    public string? Area { get; set; }

    public string? CorreoElectro { get; set; }

    public string? Pp { get; set; }

    public string? NComponente { get; set; }

    public int? NActividad { get; set; }

    public string? Justificacion { get; set; }

    public string? DescripcionDocumento { get; set; }

    public string? RecursoFederal { get; set; }

    public string? RecursoEstatal { get; set; }

    public string? ProgramaSocial { get; set; }

    public string? DescripcionActividad { get; set; }

    public string? NombreIndicador { get; set; }

    public string? DefinicionIndicador { get; set; }

    public decimal? Primero { get; set; }

    public decimal? Segundo { get; set; }

    public decimal? Tercero { get; set; }

    public decimal? Cuarto { get; set; }

    public int? Servicio1 { get; set; }

    public int? Servicio2 { get; set; }

    public int? Servicio3 { get; set; }

    public int? Servicio4 { get; set; }

    public int? Personas1 { get; set; }

    public int? Personas2 { get; set; }

    public int? Personas3 { get; set; }

    public int? Personas4 { get; set; }

    public string? UnidadMedida { get; set; }

    public string? MediosVerificac { get; set; }

    public string? SerieInfo { get; set; }

    public string? FuenteInfo { get; set; }

    public string? IntervienenDelegaciones { get; set; }

    public string? IntervienenDelegacionesManera { get; set; }

    public int? Anos { get; set; }

    public int? Anos2 { get; set; }

    public decimal? Valor { get; set; }

    public decimal? Valor2 { get; set; }

    public int? BienServicio { get; set; }

    public int? BienServicio2 { get; set; }

    public int? NoPersonas { get; set; }

    public decimal? NoPersonas2 { get; set; }

    public string? Acumulable { get; set; }

    public int? Mes1 { get; set; }

    public int? Mes2 { get; set; }

    public int? Mes3 { get; set; }

    public int? Mes4 { get; set; }

    public int? Mes5 { get; set; }

    public int? Mes6 { get; set; }

    public int? Mes11 { get; set; }

    public int? Mes12 { get; set; }

    public int? Mes13 { get; set; }

    public int? Mes14 { get; set; }

    public int? Mes15 { get; set; }

    public int? Mes16 { get; set; }

    public int? Mes7 { get; set; }

    public int? Mes8 { get; set; }

    public int? Mes9 { get; set; }

    public int? Mes10 { get; set; }

    public int? Mes111 { get; set; }

    public int? Mes121 { get; set; }

    public int? Mes17 { get; set; }

    public int? Mes18 { get; set; }

    public int? Mes19 { get; set; }

    public int? Mes110 { get; set; }

    public int? Mes1111 { get; set; }

    public int? Mes112 { get; set; }

    public int? Totalanos { get; set; }

    public decimal? Totalanos2 { get; set; }

    public string? Actividad1 { get; set; }

    public DateOnly? FechaProgramacion1 { get; set; }

    public string? Actividad2 { get; set; }

    public DateOnly? FechaProgramacion2 { get; set; }

    public string? Actividad3 { get; set; }

    public DateOnly? FechaProgramacion3 { get; set; }

    public string? Actividad4 { get; set; }

    public DateOnly? FechaProgramacion4 { get; set; }

    public string? Actividad5 { get; set; }

    public DateOnly? FechaProgramacion5 { get; set; }

    public int? IdLlenado { get; set; }

    public string? SerieInfo2 { get; set; }

    public string? Beneficiarios { get; set; }

    public string? Frecuencia1 { get; set; }

    public string? Frecuencia2 { get; set; }

    public string? Frecuencia3 { get; set; }

    public string? Frecuencia4 { get; set; }

    public string? Frecuencia5 { get; set; }

    public string? ElaboraNombre { get; set; }

    public string? ElaboroCargo { get; set; }

    public string? ValidoNombre { get; set; }

    public string? ValidoCargo { get; set; }

    public string? AutorizoNombre { get; set; }

    public string? AutorizoCargo { get; set; }

    public int? IdEstatus { get; set; }

    public virtual ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();

    public virtual LlenadoInterno? IdLlenadoNavigation { get; set; }

    public virtual ICollection<ServiciosMunicipio> ServiciosMunicipios { get; set; } = new List<ServiciosMunicipio>();
}
