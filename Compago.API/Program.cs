using Asp.Versioning;
using AutoMapper;
using Compago.API.ExceptionHandling;
using Compago.API.SwaggerConfig;
using Compago.Data;
using Compago.Mapping;
using Compago.Service;
using Compago.Service.ExternalSource.GSuite;
using Compago.Service.ExternalSource.MicrosoftAzure;
using Compago.Service.Settings;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var logger = LoggerFactory.Create(config =>
{
    config.AddConsole();
    config.AddConfiguration(builder.Configuration.GetSection("Logging"));
});

builder.Services.AddControllers();
builder.Services.AddApiVersioning(opt =>
{
    opt.DefaultApiVersion = new ApiVersion(1, 0);
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.ReportApiVersions = true;
    opt.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
                                                    new HeaderApiVersionReader("x-api-version"),
                                                    new MediaTypeApiVersionReader("x-api-version"));
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(_ => _.SchemaFilter<EnumSchemaFilter>());
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddMvc()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Host.ConfigureServices((context, services) =>
{
    services.Configure<ExternalSourceSettings.GSuite>(context.Configuration.GetSection("ExternalSourceSettings:GSuite"));
    services.Configure<ExternalSourceSettings.MicrosoftAzure>(context.Configuration.GetSection("ExternalSourceSettings:MicrosoftAzure"));
    services.Configure<CurrencyServiceSettings.EX>(context.Configuration.GetSection("CurrencyServiceSettings:EX"));
});

builder.Services.AddDbContext<CompagoDbContext>(options =>
{
    options.UseSqlServer(configuration.GetValue<string>("Database:ConnectionString"));
});

var mappingConfig = new MapperConfiguration(config =>
{
    config.AddProfiles([
        new GSuiteProfile(), 
        new MicrosoftAzureProfile(),
        new InternalProfile()
    ]);
}, logger);
var mapper = mappingConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddScoped<IDelegateService, DelegateService>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<IGSuiteService, GSuiteService>();
builder.Services.AddScoped<IMicrosoftAzureService, MicrosoftAzureService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IInvoiceTagService, InvoiceTagService>();

var app = builder.Build();

app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.MapOpenApi();

app.UseSwagger();

app.UseSwaggerUI();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }