using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamWorkFlow.Infrastructure.Migrations
{
    public partial class EnhancedDashboardData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7bf9623c-54d9-45ba-84c6-52806dcee7bd",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "387a062b-b551-4018-8cfd-b98315450d8e", "AQAAAAEAACcQAAAAEEwpSSfE8u3pDNnS43zEGTFetB07TBMO+O7bUHhlEUpl7c0+LcO/RXQb9lecvFN6lQ==", "924061a2-423c-40e9-9d62-e00e484674c2" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b806eee6-2ceb-4956-9643-e2e2e82289d2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "641a62b2-bef1-4e21-9fbf-2ce4c7cf5b4c", "AQAAAAEAACcQAAAAEJ2S+Lx6yxzFz3lyXj7GTQMUc53mZp9HaXO10mUMxrycK7OlL40R4e83JceaKB/biw==", "61c87108-edea-4999-b9bc-beea59db8ffd" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "cf41999b-9cad-4b75-977d-a2fdb3d02e77",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5ebd1e37-b1db-4568-9791-5b192586419d", "AQAAAAEAACcQAAAAEI4u+ymnhM6ZPBhezA0gJkZb1/kCk8MBUQht8126rFBLI4oyGo2Pefb/udl8sq4RpA==", "da739b5a-8898-42af-8837-d2551cca83be" });

            // Add comprehensive data for chart visualization
            migrationBuilder.Sql(@"
                -- Ensure we have task-operator assignments for existing tasks
                INSERT INTO TasksOperators (TaskId, OperatorId)
                SELECT t.Id, 1 FROM Tasks t
                WHERE t.Name IN ('Database Optimization', 'API Documentation', 'Security Audit', 'Performance Testing', 'Code Review Process')
                AND NOT EXISTS (SELECT 1 FROM TasksOperators tsk_op WHERE tsk_op.TaskId = t.Id AND tsk_op.OperatorId = 1);

                INSERT INTO TasksOperators (TaskId, OperatorId)
                SELECT t.Id, 2 FROM Tasks t
                WHERE t.Name IN ('Mobile Responsiveness', 'Chart.js Integration')
                AND NOT EXISTS (SELECT 1 FROM TasksOperators tsk_op WHERE tsk_op.TaskId = t.Id AND tsk_op.OperatorId = 2);

                -- Add more comprehensive time tracking entries for better analytics
                DECLARE @TaskIds TABLE (Id INT, Name NVARCHAR(100));
                INSERT INTO @TaskIds (Id, Name)
                SELECT Id, Name FROM Tasks
                WHERE Name IN ('Database Optimization', 'API Documentation', 'Security Audit', 'Performance Testing', 'Code Review Process', 'Mobile Responsiveness', 'Chart.js Integration');

                -- Add more detailed time entries for Security Audit
                INSERT INTO TaskTimeEntries (TaskId, OperatorId, StartTime, EndTime, DurationMinutes, Notes, SessionType, CreatedAt)
                SELECT t.Id, 1, '2025-08-25 09:00:00', '2025-08-25 17:00:00', 480, 'Authentication system review', 'Review', '2025-08-25 17:00:00'
                FROM @TaskIds t WHERE t.Name = 'Security Audit'
                AND NOT EXISTS (SELECT 1 FROM TaskTimeEntries tte WHERE tte.TaskId = t.Id AND tte.StartTime = '2025-08-25 09:00:00')
                UNION ALL
                SELECT t.Id, 1, '2025-08-26 08:30:00', '2025-08-26 16:30:00', 480, 'Vulnerability assessment', 'Testing', '2025-08-26 16:30:00'
                FROM @TaskIds t WHERE t.Name = 'Security Audit'
                AND NOT EXISTS (SELECT 1 FROM TaskTimeEntries tte WHERE tte.TaskId = t.Id AND tte.StartTime = '2025-08-26 08:30:00')
                UNION ALL
                SELECT t.Id, 1, '2025-08-27 10:00:00', '2025-08-27 18:00:00', 480, 'Security improvements implementation', 'Development', '2025-08-27 18:00:00'
                FROM @TaskIds t WHERE t.Name = 'Security Audit'
                AND NOT EXISTS (SELECT 1 FROM TaskTimeEntries tte WHERE tte.TaskId = t.Id AND tte.StartTime = '2025-08-27 10:00:00')
                UNION ALL
                SELECT t.Id, 1, '2025-08-28 09:00:00', '2025-08-28 13:30:00', 270, 'Load testing setup', 'Testing', '2025-08-28 13:30:00'
                FROM @TaskIds t WHERE t.Name = 'Performance Testing'
                AND NOT EXISTS (SELECT 1 FROM TaskTimeEntries tte WHERE tte.TaskId = t.Id AND tte.StartTime = '2025-08-28 09:00:00')
                UNION ALL
                SELECT t.Id, 1, '2025-08-29 10:00:00', '2025-08-29 16:00:00', 360, 'Performance analysis', 'Testing', '2025-08-29 16:00:00'
                FROM @TaskIds t WHERE t.Name = 'Performance Testing'
                AND NOT EXISTS (SELECT 1 FROM TaskTimeEntries tte WHERE tte.TaskId = t.Id AND tte.StartTime = '2025-08-29 10:00:00')
                UNION ALL
                SELECT t.Id, 1, '2025-08-30 09:00:00', '2025-08-30 13:00:00', 240, 'Report generation', 'Documentation', '2025-08-30 13:00:00'
                FROM @TaskIds t WHERE t.Name = 'Performance Testing'
                AND NOT EXISTS (SELECT 1 FROM TaskTimeEntries tte WHERE tte.TaskId = t.Id AND tte.StartTime = '2025-08-30 09:00:00');

                -- Add more time entries for Code Review Process
                INSERT INTO TaskTimeEntries (TaskId, OperatorId, StartTime, EndTime, DurationMinutes, Notes, SessionType, CreatedAt)
                SELECT t.Id, 1, '2025-09-01 08:00:00', '2025-09-01 12:00:00', 240, 'Research best practices', 'Research', '2025-09-01 12:00:00'
                FROM @TaskIds t WHERE t.Name = 'Code Review Process'
                AND NOT EXISTS (SELECT 1 FROM TaskTimeEntries tte WHERE tte.TaskId = t.Id AND tte.StartTime = '2025-09-01 08:00:00')
                UNION ALL
                SELECT t.Id, 1, '2025-09-02 09:00:00', '2025-09-02 17:00:00', 480, 'Guidelines documentation', 'Documentation', '2025-09-02 17:00:00'
                FROM @TaskIds t WHERE t.Name = 'Code Review Process'
                AND NOT EXISTS (SELECT 1 FROM TaskTimeEntries tte WHERE tte.TaskId = t.Id AND tte.StartTime = '2025-09-02 09:00:00')
                UNION ALL
                SELECT t.Id, 1, '2025-09-03 10:00:00', '2025-09-03 16:00:00', 360, 'Team training and review', 'Training', '2025-09-03 16:00:00'
                FROM @TaskIds t WHERE t.Name = 'Code Review Process'
                AND NOT EXISTS (SELECT 1 FROM TaskTimeEntries tte WHERE tte.TaskId = t.Id AND tte.StartTime = '2025-09-03 10:00:00')
                UNION ALL
                SELECT t.Id, 2, '2025-09-07 10:00:00', '2025-09-07 18:00:00', 480, 'Data visualization implementation', 'Development', '2025-09-07 18:00:00'
                FROM @TaskIds t WHERE t.Name = 'Chart.js Integration'
                AND NOT EXISTS (SELECT 1 FROM TaskTimeEntries tte WHERE tte.TaskId = t.Id AND tte.StartTime = '2025-09-07 10:00:00')
                UNION ALL
                SELECT t.Id, 2, '2025-09-08 09:00:00', '2025-09-08 15:00:00', 360, 'Chart configuration and testing', 'Development', '2025-09-08 15:00:00'
                FROM @TaskIds t WHERE t.Name = 'Chart.js Integration'
                AND NOT EXISTS (SELECT 1 FROM TaskTimeEntries tte WHERE tte.TaskId = t.Id AND tte.StartTime = '2025-09-08 09:00:00');

                -- Add additional tasks for more comprehensive data
                INSERT INTO Tasks (Name, Description, EstimatedTime, ActualTime, StartDate, EndDate, DeadLine, CreatorId, CompletedById, TaskStatusId, PriorityId, ProjectId, IsInSprint, Comment)
                SELECT 'UI Testing', 'Comprehensive UI testing across all browsers', 18.0, 20.0, '2025-08-10', '2025-08-13', '2025-08-15', 'cf41999b-9cad-4b75-977d-a2fdb3d02e77', '7bf9623c-54d9-45ba-84c6-52806dcee7bd', 3, 2, 1, 0, 'Testing completed with minor issues'
                WHERE NOT EXISTS (SELECT 1 FROM Tasks WHERE Name = 'UI Testing')
                UNION ALL
                SELECT 'Data Migration', 'Migrate legacy data to new system', 25.0, 23.0, '2025-08-05', '2025-08-08', '2025-08-10', 'cf41999b-9cad-4b75-977d-a2fdb3d02e77', 'cf41999b-9cad-4b75-977d-a2fdb3d02e77', 3, 3, 2, 0, 'Migration successful'
                WHERE NOT EXISTS (SELECT 1 FROM Tasks WHERE Name = 'Data Migration')
                UNION ALL
                SELECT 'Email Integration', 'Integrate email notification system', 30.0, NULL, '2025-09-10', NULL, '2025-09-18', 'cf41999b-9cad-4b75-977d-a2fdb3d02e77', NULL, 2, 1, 1, 1, 'Email templates in progress'
                WHERE NOT EXISTS (SELECT 1 FROM Tasks WHERE Name = 'Email Integration')
                UNION ALL
                SELECT 'Report Generation', 'Automated report generation feature', 22.0, NULL, '2025-09-12', NULL, '2025-09-20', 'cf41999b-9cad-4b75-977d-a2fdb3d02e77', NULL, 2, 2, 2, 1, 'Working on PDF generation'
                WHERE NOT EXISTS (SELECT 1 FROM Tasks WHERE Name = 'Report Generation');

                -- Add time tracking for the new tasks
                INSERT INTO TaskTimeEntries (TaskId, OperatorId, StartTime, EndTime, DurationMinutes, Notes, SessionType, CreatedAt)
                SELECT t.Id, 2, '2025-08-10 09:00:00', '2025-08-10 17:00:00', 480, 'Cross-browser testing setup', 'Testing', '2025-08-10 17:00:00'
                FROM Tasks t WHERE t.Name = 'UI Testing'
                AND NOT EXISTS (SELECT 1 FROM TaskTimeEntries tte WHERE tte.TaskId = t.Id)
                UNION ALL
                SELECT t.Id, 2, '2025-08-11 08:30:00', '2025-08-11 16:30:00', 480, 'Automated test scripts', 'Development', '2025-08-11 16:30:00'
                FROM Tasks t WHERE t.Name = 'UI Testing'
                AND NOT EXISTS (SELECT 1 FROM TaskTimeEntries tte WHERE tte.TaskId = t.Id AND tte.StartTime = '2025-08-11 08:30:00')
                UNION ALL
                SELECT t.Id, 2, '2025-08-12 10:00:00', '2025-08-12 18:00:00', 480, 'Manual testing and bug fixes', 'Testing', '2025-08-12 18:00:00'
                FROM Tasks t WHERE t.Name = 'UI Testing'
                AND NOT EXISTS (SELECT 1 FROM TaskTimeEntries tte WHERE tte.TaskId = t.Id AND tte.StartTime = '2025-08-12 10:00:00')
                UNION ALL
                SELECT t.Id, 1, '2025-08-05 08:00:00', '2025-08-05 16:00:00', 480, 'Data analysis and mapping', 'Analysis', '2025-08-05 16:00:00'
                FROM Tasks t WHERE t.Name = 'Data Migration'
                AND NOT EXISTS (SELECT 1 FROM TaskTimeEntries tte WHERE tte.TaskId = t.Id)
                UNION ALL
                SELECT t.Id, 1, '2025-08-06 09:00:00', '2025-08-06 17:00:00', 480, 'Migration script development', 'Development', '2025-08-06 17:00:00'
                FROM Tasks t WHERE t.Name = 'Data Migration'
                AND NOT EXISTS (SELECT 1 FROM TaskTimeEntries tte WHERE tte.TaskId = t.Id AND tte.StartTime = '2025-08-06 09:00:00')
                UNION ALL
                SELECT t.Id, 1, '2025-08-07 10:00:00', '2025-08-07 15:00:00', 300, 'Data validation and testing', 'Testing', '2025-08-07 15:00:00'
                FROM Tasks t WHERE t.Name = 'Data Migration'
                AND NOT EXISTS (SELECT 1 FROM TaskTimeEntries tte WHERE tte.TaskId = t.Id AND tte.StartTime = '2025-08-07 10:00:00')
                UNION ALL
                SELECT t.Id, 2, '2025-09-10 09:00:00', '2025-09-10 17:00:00', 480, 'Email service integration', 'Development', '2025-09-10 17:00:00'
                FROM Tasks t WHERE t.Name = 'Email Integration'
                AND NOT EXISTS (SELECT 1 FROM TaskTimeEntries tte WHERE tte.TaskId = t.Id)
                UNION ALL
                SELECT t.Id, 1, '2025-09-12 08:30:00', '2025-09-12 16:30:00', 480, 'Report template design', 'Development', '2025-09-12 16:30:00'
                FROM Tasks t WHERE t.Name = 'Report Generation'
                AND NOT EXISTS (SELECT 1 FROM TaskTimeEntries tte WHERE tte.TaskId = t.Id);

                -- Add task-operator assignments for new tasks
                INSERT INTO TasksOperators (TaskId, OperatorId)
                SELECT t.Id, 2 FROM Tasks t WHERE t.Name IN ('UI Testing', 'Email Integration')
                AND NOT EXISTS (SELECT 1 FROM TasksOperators tsk_op WHERE tsk_op.TaskId = t.Id AND tsk_op.OperatorId = 2);

                INSERT INTO TasksOperators (TaskId, OperatorId)
                SELECT t.Id, 1 FROM Tasks t WHERE t.Name IN ('Data Migration', 'Report Generation')
                AND NOT EXISTS (SELECT 1 FROM TasksOperators tsk_op WHERE tsk_op.TaskId = t.Id AND tsk_op.OperatorId = 1);
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove the enhanced data
            migrationBuilder.Sql(@"
                -- Remove task-operator assignments
                DELETE FROM TasksOperators
                WHERE TaskId IN (SELECT Id FROM Tasks WHERE Name IN ('UI Testing', 'Data Migration', 'Email Integration', 'Report Generation'));

                -- Remove time entries for new tasks
                DELETE FROM TaskTimeEntries
                WHERE TaskId IN (SELECT Id FROM Tasks WHERE Name IN ('UI Testing', 'Data Migration', 'Email Integration', 'Report Generation'));

                -- Remove new tasks
                DELETE FROM Tasks
                WHERE Name IN ('UI Testing', 'Data Migration', 'Email Integration', 'Report Generation');
            ");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7bf9623c-54d9-45ba-84c6-52806dcee7bd",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c6c43743-a77b-454d-95fd-fb5d092b1383", "AQAAAAEAACcQAAAAEEyT150B0qFgVHfJz3POdQx+U4OMorxfc7fgpPwj1axnpte6FVECovBB+z+hkRpqGA==", "cf173120-fb95-4ae5-a3f1-70f5c223bb87" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b806eee6-2ceb-4956-9643-e2e2e82289d2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "6473961e-64b2-4948-88c9-4f4160b5f66e", "AQAAAAEAACcQAAAAEPwKMH+B2ooDYrLV/duqz793jklozzTl41zMvHiJGWuYhhs81xgIpxfFPWYnGjFlCg==", "6cacd335-6b2c-4e91-8172-660e7e8b3008" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "cf41999b-9cad-4b75-977d-a2fdb3d02e77",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7e33335c-9d7f-45ea-9716-d8f25b868d5e", "AQAAAAEAACcQAAAAEJd31KmVpg7bO25eqEcrZoDHL58MQ9PeapB0fKVP44bltBWx/Enw+1KJ+Jmfm2lSeA==", "383e2e0a-b81b-4628-91af-5ed197cbca3e" });
        }
    }
}
