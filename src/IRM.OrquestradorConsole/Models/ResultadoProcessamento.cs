using System.Text.Json.Serialization;

namespace OrquestradorConsole.Models;

// Classe para representar o resultado do processamento
public class ResultadoProcessamento
{
    [JsonPropertyName("codRetorno")]
    public int CodRetorno { get; set; }

    [JsonPropertyName("msgRetorno")]
    public string MsgRetorno { get; set; }

    [JsonPropertyName("result")]
    [JsonIgnore]
    public List<Erro> Result { get; set; }
}
