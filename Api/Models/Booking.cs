namespace Api.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public int PropertyId { get; set; }
        public int TenantId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = "Pending"; 

        public RentalProperty Property { get; set; } = null!;
        public User Tenant { get; set; } = null!;
    }
}
