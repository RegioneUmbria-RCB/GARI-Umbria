using AgronicaDataProvider6.Interfaces;
using AgronicaDataProvider6;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Serilog.Events;
using Serilog;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
using Swashbuckle.AspNetCore.SwaggerGen;
using Humanizer;
using Newtonsoft.Json.Converters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AgronicaCoreAPI.DataProtection;
using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using System.Net.Http;
using CoreApi.BusinessLayer.Services;
using AgronicaNetCore.SuperServer.DAL.Autenticazione;
using AgronicaNetCore.SuperServer.BIZ.Autenticazione;
using AgronicaDataProvider6.Models;
using AgronicaNetCore.Operazione.DAL.DataLayer.OperazioneCausale;
using AgronicaNetCore.Operazione.BIZ.Services.OperazioneCausale;
using AgronicaNetCore.UtilityDB.DAL.DataLayer.UtilityDB;
using AgronicaNetCore.UtilityDB.DAL.DataLayer.Agro_Sequenze;
using AgronicaCoreModelsSTD.AgronicaChatGPT;
using AgronicaNetCore.Base.Utility;
using System.Security.Cryptography.Xml;
using AgronicaNetCore.Base.Services.Security;
using AgronicaNetCore.Base.DataLayer.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using AgronicaNetCore.Base.Services.Culture;
using AgronicaNetCore.Base.DataLayer.LastCUAA;
using AgronicaCoreAPI.Helpers;
using AgronicaNetCore.Base.Services.Email;
using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Utenti.DAL.DataLayer.Utenti;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiPassword;
using AgronicaNetCore.Utenti.BIZ.Services.UtentiPassword;
using AgronicaNetCore.Utenti.BIZ.Services.PasswordExpirationConfig;
using AgronicaNetCore.Utenti.BIZ.Services.PasswordExpirationCalculator;
using AgronicaNetCore.Utenti.BIZ.Services.ControlloScadenzaPasswordAlLogin;
using AgronicaNetCore.Utenti.BIZ.Services.NotificaScadenzaPassword;
using AgronicaCoreAPI.Messaging;
using AgronicaNetCore.Base.DataLayer.ConfigurazioneElasticSearch;


