using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_Catedra_Medicamento.Models
{
    [Table("venta")]
    public class Venta
    {
        [Key]
        public int id_venta { get; set; }

        [Required(ErrorMessage = "La fecha de la venta es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime fecha { get; set; }

        [Required(ErrorMessage = "Debe especificar el usuario que realiza la venta")]
        public int id_usuario { get; set; }

        // Relación con Usuario
        [ForeignKey("id_usuario")]
        public Usuario Usuario { get; set; }

        // Relación con DetalleVenta
        public ICollection<DetalleVenta> Detalles { get; set; }
    }
}
