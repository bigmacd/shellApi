using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text;
using Swashbuckle.AspNetCore.Swagger;
using System.Data.SqlClient;
using shellApi.Models;
using shellApi.Schemas;

namespace shellApi
{

    public class Startup
    {
        string environment;
        ILogger _logger;
        public Startup(IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Startup>();
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            //environment = env.EnvironmentName;
            environment = Environment.GetEnvironmentVariable("environment");
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            string appConnString = "ConnectionStrings:" + environment;
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(Configuration[appConnString]);
            builder.IntegratedSecurity = false;
            services.AddDbContext<ShellContext>(options => options.UseSqlServer(builder.ConnectionString));
            services.AddSingleton<IConfiguration>(Configuration);

            string hostString = "Statsd:" + environment + ":host";
            string portString = "Statsd:" + environment + ":port";
            services.AddSingleton<StatsN.IStatsd>(provider => new StatsN.Statsd(new StatsN.StatsdOptions()
            {
                HostOrIp = Configuration[hostString],
                Port = Convert.ToInt32(Configuration[portString])
            }));
             
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", 
                new Info 
                { 
                    Title = "Shell API", 
                    Version = "Version 1",
                    Description = "Reference Implementation",
                    TermsOfService = "Limited"
                });
                //c.IncludeXmlComments("shellApi.xml");
                c.SchemaFilter<ShellSchemas>();
            });

            services.AddSingleton<IAuditPublisher>(provider => new AuditPublisher(Configuration, environment));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));
            app.UseMvc();
            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swagger, httpReq) => swagger.Host = httpReq.Host.Value);

            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/shellApi/swagger/v1/swagger.json", "Proxy Access");
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Local Access");
            });
        }
    }
}
