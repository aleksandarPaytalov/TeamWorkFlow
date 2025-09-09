using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamWorkFlow.Infrastructure.Migrations
{
    public partial class AddDashboardSampleData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add sample tasks for dashboard analytics using SQL commands
            // This is the secure, standard way to seed data in migrations

            migrationBuilder.Sql(@"
                INSERT INTO Tasks (Name, Description, EstimatedTime, ActualTime, StartDate, EndDate, DeadLine, CreatorId, CompletedById, TaskStatusId, PriorityId, ProjectId, IsInSprint, Comment)
                VALUES
                ('Database Optimization', 'Optimize database queries for better performance', 40.0, 38.5, '2025-08-15', '2025-08-18', '2025-08-20', 'cf41999b-9cad-4b75-977d-a2fdb3d02e77', 'cf41999b-9cad-4b75-977d-a2fdb3d02e77', 3, 2, 1, 0, 'Completed ahead of schedule'),
                ('API Documentation', 'Create comprehensive API documentation', 24.0, 26.0, '2025-08-20', '2025-08-23', '2025-08-25', 'cf41999b-9cad-4b75-977d-a2fdb3d02e77', '7bf9623c-54d9-45ba-84c6-52806dcee7bd', 3, 1, 2, 0, 'Documentation completed'),
                ('Security Audit', 'Perform security audit on authentication system', 32.0, 35.0, '2025-08-25', '2025-08-28', '2025-08-30', 'cf41999b-9cad-4b75-977d-a2fdb3d02e77', 'cf41999b-9cad-4b75-977d-a2fdb3d02e77', 3, 3, 1, 0, 'Security improvements implemented'),
                ('Performance Testing', 'Load testing for dashboard components', 16.0, 14.5, '2025-08-28', '2025-08-30', '2025-09-01', 'cf41999b-9cad-4b75-977d-a2fdb3d02e77', '7bf9623c-54d9-45ba-84c6-52806dcee7bd', 3, 2, 2, 0, 'Performance targets met'),
                ('Code Review Process', 'Establish code review guidelines', 20.0, 22.0, '2025-09-01', '2025-09-03', '2025-09-05', 'cf41999b-9cad-4b75-977d-a2fdb3d02e77', 'cf41999b-9cad-4b75-977d-a2fdb3d02e77', 3, 1, 1, 0, 'Guidelines documented'),
                ('Mobile Responsiveness', 'Improve mobile responsiveness across all pages', 28.0, NULL, '2025-09-05', NULL, '2025-09-12', 'cf41999b-9cad-4b75-977d-a2fdb3d02e77', NULL, 2, 2, 2, 1, 'In progress - 60% complete'),
                ('Chart.js Integration', 'Integrate advanced charting capabilities', 36.0, NULL, '2025-09-06', NULL, '2025-09-15', 'cf41999b-9cad-4b75-977d-a2fdb3d02e77', NULL, 2, 1, 1, 1, 'Working on data visualization'),
                ('User Notifications', 'Implement real-time user notifications', 24.0, NULL, '2025-09-15', NULL, '2025-09-20', 'cf41999b-9cad-4b75-977d-a2fdb3d02e77', NULL, 1, 2, 2, 1, 'Planned for next sprint'),
                ('Export Functionality', 'Add PDF and Excel export features', 32.0, NULL, '2025-09-20', NULL, '2025-09-25', 'cf41999b-9cad-4b75-977d-a2fdb3d02e77', NULL, 1, 1, 1, 1, 'Requirements gathering phase'),
                ('Backup System', 'Implement automated backup system', 40.0, NULL, '2025-09-25', NULL, '2025-10-01', 'cf41999b-9cad-4b75-977d-a2fdb3d02e77', NULL, 1, 3, 1, 0, 'Infrastructure planning');
            ");

            // Add sample time tracking entries using SQL
            migrationBuilder.Sql(@"
                -- Get the task IDs for the newly created tasks
                DECLARE @TaskIds TABLE (Id INT, Name NVARCHAR(100));
                INSERT INTO @TaskIds (Id, Name)
                SELECT Id, Name FROM Tasks
                WHERE Name IN ('Database Optimization', 'API Documentation', 'Security Audit', 'Performance Testing', 'Code Review Process', 'Mobile Responsiveness', 'Chart.js Integration');

                -- Add time tracking entries
                INSERT INTO TaskTimeEntries (TaskId, OperatorId, StartTime, EndTime, DurationMinutes, Notes, SessionType, CreatedAt)
                SELECT t.Id, 1, '2025-08-15 09:00:00', '2025-08-15 13:00:00', 240, 'Initial analysis and query optimization', 'Development', '2025-08-15 13:00:00'
                FROM @TaskIds t WHERE t.Name = 'Database Optimization'
                UNION ALL
                SELECT t.Id, 1, '2025-08-16 10:00:00', '2025-08-16 15:30:00', 330, 'Index optimization and testing', 'Development', '2025-08-16 15:30:00'
                FROM @TaskIds t WHERE t.Name = 'Database Optimization'
                UNION ALL
                SELECT t.Id, 1, '2025-08-17 09:30:00', '2025-08-17 14:00:00', 270, 'Performance testing and validation', 'Testing', '2025-08-17 14:00:00'
                FROM @TaskIds t WHERE t.Name = 'Database Optimization'
                UNION ALL
                SELECT t.Id, 1, '2025-08-20 08:00:00', '2025-08-20 12:00:00', 240, 'API endpoint documentation', 'Documentation', '2025-08-20 12:00:00'
                FROM @TaskIds t WHERE t.Name = 'API Documentation'
                UNION ALL
                SELECT t.Id, 1, '2025-08-21 09:00:00', '2025-08-21 16:00:00', 420, 'Examples and code samples', 'Documentation', '2025-08-21 16:00:00'
                FROM @TaskIds t WHERE t.Name = 'API Documentation'
                UNION ALL
                SELECT t.Id, 1, '2025-08-22 10:00:00', '2025-08-22 15:00:00', 300, 'Review and formatting', 'Review', '2025-08-22 15:00:00'
                FROM @TaskIds t WHERE t.Name = 'API Documentation'
                UNION ALL
                SELECT t.Id, 1, '2025-09-05 09:00:00', '2025-09-05 17:00:00', 480, 'Mobile layout analysis', 'Development', '2025-09-05 17:00:00'
                FROM @TaskIds t WHERE t.Name = 'Mobile Responsiveness'
                UNION ALL
                SELECT t.Id, 1, '2025-09-06 08:30:00', '2025-09-06 16:30:00', 480, 'CSS responsive improvements', 'Development', '2025-09-06 16:30:00'
                FROM @TaskIds t WHERE t.Name = 'Mobile Responsiveness';
            ");

            // Add an active session for one of the in-progress tasks
            migrationBuilder.Sql(@"
                INSERT INTO TaskTimeSessions (TaskId, OperatorId, StartTime, TotalPausedMinutes, IsPaused, Status, CreatedAt, UpdatedAt)
                SELECT t.Id, 1, '2025-09-09 09:00:00', 0, 0, 'Active', '2025-09-09 09:00:00', '2025-09-09 09:00:00'
                FROM Tasks t WHERE t.Name = 'Mobile Responsiveness';
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove the sample data using SQL
            migrationBuilder.Sql(@"
                -- Remove time sessions
                DELETE FROM TaskTimeSessions
                WHERE TaskId IN (SELECT Id FROM Tasks WHERE Name IN ('Mobile Responsiveness', 'Chart.js Integration'));

                -- Remove time entries
                DELETE FROM TaskTimeEntries
                WHERE TaskId IN (SELECT Id FROM Tasks WHERE Name IN ('Database Optimization', 'API Documentation', 'Security Audit', 'Performance Testing', 'Code Review Process', 'Mobile Responsiveness', 'Chart.js Integration'));

                -- Remove tasks
                DELETE FROM Tasks
                WHERE Name IN ('Database Optimization', 'API Documentation', 'Security Audit', 'Performance Testing', 'Code Review Process', 'Mobile Responsiveness', 'Chart.js Integration', 'User Notifications', 'Export Functionality', 'Backup System');
            ");
        }
    }
}
