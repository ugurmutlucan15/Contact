using ContactMicroService.Data.Interfaces;
using ContactMicroService.Entities;
using ContactMicroService.Settings;
using MongoDB.Driver;

namespace ContactMicroService.Data
{
    public class ContactContext : IContactContext
    {
        public ContactContext(IContactDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            Contanct = database.GetCollection<Contact>(nameof(Contact));
        }

        public IMongoCollection<Contact> Contanct { get; }
    }
}