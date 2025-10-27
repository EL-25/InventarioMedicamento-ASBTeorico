using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Proyecto_Catedra_Medicamento.Models
{
    [Table("lote")]
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

        [Required(ErrorMessage = "La descripción del lote es obligatoria")]
        [StringLength(200, ErrorMessage = "La descripción no debe exceder los 200 caracteres")]
        public string descripcion { get; set; }

        [Required(ErrorMessage = "La cantidad disponible es obligatoria")]
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad debe ser cero o mayor")]
        public int cantidad { get; set; }

        [ForeignKey("id_medicamento")]
        public Medicamento Medicamento { get; set; }

        public ICollection<Entrada> Entradas { get; set; }
        public ICollection<Salida> Salidas { get; set; }

        public ICollection<DetalleVenta> DetalleVentas { get; set; }

    }
}
