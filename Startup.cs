using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using EASY.Website.Utility;

namespace myvapi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddCors();
            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            { 
                builder.WithOrigins()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
                /*
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                   */
            }));

            services.AddMvc().AddXmlSerializerFormatters(); //for xml Output

            //services.AddControllers();    //just a controller for API only
            services.AddControllersWithViews(); //includes  view  for html
            services.AddHttpClient();
            var appSettingsSection = Configuration.GetSection("MySettings");
            services.Configure<MySettingsModel>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<MySettingsModel>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = "ats",
                    ValidIssuer = "aresh"
                };
            });
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
                app.UseExceptionHandler("/Home");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }       
            app.UseCors("MyPolicy");
            //app.UseHttpsRedirection(); //Causes CORS ERROR forces HTTPS
            //app.UseDefaultFiles(); //for html
            app.UseStaticFiles();  //for html

            app.UseRouting();
            
            app.UseAuthentication();
            app.UseAuthorization();
            string defaultPage = "/Home";
            app.UseStatusCodePagesWithRedirects(defaultPage);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/",  context =>
                {
                     context.Response.Redirect(defaultPage, permanent: false);
                     return Task.FromResult(0);
                });
            });
        }
    }
}
