Console.WriteLine("=== Password Hash Testing Tool ===\n");

var existingHash = "$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyYIq7MRnH.m";

var commonPasswords = new[]
{
    "Password123!",
    "Admin123!",
    "Manager123!",
    "Cashier123!",
    "password",
    "admin",
    "admin123",
    "Password1!",
    "Test123!",
    "demo",
    "Demo123!",
    "Welcome123!",
    "Passw0rd!",
    "P@ssw0rd",
    "123456",
    "password123"
};

Console.WriteLine("Testing existing hash from seed data:");
Console.WriteLine($"Hash: {existingHash}\n");

string? matchedPassword = null;
foreach (var pwd in commonPasswords)
{
    var matches = BCrypt.Net.BCrypt.Verify(pwd, existingHash);
    if (matches)
    {
        Console.WriteLine($"✓✓✓ MATCH FOUND: '{pwd}' ✓✓✓\n");
        matchedPassword = pwd;
        break;
    }
    else
    {
        Console.WriteLine($"✗ '{pwd}' - no match");
    }
}

if (matchedPassword == null)
{
    Console.WriteLine("\n❌ No match found for existing hash!\n");
}

// Generate new hashes for the intended passwords
Console.WriteLine("\n\n=== Generating CORRECT hashes for seed data ===\n");

var users = new[]
{
    new { Name = "admin", Password = "Admin123!" },
    new { Name = "manager", Password = "Manager123!" },
    new { Name = "cashier", Password = "Cashier123!" }
};

foreach (var user in users)
{
    var newHash = BCrypt.Net.BCrypt.HashPassword(user.Password, 12);
    Console.WriteLine($"User: {user.Name}");
    Console.WriteLine($"Password: {user.Password}");
    Console.WriteLine($"Hash: {newHash}");
    Console.WriteLine($"Verify: {BCrypt.Net.BCrypt.Verify(user.Password, newHash)}");
    Console.WriteLine();
}

Console.WriteLine("\n✅ Copy the hashes above and update DataSeeder.cs");
