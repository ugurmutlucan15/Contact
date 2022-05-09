using ContactMicroService.Entities;
using ContactMicroService.Models;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ContactMicroService.Repositories.Interfaces
{
    public interface IContactRepository
    {
        Task<IEnumerable<Contact>> GetContacts(Expression<Func<Contact, bool>> filter = null);

        Task<List<ReporDetailtModel>> GetReport();

        Task<Contact> GetContact(string id);

        Task<Contact> Create(Contact model);

        Task<bool> Update(Contact model);

        Task<bool> Delete(string id);

        Task<bool> CreateDetail(ContactDetail contactDetail, string contactId);

        Task<bool> UpdateDetail(ContactDetail contactDetail);

        Task<bool> DeleteDetail(string id);
    }
}