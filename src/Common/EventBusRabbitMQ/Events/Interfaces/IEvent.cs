using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBusRabbitMQ.Events.Interfaces
{
    public enum Status
    {
        Preparing,
        Completed
    }

    public abstract class IEvent
    {
        public Guid RequestId { get; private init; }
        public DateTime CrateDate { get; private init; }

        public Status Status { get; set; }

        public object Data { get; set; }

        public IEvent()
        {
            RequestId = Guid.NewGuid();
            CrateDate = DateTime.UtcNow;
        }
    }
}