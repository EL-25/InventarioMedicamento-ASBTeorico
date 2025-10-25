using System.ComponentModel.DataAnnotations;

namespace Proyecto_Catedra_Medicamento.Models
{
    public class Proveedor
    {
        [Key]
        public int id_proveedor { get; set; }

        [Required(ErrorMessage = "El nombre del proveedor es obligatorio")]
        [StringLength(100)]
        public string nombre { get; set; }
    }
}
