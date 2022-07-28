using System.ComponentModel.DataAnnotations;

namespace GlobalGames.Models
{
    public class LoginViewModel
    {

        [Required]
        [EmailAddress] // @ obrigatório
        public string Username { get; set; }

        [Required]
        [MinLength(6)] // 6 caracteres da password
        public string Password { get; set; }

        // lembra da conta
        public bool RememberMe { get; set; }
    }
}
