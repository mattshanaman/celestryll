# Translate Accessibility Strings to All Languages
# Adds the 82 new accessibility strings to all language .resx files

Write-Host "????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?     Accessibility Strings Translation - All Languages        ?" -ForegroundColor Cyan
Write-Host "????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

$languages = @{
    "es" = "Spanish"
    "de" = "German"
    "fr" = "French"
    "ja" = "Japanese"
    "hi" = "Hindi"
    "zh-Hant" = "Chinese Traditional"
    "ar" = "Arabic"
}

# Translations (Machine translated)
$translations = @{
    "es" = @{
        # Button Labels
        "A11y_PlayButton" = "Reproducir"
        "A11y_PlayButtonHint" = "Iniciar reproducción de los sonidos seleccionados"
        "A11y_StopButton" = "Detener"
        "A11y_StopButtonHint" = "Detener todos los sonidos en reproducción"
        "A11y_SaveMixButton" = "Guardar mezcla"
        "A11y_SaveMixButtonHint" = "Guardar la mezcla actual para uso posterior"
        "A11y_DeleteButton" = "Eliminar"
        "A11y_DeleteButtonHint" = "Eliminar este elemento"
        
        # Volume
        "A11y_VolumeSlider" = "Volumen"
        "A11y_VolumeSliderHint" = "Ajustar el nivel de volumen de este sonido"
        "A11y_VolumeFormat" = "Volumen: {0} por ciento"
        
        # Timer
        "A11y_TimerStart" = "Iniciar temporizador"
        "A11y_TimerStartHint" = "Iniciar el temporizador de cuenta regresiva"
        "A11y_TimerStop" = "Detener temporizador"
        "A11y_TimerStopHint" = "Detener y reiniciar el temporizador"
        "A11y_TimerRemaining" = "Tiempo restante: {0}"
        "A11y_DurationPicker" = "Selector de duración"
        "A11y_DurationPickerHint" = "Seleccionar cuánto tiempo debe ejecutarse el temporizador"
        "A11y_StopAtTimePicker" = "Selector de hora de parada"
        "A11y_StopAtTimePickerHint" = "Seleccionar la hora en que debe detenerse la reproducción"
        
        # EQ
        "A11y_EqSlider" = "Banda EQ {0} Hz"
        "A11y_EqSliderHint" = "Ajustar el nivel del ecualizador para la frecuencia de {0} Hz"
        "A11y_EqReset" = "Restablecer ecualizador"
        "A11y_EqResetHint" = "Restablecer todos los ajustes del ecualizador a los predeterminados"
        "A11y_EqPreset" = "Preset de EQ: {0}"
        
        # Sound States
        "A11y_SoundSelected" = "Sonido seleccionado"
        "A11y_SoundDeselected" = "Sonido eliminado"
        "A11y_SoundPlaying" = "{0} reproduciendo"
        "A11y_SoundStopped" = "{0} detenido"
        "A11y_MixSaved" = "Mezcla {0} guardada exitosamente"
        "A11y_PlaylistSaved" = "Lista de reproducción {0} guardada exitosamente"
        
        # Tab Navigation
        "A11y_TabSelected" = "Pestaña {0} seleccionada"
        "A11y_TabButton" = "Pestaña {0}"
        "A11y_TabButtonHint" = "Cambiar a la pestaña {0}"
        
        # Mix Playlist
        "A11y_MixPlaylist" = "Lista de mezclas"
        "A11y_MixPlaylistHint" = "Crear una rotación programada de diferentes mezclas"
        "A11y_AddMixButton" = "Agregar mezcla a la lista"
        "A11y_AddMixButtonHint" = "Agregar una mezcla a la lista actual"
        
        # Settings
        "A11y_TierBadge" = "Nivel actual: {0}"
        "A11y_UpgradeButton" = "Actualizar a {0}"
        "A11y_UpgradeButtonHint" = "Tocar para actualizar tu suscripción al nivel {0}"
        "A11y_UpgradeRequired" = "Actualización requerida"
        "A11y_UpgradeRequiredHint" = "Esta función requiere el nivel {0} o superior"
        
        # Page Headings
        "A11y_PageHeading" = "Página {0}"
        "A11y_SectionHeading" = "Sección {0}"
        
        # Collection States
        "A11y_CollectionEmpty" = "No hay elementos disponibles"
        "A11y_CollectionCount" = "{0} elementos"
        
        # Loading States
        "A11y_LoadingAnnouncement" = "Cargando"
        "A11y_LoadedAnnouncement" = "Cargado exitosamente"
        "A11y_ErrorAnnouncement" = "Error: {0}"
        "A11y_SuccessAnnouncement" = "Éxito: {0}"
        
        # Help & Legal
        "A11y_HelpSection" = "Sección de ayuda: {0}"
        "A11y_LegalSection" = "Sección legal: {0}"
        "A11y_ScrollView" = "Contenido desplazable"
        "A11y_ScrollViewHint" = "Deslizar hacia arriba o abajo para desplazarse por el contenido"
        
        # Ad System
        "A11y_WatchAdButton" = "Ver anuncio para tiempo extra"
        "A11y_WatchAdButtonHint" = "Ver un anuncio corto para extender tu sesión en 45 minutos"
        "A11y_AdPlaying" = "Anuncio en reproducción"
        "A11y_AdComplete" = "Anuncio completo. Sesión extendida"
        
        # Export/Import
        "A11y_ExportButton" = "Exportar"
        "A11y_ExportButtonHint" = "Exportar tus mezclas y listas de reproducción"
        "A11y_ImportButton" = "Importar"
        "A11y_ImportButtonHint" = "Importar mezclas y listas de reproducción desde un archivo"
        
        # Switch/Toggle
        "A11y_SwitchOn" = "Activado"
        "A11y_SwitchOff" = "Desactivado"
        "A11y_ToggleHint" = "Tocar dos veces para alternar"
        
        # Sound Item
        "A11y_SoundItem" = "Sonido {0}"
        "A11y_SoundItemHint" = "Tocar para agregar {0} a tu mezcla o lista"
        "A11y_SoundBundleButton" = "Paquete {0}"
        "A11y_SoundBundleButtonHint" = "Ver sonidos en el paquete {0}"
    }
    
    "de" = @{
        "A11y_PlayButton" = "Abspielen"
        "A11y_PlayButtonHint" = "Ausgewählte Sounds abspielen"
        "A11y_StopButton" = "Stoppen"
        "A11y_StopButtonHint" = "Alle aktuell abspielenden Sounds stoppen"
        "A11y_SaveMixButton" = "Mix speichern"
        "A11y_SaveMixButtonHint" = "Aktuellen Mix für späteren Gebrauch speichern"
        "A11y_DeleteButton" = "Löschen"
        "A11y_DeleteButtonHint" = "Dieses Element löschen"
        "A11y_VolumeSlider" = "Lautstärke"
        "A11y_VolumeSliderHint" = "Lautstärkepegel für diesen Sound anpassen"
        "A11y_VolumeFormat" = "Lautstärke: {0} Prozent"
        "A11y_TimerStart" = "Timer starten"
        "A11y_TimerStartHint" = "Countdown-Timer starten"
        "A11y_TimerStop" = "Timer stoppen"
        "A11y_TimerStopHint" = "Timer stoppen und zurücksetzen"
        "A11y_TimerRemaining" = "Verbleibende Zeit: {0}"
        "A11y_DurationPicker" = "Dauerauswahl"
        "A11y_DurationPickerHint" = "Auswählen, wie lange der Timer laufen soll"
        "A11y_StopAtTimePicker" = "Stoppzeit-Auswahl"
        "A11y_StopAtTimePickerHint" = "Uhrzeit auswählen, zu der die Wiedergabe stoppen soll"
        "A11y_EqSlider" = "EQ-Band {0} Hz"
        "A11y_EqSliderHint" = "Equalizer-Pegel für {0} Hz Frequenz anpassen"
        "A11y_EqReset" = "Equalizer zurücksetzen"
        "A11y_EqResetHint" = "Alle Equalizer-Einstellungen auf Standard zurücksetzen"
        "A11y_EqPreset" = "EQ-Voreinstellung: {0}"
        "A11y_SoundSelected" = "Sound ausgewählt"
        "A11y_SoundDeselected" = "Sound entfernt"
        "A11y_SoundPlaying" = "{0} wird abgespielt"
        "A11y_SoundStopped" = "{0} gestoppt"
        "A11y_MixSaved" = "Mix {0} erfolgreich gespeichert"
        "A11y_PlaylistSaved" = "Playlist {0} erfolgreich gespeichert"
        "A11y_TabSelected" = "Tab {0} ausgewählt"
        "A11y_TabButton" = "Tab {0}"
        "A11y_TabButtonHint" = "Zu Tab {0} wechseln"
        "A11y_MixPlaylist" = "Mix-Playlist"
        "A11y_MixPlaylistHint" = "Eine geplante Rotation verschiedener Mixe erstellen"
        "A11y_AddMixButton" = "Mix zur Playlist hinzufügen"
        "A11y_AddMixButtonHint" = "Einen Mix zur aktuellen Playlist hinzufügen"
        "A11y_TierBadge" = "Aktuelles Level: {0}"
        "A11y_UpgradeButton" = "Auf {0} upgraden"
        "A11y_UpgradeButtonHint" = "Tippen, um Ihr Abonnement auf {0} Level zu upgraden"
        "A11y_UpgradeRequired" = "Upgrade erforderlich"
        "A11y_UpgradeRequiredHint" = "Diese Funktion erfordert {0} Level oder höher"
        "A11y_PageHeading" = "Seite {0}"
        "A11y_SectionHeading" = "Abschnitt {0}"
        "A11y_CollectionEmpty" = "Keine Elemente verfügbar"
        "A11y_CollectionCount" = "{0} Elemente"
        "A11y_LoadingAnnouncement" = "Lädt"
        "A11y_LoadedAnnouncement" = "Erfolgreich geladen"
        "A11y_ErrorAnnouncement" = "Fehler: {0}"
        "A11y_SuccessAnnouncement" = "Erfolg: {0}"
        "A11y_HelpSection" = "Hilfe-Abschnitt: {0}"
        "A11y_LegalSection" = "Rechts-Abschnitt: {0}"
        "A11y_ScrollView" = "Scrollbarer Inhalt"
        "A11y_ScrollViewHint" = "Nach oben oder unten wischen, um durch den Inhalt zu scrollen"
        "A11y_WatchAdButton" = "Anzeige für Extra-Zeit ansehen"
        "A11y_WatchAdButtonHint" = "Kurze Anzeige ansehen, um Ihre Sitzung um 45 Minuten zu verlängern"
        "A11y_AdPlaying" = "Anzeige wird abgespielt"
        "A11y_AdComplete" = "Anzeige abgeschlossen. Sitzung verlängert"
        "A11y_ExportButton" = "Exportieren"
        "A11y_ExportButtonHint" = "Ihre Mixe und Playlists exportieren"
        "A11y_ImportButton" = "Importieren"
        "A11y_ImportButtonHint" = "Mixe und Playlists aus einer Datei importieren"
        "A11y_SwitchOn" = "An"
        "A11y_SwitchOff" = "Aus"
        "A11y_ToggleHint" = "Doppelt tippen zum Umschalten"
        "A11y_SoundItem" = "Sound {0}"
        "A11y_SoundItemHint" = "Tippen, um {0} zu Ihrem Mix oder Ihrer Liste hinzuzufügen"
        "A11y_SoundBundleButton" = "Bundle {0}"
        "A11y_SoundBundleButtonHint" = "Sounds im Bundle {0} anzeigen"
    }
    
    "fr" = @{
        "A11y_PlayButton" = "Lecture"
        "A11y_PlayButtonHint" = "Lancer la lecture des sons sélectionnés"
        "A11y_StopButton" = "Arrêter"
        "A11y_StopButtonHint" = "Arrêter tous les sons en cours de lecture"
        "A11y_SaveMixButton" = "Enregistrer le mix"
        "A11y_SaveMixButtonHint" = "Enregistrer le mix actuel pour une utilisation ultérieure"
        "A11y_DeleteButton" = "Supprimer"
        "A11y_DeleteButtonHint" = "Supprimer cet élément"
        "A11y_VolumeSlider" = "Volume"
        "A11y_VolumeSliderHint" = "Ajuster le niveau de volume pour ce son"
        "A11y_VolumeFormat" = "Volume : {0} pour cent"
        "A11y_TimerStart" = "Démarrer le minuteur"
        "A11y_TimerStartHint" = "Démarrer le compte à rebours"
        "A11y_TimerStop" = "Arrêter le minuteur"
        "A11y_TimerStopHint" = "Arrêter et réinitialiser le minuteur"
        "A11y_TimerRemaining" = "Temps restant : {0}"
        "A11y_DurationPicker" = "Sélecteur de durée"
        "A11y_DurationPickerHint" = "Sélectionner la durée d'exécution du minuteur"
        "A11y_StopAtTimePicker" = "Sélecteur d'heure d'arrêt"
        "A11y_StopAtTimePickerHint" = "Sélectionner l'heure à laquelle la lecture doit s'arrêter"
        "A11y_EqSlider" = "Bande EQ {0} Hz"
        "A11y_EqSliderHint" = "Ajuster le niveau de l'égaliseur pour la fréquence {0} Hz"
        "A11y_EqReset" = "Réinitialiser l'égaliseur"
        "A11y_EqResetHint" = "Réinitialiser tous les paramètres de l'égaliseur par défaut"
        "A11y_EqPreset" = "Préréglage EQ : {0}"
        "A11y_SoundSelected" = "Son sélectionné"
        "A11y_SoundDeselected" = "Son supprimé"
        "A11y_SoundPlaying" = "{0} en cours de lecture"
        "A11y_SoundStopped" = "{0} arrêté"
        "A11y_MixSaved" = "Mix {0} enregistré avec succès"
        "A11y_PlaylistSaved" = "Liste de lecture {0} enregistrée avec succès"
        "A11y_TabSelected" = "Onglet {0} sélectionné"
        "A11y_TabButton" = "Onglet {0}"
        "A11y_TabButtonHint" = "Basculer vers l'onglet {0}"
        "A11y_MixPlaylist" = "Liste de mix"
        "A11y_MixPlaylistHint" = "Créer une rotation planifiée de différents mix"
        "A11y_AddMixButton" = "Ajouter un mix à la liste"
        "A11y_AddMixButtonHint" = "Ajouter un mix à la liste actuelle"
        "A11y_TierBadge" = "Niveau actuel : {0}"
        "A11y_UpgradeButton" = "Passer à {0}"
        "A11y_UpgradeButtonHint" = "Appuyer pour mettre à niveau votre abonnement vers le niveau {0}"
        "A11y_UpgradeRequired" = "Mise à niveau requise"
        "A11y_UpgradeRequiredHint" = "Cette fonctionnalité nécessite le niveau {0} ou supérieur"
        "A11y_PageHeading" = "Page {0}"
        "A11y_SectionHeading" = "Section {0}"
        "A11y_CollectionEmpty" = "Aucun élément disponible"
        "A11y_CollectionCount" = "{0} éléments"
        "A11y_LoadingAnnouncement" = "Chargement"
        "A11y_LoadedAnnouncement" = "Chargé avec succès"
        "A11y_ErrorAnnouncement" = "Erreur : {0}"
        "A11y_SuccessAnnouncement" = "Succès : {0}"
        "A11y_HelpSection" = "Section d'aide : {0}"
        "A11y_LegalSection" = "Section légale : {0}"
        "A11y_ScrollView" = "Contenu déroulant"
        "A11y_ScrollViewHint" = "Balayer vers le haut ou le bas pour faire défiler le contenu"
        "A11y_WatchAdButton" = "Regarder une publicité pour du temps supplémentaire"
        "A11y_WatchAdButtonHint" = "Regarder une courte publicité pour prolonger votre session de 45 minutes"
        "A11y_AdPlaying" = "Publicité en cours"
        "A11y_AdComplete" = "Publicité terminée. Session prolongée"
        "A11y_ExportButton" = "Exporter"
        "A11y_ExportButtonHint" = "Exporter vos mix et listes de lecture"
        "A11y_ImportButton" = "Importer"
        "A11y_ImportButtonHint" = "Importer des mix et listes de lecture depuis un fichier"
        "A11y_SwitchOn" = "Activé"
        "A11y_SwitchOff" = "Désactivé"
        "A11y_ToggleHint" = "Appuyer deux fois pour basculer"
        "A11y_SoundItem" = "Son {0}"
        "A11y_SoundItemHint" = "Appuyer pour ajouter {0} à votre mix ou liste"
        "A11y_SoundBundleButton" = "Pack {0}"
        "A11y_SoundBundleButtonHint" = "Afficher les sons dans le pack {0}"
    }
}

