using System;
using System.Collections.Generic;

namespace Metas.Entity;

public partial class LlenadoExterno
{
    public int IdLlenado { get; set; }

    public int? Realizado { get; set; }

    public int? PersonasAtendidas { get; set; }

    public int? MujeresAtendidas { get; set; }

    public int? HombresAtendidos { get; set; }

    public int? _03anos { get; set; }

    public int? _48anos { get; set; }

    public int? _912anos { get; set; }

    public int? _1317anos { get; set; }

    public int? _1829anos { get; set; }

    public int? _3059anos { get; set; }

    public int? _60amasanos { get; set; }

    public int? Indigena { get; set; }

    public int? Meses { get; set; }

    public int? IdProceso { get; set; }

    public string? Evidencia { get; set; }

    public string? Justificacion { get; set; }

    public int? NoDefinida { get; set; }

    public virtual LlenadoInterno? IdProcesoNavigation { get; set; }
}
