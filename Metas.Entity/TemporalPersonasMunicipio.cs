using System;
using System.Collections.Generic;

namespace Metas.Entity;

public partial class TemporalPersonasMunicipio
{
    public int IdTemporal { get; set; }

    public int? IdMunicipio { get; set; }

    public int? NumeroBien { get; set; }

    public string? Tiempo { get; set; }

    public virtual Municipio? IdMunicipioNavigation { get; set; }
}
