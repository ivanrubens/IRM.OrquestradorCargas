using IRM.OrquestradorConsole;

namespace IRM.OrquestradorConsole;

internal class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Iniciando processamento...");

        // Define o callback que será chamado quando a tarefa terminar
        Action<string> callback = resultado =>
        {
            Console.WriteLine($"Callback chamado! Resultado: {resultado}");
        };

        // Chama o método assíncrono com o callback
        await ProcessarAsync(callback);

        Console.WriteLine("Processamento finalizado.");
    }

    static async Task ProcessarAsync(Action<string> callback)
    {
        HttpClient httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri("https://localhost:7287");

        var cargaService = new CargaSequencialCallbackSingleService(httpClient);
        await cargaService.ProcessarCargasAsync();

        // Resultado da tarefa
        string resultado = "Tarefa concluída com sucesso!";

        // Invoca o callback com o resultado
        callback(resultado);
    }
}
