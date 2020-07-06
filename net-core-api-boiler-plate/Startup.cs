using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using net_core_api_boiler_plate.Database.DB;
using net_core_api_boiler_plate.Extensions.Services;
using Serilog;

namespace net_core_api_boiler_plate
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLoggingService(Configuration);
            Log.Information("Adding Controller and NewtonSoftJson Service");
            services.AddControllers().AddNewtonsoftJson();
            Log.Information("Adding Swashbuckle Service");
            services.AddSwashbuckleService();
            Log.Information("Adding TestController Service");
            services.AddTestControllerService();
            Log.Information("Adding Database Service");
            services.AddDatabaseService(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DatabaseContext db)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            db.Database.EnsureCreated();
            db.Database.Migrate();
        }
    }
}
