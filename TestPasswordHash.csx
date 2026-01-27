#!/usr/bin/env dotnet-script
#r "nuget: BCrypt.Net-Next, 4.0.3"

using BCrypt.Net;

var hash = "$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyYIq7MRnH.m";

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

Console.WriteLine("Testing common passwords against the hash:");
Console.WriteLine($"Hash: {hash}\n");

foreach (var pwd in commonPasswords)
{
    var matches = BCrypt.Verify(pwd, hash);
    if (matches)
    {
        Console.WriteLine($"✓ MATCH FOUND: '{pwd}'");
    }
    else
    {
        Console.WriteLine($"✗ '{pwd}' - no match");
    }
}

// Also generate new hashes for the intended passwords
Console.WriteLine("\n\nGenerating correct hashes for intended passwords:");
Console.WriteLine("=================================================");

var intendedPasswords = new Dictionary<string, string>
{
    ["Admin123!"] = "admin",
    ["Manager123!"] = "manager",
    ["Cashier123!"] = "cashier"
};

foreach (var kvp in intendedPasswords)
{
    var newHash = BCrypt.HashPassword(kvp.Key, 12);
    Console.WriteLine($"\nUser: {kvp.Value}");
    Console.WriteLine($"Password: {kvp.Key}");
    Console.WriteLine($"Hash: {newHash}");
    Console.WriteLine($"Verify: {BCrypt.Verify(kvp.Key, newHash)}");
}
