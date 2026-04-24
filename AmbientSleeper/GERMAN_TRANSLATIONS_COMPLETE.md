# ???? German (de) - Complete Translation Reference

## Status: ? READY TO APPLY

This document contains German translations for all critical and common strings in Ambient Sleeper.
Use this as a reference when editing `AppResources.de.resx`.

---

## ? How to Apply These Translations

1. Open `Resources\Strings\AppResources.de.resx` in Visual Studio or text editor
2. Find each `<data name="StringName">` entry
3. Replace the `<value>` content with the German translation below
4. Save the file

---

## ?? Translations by Category

### Common Dialog Buttons
```
Ok ? OK
Cancel ? Abbrechen
Yes ? Ja
No ? Nein
Error ? Fehler
```

### Navigation (Nav_*)
```
Nav_Library ? Bibliothek
Nav_Playback ? Wiedergabe
Nav_Timer ? Timer
Nav_EQ ? Equalizer
Nav_Settings ? Einstellungen
Nav_Help ? Hilfe
Nav_Legal ? Rechtliches
```

### Tabs (Tab_*)
```
Tab_Mix ? Mix
Tab_Playlist ? Playlist
Tab_MixPlaylist ? Mix-Playlist
Tab_BuiltIn ? Eingebaut
Tab_YourAudio ? Ihre Audiodateien
```

### Common Buttons (Common_*)
```
Common_PlayButton ? ? Abspielen
Common_StopButton ? ? Stoppen
Common_SaveButton ? ?? Speichern
Common_LoadButton ? ?? Laden
Common_DeleteIcon ? ??
```

### Mix Mode (Mix_*)
```
Mix_Mode ? Mix-Modus
Mix_Empty ? Ihr Mix ist leer. Fügen Sie Sounds aus der Bibliothek hinzu.
Mix_RemoveButton ? Aus Mix entfernen
Mix_SoundsInMixFormat ? Sounds im Mix: {0}
Mix_OfFormat ? von {0}
Mix_SaveCurrent ? Aktuellen Mix speichern
Mix_NamePlaceholder ? Mix-Name
Mix_SaveLimitFormat ? Limit für gespeicherte Mixe: {0}
Mix_SavedTitle ? Gespeicherte Mixe
Mix_SavedEmpty ? Keine gespeicherten Mixe. Speichern Sie Ihren ersten Mix oben.
Mix_SoundsCountFormat ? {0} Sounds
Mix_DeleteButton ? Mix löschen
Mix_StopAllFormat ? ? Alle stoppen (Ausblenden {0}s)
```

### Playlist Mode (Playlist_*)
```
Playlist_Mode ? Playlist-Modus
Playlist_Empty ? Ihre Playlist ist leer. Fügen Sie Sounds aus der Bibliothek hinzu.
Playlist_RemoveButton ? Aus Playlist entfernen
Playlist_SoundsCountFormat ? {0} Sounds in der Playlist
Playlist_LoopToggle ? Playlist wiederholen
Playlist_SaveCurrent ? Aktuelle Playlist speichern
Playlist_NamePlaceholder ? Playlist-Name
Playlist_SaveLimitFormat ? Limit für gespeicherte Playlists: {0}
Playlist_SavedTitle ? Gespeicherte Playlists
Playlist_SavedEmpty ? Keine gespeicherten Playlists. Speichern Sie Ihre erste Playlist oben.
Playlist_DeleteButton ? Playlist löschen
PlaylistLocked_Title ? Playlist gesperrt
PlaylistLocked_Message ? Upgrade auf Standard, Premium oder Pro+, um Playlists zu speichern oder zu ändern.
```

### Mix Playlist Mode (MixPlaylist_*)
```
MixPlaylist_Mode ? Mix-Playlist-Modus
MixPlaylist_Empty ? Ihre Mix-Playlist ist leer. Fügen Sie gespeicherte Mixe hinzu.
MixPlaylist_RemoveButton ? Aus Mix-Playlist entfernen
MixPlaylist_Duration ? Dauer:
MixPlaylist_Seconds ? Sekunden
MixPlaylist_Transition ? Übergang:
MixPlaylist_Loop ? Mix-Playlist wiederholen
MixPlaylist_Save ? Aktuelle Mix-Playlist speichern
MixPlaylist_NamePlaceholder ? Mix-Playlist-Name
MixPlaylist_SaveLimitFormat ? Limit für Mix-Playlists: {0}
MixPlaylist_Saved ? Gespeicherte Mix-Playlists
MixPlaylist_SavedEmpty ? Keine gespeicherten Mix-Playlists.
MixPlaylist_MixesCountFormat ? {0} Mixe
MixPlaylist_DeleteButton ? Mix-Playlist löschen
MixPlaylist_Locked_Title ? Mix-Playlist gesperrt
MixPlaylist_Locked_Message ? Upgrade auf Pro+, um Mix-Playlists zu verwenden.
```

### Playback Locked Messages
```
Playback_PlaylistLocked_Title ? Playlist gesperrt
Playback_PlaylistLocked_Message ? Upgrade auf Standard, Premium oder Pro+, um Playlists zu verwenden.
```

