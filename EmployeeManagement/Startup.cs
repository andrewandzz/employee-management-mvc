using EmployeeManagement.Models;
using EmployeeManagement.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace EmployeeManagement
{
    public class Startup
    {
        private readonly IConfiguration config;

        public Startup(IConfiguration config)
        {
            this.config = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<AppDbContext>((options) =>
            {
                options.UseSqlServer(this.config.GetConnectionString("EmployeesDBConnection"));
            });

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredUniqueChars = 3;
                options.SignIn.RequireConfirmedEmail = true;
                options.Tokens.EmailConfirmationTokenProvider = "EmailConfirmation";
            }).AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders()
            .AddTokenProvider<EmailConfirmationTokenProvider<ApplicationUser>>("EmailConfirmation");

            services.AddMvc(options => { options.EnableEndpointRouting = false; });

            services.Configure<DataProtectionTokenProviderOptions>(options => options.TokenLifespan = TimeSpan.FromHours(1.0));
            services.Configure<EmailConfirmationTokenProviderOptions>(options => options.TokenLifespan = TimeSpan.FromDays(3));

            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = "1034968345169-c2mpbivqaq898dllia8d3h0j4k5evae5.apps.googleusercontent.com";
                    options.ClientSecret = "AErNgpmsXDY-2w0KgoHaUrN2";
                })
                .AddFacebook(options =>
                {
                    options.AppId = "2811470949129079";
                    options.AppSecret = "8ad3a05ec06462129b2f149005301c08";
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("EditAdminPolicy", policyBuilder => policyBuilder.AddRequirements(new ManageAdminRolesAndClaimsRequirement()));
                options.AddPolicy("ReadPolicy", policyBuilder => policyBuilder.RequireClaim("Read"));
                options.AddPolicy("EditPolicy", policyBuilder => policyBuilder.RequireClaim("Edit"));
                options.AddPolicy("CreatePolicy", policyBuilder => policyBuilder.RequireClaim("Create"));
                options.AddPolicy("DeletePolicy", policyBuilder => policyBuilder.RequireClaim("Delete"));
            });

            services.AddScoped<IEmployeeRepository, SqlEmployeeRepository>();
            services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaimsHandler>();
            services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(config =>
            {
                config.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}