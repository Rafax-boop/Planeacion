using System;
using System.Collections.Generic;

namespace Metas.Entity;

public partial class EvidenciasAlbergue
{
    public int IdEvidenciaCasaAlbergues { get; set; }

    public int? IdActividad { get; set; }

    public string? Imagen1 { get; set; }

    public string? Imagen2 { get; set; }

    public string? Imagen3 { get; set; }

    public DateOnly? Fecha1 { get; set; }

    public string? Evento1 { get; set; }

    public string? Municipio1 { get; set; }

    public string? Descripcion1 { get; set; }

    public DateOnly? Fecha2 { get; set; }

    public string? Evento2 { get; set; }

    public string? Municipio2 { get; set; }

    public string? Descripcion2 { get; set; }

    public DateOnly? Fecha3 { get; set; }

    public string? Evento3 { get; set; }

    public string? Municipio3 { get; set; }

    public string? Descripcion3 { get; set; }

    public int? TotalBien { get; set; }

    public int? MujeresBene { get; set; }

    public int? HombresBene { get; set; }

    public int? TotalBene { get; set; }

    public int? Mes { get; set; }

    public int? InicialHombres { get; set; }

    public int? InicialMujeres { get; set; }

    public int? AltasHombres { get; set; }

    public int? AltasMujeres { get; set; }

    public int? BajasHombres { get; set; }

    public int? BajasMujeres { get; set; }

    public int? ResultadoBajaTotalHombres { get; set; }

    public int? ResultadoBajaTotalMujeres { get; set; }

    public string? Evidencia { get; set; }
}
