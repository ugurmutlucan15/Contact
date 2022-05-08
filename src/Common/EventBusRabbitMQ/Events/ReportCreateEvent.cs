using EventBusRabbitMQ.Events.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBusRabbitMQ.Events
{
    public enum ReportStatus
    {
        Preparing,
        Completed
    }

    public class ReportCreateEvent : IEvent
    {
        public string Id { get; set; }

        public DateTime CreationDate { get; set; }

        public ReportStatus ReportStatus { get; set; }

    }
}