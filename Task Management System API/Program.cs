using Microsoft.EntityFrameworkCore;
using Task_Management_System_API.Models;
using Task_Management_System_API.Services;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<TaskDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Define a CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("Policy1", policy =>
    {
        policy.AllowAnyOrigin()        // Allows any origin to access the API
              .AllowAnyMethod()        // Allows any HTTP method (GET, POST, etc.)
              .AllowAnyHeader();       // Allows any headers in the requests
    });
});

builder.Services.AddEndpointsApiExplorer(); // Necessary for endpoint discovery
builder.Services.AddSwaggerGen();            // Adds Swagger generation
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

// Register EmailService as a transient dependency
builder.Services.AddTransient<IEmailService, EmailService>();
var app = builder.Build();

// Use CORS policy globally (for all endpoints)
app.UseCors("Policy1");

// Enable Swagger middleware
app.UseSwagger();  // This enables the generation of the Swagger specification
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"); // Set Swagger UI's endpoint
    options.RoutePrefix = string.Empty;  // Makes Swagger UI accessible at the root of the app
});

app.MapControllers();

app.Run();
