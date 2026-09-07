using System;

namespace Metas.Entity;

public partial class RangoDesempenio
{
    public int Id { get; set; }

    public string Nombre { get; set; }

    public decimal Minimo { get; set; }

    public decimal? Maximo { get; set; }

    public string ClaseColor { get; set; }

    public bool RequiereJustificacion { get; set; }

    public int Orden { get; set; }
}