namespace AgronicaCoreApi.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration, ILoggingBuilder logging, IWebHostEnvironment env)
        {
            #region "Logger"
            logging.ClearProviders();
            logging.AddSerilog(new LoggerConfiguration()
                            .ReadFrom.Configuration(configuration)
                            .WriteTo.Logger(c1 => c1
                                            .Filter.ByIncludingOnly(e => e.Properties.ContainsKey("LogPath"))
                                            .WriteTo.Map("LogPath", (pathLog, wt) => wt.File($"{pathLog}-.log", rollingInterval: RollingInterval.Day, shared: true))
                                            )
                            .WriteTo.Logger(c1 => c1
                                            .Filter.ByExcluding(e => e.Properties.ContainsKey("LogPath"))
                                            .WriteTo.File(
                                                configuration.GetValue<string>("Serilog:WriteTo:1:Args:configureLogger:WriteTo:0:Args:path"),
                                                rollingInterval: RollingInterval.Day,
                                                shared: true)
                                            )
                            .WriteTo.Logger(c2 => c2
                                .Filter.ByIncludingOnly(e => e.Properties.ContainsKey("LogWithElasticSearch"))
                                .WriteTo.Http(
                                    "ElasticSearchUrl",
                                    null,
                                    httpClient: new LoggingHttpClientHelper(),
                                    restrictedToMinimumLevel: LogEventLevel.Information,
                                    logEventsInBatchLimit: 50,
                                    period: new TimeSpan(0, 0, 5)))
                            .CreateLogger());
            #endregion

            var connectionString = configuration.GetValue<string>("ConnectionString");
            
            // decodifica la stringa di connessione se criptata
            if (configuration.GetValue<bool>("ConnectionStringEncoded")) {
                connectionString = Security.DecryptString(connectionString, configuration.GetValue<string>("cr2"));
            }

            #region "Localization"
            services.AddLocalization(options => options.ResourcesPath = "");
            #endregion

            #region "Security"
            //0. abilitazione DataProtection per sharing crypt key for cookies
            services.AddDbContext<AgronicaDataProtectionContext>(opt =>
            {
                opt.UseSqlServer(connectionString);
            });

            services.AddDataProtection()
                    .SetApplicationName("Agronica.Auth")
                .PersistKeysToDbContext<AgronicaDataProtectionContext>();

            if (configuration.GetValue<Boolean>("enableResponseCompression"))
            {
                services.AddResponseCompression(options => { options.EnableForHttps = true; });
            }

            // leggo la chiave jwt dalle varibili di ambiente se presente altrimenti dall'appsettings
            var key = Environment.GetEnvironmentVariable(configuration["Jwt:KeyName"] ?? "JWT_KEY") ?? configuration["Jwt:Key"] ??
                throw new ApplicationException("JWT key is not configured.");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            //1 - Authentication & JWT/Cookies
            services
                .AddAuthentication(o =>
                {
                    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Issuer"],
                        IssuerSigningKey = securityKey
                    };
                    // Se il token è presente ma non valido (scaduto, firma errata ecc.) su un endpoint
                    // [AllowAnonymous], non bloccare la request: la request procede non autenticata
                    // e il controller decide. Senza questo, JWT handler restituisce 401 prima
                    // di raggiungere [AllowAnonymous].
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = ctx =>
                        {
                            ctx.NoResult();
                            return System.Threading.Tasks.Task.CompletedTask;
                        }
                    };
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o =>
                {
                    o.ExpireTimeSpan = TimeSpan.FromMinutes(240); // optional
                    o.Cookie.Name = "auth_cookie";
                    o.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
                    o.Cookie.Path = "/";
                    o.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
                });


            services.AddControllers().AddNewtonsoftJson(x =>
            {
                x.SerializerSettings.Converters.Add(new StringEnumConverter());
                // x.SerializerSettings.Converters.Add(new AttivitaConverter());
                // x.SerializerSettings.Converters.Add(new UtilizzoTerrenoConverter());
                x.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                x.SerializerSettings.ContractResolver = new DefaultContractResolver();
                x.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                x.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
                // x.SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();

            services.AddHttpClient("base", c => {
                c.Timeout = new TimeSpan(0, 15, 0);

            }).ConfigurePrimaryHttpMessageHandler(x => new HttpClientHandler()
            {
                //MaxConnectionsPerServer = int.MaxValue,
            });
            services.AddSingleton<IGisDataReadParam, GisDataReadParamValidator>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AgronicaCoreAPI", Version = "v1" });
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "AgronicaCoreDTOStd.xml"), includeControllerXmlComments: true);
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "AgronicaCoreModelsSTD.xml"), includeControllerXmlComments: true);
                // Aggiungere documentazione swagger
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                // To Enable authorization using Swagger (JWT)
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                });
                c.CustomSchemaIds(x => x.ToString());
                c.CustomOperationIds(description => description.TryGetMethodInfo(out MethodInfo methodInfo) ? $"{methodInfo.DeclaringType!.Name.Replace("Controller", string.Empty)}{methodInfo.Name.Humanize().Pascalize()}" : null);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}

                    }
                });
            })
            .AddSwaggerGenNewtonsoftSupport();

            //- Authorization Linked to Authenticated
            var multiSchemePolicy = new AuthorizationPolicyBuilder(
                                                    CookieAuthenticationDefaults.AuthenticationScheme,
                                                    JwtBearerDefaults.AuthenticationScheme)
                                                  .RequireAuthenticatedUser()
                                                  .Build();
            services.AddAuthorization(o => o.DefaultPolicy = multiSchemePolicy);
            #endregion

            //CORS
            services.AddCors(options =>
            {
                options.AddPolicy(name: "AllowOrigin",
                    builder =>
                    {
                        //Lavez - 06/03/2025 - Definizione della policy di CORS, la quale è abilitata SOLO se il flag "allowCORS" in appsettings è true
                        builder.AllowAnyOrigin();
                        builder.WithOrigins("https://localhost:44320", "https://localhost:4200", "http://localhost:4200");
                        builder.WithMethods("POST", "GET", "OPTIONS");
                        //builder.AllowAnyOrigin();
                        builder.AllowAnyHeader();
                        //builder.AllowAnyMethod();
                        builder.AllowCredentials();
                    });
            });


            #region "Services"
            // Add services to the container.

            //Generali
            //services.AddScoped<IIdentityService, IdentityService>();

            services.Configure<SecuritySettings>(configuration.GetSection("SecuritySettings"));

            // security service
            services.AddSingleton<ISecurityService, SecurityService>();
            // security service
            services.AddSingleton<ICultureService, CultureService>();

            //DataProviderFactory
            services.AddSingleton<IDataProvider6Factory, DataProvider6Factory>();

            #region "DAL"
            services.AddTransient<IUtilityDB, UtilityDB>();
            services.AddTransient<IAgro_Sequence, Agro_Sequence>();
            services.AddTransient<IAutenticazione, Autenticazione>();
            services.AddTransient<IOperazioneCausale, OperazioneCausale>();
            services.AddTransient<ISecurityLayer, SecurityLayer>();
            services.AddTransient<ISecurityLayerDAL, SecurityLayerDAL>();
            services.AddTransient<ILastCUAA, LastCUAA>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IUtenti, Utenti>();
            services.AddTransient<IUtentiPassword, UtentiPassword>();
            services.AddTransient<IConfigurazioneElasticSearchDAL, ConfigurazioneElasticSearchDAL>();
            #endregion

            #region "BIZ"
            services.AddScoped<IAutenticazioneService, AutenticazioneService>();
            services.AddScoped<IOperazioneCausaleService, OperazioneCausaleService>();
            services.AddScoped<IUtentiPasswordService, UtentiPasswordService>();
            services.AddScoped<IPasswordExpirationConfigService, PasswordExpirationConfigService>();
            services.AddScoped<IPasswordExpirationCalculatorService, PasswordExpirationCalculatorService>();
            services.AddScoped<IControlloScadenzaPasswordAlLoginService, ControlloScadenzaPasswordAlLoginService>();
            services.AddScoped<IUtentiNotificaScadenzaPasswordService, UtentiNotificaScadenzaPasswordService>();
            #endregion

            #region Logging service
            services.AddTransient<ILoggingService, LoggingService>();
            #endregion

            #endregion

            //cache
            services.AddMemoryCache();

            // Messaggistica asincrona RabbitMQ (attiva solo se RabbitMQ:Enabled = true)
            services.AddMessaging(configuration);

            return services;
        }
    }
}
