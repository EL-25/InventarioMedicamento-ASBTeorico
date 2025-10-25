using System;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_Catedra_Medicamento.Models
{
    public class Salida
    {
        [Key]
        public int id_salida { get; set; }

        [Required(ErrorMessage = "La fecha de salida es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime fecha { get; set; }

        [Required(ErrorMessage = "La cantidad egresada es obligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero")]
        public int cantidad { get; set; }

        [StringLength(500, ErrorMessage = "Las observaciones no deben exceder los 500 caracteres")]
        public string observaciones { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un lote")]
        public int id_lote { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un usuario")]
        public int id_usuario { get; set; }
    }
}
