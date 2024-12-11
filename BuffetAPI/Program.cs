using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BuffetAPI.Data;
using Serilog;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BuffetAPI.Auth;
using BuffetAPI.Configurations;

namespace BuffetAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<BuffetAPIContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("BuffetAPIContext") ?? throw new InvalidOperationException("Connection string 'BuffetAPIContext' not found.")));

            // Add services to the container.
            /**/
            builder.Services.AddIdentityCore<IdentityUser>()
                .AddRoles<IdentityRole>()
                /*?*/.AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("HotelListingApi")
                .AddEntityFrameworkStores<BuffetAPIContext>()
                .AddDefaultTokenProviders();
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            /*...*/
            builder.Services.AddSwaggerGen();
            builder.Host.UseSerilog((context, config) =>
            {
                config.WriteTo.Console().ReadFrom.Configuration(context.Configuration);
            });

            builder.Services.AddAutoMapper(typeof(AutoMapperConfig));

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidAudience = builder.Configuration["JWT:Audience"],
                        ValidIssuer = builder.Configuration["JWT:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
                    };
                });
            builder.Services.AddScoped<IAuthManager, AuthManager>();
            var app = builder.Build();
            /*...*/
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseSerilogRequestLogging();

            app.UseHttpsRedirection();

            /**/
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
