namespace Fluxera.HttpStatusCodes.Controllers
{
    using System;
    using System.Linq;
    using Fluxera.HttpStatusCodes.Model;
    using Fluxera.HttpStatusCodes.Pages.Shared;
    using Fluxera.HttpStatusCodes.Services;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.WebUtilities;

    [ApiController]
	public sealed class StatusCodesController : Controller
	{
		private readonly IStatusCodeModelRepository repository;

		public StatusCodesController(IStatusCodeModelRepository repository)
		{
			this.repository = repository;
		}

		[HttpGet("{statusCode:int}")]
		[ResponseCache(Duration = 60 * 60 * 24, NoStore = false, VaryByQueryKeys = ["statusCode"])]
		public IActionResult StatusCodeView(int statusCode)
		{
			if (!this.repository.ExistsStatusCodePageContent(statusCode))
			{
				return this.StatusCode(404);
			}

			StatusCodePageContent pageContent = this.repository.GetStatusCodePageContent(statusCode);
			StatusCodeClass statusCodeClass = this.repository.GetStatusCodeClass(pageContent.Set);

			StatusCodeModel model = new StatusCodeModel(pageContent, statusCodeClass);

			return this.View(model);
		}

		[HttpGet("{statusCode:int}.json")]
		[EnableCors("Default")]
		[Produces("application/json", Type = typeof(ProblemDetails))]
		[ResponseCache(Duration = 60 * 60 * 24, NoStore = false, VaryByQueryKeys = ["statusCode"])]
		public IActionResult Get(int statusCode)
		{
			try
			{
				if (!this.repository.ExistsStatusCodePageContent(statusCode))
				{
					return this.Problem(
						statusCode: 404,
						type: "https://httpstatuscodes.io/404",
						title: ReasonPhrases.GetReasonPhrase(404),
						instance: $"https://httpstatuscodes.io/{statusCode}.json");
				}

				StatusCodePageContent content = this.repository.GetStatusCodePageContent(statusCode);
				StatusCodeClass statusCodeClass = this.repository.GetStatusCodeClass(content.Set);

				return this.Ok(new
				{
					location = $"https://httpstatuscodes.io/{statusCode}",
					statusCode = content.Code,
					title = content.Title,
					category = statusCodeClass.Title
						.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
						.LastOrDefault() ?? string.Empty,
					description = content.Excerpt
				});
			}
			catch
			{
				return this.Problem(
					statusCode: 500,
					type: "https://httpstatuscodes.io/500",
					title: ReasonPhrases.GetReasonPhrase(500),
					instance: $"https://httpstatuscodes.io/{statusCode}.json");
			}
		}
	}
}
