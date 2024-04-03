using Apito.Extensions;
using Apito.Models;

var builder = WebApplication.CreateBuilder(args);
await builder.Services.AddAll(builder.Configuration);

var app = builder.Build();
app.AddAllUses();

app.RegisterAllEndpoints();

app.MapGet("/version", () => { return AppValues.Version; }).WithName("GetVersion").WithOpenApi();

app.Run();

//https://www.youtube.com/watch?v=9MOpm5id2AI&list=PLgRlicSxjeMOUGRV28LGyqDgL0IySmGJ6&index=3