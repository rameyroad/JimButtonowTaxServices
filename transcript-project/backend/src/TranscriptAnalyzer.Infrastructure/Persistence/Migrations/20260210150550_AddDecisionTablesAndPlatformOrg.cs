using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TranscriptAnalyzer.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDecisionTablesAndPlatformOrg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_platform_organization",
                table: "organizations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "decision_tables",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Draft"),
                    published_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    published_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    version = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_decision_tables", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "decision_rules",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    decision_table_id = table.Column<Guid>(type: "uuid", nullable: false),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_decision_rules", x => x.id);
                    table.ForeignKey(
                        name: "fk_decision_rules_decision_tables_decision_table_id",
                        column: x => x.decision_table_id,
                        principalTable: "decision_tables",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "decision_table_columns",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    decision_table_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    data_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    is_input = table.Column<bool>(type: "boolean", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_decision_table_columns", x => x.id);
                    table.ForeignKey(
                        name: "fk_decision_table_columns_decision_tables_decision_table_id",
                        column: x => x.decision_table_id,
                        principalTable: "decision_tables",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rule_conditions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    decision_rule_id = table.Column<Guid>(type: "uuid", nullable: false),
                    column_key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    @operator = table.Column<string>(name: "operator", type: "character varying(30)", maxLength: 30, nullable: false),
                    value = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    value2 = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rule_conditions", x => x.id);
                    table.ForeignKey(
                        name: "fk_rule_conditions_decision_rules_decision_rule_id",
                        column: x => x.decision_rule_id,
                        principalTable: "decision_rules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rule_outputs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    decision_rule_id = table.Column<Guid>(type: "uuid", nullable: false),
                    column_key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    value = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rule_outputs", x => x.id);
                    table.ForeignKey(
                        name: "fk_rule_outputs_decision_rules_decision_rule_id",
                        column: x => x.decision_rule_id,
                        principalTable: "decision_rules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_decision_rules_table_priority",
                table: "decision_rules",
                columns: new[] { "decision_table_id", "priority" });

            migrationBuilder.CreateIndex(
                name: "ix_decision_table_columns_table_key",
                table: "decision_table_columns",
                columns: new[] { "decision_table_id", "key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_decision_tables_name",
                table: "decision_tables",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_decision_tables_status",
                table: "decision_tables",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_rule_conditions_decision_rule_id",
                table: "rule_conditions",
                column: "decision_rule_id");

            migrationBuilder.CreateIndex(
                name: "ix_rule_outputs_decision_rule_id",
                table: "rule_outputs",
                column: "decision_rule_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "decision_table_columns");

            migrationBuilder.DropTable(
                name: "rule_conditions");

            migrationBuilder.DropTable(
                name: "rule_outputs");

            migrationBuilder.DropTable(
                name: "decision_rules");

            migrationBuilder.DropTable(
                name: "decision_tables");

            migrationBuilder.DropColumn(
                name: "is_platform_organization",
                table: "organizations");
        }
    }
}
