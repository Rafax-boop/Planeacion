using System;
using System.Collections.Generic;

namespace Metas.Entity;

public partial class FechaCaptura
{
    public int IdFechaCaptura { get; set; }

    public DateOnly? FechaInicio { get; set; }

    public DateOnly? FechaFin { get; set; }

    public int? Ano { get; set; }

    public string? Mes { get; set; }
}
