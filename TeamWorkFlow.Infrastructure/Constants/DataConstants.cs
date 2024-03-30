namespace TeamWorkFlow.Infrastructure.Constants
{
    public static class DataConstants
    {
        /// <summary>
        /// AvailabilityStatus DataConstants
        /// </summary>
        public const int AvailabilityStatusNameMinLength = 5;
        public const int AvailabilityStatusNameMaxLength = 15;

        /// <summary>
        /// Operator DataConstants
        /// </summary>

        public const int OperatorFullNameMinLength = 2;
        public const int OperatorFullNameMaxLength = 100;

		public const int OperatorEmailMinLength = 5;
        public const int OperatorEmailMaxLength = 100;

        public const int OperatorPhoneMinLength = 5;
        public const int OperatorPhoneMaxLength = 20;

        public const int OperatorMinCapacity = 4;
        public const int OperatorMaxCapacity = 12;

        /// <summary>
        /// Task DataConstants
        /// </summary>
        public const int TaskNameMinLength = 5;
        public const int TaskNameMaxLength = 100;

        public const int TaskDescriptionMinLength = 5;
        public const int TaskDescriptionMaxLength = 1500;

        public const int TaskCreatorIdMaxLength = 450;

        public const int TaskCommentMaxLength = 2500;
        public const int TaskAttachmentsMaxLength = 1000;

        /// <summary>
        /// Priority DataConstants
        /// </summary>
        public const int PriorityNameMinLength = 3;
        public const int PriorityNameMaxLength = 15;

        /// <summary>
        /// Machine DataConstants
        /// </summary>
        public const int MachineNameMinLength = 3;
        public const int MachineNameMaxLength = 50;

        /// <summary>
        /// PartStatus DataConstants
        /// </summary>
        public const int PartStatusNameMinLength = 2;
        public const int PartStatusNameMaxLength = 25;

        /// <summary>
        /// Part DataConstants
        /// </summary>
        public const int PartNameMinLength = 2;
        public const int PartNameMaxLength = 150;

        public const int PartArticleNumberMinLength = 5;
        public const int PartArticleNumberMaxLength = 30;

        public const int PartClientNumberMinLength = 5;
        public const int PartClientNumberMaxLength = 30;

        public const int PartToolNumberMinValue = 1000;
        public const int PartToolNumberMaxValue = 9999;

        public const int PartImageUrlMinLength = 10; 
        public const int PartImageUrlMaxLength = 300;

        public const int PartModelMinLength = 10;
        public const int PartModelMaxLength = 200;

        /// <summary>
        /// Project DataConstants 
        /// </summary>
        public const int ProjectNumberMinLength = 6;
        public const int ProjectNumberMaxLength = 10;

        public const int ProjectNameMinLength = 5;
        public const int ProjectNameMaxLength = 100;

        public const int ProjectClientNameMinLength = 2;
        public const int ProjectClientNameMaxLength = 100;

        public const int ProjectApplianceMinLength = 2;
        public const int ProjectApplianceMaxLength = 100;

        /// <summary>
        /// ProjectStatus DataConstants
        /// </summary>
        public const int ProjectStatusNameMinLength = 5;
        public const int ProjectStatusNameMaxLength = 30;

        /// <summary>
        /// TaskStatus DataConstants 
        /// </summary>
        public const int TaskStatusNameMinLength = 2;
        public const int TaskStatusNameMaxLength = 30;
    }
}
