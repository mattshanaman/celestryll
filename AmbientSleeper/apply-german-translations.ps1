# Apply German Translations to AppResources.de.resx
# This script replaces English values with German translations

Write-Host "=================================" -ForegroundColor Cyan
Write-Host "German Translation Applicator" -ForegroundColor Cyan
Write-Host "=================================" -ForegroundColor Cyan
Write-Host ""

$resxFile = "Resources\Strings\AppResources.de.resx"

if (-not (Test-Path $resxFile)) {
    Write-Host "ERROR: File not found: $resxFile" -ForegroundColor Red
    exit 1
}

Write-Host "Reading file: $resxFile" -ForegroundColor Green

# Read the file content
$content = Get-Content $resxFile -Raw -Encoding UTF8

# Define German translations
$translations = @{
    # Common buttons
    "Ok" = "OK"
    "Cancel" = "Abbrechen"
    "Yes" = "Ja"
    "No" = "Nein"
    "Error" = "Fehler"
    
    # Navigation
    "Nav_Library" = "Bibliothek"
    "Nav_Playback" = "Wiedergabe"
    "Nav_Timer" = "Timer"
    "Nav_EQ" = "Equalizer"
    "Nav_Settings" = "Einstellungen"
    "Nav_Help" = "Hilfe"
    "Nav_Legal" = "Rechtliches"
    
    # Tabs
    "Tab_Mix" = "Mix"
    "Tab_Playlist" = "Playlist"
    "Tab_MixPlaylist" = "Mix-Playlist"
    "Tab_BuiltIn" = "Eingebaut"
    "Tab_YourAudio" = "Ihre Audiodateien"
    
    # Common buttons with icons
    "Common_PlayButton" = "? Abspielen"
    "Common_StopButton" = "? Stoppen"
    "Common_SaveButton" = "?? Speichern"
    "Common_LoadButton" = "?? Laden"
    "Common_DeleteIcon" = "??"
    
    # Library
    "Library_Title" = "Bibliothek"
    "ImportFromDevice" = "Vom Gerät importieren"
    "AudioBundles" = "Pakete"
    "A11y_AddToMix" = "Zum Mix hinzufügen"
    "A11y_AddToPlaylist" = "Zur Playlist hinzufügen"
    "Confirmation_SoundAdded" = "Sound zur Playlist hinzugefügt"
    
    # Playlist
    "PlaylistLocked_Title" = "Playlist gesperrt"
    "PlaylistLocked_Message" = "Upgrade auf Standard, Premium oder Pro+, um Playlists zu speichern oder zu ändern."
    "DefaultPlaylistCreated_Title" = "Standard-Playlist erstellt"
    "DefaultPlaylistCreated_Message" = "Eine 'Standard'-Playlist wurde erstellt."
    "SelectPlaylist_Title" = "Playlist auswählen"
    "EmptyPlaylist_Title" = "Leere Playlist"
    "EmptyPlaylist_Message" = "Fügen Sie zuerst Sounds zur Playlist-Warteschlange hinzu oder laden Sie eine gespeicherte Playlist."
    
    # Export/Import
    "ExportScope_Title" = "Export-Umfang"
    "ExportScope_Personal" = "Persönlich (Standard+)"
    "ExportScope_Shareable" = "Teilbar (Premium/Pro+)"
    "ExportLocked_Title" = "Export gesperrt"
    "ExportLocked_Message" = "Upgrade auf Standard, Premium oder Pro+, um Ihre Mixe zu exportieren."
    "ExportComplete_Title" = "Export abgeschlossen"
    "ExportComplete_Message" = "Ihre Mixe und Playlists wurden exportiert. Wählen Sie einen Speicherort für die Datei."
    "ExportFailed_Title" = "Export fehlgeschlagen"
    "ImportComplete_Title" = "Import abgeschlossen"
    "ImportComplete_MessageFormat" = "{0} Elemente importiert."
    "ImportFailed_Title" = "Import fehlgeschlagen"
    "PickExport_Title" = "Export auswählen"
    "PickAudio_Title" = "Audio auswählen"
    "PickAudio_TitleAlt" = "Audiodatei auswählen"
    "ExportShare_Title" = "Export teilen"
    "ExportShare_ShareSheet" = "Teilen"
    "ExportShare_Email" = "E-Mail"
    "ExportEmail_Subject" = "Ambient Sleeper Einstellungen"
    
    # Subscription
    "Subscription_Title" = "Abonnement"
    "Purchased_StandardLifetime" = "Standard Lifetime gekauft"
    "Purchased_PremiumLifetime" = "Premium Lifetime gekauft"
    "LifetimeSuffix" = "Lifetime"
    "CurrentTier_Format" = "Aktuelles Level: {0} {1}"
    
    # Mix Mode
    "Mix_Mode" = "Mix-Modus"
    "Mix_Empty" = "Ihr Mix ist leer. Fügen Sie Sounds aus der Bibliothek hinzu."
    "Mix_RemoveButton" = "Aus Mix entfernen"
    "Mix_SoundsInMixFormat" = "Sounds im Mix: {0}"
    "Mix_OfFormat" = "von {0}"
    "Mix_SaveCurrent" = "Aktuellen Mix speichern"
    "Mix_NamePlaceholder" = "Mix-Name"
    "Mix_SaveLimitFormat" = "Limit für gespeicherte Mixe: {0}"
    "Mix_SavedTitle" = "Gespeicherte Mixe"
    "Mix_SavedEmpty" = "Keine gespeicherten Mixe. Speichern Sie Ihren ersten Mix oben."
    "Mix_SoundsCountFormat" = "{0} Sounds"
    "Mix_DeleteButton" = "Mix löschen"
    "Mix_StopAllFormat" = "? Alle stoppen (Ausblenden {0}s)"
    
    # Playlist Mode
    "Playlist_Mode" = "Playlist-Modus"
    "Playlist_Empty" = "Ihre Playlist ist leer. Fügen Sie Sounds aus der Bibliothek hinzu."
    "Playlist_RemoveButton" = "Aus Playlist entfernen"
    "Playlist_SoundsCountFormat" = "{0} Sounds in der Playlist"
    "Playlist_LoopToggle" = "Playlist wiederholen"
    "Playlist_SaveCurrent" = "Aktuelle Playlist speichern"
    "Playlist_NamePlaceholder" = "Playlist-Name"
    "Playlist_SaveLimitFormat" = "Limit für gespeicherte Playlists: {0}"
    "Playlist_SavedTitle" = "Gespeicherte Playlists"
    "Playlist_SavedEmpty" = "Keine gespeicherten Playlists. Speichern Sie Ihre erste Playlist oben."
    "Playlist_DeleteButton" = "Playlist löschen"
    
    # Mix Playlist Mode
    "MixPlaylist_Mode" = "Mix-Playlist-Modus"
    "MixPlaylist_Empty" = "Ihre Mix-Playlist ist leer. Fügen Sie gespeicherte Mixe hinzu."
    "MixPlaylist_AddMix" = "Mix hinzufügen"
    "MixPlaylist_Remove" = "Entfernen"
    "MixPlaylist_RemoveButton" = "Aus Mix-Playlist entfernen"
    "MixPlaylist_Duration" = "Dauer:"
    "MixPlaylist_Seconds" = "Sekunden"
    "MixPlaylist_Transition" = "Übergang:"
    "MixPlaylist_Loop" = "Mix-Playlist wiederholen"
    "MixPlaylist_Save" = "Aktuelle Mix-Playlist speichern"
    "MixPlaylist_NamePlaceholder" = "Mix-Playlist-Name"
    "MixPlaylist_SaveLimitFormat" = "Limit für Mix-Playlists: {0}"
    "MixPlaylist_Saved" = "Gespeicherte Mix-Playlists"
    "MixPlaylist_SavedEmpty" = "Keine gespeicherten Mix-Playlists."
    "MixPlaylist_MixesCountFormat" = "{0} Mixe"
    "MixPlaylist_DeleteButton" = "Mix-Playlist löschen"
    "MixPlaylist_Locked_Title" = "Mix-Playlist gesperrt"
    "MixPlaylist_Locked_Message" = "Upgrade auf Pro+, um Mix-Playlists zu verwenden."
    
    # Playback Locked
    "Playback_PlaylistLocked_Title" = "Playlist gesperrt"
    "Playback_PlaylistLocked_Message" = "Upgrade auf Standard, Premium oder Pro+, um Playlists zu verwenden."
    
    # Toolbar
    "Toolbar_Tier" = "Level"
    "Toolbar_EQ" = "EQ"
    "Toolbar_Audio" = "Audio"
    "Toolbar_Export" = "Exportieren"
    "Toolbar_Import" = "Importieren"
    
    # Settings
    "Settings_Title" = "Abonnement-Einstellungen"
    "Settings_TierFree" = "Kostenlos"
    "Settings_TierStandard" = "Standard"
    "Settings_TierPremium" = "Premium"
    "Settings_TierProPlus" = "Pro+"
    "Settings_Recurring" = "Wiederkehrendes Abonnement"
    "Settings_Lifetime" = "Lifetime"
    "Settings_StandardLifetime" = "Standard Lifetime"
    "Settings_PremiumLifetime" = "Premium Lifetime"
    "Settings_CheckHealth" = "Status prüfen"
    "Settings_ErrorReport" = "Fehlerbericht"
    
    # Timer
    "Timer_Title" = "Schlaf-Timer"
    "Timer_StopAfterDuration" = "Nach Dauer stoppen"
    "Timer_StopAtTime" = "Zu bestimmter Zeit stoppen"
    "Timer_Duration" = "Dauer:"
    "Timer_StopAt" = "Stoppen um:"
    "Timer_Start" = "Timer starten"
    "Timer_Stop" = "Timer stoppen"
    "Timer_Remaining" = "Verbleibende Zeit: {0}"
    "Timer_Alarm" = "Alarm (Pro+/Premium)"
    "Timer_AlarmEnabled" = "Alarm aktiviert"
    "Alarm_Title" = "Alarm"
    "Alarm_Free_Message" = "Die Alarm-Funktion erfordert ein Premium- oder Pro+-Abonnement."
    
    # EQ
    "EQ_Title" = "Equalizer (Premium/Pro+)"
    "EQ_Locked" = "Equalizer erfordert Premium- oder Pro+-Abonnement"
    "EQ_Bass" = "Bass"
    "EQ_Mids" = "Mitten"
    "EQ_Treble" = "Höhen"
    "EQ_Reset" = "Zurücksetzen"
    
    # Bundles
    "Bundle_BuiltIn" = "Eingebaut"
    "Bundle_Locked" = "Gesperrt"
    "Bundle_UnlockWith" = "Entsperren mit {0}"
    
    # Notifications
    "Notification_TimerComplete_Title" = "Timer abgeschlossen"
    "Notification_TimerComplete_Description" = "Ihr Schlaf-Timer ist abgelaufen"
    
    # Navigation Errors
    "NavigationError_Title" = "Navigationsfehler"
    "NavigationError_Settings" = "Einstellungsseite konnte nicht geöffnet werden"
    
    # Health Check
    "HealthCheck_Checking" = "Status wird geprüft..."
    "HealthCheck_AllHealthy" = "Alles in Ordnung"
    "HealthCheck_Passed" = "Statusprüfung bestanden"
    "HealthCheck_IssuesDetected" = "{0} Probleme erkannt"
    "HealthCheck_ResultsTitle" = "Prüfungsergebnisse"
    "HealthCheck_IssuesMessage" = "Folgende Probleme wurden gefunden:\n• {0}"
    "HealthCheck_Failed" = "Prüfung fehlgeschlagen"
    "HealthCheck_FailedMessage" = "Fehler beim Prüfen des Status: {0}"
    
    # Error Reporting
    "ErrorReport_Title" = "Fehlerbericht"
    "ErrorReport_ServiceUnavailable" = "Fehlerberichtsdienst nicht verfügbar"
    "ErrorReport_NoErrors" = "Keine Fehler aufgezeichnet"
    "ErrorReport_CountFormat" = "{0} Fehler aufgezeichnet"
    "ErrorReport_ViewDetails" = "Details anzeigen"
    "ErrorReport_ShareReport" = "Bericht teilen"
    "ErrorReport_ClearErrors" = "Fehler löschen"
    "ErrorReport_SharedSuccess" = "Bericht erfolgreich geteilt"
    "ErrorReport_ClearConfirmTitle" = "Fehler löschen"
    "ErrorReport_ClearConfirmMessage" = "Sind Sie sicher, dass Sie alle Fehler löschen möchten?"
    "ErrorReport_Cleared" = "Fehler gelöscht"
    "ErrorReport_ViewFailed" = "Fehler beim Anzeigen des Berichts: {0}"
    
    # === NEW TRANSLATIONS - 195 MISSING STRINGS ===
    
    # Ad System (14 strings)
    "Ad_TimeAlmostUp_Title" = "? Zeit fast abgelaufen"
    "Ad_TimeRemaining" = "{0} Minuten verbleibend"
    "Ad_SessionExtended_Title" = "Sitzung verlängert!"
    "Ad_SessionExtended_Message" = "Sie haben 45 weitere Minuten verdient. Viel Spaß!"
    "Ad_SoundUnlocked_Title" = "Sound freigeschaltet!"
    "Ad_SoundUnlocked_Message" = "Sie können diesen Sound verwenden, bis Sie die App schließen."
    "Ad_WatchForExtension_Title" = "45 weitere Minuten erhalten"
    "Ad_WatchToUnlock_Message" = "Sehen Sie eine kurze Anzeige, um bis zum Schließen der App freizuschalten"
    "Ad_OrUpgrade" = "Oder upgraden Sie auf Premium für unbegrenzte Sitzungen"
    "Ad_LoadingAd" = "Anzeige wird geladen..."
    "Ad_AdNotAvailable_Title" = "Anzeige nicht verfügbar"
    "Ad_AdNotAvailable_Message" = "Momentan keine Anzeige verfügbar. Bitte versuchen Sie es gleich noch einmal."
    "Ad_Type_Banner" = "Banner"
    "Ad_Type_Rewarded" = "Belohnung"
    
    # Playback Settings (23 strings)
    "PlaybackSettings_Title" = "Wiedergabe-Einstellungen"
    "PlaybackSettings_Description" = "Ausblendzeit und Alarm-Integration konfigurieren"
    "PlaybackSettings_FadeOutTitle" = "Ausblend-Dauer"
    "PlaybackSettings_FadeOutDescription" = "Wählen Sie, wie lange Audio ausblendet, wenn der Timer endet"
    "PlaybackSettings_SecondsFormat" = "{0} Sekunden"
    "PlaybackSettings_TierAllowsFormat" = "Ihre Stufe erlaubt bis zu {0} Sekunden"
    "PlaybackSettings_Off" = "Aus"
    "PlaybackSettings_AudioSettings" = "Audio-Einstellungen"
    "PlaybackSettings_EnableAlarm" = "Alarm aktivieren"
    "PlaybackSettings_ChooseAlarmSound" = "Alarmton wählen"
    "PlaybackSettings_SelectedAlarm" = "Ausgewählter Alarm:"
    "PlaybackSettings_NoneSelected" = "Keiner ausgewählt"
    "PlaybackSettings_AlarmDescription" = "Umgebungsklänge stoppen und Alarm abspielen, wenn Timer endet"
    "PlaybackSettings_AlarmFreeNote" = "Free-Stufe verwendet Standard-Alarmton Ihres Geräts."
    "PlaybackSettings_AlarmLocked_Title" = "?? Alarm-Integration gesperrt"
    "PlaybackSettings_AlarmLocked_Message" = "Wachen Sie mit benutzerdefinierten Alarmtönen auf. Verfügbar ab Standard-Stufe."
    "PlaybackSettings_TipsTitle" = "?? Tipps"
    "PlaybackSettings_Tip1" = "• Längere Ausblendungen sorgen für sanfteres Aufwachen"
    "PlaybackSettings_Tip2" = "• Alarm-Integration erfordert aktive Timer-Funktion"
    "PlaybackSettings_Tip3" = "• Ihre Alarmton-Auswahl wird zwischen Sitzungen gespeichert"
    
    # EQ Page (33 strings)
    "EQ_FlatButton" = "0 Flach"
    "EQ_ApplyButton" = "? Anwenden"
    "EQ_ResetButton" = "? Zurücksetzen"
    "EQ_ChoosePreset" = "Voreinstellung wählen"
    "EQ_QuickPresets" = "Schnell-Voreinstellungen"
    "EQ_PresetsList" = "Voreinstellungen: Flach, Bass-Boost, Stimmen-Verbesserung, Höhen-Hell und mehr"
    "EQ_PresetsPremium" = "Premium"
    "EQ_PresetsProPlus" = "Pro+"
    "EQ_10BandParametric" = "10-Band parametrischer EQ"
    "EQ_AdvancedDescription" = "Erweiterte Steuerung mit Frequenz, Verstärkung und Q (Bandbreite) für jedes Band. Verwenden Sie Voreinstellungen für schnelle Einrichtung."
    "EQ_ProPlusBadge" = "Pro+"
    "EQ_Locked_Message" = "Passen Sie Ihren Sound mit professionellen Frequenzsteuerungen an. Verfügbar in Premium und Pro+ Stufen."
    "EQ_BandFormat" = "Band: {0:F0} Hz"
    "EQ_Q" = "Q (Bandbreite):"
    "EQ_QWidth" = "Q (Breite):"
    "EQ_Wide" = "Breit"
    "EQ_Narrow" = "Schmal"
    "EQ_MinFreq" = "20"
    "EQ_MinGain" = "-12"
    "EQ_MaxGain" = "+12"
    "EQ_Tip_Bass" = "• Bass (60-250 Hz): Erhöhen für tieferen, volleren Klang"
    "EQ_Tip_Mids" = "• Mitten (500-2k Hz): Anpassen für Klarheit und Wärme"
    "EQ_Tip_SmallAdjustments" = "• Beginnen Sie mit kleinen Anpassungen (±3 dB)"
    "EQ_Tip2" = "• Verwenden Sie Voreinstellungen als Startpunkt, dann fein abstimmen"
    "EQ_Tip3" = "• Vermeiden Sie zu viele Frequenzen zu erhöhen (verursacht Verzerrung)"
    "EQ_Tip4" = "• Machen Sie subtile Anpassungen für beste Ergebnisse"
    
    # Timer Page (17 strings)
    "Timer_AlarmLabel" = "Alarm abspielen wenn Timer abläuft"
    "Timer_FadeOutLabel" = "Ausblend-Dauer (Sekunden):"
    "Timer_ModeLabel" = "Timer-Modus"
    "Timer_DurationLabel" = "Dauer:"
    "Timer_DurationDescription" = "Festlegen, wie lange die Wiedergabe fortgesetzt werden soll (z.B. 30 Minuten)"
    "Timer_TimeDescription" = "Genaue Uhrzeit festlegen, wann Wiedergabe stoppen soll (z.B. 23:00 Uhr)"
    "Timer_FadeOutDescription" = "Lautstärke vor dem Stoppen allmählich reduzieren"
    "Timer_Controls" = "Timer-Steuerung"
    "Timer_TimerActive" = "?? Timer aktiv"
    "Timer_TimeRemaining" = "Verbleibende Zeit: {0:hh\\:mm\\:ss}"
    "Timer_WillStopIn" = "Wiedergabe stoppt in:"
    "Timer_WillFadeOut" = "Timer stoppt Wiedergabe und blendet Audio aus, wenn Zeit abläuft"
    "Timer_Remove5Min" = "? 5 Min. entfernen"
    "Timer_StopAlarm" = "?? Alarm stoppen"
    "Timer_StopAlarmDescription" = "Alarmton stoppen, falls aktuell abgespielt"
    "Timer_Tip1" = "• Verwenden Sie Dauer-Modus für Nickerchen (z.B. 30 Minuten)"
    "Timer_Tip2" = "• Verwenden Sie Stopp-Zeit-Modus zum Schlafengehen (z.B. 23:00 Uhr)"
    "Timer_Tip3" = "• Audio wird allmählich ausgeblendet, wenn Timer abläuft"
    
    # Settings Page (48 strings)
    "Settings_Description" = "Wählen Sie den Plan, der am besten zu Ihren Bedürfnissen passt. Upgraden Sie jederzeit, um mehr Funktionen freizuschalten."
    "Settings_CurrentStatus" = "Aktueller Status"
    "Settings_RestorePurchases" = "Käufe wiederherstellen"
    "Settings_RestoreNone" = "Keine Käufe zum Wiederherstellen"
    "Settings_Diagnostics_Description" = "App-Status prüfen und Fehlerberichte zur Fehlerbehebung anzeigen"
    "Settings_LifetimeTitle" = "Lebenslange Käufe"
    "Settings_Free_Title" = "Kostenlos"
    "Settings_Free_Description" = "Perfekt zum Ausprobieren der App"
    "Settings_Free_Feature2" = "? Zugriff auf alle Sound-Bundles"
    "Settings_Free_Feature3" = "? Bis zu 3 Mixe speichern"
    "Settings_Free_Limit" = "• 15-Minuten-Sitzungslimit"
    "Settings_Standard_Title" = "Standard"
    "Settings_Standard_Feature1" = "? 3 Sounds mischen"
    "Settings_Standard_Feature3" = "? Bis zu 10 Mixe speichern"
    "Settings_Standard_Yearly" = "$24/Jahr"
    "Settings_Premium_Title" = "Premium"
    "Settings_Premium_Badge" = "? Am beliebtesten"
    "Settings_Premium_Monthly" = "$9.99/Monat"
    "Settings_Premium_Feature1" = "? 10 Sounds mischen"
    "Settings_Premium_Feature2" = "? Unbegrenzte Mixe & Playlists"
    "Settings_Premium_Feature5" = "? Export/Import (teilbar)"
    "Settings_ProPlus_Title" = "Pro+"
    "Settings_ProPlus_Badge" = "?? Maximale Leistung"
    "Settings_ProPlus_Monthly" = "$14.99/Monat"
    "Settings_ProPlus_Yearly" = "$74/Jahr"
    "Settings_ProPlus_Feature3" = "? Benutzerdefinierte EQ-Voreinstellungen"
    "Settings_ProPlus_Feature4" = "? Mix-Übergänge"
    "Settings_ProPlus_Feature5" = "? Prioritäts-Support"
    "Settings_StandardLifetime_Price" = "$59"
    "Settings_StandardLifetime_Savings" = "?? Sparen Sie $35 vs. 3 Jahre Abonnement"
    "Settings_PremiumLifetime_Title" = "Premium Lebenslang"
    "Settings_PremiumLifetime_Price" = "$99"
    "Settings_PremiumLifetime_Savings" = "?? Sparen Sie $140 vs. 3 Jahre Abonnement"
    "Settings_PremiumLifetime_Badge" = "?? BESTER WERT"
    
    # Upgrade Page (4 strings)
    "Upgrade_Title" = "Upgrade"
    "Upgrade_Description" = "Kostenlose Sitzungen sind auf 15 Minuten begrenzt. Upgraden Sie für längere Sitzungen, Playlists, mehr gespeicherte Mixe und mehr."
    "Upgrade_UnlockLongerSessions" = "Längere Sitzungen freischalten"
    "Upgrade_NotNow" = "Nicht jetzt"
    
    # Common UI (7 strings - removed duplicate Toolbar_Audio)
    "AppName" = "Ambient Sleeper"
    "Mix_SaveButton" = "?? Speichern"
    "Mix_StopButton" = "? Stoppen"
    "Common_RemoveIcon" = "?"
    
    # Playback (2 strings)
    "Playback_MixPlaylistLocked_Message" = "Planen Sie verschiedene Klanglandschaften während Ihrer Sitzung. Verfügbar in Premium und Pro+ Stufen."
    "Playback_MixPlaylistEmpty" = "Erstellen und speichern Sie zuerst Mixe, dann fügen Sie sie hier hinzu, um eine geplante Playlist zu erstellen."
    
    # Legal Footer (1 string)
    "Legal_Footer_Copyright" = "Alle Rechte vorbehalten"
}
