[![コア アプリ ASP.Net ビルドして Azure Web App にデプロイする - Coworkee](https://github.com/fgilde/CleanArchitectureBaseBlazor/actions/workflows/master_coworkee.yml/badge.svg)](https://github.com/fgilde/CleanArchitectureBaseBlazor/actions/workflows/master_coworkee.yml)

シングル ページ アプリ (Blazor) とクリーン アーキテクチャの原則に従った ASP.NET コア サーバー。
<br/>

このソリューションは [CleanArchitectureBase](https://github.com/fgilde/CleanArchitectureBase) の新しいバージョンです。
Blazor Web アセンブリ フロントエンドが追加されました。

[実行中のデモはこちらから入手できます](https://coworkee.azurewebsites.net/)

## テクノロジー

*ASP.NET コア6
* [Entity Framework Core 6](https://docs.microsoft.com/en-us/ef/core/)
* [シグナルR](https://docs.microsoft.com/en-US/aspnet/signalr/overview/gettting-started/introduction-to-signalr)
* [Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor)
* [泥のブレイザー](https://mudblazor.com/getting 起動/インストール#手動インストール)
* [メディアR](https://github.com/jbogard/MediatR)
* [FluentValidation](https://fluentvalidation.net/)
* [NUnit](https://nunit.org/), [FluentAssertions](https://fluentassertions.com/), [Moq](https://github.com/moq) & [Respawn](https://github.com/jbogard/Respawn)
* [ドッカー](https://www.docker.com/)

## はじめに

1. 最新の [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) をインストールする
2. 'src/Server' に移動し、'dotnet run' を実行してバックエンドと Web アセンブリ クライアント (コア Web API ASP.NET) を起動するか、Visual Studio でソリューションを開いてサーバーを起動します。
	(クライアントをサーバーから完全に分離するには、Server.csprojのクライアントプロジェクトへの参照を削除するだけです)

### ドッカーの設定 (未完成)

Dockerを動作させるには、一時的なSSL証明書を追加し、その証明書を保持するボリュームをマウントする必要があります。
Windows、macOS、および Linux に必要な手順を記述した [Microsoft Docs](https://docs.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-3.1) があります。

ウィンドウズの場合:
証明書を作成するには、端末から以下を実行する必要があります
'dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p Your_password123'
'dotnet dev-certs https --trust'

注: PowerShell を使用する場合は、%USERPROFILE% を $env:USERPROFILE に置き換えてください。

マコの場合:
'dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p Your_password123'
'dotnet dev-certs https --trust'

リナックスの場合:
'dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p Your_password123'

ドッカーコンテナをビルドして実行するには、 'docker-compose -f 'docker-compose.yml' up --build'を、docker-compose.ymlファイルがあるソリューションのルートから実行します。 また、Visual Studio の "Docker Compose" をデバッグの目的で使用することもできます。
次に、ブラウザで http://localhost:5000 を開きます。

### データベース設定

テンプレートは、既定でメモリ内データベースを使用するように構成されています。これにより、すべてのユーザーが追加のインフラストラクチャ (サーバーなど) をセットアップすることなくソリューションを実行.SQLきます。

SQL Server を使用する場合は、次のように **WebAPI/appsettings.json** を更新する必要があります。

'''json
  "UseInMemoryDatabase": false,
```

**appsettings.json** 内の **DefaultConnection** 接続文字列が有効な SQL Server インスタンスを指していることを確認します。

アプリケーションを実行すると、必要に応じてデータベースが自動的に作成され、最新の移行が適用されます。

### データベースの移行

移行に 'dotnet-ef'を使用するには、コマンドに次のフラグを追加してください(値はリポジトリルートから実行していることを前提としています)

* '--project src/Infrastructure' (このフォルダにある場合はオプション)
* '--スタートアッププロジェクト src/WebAPI'
* '--出力ディレクトリの移行'

たとえば、ルートフォルダから新しい移行を追加するには:

'dotnet ef migrations add "SampleMigration" --project src\Infrastructure --startup-project src\Server --output-dir Migrations'

## 概要

### ドメイン

これには、ドメイン層に固有のすべてのエンティティ、列挙型、例外、インターフェイス、型、およびロジックが含まれます。

### アプリケーション

このレイヤーには、すべてのアプリケーション ロジックが含まれています。ドメイン層に依存しますが、他の層やプロジェクトには依存しません。このレイヤーは、外部レイヤーによって実装されるインターフェイスを定義します。たとえば、アプリケーションが通知サービスにアクセスする必要がある場合は、新しいインターフェイスがアプリケーションに追加され、インフラストラクチャ内に実装が作成されます。

### インフラストラクチャ

このレイヤーには、ファイルシステム、Web サービス、smtp などの外部リソースにアクセスするためのクラスが含まれています。これらのクラスは、アプリケーション層内で定義されたインターフェイスに基づいている必要があります。

### ウェブ

このレイヤーは、Blazor と Core 5 としてのサーバーと API に基づく単一ページ アプリケ ASP.NET ションです。この層はアプリケーション層とインフラストラクチャ層の両方に依存しますが、インフラストラクチャへの依存度は依存性注入をサポートするためにnly。したがって、*スタートアップ.cs*のみがインフラストラクチャを参照する必要があります。
