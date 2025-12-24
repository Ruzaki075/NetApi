using AutoMapper;
using Api.DTOs;
using Api.Models;
using Api.Repositories.Interfaces;
using Api.Services.Interfaces;

namespace Api.Services
{
    /// <summary>
    /// Сервис для работы с пользователями
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Конструктор сервиса пользователей
        /// </summary>
        /// <param name="userRepository">Репозиторий для работы с пользователями</param>
        /// <param name="mapper">Маппер для преобразования сущностей в DTO</param>
        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Получает список всех пользователей
        /// </summary>
        /// <returns>Коллекция DTO пользователей</returns>
        public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserResponseDto>>(users);
        }

        /// <summary>
        /// Получает пользователя по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns>DTO пользователя или null, если пользователь не найден</returns>
        public async Task<UserResponseDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user == null ? null : _mapper.Map<UserResponseDto>(user);
        }

        /// <summary>
        /// Создает нового пользователя
        /// </summary>
        /// <param name="createUserDto">DTO с данными для создания пользователя</param>
        /// <returns>DTO созданного пользователя</returns>
        /// <exception cref="InvalidOperationException">Выбрасывается, если пользователь с таким email уже существует</exception>
        public async Task<UserResponseDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            // Проверка уникальности email
            var existingUser = await _userRepository.GetByEmailAsync(createUserDto.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException($"Пользователь с email {createUserDto.Email} уже существует");
            }

            var user = _mapper.Map<User>(createUserDto);
            var createdUser = await _userRepository.AddAsync(user);
            return _mapper.Map<UserResponseDto>(createdUser);
        }

        /// <summary>
        /// Обновляет данные пользователя
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <param name="updateUserDto">DTO с данными для обновления</param>
        /// <returns>DTO обновленного пользователя или null, если пользователь не найден</returns>
        /// <exception cref="InvalidOperationException">Выбрасывается, если новый email уже используется другим пользователем</exception>
        public async Task<UserResponseDto?> UpdateUserAsync(int id, UpdateUserDto updateUserDto)
        {
            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null) return null;

            // Проверка уникальности email при обновлении
            if (!string.IsNullOrEmpty(updateUserDto.Email) && updateUserDto.Email != existingUser.Email)
            {
                var userWithEmail = await _userRepository.GetByEmailAsync(updateUserDto.Email);
                if (userWithEmail != null && userWithEmail.Id != id)
                {
                    throw new InvalidOperationException($"Пользователь с email {updateUserDto.Email} уже существует");
                }
            }

            _mapper.Map(updateUserDto, existingUser);
            await _userRepository.UpdateAsync(existingUser);
            return _mapper.Map<UserResponseDto>(existingUser);
        }

        /// <summary>
        /// Удаляет пользователя
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns>true, если пользователь удален, false если пользователь не найден</returns>
        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return false;

            await _userRepository.DeleteAsync(user);
            return true;
        }
    }
}