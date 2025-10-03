using System;
using System.Collections.Generic;

namespace Metas.Entity;

public partial class EvidenciaAlimentarium
{
    public int IdEvidenciaAlimentaria { get; set; }

    public int? IdActividad { get; set; }

    public string? Imagen1 { get; set; }

    public string? Imagen2 { get; set; }

    public string? Imagen3 { get; set; }

    public DateOnly? Fecha1 { get; set; }

    public string? Evento1 { get; set; }

    public string? Descripcion1 { get; set; }

    public DateOnly? Fecha2 { get; set; }

    public string? Evento2 { get; set; }

    public string? Descripcion2 { get; set; }

    public DateOnly? Fecha3 { get; set; }

    public string? Evento3 { get; set; }

    public string? Descripcion3 { get; set; }

    public int? TotalBien { get; set; }

    public int? MujeresBene { get; set; }

    public int? HombresBene { get; set; }

    public int? TotalBene { get; set; }

    public int? Mes { get; set; }

    public int? InicialHombre { get; set; }

    public int? InicialMujer { get; set; }

    public int? AltasHombre { get; set; }

    public int? AltasMujer { get; set; }

    public int? BajasHombre { get; set; }

    public int? BajasMujer { get; set; }

    public string? Evidencia { get; set; }

    public virtual LlenadoInterno? IdActividadNavigation { get; set; }
}
