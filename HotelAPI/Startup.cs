using Microsoft.EntityFrameworkCore;
using HotelAPI.Data;
using HotelAPI.Data.Repositories.IRepositories;
using HotelAPI.Data.Repositories;
using HotelAPI.Extensions;
using HotelAPI.Security;
using HotelAPI.Security.Core;
using HotelAPI.Services;
using HotelAPI.Services.iServices;
using System.Text.Json.Serialization;
using HotelAPI.ErrorHandling;

namespace HotelAPI
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
            // Add services to the container.
            services.AddControllers().AddJsonOptions(x =>
            {
                // serialize enums as strings in api responses (e.g. Role)
                x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            }); 
            services.AddDbContext<HotelierDBConText>(opt => opt.UseInMemoryDatabase(databaseName: "Hotelier"));
            services.AddMvc();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IJWTTokenService, JWTTokenService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.Configure<JWTTokenOptions>(Configuration.GetSection("TokenOptions"));
            services.AddAutoMapper(typeof(Startup));
            services.AddEndpointsApiExplorer();
            services.AddCustomSwagger();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            app.UseCors(x => x
             .SetIsOriginAllowed(origin => true)
             .AllowAnyMethod()
             .AllowAnyHeader()
             .AllowCredentials());

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCustomSwagger();

            app.UseAuthentication();

            app.UseAuthorization();
            
            app.UseMiddleware<ErrorHandlerMiddleware>();

            // custom jwt auth middleware
            app.UseMiddleware<JwtMiddleware>();

            app.UseEndpoints(end => end.MapControllers());

        }
    }
}