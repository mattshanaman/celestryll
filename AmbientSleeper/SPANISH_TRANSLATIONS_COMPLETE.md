# ???? Spanish (es) - Complete Translation Reference

## Status: ? READY TO APPLY

This document contains Spanish translations for all critical and common strings in Ambient Sleeper.
Use this as a reference when editing `AppResources.es.resx`.

---

## ? How to Apply These Translations

1. Open `Resources\Strings\AppResources.es.resx` in Visual Studio or text editor
2. Find each `<data name="StringName">` entry
3. Replace the `<value>` content with the Spanish translation below
4. Save the file

---

## ?? Translations by Category

### Common Dialog Buttons
```
Ok ? Aceptar
Cancel ? Cancelar
Yes ? Sí
No ? No
Error ? Error
```

### Navigation (Nav_*)
```
Nav_Library ? Biblioteca
Nav_Playback ? Reproducción
Nav_Timer ? Temporizador
Nav_EQ ? Ecualizador
Nav_Settings ? Ajustes
Nav_Help ? Ayuda
Nav_Legal ? Legal
```

### Tabs (Tab_*)
```
Tab_Mix ? Mezcla
Tab_Playlist ? Lista
Tab_MixPlaylist ? Lista de mezclas
Tab_BuiltIn ? Incorporado
Tab_YourAudio ? Tu audio
```

### Common Buttons (Common_*)
```
Common_PlayButton ? ? Reproducir
Common_StopButton ? ? Detener
Common_SaveButton ? ?? Guardar
Common_LoadButton ? ?? Cargar
Common_DeleteIcon ? ??
```

### Mix Mode (Mix_*)
```
Mix_Mode ? Modo de mezcla
Mix_Empty ? Tu mezcla está vacía. Agrega sonidos desde la biblioteca.
Mix_RemoveButton ? Quitar de la mezcla
Mix_SoundsInMixFormat ? Sonidos en la mezcla: {0}
Mix_OfFormat ? de {0}
Mix_SaveCurrent ? Guardar mezcla actual
Mix_NamePlaceholder ? Nombre de la mezcla
Mix_SaveLimitFormat ? Límite de mezclas guardadas: {0}
Mix_SavedTitle ? Mezclas guardadas
Mix_SavedEmpty ? No hay mezclas guardadas. Guarda tu primera mezcla arriba.
Mix_SoundsCountFormat ? {0} sonidos
Mix_DeleteButton ? Eliminar mezcla
Mix_StopAllFormat ? ? Detener todo (desvanecer {0}s)
```

### Playlist Mode (Playlist_*)
```
Playlist_Mode ? Modo de lista de reproducción
Playlist_Empty ? Tu lista de reproducción está vacía. Agrega sonidos desde la biblioteca.
Playlist_RemoveButton ? Quitar de la lista
Playlist_SoundsCountFormat ? {0} sonidos en la lista
Playlist_LoopToggle ? Repetir lista
Playlist_SaveCurrent ? Guardar lista actual
Playlist_NamePlaceholder ? Nombre de la lista
Playlist_SaveLimitFormat ? Límite de listas guardadas: {0}
Playlist_SavedTitle ? Listas guardadas
Playlist_SavedEmpty ? No hay listas guardadas. Guarda tu primera lista arriba.
Playlist_DeleteButton ? Eliminar lista
PlaylistLocked_Title ? Lista bloqueada
PlaylistLocked_Message ? Actualiza a Estándar, Premium o Pro+ para guardar o modificar listas.
```

### Mix Playlist Mode (MixPlaylist_*)
```
MixPlaylist_Mode ? Modo de lista de mezclas
MixPlaylist_Empty ? Tu lista de mezclas está vacía. Agrega mezclas guardadas.
MixPlaylist_RemoveButton ? Quitar de la lista de mezclas
MixPlaylist_Duration ? Duración:
MixPlaylist_Seconds ? segundos
MixPlaylist_Transition ? Transición:
MixPlaylist_Loop ? Repetir lista de mezclas
MixPlaylist_Save ? Guardar lista de mezclas actual
MixPlaylist_NamePlaceholder ? Nombre de la lista de mezclas
MixPlaylist_SaveLimitFormat ? Límite de listas de mezclas: {0}
MixPlaylist_Saved ? Listas de mezclas guardadas
MixPlaylist_SavedEmpty ? No hay listas de mezclas guardadas.
MixPlaylist_MixesCountFormat ? {0} mezclas
MixPlaylist_DeleteButton ? Eliminar lista de mezclas
MixPlaylist_Locked_Title ? Lista de mezclas bloqueada
MixPlaylist_Locked_Message ? Actualiza a Pro+ para usar listas de mezclas.
```

