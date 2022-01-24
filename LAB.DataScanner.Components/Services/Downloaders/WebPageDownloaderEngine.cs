using LAB.DataScanner.Components.Services.MessageBroker.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LAB.DataScanner.Components.Services.Downloaders
{
    public class WebPageDownloaderEngine
    {
        private readonly IConfigurationSection _bindingConfiguration;

        private readonly IDataRetriever _dataRetriever;

        private readonly IRmqPublisher _rmqPublisher;

        private readonly IRmqConsumer _rmqConsumer;

        private readonly string[] _routingKeys;

        private readonly string _exchangeName;

        private readonly UrlsValidator _urlsValidator;

        private ConcurrentStack<Task<string>> _htmlContentConcurrentStack;

        public WebPageDownloaderEngine(IConfigurationSection bindingConfiguration,
            IDataRetriever dataRetriever, IRmqPublisher rmqPublisher, 
            IRmqConsumer rmqConsumer, UrlsValidator urlsValidator)
        {
            _bindingConfiguration = bindingConfiguration;

            _dataRetriever = dataRetriever;

            _rmqPublisher = rmqPublisher;

            _rmqConsumer = rmqConsumer;

            _urlsValidator = urlsValidator;

            _htmlContentConcurrentStack = new ConcurrentStack<Task<string>>();

            _routingKeys = JsonConvert.DeserializeObject<string[]>(_bindingConfiguration
                .GetSection("SenderRoutingKeys").Value ?? "");

            _exchangeName = _bindingConfiguration.GetSection("SenderExchange").Value ?? "";
        }

        public void Start()
        {
            _rmqConsumer.StartListening(OnReceive);

            Timer timer = new Timer(TimerCallBack, 0, 0, 15000);

            Console.ReadKey();
        }

        public void OnReceive(object model, BasicDeliverEventArgs ea) 
        {
            var body = ea.Body.ToArray();

            var url = Encoding.UTF8.GetString(body);

            var routingKey = ea.RoutingKey;

            _rmqConsumer.Ack(ea);

            Console.WriteLine("[x] Received '{0}':'{1}'", routingKey, url);

            if (_urlsValidator.UrlIsValid(url))
            {
                _htmlContentConcurrentStack.Push(_dataRetriever.RetrieveStringAsync(url));
            }     
        }

        private void TimerCallBack(object obj) 
        {
            Console.WriteLine("***Callback just has been called***");

            while (_htmlContentConcurrentStack.Count > 0)
            {
                if (_htmlContentConcurrentStack.TryPop(out Task<string> htmlContent))
                {
                    var byteArrHtmlContent = Encoding.UTF8.GetBytes(htmlContent.Result);

                    _rmqPublisher.Publish(byteArrHtmlContent, _exchangeName, _routingKeys);
                }
            }
        }
    }
}
