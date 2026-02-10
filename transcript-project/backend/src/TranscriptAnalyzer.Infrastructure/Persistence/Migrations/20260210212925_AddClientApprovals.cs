using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TranscriptAnalyzer.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddClientApprovals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "client_approvals",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    case_workflow_id = table.Column<Guid>(type: "uuid", nullable: false),
                    step_execution_id = table.Column<Guid>(type: "uuid", nullable: false),
                    client_id = table.Column<Guid>(type: "uuid", nullable: false),
                    token = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    token_expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    responded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    response_notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    reminder_sent_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_client_approvals", x => x.id);
                    table.ForeignKey(
                        name: "fk_client_approvals_case_workflows_case_workflow_id",
                        column: x => x.case_workflow_id,
                        principalTable: "case_workflows",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_client_approvals_clients_client_id",
                        column: x => x.client_id,
                        principalTable: "clients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_client_approvals_step_executions_step_execution_id",
                        column: x => x.step_execution_id,
                        principalTable: "step_executions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_client_approvals_case_workflow_id",
                table: "client_approvals",
                column: "case_workflow_id");

            migrationBuilder.CreateIndex(
                name: "ix_client_approvals_client_id",
                table: "client_approvals",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "ix_client_approvals_organization_id",
                table: "client_approvals",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_client_approvals_step_execution_id",
                table: "client_approvals",
                column: "step_execution_id");

            migrationBuilder.CreateIndex(
                name: "ix_client_approvals_token",
                table: "client_approvals",
                column: "token",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "client_approvals");
        }
    }
}
