using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

using System.Net.Http;

namespace xUnitTest
{
    public static class TestClientProvider
    {
        public static HttpClient ClientContact => ContactMicroService.CreateClient();
        public static HttpClient ClientReport => ReportMicroService.CreateClient();

        public static TestServer ContactMicroService { get; }
        public static TestServer ReportMicroService { get; }

        static TestClientProvider()
        {
            var contactServer = new TestServer(new WebHostBuilder().UseStartup<ContactMicroService.Startup>());
            var reportServer = new TestServer(new WebHostBuilder().UseStartup<ReportMicroService.Startup>());
            ContactMicroService = contactServer;
            ReportMicroService = reportServer;
        }
    }
}