[! [Creare e distribuire ASP.Net app di base in App Web di Azure - Coworkee] (https://github.com/fgilde/CleanArchitectureBaseBlazor/actions/workflows/master_coworkee.yml/badge.svg)] (https://github.com/fgilde/CleanArchitectureBaseBlazor/actions/workflows/master_coworkee.yml)

Single Page App (Blazor) e un ASP.NET Core Server seguendo i principi della Clean Architecture. 
<br/>

Questa soluzione è la nuova versione successiva di [CleanArchitectureBase](https://github.com/fgilde/CleanArchitectureBase) 
Ora con un Frontend Blazor Web Assembly.

[Una demo in esecuzione è disponibile qui] (https://coworkee.azurewebsites.net/)

## Tecnologie

* ASP.NET Core 6
* [Entity Framework Core 6](https://docs.microsoft.com/en-us/ef/core/)
* [Signal R](https://docs.microsoft.com/en-US/aspnet/signalr/overview/getting-started/introduction-to-signalr)
* [Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor)
* [Mud Blazor](https://mudblazor.com/getting-avviato/installazione#installazione-manuale)
* [MediatR](https://github.com/jbogard/MediatR)
* [FluentValidation](https://fluentvalidation.net/)
* [NUnit](https://nunit.org/), [FluentAssertions](https://fluentassertions.com/), [Moq](https://github.com/moq) & [Respawn](https://github.com/jbogard/Respawn)
* [Docker](https://www.docker.com/)

## Per iniziare

1. Installare la versione più recente di [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
2. Passare a 'src/Server' ed eseguire 'dotnet run' per avviare il back-end e il client webassembly (ASP.NET Core Web API) o aprire la soluzione in Visual Studio e avviare Server
	(Avviso per separare completamente il client dal server basta rimuovere il riferimento al progetto client in Server.csproj)

### Configurazione Docker (non completata)

Per far funzionare Docker, è necessario aggiungere un certificato SSL temporaneo e montare un volume per contenere tale certificato.
È possibile trovare [Microsoft Docs](https://docs.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-3.1) che descrive i passaggi necessari per Windows, macOS e Linux.

Per Windows:
Quanto segue dovrà essere eseguito dal terminale per creare un certificato
'dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p Your_password123'
'dotnet dev-certs https --trust'

Nota : quando si usa PowerShell, sostituire %USERPROFILE% con $env:USERPROFILE.

PER macOS:
'dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p Your_password123'
'dotnet dev-certs https --trust'

PER Linux:
'dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p Your_password123'

Per compilare ed eseguire i contenitori docker, eseguire 'docker-compose -f 'docker-compose.yml' up --build' dalla radice della soluzione in cui si trova il file docker-compose.yml.  È inoltre possibile utilizzare "Docker Compose" da Visual Studio per scopi di debug.
Quindi apri http://localhost:5000 sul tuo browser.

### Configurazione del database

Il modello è configurato per l'utilizzo di un database in memoria per impostazione predefinita. Ciò garantisce che tutti gli utenti siano in grado di eseguire la soluzione senza dover configurare un'infrastruttura aggiuntiva (ad esempio.SQL Server).

Se si desidera utilizzare SQL Server, sarà necessario aggiornare **WebAPI/appsettings.json** come segue:

'''json
  "UseInMemoryDatabase": false,
```

Verificare che la stringa di connessione **DefaultConnection** all'interno di **appsettings.json** punti a un'istanza di SQL Server valida. 

Quando si esegue l'applicazione, il database verrà creato automaticamente (se necessario) e verranno applicate le migrazioni più recenti.

### Migrazioni di database

Per utilizzare 'dotnet-ef' per le migrazioni, aggiungi i seguenti flag al tuo comando (i valori presuppongono che tu stia eseguendo dalla radice del repository)

* '--project src/Infrastructure' (facoltativo se presente in questa cartella)
* '--startup-project src/WebAPI'
* '--output-dir Migrazioni'

Ad esempio, per aggiungere una nuova migrazione dalla cartella principale:

'dotnet ef migrations add "SampleMigration" --project src\Infrastructure --startup-project src\Server --output-dir Migrations`

## Panoramica

### Dominio

Questo conterrà tutte le entità, enum, eccezioni, interfacce, tipi e logica specifici per il livello di dominio.

### Applicazione

Questo livello contiene tutta la logica dell'applicazione. Dipende dal livello di dominio, ma non ha dipendenze da altri livelli o progetti. Questo livello definisce le interfacce implementate dai livelli esterni. Ad esempio, se l'applicazione deve accedere a un servizio di notifica, verrà aggiunta una nuova interfaccia all'applicazione e verrà creata un'implementazione all'interno dell'infrastruttura.

### Infrastruttura

Questo livello contiene classi per l'accesso a risorse esterne quali file system, servizi Web, smtp e così via. Queste classi devono essere basate su interfacce definite all'interno del livello dell'applicazione.

### Web

Questo livello è l'applicazione a pagina singola basata su Blazor e il server e l'API come ASP.NET Core 5. Questo livello dipende sia dai livelli Applicazione che Infrastruttura, tuttavia, la dipendenza dall'infrastruttura è solo per supportare l'iniezione di dipendenze. Pertanto solo *Startup.cs* dovrebbe fare riferimento all'infrastruttura.
