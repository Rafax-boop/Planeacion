using System;
using System.Collections.Generic;

namespace Metas.Entity;

public partial class Vinculacion
{
    public int IdVinculacion { get; set; }

    public int? IdLlenado { get; set; }

    public int? IdMunicipio { get; set; }

    public int? Mes { get; set; }

    public int? NumeroBien { get; set; }

    public virtual LlenadoInterno? IdLlenadoNavigation { get; set; }

    public virtual Municipio? IdMunicipioNavigation { get; set; }
}
