namespace Proyecto_Catedra_Medicamento.Models.ViewModels
{
    public class VentaViewModel
    {
        public int id_medicamento { get; set; }
        public int id_lote { get; set; }
        public int cantidad { get; set; }
        public decimal precio_unitario { get; set; }
        public string nombre_medicamento { get; set; }
        public string lote_info { get; set; }
        public decimal subtotal => cantidad * precio_unitario;
    }
}
