using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NineNineQuotes
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<Microsoft.AspNetCore.Routing.RouteOptions>(options => options.LowercaseUrls = true);

            services.AddDbProvider(Environment, Configuration);

            services.AddScoped<Services.QuoteService>();

            services.AddControllers(options => options.OutputFormatters.RemoveType<Microsoft.AspNetCore.Mvc.Formatters.StringOutputFormatter>())
                .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Brooklyn-99-Quotes", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Brooklyn-99-Quotes v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    public static class AddDbProviderExtensions
    {
        public static IServiceCollection AddDbProvider(this IServiceCollection services, IWebHostEnvironment env, IConfiguration config)
        {
            string connStr = "";
            if (env.EnvironmentName == "Development")
            {
                connStr = config.GetConnectionString("DefaultConnection");
            }

            if (env.EnvironmentName == "Production")
            {
                string connUrl = config.GetSection("DATABASE_URL").Value;

                // Parse connection URL to connection string for Npgsql
                connUrl = connUrl.Replace("postgres://", string.Empty);

                string pgUserPass = connUrl.Split("@")[0];
                string pgHostPortDb = connUrl.Split("@")[1];
                string pgHostPort = pgHostPortDb.Split("/")[0];

                string pgDb = pgHostPortDb.Split("/")[1];
                string pgUser = pgUserPass.Split(":")[0];
                string pgPass = pgUserPass.Split(":")[1];
                string pgHost = pgHostPort.Split(":")[0];
                string pgPort = pgHostPort.Split(":")[1];

                connStr = $"Server={pgHost};Port={pgPort};User Id={pgUser};Password={pgPass};Database={pgDb};sslmode=Prefer;Trust Server Certificate=true";
            }

            services.AddDbContext<Data.AppDbContext>(options => options.UseNpgsql(connStr,
                o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));
            return services;
        }
    }
}
