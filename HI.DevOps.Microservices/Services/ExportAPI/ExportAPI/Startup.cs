using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Hi.DevOps.Export.API.Application.BusinessManager;
using Hi.DevOps.Export.API.Application.DataBaseRepo;
using Hi.DevOps.Export.API.Application.Helpers;
using Hi.DevOps.Export.API.Application.IBusinessManager;
using Hi.DevOps.Export.API.Application.IDataBaseRepo;
using Hi.DevOps.Export.API.DataObject.AuthorizeDO;
using Hi.DevOps.Export.API.DataObject.ErrorDO;
using log4net;
using log4net.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Hi.DevOps.Export.API
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            ConfigureLog4Net();
            if (SysLog.IsDebugEnabled) SysLog.Debug("Entering Startup.");
            var builder = new ConfigurationBuilder().SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, false)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            if (SysLog.IsDebugEnabled) SysLog.Debug("Exiting Startup.");
        }

        #region Private Members

        public static ILog SysLog { get; private set; }

        #endregion

        public IConfiguration Configuration { get; }

        #region Private Methods

        /// <summary>
        ///     Initializes the application trace logger.
        /// </summary>
        private void ConfigureLog4Net()
        {
            SysLog = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            var loggingRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(loggingRepository, new FileInfo("log4net-logging.config"));
        }

        #endregion

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (SysLog.IsDebugEnabled) SysLog.Debug("Entering ConfigureServices.");
            services.AddControllers();

            var authSettings = Configuration.GetSection(nameof(AppSettings));
            services.Configure<AppSettings>(authSettings);
            var signingKey =
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(authSettings[nameof(AppSettings.Secret)]));

            // jwt wire up
            // Get options from app settings
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

            // Configure JwtIssuerOptions
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            });
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],
                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(configureOptions =>
            {
                configureOptions.ClaimsIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                configureOptions.TokenValidationParameters = tokenValidationParameters;
                configureOptions.SaveToken = true;
                configureOptions.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            context.Response.Headers.Add("Token-Expired", "true");
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddScoped<IExportBM, ExportBM>();
            services.AddScoped<IExportRepo>(c => new ExportRepo(Configuration["ConnectionString:Path"]));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "TimeSheet Server", Version = "v1"});
                c.AddSecurityDefinition("Bearer",
                    new OpenApiSecurityScheme
                    {
                        Description =
                            "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer <Key>\"",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Authentication server"
                    });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "Bearer"},
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });
            if (SysLog.IsDebugEnabled) SysLog.Debug("Exiting ConfigureServices.");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (SysLog.IsDebugEnabled) SysLog.Debug("Entering Configure.");

            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseDeveloperExceptionPage();

            app.UseSwagger();

            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Authentication API v1"); });

            // Acting the middle ware between the services and client devices. 
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            // For worst case handling the all unexpected error during run time .
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        await context.Response.WriteAsync(new ErrorDO
                        {
                            Id = context.Response.StatusCode,
                            Message = "Internal Server Error.",
                            ResponseValue = contextFeature.Error.Message
                        }.ToString());
                        if (SysLog.IsErrorEnabled) SysLog.Error("Internal Server Error :", contextFeature.Error);
                    }
                });
            });
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            if (SysLog.IsDebugEnabled) SysLog.Debug("Exiting Configure.");
        }
    }
}