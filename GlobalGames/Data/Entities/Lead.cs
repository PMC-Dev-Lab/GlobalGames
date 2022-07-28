using System.ComponentModel.DataAnnotations;

namespace GlobalGames.Data.Entities
{
    public class Lead : IEntity
    {
        [Display(Name = "Nome")]
        [MaxLength(50, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public string Nome { get; set; }

        [Key]
        [Display(Name = "Email")]
        [MaxLength(250, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }


        [Display(Name = "Message")]
        [MaxLength(1250, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public string Message { get; set; }
    }
}
