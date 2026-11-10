# Dentra System - Architecture & Technical Design Document

This document provides a deep architectural analysis of the **Dentra System (Dental Patient Record System)**, detailing design decisions, domain modeling, data flows, and design patterns.

---

## 1. Architectural Style: Clean Architecture

Dentra is structured following Uncle Bob's **Clean Architecture** principles, prioritizing domain independence and testability.

```
                      +-----------------------------+
                      |          Dental.UI          |
                      |   (Views, Controls, DI)     |
                      +--------------+--------------+
                                     |
                                     v
                      +-----------------------------+
                      |     Dental.Application      |
                      |   (Services, Use Cases)     |
                      +-------+-------------+-------+
                              |             |
                              v             v
       +----------------------------+     +----------------------------+
       |        Dental.Core         |     |   Dental.Infrastructure    |
       |  (Domain Models & Enums)   | <---+ (EF Core, SQL Server, Mig) |
       +----------------------------+     +----------------------------+
```

### Dependency Rules
1. **Inward Dependencies Only**: The UI layer depends on Application and Core. Application depends on Core and Infrastructure. Core depends on **nothing**.
2. **Persistence Ignorance**: Domain models in `Dental.Core` are Plain Old C# Objects (POCOs) with zero EF Core annotations or database engine dependencies.
3. **Inversion of Control**: High-level policies do not depend on low-level implementation details. DbContext and Services are injected via `Microsoft.Extensions.DependencyInjection`.

---

## 2. Domain Model & Design

### Core Entities
- **`Patient`**: Aggregate root representing a clinical subject. Owns 1:1 medical and dental histories, and has a 1:32 collection of `Tooth` entities.
- **`Tooth`**: Represents an individual anatomical tooth (numbered 1 through 32 in the Universal Numbering System). Tracks quadrant arch position and diagnostic health status.
- **`ToothRecord`**: Represents a clinical encounter/visit tied to a specific tooth. Captures visit timestamp, clinical procedures performed (e.g., endodontic canal measurement, post & core, composite restorations), and clinician notes.
- **`ToothImage`**: Represents a visual asset associated with an encounter, classified as either a clinical photograph (`ImageType.Real`) or radiological scan (`ImageType.XRay`).
- **`MedicalHistory`**: Systemic medical condition checklist covering hypertension, diabetes, cardiac conditions, bleeding disorders, allergies, and oncological treatments.
- **`DentalHistory`**: Anamnesis detailing past extractions, crowns, root canals, oral hygiene habits, and active symptoms.

### Enums
- **`ToothStatus`**:
  - `Healthy` (1): Sound, intact tooth structure.
  - `WithNotes` (2): Incipient caries or surfaces requiring monitoring.
  - `Treated` (3): Restored tooth (fillings, crowns, endodontic therapy).
  - `Problem` (4): Active pathology or trauma requiring intervention.
- **`ToothArch`**:
  - `UpperRight` (Teeth 1–8)
  - `UpperLeft` (Teeth 9–16)
  - `LowerLeft` (Teeth 17–24)
  - `LowerRight` (Teeth 25–32)
- **`ImageType`**:
  - `Real` (Intraoral / extraoral photography)
  - `XRay` (Radiological imaging)

---

## 3. Data Access & Persistence Layer

### Entity Framework Core 10
- **Code-First Approach**: Schema migrations are generated from C# models and tracked in `Dental.Infrastructure/Migrations/`.
- **Fluent API Configuration**: Configured in `AppDbContext.OnModelCreating`:
  - 1:1 `Patient` -> `MedicalHistory` (ForeignKey `PatientId`)
  - 1:1 `Patient` -> `DentalHistory` (ForeignKey `PatientId`)
  - 1:N `Patient` -> `Tooth` (ForeignKey `PatientId`)
  - 1:N `Tooth` -> `ToothRecord` (ForeignKey `ToothId`)
  - 1:N `ToothRecord` -> `ToothImage` (ForeignKey `ToothRecordId`)
- **Query Optimization**: Read-only queries use `.AsNoTracking()` to reduce memory footprint and bypass the EF Core change tracker when fetching visit histories and image details.

---

## 4. UI Layer Architecture (WPF)

### Modern Desktop Presentation
- **WPF (XAML)**: Declarative UI with clear separation between styling (Brushes, Styles, ControlTemplates) and code-behind.
- **Dependency Injection Integration**:
  - `App.xaml.cs` sets up `ServiceCollection`, registers `AppDbContext` and scoped application services (`PatientService`, `ToothService`, `ToothRecordService`).
  - Windows obtain required services via `App.Services.GetRequiredService<T>()`.
- **Responsive Layout**: Two-column responsive Grid architecture with a dark navy fixed sidebar (`#063B75`) and a dynamic scrollable content surface.
- **Modal Communication**: Specialized dialogs (`AddNewVisitWindow`, `ImageViewerWindow`, `AddNoteWindow`) communicate state back to caller windows via properties and event handlers.

---

## 5. Security & Reliability

1. **SQL Injection Immunity**: All database queries are executed via EF Core parameterized LINQ queries, preventing SQL injection vulnerabilities.
2. **Safe Image Management**: Image uploads generate non-colliding `Guid`-based file names and are isolated into structured patient/tooth folders.
3. **Exception Resilience**: Robust try-catch blocks and user-friendly dialogs protect against missing files or database connection interruptions.
