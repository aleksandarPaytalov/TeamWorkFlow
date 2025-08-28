namespace TeamWorkFlow.Core.Models.Project
{
    /// <summary>
    /// Service model representing an operator's time contribution to a project
    /// </summary>
    public class OperatorContributionServiceModel
    {
        /// <summary>
        /// Operator identifier
        /// </summary>
        public int OperatorId { get; set; }

        /// <summary>
        /// Full name of the operator
        /// </summary>
        public string OperatorName { get; set; } = string.Empty;

        /// <summary>
        /// Total hours contributed by this operator on finished tasks for the project
        /// </summary>
        public int TotalHours { get; set; }

        /// <summary>
        /// Formatted duration display (e.g., "2d 4h" or "6h")
        /// </summary>
        public string FormattedDuration
        {
            get
            {
                if (TotalHours < 8)
                    return $"{TotalHours}h";

                int days = TotalHours / 8;
                int remainingHours = TotalHours % 8;

                if (remainingHours == 0)
                    return $"{days}d";

                return $"{days}d {remainingHours}h";
            }
        }

        /// <summary>
        /// Percentage of total project hours contributed by this operator
        /// </summary>
        public double ContributionPercentage { get; set; }

        /// <summary>
        /// Formatted contribution percentage for display
        /// </summary>
        public string FormattedContributionPercentage => $"{ContributionPercentage:F1}%";
    }
}
