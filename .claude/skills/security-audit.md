---
name: security-audit
description: Checks for OWASP Top 10 vulnerabilities and security best practices
---

# Security Audit

## OWASP Top 10 (2021) Checklist

### 1. Broken Access Control
```csharp
// ❌ VULNERABLE - Missing authorization check
[HttpDelete("{id}")]
public async Task<ActionResult> DeleteProduct(Guid id)
{
    await _productService.DeleteAsync(id);
    return NoContent();
}

// ✅ SECURE - Authorization enforced
[HttpDelete("{id}")]
[Authorize(Policy = "products.delete")]
public async Task<ActionResult> DeleteProduct(Guid id)
{
    // Multi-tenant check happens automatically via query filters
    var result = await _mediator.Send(new DeleteProductCommand(id));
    return result.IsSuccess ? NoContent() : NotFound();
}
```

**Check:**
- [ ] All endpoints have `[Authorize]` attribute (or explicitly `[AllowAnonymous]`)
- [ ] Permission-based authorization for sensitive operations
- [ ] Multi-tenant isolation enforced (never accept tenant_id from client)
- [ ] IDOR prevention (check ownership before operations)

### 2. Cryptographic Failures
```csharp
// ❌ VULNERABLE - Storing plain text passwords
public class User
{
    public string Password { get; set; } // DON'T!
}

// ✅ SECURE - Hashed passwords
public class User
{
    public string PasswordHash { get; set; }
}

// Use BCrypt or PBKDF2
var passwordHash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
```

**Check:**
- [ ] Passwords hashed with BCrypt (cost factor 12+) or PBKDF2
- [ ] Sensitive data encrypted at rest (PII, financial data)
- [ ] HTTPS/TLS 1.3 enforced for all endpoints
- [ ] Secrets in Azure Key Vault (not in code or config files)
- [ ] CSD certificates stored securely (Key Vault only)

### 3. Injection
```csharp
// ❌ VULNERABLE - SQL Injection
public async Task<List<Product>> Search(string query)
{
    var sql = $"SELECT * FROM products WHERE name LIKE '%{query}%'";
    return await _context.Products.FromSqlRaw(sql).ToListAsync();
}

// ✅ SECURE - Parameterized query
public async Task<List<Product>> Search(string query)
{
    return await _context.Products
        .Where(p => p.Name.Contains(query))
        .ToListAsync();
}

// ✅ SECURE - If raw SQL is needed
public async Task<List<Product>> Search(string query)
{
    return await _context.Products
        .FromSqlRaw("SELECT * FROM products WHERE name LIKE {0}", $"%{query}%")
        .ToListAsync();
}
```

**Check:**
- [ ] No string concatenation in SQL queries
- [ ] Use LINQ or parameterized queries only
- [ ] User input validated and sanitized
- [ ] No command injection in bash/PowerShell calls
- [ ] LDAP/XPath queries parameterized

### 4. Insecure Design
```csharp
// ❌ VULNERABLE - No rate limiting on login
[HttpPost("login")]
public async Task<ActionResult<TokenResponse>> Login(LoginRequest request)
{
    // Brute force attack possible!
}

// ✅ SECURE - Rate limiting enforced
[HttpPost("login")]
[EnableRateLimiting("login")]
public async Task<ActionResult<TokenResponse>> Login(LoginRequest request)
{
    // Limited to 5 attempts per minute
}

// Rate limiter configuration
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("login", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1)
            }));
});
```

**Check:**
- [ ] Rate limiting on authentication endpoints
- [ ] Account lockout after failed login attempts
- [ ] CAPTCHA for public endpoints
- [ ] Security headers configured (CSP, HSTS, X-Frame-Options)
- [ ] Defense in depth (multiple security layers)

### 5. Security Misconfiguration
```csharp
// ❌ VULNERABLE - Detailed errors in production
app.UseDeveloperExceptionPage(); // DON'T in production!

// ✅ SECURE - Generic errors in production
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

// Return generic error message
[ApiExplorerSettings(IgnoreApi = true)]
[Route("/error")]
public IActionResult HandleError()
{
    return Problem(detail: "An error occurred", statusCode: 500);
}
```

**Check:**
- [ ] No detailed error messages in production
- [ ] Default credentials changed
- [ ] Unnecessary features/endpoints disabled
- [ ] Security headers configured
- [ ] CORS properly configured (not wildcard)
- [ ] Swagger/OpenAPI disabled in production (or secured)

### 6. Vulnerable and Outdated Components
```bash
# Check for vulnerabilities
dotnet list package --vulnerable
dotnet list package --outdated

# Update packages
dotnet add package [PackageName] --version [LatestVersion]
```

