[! [Byg og udrul ASP.Net Core-appen til Azure Web App – Coworkee] (https://github.com/fgilde/CleanArchitectureBaseBlazor/actions/workflows/master_coworkee.yml/badge.svg)] (https://github.com/fgilde/CleanArchitectureBaseBlazor/actions/workflows/master_coworkee.yml)

Single Page App (Blazor) og en ASP.NET Core Server efter principperne for Clean Architecture. 
<br/>

Denne løsning er ny Næste version af [CleanArchitectureBase](https://github.com/fgilde/CleanArchitectureBase) 
Nu med en Blazor Web Assembly Frontend.

[En kørende demo er tilgængelig her] (https://coworkee.azurewebsites.net/)

## Teknologier

* ASP.NET Core 6
* [Entity Framework Core 6](https://docs.microsoft.com/en-us/ef/core/)
* [Signal R](https://docs.microsoft.com/en-US/aspnet/signalr/overview/getting-started/introduction-to-signalr)
* [Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor)
* [Mud Blazor](https://mudblazor.com/getting-started/installation#manual-install)
* [MediatR](https://github.com/jbogard/MediatR)
* [Flydendevalidering](https://fluentvalidation.net/)
* [NUnit](https://nunit.org/), [FluentAssertions](https://fluentassertions.com/), [Moq](https://github.com/moq) & [Respawn](https://github.com/jbogard/Respawn)
* [Docker](https://www.docker.com/)

## Sådan kommer du i gang

1. Installer den nyeste [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
2. Naviger til 'src / Server' og kør 'dotnet run' for at starte backend og webassembly-klienten (ASP.NET Core Web API) eller åbne Solution i Visual Studio og starte Server
	(Meddelelse om at adskille klienten helt fra serveren skal du bare fjerne henvisningen til klientprojektet i Server.csproj)

### Docker-konfiguration (ufærdig)

For at få Docker til at fungere, skal du tilføje et midlertidigt SSL-certifikat og montere et volumen for at holde det certifikat.
Du kan finde [Microsoft Docs](https://docs.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-3.1), der beskriver de trin, der kræves til Windows, macOS og Linux.

For Windows:
Følgende skal udføres fra din terminal for at oprette et certifikat
'dotnet dev-certs https -ep%USERPROFILE%\.aspnet\https\aspnetapp.pfx -p Your_password123'
'dotnet dev-certs https --trust'

BEMÆRK: Når du bruger PowerShell, skal du erstatte %USERPROFILE% med $env:USERPROFILE.

TIL macOS:
'dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p Your_password123'
'dotnet dev-certs https --trust'

TIL Linux:
'dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p Your_password123'

For at opbygge og køre dockercontainerne skal du udføre 'docker-compose -f'docker-compose.yml' op --build' fra roden af løsningen, hvor du finder docker-compose.yml-filen.  Du kan også bruge "Docker Compose" fra Visual Studio til fejlfindingsformål.
Åbn derefter http://localhost:5000 i din browser.

### Database konfiguration

Skabelonen er som standard konfigureret til at bruge en database i hukommelsen. Dette sikrer, at alle brugere kan køre løsningen uden at skulle konfigurere yderligere infrastruktur (f.eks.SQL Server).

Hvis du vil bruge SQL Server, skal du opdatere **WebAPI/appsettings.json** på følgende:

'''json
  "UseInMemoryDatabase": falsk,
```

Kontrollér, at forbindelsesstrengen **DefaultConnection** i **appsettings.json** peger på en gyldig SQL Server-forekomst. 

Når du kører programmet, oprettes databasen automatisk (hvis det er nødvendigt), og de seneste vandringer anvendes.

### Database migreringer

Hvis du vil bruge 'dotnet-ef' til dine vandringer, skal du føje følgende flag til din kommando (værdier forudsætter, at du udfører fra lagerrod)

* '--project src/Infrastructure' (valgfrit, hvis det er i denne mappe)
* '--startup-projekt src/WebAPI'
* '--output-dir Migrationer'

Hvis du f.eks. vil tilføje en ny overførsel fra rodmappen:

'dotnet ef migrations add "SampleMigration" --project src\Infrastructure --startup-project src\Server --output-dir Migrations'

## Oversigt

### Domæne

Dette vil indeholde alle objekter, enums, undtagelser, grænseflader, typer og logik, der er specifikke for domænelaget.

### Ansøgning

Dette lag indeholder al applikationslogik. Det afhænger af domænelaget, men har ingen afhængigheder af noget andet lag eller projekt. Dette lag definerer grænseflader, der implementeres af eksterne lag. Hvis applikationen f.eks. har brug for at få adgang til en meddelelsestjeneste, føjes der en ny grænseflade til applikationen, og der oprettes en implementering inden for infrastrukturen.

### Infrastruktur

Dette lag indeholder klasser til adgang til eksterne ressourcer såsom filsystemer, webtjenester, smtp osv. Disse klasser skal være baseret på grænseflader, der er defineret i applikationslaget.

### Web

Dette lag er enkeltsideapplikationen baseret på Blazor og serveren og API'en som ASP.NET Core 5. Dette lag afhænger af både applikations- og infrastrukturlagene, men afhængigheden af infrastruktur er only for at understøtte afhængighedsinjektion. Derfor bør kun *Startup.cs* henvise til Infrastruktur.
