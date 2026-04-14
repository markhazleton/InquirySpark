# Unified UX Conventions

**Spec:** `001-unified-web-experience`  
**Task:** T031  
**Status:** Active Governance Document

---

## 1. Purpose

This document defines the authoritative UX conventions for all views in the `InquirySpark.Web` Unified area. All developers and AI coding agents must follow these conventions when creating or modifying views under `Areas/Unified/`.

---

## 2. Layout & Structure

### Card Pattern
All capability list views MUST use the Bootstrap card structure:

```html
<div class="card border-0 shadow-sm">
    <div class="card-header bg-primary text-white d-flex justify-content-between align-items-center">
        <h2 class="mb-0"><i class="bi bi-{icon}"></i> {Title}</h2>
        <!-- Optional action button(s) -->
    </div>
    <div class="card-body p-0">
        <!-- Content -->
    </div>
    <div class="card-footer text-muted d-flex justify-content-between align-items-center">
        <span><i class="bi bi-info-circle"></i> Total: @Model.Count records</span>
        <span><i class="bi bi-lightning"></i> Capability: {CAP-ID}</span>
    </div>
</div>
```

### Page Header Pattern
Every capability view MUST begin with a page header partial:

```html
@await Html.PartialAsync("Unified/_PageHeaderActions", new PageHeaderActionsModel { ... })
```

---

## 3. Bootstrap Utility Rules

| Rule | Correct Usage | Forbidden |
|---|---|---|
| Colors | `bg-primary`, `text-white`, `btn-outline-info` | Inline `style=""` attributes |
| Spacing | `mb-0`, `p-0`, `d-flex`, `gap-2` | `margin: 0px`, `padding: 0` |
| Icons | Bootstrap Icons (`bi-*`) | Font Awesome, custom SVG inline |
| Tables | `.table.table-striped.table-hover.align-middle` | Plain `<table>` without classes |
| Buttons | `btn btn-outline-{color} btn-sm` | `<a>` styled as buttons with custom CSS |

---

## 4. DataTables Standards

- All list views with tabular data MUST use `.table` class (auto-initializes DataTables)
- Action columns MUST have `class="no-sort"` on `<th>`
- Default configuration: 25 rows, state persistence (24h)
- Export (Excel/PDF/CSV): add `.datatable-export` to `<table>`
- Disable DataTables on non-list tables: `data-datatable="false"`

---

## 5. Terminology Map

All user-visible labels must use canonical unified terminology. The authoritative map is in `InquirySpark.Web/Configuration/Unified/TerminologyMap.cs`. Key mappings:

| Legacy Term (DecisionSpark) | Legacy Term (Admin) | Canonical Unified Term |
|---|---|---|
| Conversation | — | Decision Conversation |
| Spec / Decision Spec | — | Decision Specification |
| Application | App | Application |
| Survey | Survey | Survey |
| Question Group Members | Group Members | Question Group Members |
| LuApplicationType | Application Type | Application Type |
| LuQuestionType | Question Type | Question Type |

---

## 6. Action Button Standards

Capability-level action groups (View / Edit / Delete) MUST use:

```html
<div class="btn-group btn-group-sm" role="group">
    <a asp-action="Details" asp-route-id="@item.Id" class="btn btn-outline-info" title="View Details">
        <i class="bi bi-eye"></i>
    </a>
    <a asp-action="Edit" asp-route-id="@item.Id" class="btn btn-outline-primary" title="Edit">
        <i class="bi bi-pencil"></i>
    </a>
    <a asp-action="Delete" asp-route-id="@item.Id" class="btn btn-outline-danger" title="Delete">
        <i class="bi bi-trash"></i>
    </a>
</div>
```

---

## 7. Breadcrumb & Navigation

- Active route is detected via `CurrentPath.StartsWith(node.NavKey)` in `_Layout.cshtml`
- The `NavKey` must be a lowercase, slash-delimited prefix (e.g., `/unified/inquiryadministration`)
- No hardcoded breadcrumb HTML — breadcrumbs are injected via `ViewData["Breadcrumb"]` if needed

---

## 8. Error and Empty States

- Empty list state: show `alert alert-info` with descriptive message and create-new action
- Error state: show `alert alert-warning` with `<i class="bi bi-exclamation-triangle"></i>` prefix
- Loading state: not applicable (server-rendered; no spinners in base implementation)

---

## 9. Authorization Attributes

Apply the appropriate authorization policy per capability family:

| Area | Policy |
|---|---|
| Inquiry Administration | `Analyst` or `Operator` |
| Inquiry Authoring | `Analyst` |
| Inquiry Operations | `Operator` |
| Decision Workspace | `Consultant` |
| Operations Support (Charting) | `Analyst` |
| Operations Support (Health) | Anonymous |
| Capability Matrix | Any authenticated user |

---

## 10. Accessibility Requirements

- All icon-only buttons must have `title=""` attribute
- All images must have `alt=""` attribute
- Table headers must use `<th scope="col">`
- Form labels must be associated with inputs (`for` attribute)
