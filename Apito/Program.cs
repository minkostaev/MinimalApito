using Apito.Extensions;
using Apito.Models;

var builder = WebApplication.CreateBuilder(args);
AppSettings.Init(builder.Configuration);//appsettings.json to obj
await builder.Services.AddAll();//Extension
var app = builder.Build();
///app.Environment.IsDevelopment();
app.AddAllUses();//Extension
app.RegisterAllEndpoints();//Extension
await app.RunAsync();

// issue:
//Unable to start Kestrel. System.IO.IOException: Failed to bind to address
//http://127.0.0.1:5000: address already in use
// solution:
//netsh interface ipv4 show excludedportrange protocol=tcp
//net stop winnat
//net start winnat

// header in request -      x-api-version

// to checkout
// https://geektest.co/programming-tests

//https://www.youtube.com/watch?v=9MOpm5id2AI&list=PLgRlicSxjeMOUGRV28LGyqDgL0IySmGJ6&index=3
