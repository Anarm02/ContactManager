using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactManager.Filters.ResultFilters
{
	public class PersonsListResultFilter : IAsyncResultFilter
	{
		public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
		{
			context.HttpContext.Response.Headers["Last-Modified"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
			await next();
		}
	}
}
