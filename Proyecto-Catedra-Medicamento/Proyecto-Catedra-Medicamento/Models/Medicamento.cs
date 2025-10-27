using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_Catedra_Medicamento.Models
{
    [Table("medicamento")] // ← nombre real de la tabla en MySQL
    public class Medicamento
    {
        [Key]
        public int id_medicamento { get; set; }

        [Required(ErrorMessage = "El nombre del medicamento es obligatorio")]
        [StringLength(100)]
        public string nombre { get; set; }

        [Required(ErrorMessage = "La presentación es obligatoria")]
        [StringLength(50)]
        public string presentacion { get; set; }

        [StringLength(50)]
        public string marca { get; set; }

        public ICollection<Lote> Lotes { get; set; }
    }
}
