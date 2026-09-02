# DesktopDevelopment_Act1

1. Solution Architecture
    Domain
        Holds the core concepts and rules of the borrowing system, independent of any technical concerns. This includes Student, Equipment, Borrowing, and BorrowingStatus. These classes represent what a student, a piece of equipment, and how a borrowing record is, along with behavior that protects their own state.
    Application
        Holds the use cases and business logic that coordinate Domain objects to perform an operation. BorrowEquipmentService applies the "Borrow Equipment" use case, applying rules such as student authorization, equipment availability, and borrow limits. This layer also defines the repository interfaces (IStudentRepository, IEquipmentRepository, IBorrowingRepository) that describe what data operations the application needs, without specifying how that data is stored.
    Infrasturcture
        Contains the concrete implementations of the repository interfaces defined in Application. For this scenario, InMemoryStudentRepository, InMemoryEquipmentRepository, and InMemoryBorrowingRepository store data in in-memory collections (Dictionary and List) instead of a real database, since no database is required yet.
    Tests
        Contains the automated test project structure (EquipmentBorrowing.Tests), set up per the activity's requirements as an initial basis for future test coverage.
2. Dependency Direction

3. Case Mapping
Actor: Student

Use Case: Borrow Equipment

Application Service: BorrowEquipmentService.BorrowEquipmentAsync

Domain Objects Used: Student, Equipment, Borrowing, BorrowingStatus

Repository Interfaces Used: IStudentRepository, IEquipmentRepository, IBorrowingRepository

Infrastructure Implementations Used: InMemoryStudentRepository, InMemoryEquipmentRepository, InMemoryBorrowingRepository

4. Reflection
    Why should the application service depend on a repository interface instead of directly depending on a database implementation?

    Which parts of your current solution could remain unchanged if SQLite were added later?

    Which project would eventually contain Avalonia Views?

    Should an Avalonia button directly execute database queries? Why or why not?

    What part of your implementation represents the actual business operation requested by the actor?
