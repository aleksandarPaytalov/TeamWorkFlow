using System.Text.RegularExpressions;
using TeamWorkFlow.Core.Contracts;

namespace TeamWorkFlow.Core.Extensions
{
	public static class OperatorModelExtension
	{
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

        private static string GetOperatorEmail(string email)
        {
            string[] separators = { ".", "_", "@", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            string operatorEmail = string.Join("-", email.Split(separators, StringSplitOptions.RemoveEmptyEntries).Take(1));

            return operatorEmail;
        }

    }
}
