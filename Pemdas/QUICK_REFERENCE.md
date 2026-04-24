# Quick Reference: Error Handling & Database Security

## 🔐 Database Security - Quick Facts

### Access Control
```
Location: FileSystem.AppDataDirectory/badlydefined.db3
Encryption: AES-256
Key Storage: Platform Secure Storage (KeyChain/KeyStore)
End User Access: ❌ NO - File is encrypted and sandboxed
VS Debug Access: ✅ YES - Via Device File Explorer in Debug mode
Other Apps: ❌ NO - App sandbox prevents access
```

### Visual Studio Database Access
**Android:**
1. Run app in Debug mode
2. View → Other Windows → Device File Explorer
3. Navigate to: `/data/data/com.ambient.badlydefined/files/`
4. Right-click `badlydefined.db3` → Download
5. Note: File is encrypted, need decryption key to read

**iOS:**
1. Window → Devices and Simulators
2. Select device → Installed Apps
3. Find BadlyDefined → Download Container
4. Navigate to Documents folder

## 📧 Email Field Usage

### ProfileViewModel
```csharp
// Properties available:
UserEmail           // string - The email address
IsEmailValid        // bool - Validation result
EmailValidationMessage  // string - "✓ Valid" or "✗ Invalid"

// Command:
SaveEmailCommand    // Saves email to encrypted database
```

### Validation Rules
- Pattern: `^[^@\s]+@[^@\s]+\.[^@\s]+$`
- Optional field (can be empty)
- Auto-converts to lowercase
- Trims whitespace
- Stored encrypted in UserProgress table

## 🚨 Error Handling Patterns

### In Services
```csharp
try
{
    await _errorLogger.LogInfoAsync("Starting operation", "ServiceName.Method");
    // Your code here
    await _errorLogger.LogInfoAsync("Operation completed", "ServiceName.Method");
}
catch (Exception ex)
{
    await _errorLogger.LogErrorAsync(ex, "ServiceName.Method", new Dictionary<string, object>
    {
        { "ContextInfo", value }
    });
    throw new InvalidOperationException("User-friendly message", ex);
}
```

### In ViewModels
```csharp
try
{
    IsBusy = true;
    // Your code here
}
catch (Exception ex)
{
    await _errorLogger.LogErrorAsync(ex, "ViewModel.Method");
    ErrorMessage = _errorLogger.GetUserFriendlyMessage(ex);
    HasError = true;
    
    // Optional: Show alert
    await Application.Current.Windows[0].Page.DisplayAlertAsync(
        "Error",
        ErrorMessage,
        "OK");
}
finally
{
    IsBusy = false;
}
```

## 🔍 Error Log Access

### Get Recent Logs
```csharp
var logger = ServiceHelper.GetService<ErrorLoggingService>();
var logs = await logger.GetRecentLogsAsync(100);
```

### Clear Old Logs
```csharp
await logger.ClearOldLogsAsync();
```

### Log Location
```
Path: FileSystem.AppDataDirectory/error_log.txt
Max Size: 5MB
Retention: Last 1000 entries
Auto-Rotation: Yes
```

## 📊 User-Friendly Error Messages

```csharp
Exception Type                  → User Message
─────────────────────────────────────────────────────────────
InvalidOperationException       → "An operation could not be completed. Please try again."
UnauthorizedAccessException     → "Access denied. Please check your permissions."
IOException                     → "File operation failed. Please ensure the app has proper permissions."
SQLiteException                 → "Database error occurred. If this persists, try restarting the app."
ArgumentException               → "Invalid input provided. Please check your entry and try again."
TimeoutException                → "Operation timed out. Please try again."
Other                          → "An unexpected error occurred. Please try again or restart the app."
```

## 🛠️ Debugging Checklist

### Check Error Logs
```
1. Enable Debug output in VS
2. Look for emoji indicators:
   ✅ = Success
   ⚠️ = Warning
   ❌ = Error
   🔧 = Initialization
   📁 = File operations
3. Check error_log.txt file
```

### Common Issues

**Database Locked:**
```
Cause: Concurrent access attempt
Fix: Database uses FullMutex, should auto-resolve
Check: _initLock semaphore working correctly
```

**Encryption Key Error:**
```
Cause: SecureStorage access failed
Fix: Falls back to device-identifier
Check: Platform permissions for secure storage
```

**Email Validation Fails:**
```
Cause: Invalid format
Fix: User sees "✗ Invalid email format"
Check: Regex pattern matches requirements
```

## 🚀 Performance Tips

### Async Best Practices
```csharp
✅ DO: await operations
✅ DO: Use Task.WhenAll for parallel ops
✅ DO: Check IsBusy before operations
❌ DON'T: Use .Wait() or .Result
❌ DON'T: Block UI thread
❌ DON'T: Forget try-catch
```

### Database Performance
```csharp
✅ DO: Batch inserts with InsertAllAsync
✅ DO: Use indexed columns for queries
✅ DO: Keep connection pooled
❌ DON'T: Open multiple connections
❌ DON'T: Perform heavy queries on UI thread
❌ DON'T: Query in loops
```

## 🔒 Security Reminders

1. **Never log passwords or tokens**
2. **Email is considered PII** - handle carefully
3. **Database auto-encrypted** - no action needed
4. **Secure Storage handles keys** - no manual management
5. **App sandbox protects files** - OS enforced

## 📱 User Support

### When User Reports Error:
1. Ask for error message shown in app
2. Request device type and OS version
3. Check error_log.txt (if accessible)
4. Look for patterns in Debug output
5. Reproduce scenario if possible

### Recovery Steps:
```
1. Restart app
2. Clear app cache (Settings → Apps → BadlyDefined → Clear Cache)
3. Reinstall app (database recreated automatically)
4. Check device storage space
5. Verify OS version compatibility
```

---

**Need More Help?**
- See: `PERFORMANCE_SECURITY_REVIEW.md` for details
- See: `PERFORMANCE_SECURITY_REVIEW_SUMMARY.md` for complete overview
- Check error logs in app directory
- Use Debug output window in Visual Studio
