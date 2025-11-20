using System;
using System.Collections.Generic;

namespace Metas.Entity;

public partial class Estatus
{
    public int IdEstatus { get; set; }

    public string? Valor { get; set; }

    public virtual ICollection<Programacion> Programacions { get; set; } = new List<Programacion>();
}
