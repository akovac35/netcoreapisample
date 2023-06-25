using System.Runtime.Loader;
using System.Security.Claims;
using System.Text;
using Domain;
using Domain.AuthorRelated;
using Domain.BookRelated;
using Domain.ConsentRelated;
using Domain.Dtos;
using Domain.Persistance;
using FluentValidation;
using Infrastructure.AuthorRelated;
using Infrastructure.Behaviors;
using Infrastructure.BookRelated;
using Infrastructure.ConsentRelated;
using Infrastructure.Diagnostics;
using Infrastructure.Migrations.Oracle;
using Infrastructure.Migrations.SqlServer;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;
using WebApi.Filters;
using WebApi.Middleware;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddWebApi(this IServiceCollection services, IConfigurationSection sampleConfigSection)
        {
            // Add services to the container.
            _ = services
                .AddOptions<SampleConfig>()
                .Bind(sampleConfigSection)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            var config = sampleConfigSection.Get<SampleConfig>() ?? throw new InvalidOperationException();

            _ = services.AddControllers()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });
            _ = services.AddHttpContextAccessor();

            _ = services.AddWebApiSecurity(config);

            _ = services.AddEndpointsApiExplorer();
            _ = services.AddSwaggerGen(c =>
            {
                c.OperationFilter<DefaultResponseOperationFilter>();

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Description = "JWT Authorization header using the Bearer scheme"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new List<string>()
                    }
                });

                static string SwashbuckleNameFactory(Type type)
                {
                    var name = type.FullName!.Contains('+', StringComparison.InvariantCulture)
                            ? type.FullName!
                            .Replace($"{type.Namespace}.", string.Empty, StringComparison.InvariantCulture)
                            .Replace("+", string.Empty, StringComparison.InvariantCulture)
                            : type.Name;

                    if (type.IsGenericType)
                    {
                        var genericArgs = string.Join("And", type.GetGenericArguments().Select(SwashbuckleNameFactory));

                        var index = name.IndexOf('`', StringComparison.InvariantCulture);
                        var typeNameWithoutGenericArity = index == -1 ? name : name[..index];

                        name = $"{typeNameWithoutGenericArity}Of{genericArgs}";
                    }

                    return name;
                }

                c.CustomSchemaIds(SwashbuckleNameFactory);

                // Enables support for nullable object properties
                c.UseAllOfToExtendReferenceSchemas();
                // Enable detection of non nullable reference types to set Nullable flag accordingly on schema properties
                c.SupportNonNullableReferenceTypes();
            })
            .AddSwaggerGenNewtonsoftSupport();

            services.EnableActivityLogging();

            services.TryAddTransient<ActivityMiddleware>();
            services.TryAddTransient<ExceptionHandlingMiddleware>();

            _ = services.AddLogging(configure =>
            {
                _ = configure.ClearProviders();
                _ = configure.AddNLog();
            });

            _ = services.AddWebApiEssentials();

            return services;
        }

        public static IServiceCollection AddWebApiSecurity(this IServiceCollection services, SampleConfig config)
        {
            _ = services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                // https is handled by hosting
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = Constants.ApplicationAcronym,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.JwtConfig.JwtSecretKey)),
                    ValidAudience = Constants.JwtAudience
                };
            });

            _ = services.AddAuthorization(options =>
            {
                options.AddPolicy(Constants.RequireAdminRolePolicyKey, policy =>
                {
                    _ = policy.RequireClaim(ClaimTypes.Role, Constants.AdminRoleKey);
                });
            });

            return services;
        }

        // Required for in-memory database. If there are no open connections, the database is deleted
        public static SqliteConnection? InMemoryKeepAliveConnection { get; private set; }

        public static IServiceCollection AddWebApiEssentials(this IServiceCollection services)
        {
            var logger = LogManager.GetCurrentClassLogger();

            ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Continue;
            ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Continue;

            _ = services.AddLocalization();

            // Add scoped
            services.TryAddScoped<IBookRepository, BookRepository>();
            services.TryAddScoped<IAuthorRepository, AuthorRepository>();
            services.TryAddScoped<IConsentRepository, ConsentRepository>();
            services.TryAddScoped<IDatabaseUnitOfWork, DatabaseUnitOfWork>();

            // Add transient
            services.TryAddTransient<IDtoMapper<BookDbo, BookDto, Guid>, BookDtoMapper>();
            services.TryAddTransient<IDtoMapper<AuthorDbo, AuthorDto, Guid>, AuthorDtoMapper>();
            services.TryAddTransient<IDtoMapper<ConsentDbo, ConsentDto, Guid>, ConsentDtoMapper>();

            // Add misc
            _ = services.AddMediatR(AssemblyLoadContext.Default.Assemblies.ToArray());
            _ = services.AddValidatorsFromAssemblies(AssemblyLoadContext.Default.Assemblies, ServiceLifetime.Transient);
            _ = services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
            _ = services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            _ = services.AddMemoryCache();

            _ = services.AddDbContext<SampleDbContext>((serviceProvider, options) =>
            {
                var config = serviceProvider.GetRequiredService<IOptions<SampleConfig>>();

                if (config.Value.UseInMemoryDb != null)
                {
                    if (config.Value.UseInMemoryDb == true
                        && InMemoryKeepAliveConnection == null)
                    {
                        var keepAliveConnection = new SqliteConnection(config.Value.DbConnectionString);
                        keepAliveConnection.Open();
                        InMemoryKeepAliveConnection = keepAliveConnection;
                    }

                    if(config.Value.UseInMemoryDb.Value)
                    {
                        logger.Info("Using Sqlite");
                        options.UseSqlite(config.Value.DbConnectionString);
                    }
                    else
                    {
                        if (config.Value.DbType == DbType.SqlServer)
                        {
                            logger.Info("Using SqlServer");
                            options.UseSqlServer(config.Value.DbConnectionString,
                                opt => opt.MigrationsAssembly(typeof(SqlServerMigrationsMarker).Assembly.GetName().Name));
                        }
                        else if (config.Value.DbType == DbType.Oracle)
                        {
                            logger.Info("Using Oracle");
                            options.UseOracle(config.Value.DbConnectionString,
                                opt => opt.MigrationsAssembly(typeof(OracleMigrationsMarker).Assembly.GetName().Name));
                        }
                        // else we do nothing
                    }

                    if (config.Value.UseInMemoryDb.Value)
                    {
                        _ = options.EnableSensitiveDataLogging();
                    }
                }
                // else we skip context configuration
            });

            return services;
        }

        public static async Task InitializeWebApiAsync(this IServiceProvider services)
        {
            var logger = LogManager.GetCurrentClassLogger();

            using var serviceScope = services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var config = serviceScope.ServiceProvider.GetRequiredService<IOptions<SampleConfig>>();
            var hostApplicationLifetime = serviceScope.ServiceProvider.GetRequiredService<IHostApplicationLifetime>();

            if (config.Value.UseInMemoryDb == false)
            {
                logger.Info("Migrating database");

                var context = serviceScope.ServiceProvider.GetRequiredService<SampleDbContext>();
                await context.Database.MigrateAsync(cancellationToken: hostApplicationLifetime.ApplicationStopping);
            }
            else if (config.Value.UseInMemoryDb == true)
            {
                logger.Info("Creating in-memory database");

                var context = serviceScope.ServiceProvider.GetRequiredService<SampleDbContext>();
                _ = await context.Database.EnsureCreatedAsync(hostApplicationLifetime.ApplicationStopping);
            }
            // else we skip the db initialization
        }

        public static async Task DeleteDbAsync(this IServiceProvider services)
        {
            var logger = LogManager.GetCurrentClassLogger();

            using var serviceScope = services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var config = serviceScope.ServiceProvider.GetRequiredService<IOptions<SampleConfig>>();
            var hostApplicationLifetime = serviceScope.ServiceProvider.GetRequiredService<IHostApplicationLifetime>();

            if (config.Value.UseInMemoryDb == false)
            {
                logger.Info("Deleting database");

                var context = serviceScope.ServiceProvider.GetRequiredService<SampleDbContext>();
                await context.Database.EnsureDeletedAsync(hostApplicationLifetime.ApplicationStopping);
            }
        }
    }
}
