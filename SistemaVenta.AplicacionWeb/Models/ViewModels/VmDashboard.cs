namespace SistemaVenta.AplicacionWeb.Models.ViewModels
{
    public class VmDashboard
    {
        public int TotalVentas { get; set; }
        public string? TotalIngresos { get; set; }
        public int TotalProductos { get; set; }
        public int TotalCategorias { get; set; }
        public List<VmVentasSemana> VentasUltimaSemana { get; set; }
        public List<VmProductosSemana> ProductosTopUltimaSemana { get; set; }
    }
}
