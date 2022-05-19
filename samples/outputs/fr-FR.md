[! [Générer et déployer ASP.Net application Principale sur Azure Web App - Coworkee] (https://github.com/fgilde/CleanArchitectureBaseBlazor/actions/workflows/master_coworkee.yml/badge.svg)] (https://github.com/fgilde/CleanArchitectureBaseBlazor/actions/workflows/master_coworkee.yml)

Application monopage (Blazor) et un serveur de base ASP.NET suivant les principes de l’architecture propre. 
<br/>

Cette solution est la nouvelle version suivante de [CleanArchitectureBase](https://github.com/fgilde/CleanArchitectureBase) 
Maintenant avec un Frontend d’assemblage Web Blazor.

[Une démo en cours d’exécution est disponible ici] (https://coworkee.azurewebsites.net/)

## Technologies

* ASP.NET Core 6
* [Entity Framework Core 6](https://docs.microsoft.com/en-us/ef/core/)
* [Signal R](https://docs.microsoft.com/en-US/aspnet/signalr/overview/getting-started/introduction-to-signalr)
* [Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor)
* [Mud Blazor](https://mudblazor.com/getting-démarré/installation#manual-install)
* [MediatR](https://github.com/jbogard/MediatR)
* [FluentValidation](https://fluentvalidation.net/)
* [NUnit](https://nunit.org/), [FluentAssertions](https://fluentassertions.com/), [Moq](https://github.com/moq) & [Respawn](https://github.com/jbogard/Respawn)
* [Docker](https://www.docker.com/)

## Mise en route

1. Installez la dernière version du [SDK.NET 6](https://dotnet.microsoft.com/download/dotnet/6.0)
2. Accédez à 'src/Server' et exécutez 'dotnet run' pour lancer le back-end et le client webassembly (ASP.NET Core Web API) ou ouvrez Solution dans Visual Studio et lancez Server
	(Avis pour séparer complètement le client du serveur, il suffit de supprimer la référence au projet client dans Server.csproj)

### Configuration de Docker (inachevée)

Pour que Docker fonctionne, vous devrez ajouter un certificat SSL temporaire et monter un volume pour conserver ce certificat.
Vous pouvez trouver [Microsoft Docs](https://docs.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-3.1) qui décrit les étapes requises pour Windows, macOS et Linux.

Pour Windows :
Les éléments suivants devront être exécutés à partir de votre terminal pour créer un certificat
'dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p Your_password123'
'dotnet dev-certs https --trust'

Remarque : Lorsque vous utilisez PowerShell, remplacez %USERPROFILE% par $env:USERPROFILE.

POUR macOS :
'dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p Your_password123'
'dotnet dev-certs https --trust'

POUR Linux :
'dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p Your_password123'

Pour générer et exécuter les conteneurs docker, exécutez 'docker-compose -f 'docker-compose.yml' up --build' à partir de la racine de la solution où vous trouvez le fichier docker-compose.yml.  Vous pouvez également utiliser « Docker Compose » à partir de Visual Studio à des fins de débogage.
Ouvrez ensuite http://localhost:5000 sur votre navigateur.

### Configuration de la base de données

Le modèle est configuré pour utiliser une base de données en mémoire par défaut. Cela garantit que tous les utilisateurs seront en mesure d’exécuter la solution sans avoir besoin de configurer une infrastructure supplémentaire (par exemple.SQL Server).

Si vous souhaitez utiliser SQL Server, vous devez mettre à jour **WebAPI/appsettings.json** comme suit :

'''json
  « UseInMemoryDatabase »: false,
```

Vérifiez que la chaîne de connexion **DefaultConnection** dans **appsettings.json** pointe vers une instance SQL Server valide. 

Lorsque vous exécutez l’application, la base de données est automatiquement créée (si nécessaire) et les dernières migrations sont appliquées.

### Migrations de bases de données

Pour utiliser 'dotnet-ef' pour vos migrations, veuillez ajouter les indicateurs suivants à votre commande (les valeurs supposent que vous exécutez à partir de la racine du référentiel)

* '--project src/Infrastructure' (facultatif si dans ce dossier)
* '--startup-project src/WebAPI'
* '--output-dir Migrations'

Par exemple, pour ajouter une nouvelle migration à partir du dossier racine :

'dotnet ef migrations add « SampleMigration » --project src\Infrastructure --startup-project src\Server --output-dir Migrations'

## Vue d’ensemble

### Domaine

Celui-ci contiendra toutes les entités, énumérations, exceptions, interfaces, types et logiques spécifiques à la couche de domaine.

### Demande

Cette couche contient toute la logique d’application. Il dépend de la couche de domaine, mais n’a aucune dépendance sur une autre couche ou projet. Cette couche définit les interfaces implémentées par les couches externes. Par exemple, si l’application doit accéder à un service de notification, une nouvelle interface sera ajoutée à l’application et une implémentation sera créée au sein de l’infrastructure.

### Infrastructure

Cette couche contient des classes permettant d’accéder à des ressources externes telles que des systèmes de fichiers, des services Web, smtp, etc. Ces classes doivent être basées sur des interfaces définies dans la couche application.

### Web

Cette couche est l’application à page unique basée sur Blazor et le serveur et l’API comme ASP.NET Core 5. Cette couche dépend à la fois des couches Application et Infrastructure, cependant, la dépendance à l’infrastructure est only pour soutenir l’injection de dépendance. Par conséquent, seul *Startup.cs* doit faire référence à Infrastructure.
