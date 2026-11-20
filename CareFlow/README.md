# CareFlow - Diagnostic Booking Management

ITSD81 Performance Task 4 desktop application.

## Pair members

- Member 1: **Phillip Jan Lemuel O. Pagasi-an II**
-

## Requirements

- Windows with the .NET 10 SDK.
- No database server is required. CareFlow stores its SQLite database at `%LOCALAPPDATA%\CareFlow\careflow.db`.

## Initialize and run

From the repository root:

```powershell
dotnet restore .\CareFlow\CareFlow.sln
dotnet build .\CareFlow\CareFlow.sln
dotnet run --project .\CareFlow\src\CareFlow.Desktop\CareFlow.Desktop.csproj
```

On first launch, EF Core applies the checked-in database migration and creates fictional sample patients and diagnostic services. Confirmed bookings are retained when the application is closed and reopened.

## Booking rules

- Select an existing patient, today or a future date, and one to three distinct diagnostic services.
- Availability is calculated from stored bookings for the selected date.
- Capacity is checked again as part of confirmation. The booking and its service rows are saved in one database transaction; a rejected or failed request is rolled back.
- Confirmed bookings are permanent for this activity. Cancellation, rescheduling, patient registration, and service administration are out of scope.

## Project layout

- `src/CareFlow.Domain` - Patient, diagnostic service, booking, and booking-service entities.
- `src/CareFlow.Application` - Booking validation, availability records, and repository contract.
- `src/CareFlow.Infrastructure` - EF Core SQLite persistence, transaction boundary, migration, and fictional seed data.
- `src/CareFlow.Desktop` - Avalonia booking and retrieval interface.
- `docs/relational-design.md` - Entity relationship diagram and keys.
- `docs/verification-checklist.md` - Acceptance verification record.

## AI assistance acknowledgement

AI-assisted development was used to help plan and implement parts of the CareFlow application and supporting documentation. The pair reviewed and remains responsible for the submitted work.

