using Microsoft.EntityFrameworkCore.Migrations;

namespace Jwt.Identity.Data.Migrations
{
    public partial class f4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserLoginPolicyOptions_ApplicationUserPolicies_Id",
                table: "UserLoginPolicyOptions");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_ApplicationUserPolicies_PolicyId",
                table: "ApplicationUserPolicies");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserPolicies_PolicyId",
                table: "ApplicationUserPolicies",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserPolicies_UserId",
                table: "ApplicationUserPolicies",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserPolicies_UserLoginPolicyOptions_PolicyId",
                table: "ApplicationUserPolicies",
                column: "PolicyId",
                principalTable: "UserLoginPolicyOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserPolicies_UserLoginPolicyOptions_PolicyId",
                table: "ApplicationUserPolicies");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationUserPolicies_PolicyId",
                table: "ApplicationUserPolicies");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationUserPolicies_UserId",
                table: "ApplicationUserPolicies");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_ApplicationUserPolicies_PolicyId",
                table: "ApplicationUserPolicies",
                column: "PolicyId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserLoginPolicyOptions_ApplicationUserPolicies_Id",
                table: "UserLoginPolicyOptions",
                column: "Id",
                principalTable: "ApplicationUserPolicies",
                principalColumn: "PolicyId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
