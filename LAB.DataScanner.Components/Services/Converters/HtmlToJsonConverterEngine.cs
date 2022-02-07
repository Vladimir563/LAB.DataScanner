using HtmlAgilityPack;
using LAB.DataScanner.Components.Interfaces.Converters;
using LAB.DataScanner.Components.Services.MessageBroker.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LAB.DataScanner.Components.Services.Converters
{
    public class HtmlToJsonConverterEngine : IConverterEngine<string, string>
    {
        private readonly IConfigurationRoot _htmlToJsonConverterEngineSettings;

        private readonly IRmqPublisher _rmqPublisher;

        private readonly IRmqConsumer _rmqConsumer;

        private readonly ILogger<HtmlToJsonConverterEngine> _logger;

        private readonly string[] _routingKeys;

        private readonly string _exchangeName;

        private readonly string _commonPattern;

        private readonly string [] _columsNamesArr;

        private readonly string [] _patterns;

        public HtmlToJsonConverterEngine(IConfigurationRoot htmlToJsonConverterEngineSettings, 
            IRmqPublisher rmqPublisher, IRmqConsumer rmqConsumer, ILogger<HtmlToJsonConverterEngine> logger)
        {
            _htmlToJsonConverterEngineSettings = htmlToJsonConverterEngineSettings;

            _rmqPublisher = rmqPublisher;

            _rmqConsumer = rmqConsumer;

            _logger = logger;

            _routingKeys = JsonConvert.DeserializeObject<string[]>(_htmlToJsonConverterEngineSettings
                .GetSection("Binding:SenderRoutingKeys").Value ?? "");

            _exchangeName = _htmlToJsonConverterEngineSettings
                .GetSection("Binding:SenderExchange").Value ?? "";

            var colums = htmlToJsonConverterEngineSettings.GetSection("DBTableCreationSettings:colums").Value;

            var columsArr = colums.Split(',');

            _columsNamesArr = columsArr.Select(p => p.Trim().Split(' ')).Select(p => p[0]).ToArray();

            _commonPattern = htmlToJsonConverterEngineSettings
                .GetSection("Application:HtmlFragmentExpression").Value;

            _patterns = _commonPattern.Split('&').ToArray();
        }

        public void Start() 
        {
            _logger.LogInformation("HtmlToJsonConverterEngine.Start() has been executed.");

            _rmqConsumer.StartListening(OnReceive);
        }

        public string Convert(string htmlContent)
        {
            List<HtmlNodeCollection> extractedHtmlNodes = new List<HtmlNodeCollection>();

            var htmlDoc = new HtmlDocument();

            var htmlFragmentStrategy = _htmlToJsonConverterEngineSettings.GetSection("Application:HtmlFragmentStrategy").Value ?? "";

            var htmlFragmentExpression = _htmlToJsonConverterEngineSettings.GetSection("Application:HtmlFragmentExpression").Value ?? "";

            if (!htmlFragmentStrategy.Equals("SelectNodes", StringComparison.OrdinalIgnoreCase) &&
                !htmlFragmentStrategy.Equals("SelectSingleNode", StringComparison.OrdinalIgnoreCase)) 
            {
                _logger.LogInformation("HtmlFragmentStrategy is not valid or not set");
                return "";
            }

            if (htmlFragmentExpression.Equals("", StringComparison.OrdinalIgnoreCase)) 
            {
                _logger.LogInformation("HtmlFragmentExpression is not set");
                return "";
            }

            //select parsing strategy pattern
            if (htmlFragmentStrategy.Equals("SelectNodes", StringComparison.OrdinalIgnoreCase))
            {
                extractedHtmlNodes = GetExtractedHtmlNodeCollection(htmlDoc, htmlContent, false);
            }
            else
            {
                extractedHtmlNodes = GetExtractedHtmlNodeCollection(htmlDoc, htmlContent, true);
            }

            return HtmlToJsonParse(extractedHtmlNodes, _columsNamesArr);
        }

        public void OnReceive(object model, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();

            var htmlContent = Encoding.UTF8.GetString(body);

            _rmqConsumer.Ack(ea);

            var jsonContent = Encoding.UTF8.GetBytes(Convert(htmlContent));

            if (!(jsonContent is null))
            {
                _rmqPublisher.Publish(jsonContent, _exchangeName, _routingKeys);
            }
            else 
            {
                _logger.LogError("No html fragments matches");
            }
        }

        public List<HtmlNodeCollection> GetExtractedHtmlNodeCollection(HtmlDocument htmlDoc, string htmlContent, bool isSingleNodeRequire)
        {
            htmlDoc.LoadHtml(htmlContent);

            List<HtmlNodeCollection> nodeCollectionList = new List<HtmlNodeCollection>();

            if (isSingleNodeRequire)
            {
                var singleNode = htmlDoc.DocumentNode.SelectSingleNode(_patterns[0]);

                var singleNodeCollection = new HtmlNodeCollection(singleNode);

                nodeCollectionList.Add(singleNodeCollection);
            }
            else 
            {
                foreach (var item in _patterns)
                {
                    var htmlNodes = htmlDoc.DocumentNode.SelectNodes(item);
                    if (htmlNodes is null) 
                    {
                        _logger.LogInformation("HtmlContent doesn't store any items");
                        break;
                    }
                    nodeCollectionList.Add(htmlNodes);
                }
            }
            return nodeCollectionList;
        }

        public static string HtmlToJsonParse(List<HtmlNodeCollection> nodeCollectionList, string[] columsNamesArr)
        {
            if (nodeCollectionList.Count == 0) return "";

            StringBuilder nodesHtmlString = new StringBuilder();

            for (int i = 0; i < nodeCollectionList[0].Count; i++)
            {
                nodesHtmlString.AppendLine("{");

                for (int j = 0; j < nodeCollectionList.Count; j++)
                {
                    nodesHtmlString.AppendLine($"\"{columsNamesArr[j]}\" : \"{nodeCollectionList[j][i].InnerText}\"{(j != nodeCollectionList.Count - 1 ? ',' : ' ')}");
                }
                nodesHtmlString.AppendLine("}" + $"{(i == nodeCollectionList[0].Count - 1 ? ' ' : ',')}");
            }

            var nodesHtmlStringPrep = $"{String.Join($"", nodesHtmlString.ToString().Select(ch => ch.Equals('\'') ? "\'\'" : ch.ToString()).ToArray())}";

            return $"N'[\n{nodesHtmlStringPrep}]'";
        }

    }
}
