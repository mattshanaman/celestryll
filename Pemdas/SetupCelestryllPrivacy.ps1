Write-Host "🌟 CELESTRYLL MULTI-APP PRIVACY POLICY SETUP" -ForegroundColor Cyan
Write-Host "=" * 70 -ForegroundColor Gray
Write-Host ""

# Get GitHub username
Write-Host "📝 Please enter your GitHub username:" -ForegroundColor Yellow
$githubUsername = Read-Host

if ([string]::IsNullOrWhiteSpace($githubUsername)) {
    Write-Host "❌ GitHub username is required!" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "✅ GitHub Username: $githubUsername" -ForegroundColor Green
Write-Host ""

# Set up the repository path
$repoPath = Join-Path $env:USERPROFILE "Documents\celestryll-privacy"
$privacyHtmlSource = "celestryll-privacy.html"

# Check if celestryll-privacy.html exists
if (-not (Test-Path $privacyHtmlSource)) {
    Write-Host "❌ ERROR: celestryll-privacy.html not found in current directory" -ForegroundColor Red
    Write-Host "   Make sure you're in the Pemdas root directory" -ForegroundColor Yellow
    exit 1
}

Write-Host "📂 Creating local repository at: $repoPath" -ForegroundColor Yellow

# Check if directory exists
if (Test-Path $repoPath) {
    Write-Host "⚠️  Directory already exists. Remove it? (y/n)" -ForegroundColor Yellow
    $response = Read-Host
    if ($response -eq 'y') {
        Remove-Item $repoPath -Recurse -Force
        Write-Host "✅ Removed existing directory" -ForegroundColor Green
    } else {
        Write-Host "❌ Cancelled" -ForegroundColor Red
        exit 1
    }
}

# Create directory and initialize git
New-Item -ItemType Directory -Path $repoPath | Out-Null
Set-Location $repoPath

Write-Host "🔧 Initializing git repository..." -ForegroundColor Yellow
git init
git branch -M main

# Copy celestryll-privacy.html as index.html
Copy-Item $privacyHtmlSource "index.html"
Write-Host "✅ Copied celestryll-privacy.html → index.html" -ForegroundColor Green

# Create README
$readmeContent = @"
# Celestryll Privacy Policy

This repository hosts the unified privacy policy for all Celestryll mobile applications.

**View Privacy Policy:** [https://$githubUsername.github.io/celestryll-privacy/](https://$githubUsername.github.io/celestryll-privacy/)

## 🌟 About Celestryll

Celestryll creates high-quality mobile applications with a focus on user privacy and security.

## 📱 Apps Covered

- **BadlyDefined** - Daily word puzzle game
- Future Celestryll apps (listed as released)

## 🔐 Privacy Principles

- ✅ Privacy First - Your data belongs to you
- ✅ Local Storage - Data stays on your device
- ✅ Encryption - Sensitive data is always encrypted
- ✅ No Tracking - We don't track your behavior
- ✅ No Selling - We never sell your data
- ✅ Minimal Collection - Only what's essential
- ✅ Transparency - Clear disclosure of all practices

## 📧 Contact

- **Privacy Questions:** privacy@celestryll.com
- **Support:** support@celestryll.com

---

**Last Updated:** $(Get-Date -Format "MMMM dd, yyyy")  
© 2025 Celestryll | All Rights Reserved
"@

Set-Content "README.md" $readmeContent
Write-Host "✅ Created README.md" -ForegroundColor Green

# Add all files
git add .
git commit -m "Initial commit: Celestryll multi-app privacy policy"

Write-Host ""
Write-Host "=" * 70 -ForegroundColor Gray
Write-Host "🎯 ALMOST DONE! FINAL STEPS:" -ForegroundColor Cyan
Write-Host ""

Write-Host "1️⃣  CREATE GITHUB REPOSITORY:" -ForegroundColor Yellow
Write-Host "   Go to: https://github.com/new" -ForegroundColor White
Write-Host "   Repository name: celestryll-privacy" -ForegroundColor White
Write-Host "   Description: Privacy Policy for Celestryll Applications" -ForegroundColor White
Write-Host "   ✅ Make it PUBLIC (required for GitHub Pages)" -ForegroundColor Green
Write-Host "   ❌ DON'T initialize with README (we have one)" -ForegroundColor Red
Write-Host "   Click 'Create repository'" -ForegroundColor White
Write-Host ""

Write-Host "2️⃣  PUSH YOUR CODE:" -ForegroundColor Yellow
Write-Host "   Run these commands:" -ForegroundColor White
Write-Host ""
Write-Host "   git remote add origin https://github.com/$githubUsername/celestryll-privacy.git" -ForegroundColor Cyan
Write-Host "   git push -u origin main" -ForegroundColor Cyan
Write-Host ""

Write-Host "3️⃣  ENABLE GITHUB PAGES:" -ForegroundColor Yellow
Write-Host "   Go to: https://github.com/$githubUsername/celestryll-privacy/settings/pages" -ForegroundColor White
Write-Host "   Under 'Source': Select 'Deploy from a branch'" -ForegroundColor White
Write-Host "   Branch: main, Folder: / (root)" -ForegroundColor White
Write-Host "   Click 'Save'" -ForegroundColor White
Write-Host ""

Write-Host "4️⃣  WAIT 1-2 MINUTES, then visit:" -ForegroundColor Yellow
Write-Host "   https://$githubUsername.github.io/celestryll-privacy/" -ForegroundColor Green
Write-Host ""

Write-Host "5️⃣  USE FOR ALL YOUR APPS:" -ForegroundColor Yellow
Write-Host "   BadlyDefined: Paste URL into Play Console → Privacy Policy" -ForegroundColor White
Write-Host "   Future Apps: Use same URL - policy covers all Celestryll apps!" -ForegroundColor White
Write-Host ""

Write-Host "=" * 70 -ForegroundColor Gray
Write-Host ""
Write-Host "🌟 BENEFITS OF MULTI-APP POLICY:" -ForegroundColor Cyan
Write-Host "   ✅ One URL for all your apps" -ForegroundColor Green
Write-Host "   ✅ Consistent branding (Celestryll)" -ForegroundColor Green
Write-Host "   ✅ Easy to update (one place)" -ForegroundColor Green
Write-Host "   ✅ Professional appearance" -ForegroundColor Green
Write-Host "   ✅ Covers future apps automatically" -ForegroundColor Green
Write-Host ""

Write-Host "📍 You are now in: $repoPath" -ForegroundColor Cyan
Write-Host ""
Write-Host "Ready to continue with steps 1-5 above? Press any key..." -ForegroundColor Yellow
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
