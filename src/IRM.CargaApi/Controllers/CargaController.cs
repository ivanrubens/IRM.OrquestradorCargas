using Microsoft.AspNetCore.Mvc;
using IRM.Orquestrador.Shared.Dtos;
using IRM.Orquestrador.Shared.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IRM.CargaApi.Controllers;

[Route("api/v1")]
[ApiController]
public class CargaController : ControllerBase
{
    List<CargaProcessarDto> listaCargas;

    public CargaController()
    {
        var indice = AleatorioHelper.ObterNumeroAleatorio(0, 10);

        listaCargas = new List<CargaProcessarDto>();
        for (int i = 0; i < indice; i++)
        {
            var carga = AleatorioHelper.RetornarCargaAProcessarAleatorio();
            listaCargas.Add(carga);
        }
    }

    [HttpGet("carga/obter-lista-a-processar")]
    public IEnumerable<CargaProcessarDto> ObterListaAProcessar()
    {
        return listaCargas.Where(t => t.Situacao != "F");
    }

    [HttpPost("processar/carregar-temp")]
    public ResultDto ProcessarTemp(ProcessarRequest request)
    {
        var retorno = AleatorioHelper.RetornarProcessoAleatorio();

        return retorno;
    }

    [HttpPost("processar/carregar-controle")]
    public ResultDto ProcessarCarga(ProcessarRequest request)
    {
        var retorno = AleatorioHelper.RetornarProcessoAleatorio();

        return retorno;
    }
    [HttpPost("processar/carregar-top")]
    public ResultDto ProcessarTop(ProcessarRequest request)
    {
        var retorno = AleatorioHelper.RetornarProcessoAleatorio();

        return retorno;
    }
}
