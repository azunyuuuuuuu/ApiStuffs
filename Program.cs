using Ical.Net;
using Ical.Net.Serialization;
using Ical.Net.Proxies;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// setup the modules
builder.RegisterModules();

// register additional services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// build the app
var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

// explore the api
app.UseSwagger();
app.UseSwaggerUI();

// map the endpoints
app.MapEndpoints();

app.Run();
