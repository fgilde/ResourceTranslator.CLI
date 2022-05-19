[! [Erstellen und Bereitstellen ASP.Net Core-App in der Azure-Web-App – Coworkee] (https://github.com/fgilde/CleanArchitectureBaseBlazor/actions/workflows/master_coworkee.yml/badge.svg)] (https://github.com/fgilde/CleanArchitectureBaseBlazor/actions/workflows/master_coworkee.yml)

Single Page App (Blazor) und ein ASP.NET Core Server nach den Prinzipien der Clean Architecture. 
<br/>

Diese Lösung ist neu Nächste Version von [CleanArchitectureBase](https://github.com/fgilde/CleanArchitectureBase) 
Jetzt mit einem Blazor Web Assembly Frontend.

[Eine laufende Demo ist hier verfügbar] (https://coworkee.azurewebsites.net/)

## Technologien

* ASP.NET Kern 6
* [Entity Framework Core 6](https://docs.microsoft.com/en-us/ef/core/)
* [Signal R](https://docs.microsoft.com/en-US/aspnet/signalr/overview/getting-started/introduction-to-signalr)
* [Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor)
* [Mud Blazor](https://mudblazor.com/getting-started/installation#manual-install)
* [MediatR](https://github.com/jbogard/MediatR)
* [FluentValidation](https://fluentvalidation.net/)
* [NUnit](https://nunit.org/), [FluentAssertions](https://fluentassertions.com/), [Moq](https://github.com/moq) & [Respawn](https://github.com/jbogard/Respawn)
* [Docker](https://www.docker.com/)

## Erste Schritte

1. Installieren Sie das neueste [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
2. Navigieren Sie zu 'src/Server' und führen Sie 'dotnet run' aus, um das Back-End und den Webassembly-Client (ASP.NET Core Web API) zu starten, oder öffnen Sie Solution in Visual Studio und starten Sie Server
	(Hinweis, den Client vollständig vom Server zu trennen, entfernen Sie einfach den Verweis auf das Client-Projekt in Server.csproj)

### Docker-Konfiguration (unvollendet)

Damit Docker funktioniert, müssen Sie ein temporäres SSL-Zertifikat hinzufügen und ein Volume mounten, um dieses Zertifikat zu speichern.
Sie finden [Microsoft Docs](https://docs.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-3.1), in dem die für Windows, macOS und Linux erforderlichen Schritte beschrieben werden.

Für Windows:
Folgendes muss von Ihrem Terminal aus ausgeführt werden, um ein Zertifikat zu erstellen
'dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p Your_password123'
'dotnet dev-certs https --trust'

Hinweis: Wenn Sie PowerShell verwenden, ersetzen Sie %USERPROFILE% durch $env:USERPROFILE.

FÜR macOS:
'dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p Your_password123'
'dotnet dev-certs https --trust'

FÜR Linux:
'dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p Your_password123'

Um die Docker-Container zu erstellen und auszuführen, führen Sie 'docker-compose -f 'docker-compose.yml' up --build' aus dem Stammverzeichnis der Lösung aus, wo Sie die Datei docker-compose.yml finden.  Sie können "Docker Compose" auch aus Visual Studio für Debugzwecke verwenden.
Öffnen Sie dann http://localhost:5000 in Ihrem Browser.

### Datenbankkonfiguration

Die Vorlage ist standardmäßig für die Verwendung einer speicherinternen Datenbank konfiguriert. Dadurch wird sichergestellt, dass alle Benutzer die Lösung ausführen können, ohne eine zusätzliche Infrastruktur (z. B.SQL Server) einrichten zu müssen.

Wenn Sie SQL Server verwenden möchten, müssen Sie die Datei WebAPI/appsettings.json wie folgt aktualisieren:

'''json
  "UseInMemoryDatabase": false,
```

Stellen Sie sicher, dass die Verbindungszeichenfolge **DefaultConnection** in **appsettings.json** auf eine gültige SQL Server-Instanz verweist. 

Wenn Sie die Anwendung ausführen, wird die Datenbank automatisch erstellt (falls erforderlich) und die neuesten Migrationen werden angewendet.

### Datenbank-Migrationen

Um 'dotnet-ef' für Ihre Migrationen zu verwenden, fügen Sie bitte die folgenden Flags zu Ihrem Befehl hinzu (Werte gehen davon aus, dass Sie vom Repository-Stammverzeichnis aus ausgeführt werden)

* '--project src/Infrastructure' (optional, wenn in diesem Ordner)
* '--startup-project src/WebAPI'
* '--output-dir Migrationen'

So fügen Sie beispielsweise eine neue Migration aus dem Stammordner hinzu:

'dotnet ef migrations add 'SampleMigration' --project src\Infrastructure --startup-project src\Server --output-dir Migrations'

## Übersicht

### Domäne

Dies enthält alle Entitäten, Enumerationen, Ausnahmen, Schnittstellen, Typen und Logik, die für die Domänenschicht spezifisch sind.

### Bewerbung

Diese Schicht enthält die gesamte Anwendungslogik. Sie ist von der Domänenschicht abhängig, weist jedoch keine Abhängigkeiten von einer anderen Ebene oder einem anderen Projekt auf. Diese Schicht definiert Schnittstellen, die von externen Schichten implementiert werden. Wenn die Anwendung beispielsweise auf einen Benachrichtigungsdienst zugreifen muss, wird der Anwendung eine neue Schnittstelle hinzugefügt, und eine Implementierung wird innerhalb der Infrastruktur erstellt.

### Infrastruktur

Diese Schicht enthält Klassen für den Zugriff auf externe Ressourcen wie Dateisysteme, Webdienste, SMTP usw. Diese Klassen sollten auf Schnittstellen basieren, die innerhalb der Anwendungsschicht definiert sind.

### Weblinks

Diese Schicht ist die Single-Page-Anwendung, die auf Blazor und dem Server und der API als ASP.NET Core 5 basiert. Diese Schicht hängt sowohl von der Anwendungs- als auch von der Infrastrukturschicht ab, die Abhängigkeit von der Infrastruktur ist jedoch only, um die Abhängigkeitsinjektion zu unterstützen. Daher sollte nur *Startup.cs* auf Infrastruktur verweisen.
