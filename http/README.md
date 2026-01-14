# HTTP Test Files

This directory contains HTTP request files for testing the Corelio API endpoints using the HTTP Client in Visual Studio Code or JetBrains IDEs.

## Prerequisites

### Visual Studio Code
Install the **REST Client** extension:
- Extension ID: `humao.rest-client`
- Install from: https://marketplace.visualstudio.com/items?itemName=humao.rest-client

### JetBrains IDEs (Rider, IntelliJ IDEA, etc.)
HTTP Client is built-in, no installation needed.

## Files Overview

### 1. `authentication.http`
**Purpose:** Happy path testing for all authentication endpoints.

**Endpoints Tested:**
- ✅ Register Tenant
- ✅ Login
- ✅ Register User (requires auth)
- ✅ Refresh Token
- ✅ Forgot Password
- ✅ Reset Password
- ✅ Logout

**Usage:**
1. Open `authentication.http`
2. Click "Send Request" above each `###` section
3. Variables are automatically extracted from responses using `@name` annotations
4. Execute requests in order for best results

### 2. `authentication-edge-cases.http`
**Purpose:** Testing error scenarios and validation.

**Test Categories:**
- ❌ Validation Errors (invalid email, short password, missing fields)
- ❌ Authentication Errors (wrong password, non-existent user)
- ❌ Authorization Errors (missing/invalid token)
- ❌ Duplicate Registration (subdomain, email conflicts)
- ❌ Invalid RFC Formats (Mexican Tax ID validation)
- ⏱️ Rate Limiting Tests (future implementation)

**Expected Results:**
- All requests should return appropriate error responses (400, 401, 409)
- Error messages should be descriptive

### 3. `multi-tenancy-tests.http`
**Purpose:** Verify multi-tenancy isolation and security.

**Test Scenarios:**
1. **Setup**: Register two separate tenants (Alpha & Beta)
2. **Operations**: Login and register employees for each tenant
3. **Isolation**: Verify users cannot cross tenant boundaries
4. **Security**: Confirm JWT claims enforce tenant isolation

**Key Tests:**
- ❌ Tenant A user cannot login with Tenant B subdomain
- ❌ Tenant B user cannot login with Tenant A subdomain
- ✅ Each tenant can only register users in their own tenant
- ✅ JWT tokens contain correct tenant_id claim

## How to Use

### Running a Single Request

1. Open any `.http` file
2. Click the **"Send Request"** link above the request
3. View the response in the Response panel

### Running All Requests in a File

**Visual Studio Code:**
```
Right-click anywhere in the file → "Run All Requests"
```

**JetBrains IDEs:**
```
Click the green arrow next to the file name → "Run All Requests in File"
```

### Variables

Variables are defined with `@variableName = value` syntax:

```http
@baseUrl = https://localhost:5001
@apiVersion = v1
```

Variables can be extracted from responses:

```http
# @name login
POST {{baseUrl}}/api/{{apiVersion}}/auth/login

### Extract from response
@accessToken = {{login.response.body.$.tokens.accessToken}}
```

### Environment-Specific Variables

Create separate environment files:

**`http-client.env.json`** (in `.vscode` or `.idea/httpRequests` folder):
```json
{
  "development": {
    "baseUrl": "https://localhost:5001",
    "apiVersion": "v1"
  },
  "staging": {
    "baseUrl": "https://staging.corelio.com",
    "apiVersion": "v1"
  },
  "production": {
    "baseUrl": "https://api.corelio.com",
    "apiVersion": "v1"
  }
}
```

## Testing Workflow

### 1. Start the API
```bash
# Start with Aspire
dotnet run --project src/Aspire/Corelio.AppHost

# Or start API directly
dotnet run --project src/Presentation/Corelio.WebAPI
```

### 2. Access Scalar API Documentation
Open your browser to:
```
https://localhost:5001/scalar/v1
```

**Scalar Features:**
- Modern, interactive API documentation
- Built-in authentication support
- Request/response examples
- Schema validation
- Dark mode support

### 3. Run HTTP Tests

**Recommended Order:**
1. `authentication.http` - Test happy path first
2. `multi-tenancy-tests.http` - Verify isolation
3. `authentication-edge-cases.http` - Test error handling

### 4. Inspect JWT Tokens

Copy any JWT token and decode at: https://jwt.io

**Expected Claims:**
```json
{
  "sub": "user-guid",
  "email": "user@example.com",
  "tenant_id": "tenant-guid",
  "roles": ["Owner", "Admin"],
  "permissions": ["users.view", "users.create", ...],
  "iss": "Corelio",
  "aud": "Corelio.WebAPI",
  "exp": 1234567890,
  "iat": 1234567890
}
```

## Troubleshooting

### SSL Certificate Errors
If you get SSL certificate errors:

1. Trust the development certificate:
```bash
dotnet dev-certs https --trust
```

2. Or disable SSL verification (not recommended for production):
   - VS Code: Add to settings.json: `"rest-client.enableSslVerification": false`
   - Rider: Settings → Tools → HTTP Client → Disable SSL verification

### Connection Refused
- Ensure the API is running
- Check the port in `@baseUrl` matches your API port
- Verify firewall settings

### 401 Unauthorized
- Check that your access token is valid and not expired (1 hour lifetime)
- Ensure the `Authorization: Bearer {{accessToken}}` header is present
- Verify the token was extracted correctly from the login response

### 409 Conflict (Duplicate Subdomain)
- Tenant subdomains must be unique globally
- Use a different subdomain or delete the existing tenant from the database

## Tips & Best Practices

1. **Variables**: Use variables for tokens and IDs to avoid copy-paste errors
2. **Comments**: Add comments using `###` to organize your requests
3. **Naming**: Use `# @name requestName` to name requests for variable extraction
4. **Order**: Execute requests in logical order (register → login → use token)
5. **Cleanup**: Logout when done to revoke refresh tokens

## Additional Resources

- [REST Client Extension Docs](https://github.com/Huachao/vscode-restclient)
- [JetBrains HTTP Client](https://www.jetbrains.com/help/rider/Http_client_in__product__code_editor.html)
- [Scalar Documentation](https://scalar.com/docs)
- [JWT.io](https://jwt.io) - JWT decoder and validator
