# DesktopDevelopment_Act1
<!-- Testing -->
## 1. Solution Architecture

**Domain**

Holds the core concepts and rules of the borrowing system, independent of any technical concerns. This includes Student, Equipment, Borrowing, and BorrowingStatus. These classes represent what a student, a piece of equipment, and how a borrowing record is, along with behavior that protects their own state.

**Application**

Holds the use cases and business logic that coordinate Domain objects to perform an operation. BorrowEquipmentService applies the "Borrow Equipment" use case, applying rules such as student authorization, equipment availability, and borrow limits. This layer also defines the repository interfaces (IStudentRepository, IEquipmentRepository, IBorrowingRepository) that describe what data operations the application needs, without specifying how that data is stored.

**Infrastructure**

Contains the concrete implementations of the repository interfaces defined in Application. For this scenario, InMemoryStudentRepository, InMemoryEquipmentRepository, and InMemoryBorrowingRepository store data in in-memory collections (Dictionary and List) instead of a real database, since no database is required yet.

**Tests**

Contains the automated test project structure (EquipmentBorrowing.Tests), set up per the activity's requirements as an initial basis for future test coverage.

## 2. Dependency Direction

```text
ConsoleDemo --> Application
ConsoleDemo --> Infrastructure
Application --> Domain
Infrastructure --> Application
Infrastructure --> Domain
```
- **Application** depends on **Domain** and defines interfaces that **Infrastructure** implements.
- **Infrastructure** depends on both **Domain** and **Application** (to implement its repository interfaces).
- **ConsoleDemo** depends on **Application** and **Infrastructure**, tying everything together.
- **Domain** depends on nothing else.

## 3. Case Mapping

**Actor:** Student

**Use Case:** Borrow Equipment

**Application Service:** `BorrowEquipmentService.BorrowEquipmentAsync`

**Domain Objects Used:** `Student`, `Equipment`, `Borrowing`, `BorrowingStatus`

**Repository Interfaces Used:** `IStudentRepository`, `IEquipmentRepository`, `IBorrowingRepository`

**Infrastructure Implementations Used:** `InMemoryStudentRepository`, `InMemoryEquipmentRepository`, `InMemoryBorrowingRepository`

## 4. Reflection
**1. Why should the application service depend on a repository interface instead of directly depending on a database implementation?**

So that the application layer can focus on business rules, and allow the underlying storage mechanism to be swapped without changing any application logic.

**2. Which parts of your current solution could remain unchanged if SQLite were added later?**

The Domain and Application layers would remain unchanged. Adding SQLite would only need writing new repository implementations in the Infrastructure layer.

**3. Which project would eventually contain Avalonia Views?**

 A new UI project would contain Avalonia Views, separate from Domain, Application, and Infrastructure.

**4. Should an Avalonia button directly execute database queries? Why or why not?**

No. A button's click handler should call into the Application layer, which then coordinates with Infrastructure through repository interfaces. Directly executing database queries from a UI event handler would break the layered architecture.

**5. What part of your implementation represents the actual business operation requested by the actor?**

`BorrowEquipmentService.BorrowEquipmentAsync` represents the actual business operation — it enforces all the borrowing rules before creating a `Borrowing` record.

## 5. Desktop Project

The EquipmentBorrowing.Desktop is responsible for showing data(equipment list, student list, and borrowing list), Capture user actions(button clicks and selections) and Hnad off any real work to the Application layer's services thus never doing the work itself. It references Application and Infrastructure. It does not reference Domain directly for logic because it just displays Domain objects that flow to it through the ViewModels.

## 6. Updated Architecture

```text
Avalonia View
    |
    | Binding / Command
    v
ViewModel
    |
    | IEquipmentBorrowingOperations
    v
ScopedEquipmentBorrowingOperations
    |
    v
Application Service --> Domain
    |
    v
Repository Interface
    ^
    |
EF Repository Implementation
    |
    v
SQLite Database
```

## 7. Borrow Equipment Flow

