using IRM.Orquestrador.Shared.Models;
using Polly;
using System.Text;
using System.Text.Json;

namespace IRM.OrquestradorWorker;

public class OrquestradorCargasService
{
    private readonly HttpClient _httpClient;

    public OrquestradorCargasService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task ProcessarCargasAsync()
    {
        // Obter a lista de cargas
        Console.WriteLine("Obtendo nova lista de cargas a processar...");

        var listaCargas = await ObterListaCargasAsync();
        if (listaCargas == null || listaCargas.Count == 0)
        {
            Console.WriteLine("Nenhuma carga disponível para processamento.");
            return;
        }

        int maxTarefasParalelas = 5; // Ajuste conforme necessário
        var semaphore = new SemaphoreSlim(maxTarefasParalelas);

        var retryPolicy = Policy
        .Handle<HttpRequestException>()
        .Or<TaskCanceledException>()
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        var tarefas = listaCargas.Select(async carga =>
        {
            await semaphore.WaitAsync();
            try
            {
                await retryPolicy.ExecuteAsync(() => ProcessarCargaAsync(carga));
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tarefas);

        // Callback após todas as tarefas
        Console.WriteLine("Todas as cargas foram processadas.");
    }

    private async Task<List<Carga>> ObterListaCargasAsync()
    {
        Console.WriteLine("Obtendo lista de cargas...");
        var response = await _httpClient.GetAsync("/api/v1/carga/obter-lista-a-processar");

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Erro ao obter lista de cargas: {response.StatusCode}");
            return null;
        }

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<Carga>>(content);
    }

    private async Task ProcessarCargaAsync(Carga carga)
    {
        try
        {
            string endpoint = carga.Situacao switch
            {
                "P" => "/api/v1/processar/carregar-temp",
                "T" => "/api/v1/processar/carregar-controle",
                "C" => "/api/v1/processar/carregar-top",
                _ => throw new InvalidOperationException($"Status inválido: {carga.Situacao}")
            };

            await ChamarEndpointCarga(endpoint, carga);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao processar carga {carga.SeqCarga}: {ex.Message}");
        }
    }

    private async Task ChamarEndpointCarga(string endpoint, Carga carga)
    {
        Console.WriteLine($"Processando carga {carga.SeqCarga} no endpoint {endpoint}...");

        var payload = JsonSerializer.Serialize(new { SeqCarga = carga.SeqCarga });
        var content = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(endpoint, content);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Erro ao processar carga {carga.SeqCarga}: {response.StatusCode}");
            return;
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        var resultado = JsonSerializer.Deserialize<ResultadoProcessamento>(responseContent);

        if (resultado.CodRetorno == 0)
        {
            Console.WriteLine($"Carga {carga.SeqCarga} processada com sucesso no endpoint {endpoint}.");
        }
        else
        {
            Console.WriteLine($"Erro ao processar carga {carga.SeqCarga}: {resultado.MsgRetorno}");
            if (resultado.Result != null)
            {
                foreach (var erro in resultado.Result)
                {
                    Console.WriteLine($" - Código Erro: {erro.CodErro}, Mensagem: {erro.MsgErro}");
                }
            }
        }
    }
}
