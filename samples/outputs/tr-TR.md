[! [ASP.Net Core uygulaması oluşturma ve Azure Web App'e dağıtma - Coworkee] (https://github.com/fgilde/CleanArchitectureBaseBlazor/actions/workflows/master_coworkee.yml/badge.svg)] (https://github.com/fgilde/CleanArchitectureBaseBlazor/actions/workflows/master_coworkee.yml)

Tek Sayfa Uygulaması (Blazor) ve Temiz Mimari ilkelerini izleyen ASP.NET Core Server. 
<br/>

Bu çözüm yeni [CleanArchitectureBase](https://github.com/fgilde/CleanArchitectureBase) Sonraki Sürümüdür 
Şimdi Blazor Web Assembly Ön Ucu ile.

[Çalışan bir demo burada mevcuttur] (https://coworkee.azurewebsites.net/)

## Teknolojiler

* ASP.NET Çekirdek 6
* [Entity Framework Core 6](https://docs.microsoft.com/en-us/ef/core/)
* [Sinyal R](https://docs.microsoft.com/en-US/aspnet/signalr/overview/getting-started/introduction-to-signalr)
* [Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor)
* [Çamur Blazor] (https://mudblazor.com/getting-başlattı/kurulum#manuel-kurulum)
* [MediatR](https://github.com/jbogard/MediatR)
* [FluentValidation](https://fluentvalidation.net/)
* [NUnit](https://nunit.org/), [FluentAssertions](https://fluentassertions.com/), [Moq](https://github.com/moq) & [Respawn](https://github.com/jbogard/Respawn)
* [Docker](https://www.docker.com/)

## Başlarken

1. En son [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) sürümünü yükleyin
2. Arka ucu ve webassembly istemcisini (ASP.NET Core Web API'si) başlatmak için 'src/Server' öğesine gidin ve 'dotnet run' komutunu çalıştırın veya Visual Studio'da Çözüm'ü açın ve Server'ı başlatın
	(İstemciyi sunucudan tamamen ayırma bildirimi, Server.csproj'daki istemci projesine başvuruyu kaldırmanız yeterlidir)

### Docker Yapılandırması (Bitmemiş)

Docker'ın çalışmasını sağlamak için geçici bir SSL sertifikası eklemeniz ve bu sertifikayı tutmak için bir birim bağlamanız gerekir.
Windows, macOS ve Linux için gerekli adımları açıklayan [Microsoft Dokümanlar](https://docs.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-3.1) dosyasını bulabilirsiniz.

Windows için:
Sertifika oluşturmak için terminalinizden aşağıdakilerin yürütülmesi gerekir
'dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p Your_password123'
'dotnet dev-certs https --trust'

NOT: PowerShell kullanırken, %USERPROFILE% değerini $env:USERPROFILE ile değiştirin.

macOS İÇİN:
'dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p Your_password123'
'dotnet dev-certs https --trust'

Linux İÇİN:
'dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p Your_password123'

docker kapsayıcılarını oluşturmak ve çalıştırmak için, docker-compose.yml dosyasını bulduğunuz çözümün kökünden 'docker-compose -f 'docker-compose.yml' up --build' komutunu yürütün.  Hata Ayıklama amacıyla Visual Studio'dan "Docker Compose" komutunu da kullanabilirsiniz.
Ardından tarayıcınızda http://localhost:5000'ı açın.

### Veritabanı Yapılandırması

Şablon, varsayılan olarak bellek içi veritabanı kullanacak şekilde yapılandırılır. Bu, tüm kullanıcıların ek altyapı (örneğin.SQL Server) kurmaya gerek kalmadan çözümü çalıştırabilmesini sağlar.

SQL Server'ı kullanmak istiyorsanız, **WebAPI/appsettings.json** öğesini aşağıdaki gibi güncelleştirmeniz gerekir:

''json
  "UseInMemoryDatabase": false,
```

**appsettings.json** içindeki **DefaultConnection** bağlantı dizesinin geçerli bir SQL Server örneğine işaret ettiğini doğrulayın. 

Uygulamayı çalıştırdığınızda, veritabanı otomatik olarak oluşturulur (gerekirse) ve en son geçişler uygulanır.

### Veritabanı Geçişleri

Geçişlerinizde 'dotnet-ef' kullanmak için lütfen komutunuza aşağıdaki bayrakları ekleyin (değerler, depo kökünden yürüttüğünüzü varsayar)

* '--project src/Infrastructure' (bu klasörde ise isteğe bağlı)
* '--startup-project src/WebAPI'
* '--output-dir Migrations'

Örneğin, kök klasörden yeni bir geçiş eklemek için:

'dotnet ef migrations add "SampleMigration" --project src\Infrastructure --startup-project src\Server --output-dir Migrations'

## Genel Bakış

### Etki Alanı

Bu, etki alanı katmanına özgü tüm varlıkları, enumları, özel durumları, arabirimleri, türleri ve mantığı içerir.

### Uygulama

Bu katman tüm uygulama mantığını içerir. Etki alanı katmanına bağımlıdır, ancak başka bir katmana veya projeye bağımlılığı yoktur. Bu katman, dış katmanlar tarafından uygulanan arabirimleri tanımlar. Örneğin, uygulamanın bir bildirim hizmetine erişmesi gerekiyorsa, uygulamaya yeni bir arabirim eklenir ve altyapı içinde bir uygulama oluşturulur.

### Altyapı

Bu katman, dosya sistemleri, web hizmetleri, smtp vb. gibi dış kaynaklara erişmek için sınıflar içerir. Bu sınıflar, uygulama katmanı içinde tanımlanan arabirimleri temel almalıdır.

### Web

Bu katman, Blazor ve Core 5'ASP.NET Sunucu ve API'yi temel alan tek sayfalık bir uygulamadır. Bu katman hem Uygulama hem de Altyapı katmanlarına bağlıdır, ancak Altyapıya bağımlılık obağımlılık enjeksiyonunu desteklemek için nly. Bu nedenle yalnızca *Startup.cs* Altyapı'ya başvurmalıdır.
