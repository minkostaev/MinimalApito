using Apito.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Init();//Extension
await builder.Services.AddAll();//Extension
builder.Services.AddSwaggerServices();//Extension
var app = builder.Build();
app.AddAllUses();//Extension
app.RegisterAllEndpoints();//Extension
app.AddSwaggerUses();//Extension
await app.RunAsync();

// issue:
//Unable to start Kestrel. System.IO.IOException: Failed to bind to address
//http://127.0.0.1:5000: address already in use
// solution:
//netsh interface ipv4 show excludedportrange protocol=tcp
//net stop winnat
//net start winnat

// to checkout
// https://geektest.co/programming-tests
// https://www.youtube.com/watch?v=9MOpm5id2AI&list=PLgRlicSxjeMOUGRV28LGyqDgL0IySmGJ6&index=3
