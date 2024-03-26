using SignalRSample.Abstractions;
using System.Diagnostics;

namespace SignalRSample.Providers
{
    public class CupsPrintService : ICupsPrintService
    {
        public async Task PrintDocumentAsync(string printerName, string documentPath)
        {
            if (string.IsNullOrEmpty(printerName))
                throw new ArgumentException("Printer name cannot be null or empty.", printerName);
            if (string.IsNullOrEmpty(documentPath) || !File.Exists(documentPath))
                throw new ArgumentException("Document path is not valid.", documentPath);

            string command = $"lp -d {printerName} {documentPath}";
            await ExecuteShellCommandAsync(command);
        }

        private static async Task ExecuteShellCommandAsync(string command)
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = "/bin/bash";
                process.StartInfo.Arguments = $"-c \"{command}\"";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                process.Start();

                string output = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();

                process.WaitForExit();

                if (process.ExitCode != 0)
                    throw new InvalidOperationException($"Command '{command}' failed with error: {error}");
            }
        }
    }
}
