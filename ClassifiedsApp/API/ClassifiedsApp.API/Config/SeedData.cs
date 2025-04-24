using ClassifiedsApp.Application.Interfaces.Repositories.Categories;
using ClassifiedsApp.Application.Interfaces.Repositories.Locations;
using ClassifiedsApp.Core.Entities;
using ClassifiedsApp.Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ClassifiedsApp.API.Config;

public static class SeedData
{
	public static async Task AddSeedRolesAndUsersAsync(RoleManager<AppRole> roleManager,
														UserManager<AppUser> userManager,
														IConfigurationManager configuration)
	{
		if (!await roleManager.RoleExistsAsync("Admin"))
			await roleManager.CreateAsync(new AppRole("Admin"));

		if (!await roleManager.RoleExistsAsync("User"))
			await roleManager.CreateAsync(new AppRole("User"));

		var adminEmail = configuration["Admin:Email"]!;
		var adminUser = await userManager.FindByEmailAsync(adminEmail);

		if (adminUser is null)
		{
			var admin = new AppUser()
			{
				UserName = adminEmail,
				Email = adminEmail,
				EmailConfirmed = true,
				Name = configuration["Admin:Name"]!,
				PhoneNumber = configuration["Admin:PhoneNumber"]!,
			};

			var result = await userManager.CreateAsync(admin, configuration["Admin:Password"]!);

			if (result.Succeeded)
				await userManager.AddToRoleAsync(admin, "Admin");
		}
	}

