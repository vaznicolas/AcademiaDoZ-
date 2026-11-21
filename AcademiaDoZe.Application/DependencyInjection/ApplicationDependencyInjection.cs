
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

        // Fábrica do AcessoAlunoRepository
        services.AddTransient(
            provider =>
            {
                var config =
                    provider.GetRequiredService<RepositoryConfig>();

                return (Func<IAcessoAlunoRepository>)(() =>
                    new AcessoAlunoRepository(
                        config.ConnectionString,
                        config.DatabaseType,
                        new AlunoRepository(
                            config.ConnectionString,
                            config.DatabaseType)));
            });


        // Fábrica do AcessoColaboradorRepository
        services.AddTransient(
            provider =>
            {
                var config =
                    provider.GetRequiredService<RepositoryConfig>();

                return (Func<IAcessoColaboradorRepository>)(() =>
                    new AcessoColaboradorRepository(
                        config.ConnectionString,
                        config.DatabaseType,
                        new ColaboradorRepository(
                            config.ConnectionString,
                            config.DatabaseType)));
            });


        // Serviço de controle de acesso dos alunos
        services.AddTransient<IAcessoAlunoService>(provider =>
            new AcessoAlunoService(
                provider.GetRequiredService<Func<IAlunoRepository>>()(),
                provider.GetRequiredService<Func<IMatriculaRepository>>()(),
                provider.GetRequiredService<Func<IAcessoAlunoRepository>>()()
            ));

        // Serviço de controle de acesso dos colaboradores
        services.AddTransient<IAcessoColaboradorService>(provider =>
            new AcessoColaboradorService(
                provider.GetRequiredService<Func<IColaboradorRepository>>()(),
                provider.GetRequiredService<Func<IAcessoColaboradorRepository>>()()
            ));

        return services;
    }
}
