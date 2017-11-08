using System.ComponentModel.DataAnnotations;

namespace scaffold.Models
{
    public class BidView : BaseEntity
    {
        [Required]
        public float current_bid { get; set; }

        [Required]
        public int listing_id { get; set;}

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public float amount { get; set; }
    }
}