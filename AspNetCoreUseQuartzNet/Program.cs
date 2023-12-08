using AspNetCoreUseQuartzNet.Jobs;
using CrystalQuartz.AspNetCore;
using Quartz;
using Quartz.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// base configuration from appsettings.json
builder.Services.Configure<QuartzOptions>(builder.Configuration.GetSection("Quartz"));

builder.Services.AddQuartz(options =>
{
    options.AddJob<HelloJob>(c =>
    {
        c.WithDescription("Hello");
        c.WithIdentity(HelloJob.Key);
        c.StoreDurably();
    });
    options.UsePersistentStore(s =>
    {
        s.UseMySqlConnector(mysql =>
        {
            mysql.ConnectionString = "server=127.0.0.1;port=3306;database=quartz;user id=root;password=root;CharacterSet=utf8mb4;SslMode=None;Allow User Variables=true;";
        });
        s.UseNewtonsoftJsonSerializer();
    });
});

builder.Services.AddQuartzServer();

var app = builder.Build();

var scheduler = await app.Services.GetRequiredService<ISchedulerFactory>().GetScheduler();
app.UseCrystalQuartz(() => scheduler);

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
