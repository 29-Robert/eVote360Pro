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

// 1. Registro de Servicios del Framework
builder.Services.AddControllersWithViews();
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// 2. Registro de las Capas del Proyecto (Onion Architecture)
builder.Services.AddApplicationLayer();
builder.Services.AddPersistenceInfrastructure(builder.Configuration);
builder.Services.AddSharedInfrastructure();

builder.Services.AddTransient<IOcrService, TesseractOcrService>();

builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<IOtpService, OtpService>();
builder.Services.AddTransient<IHomeDirigenteService, HomeDirigenteService>();
builder.Services.AddScoped<ICandidatoService, CandidatoService>();
builder.Services.AddScoped<ICandidatoPuestoService, CandidatoPuestoService>();


// 3. Registro de Repositorios Específicos
builder.Services.AddScoped<IEleccionRepository, EleccionRepository>();
builder.Services.AddScoped<ICandidatoRepository, CandidatoRepository>();
builder.Services.AddScoped<IAlianzaRepository, AlianzaRepository>();
builder.Services.AddScoped<ICandidatoPuestoRepository, CandidatoPuestoRepository>();
builder.Services.AddScoped<IPartidoPoliticoRepository, PartidoPoliticoRepository>();
builder.Services.AddTransient<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IPuestoElectivoRepository, PuestoElectivoRepository>();

// 4. Servicio requerido por CandidatoPuestoService para Claims/Usuarios (¡Colocado correctamente aquí!)
builder.Services.AddHttpContextAccessor();

// 5. Construcción de la Aplicación (Cierre del contenedor)
var app = builder.Build();

// 6. Ejecución de Seeders (Datos iniciales)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DefaultAdminUser.SeedAsync(services);
}



// 7. Configuración del Pipeline de HTTP (Middlewares)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Elector}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();