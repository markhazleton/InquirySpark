# InquirySpark .NET 10.0 Upgrade Plan

## Executive Summary

### Scenario
Upgrade InquirySpark solution from .NET 8.0 to .NET 10.0 (GA) with simultaneous package updates and security vulnerability remediation.

### Scope
- **Total Projects**: 6 projects
- **Current State**: All projects targeting net8.0
- **Target State**: All projects targeting net10.0
- **Total LOC**: ~31,994 lines of code across all projects

### Selected Strategy
**Big Bang Strategy** - All projects will be upgraded simultaneously in a single atomic operation.

**Rationale for Big Bang Approach**:
- Small solution (6 projects) with manageable complexity
- All projects currently on .NET 8.0 (modern framework)
- Clear, simple dependency structure (no circular dependencies)
- All required packages have compatible versions available for .NET 10.0
- Good SDK-style project structure across all projects
- Security vulnerability requires coordinated fix across multiple projects
- Total codebase size (~32k LOC) is well within Big Bang threshold

### Complexity Assessment
**Medium Complexity**

**Justification**:
- Medium-sized codebase (~32k LOC total)
- 6 projects with straightforward dependencies
- Security vulnerability requires immediate attention (Azure.Identity)
- One incompatible package needs removal (Microsoft.VisualStudio.Azure.Containers.Tools.Targets)
- Multiple Entity Framework projects requiring coordinated EF Core 10.0 upgrade
- ASP.NET Core projects requiring framework-specific updates

### Critical Issues

#### Security Vulnerabilities (CRITICAL - Must Address)
1. **Azure.Identity 1.10.4** - MODERATE severity vulnerability
   - Affected Projects: InquirySpark.Admin, InquirySpark.Web
   - Impact: Authentication vulnerability in enterprise systems
   - Action: Upgrade to 1.17.1 (user confirmed to include in this upgrade)

#### Incompatible Packages (BLOCKING)
2. **Microsoft.VisualStudio.Azure.Containers.Tools.Targets 1.19.6**
   - Affected Projects: InquirySpark.Repository, InquirySpark.Web, InquirySpark.WebApi
   - Status: No supported version for .NET 10.0
   - Action: Remove package references (development-time tooling, not required for build/runtime)

### Recommended Approach
Proceed with **Big Bang Strategy**: Upgrade all projects simultaneously in single coordinated operation. This approach minimizes total timeline, ensures consistency across the solution, and allows for single comprehensive testing phase.

---

## Migration Strategy

### 2.1 Approach Selection

**Chosen Strategy**: Big Bang Strategy

**Justification**: 
The InquirySpark solution is well-suited for Big Bang migration based on:
- **Project Count**: 6 projects falls within small-to-medium range
- **Framework Consistency**: All projects on net8.0, no legacy frameworks
- **Dependency Simplicity**: Clean dependency tree with no circular references
- **Codebase Size**: ~32k LOC total is manageable for atomic upgrade
- **Package Compatibility**: All critical packages have .NET 10.0 versions
- **Coordination Benefits**: Security fix (Azure.Identity) benefits from simultaneous deployment
- **Risk Profile**: Low-medium risk with clear mitigation paths

Alternative incremental approach would add unnecessary complexity and extend timeline without meaningful risk reduction.

### 2.2 Dependency-Based Ordering

While Big Bang upgrades all projects simultaneously, understanding the dependency structure informs compilation order and testing priorities:

**Dependency Layers** (bottom-up):
```
Layer 1 (Foundation - No Dependencies):
  - InquirySpark.Common (1,653 LOC)
    └─ Depended on by: Repository, WebApi, Common.Tests

Layer 2 (Mid-Tier - Depends on Layer 1):
  - InquirySpark.Repository (6,703 LOC)
    └─ Depends on: Common
    └─ Depended on by: WebApi, Admin

Layer 3 (Applications - Depends on Layers 1-2):
  - InquirySpark.WebApi (356 LOC)
    └─ Depends on: Repository, Common
  
  - InquirySpark.Admin (18,974 LOC)
    └─ Depends on: Repository
  
  - InquirySpark.Web (3,813 LOC)
    └─ No project dependencies (standalone)

Layer 4 (Tests):
  - InquirySpark.Common.Tests (495 LOC)
    └─ Depends on: Common
```

**Big Bang Execution Note**: All projects will be updated simultaneously, but the dependency structure above indicates:
- Compilation will naturally proceed from Common → Repository → Applications
- Test validation should start with Common.Tests, then integration tests
- Any breaking changes will surface first in foundation projects

### 2.3 Parallel vs Sequential Execution

**Big Bang Approach - Atomic Updates**:
All project files and package references will be updated in a single coordinated operation:

**Simultaneous Updates** (all at once):
- All 6 project TargetFramework properties: net8.0 → net10.0
- All package references across all projects
- Remove incompatible packages from 3 projects
- Security fix applied to 2 projects

**Compilation Validation**:
After all updates applied, solution-wide build will validate:
- Dependency resolution across all projects
- API compatibility with new framework and packages
- Breaking changes identification

**Testing Validation**:
After successful build, execute tests:
- InquirySpark.Common.Tests (validates foundation)
- Manual validation of application projects

**Why Simultaneous Works Here**:
- Simple dependency structure (no complex interdependencies)
- All packages have clear upgrade paths
- No partial-compatibility scenarios
- Security fix requires coordination anyway

---

## Detailed Dependency Analysis

### 3.1 Dependency Graph Summary

**Migration Phases for Big Bang Strategy**:

Given the Big Bang approach, all projects migrate simultaneously. However, phases represent logical groupings for execution organization and validation priorities:

**Phase 0: Preparation**
- Remove incompatible Microsoft.VisualStudio.Azure.Containers.Tools.Targets package

**Phase 1: Atomic Upgrade**
All projects updated simultaneously:
- **Foundation Layer**: InquirySpark.Common
- **Data Layer**: InquirySpark.Repository  
- **Application Layer**: InquirySpark.Admin, InquirySpark.Web, InquirySpark.WebApi
- **Test Layer**: InquirySpark.Common.Tests

**Phase 2: Validation**
- Build entire solution
- Fix any compilation errors
- Execute test suite

### 3.2 Project Groupings

**Group 1: Foundation (1 project)**
- InquirySpark.Common
  - Role: Core shared library
  - Complexity: Low
  - Dependencies: 0 projects
  - Package Updates: 1 package

**Group 2: Data Access (1 project)**
- InquirySpark.Repository
  - Role: Entity Framework data layer
  - Complexity: Medium
  - Dependencies: InquirySpark.Common
  - Package Updates: 5 packages + remove 1 incompatible
  - Critical: EF Core 10.0 upgrade

**Group 3: Web Applications (3 projects)**
- InquirySpark.Admin (Largest project - 18,974 LOC)
  - Role: Admin Blazor application
  - Complexity: High (size, EF, Identity)
  - Dependencies: InquirySpark.Repository
  - Package Updates: 7 packages + security fix

