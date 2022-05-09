using MongoDB.Driver;

using ReportMicroService.Entities;

namespace ReportMicroService.Data.Interfaces
{
    public interface IReportContext
    {
        IMongoCollection<Report> Report { get; }
    }
}