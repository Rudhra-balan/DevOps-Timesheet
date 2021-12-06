using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HI.DevOps.Application;
using HI.DevOps.Application.Common.Exceptions;
using HI.DevOps.DomainCore.Helper.Constant;
using HI.DevOps.DomainCore.Helper.RoleBased;
using HI.DevOps.DomainCore.Models.DevOps;
using HI.DevOps.Infrastructure;
using HI.DevOps.Web.Common.Helper.StaticFileService;
using HI.DevOps.Web.Common.Middleware;
using log4net;
using log4net.Config;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;

namespace HI.DevOps.Web
{
    public class Startup
    {
        #region Private Members

        private static ILog _sysLog;

        #endregion

        public Startup(IWebHostEnvironment env)
        {
            ConfigureLog4Net();
            if (_sysLog.IsDebugEnabled)
                _sysLog.Debug("Entering Startup.");

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, false)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, false)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            Environment = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var sessionTimeout = AppConstants.SessionIdleSystemTimeout;
            services.AddSingleton(Configuration);
            services.AddResponseCaching();

            services.AddMemoryCache();
            services.AddApplication();
            services.AddInfrastructure(Configuration, Environment);
            services.AddSession(options => { options.IdleTimeout = TimeSpan.FromMinutes(sessionTimeout); });

            // to support our authorization, disable automatic challenge

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(sessionTimeout);
                    options.LoginPath = "/Authentication/Index";
                    options.Cookie = new CookieBuilder
                    {
                        Name = ".Template.azure.Session",
                        SecurePolicy = Environment.IsProduction() ? CookieSecurePolicy.Always : CookieSecurePolicy.None,
                        HttpOnly = Environment.IsProduction()
                    };
                });

            services.Configure<DevOpsClient>(Configuration.GetSection("DevOpsClient"));
            services.AddScoped<IStaticFileCacheService, StaticFileCacheService>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy(Permissions.ReaderPolicy,
                    policy => policy.RequireClaim(Permissions.ReaderPolicyClaims.ToString()));
                options.AddPolicy(Permissions.UserPolicy,
                    policy => policy.RequireRole(Permissions.UserRole, Permissions.AdminRole));
                options.AddPolicy(Permissions.AdminPolicy, policy => policy.RequireRole(Permissions.AdminRole));
            });
            services.AddControllersWithViews(config =>
            {
                config.Filters.Add(typeof(CustomExceptionFilter));
                config.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });
            services.AddDataProtection();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            var policyCollection = new HeaderPolicyCollection()
                .AddFrameOptionsDeny()
                .AddXssProtectionBlock()
                .AddContentTypeOptionsNoSniff()
                .AddStrictTransportSecurityMaxAgeIncludeSubDomains(60 * 60 * 24 * 7)
                .AddReferrerPolicyStrictOriginWhenCrossOrigin()
                .RemoveServerHeader()
                .AddContentSecurityPolicy(builder =>
                {
                    builder.AddObjectSrc().None();
                    builder.AddFormAction().Self();
                    builder.AddFrameAncestors().None();
                });

            app.UseSecurityHeaders(policyCollection);
            app.UseHttpsRedirection();
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    // Cache static files for 30 days
                    ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=2592000");
                    ctx.Context.Response.Headers.Append("Expires",
                        DateTime.UtcNow.AddDays(30).ToString("R", CultureInfo.InvariantCulture));
                }
            });
            app.UseCookiePolicy();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            app.UseCustomExceptionHandler();
            app.UseResponseCaching();

            app.Use(async (context, nextMiddleware) =>
            {
                context.Response.OnStarting(() =>
                {
                    //// if UAT feedback states that must revalidate should be conditional only for non image responses
                    //// then must-revalidate should conditionally be added if the content type is image/png or image/gif
                    string[] contentTypeBuilder =
                    {
                        "text/css",
                        "application/javascript", "image/png", "font/woff2",
                        "application/x-font-ttf", "image/svg+xml", "image/gif"
                    };
                    if (context.Response.ContentType != null &&
                        contentTypeBuilder.Contains(context.Response.ContentType.Trim()))
                    {
                        context.Response.GetTypedHeaders().CacheControl =
                            new CacheControlHeaderValue
                            {
                                Public = true,
                                MaxAge = TimeSpan.FromDays(Configuration.GetSection("StaticContentCacheTimeoutDays")
                                    .Get<int>()),
                                MustRevalidate = true
                            };
                        context.Response.Headers[HeaderNames.Vary] =
                            new[] {"Accept-Encoding"};

                        if (!context.Response.Headers.ContainsKey("Pragma"))
                            context.Response.Headers.Add("Pragma", "Cache-Control");

                        return Task.FromResult(0);
                    }

                    if (!context.Response.Headers.ContainsKey("Pragma"))
                        context.Response.Headers.Add("Pragma", "no-cache");

                    context.Response.GetTypedHeaders().CacheControl =
                        new CacheControlHeaderValue
                        {
                            MustRevalidate = true,
                            NoCache = true,
                            NoStore = true
                        };
                    context.Response.Headers[HeaderNames.Vary] =
                        new[] {"Accept-Encoding"};

                    return Task.FromResult(0);
                });

                await nextMiddleware();
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Authentication}/{action=Index}/{id?}");
            });
        }

        //endpoints.MapControllerRoute(
        //name: "default",
        //pattern: "{controller=Authentication}/{action=Index}/{id?}");

        #region Private Methods

        private void ConfigureLog4Net()
        {
            _sysLog = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

            var loggingRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(loggingRepository, new FileInfo("log4net-logging.config"));
        }

        #endregion
    }
}