namespace ReportMicroService.Settings
{
    public interface IReportDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}