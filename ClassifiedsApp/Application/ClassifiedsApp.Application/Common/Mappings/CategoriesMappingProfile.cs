using AutoMapper;
using ClassifiedsApp.Application.Dtos.Categories;
using ClassifiedsApp.Core.Entities;

namespace ClassifiedsApp.Application.Common.Mappings;

public class CategoriesMappingProfile : Profile
{
	public CategoriesMappingProfile()
	{
		CreateMap<Category, CategoryDto>().ReverseMap();
		CreateMap<MainCategory, MainCategoryDto>().ReverseMap();
		CreateMap<SubCategory, SubCategoryDto>().ReverseMap();
		CreateMap<SubCategoryOption, SubCategoryOptionDto>().ReverseMap();
	}
}
