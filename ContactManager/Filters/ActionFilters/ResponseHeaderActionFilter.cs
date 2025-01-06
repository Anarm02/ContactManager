using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactManager.Filters.ActionFilters
{
	public class ResponseHeaderActionFilter : IAsyncActionFilter,IOrderedFilter
	{
		private readonly ILogger<ResponseHeaderActionFilter> logger;
		private readonly string key;
		private readonly string value;
		public int Order { get; set; }

		public ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger, string key, string value, int order)
		{
			this.logger = logger;
			this.key = key;
			this.value = value;
			Order = order;
		}


		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			logger.LogInformation("{FilterName}.{Method} method", nameof(ResponseHeaderActionFilter), nameof(OnActionExecutionAsync));
			context.HttpContext.Response.Headers[key] = value;
			await next();
			logger.LogInformation("{FilterName}.{Method} method", nameof(ResponseHeaderActionFilter), nameof(OnActionExecutionAsync));

		}
	}
}
