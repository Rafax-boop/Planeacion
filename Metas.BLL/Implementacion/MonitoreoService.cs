using Metas.BLL.DTO;
using Metas.BLL.Interfaces;
using Metas.DAL.DBContext;
using Metas.DAL.Interfaces;
using Metas.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metas.BLL.Implementacion
{
    public class MonitoreoService : IMonitoreoService
    {
        private readonly IGenericRepository<LlenadoInterno> _llenadoInternoRepository;
        private readonly IGenericRepository<LlenadoExterno> _llenadoExternoRepository;
        private readonly IGenericRepository<Programacion> _programacionRepository;
        private readonly MetasContext _context;

        public MonitoreoService(
        IGenericRepository<LlenadoInterno> llenadoInternoRepository,
        IGenericRepository<LlenadoExterno> llenadoExternoRepository,
        IGenericRepository<Programacion> programacionRepository,
        MetasContext context)
        {
            _llenadoInternoRepository = llenadoInternoRepository;
            _llenadoExternoRepository = llenadoExternoRepository;
            _programacionRepository = programacionRepository;
            _context = context;
        }

        public async Task<bool> GuardarActualizacion(GuardarActualizacionDTO modelo, string rutaEvidencia, string rutaJustificacion)
        {
            try
            {
                // 1. BUSCAR REGISTRO EXISTENTE (Clave: IdProceso + Mes)
                var registroExistente = await _llenadoExternoRepository.Obtener(
                    r => r.IdProceso == modelo.IdProceso && r.Meses == modelo.Mes
                );

                // Definir la entidad a usar (Existente o Nueva)
                LlenadoExterno registroExterno;
                if (registroExistente != null)
                {
                    // Es una ACTUALIZACIÓN
                    registroExterno = registroExistente;
                }
                else
                {
                    // Es una CREACIÓN
                    registroExterno = new LlenadoExterno();
                }

                // 2. MAPEO DE DATOS AL OBJETO (Común para Crear o Actualizar)
                registroExterno.IdProceso = modelo.IdProceso;
                registroExterno.Meses = modelo.Mes;

                // Datos de captura
                registroExterno.Realizado = modelo.Realizado;
                registroExterno.MujeresAtendidas = modelo.MujeresAtendidas;
                registroExterno.HombresAtendidos = modelo.HombresAtendidos;
                registroExterno.Indigena = modelo.Indigena;
                registroExterno.PersonasAtendidas = modelo.MujeresAtendidas + modelo.HombresAtendidos;

                // Rangos de edad
                registroExterno._03anos = modelo.Rango0a3;
                registroExterno._48anos = modelo.Rango4a8;
                registroExterno._912anos = modelo.Rango9a12;
                registroExterno._1317anos = modelo.Rango13a17;
                registroExterno._1829anos = modelo.Rango18a29;
                registroExterno._3059anos = modelo.Rango30a59;
                registroExterno._60amasanos = modelo.Rango60adelante;
                registroExterno.NoDefinida = modelo.RangoNoEspecifica;

                // 🎯 LÓGICA DE ARCHIVOS: Preservar la ruta si no se subió un nuevo archivo.
                // Si rutaEvidencia (la nueva ruta) está vacía, mantén la existente en el objeto registroExistente
                if (string.IsNullOrEmpty(rutaEvidencia) && registroExistente != null)
                {
                    registroExterno.Evidencia = registroExistente.Evidencia;
                }
                else
                {
                    registroExterno.Evidencia = rutaEvidencia; // Usa la nueva ruta
                }

                if (string.IsNullOrEmpty(rutaJustificacion) && registroExistente != null)
                {
                    registroExterno.Justificacion = registroExistente.Justificacion;
                }
                else
                {
                    registroExterno.Justificacion = rutaJustificacion; // Usa la nueva ruta
                }

                // 3. CREAR O ACTUALIZAR en LlenadoExterno
                if (registroExistente != null)
                {
                    await _llenadoExternoRepository.Editar(registroExterno);
                }
                else
                {
                    await _llenadoExternoRepository.Crear(registroExterno);
                }

                // --- 4. Lógica de Actualización de LlenadoInterno (Esta parte se mantiene) ---

                var metaInterna = await _llenadoInternoRepository.Obtener(
                    m => m.IdProceso == modelo.IdProceso
                );

                if (metaInterna == null)
                {
                    throw new Exception($"Error: La meta interna con IdProceso {modelo.IdProceso} no fue encontrada para actualizar.");
                }

                int mesNumero = modelo.Mes;
                int valorNuevoMensual = modelo.Realizado;
                int? totalRealizadoAnterior = metaInterna.TotalRealizado ?? 0;
                switch (mesNumero)
                {
                    case 1: // Enero
                        totalRealizadoAnterior -= metaInterna.TotalEneroRealizado ?? 0;
                        metaInterna.TotalEneroRealizado = valorNuevoMensual;
                        if (!modelo.EsBorrador) { metaInterna.Enero = true; }
                        break;
                    case 2: // Febrero

                        totalRealizadoAnterior -= metaInterna.TotalFebreroRealizado ?? 0;
                        metaInterna.TotalFebreroRealizado = valorNuevoMensual;
                        if (!modelo.EsBorrador) { metaInterna.Febrero = true; }
                        break;
                    case 3: // Marzo
                        totalRealizadoAnterior -= metaInterna.TotalMarzoRealizado ?? 0;
                        metaInterna.TotalMarzoRealizado = valorNuevoMensual;
                        if (!modelo.EsBorrador) { metaInterna.Marzo = true; }
                        break;
                    case 4: // Abril
                        totalRealizadoAnterior -= metaInterna.TotalAbrilRealizado ?? 0;
                        metaInterna.TotalAbrilRealizado = valorNuevoMensual;
                        if (!modelo.EsBorrador) { metaInterna.Abril = true; }
                        break;
                    case 5: // Mayo
                        totalRealizadoAnterior -= metaInterna.TotalMayoRealizado ?? 0;
                        metaInterna.TotalMayoRealizado = valorNuevoMensual;
                        if (!modelo.EsBorrador) { metaInterna.Mayo = true; }
                        break;
                    case 6: // Junio
                        totalRealizadoAnterior -= metaInterna.TotalJunioRealizado ?? 0;
                        metaInterna.TotalJunioRealizado = valorNuevoMensual;
                        if (!modelo.EsBorrador) { metaInterna.Junio = true; }
                        break;
                    case 7: // Julio
                        totalRealizadoAnterior -= metaInterna.TotalJulioRealizado ?? 0;
                        metaInterna.TotalJulioRealizado = valorNuevoMensual;
                        if (!modelo.EsBorrador) { metaInterna.Julio = true; }
                        break;
                    case 8: // Agosto
                        totalRealizadoAnterior -= metaInterna.TotalAgostoRealizado ?? 0;
                        metaInterna.TotalAgostoRealizado = valorNuevoMensual;
                        if (!modelo.EsBorrador) { metaInterna.Agosto = true; }
                        break;
                    case 9: // Septiembre
                        totalRealizadoAnterior -= metaInterna.TotalSeptiembreRealizado ?? 0;
                        metaInterna.TotalSeptiembreRealizado = valorNuevoMensual;
                        if (!modelo.EsBorrador) { metaInterna.Septiembre = true; }
                        break;
                    case 10: // Octubre
                        totalRealizadoAnterior -= metaInterna.TotalOctubreRealizado ?? 0;
                        metaInterna.TotalOctubreRealizado = valorNuevoMensual;
                        if (!modelo.EsBorrador) { metaInterna.Octubre = true; }
                        break;
                    case 11: // Noviembre
                        totalRealizadoAnterior -= metaInterna.TotalNoviembreRealizado ?? 0;
                        metaInterna.TotalNoviembreRealizado = valorNuevoMensual;
                        if (!modelo.EsBorrador) { metaInterna.Noviembre = true; }
                        break;
                    case 12: // Diciembre
                        totalRealizadoAnterior -= metaInterna.TotalDiciembreRealizado ?? 0;
                        metaInterna.TotalDiciembreRealizado = valorNuevoMensual;
                        if (!modelo.EsBorrador) { metaInterna.Diciembre = true; }
                        break;
                    default:
                        throw new ArgumentException($"Número de mes '{mesNumero}' no válido (Debe ser entre 1 y 12).");
                }

                // --- B. Cálculo del Total Realizado Acumulado ---
                metaInterna.TotalRealizado = totalRealizadoAnterior + valorNuevoMensual;

                // --- C. Actualización de Firmas y Estado General ---
                metaInterna.NombreRealizo = modelo.NombreRealizo;
                metaInterna.CargoRealizo = modelo.PuestoRealizo;
                metaInterna.NombreValido = modelo.NombreAutorizo;
                metaInterna.CargoValido = modelo.PuestoAutorizo;

                // C. Guardar los cambios en la tabla LlenadoInterno
                await _llenadoInternoRepository.Editar(metaInterna);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GuardarActualizacion: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> HabilitarCaptura(int idProceso, int mes)
        {
            var registro = await _llenadoInternoRepository.Obtener(r => r.IdProceso == idProceso);

            if (registro == null)
            {
                return false;
            }

            // 2. Determinar la columna a actualizar
            string columnaMes = ObtenerColumnaFecha(mes);

            // 3. Usar Reflection para actualizar la propiedad dinámicamente
            var propiedad = registro.GetType().GetProperty(columnaMes);

            // 🚨 Valor DateOnly a asignar 🚨
            var fechaActual = DateOnly.FromDateTime(DateTime.Now);

            // Verificamos si es DateOnly? (Nullable)
            if (propiedad != null && propiedad.PropertyType == typeof(DateOnly?))
            {
                propiedad.SetValue(registro, fechaActual);
            }
            // Opcional: Si el campo fuera DateOnly (no nullable)
            else if (propiedad != null && propiedad.PropertyType == typeof(DateOnly))
            {
                propiedad.SetValue(registro, fechaActual);
            }
            else
            {
                // La columna no se encontró o el tipo de dato no coincide
                return false;
            }

            // 4. ACTUALIZAR el registro en la base de datos usando el repositorio genérico
            return await _llenadoInternoRepository.Editar(registro);
        }

        public async Task<LlenadoExterno> ObtenerLlenadoMensual(int id, int mes)
        {
            var registro = await _llenadoExternoRepository.Obtener(
                r => r.IdProceso == id && r.Meses == mes
            );

            return registro;
        }

        public async Task<bool> ActualizarRegistro(DatosEdicionDTO modelo)
        {
            try
            {
                // 1. Buscar la entidad existente
                var registroDb = await _llenadoInternoRepository.Obtener(r => r.IdProceso == modelo.IdProceso);

                if (registroDb == null)
                {
                    Console.WriteLine($"No se encontró el registro con IdProceso: {modelo.IdProceso}");
                    return false;
                }

                // 2. Mapear los datos
                registroDb.Idpp = modelo.PP;
                registroDb.Pp = ObtenerNombreProgramaPresupuestario(modelo.PP);
                registroDb.Componente = modelo.Componente;
                registroDb.Actividad = modelo.Actividad;
                registroDb.DescripcionActividad = modelo.DescripcionActividad;
                registroDb.ProgramaSocial = modelo.ProgramaSocial;
                registroDb.UnidadMedida = modelo.UnidadMedida;

                registroDb.TotalEnero = modelo.TotalEnero;
                registroDb.TotalFebrero = modelo.TotalFebrero;
                registroDb.TotalMarzo = modelo.TotalMarzo;
                registroDb.TotalAbril = modelo.TotalAbril;
                registroDb.TotalMayo = modelo.TotalMayo;
                registroDb.TotalJunio = modelo.TotalJunio;
                registroDb.TotalJulio = modelo.TotalJulio;
                registroDb.TotalAgosto = modelo.TotalAgosto;
                registroDb.TotalSeptiembre = modelo.TotalSeptiembre;
                registroDb.TotalOctubre = modelo.TotalOctubre;
                registroDb.TotalNoviembre = modelo.TotalNoviembre;
                registroDb.TotalDiciembre = modelo.TotalDiciembre;

                registroDb.EneroPersona = modelo.EneroPersona;
                registroDb.FebreroPersona = modelo.FebreroPersona;
                registroDb.MarzoPersona = modelo.MarzoPersona;
                registroDb.AbrilPersona = modelo.AbrilPersona;
                registroDb.MayoPersona = modelo.MayoPersona;
                registroDb.JunioPersona = modelo.JunioPersona;
                registroDb.JulioPersona = modelo.JulioPersona;
                registroDb.AgostoPersona = modelo.AgostoPersona;
                registroDb.SeptiembrePersona = modelo.SeptiembrePersona;
                registroDb.OctubrePersona = modelo.OctubrePersona;
                registroDb.NoviembrePersona = modelo.NoviembrePersona;
                registroDb.DiciembrePersona = modelo.DiciembrePersona;

                // 3. Editar (esto hace Update y SaveChangesAsync)
                bool resultado = await _llenadoInternoRepository.Editar(registroDb);

                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar el registro: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<bool> EliminarCapturaMes(int idProceso, int mes)
        {
            try
            {
                // Validar que el mes esté en el rango correcto
                if (mes < 1 || mes > 12)
                {
                    throw new ArgumentException("El mes debe estar entre 1 y 12");
                }

                // 1. Eliminar todos los registros de LlenadoExterno para este proceso y mes
                var registrosExternos = (await _llenadoExternoRepository.Consultar(
                    le => le.IdProceso == idProceso && le.Meses == mes)).ToList();

                foreach (var registro in registrosExternos)
                {
                    await _llenadoExternoRepository.Eliminar(registro);
                }

                // 2. Actualizar el campo del mes en LlenadoInterno (ponerlo en null)
                var llenadoInterno = await _llenadoInternoRepository.Obtener(
                    li => li.IdProceso == idProceso);

                if (llenadoInterno == null)
                {
                    throw new Exception($"No se encontró el registro de LlenadoInterno con IdProceso: {idProceso}");
                }

                // 3. Establecer el campo del mes en null usando switch
                switch (mes)
                {
                    case 1:
                        llenadoInterno.Enero = null;
                        llenadoInterno.TotalEneroRealizado = null;
                        break;
                    case 2:
                        llenadoInterno.Febrero = null;
                        llenadoInterno.TotalFebreroRealizado = null;
                        break;
                    case 3:
                        llenadoInterno.Marzo = null;
                        llenadoInterno.TotalMarzoRealizado = null;
                        break;
                    case 4:
                        llenadoInterno.Abril = null;
                        llenadoInterno.TotalAbrilRealizado = null;
                        break;
                    case 5:
                        llenadoInterno.Mayo = null;
                        llenadoInterno.TotalMayoRealizado = null;
                        break;
                    case 6:
                        llenadoInterno.Junio = null;
                        llenadoInterno.TotalJunioRealizado = null;
                        break;
                    case 7:
                        llenadoInterno.Julio = null;
                        llenadoInterno.TotalJulioRealizado = null;
                        break;
                    case 8:
                        llenadoInterno.Agosto = null;
                        llenadoInterno.TotalAgostoRealizado = null;
                        break;
                    case 9:
                        llenadoInterno.Septiembre = null;
                        llenadoInterno.TotalSeptiembreRealizado = null;
                        break;
                    case 10:
                        llenadoInterno.Octubre = null;
                        llenadoInterno.TotalOctubreRealizado = null;
                        break;
                    case 11:
                        llenadoInterno.Noviembre = null;
                        llenadoInterno.TotalNoviembreRealizado = null;
                        break;
                    case 12:
                        llenadoInterno.Diciembre = null;
                        llenadoInterno.TotalDiciembreRealizado = null;
                        break;
                }

                // 4. Recalcular el total realizado (restar el valor del mes eliminado)
                llenadoInterno.TotalRealizado =
                    (llenadoInterno.TotalEneroRealizado ?? 0) +
                    (llenadoInterno.TotalFebreroRealizado ?? 0) +
                    (llenadoInterno.TotalMarzoRealizado ?? 0) +
                    (llenadoInterno.TotalAbrilRealizado ?? 0) +
                    (llenadoInterno.TotalMayoRealizado ?? 0) +
                    (llenadoInterno.TotalJunioRealizado ?? 0) +
                    (llenadoInterno.TotalJulioRealizado ?? 0) +
                    (llenadoInterno.TotalAgostoRealizado ?? 0) +
                    (llenadoInterno.TotalSeptiembreRealizado ?? 0) +
                    (llenadoInterno.TotalOctubreRealizado ?? 0) +
                    (llenadoInterno.TotalNoviembreRealizado ?? 0) +
                    (llenadoInterno.TotalDiciembreRealizado ?? 0);

                // 5. Guardar los cambios
                await _llenadoInternoRepository.Editar(llenadoInterno);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en EliminarCapturaMes: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> CrearNuevoProceso(DatosEdicionDTO modelo)
        {
            try
            {
                // Crear la entidad LlenadoInterno
                var nuevoLlenado = new LlenadoInterno
                {
                    Departamento = modelo.Departamento,
                    Ano = modelo.AnoFiscal,
                    Idpp = modelo.PP,
                    Pp = ObtenerNombreProgramaPresupuestario(modelo.PP),
                    Componente = modelo.Componente,
                    Actividad = modelo.Actividad,
                    DescripcionActividad = modelo.DescripcionActividad,
                    UnidadMedida = modelo.UnidadMedida,
                    ProgramaSocial = modelo.ProgramaSocial,

                    // Metas programadas
                    TotalEnero = modelo.TotalEnero,
                    TotalFebrero = modelo.TotalFebrero,
                    TotalMarzo = modelo.TotalMarzo,
                    TotalAbril = modelo.TotalAbril,
                    TotalMayo = modelo.TotalMayo,
                    TotalJunio = modelo.TotalJunio,
                    TotalJulio = modelo.TotalJulio,
                    TotalAgosto = modelo.TotalAgosto,
                    TotalSeptiembre = modelo.TotalSeptiembre,
                    TotalOctubre = modelo.TotalOctubre,
                    TotalNoviembre = modelo.TotalNoviembre,
                    TotalDiciembre = modelo.TotalDiciembre,

                    // Personas programadas
                    EneroPersona = modelo.EneroPersona,
                    FebreroPersona = modelo.FebreroPersona,
                    MarzoPersona = modelo.MarzoPersona,
                    AbrilPersona = modelo.AbrilPersona,
                    MayoPersona = modelo.MayoPersona,
                    JunioPersona = modelo.JunioPersona,
                    JulioPersona = modelo.JulioPersona,
                    AgostoPersona = modelo.AgostoPersona,
                    SeptiembrePersona = modelo.SeptiembrePersona,
                    OctubrePersona = modelo.OctubrePersona,
                    NoviembrePersona = modelo.NoviembrePersona,
                    DiciembrePersona = modelo.DiciembrePersona,

                    // Calcular totales
                    TotalProgramado = (modelo.TotalEnero ?? 0) + (modelo.TotalFebrero ?? 0) +
                                    (modelo.TotalMarzo ?? 0) + (modelo.TotalAbril ?? 0) +
                                    (modelo.TotalMayo ?? 0) + (modelo.TotalJunio ?? 0) +
                                    (modelo.TotalJulio ?? 0) + (modelo.TotalAgosto ?? 0) +
                                    (modelo.TotalSeptiembre ?? 0) + (modelo.TotalOctubre ?? 0) +
                                    (modelo.TotalNoviembre ?? 0) + (modelo.TotalDiciembre ?? 0),

                    TotalPersona = (modelo.EneroPersona ?? 0) + (modelo.FebreroPersona ?? 0) +
                                           (modelo.MarzoPersona ?? 0) + (modelo.AbrilPersona ?? 0) +
                                           (modelo.MayoPersona ?? 0) + (modelo.JunioPersona ?? 0) +
                                           (modelo.JulioPersona ?? 0) + (modelo.AgostoPersona ?? 0) +
                                           (modelo.SeptiembrePersona ?? 0) + (modelo.OctubrePersona ?? 0) +
                                           (modelo.NoviembrePersona ?? 0) + (modelo.DiciembrePersona ?? 0)
                };

                // Guardar usando el repositorio genérico
                var resultado = await _llenadoInternoRepository.Crear(nuevoLlenado);

                // Si tu sistema requiere crear un registro en la tabla Programacion relacionado
                // (basándome en que veo que usas IdEstatus y navegación a Programacion)
                if (resultado != null)
                {
                    var programacion = new Programacion
                    {
                        IdLlenado = resultado.IdProceso,
                        IdEstatus = 3,

                    };

                    await _programacionRepository.Crear(programacion);
                }

                return resultado != null;
            }
            catch (Exception ex)
            {
                // Log del error si tienes un logger
                throw new Exception($"Error al crear el nuevo proceso: {ex.Message}", ex);
            }
        }

        private string ObtenerColumnaFecha(int mes)
        {
            return mes switch
            {
                1 => "FechaEnero",
                2 => "FechaFebrero",
                3 => "FechaMarzo",
                4 => "FechaAbril",
                5 => "FechaMayo",
                6 => "FechaJunio",
                7 => "FechaJulio",
                8 => "FechaAgosto",
                9 => "FechaSeptiembre",
                10 => "FechaOctubre",
                11 => "FechaNoviembre",
                12 => "FechaDiciembre",
                _ => throw new ArgumentException("Mes no válido")
            };
        }

        private string ObtenerNombreProgramaPresupuestario(int? id)
        {

            switch (id)
            {
                case 3:
                    return "Agenda";
                case 1:
                    return "E046";
                case 2:
                    return "E047";
                default:
                    return string.Empty;
            }
        }
    }
}
