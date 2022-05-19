[! [Изграждане и разполагане ASP.Net ядро приложение Azure уеб приложение - Coworkee] (https://github.com/fgilde/CleanArchitectureBaseBlazor/actions/workflows/master_coworkee.yml/badge.svg)] (https://github.com/fgilde/CleanArchitectureBaseBlazor/actions/workflows/master_coworkee.yml)

Приложение за единична страница (Blazor) и ASP.NET ядрен сървър, следващ принципите на "Чиста архитектура". 
<br/>

Това решение е нова Следваща версия на [CleanArchitectureBase](https://github.com/fgilde/CleanArchitectureBase) 
Сега с Блазор Уеб Събрание Фронтенд.

[Тук се предлага работещо демо] (https://coworkee.azurewebsites.net/)

## Технологии

* ASP.NET Ядро 6
* [Рамково ядро на субекта 6](https://docs.microsoft.com/en-us/ef/core/)
* [Сигнал R](https://docs.microsoft.com/en-US/aspnet/сигнал/преглед/първи стъпки/въведение към сигналатор)
* [Blazor](https://dotnet.microsoft.com/en-нас / приложения / aspnet / уеб-приложения / блезор)
* [Mud Blazor](https://mudblazor.com/getting-стартирана/инсталация#ръчно-инсталиране)
* [MediatR](https://github.com/jbogard/MediatR)
* [FluentValidation](https://fluentvalidation.net/)
* [NUnit](https://nunit.org/), [FluentAssertions](https://fluentassertions.com/), [Moq](https://github.com/moq) & [Respawn](https://github.com/jbogard/Respawn)
* [Докер](https://www.docker.com/)

## Първи стъпки

1. Инсталирайте най-новия [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
2. Навигирайте до "src/Server" и изпълнете "dotnet run", за да стартирате задния край и клиента на webassembly (ASP.NET Core Web API) или отворете Решение в Visual Studio и стартирайте Server
	(Известие за отделяне на клиента напълно от сървъра просто премахнете препратка към клиентски проект в Server.csproj)

### Конфигурация на докера (недовършена)

За да накарате Docker да работи, ще трябва да добавите временен SSL серт и да монтирате обем, за да задържите този серт.
Можете да намерите [Microsoft Docs](https://docs.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-3.1), които описват стъпките, необходими за Windows, macOS и Linux.

За Windows:
Следното ще трябва да бъде изпълнено от вашия терминал, за да създадете серт
'дотнет dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p Your_password123'
'дотнет dev-certs https --trust'

ЗАБЕЛЕЖКА: Когато използвате PowerShell, заменете %USERPROFILE% с $env:ПОТРЕБИТЕЛСКИ ПРОФИЛ.

ЗА macOS:
'дотнет dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p Your_password123'
'дотнет dev-certs https --trust'

ЗА Linux:
'дотнет dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p Your_password123'

За да изградите и стартирате контейнерите на докера, изпълнете "docker-compose -f 'docker-compose.yml' up --build' от корена на решението, където намирате файла docker-compose.yml.  Можете също да използвате "Docker Compose" от Visual Studio за целите на отстраняването на грешки.
След това отворете http://localhost:5000 на браузъра си.

### Конфигурация на база данни

Шаблонът е конфигуриран да използва база данни в паметта по подразбиране. Това гарантира, че всички потребители ще могат да изпълняват решението, без да е необходимо да настройват допълнителна инфраструктура (напр.SQL Сървър).

Ако желаете да използвате SQL Server, ще трябва да актуализирате **WebAPI/appsettings.json** както следва:

''json
  "UseInMemoryДатабаза": невярно,
```

Проверете дали низът за връзка **DefaultConnection** в рамките на **appsettings.json** сочи към валиден екземпляр на SQL Server. 

Когато стартирате приложението базата данни ще бъде автоматично създаден (ако е необходимо) и най-новите миграции ще бъдат приложени.

### Миграции на бази данни

За да използвате 'dotnet-ef' за вашите миграции моля, добавете следните флагове към вашата команда (стойностите предполагат, че изпълнявате от корен на хранилището)

* '--проект src/Infrastructure" (по избор, ако в тази папка)
* '--стартиране-проект src/WebAPI'
* '--изход-dir Миграции'

Например, за да добавите нова миграция от главната папка:

"дотнет еф миграциите добавят "Примерна миграция" --проект src\Infrastructure --startup-project src\Server --изход-dir Миграции'

## Общ преглед

### Домейн

Това ще съдържа всички обекти, енуми, изключения, интерфейси, типове и логика, специфични за домейновия слой.

### Приложение

Този слой съдържа цялата логика на приложение. Той е зависим от домейн слой, но няма зависимости от всеки друг слой или проект. Този слой определя интерфейси, които се изпълняват от външни слоеве. Например, ако приложението трябва да получи достъп до услуга за уведомяване, към приложението би бил добавен нов интерфейс и би било създадено изпълнение в рамките на инфраструктурата.

### Инфраструктура

Този слой съдържа класове за достъп до външни ресурси като файлови системи, уеб услуги, smtp и т. н. Тези класове следва да се основават на интерфейси, определени в рамките на приложния слой.

### Уеб

Този слой е приложението за единична страница въз основа на Blazor и сървъра и API като ASP.NET Ядро 5. Този слой зависи както от Слоя приложение, така и от Инфраструктурни слоеве, обаче зависимостта от Инфраструктура е only за подпомагане на инжектирането на зависимост. Следователно само *Стартиране.cs* трябва да препраща инфраструктура.
