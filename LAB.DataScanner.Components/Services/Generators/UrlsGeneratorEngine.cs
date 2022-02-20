using LAB.DataScanner.Components.Interfaces.Engines;
using LAB.DataScanner.Components.Services.MessageBroker.Interfaces;
using LAB.DataScanner.Components.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LAB.DataScanner.Components.Services.Generators
{
    public class UrlsGeneratorEngine : IEngine
    {
        private readonly IRmqPublisher _rmqPublisher;

        private readonly UrlsGeneratorSettings _generatorSettings;

        private readonly RmqPublisherSettings _publisherSettings;

        public UrlsGeneratorEngine(IRmqPublisher rmqPublisher, UrlsGeneratorSettings generatorSettings, RmqPublisherSettings publisherSettings)
        {
            _rmqPublisher = rmqPublisher;

            _generatorSettings = generatorSettings;

            _publisherSettings = publisherSettings;
        }

        public void Start() 
        {
            var exchangeName = _publisherSettings.SenderExchange;

            var routingKeys = _publisherSettings.SenderRoutingKeys;

            var urlsList = BuildUrlsList(_generatorSettings.UrlTemplate, _generatorSettings.Sequences);

            Publish(urlsList, exchangeName, routingKeys);
        }

        public IEnumerable<string> BuildUrlsList(string templateUrl, List<int[]> rangeOptions) 
        {
            List<string> urlsList = new List<string>();

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