- InquirySpark.Web
  - Role: Public-facing web application
  - Complexity: Medium (EF, Identity)
  - Dependencies: None (standalone)
  - Package Updates: 7 packages + remove 1 incompatible + security fix

- InquirySpark.WebApi
  - Role: REST API service
  - Complexity: Low (smallest application)
  - Dependencies: InquirySpark.Repository, InquirySpark.Common
  - Package Updates: 1 package + remove 1 incompatible

**Group 4: Tests (1 project)**
- InquirySpark.Common.Tests
  - Role: Unit tests for Common library
  - Complexity: Low
  - Dependencies: InquirySpark.Common
  - Package Updates: 1 package

**Strategy-Specific Grouping Notes**: 
These groups represent logical organization for understanding the solution structure. In Big Bang execution, all groups are updated in a single atomic operation. Groups help prioritize:
- Where to look first for breaking changes (Foundation → Applications)
- Testing priority order (Common.Tests → Integration)
- Code review focus areas

---

## Project-by-Project Migration Plans

### Project: InquirySpark.Common

**Current State**
- Target Framework: net8.0
- Project Type: ClassLibrary (SDK-style)
- Dependencies: None (foundation library)
- Dependants: InquirySpark.Repository, InquirySpark.WebApi, InquirySpark.Common.Tests
- LOC: 1,653
- Files: 48
- Package Count: 1

**Target State**
- Target Framework: net10.0
- Updated Packages: 1

**Migration Steps**

1. **Prerequisites**
   - None (foundation project with no dependencies)
   - First project to update in dependency order

2. **Framework Update**
   Update `InquirySpark.Common\InquirySpark.Common.csproj`:
   ```xml
   <TargetFramework>net10.0</TargetFramework>
   ```

3. **Package Updates**

   | Package | Current Version | Target Version | Reason |
   |---------|----------------|----------------|---------|
   | Microsoft.Extensions.Configuration.UserSecrets | 8.0.0 | 10.0.0 | Framework compatibility |

4. **Expected Breaking Changes**
   - **Low Risk**: Configuration extensions typically maintain backward compatibility
   - **Potential Issues**:
     - UserSecrets API changes (unlikely but verify)
     - Configuration binding behavior changes
   - **Verification**: Check any custom configuration extension methods

5. **Code Modifications**
   - **Expected**: Minimal to none
   - **Review Areas**:
     - Configuration initialization code
     - Any custom IConfiguration extensions
     - User secrets access patterns
   - **Manual Review**: Search for `AddUserSecrets` calls

6. **Testing Strategy**
   - Unit tests: InquirySpark.Common.Tests will validate this project
   - Integration tests: Verify dependent projects (Repository, WebApi) still build
   - Configuration tests: Validate user secrets loading in development

7. **Validation Checklist**
   - [ ] Dependencies resolve correctly (dotnet restore succeeds)
   - [ ] Project builds without errors
   - [ ] Project builds without warnings
   - [ ] InquirySpark.Common.Tests pass
   - [ ] No obsolete API warnings

---

### Project: InquirySpark.Repository

**Current State**
- Target Framework: net8.0
- Project Type: ClassLibrary (SDK-style)
- Dependencies: InquirySpark.Common
- Dependants: InquirySpark.WebApi, InquirySpark.Admin
- LOC: 6,703
- Files: 56
- Package Count: 6 (5 to update, 1 to remove)

**Target State**
- Target Framework: net10.0
- Updated Packages: 5
- Removed Packages: 1 (incompatible)

**Migration Steps**

1. **Prerequisites**
   - InquirySpark.Common must be upgraded first (Big Bang: happens simultaneously)

2. **Framework Update**
   Update `InquirySpark.Repository\InquirySpark.Repository.csproj`:
   ```xml
   <TargetFramework>net10.0</TargetFramework>
   ```

3. **Package Updates**

   | Package | Current Version | Target Version | Reason |
   |---------|----------------|----------------|---------|
   | Microsoft.EntityFrameworkCore | 8.0.3 | 10.0.0 | EF Core major version upgrade |
   | Microsoft.EntityFrameworkCore.Design | 8.0.3 | 10.0.0 | EF Core tooling compatibility |
   | Microsoft.EntityFrameworkCore.SqlServer | 8.0.3 | 10.0.0 | SQL Server provider upgrade |
   | Microsoft.EntityFrameworkCore.Tools | 8.0.3 | 10.0.0 | Migration tooling upgrade |
   | Microsoft.Extensions.Configuration.UserSecrets | 8.0.0 | 10.0.0 | Framework compatibility |
   | **Microsoft.VisualStudio.Azure.Containers.Tools.Targets** | 1.19.6 | **REMOVE** | No .NET 10.0 version available |

4. **Expected Breaking Changes**
   - **Entity Framework Core 10.0 Breaking Changes** (HIGH IMPACT):
     - Query behavior changes (split query defaults)
     - Timestamp/rowversion handling changes
     - Relationship navigation changes
     - SQL generation optimizations
   - **DbContext Configuration**:
     - Connection string handling changes
     - Service lifetime changes
     - Model building convention updates
   - **Migration Files**:
     - May need regeneration if using EF migrations
     - Review existing migration code for deprecated APIs

5. **Code Modifications**
   - **Remove Package Reference**:
     - Delete `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` from .csproj
     - This is development tooling only (Docker support in VS), safe to remove
   
   - **DbContext Updates**:
     - Review all DbContext classes for EF 10.0 changes
     - Check `OnModelCreating` configurations
     - Verify relationship configurations
     - Update query splitting configurations if used
   
   - **Repository Pattern**:
     - Review LINQ queries for behavior changes
     - Check IQueryable materializations
     - Verify Include/ThenInclude patterns
   
   - **Migrations**:
     - Verify existing migrations still apply
     - Test migration rollback functionality
     - Consider regenerating if issues arise

6. **Testing Strategy**
   - Unit tests: Verify repository methods work with EF 10.0
   - Integration tests: Test database operations with SQL Server
   - Migration tests: 
     - Apply migrations to test database
     - Verify data integrity after migrations
   - Performance: Check query performance hasn't regressed

7. **Validation Checklist**
   - [ ] Dependencies resolve correctly
   - [ ] Project builds without errors
   - [ ] Project builds without warnings
   - [ ] Incompatible package successfully removed
   - [ ] All DbContext classes compile
   - [ ] Migrations apply successfully
   - [ ] Repository unit tests pass
   - [ ] Database connections work
   - [ ] No EF Core deprecation warnings

---

### Project: InquirySpark.WebApi

**Current State**
- Target Framework: net8.0
- Project Type: AspNetCore (SDK-style)
- Dependencies: InquirySpark.Repository, InquirySpark.Common
- Dependants: None (application endpoint)
- LOC: 356
- Files: 9
- Package Count: 6 (1 to update, 1 to remove, 4 compatible)

**Target State**
- Target Framework: net10.0
- Updated Packages: 1
- Removed Packages: 1

**Migration Steps**

1. **Prerequisites**
   - InquirySpark.Common and InquirySpark.Repository upgraded (Big Bang: simultaneous)

