using System;
using System.Collections.Generic;

namespace Metas.Entity;

public partial class LlenadoInterno
{
    public int IdProceso { get; set; }

    public string? Pp { get; set; }

    public int? Componente { get; set; }

    public int? Actividad { get; set; }

    public string? DescripcionActividad { get; set; }

    public string? Area { get; set; }

    public string? Departamento { get; set; }

    public string? ProgramaSocial { get; set; }

    public int? Ano { get; set; }

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

    public string? UnidadMedida { get; set; }

    public int? TotalEneroRealizado { get; set; }

    public int? TotalFebreroRealizado { get; set; }

    public int? TotalMarzoRealizado { get; set; }

    public int? TotalAbrilRealizado { get; set; }

    public int? TotalMayoRealizado { get; set; }

    public int? TotalJunioRealizado { get; set; }

    public int? TotalJulioRealizado { get; set; }

    public int? TotalAgostoRealizado { get; set; }

    public int? TotalSeptiembreRealizado { get; set; }

    public int? TotalOctubreRealizado { get; set; }

    public int? TotalNoviembreRealizado { get; set; }

    public int? TotalDiciembreRealizado { get; set; }

    public int? TotalProgramado { get; set; }

    public int? TotalRealizado { get; set; }

    public int? PersonasProgramadas { get; set; }

    public DateOnly? FechaEnero { get; set; }

    public DateOnly? FechaFebrero { get; set; }

    public DateOnly? FechaMarzo { get; set; }

    public DateOnly? FechaAbril { get; set; }

    public DateOnly? FechaMayo { get; set; }

    public DateOnly? FechaJunio { get; set; }

    public DateOnly? FechaJulio { get; set; }

    public DateOnly? FechaAgosto { get; set; }

    public DateOnly? FechaSeptiembre { get; set; }

    public DateOnly? FechaOctubre { get; set; }

    public DateOnly? FechaNoviembre { get; set; }

    public DateOnly? FechaDiciembre { get; set; }

    public int? Idpp { get; set; }

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

    public int? TotalPersona { get; set; }

    public string? NombreRealizo { get; set; }

    public string? CargoRealizo { get; set; }

    public string? NombreValido { get; set; }

    public string? CargoValido { get; set; }

    public virtual Pp? IdppNavigation { get; set; }

    public virtual ICollection<Programacion> Programacions { get; set; } = new List<Programacion>();

    public virtual ICollection<Vinculacion> Vinculacions { get; set; } = new List<Vinculacion>();
}
