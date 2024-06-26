namespace Fluxera.HttpStatusCodes.Model
{
	using System.Collections.Generic;
	using Fluxera.Guards;

	public abstract class PageContent
	{
		protected PageContent(string markdown, IDictionary<string, object> frontMatter)
		{
			this.Markdown = Guard.Against.NullOrWhiteSpace(markdown);
			this.FrontMatter = frontMatter ?? new Dictionary<string, object>();
		}

		public string Markdown { get; }

		public string Title => (string)this.FrontMatter["title"];

		protected IDictionary<string, object> FrontMatter { get; }
	}
}
