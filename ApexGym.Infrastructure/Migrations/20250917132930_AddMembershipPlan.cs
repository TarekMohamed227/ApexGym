using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ApexGym.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMembershipPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Create the new table first
            migrationBuilder.CreateTable(
                name: "MembershipPlan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DurationDays = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MembershipPlan", x => x.Id);
                });

            // 2. Seed the plan data
            migrationBuilder.InsertData(
                table: "MembershipPlan",
                columns: new[] { "Id", "Description", "DurationDays", "Name", "Price" },
                values: new object[,]
                {
            { 1, "Gym access during business hours (8 AM - 8 PM)", 30, "Basic", 49.99m },
            { 2, "24/7 gym access + locker storage", 30, "Premium", 99.99m },
            { 3, "24/7 access + locker + 2 personal training sessions per month", 30, "VIP", 199.99m }
                });

            // 3. Add the column WITHOUT a default value and make it nullable temporarily.
            // This avoids putting 0 in every existing row.
            migrationBuilder.AddColumn<int>(
                name: "MembershipPlanId",
                table: "Members",
                type: "int",
                nullable: true); // <- Change this to nullable first

            // 4. UPDATE EXISTING DATA: Set all existing members to have a valid Plan Id (e.g., 1 for Basic)
            migrationBuilder.Sql("UPDATE Members SET MembershipPlanId = 1 WHERE MembershipPlanId IS NULL");

            // 5. Now alter the column to be non-nullable, as your business logic requires.
            migrationBuilder.AlterColumn<int>(
                name: "MembershipPlanId",
                table: "Members",
                type: "int",
                nullable: false,
                defaultValue: 1, // You can set a sensible default now that all rows have a value
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            // 6. Now it's safe to create the index and foreign key constraint.
            migrationBuilder.CreateIndex(
                name: "IX_Members_MembershipPlanId",
                table: "Members",
                column: "MembershipPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Members_MembershipPlan_MembershipPlanId",
                table: "Members",
                column: "MembershipPlanId",
                principalTable: "MembershipPlan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Members_MembershipPlan_MembershipPlanId",
                table: "Members");

            migrationBuilder.DropTable(
                name: "MembershipPlan");

            migrationBuilder.DropIndex(
                name: "IX_Members_MembershipPlanId",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "MembershipPlanId",
                table: "Members");
        }
    }
}
