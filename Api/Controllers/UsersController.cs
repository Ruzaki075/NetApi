using Microsoft.AspNetCore.Mvc;
using Api.DTOs;
using Api.Services.Interfaces;

namespace Api.Controllers
{
    /// <summary>
    /// Контроллер для управления пользователями
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        /// <summary>
        /// Конструктор контроллера пользователей
        /// </summary>
        /// <param name="userService">Сервис для работы с пользователями</param>
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Получает список всех пользователей
        /// </summary>
        /// <returns>Список пользователей</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserResponseDto>))]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        /// <summary>
        /// Получает пользователя по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns>Данные пользователя</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponseDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserResponseDto>> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = $"Пользователь с ID {id} не найден" });
            }
            return Ok(user);
        }

        /// <summary>
        /// Создает нового пользователя
        /// </summary>
        /// <param name="createUserDto">Данные для создания пользователя</param>
        /// <returns>Созданный пользователь</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserResponseDto>> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors.Select(e => e.ErrorMessage))
                    .ToList();
                
                return BadRequest(new 
                { 
                    message = "Ошибка валидации данных",
                    errors = errors
                });
            }

            var createdUser = await _userService.CreateUserAsync(createUserDto);
            return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
        }

        /// <summary>
        /// Обновляет данные пользователя
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <param name="updateUserDto">Данные для обновления</param>
        /// <returns>Обновленные данные пользователя</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponseDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserResponseDto>> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors.Select(e => e.ErrorMessage))
                    .ToList();
                
                return BadRequest(new 
                { 
                    message = "Ошибка валидации данных",
                    errors = errors
                });
            }

            var updatedUser = await _userService.UpdateUserAsync(id, updateUserDto);
            if (updatedUser == null)
            {
                return NotFound(new { message = $"Пользователь с ID {id} не найден" });
            }
            return Ok(updatedUser);
        }

        /// <summary>
        /// Удаляет пользователя
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns>Результат операции</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result)
            {
                return NotFound(new { message = $"Пользователь с ID {id} не найден" });
            }
            return NoContent();
        }
    }
}