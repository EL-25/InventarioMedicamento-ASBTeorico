namespace Proyecto_Catedra_Medicamento.Models.ViewModels
{
    public class LoteDisponibleViewModel
    {
        public int id_lote { get; set; }
        public int id_medicamento { get; set; }
        public string nombre_medicamento { get; set; }
        public string presentacion { get; set; }
        public int stock_disponible { get; set; }
    }
}
