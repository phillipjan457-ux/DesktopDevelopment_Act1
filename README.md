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

```
ConsoleDemo (Executable)
        │
        ▼
    Application
        │      ▲
        ▼      │
     Domain    │
               │
          Infrastructure
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

```
Avalonia View
      │
      │  Binding / Command
      ▼
  ViewModel
      │
      │  Application Operation
      ▼
Application Service
      │
      ├──────────► Domain
      │
      ▼
Repository Interface
      ▲
      │
Infrastructure Implementation
```

## 7. Borrow Equipment Flow

User selects a student and equipment in EquipmentView, clicks "Borrow Equipment" -->  That's bound to EquipmentViewModel.BorrowCommand --> The ViewModel checks presentation-level validity (is a student/equipment actually selected?) — if not, sets StatusMessage and stops -->  If valid, it calls _borrowEquipmentService.BorrowEquipmentAsync() — this is the moment control passes from UI to business logic --> BorrowEquipmentService runs all business rules (student exists? authorized? equipment exists? available? under borrow limit?) using the repositories --> If all rules pass, it creates a Borrowing domain object, saves it via IBorrowingRepository, marks the Equipment as borrowed via IEquipmentRepository --> It returns a BorrowResult back up to the ViewModel -->  The ViewModel sets StatusMessage from the result and reloads the list if successful, the View updates automatically because of data binding

## 8. Return Equipment Flow



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

IStudentRepository, IEquipmentRepository, IBorrowingRepository (the interfaces themselves) and everything that depends on them — BorrowEquipmentService, ReturnEquipmentService, all the ViewModels will stay exactly the same. Only a new SqliteEquipmentRepository type class in Infrastructure would need to be written, since it implements the same interface contract.