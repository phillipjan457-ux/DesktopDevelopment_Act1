# Laboratory Activity 3 Database Design



## 1. Purpose



Replace the in-memory repositories with SQLite and Entity Framework Core

while keeping the existing Domain, Application, Infrastructure, and

Desktop layers.



This document describes the planned schema. EF Core migrations will

create the database in a later milestone.



## 2. Review of the Existing Models



Student currently contains StudentId, Name, Program, and IsAuthorized.



Equipment currently contains EquipmentId, Name, Type, and

IsActivelyBorrowed.



Borrowing currently contains BorrowId, Student, Equipment, BorrowDate,

ReturnDate, and Status.



All existing identifiers are strings, so they will remain strings.

ReturnDate currently means the expected return date, not the actual

date equipment was returned.



The Borrowing model will gain explicit StudentId and EquipmentId

foreign-key properties and an optional ReturnedAt property. Its

Student and Equipment navigation properties will remain.



## 3. Students Table



| Column | SQLite type | Required | Purpose |

|---|---|---|---|

| StudentId | TEXT | Yes | Primary key |

| Name | TEXT | Yes | Student's name |

| Program | TEXT | Yes | Student's academic program |

| IsAuthorized | INTEGER | Yes | 1 means allowed to borrow; 0 means not allowed |



StudentId must be unique and nonblank.

Name and Program must be nonblank.

Different students may have the same name.



StudentId is the application's existing identifier. A separate

StudentNumber is not currently required by its workflows.



## 4. Equipment Table



| Column | SQLite type | Required | Purpose |

|---|---|---|---|

| EquipmentId | TEXT | Yes | Primary key |

| Name | TEXT | Yes | Equipment name |

| Type | TEXT | Yes | Equipment category |

| IsActivelyBorrowed | INTEGER | Yes | 1 means borrowed; 0 means available |



EquipmentId must be unique and nonblank.

Name and Type must be nonblank.

Equipment names are not unique because multiple items can share a name.



Each Equipment row represents one physical item, not a quantity of items.



IsActivelyBorrowed is retained to preserve existing application behavior.

It must agree with the item's active borrowing state.



## 5. Borrowings Table



| Column | SQLite type | Required | Purpose |

|---|---|---|---|

| BorrowId | TEXT | Yes | Primary key; existing GUID string |

| StudentId | TEXT | Yes | Foreign key to Students.StudentId |

| EquipmentId | TEXT | Yes | Foreign key to Equipment.EquipmentId |

| BorrowDate | TEXT | Yes | Date and time borrowing started |

| ReturnDate | TEXT | Yes | Expected return date and time |

| ReturnedAt | TEXT | No | Actual return time; null while active |

| Status | INTEGER | Yes | 0 = Active; 1 = Returned |



BorrowId must be unique and nonblank.

ReturnDate must be later than BorrowDate.

ReturnedAt must not be earlier than BorrowDate when present.

Active records must have a null ReturnedAt.

Returned records must have a non-null ReturnedAt.



Dates will be handled consistently as UTC in application code and

stored using the provider's consistent date-time text format.



Status will use the existing BorrowingStatus enum values.



## 6. Relationships



One Student can have many Borrowings.

One Equipment item can have many Borrowings over time.

Each Borrowing belongs to exactly one Student and one Equipment item.



Students 1 ---- many Borrowings many ---- 1 Equipment



Foreign keys prevent a borrowing from referring to a nonexistent

student or equipment item.



Deleting a student or equipment item referenced by borrowing history

will be restricted so that history is not accidentally removed.



## 7. Constraints and Indexes



Primary keys provide unique record identifiers.



Required columns will be configured as NOT NULL.

Boolean columns will be limited to 0 or 1.

Status will be limited to Active or Returned.



Check constraints will enforce the date and status rules above.

Application validation will provide understandable error messages.



Planned indexes:

- Borrowings(StudentId, Status) for student borrowing-limit checks.

- Borrowings(EquipmentId) for equipment borrowing history.

- Borrowings(Status) for listing active borrowings.

- A unique partial index on Borrowings(EquipmentId) where Status = 0

&#x20; to prevent more than one active borrowing for the same item.



EquipmentId must not be unique across all borrowing rows because

the same item can be borrowed again after it is returned.



## 8. Avoiding Unnecessary Duplication



Borrowings will store StudentId and EquipmentId instead of copying

the student's name, program, equipment name, or equipment type.



Related details will be retrieved through relationships.



IsActivelyBorrowed overlaps with borrowing status but is retained

for compatibility. Borrowing and returning must save the borrowing

record and equipment state together in one transaction.



## 9. Rules That Remain in the Application



The application services will continue checking:

- whether the student exists and is authorized;

- whether equipment exists and is available;

- whether the student has reached the limit of five active borrowings;

- whether a borrowing is still active before returning it.



The existing default loan period is 14 days.



Database constraints support these rules but do not replace the

application services or their user-facing messages.



## 10. Next Steps



Create the database diagram and SQL examples.

Then implement the DbContext, mappings, migrations, and EF repositories.



Views and ViewModels will not contain SQL, table creation,

or database connection configuration.


