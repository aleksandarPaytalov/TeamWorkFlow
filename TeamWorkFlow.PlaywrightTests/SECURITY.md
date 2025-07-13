# TeamWorkFlow Playwright Tests - Security Guide

## 🔒 Security Overview

This document outlines the security measures implemented for TeamWorkFlow Playwright tests to prevent credential exposure and maintain secure testing practices.

## 🚨 Security Issues Resolved

### Before (Vulnerabilities)
- ❌ Real email addresses hardcoded in `appsettings.json`
- ❌ Real passwords committed to source control
- ❌ No environment variable support
- ❌ Risk of credential exposure on GitHub

### After (Secure Implementation)
- ✅ Placeholder values in `appsettings.json`
- ✅ Environment variable support for sensitive data
- ✅ Secure fallback configuration
- ✅ Proper `.gitignore` exclusions
- ✅ Development-specific test credentials

## 🛡️ Security Architecture

### Configuration Hierarchy (Priority Order)
1. **Environment Variables** (Highest Priority)
   - `TEST_ADMIN_EMAIL`
   - `TEST_ADMIN_PASSWORD`
   - `TEST_OPERATOR_EMAIL`
   - `TEST_OPERATOR_PASSWORD`

2. **Configuration Files** (Medium Priority)
   - `appsettings.Development.json` (local development)
   - `appsettings.json` (contains placeholders only)

3. **Fallback Values** (Lowest Priority)
   - Safe default test credentials

### File Structure
```
TeamWorkFlow.PlaywrightTests/
├── appsettings.json                 # ✅ Safe - contains placeholders only
├── appsettings.Development.json     # ✅ Safe - test credentials only
├── .env.example                     # ✅ Safe - example format
├── .env                            # ❌ EXCLUDED from git
├── setup-test-environment.ps1      # ✅ Safe - setup script
└── SECURITY.md                     # ✅ This file
```

## 🔧 Setup Instructions

### For Local Development

1. **Use Development Configuration** (Recommended)
   ```bash
   # No setup needed - uses appsettings.Development.json
   dotnet test TeamWorkFlow.PlaywrightTests/
   ```

2. **Use Environment Variables** (Optional)
   ```powershell
   # Interactive setup
   .\TeamWorkFlow.PlaywrightTests\setup-test-environment.ps1 -SetEnvironmentVariables
   
   # Or set manually
   $env:TEST_ADMIN_EMAIL = "your-admin@test.local"
   $env:TEST_ADMIN_PASSWORD = "YourSecurePassword123!"
   $env:TEST_OPERATOR_EMAIL = "your-operator@test.local"
   $env:TEST_OPERATOR_PASSWORD = "YourSecurePassword123!"
   ```

### For CI/CD Pipelines

#### GitHub Actions Example
```yaml
name: Playwright Tests
on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'
      
      - name: Run Playwright Tests
        env:
          TEST_ADMIN_EMAIL: ${{ secrets.TEST_ADMIN_EMAIL }}
          TEST_ADMIN_PASSWORD: ${{ secrets.TEST_ADMIN_PASSWORD }}
          TEST_OPERATOR_EMAIL: ${{ secrets.TEST_OPERATOR_EMAIL }}
          TEST_OPERATOR_PASSWORD: ${{ secrets.TEST_OPERATOR_PASSWORD }}
        run: dotnet test TeamWorkFlow.PlaywrightTests/
```

#### Required GitHub Secrets
- `TEST_ADMIN_EMAIL`
- `TEST_ADMIN_PASSWORD`
- `TEST_OPERATOR_EMAIL`
- `TEST_OPERATOR_PASSWORD`

## 📋 Security Checklist

### ✅ Implemented Security Measures
- [x] Removed real credentials from configuration files
- [x] Added environment variable support
- [x] Created secure fallback configuration
- [x] Updated `.gitignore` to exclude sensitive files
- [x] Added placeholder values in main configuration
- [x] Created setup scripts for secure credential management
- [x] Documented security practices

### 🔍 Security Validation Commands
```powershell
# Check current configuration
.\TeamWorkFlow.PlaywrightTests\setup-test-environment.ps1 -ShowCurrentValues

# Verify no real credentials in git
git log --all --full-history -- "TeamWorkFlow.PlaywrightTests/appsettings.json"

# Check .gitignore effectiveness
git check-ignore TeamWorkFlow.PlaywrightTests/.env
```

## ⚠️ Security Best Practices

### DO ✅
- Use test-specific accounts only
- Set environment variables in CI/CD secrets
- Use different passwords for each environment
- Regularly rotate test credentials
- Review configuration files before commits

### DON'T ❌
- Commit real credentials to source control
- Use production accounts for testing
- Share credentials in plain text
- Hardcode sensitive data in source files
- Ignore security warnings

## 🚨 Emergency Response

### If Credentials Were Exposed
1. **Immediately rotate exposed credentials**
2. **Remove from git history if committed**
   ```bash
   git filter-branch --force --index-filter \
   'git rm --cached --ignore-unmatch TeamWorkFlow.PlaywrightTests/appsettings.json' \
   --prune-empty --tag-name-filter cat -- --all
   ```
3. **Update all environments with new credentials**
4. **Review access logs for unauthorized usage**

## 📞 Support

For security-related questions or concerns:
- Review this documentation
- Check the setup script: `setup-test-environment.ps1 -Help`
- Validate configuration: `setup-test-environment.ps1 -ShowCurrentValues`

## 🔄 Version History

- **v1.0** - Initial secure implementation
  - Removed hardcoded credentials
  - Added environment variable support
  - Implemented secure configuration hierarchy
