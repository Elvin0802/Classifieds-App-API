using ClassifiedsApp.Application.Dtos.AdImages;
using Microsoft.AspNetCore.Http;

namespace ClassifiedsApp.Application.Interfaces.Services.Ads;

public interface IAdImageService
{
	Task<BlobResponseDto> UploadAsync(IFormFile file, string containerName);
	Task<BlobResponseDto> DeleteAsync(string blobName, string containerName);
	Task<BlobResponseDto> ResizeAndUploadAsync(IFormFile file, string containerName, int width = 800, int height = 600);
	Task<bool> DoesBlobExistAsync(string blobName, string containerName);
}
