using System;
using System.Collections.Generic;

namespace Metas.Entity;

public partial class Validacione
{
    public int IdValidacion { get; set; }

    public bool? Validacion1 { get; set; }

    public bool? Validacion2 { get; set; }

    public bool? Validacion3 { get; set; }

    public bool? Validacion4 { get; set; }

    public bool? Validacion5 { get; set; }

    public bool? Validacion6 { get; set; }

    public bool? Validacion7 { get; set; }

    public bool? Validacion8 { get; set; }

    public bool? Validacion9 { get; set; }

    public bool? Validacion10 { get; set; }

    public bool? Validacion11 { get; set; }

    public bool? Validacion12 { get; set; }

    public int? IdProceso { get; set; }

    public virtual LlenadoInterno? IdProcesoNavigation { get; set; }
}