	public static async Task AddSeedDataAsync(ICategoryReadRepository categoryReadRepository,
											  ICategoryWriteRepository categoryWriteRepository,
											  IMainCategoryReadRepository mainCategoryReadRepository,
											  IMainCategoryWriteRepository mainCategoryWriteRepository,
											  ISubCategoryReadRepository subCategoryReadRepository,
											  ISubCategoryWriteRepository subCategoryWriteRepository,
											  ILocationReadRepository locationReadRepository,
											  ILocationWriteRepository locationWriteRepository)
	{
		//-----------

		List<Category> categories = [
			new() { Name="Transportation", Slug="Transportation".ToLower() },
			new() { Name="Electronics", Slug="Electronics".ToLower() },
			//new() { Name="Real estate", Slug="Real estate".ToLower().Replace(" ", "-") },
			//new() { Name="Hobbies", Slug="Hobbies".ToLower().Replace(" ", "-") },
			//new() { Name="Services and business", Slug="Services and business".ToLower().Replace(" ", "-") },
			//new() { Name="Other", Slug="Other".ToLower().Replace(" ", "-") },
			//new() { Name="Childrens world", Slug="Childrens world".ToLower().Replace(" ", "-") },
			//new() { Name="Personal items", Slug="Personal items".ToLower().Replace(" ", "-") }
		];

		foreach (var c in categories)
		{
			var cat = await categoryReadRepository.Table.FirstOrDefaultAsync(ca => ca.Name == c.Name);

			if (cat is null)
				await categoryWriteRepository.AddAsync(c);
		}
		await categoryWriteRepository.SaveAsync();

		//-----------

		List<MainCategory> mainCategories = [
			new() { Name="Cars", Slug="Cars".ToLower(), ParentCategoryId=categories[0].Id },
			new() { Name="Motors", Slug="Motors".ToLower(), ParentCategoryId=categories[0].Id },
			//new() { Name="Accessories", Slug="Accessories".ToLower(), ParentCategoryId=categories[0].Id },
			new() { Name="Phones", Slug="Phones".ToLower(), ParentCategoryId=categories[1].Id },
			new() { Name="Phone numbers", Slug="Phone numbers".ToLower().Replace(" ", "-"), ParentCategoryId=categories[1].Id },
		];

		foreach (var c in mainCategories)
		{
			var cat = await mainCategoryReadRepository.Table.FirstOrDefaultAsync(ca => ca.Name == c.Name);

			if (cat is null)
				await mainCategoryWriteRepository.AddAsync(c);
		}
		await mainCategoryWriteRepository.SaveAsync();

		//-----------

		List<SubCategory> subCategories = [
			new() { Name="Marka", IsRequired=true, IsSearchable=true, Type=SubCategoryType.Select, MainCategoryId=mainCategories[0].Id },
			new() { Name="Model", IsRequired=true, IsSearchable=true, Type=SubCategoryType.Select, MainCategoryId=mainCategories[0].Id },
			new() { Name="Color", IsRequired=true, IsSearchable=true, Type=SubCategoryType.Select, MainCategoryId=mainCategories[0].Id },

			new() { Name="Gas Type", IsRequired=true, IsSearchable=true, Type=SubCategoryType.Select, MainCategoryId=mainCategories[1].Id },
			new() { Name="Km", IsRequired=true, IsSearchable=true, Type=SubCategoryType.Number, MainCategoryId=mainCategories[1].Id },
			new() { Name="Color", IsRequired=true, IsSearchable=true, Type=SubCategoryType.Select, MainCategoryId=mainCategories[1].Id },

			new() { Name="Marka", IsRequired=true, IsSearchable=true, Type=SubCategoryType.Select, MainCategoryId=mainCategories[2].Id },
			new() { Name="Model", IsRequired=true, IsSearchable=true, Type=SubCategoryType.Select, MainCategoryId=mainCategories[2].Id },
			new() { Name="Ram and Rom", IsRequired=true, IsSearchable=true, Type=SubCategoryType.Select, MainCategoryId=mainCategories[2].Id },

			new() { Name="Operator", IsRequired=true, IsSearchable=true, Type=SubCategoryType.Select, MainCategoryId=mainCategories[3].Id },
		];

		subCategories[0].Options = [
			new() { SortOrder=0, SubCategoryId=subCategories[0].Id, Value="Bmw" },
			new() { SortOrder=1, SubCategoryId=subCategories[0].Id, Value="Opel" },
			new() { SortOrder=2, SubCategoryId=subCategories[0].Id, Value="Volvo" },
		];

		subCategories[1].Options = [
			new() { SortOrder=0, SubCategoryId=subCategories[1].Id, Value="530" },
			new() { SortOrder=1, SubCategoryId=subCategories[1].Id, Value="Astra" },
			new() { SortOrder=2, SubCategoryId=subCategories[1].Id, Value="Xc90" },
		];

		subCategories[2].Options = [
			new() { SortOrder=0, SubCategoryId=subCategories[1].Id, Value="White" },
			new() { SortOrder=1, SubCategoryId=subCategories[1].Id, Value="Black" },
			new() { SortOrder=2, SubCategoryId=subCategories[1].Id, Value="Green" },
			new() { SortOrder=3, SubCategoryId=subCategories[1].Id, Value="Red" },
			new() { SortOrder=4, SubCategoryId=subCategories[1].Id, Value="Blue" },
			new() { SortOrder=5, SubCategoryId=subCategories[1].Id, Value="Yellow" },
		];

		subCategories[3].Options = [
			new() { SortOrder=0, SubCategoryId=subCategories[1].Id, Value="Benzin" },
			new() { SortOrder=1, SubCategoryId=subCategories[1].Id, Value="Dizel" },
			new() { SortOrder=2, SubCategoryId=subCategories[1].Id, Value="Elektrik" },
		];

		subCategories[5].Options = [
			new() { SortOrder=0, SubCategoryId=subCategories[1].Id, Value="White" },
			new() { SortOrder=1, SubCategoryId=subCategories[1].Id, Value="Black" },
			new() { SortOrder=2, SubCategoryId=subCategories[1].Id, Value="Green" },
			new() { SortOrder=3, SubCategoryId=subCategories[1].Id, Value="Red" },
			new() { SortOrder=4, SubCategoryId=subCategories[1].Id, Value="Blue" },
			new() { SortOrder=5, SubCategoryId=subCategories[1].Id, Value="Yellow" },
		];

		subCategories[6].Options = [
			new() { SortOrder=0, SubCategoryId=subCategories[1].Id, Value="Samsung" },
			new() { SortOrder=1, SubCategoryId=subCategories[1].Id, Value="Apple" },
			new() { SortOrder=2, SubCategoryId=subCategories[1].Id, Value="Huawei" },
			new() { SortOrder=3, SubCategoryId=subCategories[1].Id, Value="Oppo" },
			new() { SortOrder=4, SubCategoryId=subCategories[1].Id, Value="Xiaomi" },
			new() { SortOrder=5, SubCategoryId=subCategories[1].Id, Value="Nothing" },
		];

		subCategories[7].Options = [
			new() { SortOrder=0, SubCategoryId=subCategories[1].Id, Value="S 24 Ultra" },
			new() { SortOrder=1, SubCategoryId=subCategories[1].Id, Value="iPhone 16 Pro Max" },
			new() { SortOrder=2, SubCategoryId=subCategories[1].Id, Value="Pura 70 Ultra" },
			new() { SortOrder=3, SubCategoryId=subCategories[1].Id, Value="A54" },
			new() { SortOrder=4, SubCategoryId=subCategories[1].Id, Value="Mi 14 Pro" },
			new() { SortOrder=5, SubCategoryId=subCategories[1].Id, Value="Phone a1" },
		];

		subCategories[8].Options = [
			new() { SortOrder=0, SubCategoryId=subCategories[1].Id, Value="8/128" },
			new() { SortOrder=1, SubCategoryId=subCategories[1].Id, Value="12/128" },
			new() { SortOrder=2, SubCategoryId=subCategories[1].Id, Value="8/256" },
			new() { SortOrder=3, SubCategoryId=subCategories[1].Id, Value="8/512" },
			new() { SortOrder=4, SubCategoryId=subCategories[1].Id, Value="12/256" },
			new() { SortOrder=5, SubCategoryId=subCategories[1].Id, Value="12/512" },
			new() { SortOrder=6, SubCategoryId=subCategories[1].Id, Value="12/1024" },
			new() { SortOrder=7, SubCategoryId=subCategories[1].Id, Value="16/512" },
			new() { SortOrder=8, SubCategoryId=subCategories[1].Id, Value="6/128" },
			new() { SortOrder=9, SubCategoryId=subCategories[1].Id, Value="6/256" },
			new() { SortOrder=10, SubCategoryId=subCategories[1].Id, Value="4/128" },
		];

		subCategories[9].Options = [
			new() { SortOrder=0, SubCategoryId=subCategories[1].Id, Value="Azercell" },
			new() { SortOrder=1, SubCategoryId=subCategories[1].Id, Value="Bakcell" },
			new() { SortOrder=2, SubCategoryId=subCategories[1].Id, Value="Nar" },
			new() { SortOrder=3, SubCategoryId=subCategories[1].Id, Value="NaxTel" },
		];


		foreach (var c in subCategories)
		{
			var cat = await subCategoryReadRepository.Table.FirstOrDefaultAsync(ca => ca.Name == c.Name);

			if (cat is null)
				await subCategoryWriteRepository.AddAsync(c);
		}
		await subCategoryWriteRepository.SaveAsync();

		//-----------

		List<Location> ls = [
			new() { City="Baku", Country="Azerbaijan" },
			new() { City="Shusha", Country="Azerbaijan" },
			new() { City="Guba", Country="Azerbaijan" },
			new() { City="Gabala", Country="Azerbaijan" },
			new() { City="Sumgayit", Country="Azerbaijan" },
			new() { City="Lankaran", Country="Azerbaijan" },
			new() { City="Sheki", Country="Azerbaijan" },
			new() { City="Tovuz", Country="Azerbaijan" },
			new() { City="Fizuli", Country="Azerbaijan" },
			new() { City="Ganja", Country="Azerbaijan" },
			new() { City="Ismayilli", Country="Azerbaijan" },
			new() { City="Shemkir", Country="Azerbaijan" },
			new() { City="Mingachevir", Country="Azerbaijan" },
			new() { City="Gusar", Country="Azerbaijan" },
			new() { City="Khachmaz", Country="Azerbaijan" },
			new() { City="Barda", Country="Azerbaijan" },
			new() { City="Aghsu", Country="Azerbaijan" },
			new() { City="Balaken", Country="Azerbaijan" },
			new() { City="Masalli", Country="Azerbaijan" },
			new() { City="Lerik", Country="Azerbaijan" }
		];

		foreach (var l in ls)
		{
			var loc = await locationReadRepository.Table.FirstOrDefaultAsync(ca => ca.Country == l.Country && ca.City == l.City);

			if (loc is null)
				await locationWriteRepository.AddAsync(l);
		}
		await locationWriteRepository.SaveAsync();

	}
}
