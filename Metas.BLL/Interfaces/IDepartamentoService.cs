using Metas.BLL.DTO;
using Metas.Entity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metas.BLL.Interfaces
{
    public interface IDepartamentoService
    {
        Task<List<Departamento>> ObtenerDepartamentos();
        Task<List<SelectListItem>> ObtenerListaPorTipo(string tipo);
        Task<List<PpCompuesto>> ObtenerComponentes();
        Task<List<UnidadMedidum>> ObtenerMedidas();
        Task<List<Municipio>> ObtenerMunicipios();
        Task<Departamento> ObtenerDepartamento(int idDepartamento);
        Task<bool> GuardarDocumentos(DepartamentoDocumentosDTO modelo);
        Task<bool> GuardarJustificacion(int idDepartamento, string justificacion);
        Task<string> EliminarDocumento(int idDepartamento, string tipoDocumento);
    }
}
