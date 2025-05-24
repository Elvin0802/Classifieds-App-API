using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassifiedsApp.Infrastructure.Migrations
{
	/// <inheritdoc />
	public partial class Mig_7_BlcakList_Feature : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<string>(
				name: "BlacklistReason",
				table: "AspNetUsers",
				type: "nvarchar(max)",
				nullable: true);

			migrationBuilder.AddColumn<DateTimeOffset>(
				name: "BlacklistedAt",
				table: "AspNetUsers",
				type: "datetimeoffset",
				nullable: true);

			migrationBuilder.AddColumn<bool>(
				name: "IsBlacklisted",
				table: "AspNetUsers",
				type: "bit",
				nullable: false,
				defaultValue: false);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "BlacklistReason",
				table: "AspNetUsers");

			migrationBuilder.DropColumn(
				name: "BlacklistedAt",
				table: "AspNetUsers");

			migrationBuilder.DropColumn(
				name: "IsBlacklisted",
				table: "AspNetUsers");
		}
	}
}
