[! [Δημιουργήστε και αναπτύξτε ASP.Net βασική εφαρμογή στο Azure Web App - Συνεργασία] (https://github.com/fgilde/CleanArchitectureBaseBlazor/actions/workflows/master_coworkee.yml/badge.svg)] (https://github.com/fgilde/CleanArchitectureBaseBlazor/actions/workflows/master_coworkee.yml)

Εφαρμογή μίας σελίδας (Blazor) και ένας ASP.NET κεντρικός διακομιστής ακολουθώντας τις αρχές της καθαρής αρχιτεκτονικής. 
<br/>

Αυτή η λύση είναι η νέα επόμενη έκδοση του [CleanArchitectureBase](https://github.com/fgilde/CleanArchitectureBase) 
Τώρα με μια διαδικτυακή συνέλευση blazor Frontend.

[Μια επίδειξη που εκτελείται είναι διαθέσιμη εδώ] (https://coworkee.azurewebsites.net/)

## Τεχνολογίες

* ASP.NET Πυρήνας 6
* [Πυρήνας πλαισίου οντότητας 6](https://docs.microsoft.com/en-εμάς/ef/core/)
* [Σήμα R](https://docs.microsoft.com/en-ΗΠΑ/aspnet/signalr/επισκόπηση/έναρξη/εισαγωγή-σε-σηματοδότη)
* [Blazor](https://dotnet.microsoft.com/en-εμάς/εφαρμογές/aspnet/web-εφαρμογές/blazor)
* [Mud Blazor](https://mudblazor.com/getting ξεκίνησε/εγκατάσταση#χειροκίνητη εγκατάσταση)
* [Μεσολαβητής](https://github.com/jbogard/MediatR)
* [Ευφράδεια](https://fluentvalidation.net/)
* [NUnit](https://nunit.org/), [FluentAssertions](https://fluentassertions.com/), [Moq](https://github.com/moq) & [Respawn](https://github.com/jbogard/Respawn)
* [Ντόκερ](https://www.docker.com/)

## Ξεκινώντας

1. Εγκαταστήστε το πιο πρόσφατο [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
2. Μεταβείτε στο 'src/Server' και εκτελέστε το 'dotnet run' για να ξεκινήσετε το πίσω άκρο και το πρόγραμμα-πελάτη webassembly (ASP.NET Core Web API) ή ανοίξτε τη Λύση στο Visual Studio και ξεκινήστε το Server
	(Ειδοποίηση για πλήρη διαχωρισμό του υπολογιστή-πελάτη από το διακομιστή, απλώς καταργήστε την αναφορά σε έργο προγράμματος-πελάτη στο Server.csproj)

### Ρύθμιση παραμέτρων docker (ημιτελής)

Για να λειτουργήσει το Docker, θα πρέπει να προσθέσετε ένα προσωρινό πιστοποιητικό SSL και να τοποθετήσετε έναν τόμο για να κρατήσετε αυτό το πιστοποιητικό.
Μπορείτε να βρείτε [Έγγραφα της Microsoft] (https://docs.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-3.1) που περιγράφουν τα βήματα που απαιτούνται για τα Windows, macOS και Linux.

Για Windows:
Τα ακόλουθα θα πρέπει να εκτελεστούν από το τερματικό σας για να δημιουργήσετε ένα πιστοποιητικό
'dotnet dev-certs https -ep%USERPROFILE%\.aspnet\https\aspnetapp.pfx -p Your_password123»
'dotnet dev-certs https --εμπιστοσύνη'

ΣΗΜΕΙΩΣΗ: Κατά τη χρήση του PowerShell, αντικαταστήστε το %USERPROFILE% με $env:USERPROFILE.

ΓΙΑ macOS:
'dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p Your_password123»
'dotnet dev-certs https --εμπιστοσύνη'

ΓΙΑ Linux:
'dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p Your_password123»

Για να δημιουργήσετε και να εκτελέσετε τα κοντέινερ docker, εκτελέστε το αρχείο 'docker-compose -f 'docker-compose.yml' μέχρι --build' από τη ρίζα της λύσης όπου βρίσκετε το αρχείο docker-compose.yml.  Μπορείτε επίσης να χρησιμοποιήσετε το "Docker Compose" από το Visual Studio για σκοπούς εντοπισμού σφαλμάτων.
Στη συνέχεια, ανοίξτε http://localhost:5000 στο πρόγραμμα περιήγησής σας.

Ρύθμιση παραμέτρων βάσης δεδομένων ###

Το πρότυπο έχει ρυθμιστεί ώστε να χρησιμοποιεί μια βάση δεδομένων στη μνήμη από προεπιλογή. Αυτό εξασφαλίζει ότι όλοι οι χρήστες θα μπορούν να εκτελέσουν τη λύση χωρίς να χρειάζεται να θέτουν πρόσθετες υποδομές (π.χ.SQL Server).

Εάν θέλετε να χρησιμοποιήσετε τον SQL Server, θα πρέπει να ενημερώσετε **WebAPI/appsettings.json** ως εξής:

''Json''
  "Χρήση ΣεΜέμοριDatabase": ψευδές,
```

Βεβαιωθείτε ότι η συμβολοσειρά σύνδεσης **DefaultConnection** μέσα στο **appsettings.json** οδηγεί σε μια έγκυρη παρουσία του SQL Server. 

Όταν εκτελείτε την εφαρμογή, η βάση δεδομένων θα δημιουργηθεί αυτόματα (εάν είναι απαραίτητο) και θα εφαρμοστούν οι πιο πρόσφατες μετεγκαταστάσεις.

### Μετεκκαταστάσεις βάσεων δεδομένων

Για να χρησιμοποιήσετε το 'dotnet-ef' για τις μετεγκαταστάσεις σας, προσθέστε τις ακόλουθες σημαίες στην εντολή σας (οι τιμές προϋποθέτουν ότι εκτελείτε από τη ρίζα του αποθετηρίου)

* '--έργο src/Υποδομή' (προαιρετικό εάν σε αυτόν το φάκελο)
* '--εκκίνηση-έργο src/WebAPI'
* '--μετανάστευση εξόδου- dir'

Για παράδειγμα, για να προσθέσετε μια νέα μετεγκατάσταση από το ριζικό φάκελο:

«Οι μεταναστεύσεις dotnet ef προσθέτουν "SampleMigration" --έργο src\Υποδομή --εκκίνηση-έργο src\Server --μετανάστευση εξόδου- dir»

## Επισκόπηση

### Τομέας

Αυτό θα περιέχει όλες τις οντότητες, τα απαρίθμήματα, τις εξαιρέσεις, τις διεπαφές, τους τύπους και τη λογική ειδικά για το επίπεδο τομέα.

### Εφαρμογή

Αυτό το επίπεδο περιέχει όλη τη λογική εφαρμογής. Εξαρτάται από το επίπεδο τομέα, αλλά δεν έχει εξαρτήσεις από οποιοδήποτε άλλο επίπεδο ή έργο. Αυτό το επίπεδο ορίζει διεπαφές που υλοποιούνται από εξωτερικές στρώσεις. Για παράδειγμα, εάν η εφαρμογή χρειάζεται πρόσβαση σε μια υπηρεσία κοινοποίησης, θα προστεθεί μια νέα διεπαφή στην εφαρμογή και θα δημιουργηθεί μια εφαρμογή εντός της υποδομής.

### Υποδομή

Αυτό το επίπεδο περιέχει για πρόσβαση σε εξωτερικούς πόρους, όπως συστήματα αρχείων, υπηρεσίες web, smtp και ούτω καθεξής. Αυτές οι θα πρέπει να βασίζονται σε διεπαφές που ορίζονται εντός του επιπέδου εφαρμογής.

### Ιστός

Αυτό το επίπεδο είναι η εφαρμογή μίας σελίδας που βασίζεται στο Blazor και το Διακομιστή και το API ως ASP.NET πυρήνα 5. Αυτό το επίπεδο εξαρτάται τόσο από τα επίπεδα εφαρμογής όσο και από τα επίπεδα υποδομής, ωστόσο, η εξάρτηση από την υποδομήnly για την υποστήριξη της ένεσης εξάρτησης. Ως εκ τούτου, μόνο *Εκκίνηση.cs* θα πρέπει να αναφέρεται στην Υποδομή.
