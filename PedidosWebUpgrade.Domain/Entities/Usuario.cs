using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PedidosWebUpgrade.Domain.Entities
{
    public class Usuario
    {
        [Key]
        public decimal IdUsuario { get; set; }

        [Display(Name = "Cédula de identidad:")]
        [Required]
        public string? Cedula { get; set; }

        [Display(Name = "Dirección:")]
        public string? Direccion { get; set; }

        [Display(Name = "Correo electrónico:")]
        [EmailAddress(ErrorMessage = "Formato inválido")]
        public string? Email { get; set; }

        [Display(Name = "Correo electrónico Coordinador:")]
        [EmailAddress(ErrorMessage = "Formato inválido")]
        public string? MailCoordinador { get; set; }

        [Display(Name = "Estatus:")]
        public int IdEstado { get; set; }

        [Display(Name = "Login:")]
        [Required]
        public string? Login { get; set; }

        [Display(Name = "Contraseña:")]
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Display(Name = "Primer apellido:")]
        [Required]
        public string? PrimerApellido { get; set; }

        [Display(Name = "Primer nombre:")]
        [Required]
        public string? PrimerNombre { get; set; }

        [Display(Name = "Segundo nombre:")]
        public string? SegundoNombre { get; set; }

        [Display(Name = "Segundo apellido:")]
        public string? SegundoApellido { get; set; }

        [Display(Name = "Sexo:")]
        [Required]
        public string? Sexo { get; set; }

        [Display(Name = "Teléfono:")]
        [Required]
        public string? Telefono { get; set; }
        public string? usuario_auditoria { get; set; }

        [Display(Name = "Región:")]
        [Required]
        public string? Region { get; set; }

        [Display(Name = "Tipo de usuario:")]
        public string? TipoUsuario { get; set; }

        public List<ListaGeneral> Sexos = new List<ListaGeneral>();

        public List<ListaGeneral> Estatus = new List<ListaGeneral>();

        public List<PerfilUsuario> Perfiles { get; set; } = new List<PerfilUsuario>();

        public List<ListaGeneral> TiposUsuarios { get; set; } = new List<ListaGeneral>();

        //[Required(ErrorMessage = "Campo requerido!")]
        [StringLength(50, ErrorMessage = "Debe contener al menos 8 caracteres.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva contraseña")]
        public string? NuevoPassword { get; set; }

        [NotMapped]
        //[Required(ErrorMessage = "Campo requerido!")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("NuevoPassword", ErrorMessage = "La nueva contraseña y su confirmación deben coincidir.")]
        public string? ConfirmarPassword { get; set; }

        [Display(Name = "Código vendedor")]
        public string? id_vendedor { get; set; }

        /// <summary>
        /// CONSTRUCTOR
        /// </summary>
        public Usuario()
        {
            Sexos = new List<ListaGeneral>() { new ListaGeneral { Codigo = "M", Descripcion = "Masculino" },
                                               new ListaGeneral { Codigo = "F", Descripcion = "Femenino" }
                                            };

            Estatus = new List<ListaGeneral>() { new ListaGeneral { IdTipo = 0, Descripcion = "Inactivo" },
                                               new ListaGeneral { IdTipo = 1, Descripcion = "Activo" }
                                            };

        }
    }

    public class UsuarioLogin
    {

        [Required(ErrorMessage = "Ingresa tu usuario")]
        public string? Cuenta { get; set; }

        [Required(ErrorMessage = "Ingresa tu contraseña")]
        public string? Clave { get; set; }
    }
}