**Check:**
- [ ] All NuGet packages up to date
- [ ] No packages with known vulnerabilities
- [ ] Dependabot enabled for automated updates
- [ ] Regular security scans (Snyk, Trivy)
- [ ] .NET version is latest LTS

### 7. Identification and Authentication Failures
```csharp
// ❌ VULNERABLE - Weak password policy
public class PasswordValidator
{
    public bool IsValid(string password) => password.Length >= 6; // Too weak!
}

// ✅ SECURE - Strong password policy
public class PasswordValidator : AbstractValidator<string>
{
    public PasswordValidator()
    {
        RuleFor(password => password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(12).WithMessage("Password must be at least 12 characters")
            .Matches(@"[A-Z]").WithMessage("Password must contain uppercase letter")
            .Matches(@"[a-z]").WithMessage("Password must contain lowercase letter")
            .Matches(@"[0-9]").WithMessage("Password must contain digit")
            .Matches(@"[\W_]").WithMessage("Password must contain special character");
    }
}
```

**Check:**
- [ ] Strong password policy (12+ chars, complexity)
- [ ] Multi-factor authentication for sensitive accounts
- [ ] JWT tokens with short expiration (1 hour)
- [ ] Refresh tokens with rotation
- [ ] Secure session management
- [ ] Account lockout after failed attempts

### 8. Software and Data Integrity Failures
```csharp
// ❌ VULNERABLE - No integrity check
public async Task<byte[]> DownloadFileAsync(string url)
{
    using var client = new HttpClient();
    return await client.GetByteArrayAsync(url); // Could be tampered!
}

// ✅ SECURE - Verify hash/signature
public async Task<byte[]> DownloadFileAsync(string url, string expectedHash)
{
    using var client = new HttpClient();
    var data = await client.GetByteArrayAsync(url);

    using var sha256 = SHA256.Create();
    var hash = Convert.ToBase64String(sha256.ComputeHash(data));

    if (hash != expectedHash)
        throw new SecurityException("File integrity check failed");

    return data;
}
```

**Check:**
- [ ] NuGet packages from trusted sources only
- [ ] Package signatures verified
- [ ] CI/CD pipeline secured
- [ ] Code signing for deployments
- [ ] Audit logs for critical operations

### 9. Security Logging and Monitoring Failures
```csharp
// ❌ INSUFFICIENT - Not logging security events
[HttpPost("login")]
public async Task<ActionResult<TokenResponse>> Login(LoginRequest request)
{
    var result = await _authService.LoginAsync(request.Email, request.Password);
    return Ok(result);
}

// ✅ SECURE - Log security events
[HttpPost("login")]
public async Task<ActionResult<TokenResponse>> Login(LoginRequest request)
{
    var result = await _authService.LoginAsync(request.Email, request.Password);

    if (result.IsSuccess)
    {
        _logger.LogInformation(
            "User {Email} logged in successfully from {IpAddress}",
            request.Email,
            HttpContext.Connection.RemoteIpAddress);
    }
    else
    {
        _logger.LogWarning(
            "Failed login attempt for {Email} from {IpAddress}",
            request.Email,
            HttpContext.Connection.RemoteIpAddress);
    }

    return Ok(result);
}
```

**Check:**
- [ ] Log failed login attempts
- [ ] Log authorization failures
- [ ] Log sensitive operations (delete, export data)
- [ ] Log multi-tenant access violations
- [ ] Alerts for suspicious activity
- [ ] Logs retained for 90+ days

### 10. Server-Side Request Forgery (SSRF)
```csharp
// ❌ VULNERABLE - User-controlled URL
[HttpGet("fetch")]
public async Task<ActionResult> FetchUrl(string url)
{
    using var client = new HttpClient();
    var content = await client.GetStringAsync(url); // SSRF risk!
    return Ok(content);
}

// ✅ SECURE - Whitelist allowed domains
[HttpGet("fetch")]
public async Task<ActionResult> FetchUrl(string url)
{
    var allowedHosts = new[] { "api.finkel.com.mx", "www.sat.gob.mx" };

    if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
        return BadRequest("Invalid URL");

    if (!allowedHosts.Contains(uri.Host))
        return BadRequest("Host not allowed");

    using var client = new HttpClient();
    var content = await client.GetStringAsync(url);
    return Ok(content);
}
```

