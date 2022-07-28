using GlobalGames.Data.Entities;
using GlobalGames.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace GlobalGames.Helpers
{
    public interface IUserHelper
    {
        Task<User> GetUserByEmailAsync(string email);

        Task<IdentityResult> AddUserAsync(User user, string password);

        // Faz o login
        Task<SignInResult> LoginAsync(LoginViewModel model);

        // Faz o logout
        Task LogoutAsync();

        // Faz o update aos dados do utilizador
        Task<IdentityResult> UpdateUserAsync(User user);


        // Faz o update da mudança da password
        Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword);

        // Cria os roles aqui
        // verifica se tem um determinado role se não tem cria.
        //Task CheckRoleAsync(string roleName);

        // Adiciona o role Admin
        // Adiciona um determinado role a um user
        //Task AddUserToRoleAsync(User user, string roleName);

        // Se tem ou não
        // retorna true or false
        // Ve se o user já tem este Role ou não
        //Task<bool> IsUserInRoleAsync(User user, string roleName);
    }
}
