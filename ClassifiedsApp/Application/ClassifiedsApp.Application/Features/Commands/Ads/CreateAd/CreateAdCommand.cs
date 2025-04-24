using ClassifiedsApp.Application.Common.Results;
using ClassifiedsApp.Application.Dtos.Ads;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace ClassifiedsApp.Application.Features.Commands.Ads.CreateAd;

//public class CreateAdCommand : IRequest<Result>
//{
//	public string Title { get; set; }
//	public string Description { get; set; }
//	public decimal Price { get; set; }
//	public bool IsNew { get; set; }
//	public Guid CategoryId { get; set; }
//	public Guid MainCategoryId { get; set; }
//	public Guid LocationId { get; set; }

//	public IList<CreateAdSubCategoryValueDto> SubCategoryValues { get; set; }

//	public IList<IFormFile> Images { get; set; }
//}


public class CreateAdCommand : IRequest<Result>
{
	public string Title { get; set; }
	public string Description { get; set; }
	public decimal Price { get; set; }
	public bool IsNew { get; set; }
	public Guid CategoryId { get; set; }
	public Guid MainCategoryId { get; set; }
	public Guid LocationId { get; set; }

	public string SubCategoryValuesJson { get; set; }

	[NotMapped]
	public IList<CreateAdSubCategoryValueDto> SubCategoryValues =>
		string.IsNullOrWhiteSpace(SubCategoryValuesJson)
			? new List<CreateAdSubCategoryValueDto>()
			: JsonSerializer.Deserialize<IList<CreateAdSubCategoryValueDto>>(SubCategoryValuesJson)!;

	public IList<IFormFile> Images { get; set; }
}

