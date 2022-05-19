[! [إنشاء تطبيق ASP.Net Core ونشره في Azure Web App - Coworkee] (https://github.com/fgilde/CleanArchitectureBaseBlazor/actions/workflows/master_coworkee.yml/badge.svg)] (https://github.com/fgilde/CleanArchitectureBaseBlazor/actions/workflows/master_coworkee.yml)

تطبيق صفحة واحدة (Blazor) وخادم أساسي ASP.NET يتبع مبادئ البنية النظيفة. 
<br/>

هذا الحل هو الإصدار التالي الجديد من [CleanArchitectureBase] (https://github.com/fgilde/CleanArchitectureBase) 
الآن مع واجهة Blazor Web Assembly Frontend.

[يتوفر عرض توضيحي قيد التشغيل هنا] (https://coworkee.azurewebsites.net/)

## التقنيات

* ASP.NET كور 6
* [إطار عمل الكيان الأساسي 6] (https://docs.microsoft.com/en-us/ef/core/)
* [إشارة R] (https://docs.microsoft.com/en-الولايات المتحدة / aspnet / إشارة / نظرة عامة / الحصول على بدء / مقدمة إلى إشارة)
* [Blazor] (https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor)
* [الطين Blazor] (https://mudblazor.com/getting بدأت / التثبيت # دليل تثبيت)
* [MediatR] (https://github.com/jbogard/MediatR)
* [FluentValidation] (https://fluentvalidation.net/)
* [NUnit] (https://nunit.org/) ، [FluentAssertions] (https://fluentassertions.com/) ، [Moq] (https://github.com/moq) و [Respawn] (https://github.com/jbogard/Respawn)
* [دوكر] (https://www.docker.com/)

## الشروع في العمل

1. قم بتثبيت أحدث [.NET 6 SDK] (https://dotnet.microsoft.com/download/dotnet/6.0)
2. انتقل إلى "src / Server" وقم بتشغيل "dotnet run" لتشغيل الواجهة الخلفية وعميل webassembly (ASP.NET Core Web API) أو افتح الحل في Visual Studio وقم بتشغيل الخادم
	(إشعار لفصل العميل تماما عن الخادم فقط قم بإزالة الإشارة إلى مشروع العميل في Server.csproj)

### تكوين Docker (غير مكتمل)

من أجل تشغيل Docker ، ستحتاج إلى إضافة شهادة SSL مؤقتة وتركيب وحدة تخزين للاحتفاظ بهذه الشهادة.
يمكنك العثور على [Microsoft Docs] (https://docs.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-3.1) التي تصف الخطوات المطلوبة لنظام التشغيل Windows وmacOS وLinux.

لنظام التشغيل Windows:
يجب تنفيذ ما يلي من المحطة الطرفية الخاصة بك لإنشاء شهادة
'dotnet dev-certs https -ep ٪USERPROFILE٪\.aspnet\https\aspnetapp.pfx -p Your_password123'
'dotnet dev-certs https --trust'

ملاحظة: عند استخدام PowerShell، استبدل ٪USERPROFILE٪ ب $env:USERPROFILE.

لنظام التشغيل macOS:
'dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p Your_password123'
'dotnet dev-certs https --trust'

لينكس:
'dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p Your_password123'

من أجل بناء وتشغيل حاويات docker ، قم بتنفيذ "docker-compose -f 'docker-compose.yml' up --build" من جذر الحل حيث تجد ملف docker-compose.yml.  يمكنك أيضا استخدام "إنشاء Docker" من Visual Studio لأغراض تصحيح الأخطاء.
ثم افتح http://localhost:5000 على متصفحك.

### تكوين قاعدة البيانات

يتم تكوين القالب لاستخدام قاعدة بيانات في الذاكرة بشكل افتراضي. وهذا يضمن أن جميع المستخدمين سيكونون قادرين على تشغيل الحل دون الحاجة إلى إعداد بنية تحتية إضافية (على سبيل المثال.SQL Server).

إذا كنت ترغب في استخدام SQL Server ، فستحتاج إلى تحديث **WebAPI / appsettings.json ** على النحو التالي:

''''json
  "UseInMemoryDatabase": false,
```

تحقق من أن سلسلة الاتصال **DefaultConnection** داخل **appsettings.json** تشير إلى مثيل SQL Server صالح. 

عند تشغيل التطبيق ، سيتم إنشاء قاعدة البيانات تلقائيا (إذا لزم الأمر) وسيتم تطبيق أحدث عمليات الترحيل.

### عمليات ترحيل قاعدة البيانات

لاستخدام "dotnet-ef" لعمليات الترحيل الخاصة بك ، يرجى إضافة العلامات التالية إلى الأمر الخاص بك (تفترض القيم أنك تنفذ من جذر المستودع)

* "--مشروع src / البنية التحتية" (اختياري إذا كان في هذا المجلد)
* '--بدء تشغيل المشروع src / WebAPI'
* '--output-dir Migrations'

على سبيل المثال، لإضافة ترحيل جديد من المجلد الجذر:

'dotnet ef عمليات الترحيل إضافة "SampleMigration" --مشروع src\البنية التحتية --بدء التشغيل-مشروع src\الخادم --output-dir Migrations'

## نظرة عامة

### المجال

سيحتوي هذا على جميع الكيانات والتعداد والاستثناءات والواجهات والأنواع والمنطق الخاص بطبقة المجال.

### التطبيق

تحتوي هذه الطبقة على كل منطق التطبيق. يعتمد على طبقة المجال ، ولكن ليس له تبعيات على أي طبقة أو مشروع آخر. تحدد هذه الطبقة الواجهات التي يتم تنفيذها بواسطة طبقات خارجية. فعلى سبيل المثال، إذا احتاج التطبيق إلى الوصول إلى خدمة إشعار، فستضاف واجهة جديدة إلى التطبيق وسينشأ تنفيذ داخل البنية التحتية.

### البنية التحتية

تحتوي هذه الطبقة على فئات للوصول إلى الموارد الخارجية مثل أنظمة الملفات وخدمات الويب و smtp وما إلى ذلك. يجب أن تستند هذه الفئات إلى واجهات محددة داخل طبقة التطبيق.

### الويب

هذه الطبقة هي تطبيق صفحة واحدة استنادا إلى Blazor والخادم وواجهة برمجة التطبيقات كما ASP.NET Core 5. تعتمد هذه الطبقة على كل من طبقات التطبيق والبنية التحتية ، ومع ذلك ، فإن الاعتماد على البنية التحتية هو only لدعم حقن التبعية. لذلك فقط * بدء التشغيل .cs * يجب أن تشير إلى البنية التحتية.
