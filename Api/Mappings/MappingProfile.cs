using AutoMapper;
using Api.Models;
using Api.DTOs;

namespace Api.Mappings
{
    /// <summary>
    /// Профиль маппинга между сущностями и DTO
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Конструктор с настройкой маппингов
        /// </summary>
        public MappingProfile()
        {

            CreateMap<User, UserResponseDto>();

            CreateMap<CreateUserDto, User>();

            CreateMap<UpdateUserDto, User>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null));

            CreateMap<RentalProperty, PropertyResponseDto>()
                .ForMember(dest => dest.OwnerName, opt =>
                    opt.MapFrom(src => src.Owner.Name));

            CreateMap<CreatePropertyDto, RentalProperty>();

            CreateMap<UpdatePropertyDto, RentalProperty>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null));

            CreateMap<Booking, BookingResponseDto>()
                .ForMember(dest => dest.PropertyTitle, opt =>
                    opt.MapFrom(src => src.Property.Title))
                .ForMember(dest => dest.TenantName, opt =>
                    opt.MapFrom(src => src.Tenant.Name));

            CreateMap<CreateBookingDto, Booking>()
                .ForMember(dest => dest.TenantId, opt => 
                    opt.MapFrom(src => src.TenantId ?? 0)); // Временное значение, будет перезаписано в сервисе

            CreateMap<UpdateBookingDto, Booking>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                    srcMember != null));
        }
    }
}