### Playback Locked Messages
```
Playback_PlaylistLocked_Title ? Lista de reproducción bloqueada
Playback_PlaylistLocked_Message ? Actualiza a Estándar, Premium o Pro+ para usar listas de reproducción.
```

### Toolbar (Toolbar_*)
```
Toolbar_Tier ? Nivel
Toolbar_EQ ? EQ
Toolbar_Audio ? Audio
Toolbar_Export ? Exportar
Toolbar_Import ? Importar
```

### Library (Library_*)
```
Library_Title ? Biblioteca
ImportFromDevice ? Importar desde dispositivo
AudioBundles ? Paquetes
A11y_AddToMix ? Agregar a la mezcla
A11y_AddToPlaylist ? Agregar a la lista
Confirmation_SoundAdded ? Sonido agregado a la lista
DefaultPlaylistCreated_Title ? Lista predeterminada creada
DefaultPlaylistCreated_Message ? Se ha creado una lista 'Predeterminada'.
SelectPlaylist_Title ? Seleccionar lista
EmptyPlaylist_Title ? Lista vacía
EmptyPlaylist_Message ? Agrega sonidos a la cola de reproducción o carga una lista guardada primero.
```

### Export/Import
```
ExportScope_Title ? Alcance de exportación
ExportScope_Personal ? Personal (Estándar+)
ExportScope_Shareable ? Compartible (Premium/Pro+)
ExportLocked_Title ? Exportación bloqueada
ExportLocked_Message ? Actualiza a Estándar, Premium o Pro+ para exportar tus mezclas.
ExportComplete_Title ? Exportación completa
ExportComplete_Message ? Tus mezclas y listas fueron exportadas. Elige una ubicación para guardar el archivo.
ExportFailed_Title ? Exportación fallida
ImportComplete_Title ? Importación completa
ImportComplete_MessageFormat ? Se importaron {0} elementos.
ImportFailed_Title ? Importación fallida
PickExport_Title ? Elegir exportación
PickAudio_Title ? Elegir audio
PickAudio_TitleAlt ? Seleccionar archivo de audio
ExportShare_Title ? Compartir exportación
ExportShare_ShareSheet ? Hoja para compartir
ExportShare_Email ? Correo electrónico
ExportEmail_Subject ? Configuración de Ambient Sleeper
```

### Settings (Settings_*)
```
Settings_Title ? Ajustes de suscripción
Settings_TierFree ? Gratis
Settings_TierStandard ? Estándar
Settings_TierPremium ? Premium
Settings_TierProPlus ? Pro+
Settings_Recurring ? Suscripción recurrente
Settings_Lifetime ? De por vida
Settings_StandardLifetime ? Estándar de por vida
Settings_PremiumLifetime ? Premium de por vida
Settings_CheckHealth ? Verificar estado
Settings_ErrorReport ? Informe de errores
```

### Subscription
```
Subscription_Title ? Suscripción
Purchased_StandardLifetime ? Estándar de por vida comprado
Purchased_PremiumLifetime ? Premium de por vida comprado
LifetimeSuffix ? De por vida
CurrentTier_Format ? Nivel actual: {0} {1}
```

### Timer (Timer_*)
```
Timer_Title ? Temporizador para dormir
Timer_StopAfterDuration ? Detener después de duración
Timer_StopAtTime ? Detener a hora específica
Timer_Duration ? Duración:
Timer_StopAt ? Detener a:
Timer_Start ? Iniciar temporizador
Timer_Stop ? Detener temporizador
Timer_Remaining ? Tiempo restante: {0}
Timer_Alarm ? Alarma (Pro+/Premium)
Timer_AlarmEnabled ? Alarma habilitada
Alarm_Title ? Alarma
Alarm_Free_Message ? La función de alarma requiere suscripción Premium o Pro+.
```

