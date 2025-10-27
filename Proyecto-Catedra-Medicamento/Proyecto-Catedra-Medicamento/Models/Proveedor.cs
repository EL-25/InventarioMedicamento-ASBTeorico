using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_Catedra_Medicamento.Models
{
    [Table("proveedor")] // ← nombre real de la tabla en MySQL
    public class Proveedor
    {
        [Key]
        public int id_proveedor { get; set; }

        [Required(ErrorMessage = "El nombre del proveedor es obligatorio")]
        [StringLength(100)]
        public string nombre { get; set; }
    }
}