2. **Framework Update**
   Update `InquirySpark.WebApi\InquirySpark.WebApi.csproj`:
   ```xml
   <TargetFramework>net10.0</TargetFramework>
   ```

3. **Package Updates**

   | Package | Current Version | Target Version | Reason |
   |---------|----------------|----------------|---------|
   | Microsoft.EntityFrameworkCore.Tools | 8.0.3 | 10.0.0 | EF tooling upgrade |
   | **Microsoft.VisualStudio.Azure.Containers.Tools.Targets** | 1.19.6 | **REMOVE** | No .NET 10.0 version available |
   | Swashbuckle.AspNetCore | 6.5.0 | *Compatible* | No update needed |
   | Swashbuckle.AspNetCore.Annotations | 6.5.0 | *Compatible* | No update needed |
   | Swashbuckle.AspNetCore.SwaggerUI | 6.5.0 | *Compatible* | No update needed |

4. **Expected Breaking Changes**
   - **ASP.NET Core 10.0 Minimal APIs**:
     - Routing behavior changes
     - Endpoint configuration updates
     - Middleware pipeline changes
   - **Program.cs / Startup.cs**:
     - Host builder changes
     - Service registration updates
     - Configuration patterns
   - **Swagger/OpenAPI**:
     - Swashbuckle 6.5.0 is compatible but verify OpenAPI spec generation
   - **EF Core Integration**:
     - DbContext registration in services
     - Connection string configuration

5. **Code Modifications**
   - **Remove Package Reference**:
     - Delete `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` from .csproj
   
   - **Program.cs Updates**:
     - Review minimal API setup
     - Check service registrations (AddDbContext, AddControllers, etc.)
     - Verify middleware pipeline order
   
   - **API Controllers**:
     - Review controller base classes
     - Check action method signatures
     - Verify model binding behavior
   
   - **Swagger Configuration**:
     - Test Swagger UI renders correctly
     - Verify OpenAPI document generation
     - Check endpoint documentation

6. **Testing Strategy**
   - API tests: Verify all endpoints respond correctly
   - Swagger tests: Ensure OpenAPI spec generates
   - Integration tests: Test with Repository layer
   - Manual validation: 
     - Start application
     - Access Swagger UI at /swagger
     - Test sample API calls

7. **Validation Checklist**
   - [ ] Dependencies resolve correctly
   - [ ] Project builds without errors
   - [ ] Project builds without warnings
   - [ ] Incompatible package successfully removed
   - [ ] Application starts without errors
   - [ ] Swagger UI accessible and functional
   - [ ] API endpoints respond correctly
   - [ ] Database connectivity works
   - [ ] No ASP.NET Core deprecation warnings

---

### Project: InquirySpark.Admin

**Current State**
- Target Framework: net8.0
- Project Type: AspNetCore (SDK-style)
- Dependencies: InquirySpark.Repository
- Dependants: None (application endpoint)
- LOC: 18,974 (LARGEST PROJECT)
- Files: 278
- Package Count: 8

**Target State**
- Target Framework: net10.0
- Updated Packages: 8 (including security fix)

**Migration Steps**

1. **Prerequisites**
   - InquirySpark.Repository upgraded (Big Bang: simultaneous)

2. **Framework Update**
   Update `InquirySpark.Admin\InquirySpark.Admin.csproj`:
   ```xml
   <TargetFramework>net10.0</TargetFramework>
   ```

3. **Package Updates**

   | Package | Current Version | Target Version | Reason |
   |---------|----------------|----------------|---------|
   | **Azure.Identity** | **1.10.4** | **1.17.1** | **SECURITY FIX (CRITICAL)** |
   | Microsoft.AspNetCore.Components.QuickGrid.EntityFrameworkAdapter | 8.0.3 | 10.0.0 | Blazor component upgrade |
   | Microsoft.AspNetCore.Identity.EntityFrameworkCore | 8.0.3 | 10.0.0 | Identity with EF upgrade |
   | Microsoft.AspNetCore.Identity.UI | 8.0.3 | 10.0.0 | Identity UI upgrade |
   | Microsoft.EntityFrameworkCore.Sqlite | 8.0.3 | 10.0.0 | SQLite provider upgrade |
   | Microsoft.EntityFrameworkCore.SqlServer | 8.0.3 | 10.0.0 | SQL Server provider upgrade |
   | Microsoft.EntityFrameworkCore.Tools | 8.0.3 | 10.0.0 | EF tooling upgrade |
   | Microsoft.VisualStudio.Web.CodeGeneration.Design | 8.0.2 | 10.0.0-rc.1.25458.5 | Code scaffolding upgrade |

4. **Expected Breaking Changes**
   - **CRITICAL - Azure.Identity 1.17.1**:
     - Authentication flow changes
     - Token acquisition behavior updates
     - Credential chain changes
     - **Action**: Review all Azure authentication code
   
   - **ASP.NET Core Identity 10.0**:
     - User manager API changes
     - Password hashing updates
     - Two-factor authentication changes
     - Cookie authentication changes
   
   - **Blazor Components 10.0**:
     - Component lifecycle changes
     - Rendering behavior updates
     - QuickGrid API changes
     - Event handling updates
   
   - **Entity Framework Core 10.0**:
     - Same as Repository project
     - Plus: Identity schema changes
     - SQLite provider behavior changes

5. **Code Modifications**
   - **Security Fix - Azure.Identity**:
     - Review all `DefaultAzureCredential` usages
     - Check `ClientSecretCredential` configurations
     - Verify authentication chains
     - Test Azure service connections
   
   - **Identity Configuration**:
     - Update `Program.cs` Identity setup
     - Review custom Identity pages (if any)
     - Check role and claims configuration
     - Verify authentication middleware
   
   - **Blazor Components**:
     - Review all `.razor` files (278 files - focus on those using EF)
     - Update QuickGrid implementations
     - Check component parameters
     - Verify event handlers (@onclick, etc.)
   
   - **EF Core**:
     - Review DbContext configurations
     - Check both SQLite and SQL Server usage
     - Verify migrations
     - Update seeding logic if present

6. **Testing Strategy**
   - Authentication tests: Verify Azure Identity integration
   - Identity tests:
     - User registration
     - Login/logout flows
     - Password reset
     - Two-factor authentication
   - Blazor tests:
     - Component rendering
     - QuickGrid functionality
     - Form submissions
     - Navigation
   - Integration tests: Full application workflows
   - Manual validation:
     - Start application
     - Test authentication flow
     - Verify UI components render
     - Test CRUD operations

7. **Validation Checklist**
   - [ ] Dependencies resolve correctly
   - [ ] Project builds without errors
   - [ ] Project builds without warnings
   - [ ] Azure.Identity security fix applied
   - [ ] Application starts without errors
   - [ ] Authentication works (login/logout)
   - [ ] Blazor components render correctly
   - [ ] QuickGrid displays data
   - [ ] Identity operations work (register, login, password reset)
   - [ ] Database operations successful
   - [ ] No security vulnerability warnings
   - [ ] No ASP.NET Core deprecation warnings

