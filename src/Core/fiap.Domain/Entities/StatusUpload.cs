namespace fiap.Domain.Entities
{
    public enum StatusUpload
    {
        Enviado = 1,
        Processando = 2,
        Finalizado = 3,
        Cancelado = 4,
        ErroProcessamento = 5,
        SalvandoS3 = 6,
        ArquivoSalvoS3 = 7
    }
}
