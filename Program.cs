using System.Collections.Immutable;
using System.Data;
using Hangfire;
using Hangfire.Storage.SQLite;
using Microsoft.EntityFrameworkCore;
using SecurityQuestionAuthAPI;
using SecurityQuestionAuthAPI.Services;




var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connect = builder.Configuration.GetConnectionString("MyConnection");
builder.Services.AddDbContext<SecurityAuthContext>(options => options.UseMySql(connect, ServerVersion.AutoDetect(connect)));
builder.Services.AddScoped<UserAuthServices>();
builder.Services.AddScoped<IBackgroundJobClient, BackgroundJobClient>();



builder.Services.AddHangfire(config => config
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSQLiteStorage(builder.Configuration.GetConnectionString("SqlLiteConnection")));

builder.Services.AddHangfireServer();

// GlobalConfiguration.Configuration.UseStorage(
//     new MySqlStorage(connectionString));



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}



app.UseHangfireServer();

app.UseHangfireDashboard();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseHangfireDashboard();

app.MapHangfireDashboard("/hangfire");

app.Run();
