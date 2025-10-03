using System;
using System.Collections.Generic;

namespace Metas.Entity;

public partial class Pp
{
    public int IdPp { get; set; }

    public string? NombrePp { get; set; }

    public string? Clave { get; set; }

    public virtual ICollection<LlenadoInterno> LlenadoInternos { get; set; } = new List<LlenadoInterno>();
}
