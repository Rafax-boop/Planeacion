using System;
using System.Collections.Generic;

namespace Metas.Entity;

public partial class EvidenciaCapacitacione
{
    public int IdCapacitaciones { get; set; }

    public int? IdActividad { get; set; }

    public string? NombreTaller { get; set; }

    public string? Municipio { get; set; }

    public DateOnly? FechaInicio { get; set; }

    public DateOnly? FechaTermino { get; set; }

    public string? Modo { get; set; }

    public int? HombreBene { get; set; }

    public int? MujeresBene { get; set; }

    public int? TotalPersonas { get; set; }

    public int? Mes { get; set; }

    public string? Evidencia { get; set; }

    public virtual LlenadoInterno? IdActividadNavigation { get; set; }
}
