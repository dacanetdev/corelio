# Security Policy

## Supported Versions

| Version | Supported          |
| ------- | ------------------ |
| 1.x.x   | :white_check_mark: |
| < 1.0   | :x:                |

## Reporting a Vulnerability

**DO NOT** create public GitHub issues for security vulnerabilities.

### How to Report

Send security vulnerability reports to: **security@deventsoft.com**

Include:
- Description of the vulnerability
- Steps to reproduce
- Potential impact
- Suggested fix (if any)

### What to Expect

1. **Acknowledgment** within 24-48 hours
2. **Initial assessment** within 72 hours
3. **Regular updates** on progress
4. **Fix timeline** based on severity
5. **Public disclosure** after patch is released (coordinated)

### Severity Classification

| Severity | Response Time | Examples |
|----------|---------------|----------|
| **Critical** | <24 hours | Multi-tenant data leak, remote code execution |
| **High** | <72 hours | Authentication bypass, SQL injection |
| **Medium** | <1 week | XSS, CSRF, information disclosure |
| **Low** | <2 weeks | Minor information leaks, deprecated protocols |

## Security Best Practices

### For Developers

#### Multi-Tenancy Security (CRITICAL)

- ✅ **ALWAYS** use `ITenantService.GetCurrentTenantId()`
- ❌ **NEVER** accept `tenant_id` from client input
- ✅ All business entities **MUST** implement `ITenantEntity`
- ✅ Write **isolation tests** for every feature
- ✅ Review code for potential tenant leakage
- ⚠️ **Zero tolerance** for data leaks between tenants

#### Authentication & Authorization

- ✅ Use **JWT tokens** with short expiration (1 hour)
- ✅ Implement **refresh tokens** (7 days)
- ✅ Validate **permissions** on every endpoint
- ✅ Use **bcrypt** for password hashing (cost factor 12+)
- ❌ Never store passwords in plain text
- ✅ Implement **rate limiting** (prevent brute force)
- ✅ Support **2FA** for sensitive accounts

#### Data Protection

- ✅ Encrypt **sensitive data at rest**
- ✅ Use **HTTPS/TLS 1.3** for all communication
- ✅ Store **CSD certificates** in Azure Key Vault only
- ❌ Never store certificates in database
- ✅ Implement **data retention** policies
- ✅ Support **data export** for GDPR/LFPDPPP

#### Input Validation

- ✅ Use **FluentValidation** for all inputs
- ✅ Validate **RFC and CURP** formats
- ✅ Sanitize **user inputs** (prevent XSS)
- ✅ Use **parameterized queries** (prevent SQL injection)
- ✅ Validate **file uploads** (type, size, content)
- ❌ Never trust client-side validation

#### Secrets Management

- ❌ **NEVER** commit secrets to version control
- ✅ Use **dotnet user-secrets** for local development
- ✅ Use **Azure Key Vault** for production
- ✅ Use **environment variables** for configuration
- ✅ Rotate **secrets regularly**
- ✅ Use **separate secrets** per environment

#### API Security

- ✅ Implement **CORS** with whitelisted origins
- ✅ Use **CSRF protection** for state-changing operations
- ✅ Implement **rate limiting** per IP and per user
- ✅ Log **security events** (failed logins, permission denials)
- ✅ Return **generic error messages** (don't leak info)
- ✅ Implement **request size limits**

### For Deployers

#### Infrastructure

- ✅ Use **managed PostgreSQL** with encryption
- ✅ Enable **Azure Key Vault** for secrets
- ✅ Configure **network security groups**
- ✅ Use **private endpoints** where possible
- ✅ Enable **diagnostic logging**
- ✅ Implement **automated backups** (daily)

#### Monitoring

- ✅ Monitor **failed login attempts**
- ✅ Alert on **unusual access patterns**
- ✅ Track **API rate limit violations**
- ✅ Monitor **database query patterns**
- ✅ Set up **security alerts** (Azure Security Center)

#### Incident Response

- ✅ Have **incident response plan**
- ✅ Know how to **revoke compromised credentials**
- ✅ Have **backup and restore** procedures
- ✅ Document **rollback procedures**
- ✅ Maintain **audit logs** for 90+ days

## Known Security Considerations

### Multi-Tenant Architecture

Our multi-tenant architecture uses **row-level security** with:

1. **Query Filters**: Automatically applied to all queries
2. **Save Interceptors**: Automatically set `tenant_id` on inserts
3. **Tenant Middleware**: Resolves tenant from JWT claims
4. **Isolation Tests**: Verify zero data leaks

**Critical Areas**:
- Database migrations
- Background jobs
- Reporting/exports
- Admin functions

### CFDI Certificate Management

CSD certificates for CFDI signing are:

- ✅ Stored in **Azure Key Vault**
- ✅ Loaded in **memory only** during signing
- ✅ Never persisted to disk or database
- ✅ Access logged and monitored
- ✅ Rotation alerts 30 days before expiry

### Third-Party Dependencies

We regularly scan dependencies for vulnerabilities using:

- **Dependabot**: Automated dependency updates
- **Snyk**: Vulnerability scanning
- **Trivy**: Container scanning
- **OWASP Dependency Check**: Nightly scans

## Compliance

- **CFDI 4.0**: Mexican tax regulation compliance
- **LFPDPPP**: Mexican data protection law
- **GDPR**: European data protection (if applicable)
- **OWASP Top 10**: Web application security

## Security Checklist

Before deploying to production:

- [ ] All secrets moved to Azure Key Vault
- [ ] HTTPS/TLS 1.3 enforced
- [ ] Rate limiting configured
- [ ] CORS whitelist configured
- [ ] Audit logging enabled
- [ ] Backup strategy implemented
- [ ] Incident response plan documented
- [ ] Security scan passed (no high/critical issues)
- [ ] Multi-tenancy isolation verified
- [ ] Code review completed
- [ ] Penetration testing performed (if required)

## Security Tools

### Development

- **SonarQube**: Code quality and security analysis
- **Snyk**: Dependency vulnerability scanning
- **dotnet format**: Code style enforcement
- **EditorConfig**: Consistent code formatting

### CI/CD

- **GitHub Actions**: Automated security scanning
- **Trivy**: Container vulnerability scanning
- **OWASP Dependency Check**: Dependency scanning
- **CodeQL**: Static analysis (if enabled)

### Production

- **Azure Security Center**: Cloud security posture
- **Application Insights**: Monitoring and alerting
- **Azure Key Vault**: Secrets management
- **Azure AD**: Identity and access management

## Responsible Disclosure

We practice **responsible disclosure**:

1. Reporter notifies us privately
2. We acknowledge and investigate
3. We develop and test a fix
4. We release a patch
5. We publicly disclose (coordinated with reporter)
6. We credit the reporter (if desired)

## Bug Bounty

Currently, we do not have a formal bug bounty program. However, we appreciate security researchers and will:

- Acknowledge your contribution
- Credit you in release notes (if desired)
- Consider rewards on a case-by-case basis

## Contact

- **Security Email**: security@deventsoft.com
- **General Support**: support@deventsoft.com
- **PGP Key**: Available upon request

---

**Last Updated**: 2025-12-22
**Security Team**: Corelio Security Team
