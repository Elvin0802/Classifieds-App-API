using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ClassifiedsApp.Application.Dtos.AdImages;
using ClassifiedsApp.Application.Interfaces.Services.Ads;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace ClassifiedsApp.Infrastructure.Services.Ads;

public class AdImageService : IAdImageService
{
	readonly BlobServiceClient _blobServiceClient;
	readonly ILogger<AdImageService> _logger;

	public AdImageService(BlobServiceClient blobServiceClient, ILogger<AdImageService> logger)
	{
		_blobServiceClient = blobServiceClient;
		_logger = logger;
	}

	public async Task<BlobResponseDto> UploadAsync(IFormFile file, string containerName)
	{
		BlobResponseDto response = new();

		try
		{
			var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
			await containerClient.CreateIfNotExistsAsync();

			string uniqueBlobName = $"{Guid.NewGuid()}-{file.FileName}";
			var blobClient = containerClient.GetBlobClient(uniqueBlobName);

			await using var data = file.OpenReadStream();
			await blobClient.UploadAsync(data, new BlobHttpHeaders { ContentType = file.ContentType });

			response.Status = $"File {uniqueBlobName} Uploaded Successfully.";
			response.Error = false;
			response.Url = blobClient.Uri.AbsoluteUri;
			response.BlobName = uniqueBlobName;

			return response;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error uploading blob.");
			response.Status = ex.Message;
			response.Error = true;
			return response;
		}
	}

	public async Task<BlobResponseDto> ResizeAndUploadAsync(IFormFile file, string containerName, int width = 800, int height = 600)
	{
		BlobResponseDto response = new();

		try
		{
			var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
			await containerClient.CreateIfNotExistsAsync();

			string uniqueBlobName = $"{Guid.NewGuid()}-{file.FileName}";
			var blobClient = containerClient.GetBlobClient(uniqueBlobName);

			// Image resize işlemi
			using var inputStream = file.OpenReadStream();
			using var image = await Image.LoadAsync(inputStream);

			// En-boy oranını koruyarak boyutlandırma
			image.Mutate(x => x.Resize(new ResizeOptions
			{
				Size = new Size(width, height),
				Mode = ResizeMode.Max
			}));

			// Resized image'i upload etme
			using var outputStream = new MemoryStream();
			await image.SaveAsJpegAsync(outputStream);
			outputStream.Position = 0;

			await blobClient.UploadAsync(outputStream, new BlobHttpHeaders { ContentType = "image/jpeg" });

			response.Status = $"File {uniqueBlobName} Resized and Uploaded Successfully.";
			response.Error = false;
			response.Url = blobClient.Uri.AbsoluteUri;
			response.BlobName = uniqueBlobName;

			return response;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error resizing and uploading blob.");
			response.Status = ex.Message;
			response.Error = true;
			return response;
		}
	}

	public async Task<BlobResponseDto> DeleteAsync(string blobName, string containerName)
	{
		BlobResponseDto response = new();

		try
		{
			var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
			var blobClient = containerClient.GetBlobClient(blobName);

			await blobClient.DeleteIfExistsAsync();

			response.Status = $"File {blobName} Deleted Successfully.";
			response.Error = false;

			return response;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error deleting blob.");
			response.Status = ex.Message;
			response.Error = true;
			return response;
		}
	}

	public async Task<bool> DoesBlobExistAsync(string blobName, string containerName)
	{
		try
		{
			var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
			var blobClient = containerClient.GetBlobClient(blobName);

			return await blobClient.ExistsAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error checking if blob exists.");
			return false;
		}
	}
}
