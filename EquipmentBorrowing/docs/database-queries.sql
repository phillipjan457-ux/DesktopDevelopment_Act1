-- Laboratory Activity 3
-- SQL examples for the planned SQLite database.
-- Run after the EF Core migration creates the tables.
-- BorrowingStatus: 0 = Active, 1 = Returned.


-- 1. BASIC RETRIEVAL
-- Retrieve all equipment.

SELECT EquipmentId, Name, Type, IsActivelyBorrowed
FROM Equipment
ORDER BY Name, EquipmentId;


-- 2. FILTERING
-- Retrieve equipment that is currently available.
-- IsActivelyBorrowed: 0 = false, 1 = true.

SELECT EquipmentId, Name, Type
FROM Equipment
WHERE IsActivelyBorrowed = 0
ORDER BY Name, EquipmentId;


-- 3. JOIN
-- Retrieve active borrowings with student and equipment details.
-- ReturnDate represents the expected return date.

SELECT
    b.BorrowId,
    s.Name AS StudentName,
    e.Name AS EquipmentName,
    b.BorrowDate,
    b.ReturnDate AS ExpectedReturnDate
FROM Borrowings AS b
INNER JOIN Students AS s
    ON b.StudentId = s.StudentId
INNER JOIN Equipment AS e
    ON b.EquipmentId = e.EquipmentId
WHERE b.Status = 0
ORDER BY b.ReturnDate, b.BorrowId;


-- 4. AGGREGATE
-- Count active borrowings per student.
-- Include students with zero active borrowings.

SELECT
    s.StudentId,
    s.Name AS StudentName,
    COUNT(b.BorrowId) AS ActiveBorrowingCount
FROM Students AS s
LEFT JOIN Borrowings AS b
    ON b.StudentId = s.StudentId
    AND b.Status = 0
GROUP BY s.StudentId, s.Name
ORDER BY ActiveBorrowingCount DESC, s.StudentId;


-- 5. UPDATE
-- Demonstrate changing one equipment name.
-- The current seed data uses EquipmentId '1' for the laptop.
-- Run this entire transaction block together.
-- ROLLBACK restores the original value after inspection.

BEGIN TRANSACTION;

UPDATE Equipment
SET Name = 'Laptop 01'
WHERE EquipmentId = '1';

SELECT EquipmentId, Name
FROM Equipment
WHERE EquipmentId = '1';

ROLLBACK;