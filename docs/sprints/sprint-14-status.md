# Sprint 14: Administration Module

**Goal:** Deliver a tenant Administration module so store owners can manage users (employees, roles, access), warehouses, and view role definitions — all from the Blazor UI without needing direct API access.

**Duration:** ~3-4 days estimated
**Status:** 🟢 Complete
**Started:** 2026-06-18
**Total Story Points:** 16 pts

> **Prerequisites:** Sprints 1-13 complete | User, Role, Permission, Warehouse entities exist | IUserRepository, IRoleRepository, IInventoryRepository already implemented for auth/inventory flows

---

## User Story 14.1: User Management
**As an administrator, I want to list, edit, assign roles to, and deactivate users in my tenant so I can control who has access and what they can do.**

**Status:** 🟢 Complete

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-14.1.1 | Extend `IUserRepository` with `GetPagedAsync`; implement in `UserRepository`; create `UserListDto`, `UserDto`, `GetUsersQuery`, `GetUserByIdQuery`, `UpdateUserCommand`, `AssignRolesCommand` + validator | feature/US-14.1-TASK-1-5-user-management | 🟢 | Application layer complete |
| TASK-14.1.2 | `UserEndpoints.cs` — `GET /api/v1/users`, `GET /api/v1/users/{id}`, `PUT /api/v1/users/{id}`, `PUT /api/v1/users/{id}/roles`, `PUT /api/v1/users/{id}/deactivate`, `PUT /api/v1/users/{id}/activate`; register in `EndpointExtensions` | feature/US-14.1-TASK-1-5-user-management | 🟢 | User creation uses existing POST /api/v1/auth/register |
| TASK-14.1.3 | `UserModels.cs`, `IUserHttpService`, `UserHttpService`; register in `Program.cs` | feature/US-14.1-TASK-1-5-user-management | 🟢 | |
| TASK-14.1.4 | `UserList.razor` at `/admin/usuarios`, `UserDetail.razor` at `/admin/usuarios/{Id:guid}`; inline create dialog; ADMINISTRACIÓN nav section (Usuarios + Almacenes + Roles links); 60+ es-MX localization keys for all 3 stories | feature/US-14.1-TASK-1-5-user-management | 🟢 | BlazorApp build: 0 errors |
| TASK-14.1.5 | Unit tests: `GetUsersQueryHandlerTests` (4), `GetUserByIdQueryHandlerTests` (2), `UpdateUserCommandHandlerTests` (4), `AssignRolesCommandHandlerTests` (5) | feature/US-14.1-TASK-1-5-user-management | 🟢 | 15 tests total |

**Acceptance Criteria:**
- [ ] Administrators can list all users in their tenant with search and status filter
- [ ] Each user row shows name, email, position, assigned roles (as chips), active status
- [ ] Clicking a user opens detail/edit page with full profile + role assignment
- [ ] Admin can assign/replace roles via multi-select (shows available system roles)
- [ ] Admin can deactivate or reactivate any user (except themselves)
- [ ] "New User" button opens dialog that calls `POST /api/v1/auth/register` to create user with roles
- [ ] All UI text in Spanish (es-MX) via `IStringLocalizer`
- [ ] `users.view` permission gates list/detail endpoints; `users.update` gates edit/role endpoints
- [ ] Unit tests passing for all handler scenarios

---

## User Story 14.2: Warehouse Management
**As an administrator, I want to create and manage warehouses so I can track inventory across multiple store locations.**

