using System;
using System.Collections.Generic;

namespace Metas.Entity;

public partial class AnoHabilitar
{
    public int IdFecha { get; set; }

    public DateOnly? Fecha { get; set; }

    public int? Ano { get; set; }

    public DateOnly? FechaFin { get; set; }
}
