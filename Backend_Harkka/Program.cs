using Microsoft.EntityFrameworkCore;
using Backend_Harkka.Models;
using Backend_Harkka.Repositories;
using Backend_Harkka.Services;
using Backend_Harkka.Middleware;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);
// http://localhost:5031/api/Message
// http://localhost:5031/api/User
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<MessageServiceContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("MessageServiceDB")));
builder.Services.AddAuthentication("BasicAuthentication").AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<ApiKeyMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
