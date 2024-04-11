using System.Text.RegularExpressions;
using TeamWorkFlow.Core.Contracts;

namespace TeamWorkFlow.Core.Extensions
{
	public static class ProjectModelExtension
	{
		public static string GetProjectExtension (this IProjectModel project)
		{
			string info = project.ProjectName.Replace(" ", "-") + project.ProjectNumber;
			info = Regex.Replace(info, @"[^\w\d\-]", "-");

			return info;
		}

	
	}
}
