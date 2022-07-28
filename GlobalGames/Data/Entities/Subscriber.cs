using System.ComponentModel.DataAnnotations;

namespace GlobalGames.Data.Entities
{
    public class Subscriber : IEntity
    {
        [Key]
        [Display(Name = "Email")]
        [MaxLength(250, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

    }
}
