using Corelio.Domain.Entities;
using Corelio.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Corelio.Infrastructure.Persistence.Seeds;

/// <summary>
/// Seeds initial data for tenants, roles, permissions, and test users.
/// </summary>
public static class DataSeeder
{
    // Fixed dates for seed data to ensure deterministic migrations
    private static readonly DateTime SeedDate = new(2026, 01, 21, 0, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime TrialEndDate = new(2026, 02, 20, 0, 0, 0, DateTimeKind.Utc); // 30 days after SeedDate

    /// <summary>
    /// Seeds all initial data in the correct order.
    /// </summary>
    public static void Seed(ModelBuilder modelBuilder)
    {
        SeedPermissions(modelBuilder);
        SeedTenants(modelBuilder);
        SeedRoles(modelBuilder);
        SeedRolePermissions(modelBuilder);
        SeedUsers(modelBuilder);
        SeedUserRoles(modelBuilder);
    }

    private static void SeedPermissions(ModelBuilder modelBuilder)
    {
        var permissions = new List<Permission>
        {
            // Users Module
            new() { Id = Guid.Parse("a1111111-1111-1111-1111-111111111111"), Code = "users.view", Name = "View Users", Module = PermissionModule.Users, Description = "View user list", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("a1111111-1111-1111-1111-111111111112"), Code = "users.create", Name = "Create Users", Module = PermissionModule.Users, Description = "Create new users", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("a1111111-1111-1111-1111-111111111113"), Code = "users.update", Name = "Update Users", Module = PermissionModule.Users, Description = "Edit user details", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("a1111111-1111-1111-1111-111111111114"), Code = "users.delete", Name = "Delete Users", Module = PermissionModule.Users, Description = "Delete users", IsDangerous = true, CreatedAt = SeedDate },

            // Products Module
            new() { Id = Guid.Parse("a2222222-2222-2222-2222-222222222221"), Code = "products.view", Name = "View Products", Module = PermissionModule.Products, Description = "View product list", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("a2222222-2222-2222-2222-222222222222"), Code = "products.create", Name = "Create Products", Module = PermissionModule.Products, Description = "Create new products", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("a2222222-2222-2222-2222-222222222223"), Code = "products.update", Name = "Update Products", Module = PermissionModule.Products, Description = "Edit product details", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("a2222222-2222-2222-2222-222222222224"), Code = "products.delete", Name = "Delete Products", Module = PermissionModule.Products, Description = "Delete products", CreatedAt = SeedDate },

            // Sales Module
            new() { Id = Guid.Parse("a3333333-3333-3333-3333-333333333331"), Code = "sales.view", Name = "View Sales", Module = PermissionModule.Sales, Description = "View sales transactions", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("a3333333-3333-3333-3333-333333333332"), Code = "sales.create", Name = "Create Sales", Module = PermissionModule.Sales, Description = "Create new sales", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("a3333333-3333-3333-3333-333333333333"), Code = "sales.cancel", Name = "Cancel Sales", Module = PermissionModule.Sales, Description = "Cancel sales transactions", IsDangerous = true, CreatedAt = SeedDate },

            // Inventory Module
            new() { Id = Guid.Parse("a4444444-4444-4444-4444-444444444441"), Code = "inventory.view", Name = "View Inventory", Module = PermissionModule.Inventory, Description = "View inventory levels", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("a4444444-4444-4444-4444-444444444442"), Code = "inventory.adjust", Name = "Adjust Inventory", Module = PermissionModule.Inventory, Description = "Adjust inventory levels", CreatedAt = SeedDate },

            // Reports Module
            new() { Id = Guid.Parse("a5555555-5555-5555-5555-555555555551"), Code = "reports.view", Name = "View Reports", Module = PermissionModule.Reports, Description = "View business reports", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("a5555555-5555-5555-5555-555555555552"), Code = "reports.export", Name = "Export Reports", Module = PermissionModule.Reports, Description = "Export reports to file", CreatedAt = SeedDate },

            // Settings Module
            new() { Id = Guid.Parse("a6666666-6666-6666-6666-666666666661"), Code = "settings.view", Name = "View Settings", Module = PermissionModule.Settings, Description = "View system settings", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("a6666666-6666-6666-6666-666666666662"), Code = "settings.update", Name = "Update Settings", Module = PermissionModule.Settings, Description = "Modify system settings", IsDangerous = true, CreatedAt = SeedDate },

            // Pricing Module
            new() { Id = Guid.Parse("a7777777-7777-7777-7777-777777777771"), Code = "pricing.view", Name = "View Pricing", Module = PermissionModule.Pricing, Description = "View pricing configuration and product prices", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("a7777777-7777-7777-7777-777777777772"), Code = "pricing.manage", Name = "Manage Pricing", Module = PermissionModule.Pricing, Description = "Configure pricing tiers and update product prices", CreatedAt = SeedDate },

            // Customers Module
            new() { Id = Guid.Parse("a8888888-8888-8888-8888-888888888881"), Code = "customers.view", Name = "View Customers", Module = PermissionModule.Customers, Description = "View customer list and details", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("a8888888-8888-8888-8888-888888888882"), Code = "customers.create", Name = "Create Customers", Module = PermissionModule.Customers, Description = "Create new customers", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("a8888888-8888-8888-8888-888888888883"), Code = "customers.update", Name = "Update Customers", Module = PermissionModule.Customers, Description = "Edit customer details", CreatedAt = SeedDate },
            new() { Id = Guid.Parse("a8888888-8888-8888-8888-888888888884"), Code = "customers.delete", Name = "Delete Customers", Module = PermissionModule.Customers, Description = "Soft-delete customers", IsDangerous = true, CreatedAt = SeedDate },
        };

        modelBuilder.Entity<Permission>().HasData(permissions);
    }

    private static void SeedTenants(ModelBuilder modelBuilder)
    {
        var defaultTenant = new Tenant
        {
            Id = Guid.Parse("b0000000-0000-0000-0000-000000000001"),
            Name = "Demo Hardware Store",
            LegalName = "Ferretería Demo S.A. de C.V.",
            Rfc = "FDE010101ABC",
            Subdomain = "demo",
            SubscriptionPlan = Domain.Enums.SubscriptionPlan.Premium,
            SubscriptionStartsAt = SeedDate,
            MaxUsers = 10,
            MaxProducts = 5000,
            MaxSalesPerMonth = 10000,
            IsActive = true,
            IsTrial = true,
            TrialEndsAt = TrialEndDate,
            CreatedAt = SeedDate,
            UpdatedAt = SeedDate
        };

        modelBuilder.Entity<Tenant>().HasData(defaultTenant);

        var tenantConfig = new TenantConfiguration
        {
            Id = Guid.Parse("c0000000-0000-0000-0000-000000000001"),
            TenantId = defaultTenant.Id,
            CfdiPacTestMode = true,
            CfdiSeries = "A",
            CfdiNextFolio = 1,
            DefaultTaxRate = 0.1600m,
            Currency = "MXN",
            Timezone = "America/Mexico_City",
            BusinessHoursStart = new TimeOnly(9, 0),
            BusinessHoursEnd = new TimeOnly(18, 0),
            PosAutoPrintReceipt = false,
            PosRequireCustomer = false,
            PosDefaultPaymentMethod = Domain.Enums.PaymentMethod.Cash,
            PosEnableBarcodeScanner = true,
            AllowNegativeInventory = false,
            RequireProductCost = true,
            AutoCalculateMargin = true,
            EmailNotificationsEnabled = true,
            SmsNotificationsEnabled = false,
            LowStockNotificationThreshold = 20.00m,
            CreatedAt = SeedDate,
            UpdatedAt = SeedDate
        };

        modelBuilder.Entity<TenantConfiguration>().HasData(tenantConfig);
    }

    private static void SeedRoles(ModelBuilder modelBuilder)
    {
        var roles = new List<Role>
        {
            new()
            {
                Id = Guid.Parse("d1111111-1111-1111-1111-111111111111"),
                TenantId = Guid.Parse("b0000000-0000-0000-0000-000000000001"),
                Name = "Administrator",
                Description = "Full system access",
                IsSystemRole = true,
                IsDefault = false,
                CreatedAt = SeedDate,
                UpdatedAt = SeedDate
            },
            new()
            {
                Id = Guid.Parse("d2222222-2222-2222-2222-222222222222"),
                TenantId = Guid.Parse("b0000000-0000-0000-0000-000000000001"),
                Name = "Manager",
                Description = "Store manager with most permissions",
                IsSystemRole = false,
                IsDefault = false,
                CreatedAt = SeedDate,
                UpdatedAt = SeedDate
            },
            new()
            {
                Id = Guid.Parse("d3333333-3333-3333-3333-333333333333"),
                TenantId = Guid.Parse("b0000000-0000-0000-0000-000000000001"),
                Name = "Cashier",
                Description = "Point of sale operator",
                IsSystemRole = false,
                IsDefault = true,
                CreatedAt = SeedDate,
                UpdatedAt = SeedDate
            }
        };

        modelBuilder.Entity<Role>().HasData(roles);
    }

    private static void SeedRolePermissions(ModelBuilder modelBuilder)
    {
        var adminRoleId = Guid.Parse("d1111111-1111-1111-1111-111111111111");
        var managerRoleId = Guid.Parse("d2222222-2222-2222-2222-222222222222");
        var cashierRoleId = Guid.Parse("d3333333-3333-3333-3333-333333333333");

        var rolePermissions = new List<RolePermission>();

        // Administrator gets all permissions
        var allPermissionIds = new[]
        {
            Guid.Parse("a1111111-1111-1111-1111-111111111111"),
            Guid.Parse("a1111111-1111-1111-1111-111111111112"),
            Guid.Parse("a1111111-1111-1111-1111-111111111113"),
            Guid.Parse("a1111111-1111-1111-1111-111111111114"),
            Guid.Parse("a2222222-2222-2222-2222-222222222221"),
            Guid.Parse("a2222222-2222-2222-2222-222222222222"),
            Guid.Parse("a2222222-2222-2222-2222-222222222223"),
            Guid.Parse("a2222222-2222-2222-2222-222222222224"),
            Guid.Parse("a3333333-3333-3333-3333-333333333331"),
            Guid.Parse("a3333333-3333-3333-3333-333333333332"),
            Guid.Parse("a3333333-3333-3333-3333-333333333333"),
            Guid.Parse("a4444444-4444-4444-4444-444444444441"),
            Guid.Parse("a4444444-4444-4444-4444-444444444442"),
            Guid.Parse("a5555555-5555-5555-5555-555555555551"),
            Guid.Parse("a5555555-5555-5555-5555-555555555552"),
            Guid.Parse("a6666666-6666-6666-6666-666666666661"),
            Guid.Parse("a6666666-6666-6666-6666-666666666662"),
            Guid.Parse("a7777777-7777-7777-7777-777777777771"), // pricing.view
            Guid.Parse("a7777777-7777-7777-7777-777777777772"), // pricing.manage
            Guid.Parse("a8888888-8888-8888-8888-888888888881"), // customers.view
            Guid.Parse("a8888888-8888-8888-8888-888888888882"), // customers.create
            Guid.Parse("a8888888-8888-8888-8888-888888888883"), // customers.update
            Guid.Parse("a8888888-8888-8888-8888-888888888884"), // customers.delete
        };

        foreach (var permissionId in allPermissionIds)
        {
            rolePermissions.Add(new RolePermission
            {
                RoleId = adminRoleId,
                PermissionId = permissionId,
                AssignedAt = SeedDate
            });
        }

        // Manager gets most permissions except dangerous ones
        var managerPermissionIds = new[]
        {
            Guid.Parse("a1111111-1111-1111-1111-111111111111"), // users.view
            Guid.Parse("a1111111-1111-1111-1111-111111111112"), // users.create
            Guid.Parse("a1111111-1111-1111-1111-111111111113"), // users.update
            Guid.Parse("a2222222-2222-2222-2222-222222222221"), // products.view
            Guid.Parse("a2222222-2222-2222-2222-222222222222"), // products.create
            Guid.Parse("a2222222-2222-2222-2222-222222222223"), // products.update
            Guid.Parse("a2222222-2222-2222-2222-222222222224"), // products.delete
            Guid.Parse("a3333333-3333-3333-3333-333333333331"), // sales.view
            Guid.Parse("a3333333-3333-3333-3333-333333333332"), // sales.create
            Guid.Parse("a4444444-4444-4444-4444-444444444441"), // inventory.view
            Guid.Parse("a4444444-4444-4444-4444-444444444442"), // inventory.adjust
            Guid.Parse("a5555555-5555-5555-5555-555555555551"), // reports.view
            Guid.Parse("a5555555-5555-5555-5555-555555555552"), // reports.export
            Guid.Parse("a6666666-6666-6666-6666-666666666661"), // settings.view
            Guid.Parse("a7777777-7777-7777-7777-777777777771"), // pricing.view
            Guid.Parse("a7777777-7777-7777-7777-777777777772"), // pricing.manage
            Guid.Parse("a8888888-8888-8888-8888-888888888881"), // customers.view
            Guid.Parse("a8888888-8888-8888-8888-888888888882"), // customers.create
            Guid.Parse("a8888888-8888-8888-8888-888888888883"), // customers.update
        };

        foreach (var permissionId in managerPermissionIds)
        {
            rolePermissions.Add(new RolePermission
            {
                RoleId = managerRoleId,
                PermissionId = permissionId,
                AssignedAt = SeedDate
            });
        }

        // Cashier gets basic sales permissions
        var cashierPermissionIds = new[]
        {
            Guid.Parse("a2222222-2222-2222-2222-222222222221"), // products.view
            Guid.Parse("a3333333-3333-3333-3333-333333333331"), // sales.view
            Guid.Parse("a3333333-3333-3333-3333-333333333332"), // sales.create
            Guid.Parse("a4444444-4444-4444-4444-444444444441"), // inventory.view
            Guid.Parse("a8888888-8888-8888-8888-888888888881"), // customers.view
        };

        foreach (var permissionId in cashierPermissionIds)
        {
            rolePermissions.Add(new RolePermission
            {
                RoleId = cashierRoleId,
                PermissionId = permissionId,
                AssignedAt = SeedDate
            });
        }

        modelBuilder.Entity<RolePermission>().HasData(rolePermissions);
    }

    private static void SeedUsers(ModelBuilder modelBuilder)
    {
        var users = new List<User>
        {
            new()
            {
                Id = Guid.Parse("e1111111-1111-1111-1111-111111111111"),
                TenantId = Guid.Parse("b0000000-0000-0000-0000-000000000001"),
                Email = "admin@demo.corelio.app",
                Username = "admin",
                FirstName = "Admin",
                LastName = "User",
                // Password: Admin123! (BCrypt hashed with work factor 12)
                PasswordHash = "$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyYIq7MRnH.m",
                IsActive = true,
                IsEmailConfirmed = true,
                FailedLoginAttempts = 0,
                CreatedAt = SeedDate,
                UpdatedAt = SeedDate
            },
            new()
            {
                Id = Guid.Parse("e2222222-2222-2222-2222-222222222222"),
                TenantId = Guid.Parse("b0000000-0000-0000-0000-000000000001"),
                Email = "manager@demo.corelio.app",
                Username = "manager",
                FirstName = "Manager",
                LastName = "User",
                // Password: Manager123! (BCrypt hashed with work factor 12)
                PasswordHash = "$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyYIq7MRnH.m",
                IsActive = true,
                IsEmailConfirmed = true,
                FailedLoginAttempts = 0,
                CreatedAt = SeedDate,
                UpdatedAt = SeedDate
            },
            new()
            {
                Id = Guid.Parse("e3333333-3333-3333-3333-333333333333"),
                TenantId = Guid.Parse("b0000000-0000-0000-0000-000000000001"),
                Email = "cashier@demo.corelio.app",
                Username = "cashier",
                FirstName = "Cashier",
                LastName = "User",
                // Password: Cashier123! (BCrypt hashed with work factor 12)
                PasswordHash = "$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyYIq7MRnH.m",
                IsActive = true,
                IsEmailConfirmed = true,
                FailedLoginAttempts = 0,
                CreatedAt = SeedDate,
                UpdatedAt = SeedDate
            }
        };

        modelBuilder.Entity<User>().HasData(users);
    }

    private static void SeedUserRoles(ModelBuilder modelBuilder)
    {
        var userRoles = new List<UserRole>
        {
            new()
            {
                UserId = Guid.Parse("e1111111-1111-1111-1111-111111111111"),
                RoleId = Guid.Parse("d1111111-1111-1111-1111-111111111111"),
                AssignedAt = SeedDate
            },
            new()
            {
                UserId = Guid.Parse("e2222222-2222-2222-2222-222222222222"),
                RoleId = Guid.Parse("d2222222-2222-2222-2222-222222222222"),
                AssignedAt = SeedDate
            },
            new()
            {
                UserId = Guid.Parse("e3333333-3333-3333-3333-333333333333"),
                RoleId = Guid.Parse("d3333333-3333-3333-3333-333333333333"),
                AssignedAt = SeedDate
            }
        };

        modelBuilder.Entity<UserRole>().HasData(userRoles);
    }
}
