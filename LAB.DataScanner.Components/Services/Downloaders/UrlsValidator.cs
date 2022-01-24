using System;
using System.Diagnostics;

namespace LAB.DataScanner.Components.Services.Downloaders
{
    public class UrlsValidator
    {
        public bool UrlIsValid(string uriString)
        {
            try
            {
                return Uri.TryCreate(uriString, UriKind.RelativeOrAbsolute, out Uri uriResult)
                    && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            }
            catch (InvalidOperationException e)
            {
                //logging exception
                Debug.Print(e.Message);
                return false;
            }
        }
    }
}
