class PrinterApp 
{
    constructor(hubUrl) {
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl(hubUrl)
            .configureLogging(signalR.LogLevel.Information)
            .build();
        this.selectedPrinter = null;
        this.fileInput = null;
        
        this.attachConnectionListeners();
        this.attachDomEventListeners();
    }

    attachConnectionListeners() {
        this.connection.on("UpdateTotalViews", value => {
            document.getElementById("totalViewsCounter").innerText = value.toString();
        });

        this.connection.on("UpdateConnectionCount", value => {
            document.getElementById("totalConnections").innerText = value.toString();
        });

        this.connection.on("UpdatePrintersList", printers => {
            const printerSelect = document.getElementById("printerSelect");
            printers.forEach(printer => {
                const option = document.createElement("option");
                option.value = printer; 
                option.textContent = printer;
                printerSelect.appendChild(option);
            });
        });

        this.connection.on("PrintJobStatus", value => {
            document.getElementById("PrintJobStatus").innerText = value.toString();
        })
    }

    attachDomEventListeners() {
        document.getElementById("printerSelect").addEventListener("change", event => {
            this.selectedPrinter = event.target.value;
            console.log("Selected printer:", this.selectedPrinter);
        });

        const fileInputElement = document.getElementById("fileInput");
        fileInputElement.addEventListener("change", event => {
            this.fileInput = event.target.files[0];
            console.log("Selected file:", this.fileInput.name);
        });

        const sendPrintJobButton = document.getElementById("sendPrintJobButton");
        sendPrintJobButton.addEventListener("click", event => {
            event.preventDefault();
            if (this.connection && this.selectedPrinter && this.fileInput) {
                this.connection.send("SendPrintJob", this.selectedPrinter, this.fileInput.name)
                    .then(() => console.log(`Print job request sent. Printer - ${this.selectedPrinter}. File - ${this.fileInput.name}`))
                    .catch(err => console.error("Error sending print job:", err.toString()));
            } else {
                console.warn("Cannot send print job. Check connection, selected printer and file.");
            }
        });
    }

    startConnection() {
        this.connection.start()
            .then(() => {
                console.log("Connection successful");
                this.onConnectionSuccess();
            })
            .catch(err => console.error("Connection failed:", err));
    }

    onConnectionSuccess() {
        console.log("Preparing for printing tasks");
        this.connection.send("GetCupsPrintersNames");
        console.log("Initialization complete");
    }
}

const printerApp = new PrinterApp("/hubs/userPrint");
printerApp.startConnection();