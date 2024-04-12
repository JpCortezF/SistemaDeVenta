using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.BBL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;


namespace SistemaVenta.BBL.Implementacion
{
    public class MenuServices : IMenuServices
    {
        private readonly IGenericRepository<Menu> _menuRepository;
        private readonly IGenericRepository<RolMenu> _rolMenuRepository;
        private readonly IGenericRepository<Usuario> _usuarioRepository;

        public MenuServices(IGenericRepository<Menu> menuRepository, IGenericRepository<RolMenu> rolMenuRepository, IGenericRepository<Usuario> usuarioRepository)
        {
            _menuRepository = menuRepository;
            _rolMenuRepository = rolMenuRepository;
            _usuarioRepository = usuarioRepository;
        }
        public async Task<List<Menu>> ObtenerMenus(int idUsuario)
        {
            IQueryable<Usuario> tablaUsuario = await _usuarioRepository.Consultar(u => u.IdUsuario == idUsuario);
            IQueryable<RolMenu> tablaRolMenu = await _rolMenuRepository.Consultar();
            IQueryable<Menu> tablaMenu = await _menuRepository.Consultar();

            IQueryable<Menu> menuPadre = (from u in tablaUsuario
                                          join rm in tablaRolMenu on u.IdRol equals rm.IdRol
                                          join m in tablaMenu on rm.IdMenu equals m.IdMenu
                                          join mpadre in tablaMenu on m.IdMenuPadre equals mpadre.IdMenu
                                          select mpadre).Distinct().AsQueryable();  // Todos los menús padres que pertenecen según el idRol

            IQueryable<Menu> menuHijo = (from u in tablaUsuario
                                         join rm in tablaRolMenu on u.IdRol equals rm.IdRol
                                         join m in tablaMenu on rm.IdMenu equals m.IdMenu
                                         where m.IdMenu != m.IdMenuPadre
                                         select m).Distinct().AsQueryable();

            List<Menu> listaMenu = (from mpadre in menuPadre select new Menu()
            {
                Descripcion = mpadre.Descripcion,
                Icono = mpadre.Icono,
                Controlador = mpadre.Controlador,
                PaginaAccion = mpadre.PaginaAccion,
                InverseIdMenuPadreNavigation = (from mhijo in menuHijo 
                                                where mhijo.IdMenuPadre == mpadre.IdMenu
                                                select mhijo).ToList()
            }).ToList();
                                    

            return listaMenu;
                                   
        }
    }
}
