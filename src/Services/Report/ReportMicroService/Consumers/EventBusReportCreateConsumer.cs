using AutoMapper;

using EventBusRabbitMQ;
using EventBusRabbitMQ.Contact;
using EventBusRabbitMQ.Core;
using EventBusRabbitMQ.Events;

using Newtonsoft.Json;

using OfficeOpenXml;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using ReportMicroService.Entities;
using ReportMicroService.Models;
using ReportMicroService.Repositories.Interfaces;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ReportMicroService.Consumers
{
    public class EventBusReportCreateConsumer
    {
        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly IMapper _mapper;
        private readonly IReportRepository _reportRepository;
        private readonly EventBusRabbitMQContact _eventBus;

        public EventBusReportCreateConsumer(IRabbitMQPersistentConnection persistentConnection, IMapper mapper, IReportRepository reportRepository, EventBusRabbitMQContact eventBus)
        {
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _reportRepository = reportRepository;
            _eventBus = eventBus;
        }

        public void Consume()
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var channel = _persistentConnection.CreateModel();
            channel.QueueDeclare(queue: EventBusConstants.ReportComplateQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += ReceivedEvent;

            channel.BasicConsume(queue: EventBusConstants.ReportComplateQueue, autoAck: true, consumer: consumer);
        }

        private async void ReceivedEvent(object sender, BasicDeliverEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Body.Span);
            var @event = JsonConvert.DeserializeObject<ReportCreateEvent>(message);

            if (e.RoutingKey == EventBusConstants.ReportComplateQueue)
            {
                var data = JsonConvert.DeserializeObject<List<ReportDetailModel>>(@event!.Data.ToString()!);

                //var path = Directory.GetCurrentDirectory();
                var filename = $"{Guid.NewGuid()}_Report.xlsx";
                var filePath = Path.Combine("Export", filename);

                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    package.Workbook.Worksheets.Add("Report");

                    package.Workbook.Worksheets[0].Cells["A1"].LoadFromCollection<ReportDetailModel>(data, true, OfficeOpenXml.Table.TableStyles.None);
                    await package.SaveAsync();
                }

                var rep = _mapper.Map<Report>(@event);
                rep.ReportStatus = ReportStatus.Completed;
                rep.FilePath = filePath;
                await _reportRepository.Update(rep);
            }
        }

        public void Disconnect()
        {
            _persistentConnection.Dispose();
        }
    }
}