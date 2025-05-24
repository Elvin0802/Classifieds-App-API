using Microsoft.Extensions.Configuration;

namespace ClassifiedsApp.Infrastructure.Persistence.Context;

public static class Configuration
{
	static public string ConnectionString
	{
		get
		{
			ConfigurationManager configurationManager = new();
			try
			{
				configurationManager.SetBasePath(Directory.GetCurrentDirectory());

				configurationManager.AddJsonFile("appsettings.json");
			}
			catch
			{
				configurationManager.AddJsonFile("appsettings.Production.json");
			}

			return configurationManager.GetConnectionString("Default")!;
		}
	}
}
