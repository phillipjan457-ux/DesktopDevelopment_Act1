# Persistence Verification

## Applied Migrations

The application database records InitialCreate and SeedInitialData
as applied using EF Core 10.0.12.

![Applied migrations](screenshots/migration-history.png)

## Stored Borrowing History

Before returning the Camera, the database contained four returned
Laptop loans with return timestamps and one active Camera loan
with ReturnedAt set to NULL.

![Stored borrowing history](screenshots/borrowing-history.png)

## Borrowing Survives Restart

After closing and reopening the application, John Doe's Camera
loan remained visible in Active Borrowings.

![Active loan after restart](screenshots/active-borrowing-after-restart.png)

## Return Survives Restart

The Camera was returned successfully.

![Return success](screenshots/return-success.png)

After another application restart, Active Borrowings was empty.

![No active loans after restart](screenshots/no-active-borrowings-after-restart.png)

Camera appeared in Available Equipment with Borrowed: False.

![Available equipment after restart](screenshots/available-equipment-after-restart.png)

## Database Tables

The SQLite viewer shows Students, Equipment, Borrowings,
and the EF Core migration tables.

![Database tables](screenshots/database-tables.png)

## Stored Students

The database contains three seeded students. John Doe and Alice Johnson
are authorized to borrow; Jane Smith is not.

![Stored students](screenshots/stored-students.png)

## Successful Build

The complete solution built successfully using:

dotnet build .\EquipmentBorrowing\EquipmentBorrowing.sln

![Successful solution build](screenshots/build-success.png)

## Successful Borrowing

The application confirmed that equipment was borrowed successfully.

![Successful borrowing](screenshots/borrow-success.png)