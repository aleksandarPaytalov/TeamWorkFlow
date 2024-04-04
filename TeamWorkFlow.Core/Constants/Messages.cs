namespace TeamWorkFlow.Core.Constants
{
    public static class Messages
    {
        public const string DateFormat = "dd/MM/yyyy";

        public const string StringLength = "The field {0}, must be between {2} and {1} characters long.";

        public const string RequiredMessage = "The {0} field is required.";

        public const string StringNumberRange = "The field {0}, must be a positive number between {1} and {2}.";

        public const string StatusNotExisting = "Selected status does not exist";

        public const string ProjectWithGivenNumberDoNotExist = "Project with this number do not exist";

        public const string ProjectWithThisNumberAlreadyCreated = "Project with this number already have been created";

        public const string InvalidDate = "Date is not valid. It must be in format {0}";

        public const string CapacityRange = "Capacity must be value between {0} and {1}";

        public const string CalibrationStatusException = "Incorrect input. It must be true or false.";

        public const string InvalidIdInput = "Invalid Id.";

        public const string BooleanInput = "The input for IsActive field must be true or false";

	}
}