---

### Project: InquirySpark.Web

**Current State**
- Target Framework: net8.0
- Project Type: AspNetCore (SDK-style)
- Dependencies: None (standalone)
- Dependants: None (application endpoint)
- LOC: 3,813
- Files: 23
- Package Count: 8 (7 to update, 1 to remove)

**Target State**
- Target Framework: net10.0
- Updated Packages: 7 (including security fix)
- Removed Packages: 1

**Migration Steps**

1. **Prerequisites**
   - None (standalone project with no project dependencies)

2. **Framework Update**
   Update `InquirySpark.Web\InquirySpark.Web.csproj`:
   ```xml
   <TargetFramework>net10.0</TargetFramework>
   ```

3. **Package Updates**

   | Package | Current Version | Target Version | Reason |
   |---------|----------------|----------------|---------|
   | **Azure.Identity** | **1.10.4** | **1.17.1** | **SECURITY FIX (CRITICAL)** |
   | Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore | 8.0.3 | 10.0.0 | Diagnostics middleware upgrade |
   | Microsoft.AspNetCore.Identity.EntityFrameworkCore | 8.0.3 | 10.0.0 | Identity with EF upgrade |
   | Microsoft.AspNetCore.Identity.UI | 8.0.3 | 10.0.0 | Identity UI upgrade |
   | Microsoft.EntityFrameworkCore.SqlServer | 8.0.3 | 10.0.0 | SQL Server provider upgrade |
   | Microsoft.EntityFrameworkCore.Tools | 8.0.3 | 10.0.0 | EF tooling upgrade |
   | Newtonsoft.Json | 13.0.3 | 13.0.4 | Bug fixes and improvements |
   | **Microsoft.VisualStudio.Azure.Containers.Tools.Targets** | 1.19.6 | **REMOVE** | No .NET 10.0 version available |

4. **Expected Breaking Changes**
   - **CRITICAL - Azure.Identity 1.17.1**:
     - Same as Admin project
     - Review authentication code
   
   - **ASP.NET Core Identity 10.0**:
     - Same as Admin project
   
   - **EF Core Diagnostics 10.0**:
     - Database error page changes
     - Migration detection updates
   
   - **Newtonsoft.Json 13.0.4**:
     - Minor - typically backward compatible
     - Verify custom JSON serialization

5. **Code Modifications**
   - **Remove Package Reference**:
     - Delete `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` from .csproj
   
   - **Security Fix - Azure.Identity**:
     - Same review as Admin project
   
   - **Program.cs Updates**:
     - Review Identity configuration
     - Check EF diagnostics middleware
     - Verify authentication setup
   
   - **JSON Serialization**:
     - Review any custom JsonSerializerSettings
     - Check API response serialization
     - Verify date/time handling

6. **Testing Strategy**
   - Authentication tests: Same as Admin
   - Identity tests: Same as Admin
   - Diagnostics tests: Verify EF error pages display
   - JSON tests: Verify serialization/deserialization
   - Manual validation:
     - Start application
     - Test authentication
     - Test database operations
     - Trigger EF diagnostic pages (migration pending, etc.)

7. **Validation Checklist**
   - [ ] Dependencies resolve correctly
   - [ ] Project builds without errors
   - [ ] Project builds without warnings
   - [ ] Incompatible package successfully removed
   - [ ] Azure.Identity security fix applied
   - [ ] Application starts without errors
   - [ ] Authentication works
   - [ ] Identity operations work
   - [ ] Database operations successful
   - [ ] JSON serialization works correctly
   - [ ] No security vulnerability warnings
   - [ ] No ASP.NET Core deprecation warnings

---

### Project: InquirySpark.Common.Tests

**Current State**
- Target Framework: net8.0
- Project Type: DotNetCoreApp (SDK-style)
- Dependencies: InquirySpark.Common
- Dependants: None (test project)
- LOC: 495
- Files: 9
- Package Count: 4 (1 to update, 3 compatible)

**Target State**
- Target Framework: net10.0
- Updated Packages: 1

**Migration Steps**

1. **Prerequisites**
   - InquirySpark.Common upgraded (Big Bang: simultaneous)

2. **Framework Update**
   Update `InquirySpark.Common.Tests\InquirySpark.Common.Tests.csproj`:
   ```xml
   <TargetFramework>net10.0</TargetFramework>
   ```

3. **Package Updates**

   | Package | Current Version | Target Version | Reason |
   |---------|----------------|----------------|---------|
   | Microsoft.Extensions.Configuration.UserSecrets | 8.0.0 | 10.0.0 | Framework compatibility |
   | Microsoft.NET.Test.Sdk | 17.9.0 | *Compatible* | No update needed |
   | MSTest.TestAdapter | 3.2.2 | *Compatible* | No update needed |
   | MSTest.TestFramework | 3.2.2 | *Compatible* | No update needed |

4. **Expected Breaking Changes**
   - **Low Risk**: Test projects typically minimal breaking changes
   - **MSTest Compatibility**: Version 3.2.2 supports .NET 10.0
   - **Test SDK Compatibility**: Version 17.9.0 supports .NET 10.0

5. **Code Modifications**
   - **Expected**: None or minimal
   - **Review Areas**:
     - Test initialization code
     - Configuration setup in tests
     - Mock/stub implementations

6. **Testing Strategy**
   - Run all unit tests
   - Verify test discovery
   - Check test execution time
   - Ensure code coverage collection works

7. **Validation Checklist**
   - [ ] Dependencies resolve correctly
   - [ ] Project builds without errors
   - [ ] Project builds without warnings
   - [ ] All tests discovered by test runner
   - [ ] All unit tests pass
   - [ ] Test execution time acceptable
   - [ ] Code coverage collection works

---

## Package Update Reference

### Common Package Updates (affecting multiple projects)

| Package | Current | Target | Projects Affected | Update Reason |
|---------|---------|--------|-------------------|---------------|
| Microsoft.EntityFrameworkCore.Tools | 8.0.3 | 10.0.0 | 4 projects (Admin, Repository, Web, WebApi) | EF Core tooling compatibility with .NET 10.0 |
| Microsoft.EntityFrameworkCore.SqlServer | 8.0.3 | 10.0.0 | 3 projects (Admin, Repository, Web) | SQL Server provider upgrade for EF Core 10.0 |
| Microsoft.Extensions.Configuration.UserSecrets | 8.0.0 | 10.0.0 | 3 projects (Common, Repository, Common.Tests) | Framework compatibility |
| Microsoft.AspNetCore.Identity.EntityFrameworkCore | 8.0.3 | 10.0.0 | 2 projects (Admin, Web) | Identity with EF Core 10.0 integration |
| Microsoft.AspNetCore.Identity.UI | 8.0.3 | 10.0.0 | 2 projects (Admin, Web) | Identity UI for ASP.NET Core 10.0 |

### Security-Critical Updates

| Package | Current | Target | Projects Affected | Update Reason |
|---------|---------|--------|-------------------|---------------|
| **Azure.Identity** | **1.10.4** | **1.17.1** | 2 projects (Admin, Web) | **MODERATE security vulnerability fix** |