**Check:**
- [ ] Whitelist allowed external URLs
- [ ] Validate URL schemes (only https://)
- [ ] No access to internal network resources
- [ ] Network segmentation

## Security Headers

```csharp
app.Use(async (context, next) =>
{
    // Prevent clickjacking
    context.Response.Headers.Add("X-Frame-Options", "DENY");

    // XSS protection
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");

    // Content Security Policy
    context.Response.Headers.Add("Content-Security-Policy",
        "default-src 'self'; script-src 'self'; style-src 'self' 'unsafe-inline';");

    // Referrer policy
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");

    // Permissions policy
    context.Response.Headers.Add("Permissions-Policy", "geolocation=(), microphone=(), camera=()");

    await next();
});

// HSTS
app.UseHsts(); // Enforces HTTPS
```

## Input Validation

```csharp
// ✅ Validate all user inputs
public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200)
            .Matches(@"^[a-zA-Z0-9\s\-\.]+$") // Only alphanumeric and basic punctuation
            .WithMessage("Invalid characters in product name");

        RuleFor(x => x.Sku)
            .NotEmpty()
            .MaximumLength(50)
            .Matches(@"^[A-Z0-9\-]+$") // Uppercase alphanumeric and hyphen
            .WithMessage("Invalid SKU format");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(9999999999.99m); // Prevent overflow
    }
}
```

## Secure File Upload

```csharp
[HttpPost("upload")]
public async Task<ActionResult<FileUploadResponse>> UploadFile(IFormFile file)
{
    // Validate file size
    if (file.Length > 10 * 1024 * 1024) // 10 MB
        return BadRequest("File too large");

    // Validate file type
    var allowedExtensions = new[] { ".pdf", ".xml", ".png", ".jpg" };
    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
    if (!allowedExtensions.Contains(extension))
        return BadRequest("File type not allowed");

    // Validate MIME type
    var allowedMimeTypes = new[] { "application/pdf", "text/xml", "image/png", "image/jpeg" };
    if (!allowedMimeTypes.Contains(file.ContentType))
        return BadRequest("Invalid file content type");

    // Generate safe filename (prevent directory traversal)
    var safeFileName = $"{Guid.NewGuid()}{extension}";

    // Scan for malware (if available)
    if (await _malwareScanner.IsMaliciousAsync(file))
        return BadRequest("File failed security scan");

    // Save to secure location
    await _storageService.SaveAsync(file, safeFileName);

    return Ok(new FileUploadResponse { FileName = safeFileName });
}
```

## Secrets Management

```csharp
// ❌ NEVER do this
var connectionString = "Host=localhost;Username=postgres;Password=MyPassword123"; // DON'T!

// ✅ Use Azure Key Vault
public class KeyVaultService
{
    private readonly SecretClient _client;

    public KeyVaultService(IConfiguration configuration)
    {
        var keyVaultUrl = configuration["AzureKeyVault:Url"];
        _client = new SecretClient(
            new Uri(keyVaultUrl),
            new DefaultAzureCredential());
    }

    public async Task<string> GetSecretAsync(string secretName)
    {
        var secret = await _client.GetSecretAsync(secretName);
        return secret.Value.Value;
    }
}

// Usage
var connectionString = await _keyVaultService.GetSecretAsync("DatabaseConnectionString");
```

## Security Testing

```bash
# Run security scan
dotnet tool install --global security-scan
security-scan Corelio.sln

# Check for vulnerabilities
dotnet list package --vulnerable --include-transitive

# Static analysis
dotnet tool install --global security-code-scan
dotnet security-code-scan Corelio.sln
```

## Security Checklist

### Development
- [ ] No secrets in source code
- [ ] Input validation on all endpoints
- [ ] Output encoding to prevent XSS
- [ ] Parameterized queries only
- [ ] Strong password policy
- [ ] Multi-factor authentication

### Infrastructure
- [ ] HTTPS/TLS 1.3 enforced
- [ ] Security headers configured
- [ ] CORS properly configured
- [ ] Rate limiting enabled
- [ ] Firewall rules configured
- [ ] Database access restricted

### Authentication/Authorization
- [ ] JWT with short expiration
- [ ] Refresh token rotation
- [ ] Permission-based authorization
- [ ] Multi-tenant isolation
- [ ] Session timeout configured

### Monitoring
- [ ] Security event logging
- [ ] Failed login monitoring
- [ ] Unusual activity alerts
- [ ] Audit logs enabled
- [ ] SIEM integration

### Compliance
- [ ] CFDI certificate security
- [ ] PII data encrypted
- [ ] GDPR compliance (if applicable)
- [ ] LFPDPPP compliance (Mexico)
- [ ] Regular security audits

---

**Remember**: Security is not optional - it's a fundamental requirement!
