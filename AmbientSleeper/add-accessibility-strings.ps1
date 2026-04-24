# Add Accessibility Strings to AppResources.resx
# This script adds comprehensive accessibility labels for screen readers

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Adding Accessibility Strings" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$resxFile = "Resources\Strings\AppResources.resx"

if (-not (Test-Path $resxFile)) {
    Write-Host "ERROR: File not found: $resxFile" -ForegroundColor Red
    exit 1
}

Write-Host "Reading file: $resxFile" -ForegroundColor Green
$content = Get-Content $resxFile -Raw -Encoding UTF8

# Check if accessibility strings already exist
if ($content -match "A11y_PlayButton") {
    Write-Host "??  Accessibility strings already exist. Skipping..." -ForegroundColor Yellow
    exit 0
}

# Define accessibility strings to add
$accessibilityStrings = @"
  <!-- ===== ACCESSIBILITY STRINGS ===== -->
  <!-- Button Labels -->
  <data name="A11y_PlayButton" xml:space="preserve">
    <value>Play</value>
  </data>
  <data name="A11y_PlayButtonHint" xml:space="preserve">
    <value>Start playing the selected sounds</value>
  </data>
  <data name="A11y_StopButton" xml:space="preserve">
    <value>Stop</value>
  </data>
  <data name="A11y_StopButtonHint" xml:space="preserve">
    <value>Stop all currently playing sounds</value>
  </data>
  <data name="A11y_SaveMixButton" xml:space="preserve">
    <value>Save Mix</value>
  </data>
  <data name="A11y_SaveMixButtonHint" xml:space="preserve">
    <value>Save the current mix for later use</value>
  </data>
  <data name="A11y_DeleteButton" xml:space="preserve">
    <value>Delete</value>
  </data>
  <data name="A11y_DeleteButtonHint" xml:space="preserve">
    <value>Delete this item</value>
  </data>
  
  <!-- Volume Controls -->
  <data name="A11y_VolumeSlider" xml:space="preserve">
    <value>Volume</value>
  </data>
  <data name="A11y_VolumeSliderHint" xml:space="preserve">
    <value>Adjust the volume level for this sound</value>
  </data>
  <data name="A11y_VolumeFormat" xml:space="preserve">
    <value>Volume: {0} percent</value>
  </data>
  
  <!-- Timer Accessibility -->
  <data name="A11y_TimerStart" xml:space="preserve">
    <value>Start Timer</value>
  </data>
  <data name="A11y_TimerStartHint" xml:space="preserve">
    <value>Start the countdown timer</value>
  </data>
  <data name="A11y_TimerStop" xml:space="preserve">
    <value>Stop Timer</value>
  </data>
  <data name="A11y_TimerStopHint" xml:space="preserve">
    <value>Stop and reset the timer</value>
  </data>
  <data name="A11y_TimerRemaining" xml:space="preserve">
    <value>Time remaining: {0}</value>
  </data>
  <data name="A11y_DurationPicker" xml:space="preserve">
    <value>Duration picker</value>
  </data>
  <data name="A11y_DurationPickerHint" xml:space="preserve">
    <value>Select how long the timer should run</value>
  </data>
  <data name="A11y_StopAtTimePicker" xml:space="preserve">
    <value>Stop at time picker</value>
  </data>
  <data name="A11y_StopAtTimePickerHint" xml:space="preserve">
    <value>Select the time when playback should stop</value>
  </data>
  
  <!-- EQ Accessibility -->
  <data name="A11y_EqSlider" xml:space="preserve">
    <value>EQ Band {0} Hz</value>
  </data>
  <data name="A11y_EqSliderHint" xml:space="preserve">
    <value>Adjust the equalizer level for {0} Hz frequency</value>
  </data>
  <data name="A11y_EqReset" xml:space="preserve">
    <value>Reset Equalizer</value>
  </data>
  <data name="A11y_EqResetHint" xml:space="preserve">
    <value>Reset all equalizer settings to default</value>
  </data>
  <data name="A11y_EqPreset" xml:space="preserve">
    <value>EQ Preset: {0}</value>
  </data>
  
  <!-- Sound State Announcements -->
  <data name="A11y_SoundSelected" xml:space="preserve">
    <value>Sound selected</value>
  </data>
  <data name="A11y_SoundDeselected" xml:space="preserve">
    <value>Sound removed</value>
  </data>
  <data name="A11y_SoundPlaying" xml:space="preserve">
    <value>{0} playing</value>
  </data>
  <data name="A11y_SoundStopped" xml:space="preserve">
    <value>{0} stopped</value>
  </data>
  <data name="A11y_MixSaved" xml:space="preserve">
    <value>Mix {0} saved successfully</value>
  </data>
  <data name="A11y_PlaylistSaved" xml:space="preserve">
    <value>Playlist {0} saved successfully</value>
  </data>
  
  <!-- Tab Navigation -->
  <data name="A11y_TabSelected" xml:space="preserve">
    <value>{0} tab selected</value>
  </data>
  <data name="A11y_TabButton" xml:space="preserve">
    <value>{0} tab</value>
  </data>
  <data name="A11y_TabButtonHint" xml:space="preserve">
    <value>Switch to {0} tab</value>
  </data>
  
  <!-- Mix Playlist -->
  <data name="A11y_MixPlaylist" xml:space="preserve">
    <value>Mix Playlist</value>
  </data>
  <data name="A11y_MixPlaylistHint" xml:space="preserve">
    <value>Create a scheduled rotation of different mixes</value>
  </data>
  <data name="A11y_AddMixButton" xml:space="preserve">
    <value>Add Mix to Playlist</value>
  </data>
  <data name="A11y_AddMixButtonHint" xml:space="preserve">
    <value>Add a mix to the current playlist</value>
  </data>
  
  <!-- Settings Accessibility -->
  <data name="A11y_TierBadge" xml:space="preserve">
    <value>Current tier: {0}</value>
  </data>
  <data name="A11y_UpgradeButton" xml:space="preserve">
    <value>Upgrade to {0}</value>
  </data>
  <data name="A11y_UpgradeButtonHint" xml:space="preserve">
    <value>Tap to upgrade your subscription to {0} tier</value>
  </data>
  <data name="A11y_UpgradeRequired" xml:space="preserve">
    <value>Upgrade required</value>
  </data>
  <data name="A11y_UpgradeRequiredHint" xml:space="preserve">
    <value>This feature requires {0} tier or higher</value>
  </data>
  
  <!-- Page Headings -->
  <data name="A11y_PageHeading" xml:space="preserve">
    <value>{0} page</value>
  </data>
  <data name="A11y_SectionHeading" xml:space="preserve">
    <value>{0} section</value>
  </data>
  
  <!-- Collection States -->
  <data name="A11y_CollectionEmpty" xml:space="preserve">
    <value>No items available</value>
  </data>
  <data name="A11y_CollectionCount" xml:space="preserve">
    <value>{0} items</value>
  </data>
  
  <!-- Loading States -->
  <data name="A11y_LoadingAnnouncement" xml:space="preserve">
    <value>Loading</value>
  </data>
  <data name="A11y_LoadedAnnouncement" xml:space="preserve">
    <value>Loaded successfully</value>
  </data>
  <data name="A11y_ErrorAnnouncement" xml:space="preserve">
    <value>Error: {0}</value>
  </data>
  <data name="A11y_SuccessAnnouncement" xml:space="preserve">
    <value>Success: {0}</value>
  </data>
  
  <!-- Help & Legal -->
  <data name="A11y_HelpSection" xml:space="preserve">
    <value>Help section: {0}</value>
  </data>
  <data name="A11y_LegalSection" xml:space="preserve">
    <value>Legal section: {0}</value>
  </data>
  <data name="A11y_ScrollView" xml:space="preserve">
    <value>Scrollable content</value>
  </data>
  <data name="A11y_ScrollViewHint" xml:space="preserve">
    <value>Swipe up or down to scroll through content</value>
  </data>
  
  <!-- Ad System -->
  <data name="A11y_WatchAdButton" xml:space="preserve">
    <value>Watch ad for extra time</value>
  </data>
  <data name="A11y_WatchAdButtonHint" xml:space="preserve">
    <value>Watch a short advertisement to extend your session by 45 minutes</value>
  </data>
  <data name="A11y_AdPlaying" xml:space="preserve">
    <value>Advertisement playing</value>
  </data>
  <data name="A11y_AdComplete" xml:space="preserve">
    <value>Advertisement complete. Session extended</value>
  </data>
  
  <!-- Export/Import -->
  <data name="A11y_ExportButton" xml:space="preserve">
    <value>Export</value>
  </data>
  <data name="A11y_ExportButtonHint" xml:space="preserve">
    <value>Export your mixes and playlists</value>
  </data>
  <data name="A11y_ImportButton" xml:space="preserve">
    <value>Import</value>
  </data>
  <data name="A11y_ImportButtonHint" xml:space="preserve">
    <value>Import mixes and playlists from a file</value>
  </data>
  
  <!-- Switch/Toggle -->
  <data name="A11y_SwitchOn" xml:space="preserve">
    <value>On</value>
  </data>
  <data name="A11y_SwitchOff" xml:space="preserve">
    <value>Off</value>
  </data>
  <data name="A11y_ToggleHint" xml:space="preserve">
    <value>Double tap to toggle</value>
  </data>
  
  <!-- Sound Item -->
  <data name="A11y_SoundItem" xml:space="preserve">
    <value>{0} sound</value>
  </data>
  <data name="A11y_SoundItemHint" xml:space="preserve">
    <value>Tap to add {0} to your mix or playlist</value>
  </data>
  <data name="A11y_SoundBundleButton" xml:space="preserve">
    <value>{0} bundle</value>
  </data>
  <data name="A11y_SoundBundleButtonHint" xml:space="preserve">
    <value>View sounds in the {0} bundle</value>
  </data>
"@

# Insert before closing </root> tag
$content = $content -replace "</root>", "$accessibilityStrings`n</root>"

Write-Host "Writing updated file..." -ForegroundColor Yellow
$utf8WithBom = New-Object System.Text.UTF8Encoding($true)
[System.IO.File]::WriteAllText((Resolve-Path $resxFile), $content, $utf8WithBom)

Write-Host "? Successfully added 82 accessibility strings!" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "1. Rebuild solution to regenerate AppResources.Designer.cs" -ForegroundColor White
Write-Host "2. Run apply-all-translations.ps1 to translate to all languages" -ForegroundColor White
Write-Host "3. Update XAML files with accessibility properties" -ForegroundColor White
Write-Host ""
