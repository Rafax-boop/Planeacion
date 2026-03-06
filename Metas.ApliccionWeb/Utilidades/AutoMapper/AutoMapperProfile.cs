using AutoMapper;
using Metas.AplicacionWeb.Models.ViewModels;
using Metas.BLL.DTO;
using Metas.Entity;

namespace Metas.AplicacionWeb.Utilidades.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<VMUsuario, Usuario>()
                .ReverseMap();

            CreateMap<VMFechas, CapturaProgramacion>()
                .ReverseMap();

            CreateMap<VMFechas, FechaCaptura>()
                .ReverseMap();

            CreateMap<VMDatosEdicionActividad, DatosEdicionDTO>()
                .ReverseMap();
        }
    }
}
