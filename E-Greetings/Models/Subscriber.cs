using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Greetings.Models
{
    public class Subscriber
    {
        [Key]
        public int Id { get; set; }

        [Column("user_id")]
        [Required(ErrorMessage = "User Is Required For The Subscription")]
        public int? UserId { get; set; }


        [Column("email")]
        public string? Email { get;set; }


        [Column("is_guest")]
        public int IsGuest {  get; set; }

        [Column("ip_address")]
        public string? IpAddress { get; set; }

        public string? Fee { get; set; }

        [Column("status")]
        [Required(ErrorMessage = "The Status Of Subscription is Required")]
        public int? Status { get; set; }

        [Column("subscription_start_date")]
        public DateTime? SubscriptionStartDate { get; set; }

        [Column("subscription_end_date")]
        public DateTime? SubscriptionEndDate { get; set; }


        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        public virtual User? User { get; set; }

    }
}
