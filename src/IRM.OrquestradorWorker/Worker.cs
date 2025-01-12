namespace IRM.OrquestradorWorker;
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly CargaSequencialCallbackMultipleService _cargaService;

    public Worker(ILogger<Worker> logger, CargaSequencialCallbackMultipleService cargaService)
    {
        _logger = logger;
        _cargaService = cargaService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Worker iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Inicia o processamento das cargas
                Console.WriteLine("Processando cargas...");

                await _cargaService.ProcessarCargasAsync();

                // Intervalo entre execu??es
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro no Worker: {ex.Message}");
            }
        }
    }
}
