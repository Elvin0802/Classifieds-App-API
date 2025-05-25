using AutoMapper;
using ClassifiedsApp.Application.Dtos.Auth.Users;
using ClassifiedsApp.Core.Entities;

namespace ClassifiedsApp.Application.Common.Mappings;

public class AppUserMappingProfile : Profile
{
	public AppUserMappingProfile()
	{
		CreateMap<AppUser, AppUserDto>().ReverseMap();
	}
}
