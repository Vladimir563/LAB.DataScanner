using LAB.DataScanner.Components.Interfaces.Downloaders;
using Microsoft.Extensions.Logging;
using System;

namespace LAB.DataScanner.Components.Services.Downloaders
{
    public class UrlsValidator : IUrlsValidator
    {
        private readonly ILogger<UrlsValidator> _logger;

        public UrlsValidator(ILogger<UrlsValidator> logger)
        {
            _logger = logger;
        }

        public bool IsUrlValid(string uriString)
        {
            try
            {
                return Uri.TryCreate(uriString, UriKind.RelativeOrAbsolute, out Uri uriResult)
                    && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            }
            catch (InvalidOperationException e)
            {
                _logger.LogError(e.Message);
                return false;
            }
        }
    }
}
