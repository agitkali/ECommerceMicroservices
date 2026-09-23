using Microsoft.EntityFrameworkCore;
using UserService.Application.Users.CreateUser;
using UserService.Controllers;
using UserService.Infrastructure.Persistence;
using UserService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AddDbContext<UserDbContext>(
    options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString(
                "EMicroUserConnection")));

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(
        typeof(CreateUserCommand).Assembly);
});

builder.Services.AddHttpClient("OrdersController", client =>
{
    client.BaseAddress = new Uri("https://localhost:7064/");
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
