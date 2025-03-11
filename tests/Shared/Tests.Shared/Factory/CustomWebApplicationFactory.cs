using BuildingBlocks.Core.Web.Extensions;
using BuildingBlocks.Security.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Serilog;
using Serilog.Events;
using WebMotions.Fake.Authentication.JwtBearer;
using Environments = BuildingBlocks.Core.Web.Environments;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Tests.Shared.Factory;

public class CustomWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint>, IAsyncLifetime
    where TEntryPoint : class
{
    private ITestOutputHelper? _outputHelper;
    private Action<IWebHostBuilder>? _customWebHostBuilder;
    private Action<IHostBuilder>? _customHostBuilder;
    private Action<HostBuilderContext, IConfigurationBuilder>? _configureAppConfigurations;
    private Action<IServiceCollection>? _testServices;
    private readonly Dictionary<string, string?> _inMemoryConfigs = new();

    public Action<IConfiguration>? ConfigurationAction { get; set; }
    public Action<IServiceCollection>? TestConfigureServices { get; set; }
    public Action<HostBuilderContext, IConfigurationBuilder>? TestConfigureApp { get; set; }

    public ILogger Logger => Services.GetRequiredService<ILogger<CustomWebApplicationFactory<TEntryPoint>>>();

    public void ClearOutputHelper() => _outputHelper = null;

    public void SetOutputHelper(ITestOutputHelper value) => _outputHelper = value;

    public CustomWebApplicationFactory<TEntryPoint> WithTestServices(Action<IServiceCollection> services)
    {
        _testServices += services;

        return this;
    }

    public CustomWebApplicationFactory<TEntryPoint> WithConfigureAppConfigurations(
        Action<HostBuilderContext, IConfigurationBuilder> builder
    )
    {
        _configureAppConfigurations += builder;

        return this;
    }

    public new CustomWebApplicationFactory<TEntryPoint> WithWebHostBuilder(Action<IWebHostBuilder> builder)
    {
        _customWebHostBuilder = builder;

        return this;
    }

    public CustomWebApplicationFactory<TEntryPoint> WithHostBuilder(Action<IHostBuilder> builder)
    {
        _customHostBuilder = builder;

        return this;
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment(Environments.Test);
        builder.UseContentRoot(".");

        // UseSerilog on WebHostBuilder is absolute so we should use IHostBuilder
        builder.UseSerilog(
            (ctx, loggerConfiguration) =>
            {
                if (_outputHelper is not null)
                {
                    loggerConfiguration.WriteTo.TestOutput(
                        _outputHelper,
                        LogEventLevel.Information,
                        "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level} - {Message:lj}{NewLine}{Exception}"
                    );
                }
            }
        );

        builder.UseDefaultServiceProvider(
            (env, c) =>
            {
                // Handling Captive Dependency Problem
                if (env.HostingEnvironment.IsTest() || env.HostingEnvironment.IsDevelopment())
                    c.ValidateScopes = true;
            }
        );

        builder.ConfigureAppConfiguration(
            (hostingContext, configurationBuilder) =>
            {
                // configurationBuilder.Sources.Clear();
                // IHostEnvironment env = hostingContext.HostingEnvironment;
                //
                // configurationBuilder
                //     .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                //     .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
                //     .AddJsonFile("integrationappsettings.json", true, true);
                //
                // var integrationConfig = configurationBuilder.Build();
                //
                // configurationBuilder.AddConfiguration(integrationConfig);

                //// add in-memory configuration instead of using appsettings.json and override existing settings and it is accessible via IOptions and Configuration
                configurationBuilder.AddInMemoryCollection(_inMemoryConfigs);

                ConfigurationAction?.Invoke(hostingContext.Configuration);
                _configureAppConfigurations?.Invoke(hostingContext, configurationBuilder);
                TestConfigureApp?.Invoke(hostingContext, configurationBuilder);
            }
        );

        _customHostBuilder?.Invoke(builder);

        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // test services will call after registering all application services in program.cs and can override them with `Replace` or `Remove` dependencies
        builder.ConfigureTestServices(services =>
        {
            //// Don't run IHostedServices when running as a test
            // services.RemoveAll(typeof(IHostedService));

            // TODO: Web could use this in E2E test for running another service during our test
            // services.Replace(new ServiceDescriptor(typeof(IHttpClientFactory),
            //     new DelegateHttpClientFactory(ClientProvider)));

            //// This helper just supports jwt Scheme, and for Identity server Scheme will crash so we should disable AddIdentityServer()
            // services.TryAddScoped(_ => CreateAnonymouslyUserMock());
            // services.ReplaceSingleton(CreateCustomTestHttpContextAccessorMock);
            // services.AddTestAuthentication();

            // Or
            // add authentication using a fake jwt bearer - we can use SetAdminUser method to set authenticate user to existing HttContextAccessor
            services
                // will skip registering dependencies if exists previously, but will override authentication option inner configure delegate through Configure<AuthenticationOptions>
                .AddAuthentication(options =>
                {
                    // choosing `FakeBearer` scheme (instead of exiting default scheme of application) as default in runtime for authentication and authorization middleware
                    options.DefaultAuthenticateScheme = FakeJwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = FakeJwtBearerDefaults.AuthenticationScheme;
                })
                .AddFakeJwtBearer(c =>
                {
                    // for working fake token this should be set to jwt
                    c.BearerValueType = FakeJwtBearerBearerValueType.Jwt;
                })
                .Services.AddCustomAuthorization(
                    rolePolicies: new List<RolePolicy>
                    {
                        new(Constants.Users.Admin.Role, new List<string> { Constants.Users.Admin.Role }),
                        new(Constants.Users.NormalUser.Role, new List<string> { Constants.Users.NormalUser.Role }),
                    },
                    scheme: FakeJwtBearerDefaults.AuthenticationScheme
                );

            _testServices?.Invoke(services);
            TestConfigureServices?.Invoke(services);
        });

        // //https://github.com/dotnet/aspnetcore/issues/45372
        // wb.Configure(x =>
        // {
        // });

        _customWebHostBuilder?.Invoke(builder);

        base.ConfigureWebHost(builder);
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
    }

    public void AddOverrideInMemoryConfig(string key, string value)
    {
        // overriding app configs with using in-memory configs
        // add in-memory configuration instead of using appestings.json and override existing settings and it is accessible via IOptions and Configuration
        _inMemoryConfigs.Add(key, value);
    }

    public void AddOverrideInMemoryConfig(IDictionary<string, string> inMemConfigs)
    {
        // overriding app configs with using in-memory configs
        // add in-memory configuration instead of using appsettings.json and override existing settings and it is accessible via IOptions and Configuration
        inMemConfigs.ToList().ForEach(x => _inMemoryConfigs.Add(x.Key, x.Value));
    }

    public void AddOverrideEnvKeyValue(string key, string value)
    {
        // overriding app configs with using environments
        Environment.SetEnvironmentVariable(key, value);
    }

    public void AddOverrideEnvKeyValues(IDictionary<string, string> keyValues)
    {
        foreach (var (key, value) in keyValues)
        {
            // overriding app configs with using environments
            Environment.SetEnvironmentVariable(key, value);
        }
    }

    private static IHttpContextAccessor CreateCustomTestHttpContextAccessorMock(IServiceProvider serviceProvider)
    {
        var httpContextAccessorMock = Substitute.For<IHttpContextAccessor>();
        using var scope = serviceProvider.CreateScope();
        httpContextAccessorMock.HttpContext = new DefaultHttpContext { RequestServices = scope.ServiceProvider, };

        httpContextAccessorMock.HttpContext.Request.Host = new HostString("localhost", 5000);
        httpContextAccessorMock.HttpContext.Request.Scheme = "http";
        var res = httpContextAccessorMock
            .HttpContext.AuthenticateAsync(Constants.AuthConstants.Scheme)
            .GetAwaiter()
            .GetResult();
        httpContextAccessorMock.HttpContext.User = res.Ticket?.Principal!;
        return httpContextAccessorMock;
    }
}