### Category-Specific Updates

**EF Core Packages**:
- Microsoft.EntityFrameworkCore: 8.0.3 → 10.0.0 (Repository)
- Microsoft.EntityFrameworkCore.Design: 8.0.3 → 10.0.0 (Repository)
- Microsoft.EntityFrameworkCore.Sqlite: 8.0.3 → 10.0.0 (Admin)

**ASP.NET Core Packages**:
- Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore: 8.0.3 → 10.0.0 (Web)
- Microsoft.AspNetCore.Components.QuickGrid.EntityFrameworkAdapter: 8.0.3 → 10.0.0 (Admin)

**Development Tools**:
- Microsoft.VisualStudio.Web.CodeGeneration.Design: 8.0.2 → 10.0.0-rc.1.25458.5 (Admin)

**Utility Packages**:
- Newtonsoft.Json: 13.0.3 → 13.0.4 (Web)

### Packages to Remove

| Package | Current | Projects Affected | Removal Reason |
|---------|---------|-------------------|----------------|
| Microsoft.VisualStudio.Azure.Containers.Tools.Targets | 1.19.6 | 3 projects (Repository, Web, WebApi) | No .NET 10.0 compatible version; development-time tooling only |

### Compatible Packages (No Update Required)

| Package | Version | Projects | Notes |
|---------|---------|----------|-------|
| Swashbuckle.AspNetCore | 6.5.0 | WebApi | Compatible with .NET 10.0 |
| Swashbuckle.AspNetCore.Annotations | 6.5.0 | WebApi | Compatible with .NET 10.0 |
| Swashbuckle.AspNetCore.SwaggerUI | 6.5.0 | WebApi | Compatible with .NET 10.0 |
| Microsoft.NET.Test.Sdk | 17.9.0 | Common.Tests | Compatible with .NET 10.0 |
| MSTest.TestAdapter | 3.2.2 | Common.Tests | Compatible with .NET 10.0 |
| MSTest.TestFramework | 3.2.2 | Common.Tests | Compatible with .NET 10.0 |

---

## Breaking Changes Catalog

### .NET 10.0 Framework Breaking Changes

#### ASP.NET Core 10.0
1. **Minimal API Changes**
   - Endpoint routing optimizations
   - Parameter binding behavior changes
   - Middleware pipeline execution order updates

2. **Authentication & Authorization**
   - Cookie authentication defaults updated
   - JWT bearer token validation changes
   - Authorization policy evaluation changes

3. **Blazor**
   - Component rendering lifecycle updates
   - Parameter passing optimizations
   - Event handling changes
   - Enhanced navigation features

4. **Identity**
   - User manager API surface changes
   - Password hashing algorithm updates
   - Two-factor authentication improvements
   - Email/SMS confirmation flow changes

#### Entity Framework Core 10.0
1. **Query Behavior**
   - Split query defaults changed
   - Collection navigation loading updates
   - Query filter application changes
   - SQL generation optimizations

2. **Model Building**
   - Convention changes for relationships
   - Required/optional navigation conventions
   - Cascade delete behavior updates
   - Index creation defaults

3. **Migrations**
   - Migration generation improvements
   - Timestamp/rowversion handling changes
   - Schema comparison updates

4. **Database Providers**
   - SQL Server: Connection resilience updates
   - SQLite: Foreign key constraint handling
   - Provider-specific optimizations

### Package-Specific Breaking Changes

#### Azure.Identity 1.17.1
**CRITICAL - Security and Authentication Changes**

1. **DefaultAzureCredential Chain**
   - Order of credential attempts updated
   - Timeout behavior changes
   - Error handling improvements

2. **Token Caching**
   - Cache invalidation logic updated
   - Token refresh timing changes

3. **Managed Identity**
   - Endpoint detection improvements
   - Retry logic updates

**Action Items**:
- Review all `DefaultAzureCredential` instantiations
- Test authentication flows in all environments (dev, staging, prod)
- Verify Azure service connections (Key Vault, Storage, etc.)
- Update any custom credential implementations

#### Microsoft.AspNetCore.Components.QuickGrid.EntityFrameworkAdapter 10.0
1. **QuickGrid API**
   - Column definition syntax updates
   - Sorting and filtering behavior changes
   - Pagination API improvements

**Action Items**:
- Review all QuickGrid component usages
- Test sorting and filtering functionality
- Verify pagination works correctly

#### Newtonsoft.Json 13.0.4
**Low Risk - Patch Release**

1. **Minor Bug Fixes**
   - Serialization edge case improvements
   - Date/time handling refinements

**Action Items**:
- Verify custom JsonSerializerSettings still work
- Test any complex serialization scenarios

### Common Migration Patterns

#### Pattern 1: DbContext Configuration
**Before (EF Core 8)**:
```csharp
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
```

**After (EF Core 10 - may require updates)**:
```csharp
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString)
           .ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning)));
```

#### Pattern 2: Azure Identity Setup
**Before (1.10.4)**:
```csharp
var credential = new DefaultAzureCredential();
```

**After (1.17.1 - verify behavior)**:
```csharp
var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
{
    ExcludeManagedIdentityCredential = false,
    Retry = { MaxRetries = 3 }
});
```

#### Pattern 3: Identity Configuration
**Before (ASP.NET Core 8)**:
```csharp
builder.Services.AddDefaultIdentity<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
```

**After (ASP.NET Core 10 - verify API surface)**:
```csharp
// Same API but internal behavior updated
builder.Services.AddDefaultIdentity<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
```

---

## Implementation Timeline

### Phase 0: Preparation
**Duration**: 15-30 minutes  
**Effort**: Low

**Tasks**:
- Remove incompatible package from 3 projects:
  - InquirySpark.Repository
  - InquirySpark.Web
  - InquirySpark.WebApi
- Verify all project files are backed up
- Ensure Git branch is up to date

**Deliverables**:
- 3 project files updated (package reference removed)
- Commit: "Remove incompatible Azure Container Tools package for .NET 10.0 upgrade"

---

### Phase 1: Atomic Upgrade
**Duration**: 2-4 hours  
**Effort**: High  
**Risk**: Medium

**Operations** (performed as single coordinated batch):

**Step 1: Update All Project Target Frameworks** (15 minutes)
- InquirySpark.Common: net8.0 → net10.0
- InquirySpark.Repository: net8.0 → net10.0
- InquirySpark.WebApi: net8.0 → net10.0
- InquirySpark.Admin: net8.0 → net10.0
- InquirySpark.Web: net8.0 → net10.0
- InquirySpark.Common.Tests: net8.0 → net10.0

**Step 2: Update All Package References** (30 minutes)
- Update all Entity Framework packages (8.0.3 → 10.0.0)
- Update all ASP.NET Core packages (8.0.3 → 10.0.0)
- Update Azure.Identity (1.10.4 → 1.17.1) - SECURITY FIX
- Update Configuration packages (8.0.0 → 10.0.0)
- Update Newtonsoft.Json (13.0.3 → 13.0.4)
- Update code generation tools (8.0.2 → 10.0.0-rc.1.25458.5)

