using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.BBL.Interfaces;


namespace SistemaVenta.AplicacionWeb.Utilidades.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly IMenuServices _menuServices;
        private readonly IMapper _mapper;

        public MenuViewComponent(IMenuServices menuServices, IMapper mapper)
        {
            _menuServices = menuServices;
            _mapper = mapper;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            ClaimsPrincipal claimUsuario = HttpContext.User;
            List<VmMenu> listaMenus;

            if(claimUsuario.Identity.IsAuthenticated)
            {
                string idUsuario = claimUsuario.Claims
                .Where(c => c.Type == ClaimTypes.NameIdentifier) // almacena el id del usuario
                .Select(c => c.Value).SingleOrDefault();

                listaMenus = _mapper.Map<List<VmMenu>>(await _menuServices.ObtenerMenu(int.Parse(idUsuario)));
            }
            else
            {
                listaMenus = new List<VmMenu>();
            }

            return View(listaMenus);
        }
    }
}
