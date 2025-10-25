using System.ComponentModel.DataAnnotations;

namespace Proyecto_Catedra_Medicamento.Models
{
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
    }
}