User selects a student and equipment in EquipmentView, clicks "Borrow Equipment" -->  That's bound to EquipmentViewModel.BorrowCommand --> The ViewModel checks presentation-level validity (is a student/equipment actually selected?) — if not, sets StatusMessage and stops -->  If valid, it calls _borrowEquipmentService.BorrowEquipmentAsync() — this is the moment control passes from UI to business logic --> BorrowEquipmentService runs all business rules (student exists? authorized? equipment exists? available? under borrow limit?) using the repositories --> If all rules pass, it creates a Borrowing domain object, saves it via IBorrowingRepository, marks the Equipment as borrowed via IEquipmentRepository --> It returns a BorrowResult back up to the ViewModel -->  The ViewModel sets StatusMessage from the result and reloads the list if successful, the View updates automatically because of data binding

## 8. Return Equipment Flow

User selects a borrowing in BorrowingsView, clicks "Return Equipment" --> That's bound to BorrowingViewModel.ReturnCommand --> The ViewModel checks presentation-level validity (is a borrowing actually selected?) and if not, sets StatusMessage and stops --> If valid, it calls _returnEquipmentService.ReturnEquipmentAsync(SelectedBorrowing.BorrowId) — control passes from UI to business logic --> ReturnEquipmentService looks up the borrowing record by ID, checks it exists and is still Active (not already returned) --> If valid, it sets the borrowing's status to Returned and calls equipment.MarkAsAvailable() on the associated Equipment, then saves that change via IEquipmentRepository --> It returns a ReturnResult back up to the ViewModel --> The ViewModel sets StatusMessage from the result and reloads the active borrowings list if successful, so the returned item disappears from the list and the equipment shows as available again in the Equipment view.

## 9. Architectural Reflection

**1. Why should the View not call a repository directly?**

If a Button's click handler called IEquipmentRepository directly, business rules (authorization, availability checks) would either be skipped or duplicated inline in the UI which means your rules live in two places, or nowhere reliable. The View's job is display, not decision-making.

**2. Why should business rules not be implemented in the ViewModel?**

if EquipmentViewModel reimplemented the borrow limit check itself, you'd now have two sources of truth for that rule (one in the ViewModel, one in BorrowEquipmentService), and they could drift out of sync. Business rules belong in exactly one place which is the Application service.

**3. What is the responsibility of the ViewModel?**

It's the medium between View and Application where it holds UI state (SelectedStudent, StatusMessage), exposes commands the View can bind to, does lightweight presentation validation (is something selected?), and delegates any real decision to a Service.

**4. Why can the existing Application layer work without knowing that Avalonia is being used?**

Because BorrowEquipmentService and ReturnEquipmentService only depend on Domain and repository interfaces and nothing about Avalonia, UserControls, or bindings. You could swap the entire UI for a web app and these services wouldn't need to change at all. 

**5. What advantage is gained from registering dependencies in one composition point?**

A single composition root means object creation and lifetime rules (Singleton vs Transient) are decided in one place, so there's no risk of accidentally creating two different repository instances that silently disagree with each other.

**6. If the in-memory repository were replaced by SQLite later, which parts of the current interface should remain largely unchanged?**

The Views and ViewModels should remain largely unchanged because they use application services and repository interfaces. Infrastructure will provide EF Core repository implementations. Small changes may still be needed in the domain and services to store relationship IDs, record actual return times, and save related changes together.


## 10. Laboratory Activity 3

Laboratory Activity 3 introduces SQLite persistence using Entity
Framework Core while preserving the existing application layers.

### Relational Database Design

- [Database design](EquipmentBorrowing/docs/database-design.md)
- [Database diagram](EquipmentBorrowing/docs/database-diagram.md)

These documents describe the planned tables, keys, relationships,
constraints, and indexes.

### SQL Examples

[database-queries.sql](EquipmentBorrowing/docs/database-queries.sql)
contains examples of:

1. Retrieving all equipment.
2. Filtering available equipment.
3. Joining active borrowings with student and equipment details.
4. Counting active borrowings per student, including students with zero.
5. Updating an equipment name inside a transaction that is rolled back.

