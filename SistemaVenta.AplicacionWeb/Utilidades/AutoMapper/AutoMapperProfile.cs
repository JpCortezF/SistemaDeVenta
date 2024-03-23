using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.Entity;
using System.Globalization;
using AutoMapper;
using Humanizer;

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

            #region Negocio

            CreateMap<Negocio, VmNegocio>()
            .ForMember(destino =>
                destino.PorcentajeImpuesto,
                opt => opt.MapFrom(origen => Convert.ToString(origen.PorcentajeImpuesto.Value, new CultureInfo("es-AR")))
                );

            CreateMap<VmNegocio, Negocio>()
            .ForMember(destino =>
                destino.PorcentajeImpuesto,
                opt => opt.MapFrom(origen => Convert.ToDecimal(origen.PorcentajeImpuesto.Value, new CultureInfo("es-AR")))
                );
            #endregion

            #region Categoria

            CreateMap<Categoria, VmCategoria>()
            .ForMember(destino =>
                destino.EsActivo,
                opt => opt.MapFrom(origen => origen.EsActivo == true ? 1 : 0) // ViewModel.'EsActivo' es un int.
                );

            CreateMap<VmCategoria, Categoria>()
            .ForMember(destino =>
                destino.EsActivo,
                opt => opt.MapFrom(origen => origen.EsActivo == 1 ? true : false)  // Categoria.'EsActivo' es un bool.
                );

            #endregion

            #region Producto

            CreateMap<Producto, VmProducto>()
            .ForMember(destino =>
                destino.EsActivo,
                opt => opt.MapFrom(origen => origen.EsActivo == true ? 1 : 0)
                )
            .ForMember(destino =>
                destino.NombreCategoria,
                opt => opt.MapFrom(origen => origen.IdCategoriaNavigation.Descripcion)
                )
            .ForMember(destino =>
                destino.Precio,
                opt => opt.MapFrom(origen => Convert.ToString(origen.Precio.Value, new CultureInfo("es-AR")))
                );

            CreateMap<VmProducto, Producto>()
            .ForMember(destino =>
                destino.EsActivo,
                opt => opt.MapFrom(origen => origen.EsActivo == 1 ? true : false)
                )
            .ForMember(destino =>
                destino.IdCategoriaNavigation,
                opt => opt.Ignore()
                )
            .ForMember(destino =>
                destino.Precio,
                opt => opt.MapFrom(origen => Convert.ToDecimal(origen.Precio, new CultureInfo("es-AR")))
                );

            #endregion

            #region TipoDocumentoVenta

            CreateMap<TipoDocumentoVenta, VmTipoDocumentoVenta>().ReverseMap();

            #endregion

            #region Venta

            CreateMap<Venta, VmVenta>()
            .ForMember(destino =>
                destino.TipoDocumentoVenta,
                opt => opt.MapFrom(origen => origen.IdTipoDocumentoVentaNavigation.Descripcion)
                )
            .ForMember(destino =>
                destino.Usuario,
                opt => opt.MapFrom(origen => origen.IdUsuarioNavigation.Nombre)
                )
            .ForMember(destino =>
                destino.SubTotal,
                opt => opt.MapFrom(origen => Convert.ToString(origen.SubTotal.Value, new CultureInfo("es-AR")))
                )
            .ForMember(destino =>
                destino.ImpuestoTotal,
                opt => opt.MapFrom(origen => Convert.ToString(origen.ImpuestoTotal.Value, new CultureInfo("es-AR")))
                )
            .ForMember(destino =>
                destino.Total,
                opt => opt.MapFrom(origen => Convert.ToString(origen.Total.Value, new CultureInfo("es-AR")))
                )
            .ForMember(destino =>
                destino.FechaRegistro,
                opt => opt.MapFrom(origen => origen.FechaRegistro.Value.ToString("dd/MM/yyyy"))
                );


            CreateMap<VmVenta, Venta>()
            .ForMember(destino =>
                destino.SubTotal,
                opt => opt.MapFrom(origen => Convert.ToDecimal(origen.SubTotal, new CultureInfo("es-AR")))
                )
            .ForMember(destino =>
                destino.ImpuestoTotal,
                opt => opt.MapFrom(origen => Convert.ToDecimal(origen.ImpuestoTotal, new CultureInfo("es-AR")))
                )
            .ForMember(destino =>
                destino.Total,
                opt => opt.MapFrom(origen => Convert.ToDecimal(origen.Total, new CultureInfo("es-AR")))
                );

            #endregion

            #region DetalleVenta

            CreateMap<DetalleVenta, VmDetalleVenta>()
            .ForMember(destino =>
            destino.Precio,
            opt => opt.MapFrom(origen => Convert.ToString(origen.Precio.Value, new CultureInfo("es-AR")))
            )
            .ForMember(destino =>
            destino.Total,
            opt => opt.MapFrom(origen => Convert.ToString(origen.Total.Value, new CultureInfo("es-AR")))
            );

            CreateMap<VmDetalleVenta, DetalleVenta>()
            .ForMember(destino =>
            destino.Precio,
            opt => opt.MapFrom(origen => Convert.ToDecimal(origen.Precio, new CultureInfo("es-AR")))
            )
            .ForMember(destino =>
            destino.Total,
            opt => opt.MapFrom(origen => Convert.ToDecimal(origen.Total, new CultureInfo("es-AR")))
            );

            CreateMap<DetalleVenta, VmReporteVenta>()
            .ForMember(destino =>
            destino.FechaRegistro,
            opt => opt.MapFrom(origen => origen.IdVentaNavigation.FechaRegistro.Value.ToString("dd/MM/yyyy"))
            )
            .ForMember(destino =>
            destino.NumeroVenta,
            opt => opt.MapFrom(origen => origen.IdVentaNavigation.NumeroVenta)
            )
            .ForMember(destino =>
            destino.TipoDocumento,
            opt => opt.MapFrom(origen => origen.IdVentaNavigation.IdTipoDocumentoVentaNavigation.Descripcion)
            )
            .ForMember(destino =>
            destino.DocumentoCliente,
            opt => opt.MapFrom(origen => origen.IdVentaNavigation.DocumentoCliente)
            )
            .ForMember(destino =>
            destino.NombreCliente,
            opt => opt.MapFrom(origen => origen.IdVentaNavigation.NombreCliente)
            )
            .ForMember(destino =>
            destino.SubTotalVenta,
            opt => opt.MapFrom(origen => Convert.ToString(origen.IdVentaNavigation.SubTotal.Value, new CultureInfo("es-AR")))
            )
            .ForMember(destino =>
            destino.ImpuestoTotalVenta,
            opt => opt.MapFrom(origen => Convert.ToString(origen.IdVentaNavigation.ImpuestoTotal.Value, new CultureInfo("es-AR")))
            )
            .ForMember(destino =>
            destino.TotalVenta,
            opt => opt.MapFrom(origen => Convert.ToString(origen.IdVentaNavigation.Total.Value, new CultureInfo("es-AR")))
            )
            .ForMember(destino =>
            destino.Producto,
            opt => opt.MapFrom(origen => origen.DescripcionProducto)
            )
            .ForMember(destino =>
            destino.Precio,
            opt => opt.MapFrom(origen => Convert.ToString(origen.Precio.Value, new CultureInfo("es-AR")))
            )
            .ForMember(destino =>
            destino.Total,
            opt => opt.MapFrom(origen => Convert.ToString(origen.Total.Value, new CultureInfo("es-AR")))
            );

            #endregion

            #region Menu

            CreateMap<Menu, VmMenu>()
            .ForMember(destino =>
            destino.SubMenus,
            opt => opt.MapFrom(origen => origen.InverseIdMenuPadreNavigation)
            );
            #endregion
        }
    }
}
