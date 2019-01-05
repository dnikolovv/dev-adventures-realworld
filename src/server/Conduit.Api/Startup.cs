using AutoMapper;
using Conduit.Api.Configuration;
using Conduit.Api.Filters;
using Conduit.Api.ModelBinders;
using Conduit.Business.Identity;
using Conduit.Business.Services;
using Conduit.Core.Configuration;
using Conduit.Core.Identity;
using Conduit.Core.Services;
using Conduit.Data.EntityFramework;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Conduit.Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext(Configuration.GetConnectionString("DbConnectionString"));
            services.AddAutoMapper();
            services.AddSwagger();
            services.AddJwtIdentity(Configuration.GetSection(nameof(JwtConfiguration)));

            services.AddTransient<IUsersService, UsersService>();
            services.AddTransient<IProfilesService, ProfilesService>();
            services.AddTransient<IArticlesService, ArticlesService>();
            services.AddTransient<ITagsService, TagsService>();
            services.AddTransient<IArticleCommentsService, ArticleCommentsService>();
            services.AddTransient<IJwtFactory, JwtFactory>();

            services.AddMvc(options =>
            {
                options.ModelBinderProviders.Insert(0, new OptionModelBinderProvider());
                options.Filters.Add<ExceptionFilter>();
                options.Filters.Add<ModelStateFilter>();
            })
            .AddJsonOptions();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, ApplicationDbContext dbContext)
        {
            loggerFactory.AddLogging(Configuration.GetSection("Logging"));

            if (env.IsDevelopment())
            {
                dbContext.Database.EnsureCreated();
            }

            app.UseSwagger("My Web API.");
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
