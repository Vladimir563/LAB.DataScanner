using HtmlAgilityPack;
using Jsonize;
using Jsonize.Parser;
using Jsonize.Serializer.Json.Net;
using LAB.DataScanner.Components.Services.MessageBroker.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using System;
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
        }

        public void Start() 
        {
            _rmqConsumer.StartListening(OnReceive);
        }

        public string ParseHtmlToJson(string htmlContent) 
        {
            string htmlNodesString = string.Empty;

            var htmlDoc = new HtmlDocument();

            htmlDoc.LoadHtml(htmlContent);

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
                var nodes = htmlDoc.DocumentNode.SelectNodes($"{htmlFragmentExpression}");

                htmlNodesString = $"{String.Join($"\n\n", nodes.Select(p => p.OuterHtml).ToArray())}";
            }
            else
            {
                htmlNodesString = htmlDoc.DocumentNode.SelectSingleNode($"{htmlFragmentExpression}").OuterHtml;
            }

            var jsonContent = GetJsonFromHtmlString(htmlNodesString);

            Console.WriteLine(jsonContent);

            return jsonContent;
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

        private string GetJsonFromHtmlString(string htmlContent) 
        {
            JsonizeParser parser = new JsonizeParser();

            JsonizeSerializer serializer = new JsonizeSerializer();

            Jsonizer jsonizer = new Jsonizer(parser, serializer);

            return jsonizer.ParseToStringAsync(htmlContent).Result;
        }
    }
}
