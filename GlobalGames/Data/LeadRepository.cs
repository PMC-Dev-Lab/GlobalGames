using GlobalGames.Data.Entities;

namespace GlobalGames.Data
{
    public class LeadRepository : GenericRepository<Lead>, ILeadRepository
    {
        public LeadRepository(DataContext context) : base(context)
        {
        }
    }
}
