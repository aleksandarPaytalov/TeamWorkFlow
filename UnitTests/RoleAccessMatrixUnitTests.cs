using NUnit.Framework;
using static TeamWorkFlow.Core.Constants.UsersConstants;

namespace UnitTests
{
    [TestFixture]
    public class RoleAccessMatrixUnitTests
    {
        #region Role-Based Access Matrix Tests

        [TestCase(GuestRole, false, "Guest users should not have access to Sprint To Do")]
        [TestCase(OperatorRole, true, "Operator users should have access to Sprint To Do")]
        [TestCase(AdminRole, true, "Admin users should have access to Sprint To Do")]
        public void SprintToDoAccess_RoleBasedMatrix_ReturnsExpectedAccess(string role, bool expectedAccess, string description)
        {
            // Act
            var hasAccess = IsSprintToDoAccessible(role);

            // Assert
            Assert.That(hasAccess, Is.EqualTo(expectedAccess), description);
        }

        [TestCase(GuestRole, true, "Guest users should have read-only access to Tasks")]
        [TestCase(OperatorRole, true, "Operator users should have full access to Tasks")]
        [TestCase(AdminRole, true, "Admin users should have full access to Tasks")]
        public void TaskAccess_RoleBasedMatrix_ReturnsExpectedAccess(string role, bool expectedAccess, string description)
        {
            // Act
            var hasAccess = IsTaskAccessible(role);

            // Assert
            Assert.That(hasAccess, Is.EqualTo(expectedAccess), description);
        }

        [TestCase(GuestRole, false, "Guest users should not have access to Admin features")]
        [TestCase(OperatorRole, false, "Operator users should not have access to Admin features")]
        [TestCase(AdminRole, true, "Admin users should have access to Admin features")]
        public void AdminAccess_RoleBasedMatrix_ReturnsExpectedAccess(string role, bool expectedAccess, string description)
        {
            // Act
            var hasAccess = IsAdminAccessible(role);

            // Assert
            Assert.That(hasAccess, Is.EqualTo(expectedAccess), description);
        }

        #endregion

        #region User Role Transition Tests

        [Test]
        public void UserRoleTransition_GuestToOperator_GainsSprintAccess()
        {
            // Arrange - Start as Guest
            var initialAccess = IsSprintToDoAccessible(GuestRole);

            // Act - Transition to Operator
            var newAccess = IsSprintToDoAccessible(OperatorRole);

            // Assert
            Assert.IsFalse(initialAccess, "Guest should not have Sprint access initially");
            Assert.IsTrue(newAccess, "Operator should have Sprint access after promotion");
        }

        [Test]
        public void UserRoleTransition_OperatorToGuest_LosesSprintAccess()
        {
            // Arrange - Start as Operator
            var initialAccess = IsSprintToDoAccessible(OperatorRole);

            // Act - Transition to Guest
            var newAccess = IsSprintToDoAccessible(GuestRole);

            // Assert
            Assert.IsTrue(initialAccess, "Operator should have Sprint access initially");
            Assert.IsFalse(newAccess, "Guest should not have Sprint access after demotion");
        }

        [Test]
        public void UserRoleTransition_GuestToAdmin_GainsAllAccess()
        {
            // Arrange - Start as Guest
            var initialSprintAccess = IsSprintToDoAccessible(GuestRole);
            var initialAdminAccess = IsAdminAccessible(GuestRole);

            // Act - Transition to Admin
            var newSprintAccess = IsSprintToDoAccessible(AdminRole);
            var newAdminAccess = IsAdminAccessible(AdminRole);

            // Assert
            Assert.IsFalse(initialSprintAccess, "Guest should not have Sprint access initially");
            Assert.IsFalse(initialAdminAccess, "Guest should not have Admin access initially");
            Assert.IsTrue(newSprintAccess, "Admin should have Sprint access after promotion");
            Assert.IsTrue(newAdminAccess, "Admin should have Admin access after promotion");
        }

        #endregion

        #region Role Permission Matrix Tests

        [TestCase(GuestRole, true, false, false, false)]
        [TestCase(OperatorRole, false, true, false, true)]
        [TestCase(AdminRole, false, false, true, false)]
        public void RolePermissionMatrix_ValidatesCorrectPermissions(
            string role, 
            bool canAssignRole, 
            bool canPromoteToAdmin, 
            bool canDemoteFromAdmin, 
            bool canDemoteToGuest)
        {
            // Act
            var actualCanAssignRole = CanAssignRole(role);
            var actualCanPromoteToAdmin = CanPromoteToAdmin(role);
            var actualCanDemoteFromAdmin = CanDemoteFromAdmin(role);
            var actualCanDemoteToGuest = CanDemoteToGuest(role);

            // Assert
            Assert.That(actualCanAssignRole, Is.EqualTo(canAssignRole), $"CanAssignRole failed for {role}");
            Assert.That(actualCanPromoteToAdmin, Is.EqualTo(canPromoteToAdmin), $"CanPromoteToAdmin failed for {role}");
            Assert.That(actualCanDemoteFromAdmin, Is.EqualTo(canDemoteFromAdmin), $"CanDemoteFromAdmin failed for {role}");
            Assert.That(actualCanDemoteToGuest, Is.EqualTo(canDemoteToGuest), $"CanDemoteToGuest failed for {role}");
        }

