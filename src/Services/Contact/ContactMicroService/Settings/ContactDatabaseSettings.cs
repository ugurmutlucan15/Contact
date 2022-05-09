namespace ContactMicroService.Settings
{
    public class ContactDatabaseSettings : IContactDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}