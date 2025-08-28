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
                CalculatedTotalHours = 100,
                HourlyRate = 50.00m
            };

            // Assert
            Assert.That(model.ProjectId, Is.EqualTo(123));
            Assert.That(model.ProjectName, Is.EqualTo("Test Project"));
            Assert.That(model.ProjectNumber, Is.EqualTo("TP-123"));
            Assert.That(model.CalculatedTotalHours, Is.EqualTo(100));
            Assert.That(model.HourlyRate, Is.EqualTo(50.00m));
        }

        [Test]
        public void ProjectCostCalculationModel_TotalLaborCost_CalculatesCorrectly()
        {
            // Arrange
            var model = new ProjectCostCalculationModel
            {
                CalculatedTotalHours = 100,
                HourlyRate = 50.00m
            };

            // Act
            var totalCost = model.TotalLaborCost;

            // Assert
            Assert.That(totalCost, Is.EqualTo(5000.00m));
        }

        [Test]
        public void ProjectCostCalculationModel_TotalLaborCost_WithZeroHours_ReturnsZero()
        {
            // Arrange
            var model = new ProjectCostCalculationModel
            {
                CalculatedTotalHours = 0,
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
                CalculatedTotalHours = 100,
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
                CalculatedTotalHours = 150, // 150 hours
                HourlyRate = 75.50m
            };

            // Act
            var totalCost = model.TotalLaborCost;

            // Assert
            Assert.That(totalCost, Is.EqualTo(11325.00m)); // 150 * 75.50
        }

        [Test]
        public void ProjectCostCalculationModel_FormattedCalculatedTotalHours_FormatsCorrectly()
        {
            // Arrange & Act
            var model1 = new ProjectCostCalculationModel { CalculatedTotalHours = 0 };
            var model2 = new ProjectCostCalculationModel { CalculatedTotalHours = 5 };
            var model3 = new ProjectCostCalculationModel { CalculatedTotalHours = 23 };
            var model4 = new ProjectCostCalculationModel { CalculatedTotalHours = 24 };
            var model5 = new ProjectCostCalculationModel { CalculatedTotalHours = 25 };
            var model6 = new ProjectCostCalculationModel { CalculatedTotalHours = 48 };
            var model7 = new ProjectCostCalculationModel { CalculatedTotalHours = 49 };

            // Assert
            Assert.That(model1.FormattedCalculatedTotalHours, Is.EqualTo("0h"));
            Assert.That(model2.FormattedCalculatedTotalHours, Is.EqualTo("5h"));
            Assert.That(model3.FormattedCalculatedTotalHours, Is.EqualTo("23h"));
            Assert.That(model4.FormattedCalculatedTotalHours, Is.EqualTo("1d")); // 24 hours = 1 day exactly
            Assert.That(model5.FormattedCalculatedTotalHours, Is.EqualTo("1d 1h"));
            Assert.That(model6.FormattedCalculatedTotalHours, Is.EqualTo("2d"));
            Assert.That(model7.FormattedCalculatedTotalHours, Is.EqualTo("2d 1h"));
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
                CalculatedTotalHours = 120
            };

            // Assert
            Assert.That(model.HourlyRate, Is.EqualTo(75.00m));
            Assert.That(model.CalculatedTotalHours, Is.EqualTo(120));
        }

        [Test]
        public void ProjectDetailsServiceModel_TotalLaborCost_CalculatesCorrectly()
        {
            // Arrange
            var model = new ProjectDetailsServiceModel
            {
                HourlyRate = 60.00m,
                CalculatedTotalHours = 80
            };

            // Act
            var totalCost = model.TotalLaborCost;

            // Assert
            Assert.That(totalCost, Is.EqualTo(4800.00m)); // 60 * 80
        }

        [Test]
        public void ProjectDetailsServiceModel_TotalLaborCost_WithZeroValues_ReturnsZero()
        {
            // Arrange
            var model1 = new ProjectDetailsServiceModel
            {
                HourlyRate = 0m,
                CalculatedTotalHours = 100
            };

            var model2 = new ProjectDetailsServiceModel
            {
                HourlyRate = 50m,
                CalculatedTotalHours = 0
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
                CalculatedTotalHours = 500
            };

            // Act
            var totalCost = model.TotalLaborCost;

            // Assert
            Assert.That(totalCost, Is.EqualTo(75375.00m)); // 150.75 * 500
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
                CalculatedTotalHours = int.MaxValue,
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
                CalculatedTotalHours = 1,
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
                CalculatedTotalHours = 1000 // 1000 hours = 41 days + 16 hours
            };

            // Act
            var formatted = model.FormattedCalculatedTotalHours;

            // Assert
            Assert.That(formatted, Is.EqualTo("41d 16h"));
        }

        #endregion
    }
}
