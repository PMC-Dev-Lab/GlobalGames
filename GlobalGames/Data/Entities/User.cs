using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GlobalGames.Data.Entities
{
    public class User : IdentityUser
    {
        
        [Display(Name = "Nome")]
        [MaxLength(50, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public string Nome { get; set; }
    }
}
