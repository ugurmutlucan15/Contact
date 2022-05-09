using ReportMicroService.Entities;

using MongoDB.Driver;

namespace ReportMicroService.Data.Interfaces
{
    public interface IReportContext
    {
        IMongoCollection<Report> Report { get; }
    }
}