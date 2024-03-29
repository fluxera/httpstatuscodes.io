namespace Fluxera.HttpStatusCodes.Pages.Shared
{
    using Fluxera.HttpStatusCodes.Model;

    public sealed class StatusCodeModel
    {
        public StatusCodeModel(StatusCodePageContent pageContent, StatusCodeClass statusCodeClass)
        {
	        this.PageContent = pageContent;
	        this.StatusCodeClass = statusCodeClass;
        }

        public StatusCodePageContent PageContent { get; }

        public StatusCodeClass StatusCodeClass { get; }
    }
}
