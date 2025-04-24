using ClassifiedsApp.Application.Interfaces.Services.Ads;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace ClassifiedsApp.Infrastructure.Services.Ads;

public class AiService : IAiService
{
	private readonly IConfiguration _configuration;
	private readonly HttpClient _httpClient;
	private readonly ILogger<AiService> _logger;

	public AiService(IConfiguration configuration, HttpClient httpClient, ILogger<AiService> logger)
	{
		_configuration = configuration;
		_httpClient = httpClient;
		_logger = logger;
	}

	public async Task<bool> CheckAdContent(string title, string description, List<string> categories)
	{
		try
		{
			string apiKey = _configuration["Gemini:ApiKey"];
			string apiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent";

			string prompt = $@"İlan başlığı: {title}
İlan açıklaması: {description}
İlan kategorileri: {string.Join(", ", categories)}

Bu ilanın uygun olup olmadığını kontrol edin. Uygunsuz, yasadışı, spam veya site kurallarına aykırı içerik varsa 'false' döndürün, ilanı yayınlamak için uygunsa 'true' döndürün. Cevabınız sadece 'true' veya 'false' olmalıdır.";

			var request = new
			{
				contents = new[]
				{
					new
					{
						parts = new[]
						{
							new
							{
								text = prompt
							}
						}
					}
				}
			};

			var requestJson = JsonSerializer.Serialize(request);
			var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

			_logger.LogInformation("Api sending request to Ai. Ai content : {content} ;", content);

			var response = await _httpClient.PostAsync($"{apiUrl}?key={apiKey}", content);
			response.EnsureSuccessStatusCode();

			var responseJson = await response.Content.ReadAsStringAsync();
			var responseData = JsonSerializer.Deserialize<JsonElement>(responseJson);

			string aiResponse = responseData
				.GetProperty("candidates")[0]
				.GetProperty("content")
				.GetProperty("parts")[0]
				.GetProperty("text")
				.GetString()!
				.ToLower()
				.Trim();

			_logger.LogInformation("Api executes 'CheckAdContent' method successfully. Ai response : {aiResponse} ;", aiResponse);

			aiResponse = aiResponse.Trim().ToLower();

			return aiResponse.Contains("true");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error occoured when api executes 'CheckAdContent' method.");
			return false;
		}
	}
}


