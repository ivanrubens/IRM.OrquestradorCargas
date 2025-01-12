using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IRM.OrquestradorWorker;

public class Program
{
    public static void Main(string[] args)
    {
        //var builder = Host.CreateApplicationBuilder(args);
        //builder.Services.AddHostedService<Worker>();

        //var host = builder.Build();
        //host.Run();

        var builder = Host.CreateApplicationBuilder(args);

        // Configura o servi�o HTTP para a classe CargaSequencialCallbackMultipleService
        builder.Services.AddHttpClient<OrquestradorCargasService>(client =>
        {
            client.BaseAddress = new Uri("https://localhost:7287");
        });

        // Registra o Worker como um servi�o hospedado
        builder.Services.AddHostedService<Worker>();

        var app = builder.Build();

        // Executa o aplicativo
        app.Run();

    }
}