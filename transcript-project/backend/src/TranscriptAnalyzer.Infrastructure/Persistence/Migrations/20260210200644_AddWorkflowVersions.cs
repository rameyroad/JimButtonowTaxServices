using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TranscriptAnalyzer.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkflowVersions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "workflow_version_id",
                table: "case_workflows",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "workflow_versions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    workflow_definition_id = table.Column<Guid>(type: "uuid", nullable: false),
                    version_number = table.Column<int>(type: "integer", nullable: false),
                    published_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    published_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    snapshot_data = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_workflow_versions", x => x.id);
                    table.ForeignKey(
                        name: "fk_workflow_versions_workflow_definitions_workflow_definition_",
                        column: x => x.workflow_definition_id,
                        principalTable: "workflow_definitions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_case_workflows_workflow_version_id",
                table: "case_workflows",
                column: "workflow_version_id");

            migrationBuilder.CreateIndex(
                name: "ix_workflow_versions_definition_active",
                table: "workflow_versions",
                columns: new[] { "workflow_definition_id", "is_active" });

            migrationBuilder.CreateIndex(
                name: "ix_workflow_versions_definition_version",
                table: "workflow_versions",
                columns: new[] { "workflow_definition_id", "version_number" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_case_workflows_workflow_versions_workflow_version_id",
                table: "case_workflows",
                column: "workflow_version_id",
                principalTable: "workflow_versions",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_case_workflows_workflow_versions_workflow_version_id",
                table: "case_workflows");

            migrationBuilder.DropTable(
                name: "workflow_versions");

            migrationBuilder.DropIndex(
                name: "ix_case_workflows_workflow_version_id",
                table: "case_workflows");

            migrationBuilder.DropColumn(
                name: "workflow_version_id",
                table: "case_workflows");
        }
    }
}
