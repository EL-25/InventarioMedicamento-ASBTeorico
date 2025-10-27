using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_Catedra_Medicamento.Models
{
    [Table("detalle_venta")]
    public class DetalleVenta
    {
        [Key]
        public int id_detalle { get; set; }

        [Required]
        public int id_lote { get; set; }

        [Required]
        public int id_venta { get; set; }

        [Required]
        public int cantidad { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal precio_unitario { get; set; }

        [ForeignKey(nameof(id_lote))]
        public Lote Lote { get; set; }

        [ForeignKey(nameof(id_venta))]
        public Venta Venta { get; set; }
    }
}

