Write-Host "🚀 GITHUB PAGES PRIVACY POLICY SETUP" -ForegroundColor Cyan
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
$repoPath = Join-Path $env:USERPROFILE "Documents\badlydefined-privacy"
$privacyHtmlSource = "privacy.html"

# Check if privacy.html exists
if (-not (Test-Path $privacyHtmlSource)) {
    Write-Host "❌ ERROR: privacy.html not found in current directory" -ForegroundColor Red
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

# Copy privacy.html as index.html
Copy-Item $privacyHtmlSource "index.html"
Write-Host "✅ Copied privacy.html → index.html" -ForegroundColor Green

# Create README
$readmeContent = @"
# BadlyDefined Privacy Policy

This repository hosts the privacy policy for the BadlyDefined mobile game.

**View Privacy Policy:** [https://$githubUsername.github.io/badlydefined-privacy/](https://$githubUsername.github.io/badlydefined-privacy/)

## About BadlyDefined

BadlyDefined is a daily word puzzle game where players guess words from hilariously bad definitions.

- 🎮 Daily puzzles across 3 difficulty levels
- 💡 Smart hint system
- 🏆 Progress tracking and statistics
- 🔒 Encrypted local storage
- 📱 100% offline gameplay

**Download:** [Coming soon to Google Play Store]

---

**Last Updated:** $(Get-Date -Format "MMMM dd, yyyy")
"@

Set-Content "README.md" $readmeContent
Write-Host "✅ Created README.md" -ForegroundColor Green

# Add all files
git add .
git commit -m "Initial commit: Privacy policy for BadlyDefined app"

Write-Host ""
Write-Host "=" * 70 -ForegroundColor Gray
Write-Host "🎯 ALMOST DONE! FINAL STEPS:" -ForegroundColor Cyan
Write-Host ""

Write-Host "1️⃣  CREATE GITHUB REPOSITORY:" -ForegroundColor Yellow
Write-Host "   Go to: https://github.com/new" -ForegroundColor White
Write-Host "   Repository name: badlydefined-privacy" -ForegroundColor White
Write-Host "   ✅ Make it PUBLIC (required for GitHub Pages)" -ForegroundColor Green
Write-Host "   ❌ DON'T initialize with README (we have one)" -ForegroundColor Red
Write-Host "   Click 'Create repository'" -ForegroundColor White
Write-Host ""

Write-Host "2️⃣  PUSH YOUR CODE:" -ForegroundColor Yellow
Write-Host "   Run these commands:" -ForegroundColor White
Write-Host ""
Write-Host "   git remote add origin https://github.com/$githubUsername/badlydefined-privacy.git" -ForegroundColor Cyan
Write-Host "   git push -u origin main" -ForegroundColor Cyan
Write-Host ""

Write-Host "3️⃣  ENABLE GITHUB PAGES:" -ForegroundColor Yellow
Write-Host "   Go to: https://github.com/$githubUsername/badlydefined-privacy/settings/pages" -ForegroundColor White
Write-Host "   Under 'Source': Select 'Deploy from a branch'" -ForegroundColor White
Write-Host "   Branch: main, Folder: / (root)" -ForegroundColor White
Write-Host "   Click 'Save'" -ForegroundColor White
Write-Host ""

Write-Host "4️⃣  WAIT 1-2 MINUTES, then visit:" -ForegroundColor Yellow
Write-Host "   https://$githubUsername.github.io/badlydefined-privacy/" -ForegroundColor Green
Write-Host ""

Write-Host "5️⃣  ADD TO GOOGLE PLAY CONSOLE:" -ForegroundColor Yellow
Write-Host "   Paste this URL into Play Console → Privacy Policy field" -ForegroundColor White
Write-Host ""

Write-Host "=" * 70 -ForegroundColor Gray
Write-Host ""
Write-Host "📍 You are now in: $repoPath" -ForegroundColor Cyan
Write-Host ""
Write-Host "Ready to continue with steps 1-5 above? Press any key..." -ForegroundColor Yellow
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
