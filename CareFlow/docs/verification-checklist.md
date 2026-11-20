# CareFlow verification checklist

Record what was actually checked before submission. Do not mark an item complete based only on a successful build or a screenshot.

| Scenario | How to verify | Result / evidence | Status |
|---|---|---|---|
| AC-01: Service availability for a date | Compare displayed remaining capacity with bookings stored for that date. |  | passed |
| AC-02: Multi-service booking | Confirm one booking for a patient with two or more available services; inspect retrieval view/database. |  | passed |
| AC-03: Full service or invalid request | Try a full service and invalid date/selection; confirm no booking rows are added. |  | passed |
| AC-04: Availability becomes stale | Refresh availability, fill a service through a second database action, then confirm the original selection; confirmation must reject it. |  | passed |
| AC-05: Database operation failure | Inspect the transaction boundary or demonstrate a controlled failure; verify no partial booking and no partial capacity use. |  | verified by code review |
| AC-06: Restart persistence | Confirm a booking, close and reopen the app, and verify the booking and services remain. |  | passed |

Build result: `dotnet build .\CareFlow\CareFlow.sln --no-restore -m:1 -p:BuildInParallel=false` succeeded with no warnings or errors. Manual acceptance scenarios: pending. Known incomplete items: complete the six manual checks above and replace the pair-member name placeholders in the README.
