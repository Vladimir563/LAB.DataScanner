using System;
using System.Net.Http;
using System.Threading.Tasks;
using OpenQA.Selenium;
using WebDriverManager;
using WebDriverManager.Helpers;
using WebDriverManager.DriverConfigs.Impl;
using OpenQA.Selenium.Chrome;
using System.Text;
using OpenQA.Selenium.Support.UI;
using LAB.DataScanner.Components.Settings;

namespace LAB.DataScanner.Components.Services.Downloaders
{
    public class HttpDataRetriever : IDataRetriever, IDisposable
    {
        private readonly WebPageDownloaderSettings _downloaderSettings;

        private readonly HttpClient _httpClient;

        public HttpDataRetriever(WebPageDownloaderSettings downloaderSettings)
        {
            _httpClient = new HttpClient ();

            _downloaderSettings = downloaderSettings;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        public async Task<byte[]> RetrieveBytesAsync(string url) =>
            (byte[]) await  RetrieveData(url, false);

        public async Task<string> RetrieveStringAsync(string url) =>
            (string) await RetrieveData(url, true);

        private async Task<object> RetrieveData(string url, bool isMethodReturnsString)
        {
            try
            {
                if (_downloaderSettings.HtmlDataDownloadingMethod.Equals("static", StringComparison.OrdinalIgnoreCase))
                {
                    HttpResponseMessage response = await _httpClient.GetAsync(url);

                    if (!response.EnsureSuccessStatusCode().StatusCode.Equals("OK"))
                    {
                        throw new HttpRequestException($"Some issues with http request have been occured (@Status code: {response.EnsureSuccessStatusCode().StatusCode})");
                    }

                    if (isMethodReturnsString)
                    {
                        return RetrieveStaticStringAsync(response);
                    }
                    else
                    {
                        return RetrieveStaticByteArrayAsync(response);
                    }
                }
                else if (_downloaderSettings.HtmlDataDownloadingMethod.Equals("dynamic", StringComparison.OrdinalIgnoreCase))
                {
                    return await RetrieveDynamicDataAsync(url, isMethodReturnsString);
                }
            }
            catch
            {
                throw;
            }

            return null;
        }

        private async Task<object> RetrieveDynamicDataAsync(string url, bool isReturnStringContent)
        {
            return await Task.Run(() =>
            {
                new DriverManager().SetUpDriver(new ChromeConfig(), VersionResolveStrategy.MatchingBrowser);

                ChromeOptions options = new ChromeOptions();

                //Don't load images
                options.AddUserProfilePreference("profile.managed_default_content_settings.images", 2);

                //Hide the browser window
                options.AddArgument("headless");

                ChromeDriverService driverService = ChromeDriverService.CreateDefaultService();

                //Close the Chrome Driver console
                driverService.HideCommandPromptWindow = true;

                ChromeDriver driver = new ChromeDriver(driverService, options);

                IWait<IWebDriver> wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                driver.Navigate().GoToUrl(url);

                wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
   
                object resultContent = null;

                if (isReturnStringContent) 
                {
                    resultContent = driver.PageSource;
                }
                else resultContent = Encoding.UTF8.GetBytes(driver.PageSource);

                driver.Quit();

                return resultContent;
            });
        }

        private async Task<string> RetrieveStaticStringAsync(HttpResponseMessage response) 
        {
            return await Task.Run(() => response.Content.ReadAsStringAsync());
        }

        private async Task<byte[]> RetrieveStaticByteArrayAsync(HttpResponseMessage response)
        {
            return await Task.Run(() => response.Content.ReadAsByteArrayAsync());
        }
    }
}