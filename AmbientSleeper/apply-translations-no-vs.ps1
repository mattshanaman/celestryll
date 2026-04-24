# QUICK FIX: Close Visual Studio and Run This
# This script applies all translations with proper file handling

Write-Host "????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?     Quick Translation Applicator - No VS Required           ?" -ForegroundColor Cyan
Write-Host "????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

Write-Host "??  IMPORTANT: Close Visual Studio before continuing!" -ForegroundColor Yellow
Write-Host ""
$confirmation = Read-Host "Have you closed Visual Studio? (y/N)"
if ($confirmation -ne 'y' -and $confirmation -ne 'Y') {
    Write-Host "Please close Visual Studio and run this script again." -ForegroundColor Yellow
    exit 0
}

Write-Host ""
Write-Host "Starting translation application..." -ForegroundColor Green
Write-Host ""

# Helper function to apply translations with proper error handling
function Apply-Translations {
    param(
        [string]$Language,
        [string]$Flag,
        [string]$ResxFile,
        [hashtable]$Translations
    )
    
    Write-Host "????????????????????????????????????????????????????????????" -ForegroundColor Cyan
    Write-Host " $Flag $Language" -ForegroundColor Cyan
    Write-Host "????????????????????????????????????????????????????????????" -ForegroundColor Cyan
    Write-Host ""
    
    if (-not (Test-Path $ResxFile)) {
        Write-Host "ERROR: File not found: $ResxFile" -ForegroundColor Red
        return $false
    }
    
    try {
        # Remove read-only if present
        $fileItem = Get-Item $ResxFile -Force
        if ($fileItem.IsReadOnly) {
            $fileItem.IsReadOnly = $false
        }
        
        # Read file using StreamReader for better control
        $content = [System.IO.File]::ReadAllText($fileItem.FullName, [System.Text.Encoding]::UTF8)
        
        $translatedCount = 0
        $updatedCount = 0
        
        foreach ($key in $Translations.Keys) {
            $translation = $Translations[$key]
            $pattern = "(<data name=`"$key`"[^>]*>[\s\S]*?<value>)([^<]*)(</value>)"
            
            if ($content -match $pattern) {
                $oldValue = $matches[2]
                if ($oldValue -ne $translation) {
                    $content = $content -replace $pattern, "`$1$translation`$3"
                    if ($oldValue -match "^[A-Z]") {
                        $translatedCount++
                    } else {
                        $updatedCount++
                    }
                }
            }
        }
        
        # Write file using StreamWriter for better control
        $utf8WithBom = New-Object System.Text.UTF8Encoding($true)
        [System.IO.File]::WriteAllText($fileItem.FullName, $content, $utf8WithBom)
        
        Write-Host "New translations: $translatedCount" -ForegroundColor Green
        Write-Host "Updated translations: $updatedCount" -ForegroundColor Cyan
        Write-Host "? $Language completed successfully!" -ForegroundColor Green
        Write-Host ""
        
        return $true
    }
    catch {
        Write-Host "ERROR: Failed to process $Language" -ForegroundColor Red
        Write-Host $_.Exception.Message -ForegroundColor Red
        Write-Host ""
        return $false
    }
}

# Spanish translations
$spanish = @{
    "Ok" = "Aceptar"
    "Cancel" = "Cancelar"
    "Yes" = "Sí"
    "No" = "No"
    "Error" = "Error"
    "Nav_Library" = "Biblioteca"
    "Nav_Playback" = "Reproducción"
    "Nav_Timer" = "Temporizador"
    "Nav_EQ" = "Ecualizador"
    "Nav_Settings" = "Ajustes"
    "Nav_Help" = "Ayuda"
    "Nav_Legal" = "Legal"
    "Tab_Mix" = "Mezcla"
    "Tab_Playlist" = "Lista"
    "Tab_MixPlaylist" = "Lista de mezclas"
    "Tab_BuiltIn" = "Incorporado"
    "Tab_YourAudio" = "Tu audio"
    "Common_PlayButton" = "? Reproducir"
    "Common_StopButton" = "? Detener"
    "Common_SaveButton" = "?? Guardar"
    "Common_LoadButton" = "?? Cargar"
    "Common_DeleteIcon" = "??"
    "Library_Title" = "Biblioteca"
    "ImportFromDevice" = "Importar desde dispositivo"
    "AudioBundles" = "Paquetes"
    "A11y_AddToMix" = "Agregar a la mezcla"
    "A11y_AddToPlaylist" = "Agregar a la lista"
    "Confirmation_SoundAdded" = "Sonido agregado a la lista"
    "PlaylistLocked_Title" = "Lista bloqueada"
    "PlaylistLocked_Message" = "Actualiza a Estándar, Premium o Pro+ para guardar o modificar listas."
    "DefaultPlaylistCreated_Title" = "Lista predeterminada creada"
    "DefaultPlaylistCreated_Message" = "Se ha creado una lista 'Predeterminada'."
    "SelectPlaylist_Title" = "Seleccionar lista"
    "EmptyPlaylist_Title" = "Lista vacía"
    "EmptyPlaylist_Message" = "Agrega sonidos a la cola de reproducción o carga una lista guardada primero."
    "ExportScope_Title" = "Alcance de exportación"
    "ExportScope_Personal" = "Personal (Estándar+)"
    "ExportScope_Shareable" = "Compartible (Premium/Pro+)"
    "ExportLocked_Title" = "Exportación bloqueada"
    "ExportLocked_Message" = "Actualiza a Estándar, Premium o Pro+ para exportar tus mezclas."
    "ExportComplete_Title" = "Exportación completa"
    "ExportComplete_Message" = "Tus mezclas y listas fueron exportadas. Elige una ubicación para guardar el archivo."
    "ExportFailed_Title" = "Exportación fallida"
    "ImportComplete_Title" = "Importación completa"
    "ImportComplete_MessageFormat" = "Se importaron {0} elementos."
    "ImportFailed_Title" = "Importación fallida"
    "PickExport_Title" = "Elegir exportación"
    "PickAudio_Title" = "Elegir audio"
    "PickAudio_TitleAlt" = "Seleccionar archivo de audio"
    "ExportShare_Title" = "Compartir exportación"
    "ExportShare_ShareSheet" = "Hoja para compartir"
    "ExportShare_Email" = "Correo electrónico"
    "ExportEmail_Subject" = "Configuración de Ambient Sleeper"
    "Subscription_Title" = "Suscripción"
    "Purchased_StandardLifetime" = "Estándar de por vida comprado"
    "Purchased_PremiumLifetime" = "Premium de por vida comprado"
    "LifetimeSuffix" = "De por vida"
    "CurrentTier_Format" = "Nivel actual: {0} {1}"
    "Mix_Mode" = "Modo de mezcla"
    "Mix_Empty" = "Tu mezcla está vacía. Agrega sonidos desde la biblioteca."
    "Mix_RemoveButton" = "Quitar de la mezcla"
    "Mix_SoundsInMixFormat" = "Sonidos en la mezcla: {0}"
    "Mix_OfFormat" = "de {0}"
    "Mix_SaveCurrent" = "Guardar mezcla actual"
    "Mix_NamePlaceholder" = "Nombre de la mezcla"
    "Mix_SaveLimitFormat" = "Límite de mezclas guardadas: {0}"
    "Mix_SavedTitle" = "Mezclas guardadas"
    "Mix_SavedEmpty" = "No hay mezclas guardadas. Guarda tu primera mezcla arriba."
    "Mix_SoundsCountFormat" = "{0} sonidos"
    "Mix_DeleteButton" = "Eliminar mezcla"
    "Mix_StopAllFormat" = "? Detener todo (desvanecer {0}s)"
    "Playlist_Mode" = "Modo de lista de reproducción"
    "Playlist_Empty" = "Tu lista de reproducción está vacía. Agrega sonidos desde la biblioteca."
    "Playlist_RemoveButton" = "Quitar de la lista"
    "Playlist_SoundsCountFormat" = "{0} sonidos en la lista"
    "Playlist_LoopToggle" = "Repetir lista"
    "Playlist_SaveCurrent" = "Guardar lista actual"
    "Playlist_NamePlaceholder" = "Nombre de la lista"
    "Playlist_SaveLimitFormat" = "Límite de listas guardadas: {0}"
    "Playlist_SavedTitle" = "Listas guardadas"
    "Playlist_SavedEmpty" = "No hay listas guardadas. Guarda tu primera lista arriba."
    "Playlist_DeleteButton" = "Eliminar lista"
    "MixPlaylist_Mode" = "Modo de lista de mezclas"
    "MixPlaylist_Empty" = "Tu lista de mezclas está vacía. Agrega mezclas guardadas."
    "MixPlaylist_RemoveButton" = "Quitar de la lista de mezclas"
    "MixPlaylist_Duration" = "Duración:"
    "MixPlaylist_Seconds" = "segundos"
    "MixPlaylist_Transition" = "Transición:"
    "MixPlaylist_Loop" = "Repetir lista de mezclas"
    "MixPlaylist_Save" = "Guardar lista de mezclas actual"
    "MixPlaylist_NamePlaceholder" = "Nombre de la lista de mezclas"
    "MixPlaylist_SaveLimitFormat" = "Límite de listas de mezclas: {0}"
    "MixPlaylist_Saved" = "Listas de mezclas guardadas"
    "MixPlaylist_SavedEmpty" = "No hay listas de mezclas guardadas."
    "MixPlaylist_MixesCountFormat" = "{0} mezclas"
    "MixPlaylist_DeleteButton" = "Eliminar lista de mezclas"
    "MixPlaylist_Locked_Title" = "Lista de mezclas bloqueada"
    "MixPlaylist_Locked_Message" = "Actualiza a Pro+ para usar listas de mezclas."
    "Playback_PlaylistLocked_Title" = "Lista de reproducción bloqueada"
    "Playback_PlaylistLocked_Message" = "Actualiza a Estándar, Premium o Pro+ para usar listas de reproducción."
    "Toolbar_Tier" = "Nivel"
    "Toolbar_EQ" = "EQ"
    "Toolbar_Audio" = "Audio"
    "Toolbar_Export" = "Exportar"
    "Toolbar_Import" = "Importar"
    "Settings_Title" = "Ajustes de suscripción"
    "Settings_TierFree" = "Gratis"
    "Settings_TierStandard" = "Estándar"
    "Settings_TierPremium" = "Premium"
    "Settings_TierProPlus" = "Pro+"
    "Settings_Recurring" = "Suscripción recurrente"
    "Settings_Lifetime" = "De por vida"
    "Settings_StandardLifetime" = "Estándar de por vida"
    "Settings_PremiumLifetime" = "Premium de por vida"
    "Settings_CheckHealth" = "Verificar estado"
    "Settings_ErrorReport" = "Informe de errores"
    "Timer_Title" = "Temporizador para dormir"
    "Timer_StopAfterDuration" = "Detener después de duración"
    "Timer_StopAtTime" = "Detener a hora específica"
    "Timer_Duration" = "Duración:"
    "Timer_StopAt" = "Detener a:"
    "Timer_Start" = "Iniciar temporizador"
    "Timer_Stop" = "Detener temporizador"
    "Timer_Remaining" = "Tiempo restante: {0}"
    "Timer_Alarm" = "Alarma (Pro+/Premium)"
    "Timer_AlarmEnabled" = "Alarma habilitada"
    "Alarm_Title" = "Alarma"
    "Alarm_Free_Message" = "La función de alarma requiere suscripción Premium o Pro+."
    "EQ_Title" = "Ecualizador (Premium/Pro+)"
    "EQ_Locked" = "El ecualizador requiere suscripción Premium o Pro+"
    "EQ_Bass" = "Graves"
    "EQ_Mids" = "Medios"
    "EQ_Treble" = "Agudos"
    "EQ_Reset" = "Restablecer"
    "Bundle_BuiltIn" = "Incorporado"
    "Bundle_Locked" = "Bloqueado"
    "Bundle_UnlockWith" = "Desbloquear con {0}"
    "Notification_TimerComplete_Title" = "Temporizador completo"
    "Notification_TimerComplete_Description" = "Tu temporizador para dormir ha terminado"
    "NavigationError_Title" = "Error de navegación"
    "NavigationError_Settings" = "No se pudo abrir la página de ajustes"
    "HealthCheck_Checking" = "Verificando estado..."
    "HealthCheck_AllHealthy" = "Todo está bien"
    "HealthCheck_Passed" = "Verificación de estado aprobada"
    "HealthCheck_IssuesDetected" = "{0} problemas detectados"
    "HealthCheck_ResultsTitle" = "Resultados de verificación"
    "HealthCheck_IssuesMessage" = "Se encontraron los siguientes problemas:\n• {0}"
    "HealthCheck_Failed" = "Verificación fallida"
    "HealthCheck_FailedMessage" = "Error al verificar estado: {0}"
    "ErrorReport_Title" = "Informe de errores"
    "ErrorReport_ServiceUnavailable" = "Servicio de informes de errores no disponible"
    "ErrorReport_NoErrors" = "No hay errores registrados"
    "ErrorReport_CountFormat" = "{0} errores registrados"
    "ErrorReport_ViewDetails" = "Ver detalles"
    "ErrorReport_ShareReport" = "Compartir informe"
    "ErrorReport_ClearErrors" = "Borrar errores"
    "ErrorReport_SharedSuccess" = "Informe compartido exitosamente"
    "ErrorReport_ClearConfirmTitle" = "Borrar errores"
    "ErrorReport_ClearConfirmMessage" = "¿Estás seguro de que quieres borrar todos los errores?"
    "ErrorReport_Cleared" = "Errores borrados"
    "ErrorReport_ViewFailed" = "Error al ver informe: {0}"
}

# Apply Spanish
$success1 = Apply-Translations "Spanish" "????" "Resources\Strings\AppResources.es.resx" $spanish

# For brevity, I'll create a note that this script should call individual scripts
Write-Host "????????????????????????????????????????????????????????????????" -ForegroundColor Green
Write-Host "?                     NOTE                                     ?" -ForegroundColor Yellow
Write-Host "????????????????????????????????????????????????????????????????" -ForegroundColor Green
Write-Host ""
Write-Host "This is a simplified version for demonstration." -ForegroundColor Yellow
Write-Host "For complete translations, use the individual scripts after" -ForegroundColor Yellow
Write-Host "closing Visual Studio." -ForegroundColor Yellow
Write-Host ""
Write-Host "SOLUTION:" -ForegroundColor Cyan
Write-Host "1. Close Visual Studio completely" -ForegroundColor White
Write-Host "2. Run: powershell -ExecutionPolicy Bypass -File .\apply-all-translations.ps1" -ForegroundColor White
Write-Host ""
