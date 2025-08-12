using AutoMapper;
using FlashcardApp.Core.Auth.DTO;
using FlashcardApp.Core.Auth.Models;

namespace FlashcardApp.Core.MappingProfiles
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email)) // Ustaw UserName na Email
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Nickname, opt => opt.MapFrom(src => src.Nickname))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Points, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.StreakCount, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.ConsecutiveDays, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.SubscriptionType, opt => opt.MapFrom(src => "Free"))
                .ForMember(dest => dest.EmailVerified, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()); // Ignoruj, ustawimy w AuthService
        }
    }
}