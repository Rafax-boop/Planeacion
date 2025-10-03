using System;
using System.Collections.Generic;

namespace Metas.Entity;

public partial class EvidenciaBien
{
    public int IdBien { get; set; }

    public int? TotalBienes { get; set; }

    public int? TotalPersonas { get; set; }

    public int? Mujeres { get; set; }

    public int? Hombres { get; set; }

    public int? IdActividad { get; set; }

    public int? Mes { get; set; }

    public string? Evidencia { get; set; }

    public virtual LlenadoInterno? IdActividadNavigation { get; set; }
}
