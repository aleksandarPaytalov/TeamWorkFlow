namespace TeamWorkFlow.Core.Constants
{
    public static class Messages
    {
        public const string DateFormat = "dd/MM/yyyy";

        public const string StringLength = "The field {0}, must be between {2} and {1} characters long.";

        public const string RequiredMessage = "The {0} field is required.";

        public const string StringNumberRange = "The field {0}, must be a positive number between {1} and {2}.";

        public const string StatusNotExisting = "Selected status does not exist";

        public const string PriorityNotExisting = "Selected status does not exist";

		public const string ProjectWithGivenNumberDoNotExist = "Project with this number do not exist";

        public const string ProjectWithThisNumberAlreadyCreated = "Project with this number already have been created";

        public const string InvalidDate = "Date is not valid. It must be in format {0}";

        public const string CapacityRange = "Capacity must be value between {0} and {1}";

        public const string BooleanInput = "Incorrect input. It must be true or false.";

        public const string InvalidIdInput = "Invalid Id.";

        public const string OperatorWithIdDoNotExist = "Operator with Id do not exist";

        public const string StartDateGreaterThanEndDateOrDeadLine = "The start date cannot be greater than the end date or the deadline";

        public const string UserWithEmailNotRegistered = "User with email is not registered in the application";

        public const string UserMessageSuccess = "UserMessageSuccess";

        public const string UserMessageError = "UserMessageError";

        public const string ProjectNotExisting = "Project with this id do not exist";

        public const string TotalHoursNegative = "Total hours cannot be negative value";
    }
}
