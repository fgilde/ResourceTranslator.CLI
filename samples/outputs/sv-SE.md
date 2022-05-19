[! [Skapa och distribuera ASP.Net Core-app till Azure Web App – Coworkee] (https://github.com/fgilde/CleanArchitectureBaseBlazor/actions/workflows/master_coworkee.yml/badge.svg)] (https://github.com/fgilde/CleanArchitectureBaseBlazor/actions/workflows/master_coworkee.yml)

Single Page App (Blazor) och en ASP.NET Core Server enligt principerna för Clean Architecture. 
<br/>

Den här lösningen är ny Nästa version av [CleanArchitectureBase](https://github.com/fgilde/CleanArchitectureBase) 
Nu med en Blazor Web Assembly Frontend.

[En löpande demo finns här] (https://coworkee.azurewebsites.net/)

## Teknik

* ASP.NET Kärna 6
* [Entity Framework Core 6](https://docs.microsoft.com/en-us/ef/core/)
* [Signal R](https://docs.microsoft.com/en-US/aspnet/signalr/overview/getting-started/introduction-to-signalr)
* [Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor)
* [Mud Blazor](https://mudblazor.com/getting-started/installation#manual-install)
* [MediatR](https://github.com/jbogard/MediatR)
* [FluentValidation](https://fluentvalidation.net/)
* [NUnit](https://nunit.org/), [FluentAssertions](https://fluentassertions.com/), [Moq](https://github.com/moq) & [Respawn](https://github.com/jbogard/Respawn)
* [Docker](https://www.docker.com/)

## Komma igång

1. Installera den senaste [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
2. Navigera till "src/Server" och kör "dotnet run" för att starta backend och webassembly-klienten (ASP.NET Core Web API) eller öppna Solution i Visual Studio och starta Server
	(Observera att du separerar klienten helt från servern, ta bara bort referensen till klientprojektet i Server.csproj)

### Docker-konfiguration (oavslutad)

För att Docker ska fungera måste du lägga till ett tillfälligt SSL-certifikat och montera en volym för att hålla certifikatet.
Du hittar [Microsoft Docs](https://docs.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-3.1) som beskriver de steg som krävs för Windows, macOS och Linux.

För Windows:
Följande måste köras från terminalen för att skapa ett certifikat
"dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p Your_password123"
"dotnet dev-certs https --trust"

När du använder PowerShell ersätter du %USERPROFILE% med $env:USERPROFILE.

FÖR macOS:
"dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p Your_password123"
"dotnet dev-certs https --trust"

FÖR Linux:
"dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p Your_password123"

Om du vill skapa och köra docker-containrarna kör du "docker-compose -f 'docker-compose.yml" up --build' från roten i lösningen där du hittar filen docker-compose.yml.  Du kan också använda "Docker Compose" från Visual Studio i felsökningssyfte.
Öppna sedan http://localhost:5000 i din webbläsare.

### Databaskonfiguration

Mallen är konfigurerad för att använda en minnesbaserad databas som standard. Detta säkerställer att alla användare kan köra lösningen utan att behöva konfigurera ytterligare infrastruktur (t.ex.SQL Server).

Om du vill använda SQL Server måste du uppdatera **WebAPI/appsettings.json** på följande sätt:

'''json
  "UseInMemoryDatabase": falskt,
```

Kontrollera att anslutningssträngen **DefaultConnection** i **appsettings.json** pekar på en giltig SQL Server instans. 

När du kör programmet skapas databasen automatiskt (om det behövs) och de senaste migreringarna tillämpas.

### Databasmigreringar

Om du vill använda "dotnet-ef" för dina migreringar lägger du till följande flaggor i kommandot (värden förutsätter att du kör från lagringsplatsens rot)

* '--project src/Infrastructure' (valfritt om det finns i den här mappen)
* '--startup-project src/WebAPI'
* '--output-dir Migrations'

Om du till exempel vill lägga till en ny migrering från rotmappen:

'dotnet ef migrations add 'SampleMigration' --project src\Infrastructure --startup-project src\Server --output-dir Migrations`

## Översikt

### Domän

Detta kommer att innehålla alla entiteter, uppräkningar, undantag, gränssnitt, typer och logik som är specifika för domänlagret.

### Ansökan

Det här lagret innehåller all programlogik. Den är beroende av domänlagret, men har inga beroenden av något annat lager eller projekt. Det här lagret definierar gränssnitt som implementeras av externa lager. Om programmet till exempel behöver åtkomst till en meddelandetjänst läggs ett nytt gränssnitt till i programmet och en implementering skapas i infrastrukturen.

### Infrastruktur

Det här lagret innehåller klasser för åtkomst till externa resurser som filsystem, webbtjänster, smtp och så vidare. Dessa klasser bör baseras på gränssnitt som definierats i programlagret.

### Webben

Det här lagret är ensidesprogrammet baserat på Blazor och server- och API:et som ASP.NET Core 5. Det här lagret beror på både program- och infrastrukturlagren, men beroendet av infrastruktur är bara för att stödja beroendeinjektion. Därför bör endast *Startup.cs* referera till Infrastruktur.
