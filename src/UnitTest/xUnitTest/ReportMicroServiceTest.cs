using EventBusRabbitMQ.Events;

using Microsoft.Extensions.DependencyInjection;

using ReportMicroService.Entities;
using ReportMicroService.Repositories.Interfaces;

using System;
using System.Linq;

using Xunit;

namespace xUnitTest
{
    public class ReportMicroServiceTest
    {
        private readonly IReportRepository _repository;

        public ReportMicroServiceTest()
        {
            _repository = TestClientProvider.ReportMicroService.Services.GetRequiredService<IReportRepository>();
        }

        [Fact]
        public async void Gets()
        {
            var res = await _repository.GetReports();
            Assert.NotNull(res);
        }

        [Fact]
        public async void Create()
        {
            var model = new Report
            {
                CreationDate = DateTime.Now,
                ReportStatus = ReportStatus.Preparing,
            };

            var res = await _repository.Create(model);

            Assert.NotNull(res.Id);
        }

        [Fact]
        public async void Update()
        {
            var resList = await _repository.GetReports();
            if (!resList.Any())
            {
                Assert.True(false);
            }

            var lastModel = resList.LastOrDefault();
            if (lastModel != null)
            {
                lastModel.FilePath = "Test";

                var res = await _repository.Update(lastModel);

                Assert.True(res);
            }
            else
            {
                Assert.True(false);
            }
        }
    }
}