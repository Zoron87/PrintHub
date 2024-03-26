using Microsoft.AspNetCore.SignalR;
using SignalRSample.Abstractions;
using System.Diagnostics;

namespace SignalRSample.Hubs
{
    public class UserHub : Hub
    {
        public static int ConnectionCount { get; set; } = 0;
        private readonly ICupsPrintService _cupsPrintService;

        public UserHub(ICupsPrintService cupsPrintService)
        {
            _cupsPrintService = cupsPrintService;
        }

        public override async Task OnConnectedAsync()
        {
            ConnectionCount++;
            await Clients.All.SendAsync("UpdateConnectionCount", ConnectionCount);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            ConnectionCount--;
            await Clients.All.SendAsync("UpdateConnectionCount", ConnectionCount);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task GetCupsPrintersNames()
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "lpstat",
                Arguments = "-e",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using (var process = Process.Start(processStartInfo))
            {
                if (process == null)
                    throw new InvalidOperationException("Failed to start lpstat process.");

                var output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode != 0)
                    throw new InvalidOperationException("lpstat process exited with error.");

                var printers =  output.Split(new char[]{' ', '\n'});
                await Clients.All.SendAsync("UpdatePrintersList", printers);
            }
        }

        public async Task SendPrintJob(string printerName, string documentPath)
        {
            try
            {
                await _cupsPrintService.PrintDocumentAsync(printerName, documentPath);
                await Clients.Caller.SendAsync("PrintJobStatus", "Success print document!");
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("PrintJobStatus", $"Failed print document. Error - {ex}");
            }
        }
    }
}
