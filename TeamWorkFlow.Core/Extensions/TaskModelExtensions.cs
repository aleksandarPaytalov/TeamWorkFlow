using System.Text.RegularExpressions;
using TeamWorkFlow.Core.Contracts;

namespace TeamWorkFlow.Core.Extensions
{
    public static class TaskModelExtensions
    {
        public static string GetTaskExtension(this ITaskModel task)
        {
            string info = task.Name.Replace(" ", "-") + GetTaskDescription(task.Description);
			info = Regex.Replace(info, @"[^\w\d\-]", "-");

			return info;
        }

        private static string GetTaskDescription(string description)
        {
            description = string.Join("-", description.Split(" ").Take(2));

            return description;
        }

    }
}
