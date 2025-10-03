using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Metas.Entity;

namespace Metas.DAL.DBContext;

public partial class MetasContext : DbContext
{
    public MetasContext()
    {
    }

    public MetasContext(DbContextOptions<MetasContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AnoHabilitar> AnoHabilitars { get; set; }

    public virtual DbSet<CapturaProgramacion> CapturaProgramacions { get; set; }

    public virtual DbSet<Comentario> Comentarios { get; set; }

    public virtual DbSet<CorreosInstitucionale> CorreosInstitucionales { get; set; }

    public virtual DbSet<Departamento> Departamentos { get; set; }

    public virtual DbSet<Estatus> Estatuses { get; set; }

    public virtual DbSet<Evidencia> Evidencias { get; set; }

    public virtual DbSet<EvidenciaAlimentarium> EvidenciaAlimentaria { get; set; }

    public virtual DbSet<EvidenciaBien> EvidenciaBiens { get; set; }

    public virtual DbSet<EvidenciaCapacitacione> EvidenciaCapacitaciones { get; set; }

    public virtual DbSet<EvidenciaFotografica> EvidenciaFotograficas { get; set; }

    public virtual DbSet<EvidenciaFotograficaUbr> EvidenciaFotograficaUbrs { get; set; }

    public virtual DbSet<EvidenciasAlbergue> EvidenciasAlbergues { get; set; }

    public virtual DbSet<FechaCaptura> FechaCapturas { get; set; }

    public virtual DbSet<LlenadoExterno> LlenadoExternos { get; set; }

    public virtual DbSet<LlenadoInterno> LlenadoInternos { get; set; }

    public virtual DbSet<Municipio> Municipios { get; set; }

    public virtual DbSet<PersonasMunicipio> PersonasMunicipios { get; set; }

    public virtual DbSet<Pp> Pps { get; set; }

    public virtual DbSet<PpCompuesto> PpCompuestos { get; set; }

    public virtual DbSet<Programa> Programas { get; set; }

    public virtual DbSet<Programacion> Programacions { get; set; }

    public virtual DbSet<ServiciosMunicipio> ServiciosMunicipios { get; set; }

    public virtual DbSet<TemporalPersonasMunicipio> TemporalPersonasMunicipios { get; set; }

    public virtual DbSet<TemporalServiciosMunicipio> TemporalServiciosMunicipios { get; set; }

    public virtual DbSet<UnidadMedidum> UnidadMedida { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Validacione> Validaciones { get; set; }

    public virtual DbSet<Vinculacion> Vinculacions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AnoHabilitar>(entity =>
        {
            entity.HasKey(e => e.IdFecha).HasName("PK__AnoHabil__8D0F205A8DED97C6");

            entity.ToTable("AnoHabilitar");
        });

        modelBuilder.Entity<CapturaProgramacion>(entity =>
        {
            entity.HasKey(e => e.IdFechaCaptura).HasName("PK__CapturaP__0096601FA19851CD");

            entity.ToTable("CapturaProgramacion");

            entity.Property(e => e.Concepto).HasMaxLength(20);
        });

        modelBuilder.Entity<Comentario>(entity =>
        {
            entity.HasKey(e => e.IdComentario).HasName("PK__Comentar__DDBEFBF9EB384866");

            entity.HasOne(d => d.IdProgramacionNavigation).WithMany(p => p.Comentarios)
                .HasForeignKey(d => d.IdProgramacion)
                .HasConstraintName("FK_Comentarios_Programacion");
        });

        modelBuilder.Entity<CorreosInstitucionale>(entity =>
        {
            entity.HasKey(e => e.IdCorreo).HasName("PK__CorreosI__872F8EAE12A2C2D6");

            entity.Property(e => e.Area).HasMaxLength(500);
            entity.Property(e => e.CorreoElectronico).HasMaxLength(800);
            entity.Property(e => e.Departamentos).HasMaxLength(500);
        });

        modelBuilder.Entity<Departamento>(entity =>
        {
            entity.HasKey(e => e.IdDepartamento).HasName("PK__Departam__787A433D1D2DA1DB");

            entity.ToTable("Departamento");

            entity.Property(e => e.Area).HasMaxLength(200);
            entity.Property(e => e.Departamento1)
                .HasMaxLength(200)
                .HasColumnName("Departamento");
            entity.Property(e => e.Unidad).HasMaxLength(50);
            entity.Property(e => e.UnidadRepresentante).HasMaxLength(200);
        });

        modelBuilder.Entity<Estatus>(entity =>
        {
            entity.HasKey(e => e.IdEstatus).HasName("PK__Estatus__B32BA1C752B36F6B");

            entity.ToTable("Estatus");
        });

        modelBuilder.Entity<Evidencia>(entity =>
        {
            entity.HasKey(e => e.IdEvidencia).HasName("PK__Evidenci__C602EF7EF25D94FA");

            entity.Property(e => e.Evidencia1)
                .HasMaxLength(800)
                .HasColumnName("Evidencia");
        });

        modelBuilder.Entity<EvidenciaAlimentarium>(entity =>
        {
            entity.HasKey(e => e.IdEvidenciaAlimentaria).HasName("PK__Evidenci__5734E1BB1C7140EC");

            entity.Property(e => e.Evidencia).HasMaxLength(1000);

            entity.HasOne(d => d.IdActividadNavigation).WithMany(p => p.EvidenciaAlimentaria)
                .HasForeignKey(d => d.IdActividad)
                .HasConstraintName("FK_EvidenciaAlimentaria_LlenadoInterno");
        });

        modelBuilder.Entity<EvidenciaBien>(entity =>
        {
            entity.HasKey(e => e.IdBien).HasName("PK__Evidenci__24A519299BF29C64");

            entity.ToTable("EvidenciaBien");

            entity.Property(e => e.Evidencia).HasMaxLength(1000);

            entity.HasOne(d => d.IdActividadNavigation).WithMany(p => p.EvidenciaBiens)
                .HasForeignKey(d => d.IdActividad)
                .HasConstraintName("FK_EvidenciaBien_LlenadoInterno");
        });

        modelBuilder.Entity<EvidenciaCapacitacione>(entity =>
        {
            entity.HasKey(e => e.IdCapacitaciones).HasName("PK__Evidenci__6C9CB0F7B33A2E37");

            entity.Property(e => e.Evidencia).HasMaxLength(1000);
            entity.Property(e => e.Modo).HasMaxLength(150);
            entity.Property(e => e.Municipio).HasMaxLength(500);

            entity.HasOne(d => d.IdActividadNavigation).WithMany(p => p.EvidenciaCapacitaciones)
                .HasForeignKey(d => d.IdActividad)
                .HasConstraintName("FK_EvidenciaCapacitaciones_LlenadoInterno");
        });

        modelBuilder.Entity<EvidenciaFotografica>(entity =>
        {
            entity.HasKey(e => e.IdEvidenciaFotografica).HasName("PK__Evidenci__805DDA416D6DAC4D");

            entity.ToTable("EvidenciaFotografica");

            entity.Property(e => e.Evidencia).HasMaxLength(1000);
            entity.Property(e => e.Municipio1).HasMaxLength(180);
            entity.Property(e => e.Municipio2).HasMaxLength(180);
            entity.Property(e => e.Municipio3).HasMaxLength(180);
        });

        modelBuilder.Entity<EvidenciaFotograficaUbr>(entity =>
        {
            entity.HasKey(e => e.IdEvidenciaFotografica).HasName("PK__Evidenci__805DDA4174DBFA30");

            entity.ToTable("EvidenciaFotograficaUBRS");

            entity.Property(e => e.Crismes).HasColumnName("CRISMes");
            entity.Property(e => e.CrismesSupervisadas).HasColumnName("CRISMesSupervisadas");
            entity.Property(e => e.Evidencia).HasMaxLength(1000);
            entity.Property(e => e.Municipio1).HasMaxLength(180);
            entity.Property(e => e.Municipio2).HasMaxLength(180);
            entity.Property(e => e.Municipio3).HasMaxLength(180);
            entity.Property(e => e.NumeroUbrsmes).HasColumnName("NumeroUBRSmes");
            entity.Property(e => e.Ubrssupervisadasmes).HasColumnName("UBRSSupervisadasmes");
        });

        modelBuilder.Entity<EvidenciasAlbergue>(entity =>
        {
            entity.HasKey(e => e.IdEvidenciaCasaAlbergues).HasName("PK__Evidenci__950DD33BD6A0402B");

            entity.Property(e => e.Evidencia).HasMaxLength(1000);
            entity.Property(e => e.Municipio1).HasMaxLength(180);
            entity.Property(e => e.Municipio2).HasMaxLength(180);
            entity.Property(e => e.Municipio3).HasMaxLength(180);
        });

        modelBuilder.Entity<FechaCaptura>(entity =>
        {
            entity.HasKey(e => e.IdFechaCaptura).HasName("PK__FechaCap__0096601F83354380");

            entity.ToTable("FechaCaptura");

            entity.Property(e => e.Mes).HasMaxLength(15);
        });

        modelBuilder.Entity<LlenadoExterno>(entity =>
        {
            entity.HasKey(e => e.IdLlenado).HasName("PK__LlenadoE__0D4652245F824BE6");

            entity.ToTable("LlenadoExterno");

            entity.Property(e => e.Evidencia).HasMaxLength(300);
            entity.Property(e => e.Justificacion).HasMaxLength(300);
            entity.Property(e => e._03anos).HasColumnName("0-3ANOS");
            entity.Property(e => e._1317anos).HasColumnName("13-17ANOS");
            entity.Property(e => e._1829anos).HasColumnName("18-29ANOS");
            entity.Property(e => e._3059anos).HasColumnName("30-59ANOS");
            entity.Property(e => e._48anos).HasColumnName("4-8ANOS");
            entity.Property(e => e._60amasanos).HasColumnName("60AMASANOS");
            entity.Property(e => e._912anos).HasColumnName("9-12ANOS");

            entity.HasOne(d => d.IdProcesoNavigation).WithMany(p => p.LlenadoExternos)
                .HasForeignKey(d => d.IdProceso)
                .HasConstraintName("FK_LlenadoExterno_LlenadoInterno");
        });

        modelBuilder.Entity<LlenadoInterno>(entity =>
        {
            entity.HasKey(e => e.IdProceso).HasName("PK__LlenadoI__036D0743EEA42E57");

            entity.ToTable("LlenadoInterno");

            entity.Property(e => e.CargoRealizo).HasMaxLength(200);
            entity.Property(e => e.CargoValido).HasMaxLength(200);
            entity.Property(e => e.Idpp).HasColumnName("IDPP");
            entity.Property(e => e.NombreRealizo).HasMaxLength(200);
            entity.Property(e => e.NombreValido).HasMaxLength(200);
            entity.Property(e => e.Pp).HasMaxLength(30);
            entity.Property(e => e.UnidadMedida).HasMaxLength(100);

            entity.HasOne(d => d.IdppNavigation).WithMany(p => p.LlenadoInternos)
                .HasForeignKey(d => d.Idpp)
                .HasConstraintName("FK_LlenadoInterno_PPS");
        });

        modelBuilder.Entity<Municipio>(entity =>
        {
            entity.HasKey(e => e.IdMunicipio).HasName("PK__Municipi__610059786F627989");

            entity.ToTable("Municipio");

            entity.Property(e => e.IdMunicipio).ValueGeneratedNever();
            entity.Property(e => e.NombreMunicipios).HasMaxLength(50);
            entity.Property(e => e.NombreRegion).HasMaxLength(50);
            entity.Property(e => e.NumeroRegion).HasMaxLength(8);
        });

        modelBuilder.Entity<PersonasMunicipio>(entity =>
        {
            entity.HasKey(e => e.IdRegistroVinculacion).HasName("PK__Personas__565EA398558DC985");

            entity.ToTable("PersonasMunicipio");

            entity.HasOne(d => d.IdMunicipioNavigation).WithMany(p => p.PersonasMunicipios)
                .HasForeignKey(d => d.IdMunicipio)
                .HasConstraintName("FK_PersonasMunicipio_Municipio");
        });

        modelBuilder.Entity<Pp>(entity =>
        {
            entity.HasKey(e => e.IdPp).HasName("PK__PPS__B7703B43E5288482");

            entity.ToTable("PPS");

            entity.Property(e => e.Clave).HasMaxLength(150);
            entity.Property(e => e.NombrePp).HasMaxLength(150);
        });

        modelBuilder.Entity<PpCompuesto>(entity =>
        {
            entity.HasKey(e => e.IdPp).HasName("PK__PpCompue__B7703B43F255D1EA");

            entity.ToTable("PpCompuesto");

            entity.Property(e => e.Pp).HasMaxLength(350);
            entity.Property(e => e.PpCompuesto1).HasColumnName("PpCompuesto");
        });

        modelBuilder.Entity<Programa>(entity =>
        {
            entity.HasKey(e => e.IdProgramas).HasName("PK__Programa__6A731CB9C22CDC78");

            entity.Property(e => e.Departamento).HasMaxLength(1000);
        });

        modelBuilder.Entity<Programacion>(entity =>
        {
            entity.HasKey(e => e.IdRegistro).HasName("PK__Programa__FFA45A99928B1100");

            entity.ToTable("Programacion");

            entity.Property(e => e.Acumulable).HasMaxLength(4);
            entity.Property(e => e.AcumulableM).HasMaxLength(5);
            entity.Property(e => e.Beneficiarios).HasMaxLength(50);
            entity.Property(e => e.BeneficiariosM).HasMaxLength(150);
            entity.Property(e => e.Cuarto).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CuartoM).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CumplimientoPersonas).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CumplimientoPersonas1).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CumplimientoPersonas1M).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CumplimientoPersonasM).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CumplimientoServicios).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CumplimientoServicios1).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CumplimientoServicios1M).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CumplimientoServiciosM).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Frecuencia1).HasMaxLength(150);
            entity.Property(e => e.Frecuencia2).HasMaxLength(150);
            entity.Property(e => e.Frecuencia3).HasMaxLength(150);
            entity.Property(e => e.Frecuencia4).HasMaxLength(150);
            entity.Property(e => e.Frecuencia5).HasMaxLength(150);
            entity.Property(e => e.IncumplimientoAmarilloPersonas).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IncumplimientoAmarilloPersonas1).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IncumplimientoAmarilloPersonas1M).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IncumplimientoAmarilloPersonasM).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IncumplimientoAmarilloServicios).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IncumplimientoAmarilloServicios1).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IncumplimientoAmarilloServicios1M).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IncumplimientoAmarilloServiciosM).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IncumplimientoRojoPersonas).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IncumplimientoRojoPersonasM).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IncumplimientoRojosServ).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IncumplimientoRojosServM).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Mes11).HasColumnName("Mes1_1");
            entity.Property(e => e.Mes110).HasColumnName("Mes1_10");
            entity.Property(e => e.Mes110m).HasColumnName("Mes1_10M");
            entity.Property(e => e.Mes111).HasColumnName("Mes11");
            entity.Property(e => e.Mes1111).HasColumnName("Mes1_11");
            entity.Property(e => e.Mes111m).HasColumnName("Mes1_11M");
            entity.Property(e => e.Mes112).HasColumnName("Mes1_12");
            entity.Property(e => e.Mes112m).HasColumnName("Mes1_12M");
            entity.Property(e => e.Mes11m).HasColumnName("Mes1_1M");
            entity.Property(e => e.Mes12).HasColumnName("Mes1_2");
            entity.Property(e => e.Mes121).HasColumnName("Mes12");
            entity.Property(e => e.Mes12m).HasColumnName("Mes1_2M");
            entity.Property(e => e.Mes13).HasColumnName("Mes1_3");
            entity.Property(e => e.Mes13m).HasColumnName("Mes1_3M");
            entity.Property(e => e.Mes14).HasColumnName("Mes1_4");
            entity.Property(e => e.Mes14m).HasColumnName("Mes1_4M");
            entity.Property(e => e.Mes15).HasColumnName("Mes1_5");
            entity.Property(e => e.Mes15m).HasColumnName("Mes1_5M");
            entity.Property(e => e.Mes16).HasColumnName("Mes1_6");
            entity.Property(e => e.Mes16m).HasColumnName("Mes1_6M");
            entity.Property(e => e.Mes17).HasColumnName("Mes1_7");
            entity.Property(e => e.Mes17m).HasColumnName("Mes1_7M");
            entity.Property(e => e.Mes18).HasColumnName("Mes1_8");
            entity.Property(e => e.Mes18m).HasColumnName("Mes1_8M");
            entity.Property(e => e.Mes19).HasColumnName("Mes1_9");
            entity.Property(e => e.Mes19m).HasColumnName("Mes1_9M");
            entity.Property(e => e.NActividad).HasColumnName("nActividad");
            entity.Property(e => e.NComponente).HasColumnName("nComponente");
            entity.Property(e => e.NoPersonas2).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NoPersonas2M).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Pp).HasColumnName("pp");
            entity.Property(e => e.Primero).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PrimeroM).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RecursoEstatal).HasMaxLength(5);
            entity.Property(e => e.RecursoFederal).HasMaxLength(5);
            entity.Property(e => e.Segundo).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SegundoM).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SerieInfo2).HasMaxLength(5);
            entity.Property(e => e.SobreCumplimientoPersonas).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SobreCumplimientoPersonasM).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SobreCumplimientoServicios).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SobreCumplimientoServiciosM).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SolicitudTemporal).HasMaxLength(100);
            entity.Property(e => e.Tercero).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TerceroM).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalVariable2M).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Totalanos2).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Totalanos2M).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Valor).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Valor2).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Valor2M).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.IdLlenadoNavigation).WithMany(p => p.Programacions)
                .HasForeignKey(d => d.IdLlenado)
                .HasConstraintName("FK_Programacion_LlenadoInterno");
        });

        modelBuilder.Entity<ServiciosMunicipio>(entity =>
        {
            entity.HasKey(e => e.IdRegistroVinculacion).HasName("PK__Servicio__565EA398586D8315");

            entity.HasOne(d => d.IdLlenadoNavigation).WithMany(p => p.ServiciosMunicipios)
                .HasForeignKey(d => d.IdLlenado)
                .HasConstraintName("FK_ServiciosMunicipios_Programacion");

            entity.HasOne(d => d.IdMunicipioNavigation).WithMany(p => p.ServiciosMunicipios)
                .HasForeignKey(d => d.IdMunicipio)
                .HasConstraintName("FK_ServiciosMunicipios_Municipio");
        });

        modelBuilder.Entity<TemporalPersonasMunicipio>(entity =>
        {
            entity.HasKey(e => e.IdTemporal).HasName("PK__Temporal__73AC1D22E10F1998");

            entity.ToTable("TemporalPersonasMunicipio");

            entity.Property(e => e.Tiempo).HasMaxLength(100);

            entity.HasOne(d => d.IdMunicipioNavigation).WithMany(p => p.TemporalPersonasMunicipios)
                .HasForeignKey(d => d.IdMunicipio)
                .HasConstraintName("FK_TemporalPersonasMunicipio_Municipio");
        });

        modelBuilder.Entity<TemporalServiciosMunicipio>(entity =>
        {
            entity.HasKey(e => e.IdTemporal).HasName("PK__Temporal__73AC1D224F3290BD");

            entity.ToTable("TemporalServiciosMunicipio");

            entity.Property(e => e.Tiempo).HasMaxLength(100);

            entity.HasOne(d => d.IdMunicipioNavigation).WithMany(p => p.TemporalServiciosMunicipios)
                .HasForeignKey(d => d.IdMunicipio)
                .HasConstraintName("FK_TemporalServiciosMunicipio_Municipio");
        });

        modelBuilder.Entity<UnidadMedidum>(entity =>
        {
            entity.HasKey(e => e.IdUnidad).HasName("PK__UnidadMe__437725E6A7C3E899");

            entity.Property(e => e.Valor).HasMaxLength(200);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Usuario__5B65BF978CEC9D27");

            entity.Property(e => e.Area).HasMaxLength(500);
            entity.Property(e => e.Definicion).HasMaxLength(15);
            entity.Property(e => e.Direccion).HasMaxLength(200);
            entity.Property(e => e.Pass).HasMaxLength(30);
            entity.Property(e => e.Usuario1)
                .HasMaxLength(30)
                .HasColumnName("Usuario");
        });

        modelBuilder.Entity<Validacione>(entity =>
        {
            entity.HasKey(e => e.IdValidacion).HasName("PK__Validaci__5407AA746C7EE298");

            entity.HasOne(d => d.IdProcesoNavigation).WithMany(p => p.Validaciones)
                .HasForeignKey(d => d.IdProceso)
                .HasConstraintName("FK_Validaciones_LlenadoInterno");
        });

        modelBuilder.Entity<Vinculacion>(entity =>
        {
            entity.HasKey(e => e.IdVinculacion).HasName("PK__Vinculac__8F3EB47A78337A9F");

            entity.ToTable("Vinculacion");

            entity.HasOne(d => d.IdLlenadoNavigation).WithMany(p => p.Vinculacions)
                .HasForeignKey(d => d.IdLlenado)
                .HasConstraintName("FK_Vinculacion_LlenadoInterno");

            entity.HasOne(d => d.IdMunicipioNavigation).WithMany(p => p.Vinculacions)
                .HasForeignKey(d => d.IdMunicipio)
                .HasConstraintName("FK_Vinculacion_Municipio");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
