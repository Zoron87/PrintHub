namespace SignalRSample.Abstractions
{
    public interface ICupsPrintService
    {
        Task PrintDocumentAsync(string printerName, string documentPath);
    }
}
