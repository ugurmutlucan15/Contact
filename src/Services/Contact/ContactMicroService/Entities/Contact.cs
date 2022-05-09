using MongoDB.Bson.Serialization.Attributes;

using System.Collections.Generic;

namespace ContactMicroService.Entities
{
    public class Contact
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public List<ContactDetail> ContactDetails { get; set; }

        public Contact()
        {
            ContactDetails = new List<ContactDetail>();
        }
    }
}