using Microsoft.EntityFrameworkCore.Migrations;

namespace Jwt.Identity.Data.Migrations
{
    public partial class f3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserPolicies_AspNetUsers_ApliApplicationUserId",
                table: "ApplicationUserPolicies");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_UserLoginPolicyOptions_UserLoginPolicyOptionId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLoginPolicyOptions_ApplicationUserPolicies_ApplicationUserPolicyUserId_ApplicationUserPolicyPolicyId",
                table: "UserLoginPolicyOptions");

            migrationBuilder.DropIndex(
                name: "IX_UserLoginPolicyOptions_ApplicationUserPolicyUserId_ApplicationUserPolicyPolicyId",
                table: "UserLoginPolicyOptions");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_UserLoginPolicyOptionId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationUserPolicies_ApliApplicationUserId",
                table: "ApplicationUserPolicies");

            migrationBuilder.DropColumn(
                name: "ApplicationUserPolicyPolicyId",
                table: "UserLoginPolicyOptions");

            migrationBuilder.DropColumn(
                name: "ApplicationUserPolicyUserId",
                table: "UserLoginPolicyOptions");

            migrationBuilder.DropColumn(
                name: "UserLoginPolicyOptionId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ApliApplicationUserId",
                table: "ApplicationUserPolicies");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserPolicyPolicyId",
                table: "UserLoginPolicyOptions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserPolicyUserId",
                table: "UserLoginPolicyOptions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserLoginPolicyOptionId",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApliApplicationUserId",
                table: "ApplicationUserPolicies",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserLoginPolicyOptions_ApplicationUserPolicyUserId_ApplicationUserPolicyPolicyId",
                table: "UserLoginPolicyOptions",
                columns: new[] { "ApplicationUserPolicyUserId", "ApplicationUserPolicyPolicyId" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserLoginPolicyOptionId",
                table: "AspNetUsers",
                column: "UserLoginPolicyOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserPolicies_ApliApplicationUserId",
                table: "ApplicationUserPolicies",
                column: "ApliApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserPolicies_AspNetUsers_ApliApplicationUserId",
                table: "ApplicationUserPolicies",
                column: "ApliApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_UserLoginPolicyOptions_UserLoginPolicyOptionId",
                table: "AspNetUsers",
                column: "UserLoginPolicyOptionId",
                principalTable: "UserLoginPolicyOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLoginPolicyOptions_ApplicationUserPolicies_ApplicationUserPolicyUserId_ApplicationUserPolicyPolicyId",
                table: "UserLoginPolicyOptions",
                columns: new[] { "ApplicationUserPolicyUserId", "ApplicationUserPolicyPolicyId" },
                principalTable: "ApplicationUserPolicies",
                principalColumns: new[] { "UserId", "PolicyId" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
