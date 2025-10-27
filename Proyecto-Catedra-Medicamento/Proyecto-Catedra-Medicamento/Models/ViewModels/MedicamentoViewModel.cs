namespace Proyecto_Catedra_Medicamento.Models.ViewModels
{
    public class MedicamentoViewModel
    {
        public int id_medicamento { get; set; }
        public string nombre { get; set; }
        public string presentacion { get; set; }
        public string marca { get; set; }
        public string proveedor { get; set; }
        public DateTime fecha_vencimiento { get; set; }
        public int cantidad_total { get; set; }
        public ICollection<Lote> Lote { get; set; }

    }
}
