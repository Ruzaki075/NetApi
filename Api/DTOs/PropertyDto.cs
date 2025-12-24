using System.ComponentModel.DataAnnotations;

namespace Api.DTOs
{
    /// <summary>
    /// DTO для создания объекта аренды
    /// </summary>
    public class CreatePropertyDto
    {
        [Required(ErrorMessage = "Название обязательно")]
        [StringLength(200, ErrorMessage = "Название не должно превышать 200 символов")]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Адрес обязателен")]
        public string Address { get; set; } = string.Empty;

        [Range(0.01, 10000, ErrorMessage = "Цена должна быть от 0.01 до 10000")]
        public decimal PricePerDay { get; set; }

        [Required(ErrorMessage = "ID владельца обязателен")]
        [Range(1, int.MaxValue, ErrorMessage = "ID владельца должен быть больше 0")]
        public int OwnerId { get; set; }
    }

    /// <summary>
    /// DTO для обновления объекта аренды
    /// </summary>
    public class UpdatePropertyDto
    {
        [StringLength(200, ErrorMessage = "Название не должно превышать 200 символов")]
        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? Address { get; set; }

        [Range(0.01, 10000, ErrorMessage = "Цена должна быть от 0.01 до 10000")]
        public decimal? PricePerDay { get; set; }
    }

    /// <summary>
    /// DTO для ответа с данными объекта аренды
    /// </summary>
    public class PropertyResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public decimal PricePerDay { get; set; }
        public int OwnerId { get; set; }
        public string OwnerName { get; set; } = string.Empty;
    }
}