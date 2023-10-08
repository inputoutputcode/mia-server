using Game.Server.Web.Data;
using Game.Server.Web.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSignalR();

builder.Services.AddSingleton(_ => {
    var buffer = new Buffer<Point>(10);
    // start with something that can grow
    for (var i = 0; i < 7; i++)
        buffer.AddNewRandomPoint();

    return buffer;
});

builder.Services.AddHostedService<ChartValueGenerator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.UseRouting();
app.MapHub<ScoreHub>(ScoreHub.Url);

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
