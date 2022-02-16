using LAB.DataScanner.Components.Interfaces.Generators;
using LAB.DataScanner.Components.Services.MessageBroker.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LAB.DataScanner.Components.Services.Generators
{
    public class UrlsGeneratorEngine : IGeneratorEngine
    {
        private readonly IRmqPublisher _rmqPublisher;

        private readonly IConfigurationRoot _configuration;

        private readonly ILogger<IGeneratorEngine> _logger;

        public UrlsGeneratorEngine(IRmqPublisher rmqPublisher, IConfigurationRoot configuration, ILogger<IGeneratorEngine> logger)
        {
            _rmqPublisher = rmqPublisher;

            _configuration = configuration;

            _logger = logger;
        }

        public void Generate() 
        {
            _logger.LogInformation("UrlsGeneratorEngine.Generate() method has been executed.");

            var templateUrl = _configuration.GetSection("Application:UrlTemplate").Value ?? "";

            var rangeOptions = JsonConvert.DeserializeObject<string[]>(_configuration
                .GetSection("Application:Sequences").Value)
                .Select(s => s.Split("..")).Select(s => s.Select(a => int.Parse(a))).ToList();
            
            var exchangeName = _configuration
                .GetSection("Binding:SenderExchange").Value ?? "";

            var routingKeys = JsonConvert.DeserializeObject<string[]>(_configuration
                .GetSection("Binding:SenderRoutingKeys").Value ?? "");

            var urlsList = BuildUrlsList(templateUrl, rangeOptions);

            Publish(urlsList, exchangeName, routingKeys);
        }

        public IEnumerable<string> BuildUrlsList(string templateUrl, List<IEnumerable<int>> rangeOptions) 
        {
            List<string> urlsList = new List<string>();

            //TODO: If rangeOptions is null you will catch an exception
            if (rangeOptions.Count == 0 || rangeOptions is null) 
            {
                _logger.LogError("Urls sequences is not set");
                return urlsList;
            }

            var urlSequences = rangeOptions.Select(s => s.ToArray()).Select(a =>
            {
                var r = new int[a[^1] - a[0] + 1];

                var counter = 0;

                for (int i = a[0]; i <= a[^1]; i++)
                {
                    r[counter++] = i;
                }

                return r;
            });

            var crossP = urlSequences.CartesianProduct().Select(s => s.ToArray()).ToList();

            for (int i = 0; i < crossP.Count; i++)
            {
                var rangeIndex = 0;

                var newUrlString = templateUrl;

                for (int j = 0; j < rangeOptions.Count; j++)
                {
                    var pattern = new Regex(@"\{" + rangeIndex++ + @"\}");

                    newUrlString = pattern.Replace(newUrlString, crossP[i][j].ToString());
                }

                urlsList.Add(newUrlString);
            }

            return urlsList;
        }

        public void Publish(IEnumerable<string> urlsList, string exchangeName, string[] routingKeys) 
        {
            foreach (var url in urlsList)
            {
                _rmqPublisher.Publish(Encoding.UTF8.GetBytes(url), exchangeName, routingKeys);
            }
        }
    }
}
