using GlobalGames.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GlobalGames.Data
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class, IEntity
    {
        private readonly DataContext _context;


        public GenericRepository(DataContext context)
        {
            _context = context;
        }


        public IQueryable<T> GetAll()
        {
            return _context.Set<T>().AsNoTracking();
        }



        public async Task<T> GetByEmailAsync(string email)
        {
            return await _context.Set<T>()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Email == email);
        }



        public async Task CreateAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await SaveAllAsync();
        }



        public async Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await SaveAllAsync();
        }



        public async Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            await SaveAllAsync();
        }



        private async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }



        public async Task<bool> ExistAsync(string email)
        {
            return await _context.Set<T>().AnyAsync(e => e.Email == email);
        }
    }
}