The queries match the planned schema. They will be tested against
SQLite after the initial EF Core migration is applied.


### EF Core Setup

Infrastructure uses EF Core SQLite and Design packages version 10.0.12.

EquipmentBorrowingDbContext exposes Students, Equipment, and Borrowings.
Separate configuration classes define their keys, required fields,
relationships, check constraints, and indexes.

A design-time factory lets EF commands create the context without
launching Avalonia. The repository's dotnet-tools.json records the
EF tool version. Run `dotnet tool restore` after cloning or pulling it.

Borrowing now includes StudentId and EquipmentId as explicit foreign
keys. ReturnDate remains the due date, while ReturnedAt records the
actual return time and stays null for active loans.

### Verification and Progress

The solution builds successfully, and `dotnet ef dbcontext script`
successfully generates the schema SQL from the mappings.

InitialCreate and SeedInitialData have been applied successfully.
SQLite Viewer confirmed three students, three available equipment items,
and two entries in __EFMigrationsHistory. Borrowings is currently empty.

Running database update again reported that the database was already
up to date, and the seed records remained unchanged.

EF repositories and a shared unit of work are implemented and tested.
The desktop application now uses EF Core repositories and SQLite.
The console demo continues to use in-memory repositories. The SQL examples have not yet been executed against the database.

### Applying the Database Migrations

Run these commands from the repository root:

```powershell
dotnet tool restore
dotnet ef database update --project .\EquipmentBorrowing\src\EquipmentBorrowing.Infrastructure --startup-project .\EquipmentBorrowing\src\EquipmentBorrowing.Infrastructure
```

The desktop application and EF migration commands use the same database:
%LOCALAPPDATA%\EquipmentBorrowing\equipment-borrowing.db.

EquipmentDatabase builds this absolute path independently of the working
directory. Run the database update command before first launching the app.
The earlier database inside Infrastructure is no longer used by this setup.

InitialCreate creates the schema. SeedInitialData inserts the starting
students and equipment once. EF tracks applied migrations in
__EFMigrationsHistory.

Migration source files are committed to Git. Local database files are
excluded through the root .gitignore.

### EF Repositories and Unit of Work

EfStudentRepository, EfEquipmentRepository, and EfBorrowingRepository
implement the existing repository interfaces using EF Core.

Display queries use AsNoTracking. Lookups used for borrowing and
returning use tracking so EF can detect changes. Borrowing queries
include the related student and equipment for displaying their names.

Repository save methods prepare changes. The application services call
IUnitOfWork.SaveChangesAsync after preparing the complete operation.
With EfUnitOfWork and a shared DbContext, the borrowing record and
equipment availability are saved together.

InMemoryUnitOfWork supports the console demo, which continues to use
in-memory repositories. The desktop now uses EfUnitOfWork.

Validation: BorrowAndReturn_SaveChangesAcrossContexts passed using a
temporary SQLite database. It applied the migrations, borrowed equipment,
verified the saved values through a new context, returned the equipment,
and verified the returned status and availability through another context.

Desktop restart persistence has been verified for borrowing and returning.
Database failure rollback has not yet been tested.

### Desktop SQLite Integration

The view models use IEquipmentBorrowingOperations to load students,
equipment, and active borrowings, and to request borrowing and returning.

ScopedEquipmentBorrowingOperations creates and disposes a dependency
injection scope for each operation. The EF repositories and unit of work
within that scope share one DbContext.

The desktop registers EF repositories and EfUnitOfWork as scoped services.
View models display a status message when loading or an operation fails.

Manual verification:
- The desktop loaded the three seeded students and equipment items.
- John Doe borrowed the Laptop successfully.
- After closing and reopening the app, the borrowing remained active.
- The Laptop was returned successfully.
- After another restart, Active Borrowings was empty and the Laptop
  displayed Borrowed: False.

The solution build passed. The SQL examples have not yet been executed
against the database.

### LINQ and Generated SQL

See [LINQ queries and generated SQL](EquipmentBorrowing/docs/linq-and-generated-sql.md)
for three query examples, two captured SQL statements, and the
tracking explanation.