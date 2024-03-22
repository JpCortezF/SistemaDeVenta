namespace SistemaVenta.AplicacionWeb.Models.ViewModels
{
    public class VmUsuarioLogin
    {
        public string? Correo { get; set; }
        public string? Clave { get; set; }
        public bool MantenerSesion { get; set; }
    }
}