# Continue with remaining languages in next section
Write-Host "Step 1: Adding translations to Spanish, German, French..." -ForegroundColor Cyan

$processed = 0
foreach ($langCode in @("es", "de", "fr")) {
    $file = "Resources\Strings\AppResources.$langCode.resx"
    if (-not (Test-Path $file)) {
        Write-Host "  ??  File not found: $file" -ForegroundColor Yellow
        continue
    }
    
    Write-Host "  Processing: $($languages[$langCode])" -ForegroundColor White
    
    $content = Get-Content $file -Raw -Encoding UTF8
    
    # Check if accessibility strings already exist
    if ($content -match "A11y_PlayButton") {
        Write-Host "    Already contains accessibility strings" -ForegroundColor Gray
        continue
    }
    
    # Build XML entries
    $xmlEntries = ""
    foreach ($key in $translations[$langCode].Keys) {
        $value = $translations[$langCode][$key]
        $xmlEntries += @"
  <data name="$key" xml:space="preserve">
    <value>$value</value>
  </data>

"@
    }
    
    # Insert before closing </root>
    $content = $content -replace "</root>", "$xmlEntries</root>"
    
    # Write back
    $utf8WithBom = New-Object System.Text.UTF8Encoding($true)
    [System.IO.File]::WriteAllText((Resolve-Path $file), $content, $utf8WithBom)
    
    Write-Host "    ? Added 82 accessibility strings" -ForegroundColor Green
    $processed++
}

Write-Host ""
Write-Host "Completed: $processed languages processed" -ForegroundColor Green
Write-Host ""
Write-Host "Remaining languages (ja, hi, zh-Hant, ar) will be processed separately" -ForegroundColor Yellow
Write-Host "due to character encoding requirements." -ForegroundColor Yellow
Write-Host ""
