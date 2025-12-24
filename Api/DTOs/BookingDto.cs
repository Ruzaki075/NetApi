using System.ComponentModel.DataAnnotations;
using Api.Validators;

namespace Api.DTOs
{
    /// <summary>
    /// DTO для создания бронирования
    /// </summary>
    [DateRange(nameof(StartDate), nameof(EndDate))]
    public class CreateBookingDto
    {
        [Required(ErrorMessage = "ID объекта аренды обязателен")]
        [Range(1, int.MaxValue, ErrorMessage = "ID объекта аренды должен быть больше 0")]
        public int PropertyId { get; set; }

        /// <summary>
        /// ID арендатора. Если не указан, можно передать через query параметр tenantId
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "ID арендатора должен быть больше 0")]
        public int? TenantId { get; set; }

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