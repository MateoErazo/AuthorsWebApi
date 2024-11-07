using AuthorsWebApi;

var builder = WebApplication.CreateBuilder(args);
Startup startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();
var loggerService = (ILogger<Startup>)app.Services.GetService(typeof(ILogger<Startup>));
startup.Configure(app, app.Environment, loggerService);
app.Run();