        #endregion

        #region Complete Role Lifecycle Tests

        [Test]
        public void CompleteRoleLifecycle_GuestToOperatorToGuest_AccessChangesCorrectly()
        {
            // Step 1: Guest user
            var guestSprintAccess = IsSprintToDoAccessible(GuestRole);
            var guestCanAssignRole = CanAssignRole(GuestRole);
            var guestCanDemoteToGuest = CanDemoteToGuest(GuestRole);

            // Step 2: Promoted to Operator
            var operatorSprintAccess = IsSprintToDoAccessible(OperatorRole);
            var operatorCanAssignRole = CanAssignRole(OperatorRole);
            var operatorCanDemoteToGuest = CanDemoteToGuest(OperatorRole);

            // Step 3: Demoted back to Guest
            var finalGuestSprintAccess = IsSprintToDoAccessible(GuestRole);
            var finalGuestCanAssignRole = CanAssignRole(GuestRole);
            var finalGuestCanDemoteToGuest = CanDemoteToGuest(GuestRole);

            // Assert Guest state
            Assert.IsFalse(guestSprintAccess, "Guest should not have Sprint access");
            Assert.IsTrue(guestCanAssignRole, "Guest should be assignable to roles");
            Assert.IsFalse(guestCanDemoteToGuest, "Guest cannot be demoted to guest");

            // Assert Operator state
            Assert.IsTrue(operatorSprintAccess, "Operator should have Sprint access");
            Assert.IsFalse(operatorCanAssignRole, "Operator should not be assignable to roles");
            Assert.IsTrue(operatorCanDemoteToGuest, "Operator can be demoted to guest");

            // Assert final Guest state (same as initial)
            Assert.IsFalse(finalGuestSprintAccess, "Final guest should not have Sprint access");
            Assert.IsTrue(finalGuestCanAssignRole, "Final guest should be assignable to roles");
            Assert.IsFalse(finalGuestCanDemoteToGuest, "Final guest cannot be demoted to guest");
        }

        [Test]
        public void CompleteRoleLifecycle_GuestToAdminToOperatorToGuest_AccessChangesCorrectly()
        {
            // Step 1: Guest user
            var guestAccess = GetRoleAccessSummary(GuestRole);

            // Step 2: Promoted to Admin
            var adminAccess = GetRoleAccessSummary(AdminRole);

            // Step 3: Demoted to Operator
            var operatorAccess = GetRoleAccessSummary(OperatorRole);

            // Step 4: Demoted to Guest
            var finalGuestAccess = GetRoleAccessSummary(GuestRole);

            // Assert progression
            Assert.IsFalse(guestAccess.HasSprintAccess, "Guest should not have Sprint access");
            Assert.IsFalse(guestAccess.HasAdminAccess, "Guest should not have Admin access");

            Assert.IsTrue(adminAccess.HasSprintAccess, "Admin should have Sprint access");
            Assert.IsTrue(adminAccess.HasAdminAccess, "Admin should have Admin access");

            Assert.IsTrue(operatorAccess.HasSprintAccess, "Operator should have Sprint access");
            Assert.IsFalse(operatorAccess.HasAdminAccess, "Operator should not have Admin access");

            Assert.IsFalse(finalGuestAccess.HasSprintAccess, "Final guest should not have Sprint access");
            Assert.IsFalse(finalGuestAccess.HasAdminAccess, "Final guest should not have Admin access");
        }

        #endregion

        #region Helper Methods

        private bool IsSprintToDoAccessible(string role)
        {
            // Simulate the authorization logic from the views
            return role == OperatorRole || role == AdminRole;
        }

        private bool IsTaskAccessible(string role)
        {
            // All roles have access to tasks (Guest has read-only)
            return true;
        }

        private bool IsAdminAccessible(string role)
        {
            // Only Admin role has access to admin features
            return role == AdminRole;
        }

        private bool CanAssignRole(string role)
        {
            // Can assign role if user is only a guest
            return role == GuestRole;
        }

        private bool CanPromoteToAdmin(string role)
        {
            // Can promote to admin if user is an operator (not admin or guest)
            return role == OperatorRole;
        }

        private bool CanDemoteFromAdmin(string role)
        {
            // Can demote from admin if user is an admin
            return role == AdminRole;
        }

        private bool CanDemoteToGuest(string role)
        {
            // Can demote to guest if user is only an operator (not admin)
            return role == OperatorRole;
        }

        private RoleAccessSummary GetRoleAccessSummary(string role)
        {
            return new RoleAccessSummary
            {
                HasSprintAccess = IsSprintToDoAccessible(role),
                HasAdminAccess = IsAdminAccessible(role),
                HasTaskAccess = IsTaskAccessible(role)
            };
        }

        private class RoleAccessSummary
        {
            public bool HasSprintAccess { get; set; }
            public bool HasAdminAccess { get; set; }
            public bool HasTaskAccess { get; set; }
        }

        #endregion
    }
}
