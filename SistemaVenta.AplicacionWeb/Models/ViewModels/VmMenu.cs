namespace SistemaVenta.AplicacionWeb.Models.ViewModels
{
    public class VmMenu
    {
        public string? Descripcion { get; set; }
        public string? Icono { get; set; }
        public string? Controlador { get; set; }
        public string? PaginaAccion { get; set; }
        public virtual ICollection<VmMenu> SubMenus { get; set; }
    }
}
