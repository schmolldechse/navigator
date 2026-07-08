using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Navigator.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStatisticsRefreshQueue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE SCHEMA IF NOT EXISTS statistics;");

            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1
                        FROM pg_type
                        INNER JOIN pg_namespace ON pg_namespace.oid = pg_type.typnamespace
                        WHERE pg_namespace.nspname = 'statistics'
                          AND pg_type.typname = 'statistics_refresh_queue_status'
                    ) THEN
                        CREATE TYPE statistics.statistics_refresh_queue_status AS ENUM (
                            'FAILED',
                            'PENDING',
                            'RUNNING',
                            'SUCCESS'
                        );
                    END IF;
                END
                $$;
            ");

            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1
                        FROM pg_type
                        INNER JOIN pg_namespace ON pg_namespace.oid = pg_type.typnamespace
                        WHERE pg_namespace.nspname = 'statistics'
                          AND pg_type.typname = 'statistics_refresh_queue_source'
                    ) THEN
                        CREATE TYPE statistics.statistics_refresh_queue_source AS ENUM (
                            'JOURNEY_IMPORT',
                            'MANUAL'
                        );
                    END IF;
                END
                $$;
            ");

            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS statistics.statistics_refresh_queue (
                    window_start timestamp with time zone NOT NULL,
                    window_end timestamp with time zone NOT NULL,
                    status statistics.statistics_refresh_queue_status NOT NULL,
                    source statistics.statistics_refresh_queue_source NOT NULL,
                    mark_count integer NOT NULL,
                    attempt integer NOT NULL,
                    first_marked_at timestamp with time zone NOT NULL,
                    last_marked_at timestamp with time zone NOT NULL,
                    started_at timestamp with time zone NULL,
                    finished_at timestamp with time zone NULL,
                    error_kind character varying(128) NULL,
                    CONSTRAINT pk_statistics_refresh_queue PRIMARY KEY (window_start, window_end),
                    CONSTRAINT ck_statistics_refresh_queue_window CHECK (window_end > window_start)
                );
            ");

            migrationBuilder.Sql(@"
                CREATE INDEX IF NOT EXISTS ix_statistics_refresh_queue_status_window
                    ON statistics.statistics_refresh_queue (status, window_start, window_end);
            ");

            migrationBuilder.Sql(@"
                CREATE INDEX IF NOT EXISTS ix_statistics_refresh_queue_last_marked_at
                    ON statistics.statistics_refresh_queue (last_marked_at);
            ");

            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS statistics.ris_id_reactivation_holds (
                    ris_id character varying(73) NOT NULL,
                    reactivated_at timestamp with time zone NOT NULL,
                    protect_until timestamp with time zone NOT NULL,
                    activation_count integer NOT NULL,
                    last_seen_at_reactivation timestamp with time zone NULL,
                    last_inserted_at_reactivation timestamp with time zone NULL,
                    CONSTRAINT pk_ris_id_reactivation_holds PRIMARY KEY (ris_id),
                    CONSTRAINT ck_ris_id_reactivation_holds_protect_until CHECK (protect_until > reactivated_at)
                );
            ");

            migrationBuilder.Sql(@"
                CREATE INDEX IF NOT EXISTS ix_ris_id_reactivation_holds_protect_until
                    ON statistics.ris_id_reactivation_holds (protect_until);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS statistics.ris_id_reactivation_holds;");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS statistics.statistics_refresh_queue;");
            migrationBuilder.Sql(@"DROP TYPE IF EXISTS statistics.statistics_refresh_queue_source;");
            migrationBuilder.Sql(@"DROP TYPE IF EXISTS statistics.statistics_refresh_queue_status;");
        }
    }
}
