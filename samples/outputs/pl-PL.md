[! [Tworzenie i wdrażanie aplikacji ASP.Net Core w aplikacji internetowej platformy Azure — Coworkee] (https://github.com/fgilde/CleanArchitectureBaseBlazor/actions/workflows/master_coworkee.yml/badge.svg)] (https://github.com/fgilde/CleanArchitectureBaseBlazor/actions/workflows/master_coworkee.yml)

Aplikacja jednostronicowa (Blazor) i ASP.NET Core Server zgodnie z zasadami Czystej Architektury. 
<br/>

To rozwiązanie jest nową następną wersją [CleanArchitectureBase](https://github.com/fgilde/CleanArchitectureBase) 
Teraz z Blazor Web Assembly Frontend.

[Uruchomione demo jest dostępne tutaj] (https://coworkee.azurewebsites.net/)

## Technologie

* ASP.NET Core 6
* [Entity Framework Core 6] (https://docs.microsoft.com/en-us/ef/core/)
* [Signal R] (https://docs.microsoft.com/en-US/aspnet/signalr/overview/getting-started/introduction-to-signalr)
* [Blazor] (https://dotnet.microsoft.com/en-us / apps / aspnet / web-apps / blazor)
* [Mud Blazor] (https://mudblazor.com/getting-started/installation#manual-install)
* [MediatR](https://github.com/jbogard/MediatR)
* [FluentValidation] (https://fluentvalidation.net/)
* [NUnit](https://nunit.org/), [FluentAssertions](https://fluentassertions.com/), [Moq](https://github.com/moq) & [Respawn](https://github.com/jbogard/Respawn)
* [Docker](https://www.docker.com/)

## Pierwsze kroki

1. Zainstaluj najnowszą wersję [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
2. Przejdź do "src/Server" i uruchom "dotnet run", aby uruchomić back end i klienta webassembly (ASP.NET Core Web API) lub otwórz Rozwiązanie w Visual Studio i uruchom Serwer
	(Uwaga, aby całkowicie oddzielić klienta od serwera, wystarczy usunąć odwołanie do projektu klienta w Server.csproj)

### Konfiguracja dockera (niedokończona)

Aby uruchomić platformę Docker, należy dodać tymczasowy certyfikat SSL i zamontować wolumin do przechowywania tego certyfikatu.
Możesz znaleźć [Microsoft Docs] (https://docs.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-3.1), które opisują kroki wymagane dla systemów Windows, macOS i Linux.

W systemie Windows:
Aby utworzyć certyfikat, należy wykonać następujące czynności z poziomu terminala
'dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p Your_password123'
'dotnet dev-certs https --trust'

UWAGA: W przypadku korzystania z programu PowerShell zastąp %USERPROFILE% ciągiem $env:USERPROFILE.

DLA systemu macOS:
'dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p Your_password123'
'dotnet dev-certs https --trust'

DLA SYSTEMU Linux:
'dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p Your_password123'

Aby skompilować i uruchomić kontenery docker, wykonaj 'docker-compose -f 'docker-compose.yml' up --build' z katalogu głównego rozwiązania, w którym znajduje się plik docker-compose.yml.  Możesz również użyć "Docker Compose" z Visual Studio do celów debugowania.
Następnie otwórz http://localhost:5000 w przeglądarce.

### Konfiguracja bazy danych

Szablon jest domyślnie skonfigurowany do korzystania z bazy danych w pamięci. Gwarantuje to, że wszyscy użytkownicy będą mogli uruchomić rozwiązanie bez konieczności konfigurowania dodatkowej infrastruktury (np.SQL Server).

Jeśli chcesz korzystać z programu SQL Server, musisz zaktualizować **WebAPI/appsettings.json** w następujący sposób:

'''json
  "UseInMemoryDatabase": false,
```

Sprawdź, czy parametry połączenia **DefaultConnection** w pliku **appsettings.json** wskazują prawidłowe wystąpienie programu SQL Server. 

Po uruchomieniu aplikacji baza danych zostanie utworzona automatycznie (w razie potrzeby) i zostaną zastosowane najnowsze migracje.

### Migracje baz danych

Aby użyć 'dotnet-ef' do migracji, dodaj następujące flagi do polecenia (wartości zakładają, że wykonujesz z katalogu głównego repozytorium)

* '--project src/Infrastructure' (opcjonalnie, jeśli znajduje się w tym folderze)
* '--startup-project src/WebAPI'
* '--output-dir Migrations'

Na przykład, aby dodać nową migrację z folderu głównego:

'dotnet ef migrations add 'SampleMigration" --project src\Infrastructure --startup-project src\Server --output-dir Migrations'

## Przegląd

Domena ###

Będzie on zawierał wszystkie encje, enum, wyjątki, interfejsy, typy i logikę specyficzną dla warstwy domeny.

### Aplikacja

Ta warstwa zawiera całą logikę aplikacji. Jest zależny od warstwy domeny, ale nie ma zależności od żadnej innej warstwy ani projektu. Ta warstwa definiuje interfejsy, które są implementowane przez warstwy zewnętrzne. Na przykład, jeśli aplikacja musi uzyskać dostęp do usługi powiadomień, nowy interfejs zostanie dodany do aplikacji, a implementacja zostanie utworzona w infrastrukturze.

### Infrastruktura

Ta warstwa zawiera klasy umożliwiające dostęp do zasobów zewnętrznych, takich jak systemy plików, usługi internetowe, smtp itd. Klasy te powinny być oparte na interfejsach zdefiniowanych w warstwie aplikacji.

### Sieć Web

Ta warstwa jest jednostronicową aplikacją opartą na Blazorze i serwerze i API jako ASP.NET Core 5. Ta warstwa zależy zarówno od warstwy aplikacji, jak i infrastruktury, jednak zależność od infrastruktury wynosi only w celu wsparcia wstrzykiwania zależności. Dlatego tylko *Startup.cs* powinien odwoływać się do infrastruktury.
