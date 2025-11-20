using Metas.BLL.DTO;
using Metas.BLL.Interfaces;
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

        public MonitoreoService(
        IGenericRepository<LlenadoInterno> llenadoInternoRepository,
        IGenericRepository<LlenadoExterno> llenadoExternoRepository)
        {
            _llenadoInternoRepository = llenadoInternoRepository;
            _llenadoExternoRepository = llenadoExternoRepository;
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
    }
}
