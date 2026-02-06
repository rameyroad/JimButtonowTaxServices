using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TranscriptAnalyzer.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRowLevelSecurity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create application roles for RLS
            // app_user: Regular application role with RLS enforced
            // app_admin: Admin role that bypasses RLS for maintenance operations
            migrationBuilder.Sql(@"
                -- Create app_user role if it doesn't exist
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT FROM pg_roles WHERE rolname = 'app_user') THEN
                        CREATE ROLE app_user NOLOGIN;
                    END IF;
                END
                $$;

                -- Create app_admin role if it doesn't exist (bypasses RLS)
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT FROM pg_roles WHERE rolname = 'app_admin') THEN
                        CREATE ROLE app_admin NOLOGIN BYPASSRLS;
                    END IF;
                END
                $$;

                -- Grant usage on schema
                GRANT USAGE ON SCHEMA public TO app_user;
                GRANT USAGE ON SCHEMA public TO app_admin;

                -- Grant table permissions to app_user
                GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO app_user;
                GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO app_admin;

                -- Grant sequence permissions
                GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO app_user;
                GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO app_admin;

                -- Set default privileges for future tables
                ALTER DEFAULT PRIVILEGES IN SCHEMA public
                    GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO app_user;
                ALTER DEFAULT PRIVILEGES IN SCHEMA public
                    GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO app_admin;
            ");

            // Enable RLS on users table
            // Strict policy: only rows matching tenant context are visible
            // Empty context returns no rows - use app_admin role for admin operations
            migrationBuilder.Sql(@"
                ALTER TABLE users ENABLE ROW LEVEL SECURITY;
                ALTER TABLE users FORCE ROW LEVEL SECURITY;

                CREATE POLICY users_tenant_isolation ON users
                    USING (
                        organization_id::text = current_setting('app.current_tenant_id', true)
                    )
                    WITH CHECK (
                        organization_id::text = current_setting('app.current_tenant_id', true)
                    );
            ");

            // Enable RLS on clients table
            migrationBuilder.Sql(@"
                ALTER TABLE clients ENABLE ROW LEVEL SECURITY;
                ALTER TABLE clients FORCE ROW LEVEL SECURITY;

                CREATE POLICY clients_tenant_isolation ON clients
                    USING (
                        organization_id::text = current_setting('app.current_tenant_id', true)
                    )
                    WITH CHECK (
                        organization_id::text = current_setting('app.current_tenant_id', true)
                    );
            ");

            // Enable RLS on authorizations table
            migrationBuilder.Sql(@"
                ALTER TABLE authorizations ENABLE ROW LEVEL SECURITY;
                ALTER TABLE authorizations FORCE ROW LEVEL SECURITY;

                CREATE POLICY authorizations_tenant_isolation ON authorizations
                    USING (
                        organization_id::text = current_setting('app.current_tenant_id', true)
                    )
                    WITH CHECK (
                        organization_id::text = current_setting('app.current_tenant_id', true)
                    );
            ");

            // Enable RLS on transcripts table
            migrationBuilder.Sql(@"
                ALTER TABLE transcripts ENABLE ROW LEVEL SECURITY;
                ALTER TABLE transcripts FORCE ROW LEVEL SECURITY;

                CREATE POLICY transcripts_tenant_isolation ON transcripts
                    USING (
                        organization_id::text = current_setting('app.current_tenant_id', true)
                    )
                    WITH CHECK (
                        organization_id::text = current_setting('app.current_tenant_id', true)
                    );
            ");

            // Enable RLS on notifications table
            migrationBuilder.Sql(@"
                ALTER TABLE notifications ENABLE ROW LEVEL SECURITY;
                ALTER TABLE notifications FORCE ROW LEVEL SECURITY;

                CREATE POLICY notifications_tenant_isolation ON notifications
                    USING (
                        organization_id::text = current_setting('app.current_tenant_id', true)
                    )
                    WITH CHECK (
                        organization_id::text = current_setting('app.current_tenant_id', true)
                    );
            ");

            // Enable RLS on audit_logs table
            migrationBuilder.Sql(@"
                ALTER TABLE audit_logs ENABLE ROW LEVEL SECURITY;
                ALTER TABLE audit_logs FORCE ROW LEVEL SECURITY;

                CREATE POLICY audit_logs_tenant_isolation ON audit_logs
                    USING (
                        organization_id::text = current_setting('app.current_tenant_id', true)
                    )
                    WITH CHECK (
                        organization_id::text = current_setting('app.current_tenant_id', true)
                    );
            ");

            // Enable RLS on organization_settings table
            migrationBuilder.Sql(@"
                ALTER TABLE organization_settings ENABLE ROW LEVEL SECURITY;
                ALTER TABLE organization_settings FORCE ROW LEVEL SECURITY;

                CREATE POLICY organization_settings_tenant_isolation ON organization_settings
                    USING (
                        organization_id::text = current_setting('app.current_tenant_id', true)
                    )
                    WITH CHECK (
                        organization_id::text = current_setting('app.current_tenant_id', true)
                    );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop RLS policies
            migrationBuilder.Sql(@"
                DROP POLICY IF EXISTS users_tenant_isolation ON users;
                DROP POLICY IF EXISTS clients_tenant_isolation ON clients;
                DROP POLICY IF EXISTS authorizations_tenant_isolation ON authorizations;
                DROP POLICY IF EXISTS transcripts_tenant_isolation ON transcripts;
                DROP POLICY IF EXISTS notifications_tenant_isolation ON notifications;
                DROP POLICY IF EXISTS audit_logs_tenant_isolation ON audit_logs;
                DROP POLICY IF EXISTS organization_settings_tenant_isolation ON organization_settings;
            ");

            // Disable RLS on all tables
            migrationBuilder.Sql(@"
                ALTER TABLE users DISABLE ROW LEVEL SECURITY;
                ALTER TABLE clients DISABLE ROW LEVEL SECURITY;
                ALTER TABLE authorizations DISABLE ROW LEVEL SECURITY;
                ALTER TABLE transcripts DISABLE ROW LEVEL SECURITY;
                ALTER TABLE notifications DISABLE ROW LEVEL SECURITY;
                ALTER TABLE audit_logs DISABLE ROW LEVEL SECURITY;
                ALTER TABLE organization_settings DISABLE ROW LEVEL SECURITY;
            ");

            // Revoke permissions and drop roles
            migrationBuilder.Sql(@"
                REVOKE ALL ON ALL TABLES IN SCHEMA public FROM app_user;
                REVOKE ALL ON ALL TABLES IN SCHEMA public FROM app_admin;
                REVOKE ALL ON ALL SEQUENCES IN SCHEMA public FROM app_user;
                REVOKE ALL ON ALL SEQUENCES IN SCHEMA public FROM app_admin;
                REVOKE USAGE ON SCHEMA public FROM app_user;
                REVOKE USAGE ON SCHEMA public FROM app_admin;

                DROP ROLE IF EXISTS app_user;
                DROP ROLE IF EXISTS app_admin;
            ");
        }
    }
}

