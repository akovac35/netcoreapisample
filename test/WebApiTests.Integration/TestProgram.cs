using Domain;

var appBuilder = WebApplication.CreateBuilder(args);
var services = appBuilder.Services;
var sampleConfigSection = appBuilder.Configuration.GetRequiredSection(nameof(SampleConfig));

appBuilder.Services.AddWebApi(sampleConfigSection);

using var app = appBuilder.Build();

app.UseWebApi();

await app.Services.DeleteDbAsync();
await app.Services.InitializeWebApiAsync();

app.Run();

public partial class TestProgram { }
