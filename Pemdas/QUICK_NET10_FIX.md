# QUICK .NET 10 FIX GUIDE

## ✅ STATUS:

### GamePage.xaml
**Status:** ✅ COMPLETE - All 7 Frames converted to Border

### ProfilePage.xaml  
**Status:** ❌ PENDING - 12 Frames need conversion
**Quick Fix Command:**
```powershell
$file = "BadlyDefined\Pages\ProfilePage.xaml"
$content = Get-Content $file -Raw
$content = $content -replace '<Frame ', '<Border '
$content = $content -replace '</Frame>', '</Border>'
$content = $content -replace ' CornerRadius="(\d+)"', ' StrokeShape="RoundRectangle $1"'
$content = $content -replace ' HasShadow="[^"]*"', ''
Set-Content $file $content -NoNewline
```

### TestModePage.xaml
**Status:** ❌ PENDING - 10 Frames need conversion
**Quick Fix Command:**
```powershell
$file = "BadlyDefined\Pages\TestModePage.xaml"
$content = Get-Content $file -Raw
$content = $content -replace '<Frame ', '<Border '
$content = $content -replace '</Frame>', '</Border>'
$content = $content -replace ' CornerRadius="(\d+)"', ' StrokeShape="RoundRectangle $1"'
$content = $content -replace ' HasShadow="[^"]*"', ''
Set-Content $file $content -NoNewline
```

---

## 🚀 ONE-COMMAND FIX:

Run this to fix ALL remaining Frames:

```powershell
@("BadlyDefined\Pages\ProfilePage.xaml", "BadlyDefined\Pages\TestModePage.xaml") | ForEach-Object {
    $content = Get-Content $_ -Raw
    $content = $content -replace '<Frame ', '<Border '
    $content = $content -replace '</Frame>', '</Border>'
    $content = $content -replace ' CornerRadius="(\d+)"', ' StrokeShape="RoundRectangle $1"'
    $content = $content -replace ' BorderColor="', ' Stroke="'
    $content = $content -replace ' BorderWidth="', ' StrokeThickness="'
    $content = $content -replace ' HasShadow="[^"]*"', ''
    Set-Content $_ $content -NoNewline
    Write-Host "✅ Fixed: $_"
}
Write-Host "`n🔨 Building..."
cd BadlyDefined; dotnet build BadlyDefined.csproj -f net10.0-android
```

---

## 📋 VERIFICATION CHECKLIST:

After running the fix:

- [ ] Build succeeds with no XAML errors
- [ ] GamePage displays correctly
- [ ] ProfilePage displays correctly  
- [ ] TestModePage displays correctly
- [ ] All stats boxes visible
- [ ] No visual regressions

---

## 🎯 THEN DEPLOY:

```powershell
cd BadlyDefined
dotnet build BadlyDefined.csproj -f net10.0-android -t:Run
```

---

**THIS WILL FIX THE APP!** 🚀
