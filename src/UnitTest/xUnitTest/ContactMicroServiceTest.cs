using ContactMicroService.Entities;
using ContactMicroService.Repositories.Interfaces;

using Microsoft.Extensions.DependencyInjection;

using System.Collections.Generic;
using System.Linq;

using Xunit;

namespace xUnitTest
{
    public class ContactMicroServiceTest
    {
        private readonly IContactRepository _contactRepository;

        public ContactMicroServiceTest()
        {
            _contactRepository = TestClientProvider.ContactMicroService.Services.GetRequiredService<IContactRepository>();
        }

        [Fact]
        public async void Gets()
        {
            var res = await _contactRepository.GetContacts();
            Assert.NotNull(res);
        }

        [Fact]
        public async void Get()
        {
            var res = await _contactRepository.GetContacts();

            var enumerable = res.ToList();
            if (!enumerable.Any())
            {
                Assert.True(false);
            }

            var resFirst = await _contactRepository.GetContact(enumerable.FirstOrDefault()?.Id);
            Assert.NotNull(resFirst);
        }

        [Fact]
        public async void Create()
        {
            var model = new Contact
            {
                Company = "Test Firması",
                FirstName = "Test FirstName",
                LastName = "Test LastName",
                ContactDetails = new List<ContactDetail>
                {
                    new ContactDetail()
                    {
                        ContactType = ContactType.Location,
                        ContactValue = "BURDUR"
                    },
                    new ContactDetail
                    {
                        ContactType = ContactType.Email,
                        ContactValue = "ugurmutlucan15@hotmail.com"
                    },
                    new ContactDetail
                    {
                        ContactType = ContactType.PhoneNumber,
                        ContactValue = "5301151700"
                    }
                }
            };
            var res = await _contactRepository.Create(model);
            Assert.NotNull(res.Id);
        }

        [Fact]
        public async void Update()
        {
            var res = await _contactRepository.GetContacts();

            var model = res.FirstOrDefault();
            if (model != null)
            {
                model.Company = "test";

                var data = await _contactRepository.Update(model);
                Assert.True(data);
            }
            else
            {
                Assert.True(false);
            }
        }

        [Fact]
        public async void Delete()
        {
            //Metodun Çalışması için en az 1 kayıt olması gerekir.
            var contacts = await _contactRepository.GetContacts();
            var contactFirst = contacts.FirstOrDefault();

            if (contactFirst == null)
                Assert.True(false);

            var res = await _contactRepository.Delete(contactFirst.Id);
            Assert.True(res);
        }

        [Fact]
        public async void CreateDetail()
        {
            //Metodun Çalışması için en az 1 kayıt olması gerekir.
            var contacts = await _contactRepository.GetContacts();
            var contactFirst = contacts.FirstOrDefault();
            var model = new ContactDetail
            {
                ContactType = ContactType.Email,
                ContactValue = "ugurmutlucan15@hotmail.com",
            };

            if (contactFirst == null)
                Assert.True(false);

            var res = await _contactRepository.CreateDetail(model, contactFirst.Id);
            Assert.True(res);
        }

        [Fact]
        public async void UpdateDetail()
        {
            //Metodun Çalışması için en az 1 kayıt olması gerekir.
            var contacts = await _contactRepository.GetContacts();
            var contactFirst = contacts.FirstOrDefault();
            if (contactFirst != null)
            {
                var model = contactFirst.ContactDetails.FirstOrDefault();
                if (model != null)
                {
                    model.ContactValue = "test";

                    if (contactFirst == null)
                        Assert.True(false);

                    var res = await _contactRepository.CreateDetail(model, contactFirst.Id);
                    Assert.True(res);
                }
            }
            else
            {
                Assert.True(false);
            }
        }

        [Fact]
        public async void DeleteDetail()
        {
            //Metodun Çalışması için en az 1 kayıt olması gerekir.
            var contacts = await _contactRepository.GetContacts();
            var contactFirst = contacts.FirstOrDefault();

            if (contactFirst == null)
                Assert.True(false);

            var detailId = contactFirst.ContactDetails.FirstOrDefault()?.Id;

            if (detailId == null)
                Assert.True(false);

            var res = await _contactRepository.DeleteDetail(detailId);
            Assert.True(res);
        }
    }
}