using System.ComponentModel.DataAnnotations;

namespace Api.DTOs
{
    /// <summary>
    /// DTO для создания бронирования
    /// </summary>
    public class CreateBookingDto
    {
        [Required(ErrorMessage = "ID объекта аренды обязателен")]
        public int PropertyId { get; set; }

        [Required(ErrorMessage = "ID арендатора обязателен")]
        public int TenantId { get; set; }

        [Required(ErrorMessage = "Дата начала обязательна")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Дата окончания обязательна")]
        public DateTime EndDate { get; set; }
    }

    /// <summary>
    /// DTO для обновления бронирования
    /// </summary>
    public class UpdateBookingDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [StringLength(20, ErrorMessage = "Статус не должен превышать 20 символов")]
        public string? Status { get; set; }
    }

    /// <summary>
    /// DTO для ответа с данными бронирования
    /// </summary>
    public class BookingResponseDto
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public string PropertyTitle { get; set; } = string.Empty;
        public int TenantId { get; set; }
        public string TenantName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}