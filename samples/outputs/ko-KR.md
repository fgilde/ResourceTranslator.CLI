[! [Azure 웹앱 ASP.Net 핵심 앱 빌드 및 배포 - 공동 작업자] (https://github.com/fgilde/CleanArchitectureBaseBlazor/actions/workflows/master_coworkee.yml/badge.svg)] (https://github.com/fgilde/CleanArchitectureBaseBlazor/actions/workflows/master_coworkee.yml)

단일 페이지 응용 프로그램 (Blazor) 및 클린 아키텍처의 원칙을 따르는 ASP.NET 코어 서버. 
<br/>

이 솔루션은 [CleanArchitectureBase]의 새로운 다음 버전입니다(https://github.com/fgilde/CleanArchitectureBase) 
이제 Blazor 웹 어셈블리 프론트엔드가 있습니다.

[실행중인 데모는 여기에서 사용할 수 있습니다] (https://coworkee.azurewebsites.net/)

## 기술

* ASP.NET 코어 6
* [엔티티 프레임 워크 코어 6] (https://docs.microsoft.com/en - 우리 / ef / 코어 / )
* [신호 R](https://docs.microsoft.com/en-US/aspnet/signalr/개요/시작/소개-신호기)
* [Blazor](https://dotnet.microsoft.com/en - 우리 / 애플 리케이션 / 아스넷 / 웹 애플 리케이션 / 블레이저)
* [진흙 블라저](https://mudblazor.com/getting 시작/설치#수동 설치)
* [메디아트R](https://github.com/jbogard/MediatR)
* [유창한 검증](https://fluentvalidation.net/)
* [NUnit](https://nunit.org/), [FluentAssertions](https://fluentassertions.com/), [Moq](https://github.com/moq) 및 [Respawn](https://github.com/jbogard/Respawn)
* [도커](https://www.docker.com/)

## 시작하기

1. 최신 [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) 설치
2. 'src/Server'로 이동하여 'dotnet run'을 실행하여 백 엔드 및 웹어셈블리 클라이언트(ASP.NET Core Web API)를 시작하거나 Visual Studio에서 솔루션을 열고 서버를 시작합니다.
	(서버에서 클라이언트를 완전히 분리하는 것은 Server.csproj에서 클라이언트 프로젝트에 대한 참조를 제거하기만 하면 됩니다.)

### 도커 구성 (미완성)

Docker를 작동시키려면 임시 SSL 인증서를 추가하고 해당 인증서를 보유할 볼륨을 마운트해야 합니다.
Windows, macOS 및 Linux에 필요한 단계를 설명하는 [Microsoft Docs](https://docs.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-3.1)를 찾을 수 있습니다.

윈도우의 경우:
인증서를 만들려면 터미널에서 다음을 실행해야합니다.
'dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p Your_password123'
'dotnet dev-certs https --trust'

참고: PowerShell을 사용하는 경우 %USERPROFILE%를 $env:USERPROFILE로 바꿉니다.

맥OS의 경우:
'dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p Your_password123'
'dotnet dev-certs https --trust'

리눅스의 경우:
'dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p Your_password123'

도커 컨테이너를 빌드하고 실행하려면 docker-compose.yml 파일을 찾은 솔루션의 루트에서 'docker-compose -f 'docker-compose.yml'up --build'를 실행하십시오.  디버깅 목적으로 Visual Studio의 "Docker Compose"를 사용할 수도 있습니다.
그런 다음 브라우저에서 http://localhost:5000 를 엽니 다.

### 데이터베이스 구성

템플릿은 기본적으로 메모리 내 데이터베이스를 사용하도록 구성됩니다. 이렇게 하면 모든 사용자가 추가 인프라(예: 서버)를 설정할 필요 없이 솔루션을 실행할 수 있.SQL니다.

SQL Server를 사용하려면 다음과 같이 **WebAPI/appsettings.json**을 업데이트해야 합니다.

'''json
  "UseInMemoryDatabase": false,
```

**appsettings.json** 내의 **DefaultConnection** 연결 문자열이 유효한 SQL Server 인스턴스를 가리키는지 확인합니다. 

응용 프로그램을 실행하면 데이터베이스가 자동으로 만들어지고(필요한 경우) 최신 마이그레이션이 적용됩니다.

### 데이터베이스 마이그레이션

마이그레이션에 'dotnet-ef'를 사용하려면 명령에 다음 플래그를 추가하십시오 (값은 저장소 루트에서 실행 중이라고 가정).

* '--project src/Infrastructure' (이 폴더에 있는 경우 선택 사항)
* '--스타트업 프로젝트 src/WebAPI'
* '--output-dir 마이그레이션'

예를 들어 루트 폴더에서 새 마이그레이션을 추가하려면 다음과 같이 하십시오.

'dotnet ef migrations add "SampleMigration" --project src\Infrastructure --startup-project src\Server --output-dir Migrations'

## 개요

### 도메인

여기에는 도메인 계층과 관련된 모든 엔터티, 열거형, 예외, 인터페이스, 유형 및 논리가 포함됩니다.

### 응용 프로그램

이 계층에는 모든 응용 프로그램 논리가 포함됩니다. 도메인 계층에 종속되지만 다른 계층이나 프로젝트에 대한 종속성은 없습니다. 이 계층은 외부 계층에 의해 구현되는 인터페이스를 정의합니다. 예를 들어 응용 프로그램이 알림 서비스에 액세스해야 하는 경우 새 인터페이스가 응용 프로그램에 추가되고 인프라 내에서 구현이 만들어집니다.

### 인프라

이 계층에는 파일 시스템, 웹 서비스, SMTP 등과 같은 외부 리소스에 액세스하기 위한 클래스가 포함되어 있습니다. 이러한 클래스는 응용 프로그램 계층 내에 정의된 인터페이스를 기반으로 해야 합니다.

### 웹

이 계층은 Blazor와 Core 5의 서버 및 API를 기반으로 하는 단일 페이지 응용 ASP.NET 그램입니다. 이 계층은 응용 프로그램 및 인프라 계층 모두에 따라 다르지만 인프라에 대한 종속성은 only 종속성 주입을 지원합니다. 따라서 *Startup.cs*만 인프라를 참조해야 합니다.
