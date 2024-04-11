using System.Text.RegularExpressions;
using TeamWorkFlow.Core.Contracts;

namespace TeamWorkFlow.Core.Extensions
{
	public static class PartModelExtension
	{
		public static string GetPartExtension(this IPartModel part)
		{
			string partExtension = part.Name.Replace(" ", "-") + part.PartArticleNumber;
			partExtension = Regex.Replace(partExtension, @"[^\w\d\-]", "-");

			return partExtension;
		}
	}
}
