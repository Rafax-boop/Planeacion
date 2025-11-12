using Metas.BLL.DTO;
using Metas.BLL.Interfaces;
using Metas.DAL.Interfaces;
using Metas.Entity;
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

                // 3. 🎯 CREAR O ACTUALIZAR en LlenadoExterno
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

                // ... (Tu switch statement completo para actualizar TotalXxxRealizado y Enero=true/false) ...
                switch (mesNumero)
                {
                    case 1: // Enero
                        totalRealizadoAnterior -= metaInterna.TotalEneroRealizado ?? 0;
                        metaInterna.TotalEneroRealizado = valorNuevoMensual;
                        if (!modelo.EsBorrador) { metaInterna.Enero = true; }
                        break;
                    // ... (restantes meses) ...
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

        public async Task<LlenadoExterno> ObtenerLlenadoMensual(int id, int mes)
        {
            var registro = await _llenadoExternoRepository.Obtener(
                r => r.IdProceso == id && r.Meses == mes
            );

            return registro;
        }
    }
}