**Step 3: Restore Dependencies** (5-10 minutes)
```bash
dotnet restore InquirySpark.sln
```

**Step 4: Build Solution and Fix Compilation Errors** (1-2 hours)
```bash
dotnet build InquirySpark.sln
```

Expected error categories:
- EF Core API changes in Repository project
- ASP.NET Core Identity changes in Admin and Web
- Azure.Identity authentication changes
- Blazor component API updates in Admin

Fix process:
1. Start with foundation projects (Common, Repository)
2. Move to application projects (WebApi, Admin, Web)
3. Fix test project last (Common.Tests)
4. Reference breaking changes catalog for solutions
5. Rebuild after each set of fixes

**Step 5: Verify Build Success**
- Solution builds with 0 errors
- Review and address warnings (deprecation, obsolete APIs)

**Deliverables**: 
- All 6 projects targeting net10.0
- All packages updated to .NET 10.0 versions
- Security vulnerability fixed
- Solution builds successfully with 0 errors

**Commit Strategy**: 
Single comprehensive commit after successful build:
```
"Upgrade InquirySpark solution to .NET 10.0

- Updated all projects from net8.0 to net10.0
- Upgraded Entity Framework Core packages to 10.0.0
- Upgraded ASP.NET Core packages to 10.0.0
- Fixed Azure.Identity security vulnerability (1.10.4 → 1.17.1)
- Updated supporting packages for .NET 10.0 compatibility
- Resolved compilation errors from breaking changes
- Solution builds successfully"
```

---

### Phase 2: Test Validation
**Duration**: 1-2 hours  
**Effort**: Medium  
**Risk**: Medium

**Operations**:

**Step 1: Execute Unit Tests** (15 minutes)
```bash
dotnet test InquirySpark.Common.Tests\InquirySpark.Common.Tests.csproj
```

Expected outcome: All tests pass

If failures occur:
- Review test failures related to Common library changes
- Fix underlying Common library code
- Rerun tests
- Rebuild dependent projects if Common changed

**Step 2: Integration Testing** (30-45 minutes)
Manual validation of applications:

**InquirySpark.WebApi**:
- Start application
- Access Swagger UI at /swagger
- Test sample API endpoints
- Verify database connectivity
- Check error handling

**InquirySpark.Admin**:
- Start application
- Test authentication (login/logout)
- Verify Azure Identity integration
- Test Blazor components render
- Test QuickGrid functionality
- Verify database CRUD operations
- Test Identity features (registration, password reset)

**InquirySpark.Web**:
- Start application
- Test authentication
- Verify Azure Identity integration
- Test database operations
- Verify JSON serialization in API responses
- Test EF diagnostics pages

**Step 3: Security Validation** (15 minutes)
- Verify no security vulnerability warnings
- Test Azure authentication flows
- Verify credentials work in all environments

**Step 4: Performance Baseline** (15 minutes)
- Application startup time
- API response times
- Database query performance
- Compare to pre-upgrade baselines (if available)

**Deliverables**:
- All unit tests pass
- All applications start successfully
- Authentication works in all projects
- Database operations successful
- Security vulnerabilities resolved
- Performance acceptable

**Commit Strategy**:
If test fixes required:
```
"Fix test failures after .NET 10.0 upgrade

- Addressed [specific test failures]
- Updated [specific code] for .NET 10.0 compatibility
- All tests now passing"
```

---

## Source Control Strategy

### Branching Strategy

**Main Upgrade Branch**: `upgrade-to-NET10` (already created)
- Source: `main` branch
- Purpose: Contains all .NET 10.0 upgrade work
- Lifetime: Until merged back to `main`

**Feature Branch Pattern**: Not applicable for Big Bang approach
- All work happens on single upgrade branch
- No parallel feature branches during upgrade

**Integration Approach**:
- Single upgrade branch contains all changes
- Pull request to `main` after Phase 2 validation complete
- Squash commits during merge (optional, team preference)

### Commit Strategy

**Big Bang Commit Approach - Single Comprehensive Commit (Recommended)**:

Given the atomic nature of Big Bang strategy, prefer single commit after Phase 1 complete:

**Phase 0 Commit** (preparation):
```
"Remove incompatible Azure Container Tools package for .NET 10.0 upgrade

- Removed Microsoft.VisualStudio.Azure.Containers.Tools.Targets from:
  - InquirySpark.Repository
  - InquirySpark.Web  
  - InquirySpark.WebApi
- Package has no .NET 10.0 compatible version
- Development tooling only, safe to remove"
```

**Phase 1 Commit** (atomic upgrade - after successful build):
```
"Upgrade InquirySpark solution to .NET 10.0

Projects updated (6 total):
- InquirySpark.Common
- InquirySpark.Repository
- InquirySpark.Admin
- InquirySpark.Web
- InquirySpark.WebApi
- InquirySpark.Common.Tests

Framework changes:
- All projects: net8.0 → net10.0

Package updates:
- Entity Framework Core: 8.0.3 → 10.0.0 (multiple packages)
- ASP.NET Core: 8.0.3 → 10.0.0 (multiple packages)
- Azure.Identity: 1.10.4 → 1.17.1 (SECURITY FIX)
- Microsoft.Extensions.Configuration.UserSecrets: 8.0.0 → 10.0.0
- Newtonsoft.Json: 13.0.3 → 13.0.4
- Code generation tools: 8.0.2 → 10.0.0-rc.1.25458.5

Breaking changes addressed:
- EF Core 10.0 query behavior updates
- ASP.NET Core Identity API changes
- Azure Identity authentication flow updates
- Blazor component rendering changes

Build status: ✅ Successful (0 errors)
Security vulnerabilities: ✅ Resolved"
```

**Phase 2 Commit** (only if test fixes required):
```
"Fix test failures and validation issues after .NET 10.0 upgrade

- [Specific fixes made]
- All tests passing
- Applications validated"
```

**Alternative: Granular Commit Approach** (if team prefers detailed history):

Commit after each logical group:
1. "Update project files to net10.0"
2. "Update Entity Framework packages to 10.0.0"
3. "Update ASP.NET Core packages to 10.0.0"
4. "Fix Azure.Identity security vulnerability"
5. "Resolve compilation errors in Repository project"
6. "Resolve compilation errors in Admin project"
7. "Resolve compilation errors in Web project"
8. "Fix test failures"

**Recommended**: Single comprehensive commit aligns with Big Bang philosophy and creates clean history.

### Review and Merge Process

**Pull Request Requirements**:
- Title: "Upgrade InquirySpark to .NET 10.0"
- Description: Include executive summary from this plan
- Checklist in PR description:
  - [ ] All 6 projects upgraded to net10.0
  - [ ] All package updates applied
  - [ ] Azure.Identity security vulnerability fixed
  - [ ] Incompatible packages removed
  - [ ] Solution builds with 0 errors
  - [ ] All unit tests pass
  - [ ] Applications start successfully
  - [ ] Authentication tested and working
  - [ ] Database operations validated
  - [ ] No security vulnerability warnings