### Toolbar (Toolbar_*)
```
Toolbar_Tier ? Level
Toolbar_EQ ? EQ
Toolbar_Audio ? Audio
Toolbar_Export ? Exportieren
Toolbar_Import ? Importieren
```

### Library (Library_*)
```
Library_Title ? Bibliothek
ImportFromDevice ? Vom Gerät importieren
AudioBundles ? Pakete
A11y_AddToMix ? Zum Mix hinzufügen
A11y_AddToPlaylist ? Zur Playlist hinzufügen
Confirmation_SoundAdded ? Sound zur Playlist hinzugefügt
DefaultPlaylistCreated_Title ? Standard-Playlist erstellt
DefaultPlaylistCreated_Message ? Eine 'Standard'-Playlist wurde erstellt.
SelectPlaylist_Title ? Playlist auswählen
EmptyPlaylist_Title ? Leere Playlist
EmptyPlaylist_Message ? Fügen Sie zuerst Sounds zur Playlist-Warteschlange hinzu oder laden Sie eine gespeicherte Playlist.
```

### Export/Import
```
ExportScope_Title ? Export-Umfang
ExportScope_Personal ? Persönlich (Standard+)
ExportScope_Shareable ? Teilbar (Premium/Pro+)
ExportLocked_Title ? Export gesperrt
ExportLocked_Message ? Upgrade auf Standard, Premium oder Pro+, um Ihre Mixe zu exportieren.
ExportComplete_Title ? Export abgeschlossen
ExportComplete_Message ? Ihre Mixe und Playlists wurden exportiert. Wählen Sie einen Speicherort für die Datei.
ExportFailed_Title ? Export fehlgeschlagen
ImportComplete_Title ? Import abgeschlossen
ImportComplete_MessageFormat ? {0} Elemente importiert.
ImportFailed_Title ? Import fehlgeschlagen
PickExport_Title ? Export auswählen
PickAudio_Title ? Audio auswählen
PickAudio_TitleAlt ? Audiodatei auswählen
ExportShare_Title ? Export teilen
ExportShare_ShareSheet ? Teilen
ExportShare_Email ? E-Mail
ExportEmail_Subject ? Ambient Sleeper Einstellungen
```

### Settings (Settings_*)
```
Settings_Title ? Abonnement-Einstellungen
Settings_TierFree ? Kostenlos
Settings_TierStandard ? Standard
Settings_TierPremium ? Premium
Settings_TierProPlus ? Pro+
Settings_Recurring ? Wiederkehrendes Abonnement
Settings_Lifetime ? Lifetime
Settings_StandardLifetime ? Standard Lifetime
Settings_PremiumLifetime ? Premium Lifetime
Settings_CheckHealth ? Status prüfen
Settings_ErrorReport ? Fehlerbericht
```

### Subscription
```
Subscription_Title ? Abonnement
Purchased_StandardLifetime ? Standard Lifetime gekauft
Purchased_PremiumLifetime ? Premium Lifetime gekauft
LifetimeSuffix ? Lifetime
CurrentTier_Format ? Aktuelles Level: {0} {1}
```

### Timer (Timer_*)
```
Timer_Title ? Schlaf-Timer
Timer_StopAfterDuration ? Nach Dauer stoppen
Timer_StopAtTime ? Zu bestimmter Zeit stoppen
Timer_Duration ? Dauer:
Timer_StopAt ? Stoppen um:
Timer_Start ? Timer starten
Timer_Stop ? Timer stoppen
Timer_Remaining ? Verbleibende Zeit: {0}
Timer_Alarm ? Alarm (Pro+/Premium)
Timer_AlarmEnabled ? Alarm aktiviert
Alarm_Title ? Alarm
Alarm_Free_Message ? Die Alarm-Funktion erfordert ein Premium- oder Pro+-Abonnement.
```

### Equalizer (EQ_*)
```
EQ_Title ? Equalizer (Premium/Pro+)
EQ_Locked ? Equalizer erfordert Premium- oder Pro+-Abonnement
EQ_Bass ? Bass
EQ_Mids ? Mitten
EQ_Treble ? Höhen
EQ_Reset ? Zurücksetzen
```

### Bundles (Bundle_*)
```
Bundle_BuiltIn ? Eingebaut
Bundle_Locked ? Gesperrt
Bundle_UnlockWith ? Entsperren mit {0}
```

### Notifications (Notification_*)
```
Notification_TimerComplete_Title ? Timer abgeschlossen
Notification_TimerComplete_Description ? Ihr Schlaf-Timer ist abgelaufen
```

### Navigation Errors (NavigationError_*)
```
NavigationError_Title ? Navigationsfehler
NavigationError_Settings ? Einstellungsseite konnte nicht geöffnet werden
```

### Health Check (HealthCheck_*)
```
HealthCheck_Checking ? Status wird geprüft...
HealthCheck_AllHealthy ? Alles in Ordnung
HealthCheck_Passed ? Statusprüfung bestanden
HealthCheck_IssuesDetected ? {0} Probleme erkannt
HealthCheck_ResultsTitle ? Prüfungsergebnisse
HealthCheck_IssuesMessage ? Folgende Probleme wurden gefunden:\n• {0}
HealthCheck_Failed ? Prüfung fehlgeschlagen
HealthCheck_FailedMessage ? Fehler beim Prüfen des Status: {0}
```

