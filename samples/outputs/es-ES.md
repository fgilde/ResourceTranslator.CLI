[! [Compilación e implementación de la aplicación ASP.Net Core en Azure Web App - Coworkee] (https://github.com/fgilde/CleanArchitectureBaseBlazor/actions/workflows/master_coworkee.yml/badge.svg)] (https://github.com/fgilde/CleanArchitectureBaseBlazor/actions/workflows/master_coworkee.yml)

Aplicación de una sola página (Blazor) y un servidor ASP.NET Core siguiendo los principios de la arquitectura limpia. 
<br/>

Esta solución es la nueva versión next de [CleanArchitectureBase](https://github.com/fgilde/CleanArchitectureBase) 
Ahora con un Frontend de Ensamblaje Web Blazor.

[Una demostración en ejecución está disponible aquí] (https://coworkee.azurewebsites.net/)

## Tecnologías

* ASP.NET Core 6
* [Entity Framework Core 6](https://docs.microsoft.com/en-us/ef/core/)
* [Signal R](https://docs.microsoft.com/en-US/aspnet/signalr/overview/getting-started/introduction-to-signalr)
* [Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor)
* [Mud Blazor](https://mudblazor.com/getting-started/installation#manual-install)
* [MediatR](https://github.com/jbogard/MediatR)
* [FluentValidation](https://fluentvalidation.net/)
* [NUnit](https://nunit.org/), [FluentAssertions](https://fluentassertions.com/), [Moq](https://github.com/moq) & [Respawn](https://github.com/jbogard/Respawn)
* [Docker](https://www.docker.com/)

## Primeros pasos

1. Instale la versión más reciente de [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
2. Navegue a 'src/Server' y ejecute 'dotnet run' para iniciar el back-end y el cliente webassembly (ASP.NET Core Web API) o abrir la solución en Visual Studio e iniciar Server
	(Aviso para separar completamente el cliente del servidor, simplemente elimine la referencia al proyecto cliente en Server.csproj)

### Configuración de Docker (sin terminar)

Para que Docker funcione, deberá agregar un certificado SSL temporal y montar un volumen para contener ese certificado.
Puede encontrar [Microsoft Docs](https://docs.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-3.1) que describen los pasos necesarios para Windows, macOS y Linux.

Para Windows:
Lo siguiente deberá ejecutarse desde su terminal para crear un certificado
'dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p Your_password123'
'dotnet dev-certs https --trust'

Nota : cuando use PowerShell, reemplace %USERPROFILE% por $env:USERPROFILE.

PARA macOS:
'dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p Your_password123'
'dotnet dev-certs https --trust'

PARA Linux:
'dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p Your_password123'

Para compilar y ejecutar los contenedores docker, ejecute 'docker-compose -f 'docker-compose.yml' en --build' desde la raíz de la solución donde encuentre el archivo docker-compose.yml.  También puede usar "Docker Compose" de Visual Studio para fines de depuración.
Luego abra http://localhost:5000 en su navegador.

### Configuración de la base de datos

La plantilla está configurada para utilizar una base de datos en memoria de forma predeterminada. Esto garantiza que todos los usuarios podrán ejecutar la solución sin necesidad de configurar una infraestructura adicional (por ejemplo.SQL Server).

Si desea utilizar SQL Server, deberá actualizar **WebAPI/appsettings.json** de la siguiente manera:

'''json
  "UseInMemoryDatabase": false,
```

Compruebe que la cadena de conexión **DefaultConnection** de **appsettings.json** apunta a una instancia de SQL Server válida. 

Cuando ejecute la aplicación, la base de datos se creará automáticamente (si es necesario) y se aplicarán las migraciones más recientes.

### Migraciones de bases de datos

Para usar 'dotnet-ef' para sus migraciones, agregue las siguientes marcas a su comando (los valores asumen que se está ejecutando desde la raíz del repositorio)

* '--project src/Infrastructure' (opcional si está en esta carpeta)
* '--startup-project src/WebAPI'
* '--output-dir Migraciones'

Por ejemplo, para agregar una nueva migración desde la carpeta raíz:

'dotnet ef migrations add "SampleMigration" --project src\Infrastructure --startup-project src\Server --output-dir Migrations'

## Visión general

### Dominio

Esto contendrá todas las entidades, enumeraciones, excepciones, interfaces, tipos y lógica específicos de la capa de dominio.

### Aplicación

Esta capa contiene toda la lógica de la aplicación. Depende de la capa de dominio, pero no tiene dependencias de ninguna otra capa o proyecto. Esta capa define las interfaces que son implementadas por capas externas. Por ejemplo, si la aplicación necesita acceder a un servicio de notificación, se agregaría una nueva interfaz a la aplicación y se crearía una implementación dentro de la infraestructura.

### Infraestructura

Esta capa contiene clases para acceder a recursos externos como sistemas de archivos, servicios web, smtp, etc. Estas clases deben basarse en interfaces definidas dentro de la capa de aplicación.

### Web

Esta capa es la aplicación de una sola página basada en Blazor y el servidor y la API como ASP.NET Core 5. Esta capa depende de las capas De aplicación e Infraestructura, sin embargo, la dependencia de Infraestructura es only para apoyar la inyección de dependencia. Por lo tanto, solo *Startup.cs* debe hacer referencia a Infraestructura.
