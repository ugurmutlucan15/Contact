using System;
using System.Text;

using AutoMapper;

using ContactMicroService.Repositories.Interfaces;

using EventBusRabbitMQ;
using EventBusRabbitMQ.Contact;
using EventBusRabbitMQ.Core;
using EventBusRabbitMQ.Events;
using EventBusRabbitMQ.Events.Interfaces;

using MediatR;

using Newtonsoft.Json;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ContactMicroService.Consumers
{
    public class EventBusReportCreateConsumer
    {
        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly IMapper _mapper;
        private readonly IContactRepository _contactRepository;
        private readonly EventBusRabbitMQContact _eventBus;

        public EventBusReportCreateConsumer(IRabbitMQPersistentConnection persistentConnection, IMapper mapper, IContactRepository contactRepository, EventBusRabbitMQContact eventBus)
        {
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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
                catch (Exception ex)
                {
                    // _logger.LogError(e,"Error Publishing integration event:{EventId} from {AppName}",eventMessage.Id,"Report");
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