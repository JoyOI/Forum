﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using JoyOI.Forum.Models;

namespace JoyOI.Forum
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            IConfiguration Configuration;
            services.AddConfiguration(out Configuration);

            services.AddEntityFrameworkMySql()
               .AddDbContext<ForumContext>(x => x.UseMySql("User ID=postgres;Password=123456;Host=localhost;Port=5432;Database=ForumR;"));

            services.AddIdentity<User, IdentityRole<Guid>>(x =>
            {
                x.Password.RequireDigit = false;
                x.Password.RequiredLength = 0;
                x.Password.RequireLowercase = false;
                x.Password.RequireNonAlphanumeric = false;
                x.Password.RequireUppercase = false;
                x.User.AllowedUserNameCharacters = null;
            })
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ForumContext, long>();

            services.AddBlobStorage()
                .AddEntityFrameworkStorage<ForumContext>();

            services.AddMvc();

            services.AddSignalR();
            services.AddAntiXss();
            services.AddAesCrypto();
            services.AddSmtpEmailSender("smtp.exmail.qq.com", 25, "Mano Cloud", "noreply@mano.cloud", "noreply@mano.cloud", "ManoCloud123456");
            services.AddSmartCookies();
            services.AddSmartUser<User, Guid>();
        }
        
        public async void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Warning, true);

            app.UseStaticFiles();
            app.UseDeveloperExceptionPage();
            app.UseIdentity();
            app.UseBlobStorage("/assets/shared/scripts/jquery.pomelo.fileupload.js");
            app.UseIdentity();
            app.UseStaticFiles();
            app.UseWebSockets();
            app.UseSignalR();
            app.UseAutoAjax();
            app.UseMvcWithDefaultRoute();

            await SampleData.InitDB(app.ApplicationServices);
        }
    }
}
