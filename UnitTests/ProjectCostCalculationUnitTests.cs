using NUnit.Framework;
using System.ComponentModel.DataAnnotations;
using TeamWorkFlow.Core.Models.Project;
using TeamWorkFlow.Core.Models.Part;

namespace UnitTests
{
    [TestFixture]
    public class ProjectCostCalculationUnitTests
    {
        #region ProjectCostCalculationModel Tests

        [Test]
        public void ProjectCostCalculationModel_AllProperties_CanBeSetAndRetrieved()
        {
            // Arrange & Act
            var model = new ProjectCostCalculationModel
            {
                ProjectId = 123,
                ProjectName = "Test Project",
                ProjectNumber = "TP-123",
                CalculatedActualHours = 10.0,
                HourlyRate = 50.00m
            };

            // Assert
            Assert.That(model.ProjectId, Is.EqualTo(123));
            Assert.That(model.ProjectName, Is.EqualTo("Test Project"));
            Assert.That(model.ProjectNumber, Is.EqualTo("TP-123"));
            Assert.That(model.CalculatedActualHours, Is.EqualTo(10.0));
            Assert.That(model.HourlyRate, Is.EqualTo(50.00m));
        }

        [Test]
        public void ProjectCostCalculationModel_TotalLaborCost_CalculatesCorrectly()
        {
            // Arrange
            var model = new ProjectCostCalculationModel
            {
                CalculatedActualHours = 10.00,
                HourlyRate = 50.00m
            };

            // Act
            var totalCost = model.TotalLaborCost;

            // Assert
            Assert.That(totalCost, Is.EqualTo(500.00m)); // 10.0 hours * 50.00 rate
        }

        [Test]
        public void ProjectCostCalculationModel_TotalLaborCost_WithZeroHours_ReturnsZero()
        {
            // Arrange
            var model = new ProjectCostCalculationModel
            {
                CalculatedActualHours = 0.0,
                HourlyRate = 50.00m
            };

            // Act
            var totalCost = model.TotalLaborCost;

            // Assert
            Assert.That(totalCost, Is.EqualTo(0m));
        }

        [Test]
        public void ProjectCostCalculationModel_TotalLaborCost_WithZeroRate_ReturnsZero()
        {
            // Arrange
            var model = new ProjectCostCalculationModel
            {
                CalculatedActualHours = 10.00,
                HourlyRate = 0m
            };

            // Act
            var totalCost = model.TotalLaborCost;

            // Assert
            Assert.That(totalCost, Is.EqualTo(0m));
        }

        [Test]
        public void ProjectCostCalculationModel_TotalLaborCost_WithDecimalHours_CalculatesCorrectly()
        {
            // Arrange
            var model = new ProjectCostCalculationModel
            {
                CalculatedActualHours = 15.00, // 150 hours
                HourlyRate = 75.50m
            };

            // Act
            var totalCost = model.TotalLaborCost;

            // Assert
            Assert.That(totalCost, Is.EqualTo(1132.50m)); // 15.0 * 75.50
        }

        [Test]
        public void ProjectCostCalculationModel_FormattedCalculatedTotalHours_FormatsCorrectly()
        {
            // Arrange & Act
            var model1 = new ProjectCostCalculationModel { CalculatedActualHours = 0.0 };
            var model2 = new ProjectCostCalculationModel { CalculatedActualHours = 5.0 };
            var model3 = new ProjectCostCalculationModel { CalculatedActualHours = 2.03 };
            var model4 = new ProjectCostCalculationModel { CalculatedActualHours = 24.0 };
            var model5 = new ProjectCostCalculationModel { CalculatedActualHours = 25.0 }; // 1d 1h
            var model6 = new ProjectCostCalculationModel { CalculatedActualHours = 48.0 }; // 2d
            var model7 = new ProjectCostCalculationModel { CalculatedActualHours = 49.0 }; // 2d 1h

            // Assert
            Assert.That(model1.FormattedCalculatedActualHours, Is.EqualTo("0h"));
            Assert.That(model2.FormattedCalculatedActualHours, Is.EqualTo("5.0h"));
            Assert.That(model3.FormattedCalculatedActualHours, Is.EqualTo("2.0h")); // 2.03 rounds to 2.0
            Assert.That(model4.FormattedCalculatedActualHours, Is.EqualTo("1d")); // 24 hours = 1 day exactly
            Assert.That(model5.FormattedCalculatedActualHours, Is.EqualTo("1d 1.0h")); // 25 hours = 1d 1h
            Assert.That(model6.FormattedCalculatedActualHours, Is.EqualTo("2d")); // 48 hours = 2d exactly
            Assert.That(model7.FormattedCalculatedActualHours, Is.EqualTo("2d 1.0h")); // 49 hours = 2d 1h
        }

