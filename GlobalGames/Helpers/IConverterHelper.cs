using GlobalGames.Data.Entities;
using GlobalGames.Models;

namespace GlobalGames.Helpers
{
    public interface IConverterHelper
    {
        Subscriber ToSubscriber(NewsletterViewModel model, string path, bool isNew);

        NewsletterViewModel ToNewsletterViewModel(Subscriber subscriber);

        Lead ToLead(LeadViewModel model, string path, bool isNew);

        LeadViewModel ToLeadViewModel(Lead lead);
    }
}
