# Quick Reference: Help System Maintenance

## ?? Common Tasks

### Add New Help Section

**Location:** `Views/HelpPage.xaml.cs` ? `GenerateHelpHtml()` method

```csharp
// Add after existing sections (around line 150)
sb.AppendLine("<h2>Your New Section</h2>");
sb.AppendLine("<div class='feature-box'>");
sb.AppendLine("<p>Your content here...</p>");
sb.AppendLine("<ul>");
sb.AppendLine("<li>Bullet point 1</li>");
sb.AppendLine("<li>Bullet point 2</li>");
sb.AppendLine("</ul>");
sb.AppendLine("</div>");
```

### Update Subscription Pricing

**Location:** `Views/HelpPage.xaml.cs` ? Line ~120 (Subscription Tiers section)

```csharp
// Find and update these lines:
sb.AppendLine("<h3>Standard Tier ($4.99/mo or $24/yr)</h3>");
sb.AppendLine("<h3>Premium Tier ($9.99/mo or $49/yr)</h3>");
sb.AppendLine("<h3>Pro+ Tier ($14.99/mo or $74/yr)</h3>");
```

### Change Color Theme

**Location:** `Views/HelpPage.xaml.cs` ? `<style>` block (around line 40)

```css
/* Primary color */
border-bottom: 3px solid #3498db;  /* Change #3498db to your color */

/* Accent colors */
.feature-box { border-left: 4px solid #3498db; }
.tip { border-left: 4px solid #27ae60; }
.warning { border-left: 4px solid #fdcb6e; }
```

### Add Tip or Warning Box

```csharp
// Tip (green)
sb.AppendLine("<div class='tip'>");
sb.AppendLine("<strong>Tip:</strong> Your helpful tip here");
sb.AppendLine("</div>");

// Warning (yellow)
sb.AppendLine("<div class='warning'>");
sb.AppendLine("<strong>Note:</strong> Important information");
sb.AppendLine("</div>");
```

### Add Feature Box

```csharp
sb.AppendLine("<div class='feature-box'>");
sb.AppendLine("<h3>Feature Name</h3>");
sb.AppendLine("<p>Feature description</p>");
sb.AppendLine("</div>");
```

---

## ?? Troubleshooting

### Help Page Blank

**Check:**
1. Output window for exceptions
2. `GenerateHelpHtml()` method for syntax errors
3. HTML closing tags match opening tags

**Fix:**
```csharp
// Add try-catch in GenerateHelpHtml()
try {
    // HTML generation code
}
catch (Exception ex) {
    System.Diagnostics.Debug.WriteLine($"HTML Error: {ex}");
    return "<html><body><p>Error loading help</p></body></html>";
}
```

### Icons Not Showing

**Fix 1:** Use emoji instead
```xaml
<FlyoutItem Title="?? Help & Instructions" .../>
```

**Fix 2:** Add image resource
```xaml
<FlyoutItem Title="Help" Icon="help.png" .../>
```

### HTML Not Rendering Properly

**Debug:**
```csharp
// In LoadHelpContent(), add:
var html = GenerateHelpHtml();
System.Diagnostics.Debug.WriteLine(html);  // View in Output
```

---

## ?? Adding Localization

### Step 1: Add to AppResources.resx

```xml
<data name="Help_Title" xml:space="preserve">
  <value>Help & Instructions</value>
</data>
```

### Step 2: Update Code

```csharp
// At top of HelpPage.xaml.cs:
using AmbientSleeper.Resources.Strings;

// In GenerateHelpHtml():
sb.AppendLine($"<h1>{AppResources.Help_Title}</h1>");
```

### Step 3: Create Translation Files
- `AppResources.es.resx` (Spanish)
- `AppResources.fr.resx` (French)
- etc.

---

## ?? Platform-Specific Notes

### iOS
- WebView works out of the box
- Dark mode automatic
- No special permissions needed

### Android
- WebView works out of the box
- May need `android:usesCleartextTraffic="true"` in Manifest if loading external content
- For local content (our case): no changes needed

### Windows
- WebView2 required (included in .NET MAUI)
- Works out of the box
- Dark mode supported

---

## ?? CSS Classes Available

```css
.container      /* Main content container */
.section        /* Generic section wrapper */
.feature-box    /* Blue bordered box for features */
.tip            /* Green bordered box for tips */
.warning        /* Yellow bordered box for warnings */
.tier           /* Subscription tier box */
```

**Usage:**
```csharp
sb.AppendLine("<div class='tip'>");
sb.AppendLine("Your content");
sb.AppendLine("</div>");
```

---

## ? Performance Tips

### Keep HTML Generation Fast
- Current: ~10-50ms
- Target: < 100ms
- Avoid: Complex loops, file I/O, network calls

### Optimize If Needed
```csharp
// Cache HTML if it doesn't change
private static string? _cachedHtml;

private string GenerateHelpHtml() {
    if (_cachedHtml != null) return _cachedHtml;
    
    var html = /* generate HTML */;
    _cachedHtml = html;
    return html;
}
```

---

## ?? Monitoring

### Add Analytics (Optional)

```csharp
// In LoadHelpContent()
protected override void OnAppearing()
{
    base.OnAppearing();
    // Log analytics event
    // Analytics.TrackEvent("Help_Viewed");
}
```

### Track Sections Viewed

```javascript
// Add to HTML <script> block
<script>
document.addEventListener('click', function(e) {
    if (e.target.tagName === 'H2') {
        // Track which section was clicked
        console.log('Section viewed: ' + e.target.textContent);
    }
});
</script>
```

---

## ?? Related Files

| File | Purpose |
|------|---------|
| `Views/HelpPage.xaml` | UI layout |
| `Views/HelpPage.xaml.cs` | HTML generation & logic |
| `AppShell.xaml` | Navigation menu |
| `USER_INSTRUCTIONS.md` | Full documentation (markdown) |
| `HELP_STRINGS_TO_ADD.md` | Localization strings |

---

## ?? Quick Commands

```bash
# Build
dotnet build

# Clean build
dotnet clean
dotnet build

# Run on Android
dotnet build -t:Run -f net9.0-android

# Run on iOS
dotnet build -t:Run -f net9.0-ios
```

---

## ? Testing Checklist

Before deploying changes:

- [ ] HTML validates (no unclosed tags)
- [ ] Build succeeds with no warnings
- [ ] Help page loads without errors
- [ ] All sections display correctly
- [ ] Scrolling works smoothly
- [ ] Dark mode renders properly
- [ ] Back button works
- [ ] Content is accurate
- [ ] Typos checked
- [ ] Links work (if any external)

---

## ?? Getting Help

**Issue:** Can't find where to edit X
**Solution:** Search for the text in `HelpPage.xaml.cs`

**Issue:** HTML broken after edit
**Solution:** Check for unmatched quotes, unclosed tags

**Issue:** Build fails
**Solution:** Check Output window, fix syntax errors

---

## ?? Pro Tips

1. **Preview HTML:** Copy generated HTML to online HTML viewer
2. **Test Dark Mode:** Toggle device dark mode while viewing help
3. **Keep It Simple:** Don't overdo styling - readability first
4. **Version It:** Update footer version number when content changes
5. **User Feedback:** Add feedback mechanism to improve content

---

**Last Updated:** January 2025  
**Maintainer:** Development Team  
**Questions?** Check `HELP_IMPLEMENTATION_COMPLETE.md` for details
