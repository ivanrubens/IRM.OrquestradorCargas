using IRM.Orquestrador.Shared.Dtos;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;

namespace IRM.CargaApi;

public static class AleatorioHelper
{
    public static CargaProcessarDto RetornarCargaAProcessarAleatorio()
    {
        var result = new CargaProcessarDto();

        var segundos = ObterNumeroAleatorio(2, 4);
        Thread.Sleep(segundos * 1000);

        var seqCarga = ObterNumeroAleatorio(10, 30);
        var situacao = RetornarSituacao(ObterNumeroAleatorio(0, 4));

        result.SeqCarga = seqCarga;
        result.Situacao = situacao;

        return result;
    }
    public static ResultDto RetornarProcessoAleatorio()
    {
        var result = new ResultDto();

        var segundos = ObterNumeroAleatorio(3, 11);
        Thread.Sleep(segundos * 1000);

        (result.CodRetorno, result.MsgRetorno) = RetornarCodigoMensagem(ObterNumeroAleatorio(0, 11));

        return result;
    }
    public static int ObterNumeroAleatorio(int min, int max)
    {
        var randomizerTimer = RandomizerFactory.GetRandomizer(new FieldOptionsInteger { Min = min, Max = max });
        return randomizerTimer.Generate().Value;
    }

    private static string RetornarSituacao(int indice)
    {
        string[] mensagem = new string[4];


        mensagem[0] = "P";
        mensagem[1] = "T";
        mensagem[2] = "C";
        mensagem[3] = "F";

        return mensagem[indice];
    }

    private static (int, string) RetornarCodigoMensagem(int numero)
    {
        int indice;
        int[] codigo = new int[2];
        string[] mensagem = new string[2];


        codigo[0] = 0;
        codigo[1] = 9;

        mensagem[0] = "Processamento finalizado com sucesso.";
        mensagem[1] = "Erro no processamento.";

        if (numero % 2 == 0)
        {
            indice = 0;
        }
        else
        {
            indice = 1;
        }

        return (codigo[indice], mensagem[indice]);
    }
}
