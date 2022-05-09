using ContactMicroService.Data.Interfaces;
using ContactMicroService.Entities;
using ContactMicroService.Models;
using ContactMicroService.Repositories.Interfaces;

using MongoDB.Driver;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ContactMicroService.Repositories
{
    public class ContactRepository : IContactRepository
    {
        private readonly IContactContext _ctx;

        public ContactRepository(IContactContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<IEnumerable<Contact>> GetContacts(Expression<Func<Contact, bool>> filter = null)
        {
            if (filter != null)
            {
                return await _ctx.Contanct.Find(filter).ToListAsync();
            }

            return await _ctx.Contanct.Find(m => true).ToListAsync();
        }

        public async Task<List<ReporDetailtModel>> GetReport()
        {
            var filter = Builders<Contact>.Filter.Eq("ContactDetails.ContactType", (int)ContactType.Location);
            var contact = await _ctx.Contanct.Find(filter).ToListAsync();
            var res = contact.SelectMany(sm => sm.ContactDetails).Where(w => w.ContactType == ContactType.Location).GroupBy(g => g.ContactValue).Select(s =>
                    new ReporDetailtModel
                    {
                        Location = s.FirstOrDefault().ContactValue,
                        ContactCount = contact
                            .Where(w => w.ContactDetails.Exists(e => e.ContactValue == s.FirstOrDefault().ContactValue)).Count(),
                        ContactPhoneCount = contact.Where(w => w.ContactDetails.Exists(e => e.ContactValue == s.FirstOrDefault().ContactValue)).SelectMany(s => s.ContactDetails).Where(w => w.ContactType == ContactType.PhoneNumber).Count(),
                    }
            ).ToList();

            return res;
        }

        public async Task<Contact> GetContact(string id)
        {
            return await _ctx.Contanct.Find(m => m.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Contact> Create(Contact model)
        {
            await _ctx.Contanct.InsertOneAsync(model);
            return model;
        }

        public async Task<bool> Update(Contact model)
        {
            var updateResult = await _ctx.Contanct.ReplaceOneAsync(filter: g => g.Id == model.Id, replacement: model);
            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }

        public async Task<bool> Delete(string id)
        {
            var filter = Builders<Contact>.Filter.Eq(m => m.Id, id);
            var deleteResult = await _ctx.Contanct.DeleteOneAsync(filter);
            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

        public async Task<bool> CreateDetail(ContactDetail contactDetail, string contactId)
        {
            var contact = await _ctx.Contanct.Find(m => m.Id == contactId).FirstOrDefaultAsync();
            contact.ContactDetails.Add(contactDetail);
            var updateResult =
                await _ctx.Contanct.ReplaceOneAsync(filter: g => g.Id == contact.Id, replacement: contact);
            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }

        public async Task<bool> UpdateDetail(ContactDetail contactDetail)
        {
            var filter = Builders<Contact>.Filter.Eq("ContactDetails.Id", contactDetail.Id);
            var contact = await _ctx.Contanct.Find(filter).FirstOrDefaultAsync();
            if (contact == null)
                return false;

            var contactDetailEntity = contact.ContactDetails.FirstOrDefault(m => m.Id == contactDetail.Id);
            if (contactDetailEntity == null)
                return false;

            contactDetailEntity.ContactType = contactDetail.ContactType;
            contactDetailEntity.ContactValue = contactDetail.ContactValue;

            var updateResult =
                await _ctx.Contanct.ReplaceOneAsync(filter: g => g.Id == contact.Id, replacement: contact);
            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }

        public async Task<bool> DeleteDetail(string id)
        {
            var filter = Builders<Contact>.Filter.Eq("ContactDetails.Id", id);
            var contact = await _ctx.Contanct.Find(filter).FirstOrDefaultAsync();
            if (contact == null)
                return false;

            var contactDetails = contact.ContactDetails.Where(m => m.Id != id).ToList();
            contact.ContactDetails = contactDetails;

            var updateResult =
                await _ctx.Contanct.ReplaceOneAsync(filter: g => g.Id == contact.Id, replacement: contact);
            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }
    }
}