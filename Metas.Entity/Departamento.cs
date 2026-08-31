using System;
using System.Collections.Generic;

namespace Metas.Entity;

public partial class Departamento
{
    public int IdDepartamento { get; set; }

    public string? UnidadRepresentante { get; set; }

    public string? Area { get; set; }

    public string? Departamento1 { get; set; }

    public string? Unidad { get; set; }

    public string? ReglasOperacion { get; set; }

    public string? LineamientosOperacion { get; set; }

    public string? ArbolProblemasObjetivos { get; set; }

    public string? MetodologiaPadron { get; set; }

    public string? Diagnostico { get; set; }

    public string? Justificacion { get; set; }
}
