using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ContactMicroService.Entities
{
    public enum ContactType
    {
        PhoneNumber,
        Email,
        Location
    }

    public class ContactDetail
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }

        public ContactType ContactType { get; set; }

        public string ContactValue { get; set; }

        public ContactDetail()
        {
            this.Id = ObjectId.GenerateNewId().ToString();
        }
    }
}