### Equalizer (EQ_*)
```
EQ_Title ? Ecualizador (Premium/Pro+)
EQ_Locked ? El ecualizador requiere suscripción Premium o Pro+
EQ_Bass ? Graves
EQ_Mids ? Medios
EQ_Treble ? Agudos
EQ_Reset ? Restablecer
```

### Bundles (Bundle_*)
```
Bundle_BuiltIn ? Incorporado
Bundle_Locked ? Bloqueado
Bundle_UnlockWith ? Desbloquear con {0}
```

### Notifications (Notification_*)
```
Notification_TimerComplete_Title ? Temporizador completo
Notification_TimerComplete_Description ? Tu temporizador para dormir ha terminado
```

### Navigation Errors (NavigationError_*)
```
NavigationError_Title ? Error de navegación
NavigationError_Settings ? No se pudo abrir la página de ajustes
```

### Health Check (HealthCheck_*)
```
HealthCheck_Checking ? Verificando estado...
HealthCheck_AllHealthy ? Todo está bien
HealthCheck_Passed ? Verificación de estado aprobada
HealthCheck_IssuesDetected ? {0} problemas detectados
HealthCheck_ResultsTitle ? Resultados de verificación
HealthCheck_IssuesMessage ? Se encontraron los siguientes problemas:\n• {0}
HealthCheck_Failed ? Verificación fallida
HealthCheck_FailedMessage ? Error al verificar estado: {0}
```

### Error Reporting (ErrorReport_*)
```
ErrorReport_Title ? Informe de errores
ErrorReport_ServiceUnavailable ? Servicio de informes de errores no disponible
ErrorReport_NoErrors ? No hay errores registrados
ErrorReport_CountFormat ? {0} errores registrados
ErrorReport_ViewDetails ? Ver detalles
ErrorReport_ShareReport ? Compartir informe
ErrorReport_ClearErrors ? Borrar errores
ErrorReport_SharedSuccess ? Informe compartido exitosamente
ErrorReport_ClearConfirmTitle ? Borrar errores
ErrorReport_ClearConfirmMessage ? ¿Estás seguro de que quieres borrar todos los errores?
ErrorReport_Cleared ? Errores borrados
ErrorReport_ViewFailed ? Error al ver informe: {0}
```

---

## ?? IMPORTANT: Legal & Help Content

### Legal Page Strings
**DO NOT TRANSLATE YOURSELF** - Must use certified legal translator!

The Legal page contains critical medical disclaimers and liability statements that require professional legal translation. Contact a certified legal translator for these strings:

- Legal_PageTitle
- Legal_Critical_*
- Legal_Entertainment_*
- Legal_Health_*
- Legal_NoWarranty_*
- Legal_Liability_*
- etc.

### Help Page Strings
**Recommended: Professional translator**

Help content should be professionally translated for clarity. Main Help strings include:

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

1. Open `AppResources.es.resx` in VS Code or Notepad++
2. For each translation above:
   - Find: `<data name="Ok" xml:space="preserve"><value>OK</value>`
   - Replace with: `<data name="Ok" xml:space="preserve"><value>Aceptar</value>`
3. Save file

### Method 2: Visual Studio Resource Editor

1. Double-click `AppResources.es.resx` in Solution Explorer
2. Find each Name in the grid
3. Replace the Value column with Spanish translation
4. Save file

---

## ? Verification

After applying translations:

```powershell
# Build solution
dotnet build

# Check for errors
# Should build successfully

# Test in Spanish
# Change device language to Spanish (es)
# Launch app and verify translations appear
```

---

**Translation completed:** Critical and common UI strings
**Quality:** Good for production UI
**Legal review:** Required before deployment
**Native speaker review:** Recommended

---

**Document created:** December 2024
**Language:** Spanish (es)
**Status:** Ready to apply
**Coverage:** ~150 of 452 strings (critical UI complete)
