using ReportMicroService.Data.Interfaces;
using ReportMicroService.Entities;
using ReportMicroService.Settings;
using MongoDB.Driver;

namespace ReportMicroService.Data
{
    public class ReportContext : IReportContext
    {
        public ReportContext(IReportDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            Report = database.GetCollection<Report>(nameof(Report));
        }

        public IMongoCollection<Report> Report { get; }

    }
}