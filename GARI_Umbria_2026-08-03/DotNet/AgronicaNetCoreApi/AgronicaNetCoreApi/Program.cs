using AgronicaNetCoreApi.Extensions;
using System.Reflection;
using Microsoft.AspNetCore.Localization.Routing;
using AgronicaNetCore.Gis.BIZ.Services.Gis;

var builder = WebApplication.CreateBuilder(args);

//file configurazione dedicato a serilog, non è mandatorio e viene ricaricato alla modifica
builder.Configuration.AddJsonFile("Logger.settings.json", true, true);
builder.Configuration.AddJsonFile("Database.settings.json", true, true);
builder.Configuration.AddJsonFile("ParametriAggiuntivi.settings.json", true, true);

builder.Services.AddHttpContextAccessor();

builder.Services.AddApplicationServices(builder.Configuration, builder.Logging, builder.Environment);

builder.Services.AddApplicationMappers(builder.Configuration, builder.Logging, builder.Environment);



var app = builder.Build();

app.UseResponseCompression();

//Translations
var localizationOptions = new RequestLocalizationOptions()
    .AddSupportedUICultures("en", "fr", "it","pt");

// Insert culture from route
localizationOptions.RequestCultureProviders = new[]
{
    new RouteDataRequestCultureProvider { Options = localizationOptions }
};

app.UseRequestLocalization(localizationOptions);



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Enabling CORS
if (builder.Configuration.GetValue<Boolean>("allowCORS") == true)
{
    String originCORS = builder.Configuration.GetValue<String>("originCORS")!.ToString();
    List<String> listaOriginCORS = new List<string>();

    try
    {
        listaOriginCORS = builder.Configuration.GetSection("originListOfCORS").Get<List<string>>()!;
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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Init();
app.Run();
