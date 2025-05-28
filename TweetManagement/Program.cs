using Microsoft.AspNetCore.Authentication.JwtBearer; // Add this using directive
using TweetManagement.Repositories;
using TweetManagement;
using Prometheus;
using Backend.Tests.TweetManagement.Interfaces;
using TweetManagement.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "https://dev-2zn6n2l3.us.auth0.com";
        options.Audience = "https://dev-2zn6n2l3.us.auth0.com/api/v2/"; 
    });

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>();

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.AddScoped<ITweetRepository, TweetRepository>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("_myAllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "https://gentle-stone-0c0a46303.6.azurestaticapps.net")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();
app.UseRouting();
app.UseCors("_myAllowSpecificOrigins");
app.UseAuthentication();
app.UseAuthorization();

app.UseHttpMetrics(); 
app.UseEndpoints(endpoints =>
{
    endpoints.MapMetrics();  
    endpoints.MapControllers();
});

app.MapControllers();

app.Run();