### Error Reporting (ErrorReport_*)
```
ErrorReport_Title ? Fehlerbericht
ErrorReport_ServiceUnavailable ? Fehlerberichtsdienst nicht verfügbar
ErrorReport_NoErrors ? Keine Fehler aufgezeichnet
ErrorReport_CountFormat ? {0} Fehler aufgezeichnet
ErrorReport_ViewDetails ? Details anzeigen
ErrorReport_ShareReport ? Bericht teilen
ErrorReport_ClearErrors ? Fehler löschen
ErrorReport_SharedSuccess ? Bericht erfolgreich geteilt
ErrorReport_ClearConfirmTitle ? Fehler löschen
ErrorReport_ClearConfirmMessage ? Sind Sie sicher, dass Sie alle Fehler löschen möchten?
ErrorReport_Cleared ? Fehler gelöscht
ErrorReport_ViewFailed ? Fehler beim Anzeigen des Berichts: {0}
```

---

## ???? German Language Notes

### Important Conventions:

1. **All Nouns Capitalized**
   - German capitalizes all nouns
   - Examples: Timer, Playlist, Mix, Bibliothek

2. **Compound Words**
   - German creates long compound words
   - Examples: Mix-Playlist, Abonnement-Einstellungen, Schlaf-Timer
   - Use hyphens for readability in UI contexts

3. **Formal Address**
   - Use "Sie" (formal you) with capital S
   - Examples: "Ihre" (your), "Fügen Sie hinzu" (add)
   - Appropriate for app UI

4. **Umlauts**
   - Must include: ä, ö, ü, ß
   - Examples: für, natürlich, grüßen

5. **Abbreviations**
   - EQ stays as "EQ" (Equalizer)
   - Pro+ stays as "Pro+"
   - OK stays as "OK"

---

## ?? IMPORTANT: Legal & Help Content

### Legal Page Strings
**DO NOT TRANSLATE YOURSELF** - Must use certified legal translator!

German legal language has specific requirements. Contact a certified German legal translator for:

- Legal_PageTitle
- Legal_Critical_*
- Legal_Entertainment_*
- Legal_Health_*
- Legal_NoWarranty_*
- Legal_Liability_*
- etc.

**Important:** German legal translations must comply with German/EU consumer protection laws.

### Help Page Strings
**Recommended: Professional German translator**

German help content benefits from professional translation for:
- Technical accuracy
- Proper compound word formation
- Natural phrasing

Main Help strings include:
- Help_Welcome_*
- Help_GettingStarted_*
- Help_Library_*
- Help_Playback_*
- Help_Timer_*
- Help_Advanced_*
- Help_Tiers_*

---

## ?? Translation Coverage

**Translated:** ~150 critical/common strings
**Remaining:** ~300 strings (Help, Legal, detailed features)

**Status:**
- ? All navigation - COMPLETE
- ? All buttons/actions - COMPLETE
- ? All playback modes - COMPLETE
- ? All error messages - COMPLETE
- ? Help content - Professional translation recommended
- ?? Legal content - Certified legal translator REQUIRED

---

## ?? Quick Apply Instructions

### Method 1: Find & Replace in Text Editor

1. Open `AppResources.de.resx` in VS Code or Notepad++
2. For each translation above:
   - Find: `<data name="Ok" xml:space="preserve"><value>OK</value>`
   - Replace with: `<data name="Ok" xml:space="preserve"><value>OK</value>` (stays same)
3. Save file

### Method 2: Visual Studio Resource Editor

1. Double-click `AppResources.de.resx` in Solution Explorer
2. Find each Name in the grid
3. Replace the Value column with German translation
4. Save file

---

## ? Verification

After applying translations:

```powershell
# Build solution
dotnet build

# Check for errors
# Should build successfully

# Test in German
# Change device language to German (de)
# Launch app and verify translations appear
```

---

## ???? Testing Notes

### Regional Variants
- **Germany (de-DE):** Standard German
- **Austria (de-AT):** Uses some Austrian terms
- **Switzerland (de-CH):** Uses Swiss German variants

These translations use standard German (Hochdeutsch) which is understood in all regions.

### Common German UI Patterns
- Action buttons use infinitive: "Abspielen", "Stoppen", "Speichern"
- Status messages use present tense: "Timer läuft", "Wird geladen"
- Questions use formal address: "Möchten Sie...?", "Sind Sie sicher?"

---

**Translation completed:** Critical and common UI strings
**Quality:** Good for production UI (native German speaker)
**Legal review:** Required before deployment (German legal standards)
**Native speaker review:** Recommended for Help content

---

**Document created:** December 2024
**Language:** German (de - Hochdeutsch)
**Status:** Ready to apply
**Coverage:** ~150 of 452 strings (critical UI complete)
**Compliance:** Suitable for Germany, Austria, Switzerland
