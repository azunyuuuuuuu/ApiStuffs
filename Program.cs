using Ical.Net;
using Ical.Net.Serialization;
using Ical.Net.Proxies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication.CreateBuilder(args);

// setup the modules
builder.RegisterModules();

// register additional services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpLogging(logging =>
    {
        // Customize HTTP logging here.
        logging.LoggingFields =
            HttpLoggingFields.RequestProperties | HttpLoggingFields.ResponsePropertiesAndHeaders;
        // logging.RequestHeaders.Clear();
        // logging.ResponseHeaders.Clear();
        // logging.MediaTypeOptions.AddText("application/javascript");
        logging.RequestBodyLogLimit = 4096;
        logging.ResponseBodyLogLimit = 4096;
    });

// build the app
var app = builder.Build();

app.UseHttpLogging();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

// explore the api
app.UseSwagger();
app.UseSwaggerUI();

// map the endpoints

app.MapGet("/", ctx =>
{
    ctx.Response.ContentType = "text/html";
    return ctx.Response.SendFileAsync("index.html");
});

app.MapEndpoints();

app.Run();
