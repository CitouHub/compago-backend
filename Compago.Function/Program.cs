using AutoMapper;
using Compago.Data;
using Compago.Mapping;
using Compago.Service;
using Compago.Service.ExternalSource.GSuite;
using Compago.Service.ExternalSource.MicrosoftAzure;
using Compago.Service.Settings;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = FunctionsApplication.CreateBuilder(args);

var logger = LoggerFactory.Create(config =>
{
    config.AddConsole();
    config.AddConfiguration(builder.Configuration.GetSection("Logging"));
});

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Services.Configure<ExternalSourceSettings.GSuite>(settings =>
{
    settings.Username = Environment.GetEnvironmentVariable("ExternalSourceSettings:GSuite:Username") ?? null!;
    settings.Password = Environment.GetEnvironmentVariable("ExternalSourceSettings:GSuite:Password") ?? null!;
    settings.URL = Environment.GetEnvironmentVariable("ExternalSourceSettings:GSuite:URL") ?? null!;
});

builder.Services.Configure<ExternalSourceSettings.MicrosoftAzure>(settings =>
{
    settings.AccessId = Environment.GetEnvironmentVariable("ExternalSourceSettings:MicrosoftAzure:AccessId") ?? null!;
    settings.InvoiceAPIKey = Environment.GetEnvironmentVariable("ExternalSourceSettings:MicrosoftAzure:InvoiceAPIKey") ?? null!;
    settings.Subscription = Environment.GetEnvironmentVariable("ExternalSourceSettings:MicrosoftAzure:Subscription") ?? null!;
    settings.URL = Environment.GetEnvironmentVariable("ExternalSourceSettings:MicrosoftAzure:URL") ?? null!;
});

builder.Services.Configure<CurrencyServiceSettings.EX>(settings =>
{
    settings.APIKey = Environment.GetEnvironmentVariable("CurrencyServiceSettings:EX:APIKey") ?? null!;
    settings.URL = Environment.GetEnvironmentVariable("CurrencyServiceSettings:EX:URL") ?? null!;
});

builder.Services.AddDbContext<CompagoDbContext>(options =>
{
    options.UseSqlServer(Environment.GetEnvironmentVariable("Database:ConnectionString"));
});

var mappingConfig = new MapperConfiguration(config =>
{
    config.AddProfiles([
        new GSuiteProfile(),
        new InternalProfile(),
        new MicrosoftAzureProfile()
    ]);
}, logger);
var mapper = mappingConfig.CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddSingleton<ICacheService, CacheService>();

builder.Services.AddScoped<IExternalSourceService, ExternalSourceService>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<IGSuiteService, GSuiteService>();
builder.Services.AddScoped<IMicrosoftAzureService, MicrosoftAzureService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IInvoiceTagService, InvoiceTagService>();

builder.Build().Run();
