using AuthorsWebApi.Filters;
using AuthorsWebApi.Services;
using AuthorsWebApi.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

namespace AuthorsWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(MyGlobalExceptionFilter));
                options.Conventions.Add(new SwaggerGroupByVersion());
            })
                .AddJsonOptions(options => 
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
                .AddNewtonsoftJson();

            services.AddDbContext<ApplicationDbContext>( options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Configuration["JwtKey"])
                    ),
                    ClockSkew = TimeSpan.Zero
                });

            services.AddAutoMapper(typeof(Startup));

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen( c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthorsWebApi", Version = "v1"});
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "AuthorsWebApi", Version = "v2" });


                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name="Authorization",
                    Type=SecuritySchemeType.ApiKey,
                    Scheme="Bearer",
                    BearerFormat = "JWT",
                    In= ParameterLocation.Header
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });

                c.OperationFilter<AddHeaderHATEOAS>();
            });

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("IsAdmin", policy =>
                {
                    policy.RequireClaim("isAdmin", new string[] {"1"});
                });
            });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins(new string[]
                    {
                        ""
                    }).AllowAnyMethod().AllowAnyHeader();
                });
            });

            services.AddDataProtection();

            services.AddTransient<HashService>();

            services.AddTransient<LinksGenerator>();
            services.AddTransient<HATEOASAuthorFilterAttribute>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI( c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json","AuthorsWebApi v1");
                    c.SwaggerEndpoint("/swagger/v2/swagger.json", "AuthorsWebApi v2");
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });

        }
    }
}
