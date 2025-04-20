using System.ComponentModel.DataAnnotations.Schema;

namespace E_Greetings.Models
{
    public class SubscriberViewModel
    {
        
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Fee { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
        public DateTime? SubscriptionStartDate { get; set; }
        public int? Status { get; set; }
        public string? Email { get; set; }
        public int IsGuest { get; set; }
        public string? IpAddress { get; set; }
        public string? Created_at { get; set; }
    }

}
