using System;
using System.Collections.Generic;

namespace Metas.Entity;

public partial class ServiciosMunicipio
{
    public int IdRegistroVinculacion { get; set; }

    public int? IdLlenado { get; set; }

    public int? IdMunicipio { get; set; }

    public int? NumeroBien { get; set; }

    public virtual Programacion? IdLlenadoNavigation { get; set; }

    public virtual Municipio? IdMunicipioNavigation { get; set; }
}
