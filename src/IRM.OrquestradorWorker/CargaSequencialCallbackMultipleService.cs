using IRM.Orquestrador.Shared.Models;
using System.Text;
using System.Text.Json;

namespace IRM.OrquestradorWorker;

public class CargaSequencialCallbackMultipleService
{
    private readonly HttpClient _httpClient;

    public CargaSequencialCallbackMultipleService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task ProcessarCargasAsync()
    {
        // Obter a lista de cargas
        var listaCargas = await ObterListaCargasAsync();
        if (listaCargas == null || listaCargas.Count == 0)
        {
            Console.WriteLine("Nenhuma carga disponível para processamento.");
            return;
        }

        // Lista para armazenar tarefas de processamento
        var tarefas = new List<Task>();

        foreach (var carga in listaCargas)
        {
            tarefas.Add(Task.Run(async () =>
            {
                try
                {
                    switch (carga.Situacao)
                    {
                        case "P":
                            await ProcessarCargaAsync("/api/v1/processar/carregar-temp", carga);
                            break;
                        case "T":
                            await ProcessarCargaAsync("/api/v1/processar/carregar-controle", carga);
                            break;
                        case "C":
                            await ProcessarCargaAsync("/api/v1/processar/carregar-top", carga);
                            break;
                        default:
                            Console.WriteLine($"Carga {carga.SeqCarga} com status inválido: {carga.Situacao}");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao processar carga {carga.SeqCarga}: {ex.Message}");
                }
            }));
        }

        // Aguarda a conclusão de todas as tarefas
        await Task.WhenAll(tarefas);

        // Callback após todas as tarefas
        Console.WriteLine("Todas as cargas foram processadas. Obtendo nova lista atualizada...");
        //await ObterListaCargasAsync();
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

    private async Task ProcessarCargaAsync(string endpoint, Carga carga)
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
