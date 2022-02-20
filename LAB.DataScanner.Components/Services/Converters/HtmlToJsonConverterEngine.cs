using HtmlAgilityPack;
using LAB.DataScanner.Components.Interfaces.Converters;
using LAB.DataScanner.Components.Services.MessageBroker.Interfaces;
using LAB.DataScanner.Components.Settings;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LAB.DataScanner.Components.Services.Converters
{
    public class HtmlToJsonConverterEngine : IConverterEngine<string, string>
    {
        private readonly RmqPublisherSettings _publisherSettings;

        private readonly HtmlToJsonConverterSettings _converterSettings;

        private readonly IRmqPublisher _rmqPublisher;

        private readonly IRmqConsumer _rmqConsumer;

        private readonly string [] _columsNamesArr;

        private readonly string [] _patterns;

        public HtmlToJsonConverterEngine(RmqPublisherSettings publisherSettings, 
                                        HtmlToJsonConverterSettings converterSettings,
                                        IRmqPublisher rmqPublisher, IRmqConsumer rmqConsumer)
        {
            _rmqPublisher = rmqPublisher;

            _rmqConsumer = rmqConsumer;

            _publisherSettings = publisherSettings;

            _converterSettings = converterSettings;

            _columsNamesArr = _converterSettings.Colums.Select(p => p.Trim().Split(' ')).Select(p => p[0]).ToArray();

            _patterns = _converterSettings.HtmlFragmentExpression.Split('&').ToArray();
        }

        public void Start() 
        {
            _rmqConsumer.StartListening(OnReceive);
        }

        public string Convert(string htmlContent)
        {
            if (htmlContent is null) 
            {
                throw new ArgumentNullException("Failed to get an htmlContent");
            }

            List<HtmlNodeCollection> extractedHtmlNodes;

            var htmlDoc = new HtmlDocument();

            var htmlFragmentStrategy = _converterSettings.HtmlFragmentStrategy;

            if (!htmlFragmentStrategy.Equals("SelectNodes", StringComparison.OrdinalIgnoreCase) &&
                !htmlFragmentStrategy.Equals("SelectSingleNode", StringComparison.OrdinalIgnoreCase)) 
            {
                throw new ArgumentException("HtmlFragmentStrategy is not valid (possible values: \"SelectNodes\", \"SelectSingleNode\")");
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

        private void OnReceive(object model, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();

            var htmlContent = Encoding.UTF8.GetString(body);

            _rmqConsumer.Ack(ea);

            var jsonContent = Encoding.UTF8.GetBytes(Convert(htmlContent));

            if (!(jsonContent is null))
            {
                _rmqPublisher.Publish(jsonContent, _publisherSettings.SenderExchange, _publisherSettings.SenderRoutingKeys);
            }
            else 
            {
                throw new FormatException("No html fragments matches");
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
                        throw new FormatException("HtmlContent doesn't store any items");
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

            return $"[\n{nodesHtmlStringPrep}]";
        }
    }
}