**Code Review Focus Areas**:
- Breaking changes properly addressed
- Security fix (Azure.Identity) validated
- EF Core changes reviewed
- Identity/authentication changes verified
- No deprecated API usage

**Merge Criteria**:
- All PR checklist items completed
- At least one code review approval
- CI/CD pipeline passes (if configured)
- No merge conflicts
- Documentation updated (if applicable)

**Integration Validation**:
After merge to `main`:
- Deploy to test/staging environment
- Run full regression test suite
- Performance monitoring
- Security scan validation

---

## Risk Management

### 5.1 High-Risk Changes

| Project | Risk | Impact | Mitigation |
|---------|------|--------|------------|
| **InquirySpark.Admin** | High | Authentication/Identity may break, largest codebase | Extensive testing of authentication flows; dedicated validation of Blazor components; test all Identity features before merge |
| **Azure.Identity Upgrade** | High | Authentication may fail in production environments | Test in all environments (dev, staging, prod); verify managed identity works; test fallback credentials; rollback plan ready |
| **EF Core 10.0 Upgrade** | Medium-High | Database operations may fail or behave differently | Comprehensive database testing; verify migrations work; test query performance; backup databases before deployment |
| **InquirySpark.Repository** | Medium | Data access layer issues affect all dependent projects | Unit test all repository methods; integration test with actual database; verify EF migrations apply correctly |
| **Incompatible Package Removal** | Low-Medium | Docker tooling unavailable in Visual Studio | Development-time only; verify builds work without it; document alternative Docker workflows |

### 5.2 Risk Mitigation Strategies

#### Critical Security Risk: Azure.Identity
**Risk**: Authentication failures in production after upgrade

**Mitigation**:
1. **Development Testing**:
   - Test with local credentials
   - Test with service principals
   - Verify Visual Studio authentication works

2. **Environment Testing**:
   - Test in development environment first
   - Deploy to staging with managed identity
   - Verify Azure service connections (Key Vault, Storage, etc.)
   - Monitor authentication logs

3. **Rollback Plan**:
   - Keep previous version in separate branch
   - Quick revert process documented
   - Database backup before deployment

#### High Complexity Risk: InquirySpark.Admin (19k LOC)
**Risk**: Large codebase with multiple technology changes (Blazor, EF, Identity)

**Mitigation**:
1. **Systematic Approach**:
   - Fix compilation errors layer by layer
   - Test each major component separately
   - Use compiler warnings as guide

2. **Component Isolation**:
   - Test Blazor components independently
   - Validate Identity features separately
   - Test database operations in isolation

3. **Incremental Validation**:
   - Build frequently during fixes
   - Test authentication early
   - Verify UI rendering before full testing

#### Data Risk: Entity Framework 10.0
**Risk**: Query behavior changes or data corruption

**Mitigation**:
1. **Database Safety**:
   - Backup all databases before upgrade
   - Test migrations on copy of production data
   - Verify rollback migrations work

2. **Query Validation**:
   - Review all LINQ queries for behavior changes
   - Performance test critical queries
   - Monitor query execution plans

3. **Testing Strategy**:
   - Test CRUD operations thoroughly
   - Verify relationship loading works
   - Test transaction handling

### 5.3 Contingency Plans

#### Scenario 1: Blocking Compilation Errors
**Trigger**: Cannot resolve compilation errors after framework upgrade

**Response**:
1. Isolate problematic project
2. Research breaking change documentation
3. Search GitHub issues for similar problems
4. Consider temporary workarounds
5. If blocked >4 hours, escalate for team input

**Rollback Criteria**:
- Cannot resolve errors within 8 hours
- Breaking changes require major refactoring
- Critical functionality cannot be restored

#### Scenario 2: Azure.Identity Authentication Failures
**Trigger**: Authentication fails in any environment after upgrade

**Response**:
1. Verify credential configuration
2. Test with explicit service principal credentials
3. Check Azure AD application configuration
4. Review authentication logs
5. Test with previous version credentials

**Rollback Criteria**:
- Cannot authenticate in production within 2 hours
- Security team identifies new vulnerability
- Managed identity broken across environments

#### Scenario 3: EF Core Performance Degradation
**Trigger**: Database queries significantly slower after upgrade

**Response**:
1. Enable EF Core query logging
2. Compare execution plans
3. Identify regressed queries
4. Apply query optimization techniques
5. Consider split query configuration

**Rollback Criteria**:
- Performance degrades >50% on critical queries
- Cannot optimize queries within acceptable timeframe
- Database load becomes unsustainable

#### Scenario 4: Test Failures
**Trigger**: Unit or integration tests fail after upgrade

**Response**:
1. Analyze failure patterns
2. Distinguish between test issues vs. code issues
3. Fix underlying code if behavior truly broken
4. Update tests if testing outdated behavior
5. Ensure no regressions introduced

**Rollback Criteria**:
- >25% of tests fail and cannot be fixed quickly
- Critical test failures indicate major regression
- Test fixes require significant refactoring

### 5.4 Rollback Plan

**Quick Rollback Process** (if needed within first few hours):

1. **Git Revert**:
   ```bash
   git checkout main
   git branch -D upgrade-to-NET10
   # Start over if needed
   ```

2. **Restore Previous State**:
   - All project files revert to net8.0
   - All packages restore to previous versions
   - Security vulnerability returns (temporary)

3. **Communication**:
   - Notify team of rollback
   - Document reason for rollback
   - Plan corrective action

**Partial Rollback** (if specific project problematic):

Not applicable for Big Bang - must rollback all or proceed with all.

**Post-Rollback Actions**:

1. **Analysis**:
   - Identify root cause of failure
   - Document blockers encountered
   - Research solutions

2. **Planning**:
   - Adjust migration plan if needed
   - Consider alternative approach (incremental?)
   - Schedule retry attempt

3. **Prevention**:
   - Add additional validation steps
   - Enhance testing strategy
   - Improve rollback procedures

### 5.5 Success Indicators

**Technical Success Metrics**:
- [ ] All 6 projects build without errors
- [ ] All 6 projects build without warnings
- [ ] Zero security vulnerabilities reported
- [ ] All unit tests pass (100% success rate)
- [ ] All applications start without errors
- [ ] Authentication works in all projects
- [ ] Database operations successful
- [ ] API endpoints respond correctly
- [ ] Blazor UI renders correctly

**Quality Metrics**:
- [ ] Code quality maintained (no new technical debt)
- [ ] Performance within 10% of baseline
- [ ] No new deprecation warnings introduced
- [ ] Test coverage maintained or improved

**Process Metrics**:
- [ ] Upgrade completed within estimated timeline
- [ ] Big Bang strategy successfully executed
- [ ] Single PR merge to main branch
- [ ] Documentation updated

---

## Success Criteria

### 10.1 Strategy-Specific Success Criteria

