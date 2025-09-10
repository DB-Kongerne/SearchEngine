using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using SearchAPI;
using Shared;
using Shared.Database;
using SearchWeb.Shared;
using SearchWeb.Pages;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Add SearchLogic and database using the DatabaseFactory
var db = DatabaseFactory.CreateDatabase(DatabaseFactory.DatabaseType.Sqlite);
builder.Services.AddSingleton<IDatabase>(db);
builder.Services.AddSingleton<SearchLogic>();

// No need to register JSInvokable methods as they're static

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
