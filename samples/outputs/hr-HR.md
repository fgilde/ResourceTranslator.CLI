[! [Izradite i implementirajte aplikaciju ASP.Net Core u web-aplikaciju Azure Web App - Coworkee] (https://github.com/fgilde/CleanArchitectureBaseBlazor/actions/workflows/master_coworkee.yml/badge.svg)] (https://github.com/fgilde/CleanArchitectureBaseBlazor/actions/workflows/master_coworkee.yml)

Aplikacija s jednom stranicom (Blazor) i ASP.NET Core Server slijedeći principe čiste arhitekture. 
<br/>

Ovo je rješenje nova sljedeća verzija programa [CleanArchitectureBase](https://github.com/fgilde/CleanArchitectureBase) 
Sada s Blazor Web Assembly Frontendom.

[Pokrenuta demonstracija dostupna je ovdje] (https://coworkee.azurewebsites.net/)

## Tehnologije

* ASP.NET Jezgra 6
* [Jezgra okvira entiteta 6](https://docs.microsoft.com/en-us/ef/core/)
* [Signal R](https://docs.microsoft.com/en-US/aspnet/signaler/overview/getting-started/introduction-to-signaler)
* [Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor)
* [Mud Blazor](https://mudblazor.com/getting-start/installation#manual-install)
* [MediatR](https://github.com/jbogard/MediatR)
* [FluentValidation](https://fluentvalidation.net/)
* [NUnit](https://nunit.org/), [FluentAssertions](https://fluentassertions.com/), [Moq](https://github.com/moq) & [Respawn](https://github.com/jbogard/Respawn)
* [Docker](https://www.docker.com/)

## Početak rada

1. Instalirajte najnoviji [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
2. Idite na 'src/Server' i pokrenite 'dotnet run' za pokretanje stražnjeg kraja i klijenta za web-sastavljanje (ASP.NET Core Web API) ili otvorite rješenje u Visual Studiju i pokrenite Server
	(Obavijest o potpunom odvajanju klijenta od poslužitelja samo uklonite referencu na klijentski projekt u Server.csproj)

### Docker konfiguracija (nedovršena)

Da biste Dockera osposobili, morat ćete dodati privremeni SSL certifikat i montirati glasnoću kako biste držali taj certifikat.
Možete pronaći [Microsoft Docs](https://docs.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-3.1) koji opisuju korake potrebne za Windows, macOS i Linux.

Za Windows:
Sljedeće će se morati izvršiti s vašeg terminala kako bi se stvorio certifikat
'dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p Your_password123'
'dotnet dev-certs https --trust'

NAPOMENA: Prilikom korištenja ljuske PowerShell zamijenite %USERPROFILE% s $env:USERPROFILE.

ZA macOS:
'dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p Your_password123'
'dotnet dev-certs https --trust'

FOR Linux:
'dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p Your_password123'

Da biste izgradili i pokrenuli docker kontejnere, izvršite 'docker-compose -f 'docker-compose.yml' up --build' iz korijena rješenja u kojem nalazite datoteku docker-compose.yml.  Također možete koristiti "Docker Compose" iz Visual Studija u svrhu ispravljanja pogrešaka.
Zatim otvorite http://localhost:5000 u pregledniku.

### Konfiguracija baze podataka

Predložak je prema zadanim postavkama konfiguriran za korištenje baze podataka u memoriji. Time se osigurava da će svi korisnici moći pokrenuti rješenje bez potrebe za postavljanjem dodatne infrastrukture (npr.SQL Server).

Ako želite koristiti SQL Server, morat ćete ažurirati **WebAPI/appsettings.json** na sljedeći način:

'''json
  "UseInMemoryDatabase": netočno,
```

Provjerite pokazuje li niz veze **DefaultConnection** unutar **appsettings.json** na valjanu instancu sustava SQL Server. 

Kada pokrenete aplikaciju, baza podataka će se automatski stvoriti (ako je potrebno) i primijenit će se najnovije migracije.

### Migracije baze podataka

Da biste za migracije koristili 'dotnet-ef', dodajte sljedeće zastavice naredbi (vrijednosti pretpostavljaju da izvršavate iz korijena spremišta)

* '--projekt src/Infrastruktura' (neobavezno ako je u ovoj mapi)
* '--startup-project src/WebAPI'
* '--output-dir Migrations'

Na primjer, da biste dodali novu migraciju iz korijenske mape:

'dotnet ef migrations add "SampleMigration" --project src\Infrastructure --startup-project src\Server --output-dir Migrations'

## Pregled

### Domena

To će sadržavati sve entitete, brojeve, iznimke, sučelja, vrste i logiku specifičnu za sloj domene.

### Aplikacija

Ovaj sloj sadrži svu logiku primjene. Ovisi o sloju domene, ali nema ovisnosti o bilo kojem drugom sloju ili projektu. Ovaj sloj definira sučelja koja implementiraju vanjski slojevi. Na primjer, ako aplikacija treba pristupiti usluzi obavješćivanja, aplikaciji bi se dodalo novo sučelje i stvorila bi se implementacija unutar infrastrukture.

### Infrastruktura

Ovaj sloj sadrži klase za pristup vanjskim resursima kao što su datotečni sustavi, web-usluge, smtp i tako dalje. Te bi se klase trebale temeljiti na sučeljima definiranim unutar aplikacijskog sloja.

### Web

Ovaj sloj je aplikacija na jednoj stranici koja se temelji na Blazoru i Serveru i API-ju kao ASP.NET Core 5. Ovaj sloj ovisi i o aplikacijskom i o infrastrukturnom sloju, međutim, ovisnost o infrastrukturi je only podržati injekciju ovisnosti. Stoga se samo *Startup.cs* treba odnositi na Infrastrukturu.
