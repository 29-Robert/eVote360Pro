using eVote360.Core.Application;
using eVote360.Core.Application.Interfaces.Repositories;
using eVote360.Core.Application.Interfaces.Services;
using eVote360.Core.Application.Services;
using eVote360.Infrastructure.Persistence;
using eVote360.Infrastructure.Persistence.Repositories;
using eVote360.Infrastructure.Persistence.Seeders;
using eVote360.Infrastructure.Shared;
using eVote360.Infrastructure.Shared.Services;
using eVote360.Shared.Settings;
using eVote360.WebApp.Ocr;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

builder.Services.AddApplicationLayer();
builder.Services.AddPersistenceInfrastructure(builder.Configuration);
builder.Services.AddSharedInfrastructure();

builder.Services.AddTransient<IOcrService, TesseractOcrService>();

builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<IOtpService, OtpService>();
builder.Services.AddScoped<IEleccionRepository, EleccionRepository>();
builder.Services.AddScoped<ICandidatoRepository, CandidatoRepository>();
builder.Services.AddScoped<IAlianzaRepository, AlianzaRepository>();
builder.Services.AddScoped<ICandidatoPuestoRepository, CandidatoPuestoRepository>();
builder.Services.AddScoped<IPartidoPoliticoRepository, PartidoPoliticoRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DefaultAdminUser.SeedAsync(services);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Elector}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();