        [Test]
        public void ProjectCostCalculationModel_HourlyRateValidation_RequiredAttribute()
        {
            // Arrange
            var model = new ProjectCostCalculationModel();
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.That(isValid, Is.False);
            Assert.That(results.Any(r => r.MemberNames.Contains("HourlyRate")), Is.True);
        }

        [Test]
        public void ProjectCostCalculationModel_HourlyRateValidation_RangeAttribute()
        {
            // Arrange
            var model1 = new ProjectCostCalculationModel { HourlyRate = 0.005m }; // Below minimum
            var model2 = new ProjectCostCalculationModel { HourlyRate = 15000m }; // Above maximum
            var model3 = new ProjectCostCalculationModel { HourlyRate = 50m }; // Valid

            var context1 = new ValidationContext(model1);
            var context2 = new ValidationContext(model2);
            var context3 = new ValidationContext(model3);

            var results1 = new List<ValidationResult>();
            var results2 = new List<ValidationResult>();
            var results3 = new List<ValidationResult>();

            // Act
            var isValid1 = Validator.TryValidateObject(model1, context1, results1, true);
            var isValid2 = Validator.TryValidateObject(model2, context2, results2, true);
            var isValid3 = Validator.TryValidateObject(model3, context3, results3, true);

            // Assert
            Assert.That(isValid1, Is.False);
            Assert.That(isValid2, Is.False);
            Assert.That(isValid3, Is.True);

            Assert.That(results1.Any(r => r.MemberNames.Contains("HourlyRate")), Is.True);
            Assert.That(results2.Any(r => r.MemberNames.Contains("HourlyRate")), Is.True);
            Assert.That(results3.Any(r => r.MemberNames.Contains("HourlyRate")), Is.False);
        }

        [Test]
        public void ProjectCostCalculationModel_DisplayNameAttribute_IsCorrect()
        {
            // Arrange
            var property = typeof(ProjectCostCalculationModel).GetProperty("HourlyRate");

            // Act
            var displayAttribute = property?.GetCustomAttributes(typeof(DisplayAttribute), false)
                .FirstOrDefault() as DisplayAttribute;

            // Assert
            Assert.That(displayAttribute, Is.Not.Null);
            Assert.That(displayAttribute.Name, Is.EqualTo("Hourly Rate"));
        }

        #endregion

        #region ProjectDetailsServiceModel Cost Calculation Tests

        [Test]
        public void ProjectDetailsServiceModel_CostCalculationProperties_CanBeSetAndRetrieved()
        {
            // Arrange & Act
            var model = new ProjectDetailsServiceModel
            {
                HourlyRate = 75.00m,
                CalculatedActualHours = 12.00
            };

            // Assert
            Assert.That(model.HourlyRate, Is.EqualTo(75.00m));
            Assert.That(model.CalculatedActualHours, Is.EqualTo(12.0));
        }

        [Test]
        public void ProjectDetailsServiceModel_TotalLaborCost_CalculatesCorrectly()
        {
            // Arrange
            var model = new ProjectDetailsServiceModel
            {
                HourlyRate = 60.00m,
                CalculatedActualHours = 8.00
            };

            // Act
            var totalCost = model.TotalLaborCost;

            // Assert
            Assert.That(totalCost, Is.EqualTo(480.00m)); // 60 * 8.0
        }

