using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SistemaVenta.DAL.DBContext;
using Microsoft.EntityFrameworkCore;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.DAL.Implementacion;
using SistemaVenta.BBL.Interfaces;
using SistemaVenta.BBL.Implementacion;

namespace SistemaVenta.IOC
{
    public static class Dependencia
    {
        public static void InyectarDependencia(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DBVENTAContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("CadenaSQL"));
            });

            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddScoped<IVentaRepository, VentaRepository>();

            services.AddScoped<ICorreoServices, CorreoServices>();
            services.AddScoped<IFireBaseServices, FireBaseServices>();

            services.AddScoped<IUtilidadesServices, UtilidadesServices>();
            services.AddScoped<IRolServices, RolServices>();

            services.AddScoped<IUsuarioServices, UsuarioServices>();
            services.AddScoped<INegocioServices, NegocioServices>();

            services.AddScoped<ICategoriaServices, CategoriaServices>();
            services.AddScoped<IProductoServices, ProductoController>();

            services.AddScoped<ITipoDocumentoVentaServices, TipoDocumentoVentaServices>();
            services.AddScoped<IVentaServices, VentaServices>();

            services.AddScoped<IDashBoardServices, DashBoardServices>();
            services.AddScoped<IMenuServices, MenuServices>();
        }
    }
}
