using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OrderService.Application.Orders.CreateOrder;
using OrderService.Infrastructure.Messaging;
using OrderService.Infrastructure.Persistence;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<OutboxPublisherService>();

builder.Services.AddDbContext<OrderDbContext>(
    options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString(
                "EMicroOrderConnection")));

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(
        typeof(CreateOrderCommand).Assembly);
});

//builder.Services.AddInfrastructure(
//    builder.Configuration);


//// JWT Authentication
//var jwtKey = builder.Configuration["Jwt:Key"];
//var jwtIssuer = builder.Configuration["Jwt:Issuer"];
//var jwtAudience = builder.Configuration["Jwt:Audience"];

//if (string.IsNullOrWhiteSpace(jwtKey))
//{
//    throw new InvalidOperationException(
//        "JWT Key is missing. Please configure Jwt:Key in appsettings.json");
//}

//if (string.IsNullOrWhiteSpace(jwtIssuer))
//{
//    throw new InvalidOperationException(
//        "JWT Issuer is missing. Please configure Jwt:Issuer in appsettings.json");
//}

//if (string.IsNullOrWhiteSpace(jwtAudience))
//{
//    throw new InvalidOperationException(
//        "JWT Audience is missing. Please configure Jwt:Audience in appsettings.json");
//}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        builder.Configuration["Jwt:Key"]!))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                Console.WriteLine("========== JWT MESSAGE RECEIVED ==========");
                Console.WriteLine(
                    $"Authorization Header: {context.Request.Headers.Authorization}");

                return Task.CompletedTask;
            },

            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("========== JWT AUTH FAILED ==========");
                Console.WriteLine(context.Exception.GetType().Name);
                Console.WriteLine(context.Exception.Message);

                return Task.CompletedTask;
            },

            OnTokenValidated = context =>
            {
                Console.WriteLine("========== JWT VALIDATED ==========");

                foreach (var claim in context.Principal!.Claims)
                {
                    Console.WriteLine(
                        $"{claim.Type} = {claim.Value}");
                }

                return Task.CompletedTask;
            }
        };
    });



builder.Services.AddAuthorization();




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
