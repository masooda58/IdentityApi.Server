using Common.Api.Dependency.Cors;
using Common.Api.Dependency.Swagger;
using Common.Jwt.Authentication;
using Jwt.Identity.Api.Server.Helpers.CustomSignIn;
using Jwt.Identity.Api.Server.Security;
using Jwt.Identity.Api.Server.Services.ConfirmCode;
using Jwt.Identity.Api.Server.Services.MessageServices;
using Jwt.Identity.Api.Server.Services.PhoneTotpProvider;
using Jwt.Identity.Api.Server.Services.TokenServices;
using Jwt.Identity.Data.Context;
using Jwt.Identity.Data.IntialData;
using Jwt.Identity.Data.Repositories.UserRepositories;
using Jwt.Identity.Data.UnitOfWork;
using Jwt.Identity.Domain.IServices;
using Jwt.Identity.Domain.IServices.Email;
using Jwt.Identity.Domain.IServices.Totp;
using Jwt.Identity.Domain.IServices.Totp.SettingModels;
using Jwt.Identity.Domain.Token.Data;
using Jwt.Identity.Domain.Token.ITokenServices;
using Jwt.Identity.Domain.User.Entities;
using Jwt.Identity.Domain.User.Enum;
using Jwt.Identity.Framework.Identity;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using EasyCaching.Core.Configurations;
using EasyCaching.Serialization.SystemTextJson.Configurations;
using Google.Apis.Util;
using Jwt.Identity.Api.Server.Helpers.Convention;
using Jwt.Identity.Api.Server.Helpers.CustomAuthenticaton;
using Jwt.Identity.Api.Server.IOC;
using Jwt.Identity.Api.Server.IOC.CustomCache;
using Jwt.Identity.Api.Server.Setting;
using Jwt.Identity.Data.InitialData;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Caching.Distributed;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Jwt.Identity.Api.Server
{
   

    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment envinment)
        {
            Configuration = configuration;
            Envinment = envinment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Envinment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region DbContext

            services.AddDbContext<IdentityContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("IdetityDb")));
            //, ServiceLifetime.Transient
            #endregion
            
            string projectName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name.Replace(".", "");
            #region MemoryCache
            var redisOptions = Configuration.GetSection("Redis");
            services.Configure<RedisSettings>(redisOptions);
            var redisInstanceOptions = redisOptions.Get<RedisSettings>();
           // services.AddMemoryCache();
          // services.AddDistributedMemoryCache();
          services.AddEasyCaching(option =>
          {
              option.WithSystemTextJson("myjson");

              // local
              option.UseInMemory("m1");
              // distributed
              option.UseRedis(config =>
              {
                  config.DBConfig.Endpoints.Add(new ServerEndPoint(redisInstanceOptions.Ip,
                      Convert.ToInt32(redisInstanceOptions.Port)));
                  config.DBConfig.KeyPrefix = projectName + ":";
                  config.DBConfig.Database = 5;
                  config.SerializerName = "myjson";
              }, "r1");

              // combine local and distributed
              option.UseHybrid(config =>
                  {
                      config.TopicName = "test-topic";
                      config.EnableLogging = true;

                      // specify the local cache provider name after v0.5.4
                      config.LocalCacheProviderName = "m1";
                      // specify the distributed cache provider name after v0.5.4
                      config.DistributedCacheProviderName = "myredis";
                  },"h1")
                  // use redis bus
                  .WithRedisBus(busConf =>
                  {
                      busConf.Endpoints.Add(new ServerEndPoint(redisInstanceOptions.Ip,
                          Convert.ToInt32(redisInstanceOptions.Port)));

                      // do not forget to set the SerializerName for the bus here !!
                      busConf.SerializerName = "myjson";
                  });
          });
            services.AddCustomCacheConfig(Envinment,"m1","h1");
            #endregion
            services.AddTransient<UnitOfWork>();
            #region Identity

            services.IdentityConfigExtension();

            


            #endregion

            // ===== Configure Identity =======

            //services.ConfigureApplicationCookie(options =>
            //{
            //    options.Cookie.Name = "auth_cookie";

            //    options.LoginPath = new PathString("/api/contests");
            //    options.AccessDeniedPath = new PathString("/api/contests");

            //    // Not creating a new object since ASP.NET Identity has created
            //    // one already and hooked to the OnValidatePrincipal event.
            //    // See https://github.com/aspnet/AspNetCore/blob/5a64688d8e192cacffda9440e8725c1ed41a30cf/src/Identity/src/Identity/IdentityServiceCollectionExtensions.cs#L56
            //    options.Events.OnRedirectToLogin = context =>
            //    {
            //        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            //        return Task.CompletedTask;
            //    };
            //});

            #region JwtTokenSetting

            var jwtSetting = new JwtSettingModel();
            Configuration.Bind("JWT", jwtSetting);
            services.AddSingleton(jwtSetting);

            #endregion
            
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "None";
                    options.DefaultScheme = "JWT_OR_COOKIE";
                    options.DefaultChallengeScheme = "JWT_OR_COOKIE";
                    options.DefaultSignInScheme = "JWT_OR_COOKIE";
                    //configureOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                //.AddCookie("Cookies", options =>
                //{
                //    options.LoginPath = "/Api/Acount/login";
                //    options.ExpireTimeSpan = TimeSpan.FromDays(1);
                //    options.Events.OnRedirectToLogin = context =>
                //    {
                //        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                //        return Task.CompletedTask;
                //    };
                //})

                //.AddJwtBearer("Bearer", options =>
                //{
                //    options.TokenValidationParameters = new TokenValidationParameters
                //    {
                //        ValidateIssuer = true,
                //        ValidateAudience = true,
                //        ValidAudience = jwtSetting.ValidAudience,
                //        ValidIssuer = jwtSetting.ValidIssuer,
                //        RequireExpirationTime = true,
                //        ClockSkew = TimeSpan.Zero,


                //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting.Secret))
                //    };
                //    options.Events = new JwtBearerEvents
                //    {
                //        OnMessageReceived = context =>
                //        {
                //            context.Token = context.Request.Cookies["Access-TokenSession"];
                //            return Task.CompletedTask;
                //        },
                //        //OnTokenValidated = context =>
                //        //{
                //        //    var cache = context.HttpContext.RequestServices.GetService< IDistributedCache>();
                //        //    var accessToken = context.SecurityToken as JwtSecurityToken;
                //        //    var identity = context.Principal.Identity as ClaimsIdentity;
                //        //    var tt = accessToken.RawData;
                //        //    var UserID = identity.Claims.First(c => c.Type == "id").Value;
                //        //    return Task.CompletedTask;
                //        //}
                //    };
                //})
                //.AddPolicyScheme("JWT_OR_COOKIE", "JWT_OR_COOKIE", options =>
                //{
                //    // runs on each request
                //    options.ForwardDefaultSelector = context =>
                //    {
                //        // filter by auth type
                //        var reqHost = context.Request.Host.ToString();
                //        var client = InitialClients.GetClients().SingleOrDefault(c => c.BaseUrl.Contains(reqHost));
                //        if (client is { LoginType: LoginType.Token or LoginType.TokenAndCookie })
                //        {
                //            string authorization = context.Request.Headers[HeaderNames.Authorization];
                //            if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
                //                return "Bearer";
                //        }


                //        if (client is { LoginType: LoginType.Cookie or LoginType.TokenAndCookie })
                //        {
                //            string accessToken = context.Request.Headers[HeaderNames.Cookie];
                //            if (!string.IsNullOrEmpty(accessToken) && accessToken.Contains("Access-Token"))
                //                return "Bearer";
                //        }


                //        // otherwise always check for cookie auth

                //        return "Cookies";
                //    };
                //})
             .AddScheme<CustomAutenthicationOptions,CustomAuthenticationHandler>("JWT_OR_COOKIE", options =>
                {
                    
                })
                .AddGoogle(option =>
                {
                    option.ClientId = "346095678950-dhuqj3ko64i5i1becqteg2v3rv9l8j6a.apps.googleusercontent.com";
                    option.ClientSecret = "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";
                });
            // 
            services.AddAuthorization(options =>
            {
                var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
                    //JwtBearerDefaults.AuthenticationScheme,
                    //CookieAuthenticationDefaults.AuthenticationScheme,
                    "JWT_OR_COOKIE");
                defaultAuthorizationPolicyBuilder =
                    defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();

                options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
            });

         

            #region Options from AppSetting

            #region Totp

            services.Configure<TotpSettings>(Configuration.GetSection("Totp"));

            #endregion

            #region Swaager&Cors

            //Nuget.Project:Common.Api.Dependency
            services.AddOurSwagger();
            var corsOrigin = Configuration.GetSection("Cors:Origin").Get<string[]>();
            var corsMethod = Configuration.GetSection("Cors:Method").Get<string[]>();
            services.AddOurCors(corsOrigin, corsMethod);

            #endregion

            #endregion
            //c.ControllerType.Namespace?.Split('.').Last()
            
            services.AddControllers(options => options.Conventions.Add(
                    new CustomRouteToken(
                        "ProjectRout",
                        c =>projectName+"/Api/"+c.ControllerName
                    )))
                .ConfigureApiBehaviorOptions(options =>
                {
                    //   options.SuppressConsumesConstraintForFormFileParameters = true;
                    //   options.SuppressInferBindingSourcesForParameters = true;
                    // modelState مدیریت
                    options.SuppressModelStateInvalidFilter = true;
                    //   options.SuppressMapClientErrors = true;
                    //   options.ClientErrorMapping[StatusCodes.Status404NotFound].Link =
                    //      "/404";
                });
            services.AddHttpContextAccessor();
            services.AddMediatR(typeof(Startup).Assembly);
            services.AddLogging();
    
            #region dependancy

            services.AddScoped<UserManagementService>();
            
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddSingleton<ITokenGenrators, TokenGenrators>();
            services.AddSingleton<ITokenValidators, TokenValidators>();
            services.AddSingleton<IAuthClaimsGenrators, AuthClaimsGenrators>();

            services.AddScoped<IEmailSender, EmailService>();
            services.AddScoped<ISmsSender, SmsServices>();
            services.AddTransient<IPhoneTotpProvider, PhoneTotpProvider>();
            services.AddScoped<ITotpCode, TotpCode>();
            services.AddScoped<IMailCode, MailCode>();
            services.AddSingleton<DataProtectionPepuseString>();
           
            #endregion

            //services.AddCors(options =>
            //{
            //    options.AddPolicy("CORSAllowLocalHost3000",
            //        builder =>
            //            builder.WithOrigins(new[] { "http://localhost:3000", "http://localhost:8080" })
            //                .AllowAnyHeader()
            //                .AllowAnyMethod()
            //                .AllowCredentials() // <<< this is required for cookies to be set on the client - sets the 'Access-Control-Allow-Credentials' to true
            //    );
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Jwt.Identity.Api.Server v1"));
            }
            //IdentityDbContextSeed.SeedData(context);
            app.UseCors();
            app.UseHttpsRedirection();
            app.UseExceptionHandler(a => a.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature.Error;

                await context.Response.WriteAsJsonAsync(new { error = exception.Message, Data = exception.Data });
            }));

            app.UseRouting();
          
            app.UseAuthentication();
            app.UseAuthorization();
            //var projectName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;


            //app.UseEndpoints(routes =>
            //{
            //    routes.MapControllerRoute(
            //        name: "default",
            //        pattern: $"{projectName}/{{controller}}/{{action}}/{{id?}}");
            //});
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });





        }
    }
}