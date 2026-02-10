using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TranscriptAnalyzer.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkflowsAndCaseWorkflows : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "workflow_definitions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Draft"),
                    current_version = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    published_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    published_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_workflow_definitions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "workflow_steps",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    workflow_definition_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    step_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    configuration = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    next_step_on_success_id = table.Column<Guid>(type: "uuid", nullable: true),
                    next_step_on_failure_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_required = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_workflow_steps", x => x.id);
                    table.ForeignKey(
                        name: "fk_workflow_steps_workflow_definitions_workflow_definition_id",
                        column: x => x.workflow_definition_id,
                        principalTable: "workflow_definitions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_workflow_steps_workflow_steps_next_step_on_failure_id",
                        column: x => x.next_step_on_failure_id,
                        principalTable: "workflow_steps",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_workflow_steps_workflow_steps_next_step_on_success_id",
                        column: x => x.next_step_on_success_id,
                        principalTable: "workflow_steps",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "case_workflows",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    client_id = table.Column<Guid>(type: "uuid", nullable: false),
                    workflow_definition_id = table.Column<Guid>(type: "uuid", nullable: false),
                    workflow_version = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "NotStarted"),
                    started_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    started_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    current_step_id = table.Column<Guid>(type: "uuid", nullable: true),
                    error_message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_case_workflows", x => x.id);
                    table.ForeignKey(
                        name: "fk_case_workflows_clients_client_id",
                        column: x => x.client_id,
                        principalTable: "clients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_case_workflows_workflow_definitions_workflow_definition_id",
                        column: x => x.workflow_definition_id,
                        principalTable: "workflow_definitions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_case_workflows_workflow_steps_current_step_id",
                        column: x => x.current_step_id,
                        principalTable: "workflow_steps",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "step_executions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    case_workflow_id = table.Column<Guid>(type: "uuid", nullable: false),
                    workflow_step_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Pending"),
                    input_data = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    output_data = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    started_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    error_message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_step_executions", x => x.id);
                    table.ForeignKey(
                        name: "fk_step_executions_case_workflows_case_workflow_id",
                        column: x => x.case_workflow_id,
                        principalTable: "case_workflows",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_step_executions_workflow_steps_workflow_step_id",
                        column: x => x.workflow_step_id,
                        principalTable: "workflow_steps",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_case_workflows_client_id",
                table: "case_workflows",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "ix_case_workflows_current_step_id",
                table: "case_workflows",
                column: "current_step_id");

            migrationBuilder.CreateIndex(
                name: "ix_case_workflows_org_client",
                table: "case_workflows",
                columns: new[] { "organization_id", "client_id" });

            migrationBuilder.CreateIndex(
                name: "ix_case_workflows_organization_id",
                table: "case_workflows",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_case_workflows_status",
                table: "case_workflows",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_case_workflows_workflow_definition_id",
                table: "case_workflows",
                column: "workflow_definition_id");

            migrationBuilder.CreateIndex(
                name: "ix_step_executions_organization_id",
                table: "step_executions",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_step_executions_workflow_step",
                table: "step_executions",
                columns: new[] { "case_workflow_id", "workflow_step_id" });

            migrationBuilder.CreateIndex(
                name: "ix_step_executions_workflow_step_id",
                table: "step_executions",
                column: "workflow_step_id");

            migrationBuilder.CreateIndex(
                name: "ix_workflow_definitions_category",
                table: "workflow_definitions",
                column: "category");

            migrationBuilder.CreateIndex(
                name: "ix_workflow_definitions_name",
                table: "workflow_definitions",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_workflow_definitions_status",
                table: "workflow_definitions",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_workflow_steps_definition_sort",
                table: "workflow_steps",
                columns: new[] { "workflow_definition_id", "sort_order" });

            migrationBuilder.CreateIndex(
                name: "ix_workflow_steps_next_step_on_failure_id",
                table: "workflow_steps",
                column: "next_step_on_failure_id");

            migrationBuilder.CreateIndex(
                name: "ix_workflow_steps_next_step_on_success_id",
                table: "workflow_steps",
                column: "next_step_on_success_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "step_executions");

            migrationBuilder.DropTable(
                name: "case_workflows");

            migrationBuilder.DropTable(
                name: "workflow_steps");

            migrationBuilder.DropTable(
                name: "workflow_definitions");
        }
    }
}
