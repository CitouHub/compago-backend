using Compago.Data;
using Compago.Service;
using Compago.Service.ExternalSource.GSuite;
using Compago.Service.ExternalSource.MicrosoftAzure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using NSubstitute;

namespace Compago.Test.API
{
    public class CompagoAPIMock : WebApplicationFactory<Program>
    {
        private readonly Dictionary<string, string?> _inMemoryConfiguration = [];

        public readonly IDelegateService MockDelegateService = Substitute.For<IDelegateService>();
        public readonly ICurrencyService MockCurrencyService = Substitute.For<ICurrencyService>();
        public readonly IGSuiteService MockGSuiteService = Substitute.For<IGSuiteService>();
        public readonly IMicrosoftAzureService MockMicrosoftAzureService = Substitute.For<IMicrosoftAzureService>();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(_ => _.AddInMemoryCollection(_inMemoryConfiguration));
            builder.ConfigureServices(services =>
            {
                SetMemeoryDatabase(services);

                services.AddControllers();

                services.Remove(new ServiceDescriptor(typeof(IDelegateService), ServiceLifetime.Scoped));
                services.Remove(new ServiceDescriptor(typeof(ICurrencyService), ServiceLifetime.Scoped));
                services.Remove(new ServiceDescriptor(typeof(IGSuiteService), ServiceLifetime.Scoped));
                services.Remove(new ServiceDescriptor(typeof(IMicrosoftAzureService), ServiceLifetime.Scoped));

                services.AddScoped(_ => MockDelegateService);
                services.AddScoped(_ => MockCurrencyService);
                services.AddScoped(_ => MockGSuiteService);
                services.AddScoped(_ => MockMicrosoftAzureService);
            });
        }

        private void SetMemeoryDatabase(IServiceCollection services)
        {
            var datebaseRelatedServices = services.Where(_ =>
                _.ServiceType.Name.Contains(nameof(CompagoDbContext)) ||
                _.ServiceType.Name.Contains(nameof(DbContextOptions))
            ).ToList();
            datebaseRelatedServices.ForEach(_ =>
            {
                services.Remove(_);
            });

            services.AddDbContext<CompagoDbContext>(options =>
            {
                options
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .EnableSensitiveDataLogging();
            });
        }

        public void UpdateConfiguration(string key, string value)
        {
            _inMemoryConfiguration.Add(key, value);
        }
    }
}