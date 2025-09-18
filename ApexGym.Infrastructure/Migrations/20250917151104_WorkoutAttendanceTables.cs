using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ApexGym.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class WorkoutAttendanceTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Members_MembershipPlan_MembershipPlanId",
                table: "Members");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MembershipPlan",
                table: "MembershipPlan");

            migrationBuilder.RenameTable(
                name: "MembershipPlan",
                newName: "MembershipPlans");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MembershipPlans",
                table: "MembershipPlans",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Trainers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Specialization = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    YearsOfExperience = table.Column<int>(type: "int", nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trainers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutClasses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaxCapacity = table.Column<int>(type: "int", nullable: false),
                    TrainerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutClasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkoutClasses_Trainers_TrainerId",
                        column: x => x.TrainerId,
                        principalTable: "Trainers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Attendances",
                columns: table => new
                {
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    WorkoutClassId = table.Column<int>(type: "int", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Attended = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendances", x => new { x.MemberId, x.WorkoutClassId });
                    table.ForeignKey(
                        name: "FK_Attendances_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Attendances_WorkoutClasses_WorkoutClassId",
                        column: x => x.WorkoutClassId,
                        principalTable: "WorkoutClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Trainers",
                columns: new[] { "Id", "Bio", "FirstName", "IsActive", "LastName", "Specialization", "YearsOfExperience" },
                values: new object[,]
                {
                    { 1, "Expert in Vinyasa and Hatha yoga.", "John", true, "Doe", "Yoga", 5 },
                    { 2, "National level weightlifting coach.", "Sarah", true, "Smith", "Weightlifting", 8 },
                    { 3, "Passionate about high-intensity interval training.", "Mike", true, "Johnson", "HIIT", 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_WorkoutClassId",
                table: "Attendances",
                column: "WorkoutClassId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutClasses_TrainerId",
                table: "WorkoutClasses",
                column: "TrainerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Members_MembershipPlans_MembershipPlanId",
                table: "Members",
                column: "MembershipPlanId",
                principalTable: "MembershipPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Members_MembershipPlans_MembershipPlanId",
                table: "Members");

            migrationBuilder.DropTable(
                name: "Attendances");

            migrationBuilder.DropTable(
                name: "WorkoutClasses");

            migrationBuilder.DropTable(
                name: "Trainers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MembershipPlans",
                table: "MembershipPlans");

            migrationBuilder.RenameTable(
                name: "MembershipPlans",
                newName: "MembershipPlan");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MembershipPlan",
                table: "MembershipPlan",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Members_MembershipPlan_MembershipPlanId",
                table: "Members",
                column: "MembershipPlanId",
                principalTable: "MembershipPlan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
