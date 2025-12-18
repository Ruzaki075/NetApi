using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class RentalProperty
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        [Range(0.01, 10000)]
        public decimal PricePerDay { get; set; }

        public int OwnerId { get; set; }

        public User Owner { get; set; } = null!;
        public List<Booking> Bookings { get; set; } = new();
    }
}
