using ContactMicroService.Entities;
using ContactMicroService.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace xUnitTest
{
    public class ResponseTest
    {
        private readonly IConfiguration _configuration;

        public ResponseTest()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("Data.json", false, false)
                .AddEnvironmentVariables()
                .Build();
        }

        #region Contact

        [Fact]
        public async void SeetData()
        {
            var client = TestClientProvider.ClientContact;

            var city = _configuration.GetSection("City").Get<string[]>();
            var firtsName = _configuration.GetSection("FirstName").Get<string[]>();
            var companies = _configuration.GetSection("Companies").Get<string[]>();
            for (int i = 0; i < 5000; i++)
            {
                var data = new Contact
                {
                    FirstName = firtsName[new Random().Next(0, firtsName.Length)],
                    LastName = firtsName[new Random().Next(0, firtsName.Length)],
                    Company = companies[new Random().Next(0, companies.Length)],
                };

                var randomnumberlist = new List<int>();
                for (int ii = 0; ii < 4; ii++)
                {
                    var randomCount = new Random().Next(1, 4);
                    if (randomnumberlist.Any(m => m == randomCount) || randomCount == 4)
                        continue;
                    else
                        randomnumberlist.Add(randomCount);

                    switch (randomCount)
                    {
                        case 1:
                            data.ContactDetails.Add(new ContactDetail
                            {
                                ContactType = ContactType.Email,
                                ContactValue = data.FirstName + data.LastName + "@testmail.com",
                            });
                            break;
                        case 2:
                            data.ContactDetails.Add(new ContactDetail
                            {
                                ContactType = ContactType.Location,
                                ContactValue = city[new Random().Next(0, city.Length)],
                            });
                            break;
                        case 3:
                            data.ContactDetails.Add(new ContactDetail
                            {
                                ContactType = ContactType.PhoneNumber,
                                ContactValue = new Random().Next(500000000, 900000000).ToString(),
                            });
                            break;
                    }
                }

                //Act
                var res = await client.PostAsync("api/v1/Contact/CreateContact",
                    new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));
                res.EnsureSuccessStatusCode();
            }

            //Assert
            Assert.Equal("1", "1");
        }

        [Fact]
        public async void GetContacts()
        {
            var client = TestClientProvider.ClientContact;

            //Act
            var res = await client.GetAsync("api/v1/Contact/GetContacts");
            res.EnsureSuccessStatusCode();

            //Assert
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        }

        [Fact]
        public async void GetContact()
        {
            //metodun çalışabilmesi için kayıt olması gerekir
            var resList = await TestClientProvider.ContactMicroService.Services.GetRequiredService<IContactRepository>()
                .GetContacts();
            var first = resList.FirstOrDefault();
            var client = TestClientProvider.ClientContact;
            //Act
            if (first != null)
            {
                var res = await client.GetAsync($"api/v1/Contact/GetContact/{first.Id}");
                res.EnsureSuccessStatusCode();

                //Assert
                Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            }
            else
            {
                Assert.True(false);
            }
        }

        [Fact]
        public async void CreateContactDetail()
        {
            var resList = await TestClientProvider.ContactMicroService.Services.GetRequiredService<IContactRepository>()
                .GetContacts();
            var first = resList.FirstOrDefault();

            var client = TestClientProvider.ClientContact;
            if (first != null)
            {
                var res = await client.PostAsync($"api/v1/Contact/CreateContactDetail/{first.Id}", new StringContent(
                    JsonConvert.SerializeObject(new ContactDetail
                    {
                        ContactType = ContactType.Location,
                        ContactValue = "KONYA",
                    }), Encoding.UTF8, "application/json"));

                Assert.Equal(HttpStatusCode.Created, res.StatusCode);
            }
            else
            {
                Assert.True(false);
            }
        }

        [Fact]
        public async void UpdateContactDetail()
        {
            var resList = await TestClientProvider.ContactMicroService.Services.GetRequiredService<IContactRepository>()
                .GetContacts();
            var first = resList.FirstOrDefault();
            if (first != null)
            {
                var model = first.ContactDetails.FirstOrDefault();
                if (model != null)
                {
                    model.ContactValue = "Test";

                    var client = TestClientProvider.ClientContact;
                    var res = await client.PutAsync($"api/v1/Contact/UpdateContactDetail", new StringContent(
                        JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));

                    Assert.Equal(HttpStatusCode.OK, res.StatusCode);
                }
                else
                {
                    Assert.True(false);
                }
            }
            else
            {
                Assert.True(false);
            }
        }

        [Fact]
        public async void DeleteContactDetail()
        {
            var resList = await TestClientProvider.ContactMicroService.Services.GetRequiredService<IContactRepository>()
                .GetContacts();
            var first = resList.FirstOrDefault();
            if (first != null)
            {
                var model = first.ContactDetails.FirstOrDefault();

                var client = TestClientProvider.ClientContact;
                if (model != null)
                {
                    var res = await client.DeleteAsync($"api/v1/Contact/DeleteContactDetailById/{model.Id}");

                    Assert.Equal(HttpStatusCode.OK, res.StatusCode);
                }
                else
                {
                    Assert.True(false);
                }
            }
            else
            {
                Assert.True(false);
            }
        }

        #endregion

        #region Report

        [Fact]
        public async void CreateReport()
        {
            var client = TestClientProvider.ClientReport;

            //Act
            var res = await client.GetAsync("api/v1/Report/GetReportCreate");
            res.EnsureSuccessStatusCode();

            //Assert
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        }

        [Fact]
        public async void GetReports()
        {
            var client = TestClientProvider.ClientReport;

            //Act
            var res = await client.GetAsync("api/v1/Report/GetReports");
            res.EnsureSuccessStatusCode();

            //Assert
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        }

        #endregion
    }
}