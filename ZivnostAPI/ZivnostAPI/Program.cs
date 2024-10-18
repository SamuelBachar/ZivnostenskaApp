using AvantiPoint.MobileAuth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using SharedTypesLibrary.Constants;
using System;
using System.Reflection;
using System.Text.Json;
using ZivnostAPI.ApiConfigClasses;
using ZivnostAPI.ApiConfigExtensions;
using ZivnostAPI.Constants;
using ZivnostAPI.Controllers;
using ZivnostAPI.Data.CusDbContext;
using ZivnostAPI.Models.AuthProvidersData;
using ZivnostAPI.Services.CompanyService;
using ZivnostAPI.Services.Generic;
using ZivnostAPI.Services.LogInService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.ConfigureSwagger()
                .AddCustomServices()
                .AddOAuthHttpClients(builder.Configuration)
                .AddDatabaseServices(builder.Configuration)
                .CacheOAuthSettings(builder.Configuration)
                .AddOAuthUrlBuildService()
                .AddAutoMapping();

var app = builder.Build();

var jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), APIConstants.JsonExceptionFilePath);
app.LoadJsonExceptions(jsonFilePath);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCustomSwagger();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseAuthentication();
app.MapControllers();

app.Run();