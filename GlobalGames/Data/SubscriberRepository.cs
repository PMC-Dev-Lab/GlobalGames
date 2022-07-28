using GlobalGames.Data.Entities;

namespace GlobalGames.Data
{
    public class SubscriberRepository : GenericRepository<Subscriber>, ISubscriberRepository
    {
        public SubscriberRepository(DataContext context) : base(context)
        {
        }
    }
}
