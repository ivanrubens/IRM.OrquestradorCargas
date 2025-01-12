﻿using System.Text.Json.Serialization;

namespace OrquestradorConsole.Models;

// Classe para representar a carga
public class Carga
{
    [JsonPropertyName("seqCarga")]
    public int SeqCarga { get; set; }

    [JsonPropertyName("situacao")]
    public string Situacao { get; set; }
}
