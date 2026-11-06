using ConcesionarioMotos.Web.Pages;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddSingleton<EstadoConcesionario>();

var app = builder.Build();
if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler("/Error");

app.UseStaticFiles();
app.MapRazorPages();
app.Run();
