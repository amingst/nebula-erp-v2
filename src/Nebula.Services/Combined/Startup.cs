using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Nebula.Services.Authentication.Shared.Extensions;
using Nebula.Services.Base.Models;
using Nebula.Services.Combined.Models;
using Nebula.Services.Organizations.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Services.Combined
{
    public class Startup
    {
        private static byte[] PONG_RESPONSE = { (byte)'p', (byte)'o', (byte)'n', (byte)'g' };

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = new NeutralNamingPolicy();
            });
            services.AddGrpc(opt =>
            {
                opt.EnableDetailedErrors = true;
                opt.MaxReceiveMessageSize = int.MaxValue;
                opt.MaxSendMessageSize = int.MaxValue;
            });

            var securityScheme = new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JSON Web Token based security",
            };

            var securityReq = new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer",
                        },
                    },
                    new string[] { }
                },
            };

            var info = new OpenApiInfo()
            {
                Version = "v1",
                Title = "Nebula.Services API",
                Description = "Nebula.Services API",
            };

            services.AddSwaggerGen(o =>
            {
                o.SwaggerDoc("v1", info);
                o.AddSecurityDefinition("Bearer", securityScheme);
                o.AddSecurityRequirement(securityReq);
            });

            services.AddGrpcSwagger();

            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
                
            services.AddJwtAuthentication();
            services.AddAuthenticationClasses();
            services.AddAuthenticationDb(Configuration);
            services.AddOrganizationClasses();
            services.AddOrganizationsDb(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.Map(
                "/ping",
                (app1) =>
                    app1.Run(async context =>
                    {
                        await context.Response.BodyWriter.WriteAsync(PONG_RESPONSE);
                    })
            );

            app.UseStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUI();

            if (env.IsDevelopment())
                Program.IsDevelopment = true;

            app.UseRouting();

            app.UseJwtApiAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAuthenticationEndpoints();
                endpoints.MapOrganizationEndpoints();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                );
            });
        }
    }
}
