using System.Text.RegularExpressions;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Operator;

namespace TeamWorkFlow.Core.Extensions
{
	public static class OperatorModelExtension
	{
        /// <summary>
        /// Maximum working hours per day/shift (100% capacity)
        /// </summary>
        private const int MaxWorkingHours = 9;

        public static string GetOperatorExtension(this IOperatorModel operatorModel)
        {
            string fullName = operatorModel.FullName;
            string firstName = fullName.Split(' ')[0]; // Extract the first name
            string email = operatorModel.Email;
            string operatorEmail = GetOperatorEmail(email);

            string info = $"{firstName}-{operatorEmail}";
            info = Regex.Replace(info, @"[^\w\d\-]", "-");

            return info;
        }

        /// <summary>
        /// Calculates the capacity percentage based on working hours and activity status
        /// Business rule: Only operators with "at work" status can be active and have capacity > 0%
        /// </summary>
        /// <param name="operatorModel">The operator model</param>
        /// <returns>Capacity percentage (0-100)</returns>
        public static int GetCapacityPercentage(this IOperatorModel operatorModel)
        {
            // If operator is not active, capacity is 0%
            // Note: IsActive should only be true if availability status is "at work"
            if (!operatorModel.IsActive)
            {
                return 0;
            }

            // Calculate percentage based on maximum 9 hours = 100%
            // Capacity field stores working hours per day/shift
            int capacityPercentage = (int)Math.Round((double)operatorModel.Capacity / MaxWorkingHours * 100);

            // Ensure the percentage is within valid range (0-100)
            return Math.Max(0, Math.Min(100, capacityPercentage));
        }

        /// <summary>
        /// Gets the capacity display text with percentage
        /// </summary>
        /// <param name="operatorModel">The operator model</param>
        /// <returns>Formatted capacity string</returns>
        public static string GetCapacityDisplay(this IOperatorModel operatorModel)
        {
            int percentage = operatorModel.GetCapacityPercentage();
            return $"{percentage}%";
        }

        /// <summary>
        /// Gets the capacity level for styling purposes
        /// </summary>
        /// <param name="operatorModel">The operator model</param>
        /// <returns>Capacity level: "high", "medium", "low", or "zero"</returns>
        public static string GetCapacityLevel(this IOperatorModel operatorModel)
        {
            int percentage = operatorModel.GetCapacityPercentage();

            if (percentage == 0)
                return "zero";
            else if (percentage >= 80)
                return "high";
            else if (percentage >= 50)
                return "medium";
            else
                return "low";
        }

        /// <summary>
        /// Extension method for OperatorFormModel to get operator extension
        /// </summary>
        public static string GetOperatorExtension(this OperatorFormModel operatorFormModel)
        {
            string fullName = operatorFormModel.FullName;
            string firstName = fullName.Split(' ')[0]; // Extract the first name
            string email = operatorFormModel.Email;
            string operatorEmail = GetOperatorEmail(email);

            string info = $"{firstName}-{operatorEmail}";
            info = Regex.Replace(info, @"[^\w\d\-]", "-");

            return info;
        }

        private static string GetOperatorEmail(string email)
        {
            string[] separators = { ".", "_", "@", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            string operatorEmail = string.Join("-", email.Split(separators, StringSplitOptions.RemoveEmptyEntries).Take(1));

            return operatorEmail;
        }

    }
}
