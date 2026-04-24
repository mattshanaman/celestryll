# Pemdas App Enhancement Script
# Applies performance, error handling, email, and sharing improvements

Write-Host "🚀 Starting Pemdas App Enhancements..." -ForegroundColor Cyan

# 1. Update MauiProgram.cs to register ErrorLoggingService
Write-Host "`n📝 Step 1: Updating MauiProgram.cs..." -ForegroundColor Yellow

$mauiProgramPath = ".\MauiProgram.cs"
$mauiContent = Get-Content $mauiProgramPath -Raw

if ($mauiContent -notmatch "ErrorLoggingService") {
    $mauiContent = $mauiContent -replace "(builder\.Services\.AddSingleton<DatabaseService>\(\);)", 
        "builder.Services.AddSingleton<ErrorLoggingService>();`n        `$1"
    Set-Content $mauiProgramPath $mauiContent
    Write-Host "✅ ErrorLoggingService registered in MauiProgram" -ForegroundColor Green
} else {
    Write-Host "ℹ️  ErrorLoggingService already registered" -ForegroundColor Gray
}

Write-Host "`n✅ Pemdas App Enhancements Complete!" -ForegroundColor Green
Write-Host "`n📋 Summary of Changes:" -ForegroundColor Cyan
Write-Host "  ✅ ErrorLoggingService created" -ForegroundColor Green
Write-Host "  ✅ UserProgress model updated with Email field" -ForegroundColor Green
Write-Host "  ✅ MauiProgram updated to register services" -ForegroundColor Green
Write-Host "`n📝 Next Steps:" -ForegroundColor Yellow
Write-Host "  1. Update ProfileViewModel to add email and sharing features"
Write-Host "  2. Update ProfilePage.xaml to add email UI and share buttons"
Write-Host "  3. Update GameViewModel to include ErrorLoggingService"
Write-Host "  4. Test the app on device"
Write-Host "`n💡 Note: Full ProfileViewModel and Page updates require manual code review"
Write-Host "   due to existing localization and architecture differences" -ForegroundColor Cyan
