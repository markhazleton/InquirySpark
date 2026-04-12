# Unified Area Description

**Feature**: `001-unified-web-experience`
**Created**: 2026-04-12
**Task**: T004
**Purpose**: Describes the `Unified` MVC area — its purpose, boundaries, what it owns, and what it does not own.

---

## Purpose

The `Unified` area is the single user-facing surface of `InquirySpark.Web`. It is a standard ASP.NET Core MVC area (`Areas/Unified/`) that hosts all controllers, views, and view models for the consolidated experience previously split across `DecisionSpark` and `InquirySpark.Admin`.

---

## What the Unified Area Owns

- **All user-facing controllers** for both capability domains (Decision workflows, Inquiry authoring, administration, operations, analytics, governance)
- **All Razor views** for those controllers under `Areas/Unified/Views/`
- **All view models** for the Unified area under `Areas/Unified/ViewModels/`
- **Capability completion matrix** and governance UI
- **Operational readiness dashboard**

---

## What the Unified Area Does NOT Own

| Concern | Location | Reason |
|---|---|---|
| Identity UI (Login, Logout, Manage) | `Pages/` (Razor Pages) | Reused from InquirySpark.Admin via `MapRazorPages()` |
| Shared layouts | `Views/Shared/` (app root) | Accessible to both area and non-area views |
| Application-level DI registrations | `Program.cs` | T016/T016A/T016B |
| Configuration models | `Configuration/Unified/` (app root) | Separate from area views |
| Services | `Services/` (app root) | e.g., `UnifiedNavigationBuilder` |
| Static assets | `wwwroot/` | npm build pipeline |

---

## Area Registration

The area is registered in `Program.cs` with the following route pattern (T016B):

```csharp
app.MapControllerRoute(
    name: "unified",
    pattern: "Unified/{controller=Operations}/{action=Index}/{id?}",
    defaults: new { area = "Unified" });
```

Controllers in the area carry `[Area("Unified")]`.

---

## Directory Layout

```
InquirySpark.Web/
└── Areas/
    └── Unified/
        ├── Controllers/
        │   ├── OperationsController.cs
        │   ├── DecisionConversationController.cs
        │   ├── DecisionSpecificationController.cs
        │   ├── InquiryAdministrationController.cs
        │   ├── InquiryAuthoringController.cs
        │   ├── InquiryOperationsController.cs
        │   ├── OperationsSupportController.cs
        │   ├── CapabilityCompletionMatrixController.cs
        │   └── OperationalReadinessController.cs
        ├── ViewModels/
        │   ├── OperationsHomeViewModel.cs
        │   ├── Completion/CapabilityCompletionMatrixViewModel.cs
        │   └── (per-controller view model files)
        └── Views/
            ├── Operations/
            ├── DecisionConversation/
            ├── DecisionSpecification/
            ├── InquiryAdministration/
            ├── InquiryAuthoring/
            ├── InquiryOperations/
            ├── OperationsSupport/
            ├── CapabilityCompletionMatrix/
            ├── OperationalReadiness/
            ├── _ViewImports.cshtml
            └── _ViewStart.cshtml
```

---

## Conventions

- All controllers inherit from `BaseController` (providing `_logger` injection).
- All controllers use constructor injection only — no `new` for services.
- All views use the shared `_Layout.cshtml` from `Views/Shared/`.
- All list views use Bootstrap 5 + DataTables card template per the constitution.
- All action columns include `.no-sort`; buttons use Bootstrap Icon glyphs.
- No inline styles in any view.

---

## Boundaries

- The `Unified` area is the **only** user-facing area in InquirySpark.Web.
- No capabilities from DecisionSpark's `Admin` area or InquirySpark.Admin's `Inquiry` area are surfaced outside this area.
- The area has no dependency on legacy URL patterns from either source application.
