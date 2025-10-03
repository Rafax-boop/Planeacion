using System;
using System.Collections.Generic;

namespace Metas.Entity;

public partial class Municipio
{
    public int IdMunicipio { get; set; }

    public string? NumeroRegion { get; set; }

    public string? NombreRegion { get; set; }

    public int? ClaveMuni { get; set; }

    public string? NombreMunicipios { get; set; }

    public virtual ICollection<PersonasMunicipio> PersonasMunicipios { get; set; } = new List<PersonasMunicipio>();

    public virtual ICollection<ServiciosMunicipio> ServiciosMunicipios { get; set; } = new List<ServiciosMunicipio>();

    public virtual ICollection<TemporalPersonasMunicipio> TemporalPersonasMunicipios { get; set; } = new List<TemporalPersonasMunicipio>();

    public virtual ICollection<TemporalServiciosMunicipio> TemporalServiciosMunicipios { get; set; } = new List<TemporalServiciosMunicipio>();

    public virtual ICollection<Vinculacion> Vinculacions { get; set; } = new List<Vinculacion>();
}
