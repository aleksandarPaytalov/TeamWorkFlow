﻿using TeamWorkFlow.Core.Contracts;

namespace TeamWorkFlow.Core.Models.Project
{
	public class ProjectServiceModel : IProjectModel
	{
		public int Id { get; set; }

		public string ProjectName { get; set; } = string.Empty;

		public string ProjectNumber { get; set; } = string.Empty;

		public string Status { get; set; } = string.Empty;

		public int TotalParts { get; set; }
	}
}
