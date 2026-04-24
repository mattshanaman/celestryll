# Apply Spanish Translations to AppResources.es.resx
# This script replaces English values with Spanish translations
# IDEMPOTENT: Safe to re-run, will update existing translations

Write-Host "=================================" -ForegroundColor Cyan
Write-Host "Spanish Translation Applicator" -ForegroundColor Cyan
Write-Host "=================================" -ForegroundColor Cyan
Write-Host ""

$resxFile = "Resources\Strings\AppResources.es.resx"

if (-not (Test-Path $resxFile)) {
    Write-Host "ERROR: File not found: $resxFile" -ForegroundColor Red
    exit 1
}

Write-Host "Reading file: $resxFile" -ForegroundColor Green

# Read the file content
$content = Get-Content $resxFile -Raw -Encoding UTF8

# Define Spanish translations
$translations = @{
    # Common buttons
    "Ok" = "Aceptar"
    "Cancel" = "Cancelar"
    "Yes" = "Sí"
    "No" = "No"
    "Error" = "Error"
    
    # Navigation
    "Nav_Library" = "Biblioteca"
    "Nav_Playback" = "Reproducción"
    "Nav_Timer" = "Temporizador"
    "Nav_EQ" = "Ecualizador"
    "Nav_Settings" = "Ajustes"
    "Nav_Help" = "Ayuda"
    "Nav_Legal" = "Legal"
    
    # Tabs
    "Tab_Mix" = "Mezcla"
    "Tab_Playlist" = "Lista"
    "Tab_MixPlaylist" = "Lista de mezclas"
    "Tab_BuiltIn" = "Incorporado"
    "Tab_YourAudio" = "Tu audio"
    
    # Common buttons with icons
    "Common_PlayButton" = "? Reproducir"
    "Common_StopButton" = "? Detener"
    "Common_SaveButton" = "?? Guardar"
    "Common_LoadButton" = "?? Cargar"
    "Common_DeleteIcon" = "??"
    
    # Library
    "Library_Title" = "Biblioteca"
    "ImportFromDevice" = "Importar desde dispositivo"
    "AudioBundles" = "Paquetes"
    "A11y_AddToMix" = "Agregar a la mezcla"
    "A11y_AddToPlaylist" = "Agregar a la lista"
    "Confirmation_SoundAdded" = "Sonido agregado a la lista"
    
    # Playlist
    "PlaylistLocked_Title" = "Lista bloqueada"
    "PlaylistLocked_Message" = "Actualiza a Estándar, Premium o Pro+ para guardar o modificar listas."
    "DefaultPlaylistCreated_Title" = "Lista predeterminada creada"
    "DefaultPlaylistCreated_Message" = "Se ha creado una lista 'Predeterminada'."
    "SelectPlaylist_Title" = "Seleccionar lista"
    "EmptyPlaylist_Title" = "Lista vacía"
    "EmptyPlaylist_Message" = "Agrega sonidos a la cola de reproducción o carga una lista guardada primero."
    
    # Export/Import
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
    
    # Subscription
    "Subscription_Title" = "Suscripción"
    "Purchased_StandardLifetime" = "Estándar de por vida comprado"
    "Purchased_PremiumLifetime" = "Premium de por vida comprado"
    "LifetimeSuffix" = "De por vida"
    "CurrentTier_Format" = "Nivel actual: {0} {1}"
    
    # Mix Mode
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
    
    # Playlist Mode
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
    
    # Mix Playlist Mode
    "MixPlaylist_Mode" = "Modo de lista de mezclas"
    "MixPlaylist_Empty" = "Tu lista de mezclas está vacía. Agrega mezclas guardadas."
    "MixPlaylist_AddMix" = "Agregar mezcla"
    "MixPlaylist_Remove" = "Quitar"
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
    
    # Playback Locked
    "Playback_PlaylistLocked_Title" = "Lista de reproducción bloqueada"
    "Playback_PlaylistLocked_Message" = "Actualiza a Estándar, Premium o Pro+ para usar listas de reproducción."
    
    # Toolbar
    "Toolbar_Tier" = "Nivel"
    "Toolbar_EQ" = "EQ"
    "Toolbar_Audio" = "Audio"
    "Toolbar_Export" = "Exportar"
    "Toolbar_Import" = "Importar"
    
    # Settings
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
    
    # Timer
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
    
    # EQ
    "EQ_Title" = "Ecualizador (Premium/Pro+)"
    "EQ_Locked" = "El ecualizador requiere suscripción Premium o Pro+"
    "EQ_Bass" = "Graves"
    "EQ_Mids" = "Medios"
    "EQ_Treble" = "Agudos"
    "EQ_Reset" = "Restablecer"
    
    # Bundles
    "Bundle_BuiltIn" = "Incorporado"
    "Bundle_Locked" = "Bloqueado"
    "Bundle_UnlockWith" = "Desbloquear con {0}"
    
    # Notifications
    "Notification_TimerComplete_Title" = "Temporizador completo"
    "Notification_TimerComplete_Description" = "Tu temporizador para dormir ha terminado"
    
    # Navigation Errors
    "NavigationError_Title" = "Error de navegación"
    "NavigationError_Settings" = "No se pudo abrir la página de ajustes"
    
    # Health Check
    "HealthCheck_Checking" = "Verificando estado..."
    "HealthCheck_AllHealthy" = "Todo está bien"
    "HealthCheck_Passed" = "Verificación de estado aprobada"
    "HealthCheck_IssuesDetected" = "{0} problemas detectados"
    "HealthCheck_ResultsTitle" = "Resultados de verificación"
    "HealthCheck_IssuesMessage" = "Se encontraron los siguientes problemas:\n• {0}"
    "HealthCheck_Failed" = "Verificación fallida"
    "HealthCheck_FailedMessage" = "Error al verificar estado: {0}"
    
    # Error Reporting
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
    
    # === NEW TRANSLATIONS - 195 MISSING STRINGS ===
    
    # Ad System (14 strings)
    "Ad_TimeAlmostUp_Title" = "? Tiempo casi agotado"
    "Ad_TimeRemaining" = "{0} minutos restantes"
    "Ad_SessionExtended_Title" = "¡Sesión extendida!"
    "Ad_SessionExtended_Message" = "¡Has ganado 45 minutos más de escucha. Disfruta!"
    "Ad_SoundUnlocked_Title" = "¡Sonido desbloqueado!"
    "Ad_SoundUnlocked_Message" = "Puedes usar este sonido hasta que cierres la aplicación."
    "Ad_WatchForExtension_Title" = "Obtén 45 minutos más"
    "Ad_WatchToUnlock_Message" = "Mira un anuncio corto para desbloquear hasta que cierres la aplicación"
    "Ad_OrUpgrade" = "O actualiza a Premium para sesiones ilimitadas"
    "Ad_LoadingAd" = "Cargando anuncio..."
    "Ad_AdNotAvailable_Title" = "Anuncio no disponible"
    "Ad_AdNotAvailable_Message" = "No hay anuncio disponible ahora. Inténtalo de nuevo en un momento."
    "Ad_Type_Banner" = "Banner"
    "Ad_Type_Rewarded" = "Recompensado"
    
    # Playback Settings (23 strings)
    "PlaybackSettings_Title" = "Ajustes de reproducción"
    "PlaybackSettings_Description" = "Configurar tiempo de desvanecimiento e integración de alarma"
    "PlaybackSettings_FadeOutTitle" = "Duración de desvanecimiento"
    "PlaybackSettings_FadeOutDescription" = "Elige cuánto tiempo se desvanece el audio cuando termina el temporizador"
    "PlaybackSettings_SecondsFormat" = "{0} segundos"
    "PlaybackSettings_TierAllowsFormat" = "Tu nivel permite hasta {0} segundos"
    "PlaybackSettings_Off" = "Desactivado"
    "PlaybackSettings_AudioSettings" = "Ajustes de audio"
    "PlaybackSettings_EnableAlarm" = "Habilitar alarma"
    "PlaybackSettings_ChooseAlarmSound" = "Elegir sonido de alarma"
    "PlaybackSettings_SelectedAlarm" = "Alarma seleccionada:"
    "PlaybackSettings_NoneSelected" = "Ninguna seleccionada"
    "PlaybackSettings_AlarmDescription" = "Detener sonidos ambientales y reproducir una alarma cuando termine el temporizador"
    "PlaybackSettings_AlarmFreeNote" = "El nivel gratuito usa el sonido de alarma predeterminado de tu dispositivo."
    "PlaybackSettings_AlarmLocked_Title" = "?? Integración de alarma bloqueada"
    "PlaybackSettings_AlarmLocked_Message" = "Despierta con sonidos de alarma personalizados. Disponible en nivel Estándar y superior."
    "PlaybackSettings_TipsTitle" = "?? Consejos"
    "PlaybackSettings_Tip1" = "• Los desvanecimientos más largos crean una experiencia de despertar más suave"
    "PlaybackSettings_Tip2" = "• La integración de alarma requiere que la función de temporizador esté activa"
    "PlaybackSettings_Tip3" = "• Tu elección de sonido de alarma se guarda entre sesiones"
    
    # EQ Page (33 strings)
    "EQ_FlatButton" = "0 Plano"
    "EQ_ApplyButton" = "? Aplicar"
    "EQ_ResetButton" = "? Restablecer"
    "EQ_ChoosePreset" = "Elegir un preset"
    "EQ_QuickPresets" = "Presets rápidos"
    "EQ_PresetsList" = "Presets: Plano, Aumento de graves, Mejorar vocal, Agudos brillantes y más"
    "EQ_PresetsPremium" = "Premium"
    "EQ_PresetsProPlus" = "Pro+"
    "EQ_10BandParametric" = "EQ paramétrico de 10 bandas"
    "EQ_AdvancedDescription" = "Control avanzado con frecuencia, ganancia y Q (ancho de banda) para cada banda. Usa presets para configuración rápida."
    "EQ_ProPlusBadge" = "Pro+"
    "EQ_Locked_Message" = "Personaliza tu sonido con controles de frecuencia profesionales. Disponible en niveles Premium y Pro+."
    "EQ_BandFormat" = "Banda: {0:F0} Hz"
    "EQ_Q" = "Q (Ancho de banda):"
    "EQ_QWidth" = "Q (Anchura):"
    "EQ_Wide" = "Ancho"
    "EQ_Narrow" = "Estrecho"
    "EQ_MinFreq" = "20"
    "EQ_MinGain" = "-12"
    "EQ_MaxGain" = "+12"
    "EQ_Tip_Bass" = "• Graves (60-250 Hz): Aumenta para un sonido más profundo y rico"
    "EQ_Tip_Mids" = "• Medios (500-2k Hz): Ajusta para claridad y calidez"
    "EQ_Tip_SmallAdjustments" = "• Comienza con ajustes pequeños (±3 dB)"
    "EQ_Tip2" = "• Usa presets como punto de partida, luego ajusta finamente"
    "EQ_Tip3" = "• Evita aumentar demasiadas frecuencias (causa distorsión)"
    "EQ_Tip4" = "• Haz ajustes sutiles para mejores resultados"
    
    # Timer Page (17 strings)
    "Timer_AlarmLabel" = "Reproducir alarma cuando expire el temporizador"
    "Timer_FadeOutLabel" = "Duración de desvanecimiento (segundos):"
    "Timer_ModeLabel" = "Modo de temporizador"
    "Timer_DurationLabel" = "Duración:"
    "Timer_DurationDescription" = "Establece cuánto tiempo debe continuar la reproducción (ej., 30 minutos)"
    "Timer_TimeDescription" = "Establece la hora exacta en que debe detenerse la reproducción (ej., 11:00 PM)"
    "Timer_FadeOutDescription" = "Reducir gradualmente el volumen antes de detenerse"
    "Timer_Controls" = "Controles de temporizador"
    "Timer_TimerActive" = "?? Temporizador activo"
    "Timer_TimeRemaining" = "Tiempo restante: {0:hh\\:mm\\:ss}"
    "Timer_WillStopIn" = "La reproducción se detendrá en:"
    "Timer_WillFadeOut" = "El temporizador detendrá la reproducción y desvanecerá el audio cuando expire el tiempo"
    "Timer_Remove5Min" = "? Quitar 5 min"
    "Timer_StopAlarm" = "?? Detener alarma"
    "Timer_StopAlarmDescription" = "Detener el sonido de alarma si se está reproduciendo actualmente"
    "Timer_Tip1" = "• Usa el modo Duración para siestas (ej., 30 minutos)"
    "Timer_Tip2" = "• Usa el modo Detener a hora para dormir (ej., 11:00 PM)"
    "Timer_Tip3" = "• El audio se desvanecerá gradualmente cuando expire el temporizador"
    
    # Settings Page (48 strings)
    "Settings_Description" = "Selecciona el plan que mejor se adapte a tus necesidades. Actualiza en cualquier momento para desbloquear más funciones."
    "Settings_CurrentStatus" = "Estado actual"
    "Settings_RestorePurchases" = "Restaurar compras"
    "Settings_RestoreNone" = "No hay compras para restaurar"
    "Settings_Diagnostics_Description" = "Verificar el estado de la aplicación y ver informes de errores para solucionar problemas"
    "Settings_LifetimeTitle" = "Compras de por vida"
    "Settings_Free_Title" = "Gratis"
    "Settings_Free_Description" = "Perfecto para probar la aplicación"
    "Settings_Free_Feature2" = "? Acceso a todos los paquetes de sonido"
    "Settings_Free_Feature3" = "? Guardar hasta 3 mezclas"
    "Settings_Free_Limit" = "• Límite de sesión de 15 minutos"
    "Settings_Standard_Title" = "Estándar"
    "Settings_Standard_Feature1" = "? Mezclar 3 sonidos"
    "Settings_Standard_Feature3" = "? Guardar hasta 10 mezclas"
    "Settings_Standard_Yearly" = "$24/año"
    "Settings_Premium_Title" = "Premium"
    "Settings_Premium_Badge" = "? Más popular"
    "Settings_Premium_Monthly" = "$9.99/mes"
    "Settings_Premium_Feature1" = "? Mezclar 10 sonidos"
    "Settings_Premium_Feature2" = "? Mezclas y listas ilimitadas"
    "Settings_Premium_Feature5" = "? Exportar/Importar (compartible)"
    "Settings_ProPlus_Title" = "Pro+"
    "Settings_ProPlus_Badge" = "?? Máxima potencia"
    "Settings_ProPlus_Monthly" = "$14.99/mes"
    "Settings_ProPlus_Yearly" = "$74/año"
    "Settings_ProPlus_Feature3" = "? Presets de EQ personalizados"
    "Settings_ProPlus_Feature4" = "? Transiciones de mezcla"
    "Settings_ProPlus_Feature5" = "? Soporte prioritario"
    "Settings_StandardLifetime_Price" = "$59"
    "Settings_StandardLifetime_Savings" = "?? Ahorra $35 vs 3 años de suscripción"
    "Settings_PremiumLifetime_Title" = "Premium de por vida"
    "Settings_PremiumLifetime_Price" = "$99"
    "Settings_PremiumLifetime_Savings" = "?? Ahorra $140 vs 3 años de suscripción"
    "Settings_PremiumLifetime_Badge" = "?? MEJOR VALOR"
    
    # Upgrade Page (4 strings)
    "Upgrade_Title" = "Actualizar"
    "Upgrade_Description" = "Las sesiones gratuitas están limitadas a 15 minutos. Actualiza para disfrutar sesiones más largas, listas de reproducción, más mezclas guardadas y más."
    "Upgrade_UnlockLongerSessions" = "Desbloquear sesiones más largas"
    "Upgrade_NotNow" = "Ahora no"
    
    # Common UI (7 strings - removed duplicate Toolbar_Audio)
    "AppName" = "Ambient Sleeper"
    "Mix_SaveButton" = "?? Guardar"
    "Mix_StopButton" = "? Detener"
    "Common_RemoveIcon" = "?"
    
    # Playback (2 strings)
    "Playback_MixPlaylistLocked_Message" = "Programa diferentes paisajes sonoros a lo largo de tu sesión. Disponible en niveles Premium y Pro+."
    "Playback_MixPlaylistEmpty" = "Crea y guarda mezclas primero, luego agrégalas aquí para construir una lista programada."
    
    # Legal Footer (1 string)
    "Legal_Footer_Copyright" = "Todos los derechos reservados"
}
