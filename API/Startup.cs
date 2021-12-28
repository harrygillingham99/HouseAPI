namespace House.API
{
    using System.CodeDom.Compiler;
    using DAL;
    using HLL;
    using HLL.Dashboard.Bindicator;
    using HLL.Dashboard.Bindicator.Models;
    using HLL.Dashboard.WeatherFeed.Models;
    using HLL.News.Models;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Formatters;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using NJsonSchema;
    using Scrutor;

    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(cfg =>
            {
                cfg.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();
            });

            services.AddOpenApiDocument(c =>
            {
                c.Title = "House API";
                c.Description = "An api for the house";
                c.SchemaType = SchemaType.OpenApi3;
            });

            services.AddLazyCache();

            services.AddCors(options =>
            {
                options.AddPolicy("SiteCorsPolicy", builder =>
                {
                    builder
                        .AllowAnyHeader()
                        .WithMethods("GET", "POST", "PUT", "DELETE")
                        .WithOrigins("http://localhost", 
                                     "https://localhost", 
                                     "https://localhost:44359", 
                                     "https://localhost:44370",
                                     "http://localhost:62110/",
                                     "http://192.168.0.69:420", 
                                     "http://192.168.1.69");
                });
            });

            services.Configure<Lookup>(option => Configuration.GetSection("Lookup").Bind(option));
            services.Configure<ConnectionStrings>(option => Configuration.GetSection("ConnectionStrings").Bind(option));
            services.Configure<OpenWeatherApi>(option => Configuration.GetSection("OpenWeatherApi").Bind(option));
            services.Configure<NewsApi>(option => Configuration.GetSection("NewsApi").Bind(option));
            services.Configure<DbConnections>(option => Configuration.GetSection("DbConnections").Bind(option));
            ScanForAllRemainingRegistrations(services);
        }

        public static void ScanForAllRemainingRegistrations(IServiceCollection services)
        {
            services.Scan(scan => scan
                .FromAssembliesOf(typeof(Startup), typeof(BindicatorProvider), typeof(BaseRepository))
                .AddClasses(x => x.WithoutAttribute(typeof(GeneratedCodeAttribute)))
                .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                .AsImplementedInterfaces()
                .WithScopedLifetime()); 
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
                app.UseHsts();
            }

            app.UseCors("SiteCorsPolicy");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseOpenApi();

            app.UseSwaggerUi3();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
