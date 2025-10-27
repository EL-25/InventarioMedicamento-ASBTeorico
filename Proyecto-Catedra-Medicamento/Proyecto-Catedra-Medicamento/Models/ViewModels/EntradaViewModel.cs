namespace Proyecto_Catedra_Medicamento.Models.ViewModels
{
    public class EntradaViewModel
    {
        public string nombre_medicamento { get; set; }
        public string nombre_proveedor { get; set; }
        public DateTime fecha_vencimiento { get; set; }
        public int cantidad { get; set; }
        public DateTime fecha { get; set; }
        public string nombre_usuario { get; set; }
    }
}
