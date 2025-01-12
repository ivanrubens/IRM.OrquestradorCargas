namespace IRM.Orquestrador.Shared.Dtos;

public class ResultDto
{
    public int CodRetorno { get; set; }
    public string MsgRetorno { get; set; }
    public ResultErroDto[] ResultErros { get; set; }
}