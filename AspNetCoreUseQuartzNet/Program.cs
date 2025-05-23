using AspNetCoreUseQuartzNet;
using Quartz;
using Quartz.AspNetCore;
using System.ComponentModel;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new DatetimeJsonConverter());
    });

// base configuration from appsettings.json
builder.Services.Configure<QuartzOptions>(builder.Configuration.GetSection("Quartz"));

builder.Services.AddQuartz(options =>
{
    var assembly = Assembly.Load("AspNetCoreUseQuartzNet");
    List<TypeInfo> jobTypes = assembly.DefinedTypes.Where(u => u.IsClass && u.ImplementedInterfaces.Contains(typeof(IJob))).ToList();

    foreach (var jobType in jobTypes)
    {
        string description = string.Empty;
    
        var descriptionAttribute = jobType.GetCustomAttribute<DescriptionAttribute>();
        if (descriptionAttribute != null)
        {
            description = descriptionAttribute.Description;
        }

        var jobKeyInstance = jobType.GetField("Key", BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static);
        var jobKey = jobKeyInstance.GetValue(jobType) as JobKey;

        options.AddJob(jobType, null, c =>
        {
            c.WithDescription(description);
            c.WithIdentity(jobKey);
            c.StoreDurably();
        });
    }

    options.UsePersistentStore(s =>
    {
        s.UseProperties = true;
        s.UseMySqlConnector(mysql =>
        {
            mysql.ConnectionString = "server=127.0.0.1;port=3306;database=quartz;user id=root;password=root;CharacterSet=utf8mb4;SslMode=None;Allow User Variables=true;AllowPublicKeyRetrieval=True";
        });
        //s.UseNewtonsoftJsonSerializer();
        s.UseSystemTextJsonSerializer();
    });
});

builder.Services.AddQuartzServer();

builder.Services.AddHttpClient();

var app = builder.Build();

//app.UseQuartzUIAuthentication();

app.MapControllers();

app.Run();
