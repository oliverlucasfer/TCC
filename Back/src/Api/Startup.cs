using System.IO;
using Api.Application;
using Api.Application.Contratos;
using Api.Domain.Identity;
using Api.Persistence;
using Api.Persistence.Contexto;
using Api.Infrastructure;
using Api.Application.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Api
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
            services.AddCors(options =>
            {
                options.AddPolicy("Front", policy =>
                    policy.WithOrigins(Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new[] { "http://localhost:4200" })
                          .AllowAnyHeader()
                          .AllowAnyMethod());
            });
            services.AddDbContext<ApiContext>(
                context => context.UseSqlite(Configuration.GetConnectionString("Default"))
            );

            var jwtSection = Configuration.GetSection(JwtOptions.SectionName);
            services.Configure<JwtOptions>(jwtSection);
            var jwtOptions = jwtSection.Get<JwtOptions>() ?? new JwtOptions();
            if (string.IsNullOrEmpty(jwtOptions.TokenKey))
                throw new InvalidOperationException(
                    "TokenKey não configurada. Em Development use 'dotnet user-secrets set \"TokenKey\" \"<chave>\"'; em produção, defina a variável de ambiente TokenKey.");
            if (Encoding.UTF8.GetBytes(jwtOptions.TokenKey).Length < 64)
                throw new InvalidOperationException(
                    "TokenKey muito curta. HS512 exige chave de pelo menos 64 bytes. Use: openssl rand -base64 48");

            services.AddIdentityCore<User>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 8;
            })
            .AddRoles<Role>()
            .AddRoleManager<RoleManager<Role>>()
            .AddSignInManager<SignInManager<User>>()
            .AddRoleValidator<RoleValidator<Role>>()
            .AddEntityFrameworkStores<ApiContext>()
            .AddDefaultTokenProviders();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.TokenKey)),
                            ValidateIssuer = true,
                            ValidIssuer = jwtOptions.Issuer,
                            ValidateAudience = true,
                            ValidAudience = jwtOptions.Audience,
                            ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha512 },
                            ClockSkew = TimeSpan.FromMinutes(1)
                        };
                    });

            services.AddControllers();
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddHealthChecks();
            services.AddHostedService<DbInitializer>();

            services.AddScoped<IDocumentoService, DocumentoService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IFileService, DocumentoFileService>();
            services.AddScoped<IBackupService, BackupService>();

            services.AddScoped<IGeralPersistence, GeralPersistence>();
            services.AddScoped<IDocumentoPersistence, DocumentoPersistence>();
            services.AddScoped<IUserPersistence, UserPersistence>();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Api", Version = "v1" });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header usando Bearer.
                                Entre com 'Bearer' [espaço] então coloque seu token.
                                Exemplo: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecuritySchemeReference("Bearer", document),
                        new List<string>()
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api v1"));
            }

            app.Use(async (context, next) =>
            {
                var headers = context.Response.Headers;
                headers["X-Content-Type-Options"] = "nosniff";
                headers["X-Frame-Options"] = "DENY";
                headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
                headers["Content-Security-Policy"] = "default-src 'self'; style-src 'self' 'unsafe-inline'; img-src 'self' data:";
                if (!env.IsDevelopment())
                {
                    headers["Strict-Transport-Security"] = "max-age=63072000; includeSubDomains";
                }
                await next();
            });

            if (!env.IsDevelopment())
            {
                app.UseDefaultFiles();
                app.UseStaticFiles();
            }

            app.UseRouting();

            app.UseCors("Front");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/api/health");
                if (!env.IsDevelopment())
                {
                    endpoints.MapFallbackToFile("index.html");
                }
            });
        }
    }
}
