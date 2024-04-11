namespace TeamWorkFlow.Extensions
{
	public static class ControllerRouteExtensions
	{
		public static void MapEntityControllerRoutes(this IEndpointRouteBuilder endpoints, string entityName)
		{
			endpoints.MapControllerRoute(
				name: $"{entityName} Details",
				pattern: $"/{entityName}/Details/{{id}}/{{extension}}",
				defaults: new { Controller = entityName, Action = "Details" }
			);

			endpoints.MapControllerRoute(
				name: $"{entityName} Delete",
				pattern: $"/{entityName}/Delete/{{id}}/{{extension}}",
				defaults: new { Controller = entityName, Action = "Delete" }
			);

			endpoints.MapControllerRoute(
				name: $"{entityName} Edit",
				pattern: $"/{entityName}/Edit/{{id}}/{{extension}}",
				defaults: new { Controller = entityName, Action = "Edit" }
			);
		}
	}
}
