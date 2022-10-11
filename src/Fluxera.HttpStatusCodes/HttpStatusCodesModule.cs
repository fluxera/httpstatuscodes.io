﻿namespace Fluxera.HttpStatusCodes
{
	using System;
	using Fluxera.Extensions.Hosting;
	using Fluxera.Extensions.Hosting.Modules;
	using Fluxera.Extensions.Hosting.Modules.AspNetCore;
	using Fluxera.Extensions.Hosting.Modules.AspNetCore.Cors;
	using Fluxera.Extensions.Hosting.Modules.AspNetCore.HttpApi;
	using Fluxera.Extensions.Hosting.Modules.AspNetCore.RazorPages;
	using Fluxera.Extensions.Hosting.Modules.Caching;
	using Fluxera.HttpStatusCodes.Services;
	using JetBrains.Annotations;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Http;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Hosting;
	using Microsoft.Net.Http.Headers;
	using Westwind.AspNetCore.Markdown;

	[UsedImplicitly]
	[DependsOn(typeof(CachingModule))]
	[DependsOn(typeof(CorsModule))]
	[DependsOn(typeof(RazorPagesModule))]
	[DependsOn(typeof(HttpApiModule))]
	internal sealed class HttpStatusCodesModule : ConfigureApplicationModule
	{
		/// <inheritdoc />
		public override void ConfigureServices(IServiceConfigurationContext context)
		{
			context.Log("AddMarkdown", services =>
			{
				services.AddMarkdown();
				services.AddResponseCaching();
				services.AddSingleton<IStatusCodeModelRepository, StatusCodeModelRepository>();
			});
		}

		/// <inheritdoc />
		public override void Configure(IApplicationInitializationContext context)
		{
			WebApplication app = context.GetApplicationBuilder();

			context.UseHttpsRedirection();

			if(context.Environment.IsDevelopment())
			{
				context.UseDeveloperExceptionPage();
			}
			else
			{
				context.UseExceptionHandler("/errors/500");
				context.UseHsts();
			}

			context.UseStatusCodePagesWithReExecute("/errors/{0}");

			context.UseRouting();

			context.UseCors();

			app.Use(async (httpContext, next) =>
			{
				httpContext.Response.Headers.Add("Content-Security-Policy", "default-src 'self';");
				await next();
			});

			context.UseResponseCaching();

			app.Use(async (httpContext, next) =>
			{
				httpContext.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
				{
					Public = true,
					MaxAge = TimeSpan.FromSeconds(10)
				};
				httpContext.Response.Headers[HeaderNames.Vary] = new string[] { "Accept-Encoding" };

				await next();
			});

			context.UseStaticFiles();

			context.UseMarkdown();

			context.UseEndpoints();
		}

		/// <inheritdoc />
		public override void PostConfigure(IApplicationInitializationContext context)
		{
			context.LoadStatusCodeData();
		}
	}
}
