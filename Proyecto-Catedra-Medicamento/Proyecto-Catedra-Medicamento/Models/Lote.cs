using System;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_Catedra_Medicamento.Models
{
    public class Lote
    {
        [Key]
        public int id_lote { get; set; }

        [Required(ErrorMessage = "La fecha de vencimiento es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime fecha_vencimiento { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un medicamento")]
        public int id_medicamento { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un proveedor")]
        public int id_proveedor { get; set; }
    }
}
