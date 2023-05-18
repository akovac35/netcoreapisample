using System.CommandLine;
using System.Runtime.Loader;
using System.Security.Claims;
using System.Text;
using Domain;
using Domain.BookRelated;
using Domain.Dtos;
using Domain.Persistance;
using FluentValidation;
using Infrastructure.Behaviors;
using Infrastructure.BookRelated;
using Infrastructure.Diagnostics;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using NLog.Extensions.Logging;
using NLog.Web;
using WebApi.Filters;
using WebApi.Middleware;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddWebApi(this IServiceCollection services, IConfigurationSection sampleConfigSection, bool useInMemoryDb)
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

            _ = services.AddWebApiBasic(useInMemoryDb);

            return services;
        }

        public static IServiceCollection AddWebApiSecurity(this IServiceCollection services, SampleConfig config)
        {
            services.AddAuthentication(options =>
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

            services.AddAuthorization(options =>
            {
                options.AddPolicy(Constants.RequireAdminRolePolicyKey, policy =>
                {
                    policy.RequireClaim(ClaimTypes.Role, Constants.AdminRoleKey);
                });
            });

            return services;
        }

        public static IServiceCollection AddWebApiBasic(this IServiceCollection services, bool useInMemoryDb)
        {
            ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Continue;
            ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Continue;

            services.AddLocalization();

            // Add scoped
            services.TryAddScoped<IBookRepository, BookRepository>();
            services.TryAddScoped<IDatabaseUnitOfWork, DatabaseUnitOfWork>();

            // Add transient
            services.TryAddTransient<IDboMapper<Book, BookDbo, Guid>, BookDboMapper>();
            services.TryAddTransient<IDtoMapper<Book, BookDto, Guid>, BookDtoMapper>();

            // Add misc
            _ = services.AddMediatR(AssemblyLoadContext.Default.Assemblies.ToArray());
            _ = services.AddValidatorsFromAssemblies(AssemblyLoadContext.Default.Assemblies, ServiceLifetime.Transient);
            _ = services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
            _ = services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            _ = services.AddMemoryCache();

            _ = services.AddDbContext<SampleDbContext>((serviceProvider, options) =>
            {
                var config = serviceProvider.GetRequiredService<IOptions<SampleConfig>>();

                _ = useInMemoryDb
                    ? options.UseInMemoryDatabase(Constants.ApplicationAcronym)
                    : options.UseSqlServer(config.Value.DbConnectionString);

                if (useInMemoryDb)
                {
                    _ = options.EnableSensitiveDataLogging();
                }
            });

            return services;
        }
    }
}
