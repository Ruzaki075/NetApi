using System.ComponentModel.DataAnnotations;

namespace Api.DTOs
{
    /// <summary>
    /// DTO для создания пользователя
    /// </summary>
    public class CreateUserDto
    {
        [Required(ErrorMessage = "Имя обязательно")]
        [StringLength(100, ErrorMessage = "Имя не должно превышать 100 символов")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Некорректный формат телефона")]
        public string? Phone { get; set; }
    }

    /// <summary>
    /// DTO для обновления пользователя
    /// </summary>
    public class UpdateUserDto
    {
        [StringLength(100, ErrorMessage = "Имя не должно превышать 100 символов")]
        public string? Name { get; set; }

        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Некорректный формат телефона")]
        public string? Phone { get; set; }
    }

    /// <summary>
    /// DTO для ответа с данными пользователя
    /// </summary>
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
    }
}