using System.CommandLine;
using Domain;
using Infrastructure.CommandLine;
using NLog;
using NLog.Web;

var logger = NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();

try
{
    SampleRootCommand rootCommand = new();
    var useInMemoryDbOption = rootCommand.AddUseInMemoryDbOption();

    rootCommand.SetHandler(async (context) =>
    {
        var useInMemoryDbOptionValue = context.ParseResult.GetValueForOption(useInMemoryDbOption);

        var builder = WebApplication.CreateBuilder(args);
        _ = builder.WebHost.UseDefaultServiceProvider((context, options) =>
        {
            options.ValidateScopes = true;
        });

        var sampleConfigSection = builder.Configuration.GetRequiredSection(nameof(SampleConfig));

        _ = builder.Services.AddWebApi(sampleConfigSection, useInMemoryDbOptionValue);

        var app = builder.Build();

        _ = app.UseWebApi();

        await app.InitializeWebApi(useInMemoryDb: useInMemoryDbOptionValue);

        await app.RunAsync(context.GetCancellationToken());
    });

    logger.Info("Starting host");
    _ = await rootCommand.InvokeAsync(args);
    logger.Info("Exiting host");
}
catch (Exception ex)
{
    logger.Fatal(ex, ex.Message);
    throw;
}
finally
{
    try
    {
        LogManager.Flush();
    }
    catch
    {
    }

    try
    {
        LogManager.Shutdown();
    }
    catch
    {
    }
}
