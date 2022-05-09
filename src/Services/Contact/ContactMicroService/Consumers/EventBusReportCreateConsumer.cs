using ContactMicroService.Repositories.Interfaces;

using EventBusRabbitMQ;
using EventBusRabbitMQ.Contact;
using EventBusRabbitMQ.Core;
using EventBusRabbitMQ.Events;

using Newtonsoft.Json;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using System;
using System.Text;

namespace ContactMicroService.Consumers
{
    public class EventBusReportCreateConsumer
    {
        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly IContactRepository _contactRepository;
        private readonly EventBusRabbitMQContact _eventBus;

        public EventBusReportCreateConsumer(IRabbitMQPersistentConnection persistentConnection, IContactRepository contactRepository, EventBusRabbitMQContact eventBus)
        {
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _contactRepository = contactRepository;
            _eventBus = eventBus;
        }

        public void Consume()
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var channel = _persistentConnection.CreateModel();
            channel.QueueDeclare(queue: EventBusConstants.ReportCreateQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += ReceivedEvent;

            channel.BasicConsume(queue: EventBusConstants.ReportCreateQueue, autoAck: true, consumer: consumer);
        }

        private async void ReceivedEvent(object sender, BasicDeliverEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Body.Span);
            var @event = JsonConvert.DeserializeObject<ReportCreateEvent>(message);

            if (e.RoutingKey == EventBusConstants.ReportCreateQueue)
            {
                var resdata = await _contactRepository.GetReport();
                @event.Data = resdata;
                try
                {
                    _eventBus.Publish(EventBusConstants.ReportComplateQueue, @event);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public void Disconnect()
        {
            _persistentConnection.Dispose();
        }
    }
}