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
using NSubstitute;

namespace Compago.Test.API
{
    public class CompagoAPIMock() : WebApplicationFactory<Program>
    {
        private readonly Dictionary<string, string?> _inMemoryConfiguration = [];

        public readonly IExternalSourceService MockExternalSourceService = Substitute.For<IExternalSourceService>();
        public readonly ICurrencyService MockCurrencyService = Substitute.For<ICurrencyService>();
        public readonly IGSuiteService MockGSuiteService = Substitute.For<IGSuiteService>();
        public readonly IMicrosoftAzureService MockMicrosoftAzureService = Substitute.For<IMicrosoftAzureService>();
        public readonly IUserService MockUserService = Substitute.For<IUserService>();
        public readonly ITagService MockTagService = Substitute.For<ITagService>();
        public readonly IInvoiceTagService MockInvoiceTagService = Substitute.For<IInvoiceTagService>();

        private readonly string AuthorizationActiveKey = "Settings:AuthorizationActive";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(_ => _.AddInMemoryCollection(_inMemoryConfiguration));
            builder.ConfigureServices(services =>
            {
                SetMemeoryDatabase(services);

                services.AddControllers();

                services.Remove(new ServiceDescriptor(typeof(IExternalSourceService), ServiceLifetime.Scoped));
                services.Remove(new ServiceDescriptor(typeof(ICurrencyService), ServiceLifetime.Scoped));
                services.Remove(new ServiceDescriptor(typeof(IGSuiteService), ServiceLifetime.Scoped));
                services.Remove(new ServiceDescriptor(typeof(IMicrosoftAzureService), ServiceLifetime.Scoped));
                services.Remove(new ServiceDescriptor(typeof(IUserService), ServiceLifetime.Scoped));
                services.Remove(new ServiceDescriptor(typeof(ITagService), ServiceLifetime.Scoped));
                services.Remove(new ServiceDescriptor(typeof(IInvoiceTagService), ServiceLifetime.Scoped));

                services.AddScoped(_ => MockExternalSourceService);
                services.AddScoped(_ => MockCurrencyService);
                services.AddScoped(_ => MockGSuiteService);
                services.AddScoped(_ => MockMicrosoftAzureService);
                services.AddScoped(_ => MockUserService);
                services.AddScoped(_ => MockTagService);
                services.AddScoped(_ => MockInvoiceTagService);
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

        public void SetAuthorizationActive(bool active)
        {
            UpdateConfiguration(AuthorizationActiveKey, active == true ? "true" : "false");
        }

        public void UpdateConfiguration(string key, string value)
        {
            var settingExists = _inMemoryConfiguration.ContainsKey(key);
            if (settingExists == true)
            {
                _inMemoryConfiguration.Remove(key);
            }
            _inMemoryConfiguration.Add(key, value);
        }
    }
}