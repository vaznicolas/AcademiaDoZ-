// Nicolas Vaz

using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Application.Services;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace AcademiaDoZe.Application.DependencyInjection;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        // Serviços da camada Application
        services.AddTransient<
            ILogradouroService,
            LogradouroService>();

        services.AddTransient<
            IColaboradorService,
            ColaboradorService>();

        services.AddTransient<
            IAlunoService,
            AlunoService>();

        services.AddTransient<
            IMatriculaService,
            MatriculaService>();

        // Fábrica do LogradouroRepository
        services.AddTransient(
            provider =>
            {
                var config =
                    provider.GetRequiredService<RepositoryConfig>();

                return (Func<ILogradouroRepository>)(() =>
                    new LogradouroRepository(
                        config.ConnectionString,
                        config.DatabaseType));
            });

        // Fábrica do ColaboradorRepository
        services.AddTransient(
            provider =>
            {
                var config =
                    provider.GetRequiredService<RepositoryConfig>();

                return (Func<IColaboradorRepository>)(() =>
                    new ColaboradorRepository(
                        config.ConnectionString,
                        config.DatabaseType));
            });

        // Fábrica do AlunoRepository
        services.AddTransient(
            provider =>
            {
                var config =
                    provider.GetRequiredService<RepositoryConfig>();

                return (Func<IAlunoRepository>)(() =>
                    new AlunoRepository(
                        config.ConnectionString,
                        config.DatabaseType));
            });

        // Fábrica do MatriculaRepository
        services.AddTransient(
            provider =>
            {
                var config =
                    provider.GetRequiredService<RepositoryConfig>();

                return (Func<IMatriculaRepository>)(() =>
                    new MatriculaRepository(
                        config.ConnectionString,
                        config.DatabaseType));
            });

        return services;
    }
}