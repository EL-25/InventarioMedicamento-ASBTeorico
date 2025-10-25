using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_Catedra_Medicamento.Models
{
    public class Usuario
    {
        [Key]
        public int id_usuario { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string nombre { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [StringLength(50, ErrorMessage = "El usuario debe tener máximo 50 caracteres")]
        public string usuario { get; set; }

        //[Required(ErrorMessage = "La contraseña es obligatoria")]
        //[StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string contrasena { get; set; }

        [BindNever]
        public string rol { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [Phone(ErrorMessage = "Ingrese un número de teléfono válido")]
        public string telefono { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido")]
        public string correo { get; set; }

        [Required(ErrorMessage = "El departamento es obligatorio")]
        [StringLength(100)]
        public string departamento { get; set; }

        [Required(ErrorMessage = "El municipio es obligatorio")]
        [StringLength(100)]
        public string municipio { get; set; }
    }
}