        [Test]
        public void ProjectDetailsServiceModel_TotalLaborCost_WithZeroValues_ReturnsZero()
        {
            // Arrange
            var model1 = new ProjectDetailsServiceModel
            {
                HourlyRate = 0m,
                CalculatedActualHours = 10.00
            };

            var model2 = new ProjectDetailsServiceModel
            {
                HourlyRate = 50m,
                CalculatedActualHours = 0.0
            };

            // Act
            var totalCost1 = model1.TotalLaborCost;
            var totalCost2 = model2.TotalLaborCost;

            // Assert
            Assert.That(totalCost1, Is.EqualTo(0m));
            Assert.That(totalCost2, Is.EqualTo(0m));
        }

        [Test]
        public void ProjectDetailsServiceModel_TotalLaborCost_WithHighValues_CalculatesCorrectly()
        {
            // Arrange
            var model = new ProjectDetailsServiceModel
            {
                HourlyRate = 150.75m,
                CalculatedActualHours = 50.00
            };

            // Act
            var totalCost = model.TotalLaborCost;

            // Assert
            Assert.That(totalCost, Is.EqualTo(7537.50m)); // 150.75 * 50.0
        }

        [Test]
        public void ProjectDetailsServiceModel_InheritsFromProjectServiceModel()
        {
            // Arrange & Act
            var model = new ProjectDetailsServiceModel();

            // Assert
            Assert.That(model, Is.InstanceOf<ProjectServiceModel>());
        }

        [Test]
        public void ProjectDetailsServiceModel_HasOperatorContributions()
        {
            // Arrange & Act
            var model = new ProjectDetailsServiceModel();

            // Assert
            Assert.That(model.OperatorContributions, Is.Not.Null);
            Assert.That(model.OperatorContributions, Is.InstanceOf<IEnumerable<OperatorContributionServiceModel>>());
        }

        [Test]
        public void ProjectDetailsServiceModel_HasPartsCollection()
        {
            // Arrange & Act
            var model = new ProjectDetailsServiceModel();

            // Assert
            Assert.That(model.Parts, Is.Not.Null);
            Assert.That(model.Parts, Is.InstanceOf<IEnumerable<ProjectPartServiceModel>>());
        }

        #endregion

        #region Edge Cases and Boundary Tests

        [Test]
        public void ProjectCostCalculationModel_WithMaximumValues_HandlesCorrectly()
        {
            // Arrange
            var model = new ProjectCostCalculationModel
            {
                CalculatedActualHours = int.MaxValue,
                HourlyRate = 10000.00m // Maximum allowed rate
            };

            // Act & Assert - Should not throw
            Assert.DoesNotThrow(() =>
            {
                var totalCost = model.TotalLaborCost;
                Assert.That(totalCost, Is.GreaterThan(0));
            });
        }

        [Test]
        public void ProjectCostCalculationModel_WithMinimumValidValues_HandlesCorrectly()
        {
            // Arrange
            var model = new ProjectCostCalculationModel
            {
                CalculatedActualHours = 1.0,
                HourlyRate = 0.01m // Minimum allowed rate
            };

            // Act
            var totalCost = model.TotalLaborCost;

            // Assert
            Assert.That(totalCost, Is.EqualTo(0.01m));
        }

        [Test]
        public void ProjectCostCalculationModel_FormattedHours_WithLargeValues_FormatsCorrectly()
        {
            // Arrange
            var model = new ProjectCostCalculationModel
            {
                CalculatedActualHours = 1000.0 // 1000 hours = 41 days + 16 hours
            };

            // Act
            var formatted = model.FormattedCalculatedActualHours;

            // Assert
            Assert.That(formatted, Is.EqualTo("41d 16.0h")); // Double formatting shows .0
        }

        #endregion
    }
}
