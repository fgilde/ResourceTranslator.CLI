[! [ASP.Net Core-app bouwen en implementeren in Azure Web App - Coworkee] (https://github.com/fgilde/CleanArchitectureBaseBlazor/actions/workflows/master_coworkee.yml/badge.svg)] (https://github.com/fgilde/CleanArchitectureBaseBlazor/actions/workflows/master_coworkee.yml)

Single Page App (Blazor) en een ASP.NET Core Server volgens de principes van Clean Architecture. 
<br/>

Deze oplossing is de nieuwe volgende versie van [CleanArchitectureBase](https://github.com/fgilde/CleanArchitectureBase) 
Nu met een Blazor Web Assembly Frontend.

[Een lopende demo is hier beschikbaar] (https://coworkee.azurewebsites.net/)

## Technologieën

* ASP.NET Core 6
* [Entity Framework Core 6](https://docs.microsoft.com/en-us/ef/core/)
* [Signaal R] (https://docs.microsoft.com/en-US/aspnet/signalr/overview/getting-started/introduction-to-signalr)
* [Blazor] (https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor)
* [Mud Blazor] (https://mudblazor.com/getting-gestart / installatie # handmatig-installeren)
* [MediatR](https://github.com/jbogard/MediatR)
* [FluentValidation] (https://fluentvalidation.net/)
* [NUnit](https://nunit.org/), [FluentAssertions](https://fluentassertions.com/), [Moq](https://github.com/moq) & [Respawn](https://github.com/jbogard/Respawn)
* [Docker] (https://www.docker.com/)

## Aan de slag

1. Installeer de nieuwste [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
2. Navigeer naar 'src/Server' en voer 'dotnet run' uit om de back-end en de webassembly-client (ASP.NET Core Web API) te starten of open Solution in Visual Studio en start Server
	(Kennisgeving om de client volledig van de server te scheiden, verwijder gewoon de verwijzing naar het clientproject in Server.csproj)

### Docker-configuratie (onvoltooid)

Om Docker te laten werken, moet u een tijdelijk SSL-certificaat toevoegen en een volume koppelen om dat certificaat te houden.
U vindt [Microsoft Docs](https://docs.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-3.1) waarin de stappen worden beschreven die vereist zijn voor Windows, macOS en Linux.

Voor Windows:
Het volgende moet worden uitgevoerd vanaf uw terminal om een certificaat te maken
'dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p Your_password123'
'dotnet dev-certs https --trust'

OPMERKING: Vervang bij gebruik van PowerShell %USERPROFILE% door $env:USERPROFILE.

VOOR macOS:
'dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p Your_password123'
'dotnet dev-certs https --trust'

VOOR Linux:
'dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p Your_password123'

Om de docker-containers te bouwen en uit te voeren, voert u 'docker-compose -f 'docker-compose.yml' up --build' uit vanuit de root van de oplossing waar u het docker-compose.yml-bestand vindt.  U kunt ook 'Docker Compose' van Visual Studio gebruiken voor foutopsporingsdoeleinden.
Open vervolgens http://localhost:5000 in uw browser.

### Database configuratie

De sjabloon is standaard geconfigureerd voor het gebruik van een database in het geheugen. Dit zorgt ervoor dat alle gebruikers de oplossing kunnen uitvoeren zonder dat ze extra infrastructuur hoeven in te stellen (bijv.SQL Server).

Als u SQL Server wilt gebruiken, moet u **WebAPI/appsettings.json** als volgt bijwerken:

'''json
  "UseInMemoryDatabase": false,
```

Controleer of de verbindingsreeks **DefaultConnection** in **appsettings.json** verwijst naar een geldig SQL Server-exemplaar. 

Wanneer u de toepassing uitvoert, wordt de database automatisch gemaakt (indien nodig) en worden de nieuwste migraties toegepast.

### Database migraties

Om 'dotnet-ef' te gebruiken voor uw migraties, voegt u de volgende vlaggen toe aan uw opdracht (waarden gaan ervan uit dat u vanuit repository root uitvoert)

* '--project src/Infrastructure' (optioneel indien in deze map)
* '--startup-project src/WebAPI'
* '--output-dir Migraties'

Ga bijvoorbeeld als volgt te werk om een nieuwe migratie toe te voegen vanuit de hoofdmap:

'dotnet ef migrations add "SampleMigration" --project src\Infrastructure --startup-project src\Server --output-dir Migrations'

## Overzicht

### Domein

Dit bevat alle entiteiten, opsommingen, uitzonderingen, interfaces, typen en logica die specifiek zijn voor de domeinlaag.

### Toepassing

Deze laag bevat alle toepassingslogica. Het is afhankelijk van de domeinlaag, maar heeft geen afhankelijkheden van een andere laag of project. Deze laag definieert interfaces die worden geïmplementeerd door externe lagen. Als de toepassing bijvoorbeeld toegang moet hebben tot een meldingsservice, wordt een nieuwe interface aan de toepassing toegevoegd en wordt een implementatie binnen de infrastructuur gemaakt.

### Infrastructuur

Deze laag bevat klassen voor toegang tot externe bronnen zoals bestandssystemen, webservices, smtp, enzovoort. Deze klassen moeten gebaseerd zijn op interfaces die binnen de toepassingslaag zijn gedefinieerd.

### Web

Deze laag is de single page applicatie gebaseerd op Blazor en de Server en API als ASP.NET Core 5. Deze laag is afhankelijk van zowel de applicatie- als de infrastructuurlaag, maar de afhankelijkheid van infrastructuur is only ter ondersteuning van afhankelijkheidsinjectie. Daarom mag alleen *Startup.cs* verwijzen naar Infrastructuur.
