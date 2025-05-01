using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassifiedsApp.Infrastructure.Migrations
{
	/// <inheritdoc />
	public partial class Mig_7_ChatEntity_Fix : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_ChatMessages_ChatRooms_ChatRoomId",
				table: "ChatMessages");

			migrationBuilder.AlterColumn<Guid>(
				name: "ChatRoomId",
				table: "ChatMessages",
				type: "uniqueidentifier",
				nullable: false,
				defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
				oldClrType: typeof(Guid),
				oldType: "uniqueidentifier",
				oldNullable: true);

			migrationBuilder.AddForeignKey(
				name: "FK_ChatMessages_ChatRooms_ChatRoomId",
				table: "ChatMessages",
				column: "ChatRoomId",
				principalTable: "ChatRooms",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_ChatMessages_ChatRooms_ChatRoomId",
				table: "ChatMessages");

			migrationBuilder.AlterColumn<Guid>(
				name: "ChatRoomId",
				table: "ChatMessages",
				type: "uniqueidentifier",
				nullable: true,
				oldClrType: typeof(Guid),
				oldType: "uniqueidentifier");

			migrationBuilder.AddForeignKey(
				name: "FK_ChatMessages_ChatRooms_ChatRoomId",
				table: "ChatMessages",
				column: "ChatRoomId",
				principalTable: "ChatRooms",
				principalColumn: "Id");
		}
	}
}
