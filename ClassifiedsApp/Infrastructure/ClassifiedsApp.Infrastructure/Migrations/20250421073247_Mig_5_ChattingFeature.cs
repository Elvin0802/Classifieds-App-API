using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassifiedsApp.Infrastructure.Migrations
{
	/// <inheritdoc />
	public partial class Mig_5_ChattingFeature : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "ChatRooms",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					BuyerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					SellerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					AdId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
					UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
					ArchivedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_ChatRooms", x => x.Id);
					table.ForeignKey(
						name: "FK_ChatRooms_Ads_AdId",
						column: x => x.AdId,
						principalTable: "Ads",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "FK_ChatRooms_AspNetUsers_BuyerId",
						column: x => x.BuyerId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "FK_ChatRooms_AspNetUsers_SellerId",
						column: x => x.SellerId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateTable(
				name: "ChatMessages",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
					SenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					ReceiverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					AdId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					IsRead = table.Column<bool>(type: "bit", nullable: false),
					ChatRoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
					CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
					UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
					ArchivedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_ChatMessages", x => x.Id);
					table.ForeignKey(
						name: "FK_ChatMessages_Ads_AdId",
						column: x => x.AdId,
						principalTable: "Ads",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "FK_ChatMessages_AspNetUsers_ReceiverId",
						column: x => x.ReceiverId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "FK_ChatMessages_AspNetUsers_SenderId",
						column: x => x.SenderId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "FK_ChatMessages_ChatRooms_ChatRoomId",
						column: x => x.ChatRoomId,
						principalTable: "ChatRooms",
						principalColumn: "Id");
				});

			migrationBuilder.CreateIndex(
				name: "IX_ChatMessages_AdId",
				table: "ChatMessages",
				column: "AdId");

			migrationBuilder.CreateIndex(
				name: "IX_ChatMessages_ChatRoomId",
				table: "ChatMessages",
				column: "ChatRoomId");

			migrationBuilder.CreateIndex(
				name: "IX_ChatMessages_ReceiverId",
				table: "ChatMessages",
				column: "ReceiverId");

			migrationBuilder.CreateIndex(
				name: "IX_ChatMessages_SenderId",
				table: "ChatMessages",
				column: "SenderId");

			migrationBuilder.CreateIndex(
				name: "IX_ChatRooms_AdId",
				table: "ChatRooms",
				column: "AdId");

			migrationBuilder.CreateIndex(
				name: "IX_ChatRooms_BuyerId",
				table: "ChatRooms",
				column: "BuyerId");

			migrationBuilder.CreateIndex(
				name: "IX_ChatRooms_SellerId",
				table: "ChatRooms",
				column: "SellerId");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "ChatMessages");

			migrationBuilder.DropTable(
				name: "ChatRooms");
		}
	}
}