**Big Bang Strategy Validation**:
- [ ] All 6 projects upgraded in single atomic operation
- [ ] No partial upgrade state at any point
- [ ] Single comprehensive commit contains all framework changes
- [ ] Solution remains buildable throughout process
- [ ] Security vulnerability fixed across all affected projects simultaneously

### 10.2 Technical Success Criteria

**Framework Upgrade**:
- [ ] All 6 projects migrated to net10.0
- [ ] Target framework properly set in all .csproj files
- [ ] No projects remain on net8.0

**Package Updates**:
- [ ] All 16 packages updated to specified versions
- [ ] Azure.Identity upgraded to 1.17.1 in 2 projects (security fix)
- [ ] Entity Framework packages upgraded to 10.0.0 (7 packages)
- [ ] ASP.NET Core packages upgraded to 10.0.0 (6 packages)
- [ ] Microsoft.VisualStudio.Azure.Containers.Tools.Targets removed from 3 projects
- [ ] Zero security vulnerabilities in dependencies

**Build Quality**:
- [ ] Solution builds with 0 errors
- [ ] Solution builds with 0 warnings (or documented acceptable warnings)
- [ ] All projects restore dependencies successfully
- [ ] No package dependency conflicts

**Testing**:
- [ ] All automated tests pass (InquirySpark.Common.Tests)
- [ ] All applications start successfully
- [ ] No runtime errors during basic operations
- [ ] Performance within acceptable thresholds

**Security**:
- [ ] Azure.Identity security vulnerability resolved
- [ ] No new security vulnerabilities introduced
- [ ] Authentication works in all environments

### 10.3 Quality Criteria

**Code Quality**:
- [ ] Code quality maintained or improved
- [ ] No new technical debt introduced
- [ ] Breaking changes properly addressed
- [ ] No use of obsolete APIs
- [ ] No deprecated framework features

**Test Coverage**:
- [ ] Test coverage maintained or improved
- [ ] No tests disabled to pass upgrade
- [ ] New tests added for new behavior (if applicable)

**Documentation**:
- [ ] README updated with .NET 10.0 information
- [ ] Developer setup instructions updated
- [ ] Any new dependencies documented
- [ ] Breaking changes documented

### 10.4 Process Criteria

**Big Bang Strategy Principles**:
- [ ] Atomic upgrade executed as planned
- [ ] All projects updated simultaneously
- [ ] No incremental partial states
- [ ] Single comprehensive validation phase

**Source Control**:
- [ ] All changes on upgrade-to-NET10 branch
- [ ] Appropriate commit messages
- [ ] Single or minimal commits reflecting atomic nature
- [ ] Pull request created with comprehensive description

**Migration Documentation**:
- [ ] This plan followed and updated as needed
- [ ] Deviations from plan documented
- [ ] Issues encountered documented
- [ ] Lessons learned captured

---

## Post-Upgrade Actions

### Immediate (Within 24 hours)
1. **Deployment**:
   - Deploy to test/staging environment
   - Monitor for errors and performance issues
   - Validate all functionality

2. **Team Communication**:
   - Notify team of successful upgrade
   - Share any important changes developers need to know
   - Document any new patterns or practices

3. **Monitoring**:
   - Watch application logs for errors
   - Monitor performance metrics
   - Track authentication success rates

### Short Term (Within 1 week)
1. **Documentation Updates**:
   - Update developer onboarding docs
   - Update deployment documentation
   - Document any new .NET 10.0 features used

2. **CI/CD Updates**:
   - Update build pipelines for .NET 10.0
   - Update deployment scripts
   - Verify automated tests run correctly

3. **Dependency Maintenance**:
   - Schedule regular package update reviews
   - Monitor for .NET 10.0 package updates
   - Watch for security advisories

### Long Term (Within 1 month)
1. **Feature Exploitation**:
   - Identify .NET 10.0 features to leverage
   - Plan performance optimizations
   - Consider new language features (C# 12)

2. **Technical Debt**:
   - Review any workarounds implemented
   - Plan refactoring of temporary solutions
   - Modernize code to use new framework features

3. **Knowledge Sharing**:
   - Share lessons learned with team
   - Update team best practices
   - Consider blog post or internal tech talk

---

## Appendix

### A. Useful Commands

**Restore Dependencies**:
```bash
dotnet restore InquirySpark.sln
```

**Build Solution**:
```bash
dotnet build InquirySpark.sln --configuration Release
```

**Run Tests**:
```bash
dotnet test InquirySpark.Common.Tests\InquirySpark.Common.Tests.csproj
```

**Check for Package Updates**:
```bash
dotnet list package --outdated
```

**Check for Security Vulnerabilities**:
```bash
dotnet list package --vulnerable
```

**Clean Build Artifacts**:
```bash
dotnet clean InquirySpark.sln
```

### B. Key Resources

**Official Documentation**:
- [.NET 10.0 Release Notes](https://docs.microsoft.com/en-us/dotnet/core/whats-new/dotnet-10)
- [ASP.NET Core 10.0 Migration Guide](https://docs.microsoft.com/en-us/aspnet/core/migration/90-to-100)
- [EF Core 10.0 Breaking Changes](https://docs.microsoft.com/en-us/ef/core/what-is-new/ef-core-10.0/breaking-changes)
- [Azure.Identity 1.17.1 Release Notes](https://github.com/Azure/azure-sdk-for-net/releases/tag/Azure.Identity_1.17.1)

**Migration Guides**:
- [Migrating from .NET 8 to .NET 10](https://docs.microsoft.com/en-us/dotnet/core/migration/8-to-10)
- [Blazor Migration Guide](https://docs.microsoft.com/en-us/aspnet/core/blazor/migration)

**Community Resources**:
- [.NET Blog](https://devblogs.microsoft.com/dotnet/)
- [ASP.NET Core GitHub Issues](https://github.com/dotnet/aspnetcore/issues)
- [EF Core GitHub Issues](https://github.com/dotnet/efcore/issues)

### C. Contact and Escalation

**For Technical Issues**:
- Review breaking changes documentation
- Search GitHub issues for similar problems
- Post question on Stack Overflow with `[.net-10]` tag
- Open issue on relevant GitHub repository

**For Security Concerns**:
- Review Azure.Identity security advisory
- Contact Microsoft Security Response Center if needed
- Follow responsible disclosure practices

### D. Glossary

- **Big Bang Migration**: Upgrading all projects simultaneously in single operation
- **Breaking Change**: API or behavior change requiring code modifications
- **Target Framework**: The .NET version a project compiles against
- **Package Reference**: NuGet package dependency in project file
- **SDK-style Project**: Modern .csproj format (all InquirySpark projects)
- **Atomic Operation**: All-or-nothing upgrade approach
- **Security Vulnerability**: Known security issue in package dependency

---

## Plan Metadata

- **Plan Version**: 1.0
- **Created**: 2024 (Upgrade to .NET 10.0 GA)
- **Solution**: InquirySpark
- **Strategy**: Big Bang
- **Target Framework**: .NET 10.0
- **Estimated Total Effort**: 4-7 hours
- **Risk Level**: Medium
- **Projects**: 6
- **Total LOC**: ~31,994

---

**END OF PLAN**