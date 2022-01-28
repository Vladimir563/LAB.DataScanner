using HtmlAgilityPack;
using LAB.DataScanner.Components.Services.MessageBroker.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LAB.DataScanner.Components.Services.Converters
{
    public class HtmlToJsonConverterEngine
    {
        private readonly IConfigurationRoot _htmlToJsonConverterEngineSettings;

        private readonly IRmqPublisher _rmqPublisher;

        private readonly IRmqConsumer _rmqConsumer;

        private readonly string[] _routingKeys;

        private readonly string _exchangeName;

        private readonly string _commonPattern;

        private readonly string [] _columsNamesArr;

        private readonly string [] _patterns;

        public HtmlToJsonConverterEngine(IConfigurationRoot htmlToJsonConverterEngineSettings, 
            IRmqPublisher rmqPublisher, IRmqConsumer rmqConsumer)
        {
            _htmlToJsonConverterEngineSettings = htmlToJsonConverterEngineSettings;

            _rmqPublisher = rmqPublisher;

            _rmqConsumer = rmqConsumer;

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
            _rmqConsumer.StartListening(OnReceive);
        }

        public string ParseHtmlToJson(string htmlContent) 
        {
            List<HtmlNodeCollection> extractedHtmlNodes = new List<HtmlNodeCollection>();

            var htmlDoc = new HtmlDocument();

            var htmlFragmentStrategy = _htmlToJsonConverterEngineSettings.GetSection("Application:HtmlFragmentStrategy").Value ?? "";

            var htmlFragmentExpression = _htmlToJsonConverterEngineSettings.GetSection("Application:HtmlFragmentExpression").Value ?? "";

            if (!htmlFragmentStrategy.Equals("SelectNodes", StringComparison.OrdinalIgnoreCase) &&
                !htmlFragmentStrategy.Equals("SelectSingleNode", StringComparison.OrdinalIgnoreCase)) 
            {
                throw new ArgumentNullException("HtmlFragmentStrategy is not valid or not set");
            }

            if (htmlFragmentExpression.Equals("", StringComparison.OrdinalIgnoreCase)) 
            {
                throw new ArgumentNullException("HtmlFragmentExpression is not set");
            }

            //select parsing strategy pattern
            if (htmlFragmentStrategy.Equals("SelectNodes", StringComparison.OrdinalIgnoreCase))
            {
                //var nodes = htmlDoc.DocumentNode.SelectNodes($"{htmlFragmentExpression}");

                extractedHtmlNodes = GetExtractedHtmlNodeCollection(htmlDoc, htmlContent, false);

                //htmlNodesString = $"{String.Join($"\n\n\n", nodes.Select(p => p.InnerHtml).ToArray())}"; 
            }
            else
            {
                extractedHtmlNodes = GetExtractedHtmlNodeCollection(htmlDoc, htmlContent, true);

                //htmlNodesString = htmlDoc.DocumentNode.SelectSingleNode($"{htmlFragmentExpression}").InnerHtml;
            }

            return HtmlToJsonParse(extractedHtmlNodes, _columsNamesArr);
        }

        public void OnReceive(object model, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();

            var htmlContent = Encoding.UTF8.GetString(body);

            _rmqConsumer.Ack(ea);

            var jsonContent = Encoding.UTF8.GetBytes(ParseHtmlToJson(htmlContent));

            if (!(jsonContent is null))
            {
                _rmqPublisher.Publish(jsonContent, _exchangeName, _routingKeys);
            }
            else 
            {
                //log message
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

            foreach (var item in _patterns)
            {
                var htmlNodes = htmlDoc.DocumentNode.SelectNodes(item);

                nodeCollectionList.Add(htmlNodes);
            }

            return nodeCollectionList;
        }

        public static string HtmlToJsonParse(List<HtmlNodeCollection> nodeCollectionList, string[] columsNamesArr)
        {
            StringBuilder nodesHtmlString = new StringBuilder();

            for (int i = 0; i < nodeCollectionList.Count; i++)
            {
                int index = 0;

                nodesHtmlString.AppendLine("{");

                foreach (var item in nodeCollectionList)
                {
                    nodesHtmlString.AppendLine($"\"{columsNamesArr[index++]}\" : \"{item[i].InnerText}\"{(index < nodeCollectionList.Count ? ',' : ' ')}");
                }
                nodesHtmlString.AppendLine("}" + $"{(i == nodeCollectionList.Count - 1 ? ' ' : ',')}");
            }

            var nodesHtmlStringPrep = $"{String.Join($"", nodesHtmlString.ToString().Select(ch => ch.Equals('\'') ? "\'\'" : ch.ToString()).ToArray())}";

            return $"N'[\n{nodesHtmlStringPrep}]'";
        }
    }
}
