using AgronicaCoreApi.Extensions;
using AgronicaCoreAPI;
using AgronicaCoreAPI.Extensions;
using AgronicaCoreAPI.Middleware;
using AgronicaNetCore.Base.Services.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var log4netRepository = log4net.LogManager.GetRepository(Assembly.GetEntryAssembly());
log4net.Config.XmlConfigurator.Configure(log4netRepository, new FileInfo("log4net.config.xml"));

//file configurazione dedicato a serilog, non è mandatorio e viene ricaricato alla modifica
builder.Configuration.AddJsonFile("Logger.settings.json", true, true);
builder.Configuration.AddJsonFile("Database.settings.json", true, true);
builder.Configuration.AddJsonFile("ParametriAggiuntivi.settings.json", optional: true, reloadOnChange: true);

builder.Services.AddApplicationServices(builder.Configuration, builder.Logging, builder.Environment);

var app = builder.Build();

//Translations
var localizationOptions = new RequestLocalizationOptions()
    .AddSupportedUICultures("en", "fr", "it", "pt");

// Insert culture from route
localizationOptions.RequestCultureProviders = new[]
{
    new RouteDataRequestCultureProvider { Options = localizationOptions }
};

app.UseRequestLocalization(localizationOptions);

if (builder.Configuration.GetValue<Boolean>("enableResponseCompression"))
{
    app.UseResponseCompression();
}

// attiva log request/response
if (builder.Configuration.GetValue<Boolean>("enableLogTraffic"))
    app.UseRequestResponseLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();

    app.UseDeveloperExceptionPage();

    app.UseSwagger(c =>
    {
        c.SerializeAsV2 = true;
    });
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AgronicaCoreAPI v1");
        c.DefaultModelsExpandDepth(-1);
    });
}

if (builder.Configuration.GetValue<Boolean>("sslRedirect") == true)
{
    app.UseHttpsRedirection();
}


//Enabling CORS
if (builder.Configuration.GetValue<Boolean>("allowCORS") == true)
{
    String originCORS = builder.Configuration.GetValue<String>("originCORS").ToString();
    List<String> listaOriginCORS = new List<string>();

    try
    {
        listaOriginCORS = builder.Configuration.GetSection("originListOfCORS").Get<List<string>>();
    }
    catch
    {
    }

    if (!String.IsNullOrEmpty(originCORS))
    {
        listaOriginCORS.Add(originCORS);
    }

    String[] arrayOriginCORS = listaOriginCORS.ToArray();

    app.UseCors(opt => opt
    .WithOrigins(arrayOriginCORS)
    .SetIsOriginAllowedToAllowWildcardSubdomains()
    .AllowAnyHeader()
    .AllowCredentials()
    .AllowAnyMethod()
    );
}

//app.UseHttpsRedirection();
//app.UseAuthentication();
//app.UseAuthorization();
//app.MapControllers();

app.UseRouting();
app.UseAuthorization();
app.UseAuthentication();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

ServicePointManager.DefaultConnectionLimit = int.MaxValue;

app.Init();
app.Run();
