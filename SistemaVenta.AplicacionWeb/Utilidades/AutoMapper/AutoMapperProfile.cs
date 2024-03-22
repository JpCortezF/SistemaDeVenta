using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.Entity;
using System.Globalization;
using AutoMapper;

namespace SistemaVenta.AplicacionWeb.Utilidades.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            #region Rol
            CreateMap<Rol, VmRol>().ReverseMap(); // Mapeo de Rol a ViewModel Rol y viceversa
            #endregion 

            #region Usuario
            CreateMap<Usuario, VmUsuario>()
                .ForMember(destino =>
                    destino.EsActivo,
                    options => options.MapFrom(origen => origen.EsActivo == true ? 1 : 0) // Usuario es el origen
                 )
                .ForMember(destino =>
                    destino.NombreRol,
                    options => options.MapFrom(origen => origen.IdRolNavigation.Descripcion) // Guardamos en el destino.NombreRol = origen...
                );

            CreateMap<VmUsuario, Usuario>()
                .ForMember(destino =>
                    destino.EsActivo,
                    options => options.MapFrom(origen => origen.EsActivo == 1 ? true : false)
                )
                .ForMember(destino =>
                    destino.IdRolNavigation,
                    opt => opt.Ignore()
                );
            #endregion
        }
    }
}
