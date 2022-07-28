using GlobalGames.Data.Entities;
using GlobalGames.Models;

namespace GlobalGames.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        // Subscriber
        public Subscriber ToSubscriber(NewsletterViewModel model, string path, bool isNew)
        {
            return new Subscriber
            {
                Email = model.Email
            };
        }

        // Model-Subscriber
        public NewsletterViewModel ToNewsletterViewModel(Subscriber subscriber)
        {
            return new NewsletterViewModel
            {
                Email = subscriber.Email
            };
        }

        // Lead
        public Lead ToLead(LeadViewModel model, string path, bool isNew)
        {
            return new Lead
            {
                Nome = model.Nome,
                Email = model.Email,
                Message = model.Message,
            };
        }

        // Model-Lead
        public LeadViewModel ToLeadViewModel(Lead lead)
        {
            return new LeadViewModel
            {
                Nome = lead.Nome,
                Email = lead.Email,
                Message = lead.Message,
            };
        }
    }
}
