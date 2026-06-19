using eVote360.Application.Interfaces.Services;
using eVote360.Application.Services;
using eVote360.Core.Application.Interfaces.Services;
using eVote360.Core.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace eVote360.Core.Application
{
    public static class ServiceRegistration
    {
        public static void AddApplicationLayer(this IServiceCollection services)
        {
            services.AddTransient<IPuestoElectivoService, PuestoElectivoService>();
            services.AddTransient<ICiudadanoService, CiudadanoService>();
            services.AddTransient<IPartidoPoliticoService, PartidoPoliticoService>();
            services.AddTransient<IUsuarioService, UsuarioService>();
            services.AddTransient<IAsignacionDirigenteService, AsignacionDirigenteService>();
            services.AddTransient<IEleccionValidationService, EleccionValidationService>();

            services.AddTransient<ICandidatoService, CandidatoService>();
            services.AddTransient<ICandidatoPuestoService, CandidatoPuestoService>();
            services.AddTransient<IAlianzaService, AlianzaService>();
            services.AddTransient<IElectorService, ElectorService>();
            services.AddTransient<IEleccionService, EleccionService>();
            services.AddTransient<IResultadoService, ResultadoService>();
            services.AddTransient<IPuestoElectivoService, PuestoElectivoService>();
            services.AddTransient<IPartidoPoliticoService, PartidoPoliticoService>();
        }
    }
}