
namespace Metas.AplicacionWeb.Models.ViewModels
{
    public class VMGuardarActualizacion : VMDatosInternos
    {
        public int Realizado { get; set; }
        public int MujeresAtendidas { get; set; } // name="MujeresAtendidas"
        public int HombresAtendidos { get; set; } // name="HombresAtendidos"

        // =====================================
        // SECCIÓN RANGOS DE EDAD
        // =====================================
        public int Rango0a3 { get; set; }          // name="Rango0a3"
        public int Rango4a8 { get; set; }          // name="Rango4a8"
        public int Rango9a12 { get; set; }         // name="Rango9a12"
        public int Rango13a17 { get; set; }        // name="Rango13a17"
        public int Rango18a29 { get; set; }        // name="Rango18a29"
        public int Rango30a59 { get; set; }        // name="Rango30a59"
        public int Rango60adelante { get; set; }   // name="Rango60adelante"
        public int RangoNoEspecifica { get; set; } // name="RangoNoEspecifica"
        public int Indigena { get; set; } // name="Indigena"

        // =====================================
        // SECCIÓN EVIDENCIAS (Archivos)
        // Se usa IFormFile para recibir los archivos del input type="file"
        // =====================================
        public IFormFile InputEvidencia { get; set; }  // name="InputEvidencia"
        public IFormFile InputJustificacion { get; set; } // name="InputJustificacion"
        public string RutaEvidencia { get; set; }
        public string RutaJustificacion { get; set; }

        // =====================================
        // SECCIÓN REALIZÓ Y AUTORIZÓ (Firmas)
        // =====================================
        public string NombreRealizo { get; set; } // name="NombreRealizo"
        public string PuestoRealizo { get; set; } // name="PuestoRealizo"
        public string NombreAutorizo { get; set; } // name="NombreAutorizo"
        public string PuestoAutorizo { get; set; } // name="PuestoAutorizo"

        // =====================================
        // PROPIEDAD AUXILIAR PARA EL BOTÓN BORRADOR (no está en el form, se añade vía JS/AJAX)
        // =====================================
        public bool EsBorrador { get; set; } = false;
    }
}
