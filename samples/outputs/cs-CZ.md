[! [Sestavení a nasazení aplikace ASP.Net Core do webové aplikace Azure – Coworkee] (https://github.com/fgilde/CleanArchitectureBaseBlazor/actions/workflows/master_coworkee.yml/badge.svg)] (https://github.com/fgilde/CleanArchitectureBaseBlazor/actions/workflows/master_coworkee.yml)

Jednostránková aplikace (Blazor) a ASP.NET Core Server podle principů čisté architektury. 
<br/>

Toto řešení je nová další verze [CleanArchitectureBase](https://github.com/fgilde/CleanArchitectureBase) 
Teď s Blazor Web Assembly Frontendem.

[Běžící demo je k dispozici zde] (https://coworkee.azurewebsites.net/)

## Technologie

* ASP.NET Core 6
* [Entity Framework Core 6](https://docs.microsoft.com/en-us/ef/core/)
* [Signal R](https://docs.microsoft.com/en-US/aspnet/signalr/overview/getting-started/introduction-to-signalr)
* [Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor)
* [Mud Blazor] (https://mudblazor.com/getting-started/installation#manual-install)
* [MediatR](https://github.com/jbogard/MediatR)
* [FluentValidation](https://fluentvalidation.net/)
* [NUnit](https://nunit.org/), [FluentAssertions](https://fluentassertions.com/), [Moq](https://github.com/moq) & [Respawn](https://github.com/jbogard/Respawn)
* [Docker](https://www.docker.com/)

## Začínáme

1. Nainstalujte nejnovější sadu [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
2. Přejděte na 'src/Server' a spuštěním 'dotnet run' spusťte back-end a klienta webassembly (ASP.NET Core Web API) nebo otevřete řešení v sadě Visual Studio a spusťte Server.
	(Upozornění na úplné oddělení klienta od serveru stačí odstranit odkaz na klientský projekt v Server.csproj)

### Konfigurace Dockeru (nedokončená)

Aby Docker fungoval, budete muset přidat dočasný certifikát SSL a připojit svazek, který bude tento certifikát obsahovat.
Najdete [Microsoft Docs](https://docs.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-3.1), které popisují kroky požadované pro Windows, macOS a Linux.

Pro Windows:
Chcete-li vytvořit certifikát, bude nutné z terminálu provést následující
'dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p Your_password123'
'dotnet dev-certs https --trust'

POZNÁMKA: Při použití PowerShellu nahraďte %USERPROFILE% $env:USERPROFILE.

PRO macOS:
'dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p Your_password123'
'dotnet dev-certs https --trust'

PRO Linux:
'dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p Your_password123'

Aby bylo možné sestavit a spustit kontejnery dockeru, spusťte 'docker-compose -f 'docker-compose.yml' up --build' z kořenového adresáře řešení, kde najdete soubor docker-compose.yml.  Můžete také použít "Docker Compose" ze sady Visual Studio pro účely ladění.
Poté otevřete http://localhost:5000 v prohlížeči.

### Konfigurace databáze

Šablona je ve výchozím nastavení nakonfigurována tak, aby používala databázi v paměti. Tím je zajištěno, že všichni uživatelé budou moci spustit řešení bez nutnosti nastavovat další infrastrukturu (např.SQL Server).

Pokud chcete použít SQL Server, budete muset aktualizovat **WebAPI/appsettings.json** následujícím způsobem:

'''json
  "UseInMemoryDatabase": false,
```

Ověřte, že připojovací řetězec **DefaultConnection** v rámci **appsettings.json** odkazuje na platnou instanci SQL Serveru. 

Při spuštění aplikace se databáze automaticky vytvoří (v případě potřeby) a použijí se nejnovější migrace.

### Migrace databáze

Pokud chcete pro migrace použít dotnet-ef, přidejte do příkazu následující příznaky (hodnoty předpokládají, že je spouštíte z kořenového adresáře úložiště).

* '--project src/Infrastructure' (volitelné, pokud je v této složce)
* '--startup-project src/WebAPI'
* '--output-dir Migrace'

Chcete-li například přidat novou migraci z kořenové složky:

'Dotnet ef migrace add "SampleMigration" --project src\Infrastructure --startup-project src\Server --output-dir Migrations'

## Přehled

### Doména

Bude obsahovat všechny entity, výčty, výjimky, rozhraní, typy a logiku specifickou pro vrstvu domény.

### Aplikace

Tato vrstva obsahuje veškerou aplikační logiku. Je závislá na vrstvě domény, ale nemá žádné závislosti na žádné jiné vrstvě nebo projektu. Tato vrstva definuje rozhraní, která jsou implementována vnějšími vrstvami. Pokud například aplikace potřebuje přístup ke službě oznámení, bude do aplikace přidáno nové rozhraní a v rámci infrastruktury bude vytvořena implementace.

### Infrastruktura

Tato vrstva obsahuje třídy pro přístup k externím prostředkům, jako jsou systémy souborů, webové služby, smtp a tak dále. Tyto třídy by měly být založeny na rozhraních definovaných v rámci aplikační vrstvy.

### Web

Tato vrstva je jednostránková aplikace založená na Blazor a serveru a rozhraní API jako ASP.NET Core 5. Tato vrstva závisí na vrstvě Aplikace i Infrastruktura, nicméně závislost na infrastruktuře je only pro podporu vkládání závislostí. Proto by pouze *Startup.cs* měl odkazovat na infrastrukturu.
