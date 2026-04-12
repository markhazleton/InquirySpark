# UX Consistency Checklist

**Spec:** `001-unified-web-experience`  
**Task:** T037  
**Status:** Active (validate each view as it is created or modified)

---

## Purpose

This checklist is run against every view file in `InquirySpark.Web/Areas/Unified/`. Each item must be checked before a Phase 4 UX task is marked complete, and before any PR targeting the Unified area is merged.

Authoritative conventions are in [`contracts/unified-ux-conventions.md`](./unified-ux-conventions.md).

---

## Checklist

### Layout & Structure

- [ ] View begins with `@await Html.PartialAsync("Unified/_PageHeaderActions", ...)` or equivalent
- [ ] Card structure used: `card border-0 shadow-sm` → `card-header` → `card-body p-0` → `card-footer`
- [ ] Card header uses `bg-primary text-white d-flex justify-content-between align-items-center`
- [ ] `h2` or `h1` heading is inside `card-header` with `mb-0` class
- [ ] Card footer shows record count (Total: N records) and capability ID badge

### Bootstrap Utility Rules

- [ ] No inline `style=""` attributes on any element _(exception: Bootstrap progress bar `width`/`height` inline styles are required by Bootstrap's component design)_
- [ ] No custom CSS class definitions in `.cshtml` files
- [ ] All colors use Bootstrap utilities (bg-*, text-*, btn-*)
- [ ] All spacing uses Bootstrap utilities (mb-*, p-*, gap-*)

### Icons

- [ ] All icons use Bootstrap Icons (`bi bi-*`) classes
- [ ] Icon-only buttons have `title=""` tooltip attribute
- [ ] Section icons are appropriate for the domain (see UX conventions)

### DataTables

- [ ] List tables have `.table.table-striped.table-hover.align-middle` classes
- [ ] Action column header has `class="no-sort"`
- [ ] `<th>` elements use `scope="col"` for accessibility
- [ ] DataTables not disabled unless explicitly justified

### Action Buttons

- [ ] Action groups use `<div class="btn-group btn-group-sm">` wrapper
- [ ] View/Details button: `btn btn-outline-info` + `bi-eye` icon
- [ ] Edit button: `btn btn-outline-primary` + `bi-pencil` icon
- [ ] Delete button: `btn btn-outline-danger` + `bi-trash` icon
- [ ] Create button in card header: `btn btn-light btn-sm` + `bi-plus-circle` icon

### Terminology

- [ ] All user-visible labels match canonical terms in `TerminologyMap`
- [ ] No legacy terms ("App", "Spec", "Conversation") used as standalone labels
- [ ] Capability ID badges use the format `CAP-{domain}-{number}`

### Authorization

- [X] Authorization policy attribute applied per `unified-ux-conventions.md §9`
- [X] Anonymous-accessible views are explicitly tagged with `[AllowAnonymous]`
- [X] No capability view is missing an authorization attribute

### Accessibility

- [ ] All icon-only buttons have `title=""` or `aria-label=""`
- [ ] All images have `alt=""` attributes
- [ ] Table headers use `<th scope="col">`
- [ ] Form labels associated with inputs via `for` attribute or `<label asp-for="">`

### Navigation

- [X] Active state is detectable via `NavKey` (lowercase `/unified/` prefix)
- [ ] No hardcoded breadcrumb HTML without `ViewData["Breadcrumb"]` support

---

## Phase 4 US2 Validation Status

| View File | Header Partial | Card Pattern | DataTables | Action Buttons | Terminology | Authorization |
|---|---|---|---|---|---|---|
| `Operations/Index.cshtml` | ✅ | ✅ | N/A (dashboard) | N/A | ✅ | ✅ (authenticated) |
| All other Unified views | Pending | Pending | Pending | Pending | Pending | Pending |

_Update this table as views are completed in Phase 5+._
