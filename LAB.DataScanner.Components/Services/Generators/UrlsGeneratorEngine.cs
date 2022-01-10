using LAB.DataScanner.Components.Services.MessageBroker.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LAB.DataScanner.Components.Services.Generators
{
    public class UrlsGeneratorEngine
    {
        private readonly IRmqPublisher _rmqPublisher;

        private readonly IConfigurationSection _applicationSection;

        private readonly IConfigurationSection _bindingSection;

        public UrlsGeneratorEngine(IRmqPublisher rmqPublisher, IConfigurationSection applicationSection, 
            IConfigurationSection bindingSection)
        {
            _rmqPublisher = rmqPublisher;

            _applicationSection = applicationSection;

            _bindingSection = bindingSection;
        }

        public void Start() 
        {
            var templateUrl = _applicationSection.GetSection("UrlTemplate").Value;

            var rangeOptions = JsonConvert.DeserializeObject<string[]>(_applicationSection.GetSection("Sequences").Value)
                .Select(s => s.Split("..")).Select(s => s.Select(a => int.Parse(a))).ToList();

            var exchangeName = _bindingSection.GetSection("SenderExchange").Value;

            var routingKeys = JsonConvert.DeserializeObject<string[]>(_bindingSection.GetSection("SenderRoutingKeys").Value ?? "");

            var urlsList = BuildUrlsList(templateUrl, rangeOptions);

            Publish(urlsList, exchangeName, routingKeys);
        }

        public IEnumerable<string> BuildUrlsList(string templateUrl, List<IEnumerable<int>> rangeOptions) 
        {
            Regex pattern = new Regex("");

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

            for (int i = 0; i < crossP.Count(); i++)
            {
                var rangeIndex = 0;

                var newUrlString = templateUrl;

                for (int j = 0; j < rangeOptions.Count; j++)
                {
                    pattern = new Regex(@"\{" + rangeIndex++ + @"\}");

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