**Status:** 🟢 Complete

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-14.2.1 | Extend `IInventoryRepository` with warehouse CRUD methods; implement in `InventoryRepository`; `CreateWarehouseCommand`, `UpdateWarehouseCommand`, `DeleteWarehouseCommand` + handlers + validator; migration `AddWarehousePermissionsSeed` | feature/US-14.2-TASK-1-4-warehouse-management | 🟢 | Migration 20260618192240; GUIDs f1111111-...-111{1-4} |
| TASK-14.2.2 | `WarehouseEndpoints.cs` — `POST`, `PUT /{id}`, `DELETE /{id}` at `/api/v1/warehouses`; register in `EndpointExtensions` | feature/US-14.2-TASK-1-4-warehouse-management | 🟢 | GET already exists in InventoryEndpoints |
| TASK-14.2.3 | `WarehouseModels.cs`, `IWarehouseHttpService`, `WarehouseHttpService`; `WarehouseList.razor` at `/admin/almacenes`; `WarehouseForm.razor` at `/admin/almacenes/nuevo` + `/{Id:guid}/editar`; register in `Program.cs`; ~15 localization keys | feature/US-14.2-TASK-1-4-warehouse-management | 🟢 | |
| TASK-14.2.4 | Unit tests: `CreateWarehouseCommandHandlerTests` (3), `UpdateWarehouseCommandHandlerTests` (2), `DeleteWarehouseCommandHandlerTests` (3) | feature/US-14.2-TASK-1-4-warehouse-management | 🟢 | 8 tests — 279 Application Tests total |

**Acceptance Criteria:**
- [ ] Admin can list all warehouses with type, default badge, and inventory item count
- [ ] Admin can create new warehouses (Name + Type + IsDefault switch)
- [ ] Admin can edit warehouse name, type, and default status
- [ ] Setting a warehouse as default automatically unsets the previous default
- [ ] Cannot delete the default warehouse
- [ ] Cannot delete a warehouse that has inventory items
- [ ] `warehouses.view/create/update/delete` permissions seeded and assigned to Owner + Administrator roles
- [ ] All UI text in Spanish (es-MX)

---

## User Story 14.3: Roles Viewer
**As an administrator, I want to view the available roles and their permission sets so I know what access level to assign when creating or editing users.**

**Status:** 🟢 Complete

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-14.3.1 | Extend `IRoleRepository` with `GetAllWithPermissionsAsync`; implement in `RoleRepository`; `RoleDto`, `GetRolesQuery` + handler | feature/US-14.3-TASK-1-3-roles-viewer | 🟢 | RoleDto + PermissionSummaryDto; query includes permissions via eager load |
| TASK-14.3.2 | `RoleEndpoints.cs` — `GET /api/v1/roles` (requires `users.view`); register in `EndpointExtensions` | feature/US-14.3-TASK-1-3-roles-viewer | 🟢 | |
| TASK-14.3.3 | `RoleModels.cs`, `IRoleHttpService`, `RoleHttpService`; `RoleList.razor` at `/admin/roles`; Roles link already in nav from US-14.1.4; 1 new localization key | feature/US-14.3-TASK-1-3-roles-viewer | 🟢 | Expansion panels show permissions as chips grouped by module |

**Acceptance Criteria:**
- [ ] Admin can view all available roles (system roles visible to all tenants)
- [ ] Each role shows name, description, and all assigned permissions as chips
- [ ] System roles are clearly labelled with a badge
- [ ] Page is read-only (no edit/delete for system roles)
- [ ] `users.view` permission gates the endpoint
- [ ] All UI text in Spanish (es-MX)

---

## Sprint 14 Summary

| Story | Priority | SP | Status |
|-------|----------|----|--------|
| US-14.1: User Management | P0 Critical | 8 | 🟢 Complete |
| US-14.2: Warehouse Management | P1 High | 5 | 🟢 Complete |
| US-14.3: Roles Viewer | P2 Medium | 3 | 🟢 Complete |
| **Total** | | **16** | 🟢 Complete |

**Recommended execution order:** US-14.1 → US-14.2 → US-14.3

**Key dependencies:**
- TASK-14.1.1 (Application Layer) must complete before TASK-14.1.2 (endpoints)
- `users.view` permission (already seeded in DataSeeder) gates both User and Role endpoints
- ADMINISTRACIÓN nav section introduced in TASK-14.1.4 — Roles link added in TASK-14.3.3
