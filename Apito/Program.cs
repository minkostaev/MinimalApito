using Apito.Extensions;

var builder = WebApplication.CreateBuilder(args);
await builder.Services.AddAll(builder.Configuration);//Extension
var app = builder.Build();
app.AddAllUses();//Extension
app.RegisterAllEndpoints();//Extension
app.Run();