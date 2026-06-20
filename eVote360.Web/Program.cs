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
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Index";
        options.AccessDeniedPath = "/Home/AccesoDenegado";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    });

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddApplicationLayer();
builder.Services.AddPersistenceInfrastructure(builder.Configuration);
builder.Services.AddSharedInfrastructure();

builder.Services.AddTransient<IOcrService, TesseractOcrService>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<IOtpService, OtpService>();
builder.Services.AddTransient<IHomeDirigenteService, HomeDirigenteService>();
builder.Services.AddScoped<ICandidatoService, CandidatoService>();
builder.Services.AddScoped<ICandidatoPuestoService, CandidatoPuestoService>();
builder.Services.AddScoped<IEleccionRepository, EleccionRepository>();
builder.Services.AddScoped<ICandidatoRepository, CandidatoRepository>();
builder.Services.AddScoped<IAlianzaRepository, AlianzaRepository>();
builder.Services.AddScoped<ICandidatoPuestoRepository, CandidatoPuestoRepository>();
builder.Services.AddScoped<IPartidoPoliticoRepository, PartidoPoliticoRepository>();
builder.Services.AddTransient<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IPuestoElectivoRepository, PuestoElectivoRepository>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); 
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();    

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Elector}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DefaultAdminUser.SeedAsync(services);
}

app.Run();