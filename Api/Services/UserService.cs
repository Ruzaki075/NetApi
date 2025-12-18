using AutoMapper;
using Api.DTOs;
using Api.Models;
using Api.Repositories.Interfaces;
using Api.Services.Interfaces;

namespace Api.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserResponseDto>>(users);
        }

        public async Task<UserResponseDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user == null ? null : _mapper.Map<UserResponseDto>(user);
        }

        public async Task<UserResponseDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            var user = _mapper.Map<User>(createUserDto);
            var createdUser = await _userRepository.AddAsync(user);
            return _mapper.Map<UserResponseDto>(createdUser);
        }

        public async Task<UserResponseDto?> UpdateUserAsync(int id, UpdateUserDto updateUserDto)
        {
            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null) return null;

            _mapper.Map(updateUserDto, existingUser);
            await _userRepository.UpdateAsync(existingUser);
            return _mapper.Map<UserResponseDto>(existingUser);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return false;

            await _userRepository.DeleteAsync(user);
            return true;
        }
    }
}