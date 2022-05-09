using EventBusRabbitMQ.Events;

using MongoDB.Bson.Serialization.Attributes;

using System;

namespace ReportMicroService.Entities
{
    public class Report
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }

        public DateTime CreationDate { get; set; }

        public ReportStatus ReportStatus { get; set; }

        public string FilePath { get; set; }
    }
}