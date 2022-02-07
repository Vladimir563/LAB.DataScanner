using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using OpenQA.Selenium;
using WebDriverManager;
using WebDriverManager.Helpers;
using WebDriverManager.DriverConfigs.Impl;
using OpenQA.Selenium.Chrome;
using System.Text;
using OpenQA.Selenium.Support.UI;
using Microsoft.Extensions.Logging;

namespace LAB.DataScanner.Components.Services.Downloaders
{
    public class HttpDataRetriever : IDataRetriever, IDisposable
    {
        private readonly HttpClient _httpClient;

        private readonly IConfiguration _configuration;

        private string[] _configDownloadingMethods;

        private string[] _configWebBrowsers;

        private string _htmlDataDownloadingMethod;

        private string _webBrowser;

        private bool _isContinueWork = false;

        private readonly ILogger<HttpDataRetriever> _logger;

        public HttpDataRetriever(IConfigurationRoot configuration, ILogger<HttpDataRetriever> logger)
        {
            _httpClient = new HttpClient();

            _configuration = configuration;

            _logger = logger;

            CheckAllConfigParamsOnValid();
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
            if (!_isContinueWork) return null;

            try
            {
                if (_htmlDataDownloadingMethod.Equals("static", StringComparison.OrdinalIgnoreCase))
                {
                    HttpResponseMessage response = await _httpClient.GetAsync(url);

                    if (!response.EnsureSuccessStatusCode().StatusCode.Equals("OK"))
                    {
                        _logger.LogError($"Some issues with http request have been occured (@Status code: {response.EnsureSuccessStatusCode().StatusCode})");
                        return null;
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
                else if (_htmlDataDownloadingMethod.Equals("dynamic", StringComparison.OrdinalIgnoreCase))
                {
                    return await RetrieveDynamicDataAsync(url, isMethodReturnsString);
                }
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e.Message);
                return null;
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

                var isPageDownloaded = wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
   
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

        private void CheckAllConfigParamsOnValid() 
        {
            var _downloadingSettingsArrsSection = _configuration.GetSection("HtmlDataDownloadingSettingsArrs");

            var _applicationSection = _configuration.GetSection("Application");

            //get all possible DownloadingMethods from config
            _configDownloadingMethods = JsonConvert.DeserializeObject<string[]>
                (_downloadingSettingsArrsSection.GetSection("HtmlDataDownloadingMethods").Value ?? "");

            //get all possible WebBrowsers from config
            _configWebBrowsers = JsonConvert.DeserializeObject<string[]>
                (_downloadingSettingsArrsSection.GetSection("WebBrowsers").Value ?? "");

            _htmlDataDownloadingMethod = _applicationSection.GetSection("HtmlDataDownloadingMethod").Value ?? "";

            if (_htmlDataDownloadingMethod.Equals("") || !_configDownloadingMethods.Contains(_htmlDataDownloadingMethod))
            {
                _logger.LogError($"HtmlDataDownloadingMethod is not valid: " +
                    $"{_htmlDataDownloadingMethod} " +
                    $"({String.Join($", ", _configDownloadingMethods.Select(p => p.ToString()).ToArray())} " +
                    $"was expected)");

                _isContinueWork = false;
            }

            _webBrowser = _applicationSection.GetSection("WebBrowser").Value ?? "";

            if (_webBrowser.Equals("") || !_configWebBrowsers.Contains(_webBrowser))
            {
                _logger.LogError($"WebBrowser name is not valid: " +
                    $"{String.Join($", ", _configWebBrowsers.Select(p => p.ToString()).ToArray())} " +
                    $"was expected)");

                _isContinueWork = false;
            }

            _isContinueWork = true;
        }
    }
}