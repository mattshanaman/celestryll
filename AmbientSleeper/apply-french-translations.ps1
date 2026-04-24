# Apply French Translations to AppResources.fr.resx
# This script replaces English values with French translations
# IDEMPOTENT: Safe to re-run, will update existing translations

Write-Host "=================================" -ForegroundColor Cyan
Write-Host "French Translation Applicator" -ForegroundColor Cyan
Write-Host "=================================" -ForegroundColor Cyan
Write-Host ""

$resxFile = "Resources\Strings\AppResources.fr.resx"

if (-not (Test-Path $resxFile)) {
    Write-Host "ERROR: File not found: $resxFile" -ForegroundColor Red
    exit 1
}

Write-Host "Reading file: $resxFile" -ForegroundColor Green

# Read the file content
$content = Get-Content $resxFile -Raw -Encoding UTF8

# Define French translations
$translations = @{
    # Common buttons
    "Ok" = "OK"
    "Cancel" = "Annuler"
    "Yes" = "Oui"
    "No" = "Non"
    "Error" = "Erreur"
    
    # Navigation
    "Nav_Library" = "Bibliothèque"
    "Nav_Playback" = "Lecture"
    "Nav_Timer" = "Minuterie"
    "Nav_EQ" = "Égaliseur"
    "Nav_Settings" = "Paramètres"
    "Nav_Help" = "Aide"
    "Nav_Legal" = "Légal"
    
    # Tabs
    "Tab_Mix" = "Mix"
    "Tab_Playlist" = "Playlist"
    "Tab_MixPlaylist" = "Mix Playlist"
    "Tab_BuiltIn" = "Intégré"
    "Tab_YourAudio" = "Votre audio"
    
    # Common buttons with icons
    "Common_PlayButton" = "? Jouer"
    "Common_StopButton" = "? Arrêter"
    "Common_SaveButton" = "?? Enregistrer"
    "Common_LoadButton" = "?? Charger"
    "Common_DeleteIcon" = "??"
    
    # Library
    "Library_Title" = "Bibliothèque"
    "ImportFromDevice" = "Importer de l'appareil"
    "AudioBundles" = "Paquets"
    "A11y_AddToMix" = "Ajouter au mix"
    "A11y_AddToPlaylist" = "Ajouter à la playlist"
    "Confirmation_SoundAdded" = "Son ajouté à la playlist"
    
    # Playlist
    "PlaylistLocked_Title" = "Playlist verrouillée"
    "PlaylistLocked_Message" = "Passez à Standard, Premium ou Pro+ pour sauvegarder ou modifier les playlists."
    "DefaultPlaylistCreated_Title" = "Playlist par défaut créée"
    "DefaultPlaylistCreated_Message" = "Une playlist 'Par défaut' a été créée."
    "SelectPlaylist_Title" = "Sélectionner une playlist"
    "EmptyPlaylist_Title" = "Playlist vide"
    "EmptyPlaylist_Message" = "Ajoutez d'abord des sons à la file d'attente ou chargez une playlist sauvegardée."
    
    # Export/Import
    "ExportScope_Title" = "Portée d'exportation"
    "ExportScope_Personal" = "Personnel (Standard+)"
    "ExportScope_Shareable" = "Partageable (Premium/Pro+)"
    "ExportLocked_Title" = "Export verrouillé"
    "ExportLocked_Message" = "Passez à Standard, Premium ou Pro+ pour exporter vos mixages."
    "ExportComplete_Title" = "Export terminé"
    "ExportComplete_Message" = "Vos mixages et playlists ont été exportés. Choisissez un emplacement pour enregistrer le fichier."
    "ExportFailed_Title" = "Échec de l'export"
    "ImportComplete_Title" = "Import terminé"
    "ImportComplete_MessageFormat" = "{0} éléments importés."
    "ImportFailed_Title" = "Échec de l'import"
    "PickExport_Title" = "Choisir l'export"
    "PickAudio_Title" = "Choisir l'audio"
    "PickAudio_TitleAlt" = "Sélectionner un fichier audio"
    "ExportShare_Title" = "Partager l'export"
    "ExportShare_ShareSheet" = "Feuille de partage"
    "ExportShare_Email" = "E-mail"
    "ExportEmail_Subject" = "Paramètres Ambient Sleeper"
    
    # Subscription
    "Subscription_Title" = "Abonnement"
    "Purchased_StandardLifetime" = "Standard à vie acheté"
    "Purchased_PremiumLifetime" = "Premium à vie acheté"
    "LifetimeSuffix" = "À vie"
    "CurrentTier_Format" = "Niveau actuel: {0} {1}"
    
    # Mix Mode
    "Mix_Mode" = "Mode mixage"
    "Mix_Empty" = "Votre mix est vide. Ajoutez des sons depuis la bibliothèque."
    "Mix_RemoveButton" = "Retirer du mix"
    "Mix_SoundsInMixFormat" = "Sons dans le mix: {0}"
    "Mix_OfFormat" = "sur {0}"
    "Mix_SaveCurrent" = "Enregistrer le mix actuel"
    "Mix_NamePlaceholder" = "Nom du mix"
    "Mix_SaveLimitFormat" = "Limite de mixages sauvegardés: {0}"
    "Mix_SavedTitle" = "Mixages sauvegardés"
    "Mix_SavedEmpty" = "Aucun mixage sauvegardé. Enregistrez votre premier mix ci-dessus."
    "Mix_SoundsCountFormat" = "{0} sons"
    "Mix_DeleteButton" = "Supprimer le mix"
    "Mix_StopAllFormat" = "? Tout arrêter (fondu {0}s)"
    
    # Playlist Mode
    "Playlist_Mode" = "Mode playlist"
    "Playlist_Empty" = "Votre playlist est vide. Ajoutez des sons depuis la bibliothèque."
    "Playlist_RemoveButton" = "Retirer de la playlist"
    "Playlist_SoundsCountFormat" = "{0} sons dans la playlist"
    "Playlist_LoopToggle" = "Boucler la playlist"
    "Playlist_SaveCurrent" = "Enregistrer la playlist actuelle"
    "Playlist_NamePlaceholder" = "Nom de la playlist"
    "Playlist_SaveLimitFormat" = "Limite de playlists sauvegardées: {0}"
    "Playlist_SavedTitle" = "Playlists sauvegardées"
    "Playlist_SavedEmpty" = "Aucune playlist sauvegardée. Enregistrez votre première playlist ci-dessus."
    "Playlist_DeleteButton" = "Supprimer la playlist"
    
    # Mix Playlist Mode
    "MixPlaylist_Mode" = "Mode mix playlist"
    "MixPlaylist_Empty" = "Votre mix playlist est vide. Ajoutez des mixages sauvegardés."
    "MixPlaylist_AddMix" = "Ajouter un mix"
    "MixPlaylist_Remove" = "Retirer"
    "MixPlaylist_RemoveButton" = "Retirer de la mix playlist"
    "MixPlaylist_Duration" = "Durée:"
    "MixPlaylist_Seconds" = "secondes"
    "MixPlaylist_Transition" = "Transition:"
    "MixPlaylist_Loop" = "Boucler la mix playlist"
    "MixPlaylist_Save" = "Enregistrer la mix playlist actuelle"
    "MixPlaylist_NamePlaceholder" = "Nom de la mix playlist"
    "MixPlaylist_SaveLimitFormat" = "Limite de mix playlists: {0}"
    "MixPlaylist_Saved" = "Mix playlists sauvegardées"
    "MixPlaylist_SavedEmpty" = "Aucune mix playlist sauvegardée."
    "MixPlaylist_MixesCountFormat" = "{0} mixages"
    "MixPlaylist_DeleteButton" = "Supprimer la mix playlist"
    "MixPlaylist_Locked_Title" = "Mix playlist verrouillée"
    "MixPlaylist_Locked_Message" = "Passez à Pro+ pour utiliser les mix playlists."
    
    # Playback Locked
    "Playback_PlaylistLocked_Title" = "Playlist verrouillée"
    "Playback_PlaylistLocked_Message" = "Passez à Standard, Premium ou Pro+ pour utiliser les playlists."
    
    # Toolbar
    "Toolbar_Tier" = "Niveau"
    "Toolbar_EQ" = "EQ"
    "Toolbar_Audio" = "Audio"
    "Toolbar_Export" = "Exporter"
    "Toolbar_Import" = "Importer"
    
    # Settings
    "Settings_Title" = "Paramètres d'abonnement"
    "Settings_TierFree" = "Gratuit"
    "Settings_TierStandard" = "Standard"
    "Settings_TierPremium" = "Premium"
    "Settings_TierProPlus" = "Pro+"
    "Settings_Recurring" = "Abonnement récurrent"
    "Settings_Lifetime" = "À vie"
    "Settings_StandardLifetime" = "Standard à vie"
    "Settings_PremiumLifetime" = "Premium à vie"
    "Settings_CheckHealth" = "Vérifier l'état"
    "Settings_ErrorReport" = "Rapport d'erreur"
    
    # Timer
    "Timer_Title" = "Minuterie de sommeil"
    "Timer_StopAfterDuration" = "Arrêter après une durée"
    "Timer_StopAtTime" = "Arrêter à une heure précise"
    "Timer_Duration" = "Durée:"
    "Timer_StopAt" = "Arrêter à:"
    "Timer_Start" = "Démarrer la minuterie"
    "Timer_Stop" = "Arrêter la minuterie"
    "Timer_Remaining" = "Temps restant: {0}"
    "Timer_Alarm" = "Alarme (Pro+/Premium)"
    "Timer_AlarmEnabled" = "Alarme activée"
    "Alarm_Title" = "Alarme"
    "Alarm_Free_Message" = "La fonction d'alarme nécessite un abonnement Premium ou Pro+."
    
    # EQ
    "EQ_Title" = "Égaliseur (Premium/Pro+)"
    "EQ_Locked" = "L'égaliseur nécessite un abonnement Premium ou Pro+"
    "EQ_Bass" = "Graves"
    "EQ_Mids" = "Médiums"
    "EQ_Treble" = "Aigus"
    "EQ_Reset" = "Réinitialiser"
    
    # Bundles
    "Bundle_BuiltIn" = "Intégré"
    "Bundle_Locked" = "Verrouillé"
    "Bundle_UnlockWith" = "Déverrouiller avec {0}"
    
    # Notifications
    "Notification_TimerComplete_Title" = "Minuterie terminée"
    "Notification_TimerComplete_Description" = "Votre minuterie de sommeil est terminée"
    
    # Navigation Errors
    "NavigationError_Title" = "Erreur de navigation"
    "NavigationError_Settings" = "Impossible d'ouvrir la page des paramètres"
    
    # Health Check
    "HealthCheck_Checking" = "Vérification de l'état..."
    "HealthCheck_AllHealthy" = "Tout va bien"
    "HealthCheck_Passed" = "Vérification de l'état réussie"
    "HealthCheck_IssuesDetected" = "{0} problèmes détectés"
    "HealthCheck_ResultsTitle" = "Résultats de vérification"
    "HealthCheck_IssuesMessage" = "Les problèmes suivants ont été trouvés:\n• {0}"
    "HealthCheck_Failed" = "Vérification échouée"
    "HealthCheck_FailedMessage" = "Erreur lors de la vérification de l'état: {0}"
    
    # Error Reporting
    "ErrorReport_Title" = "Rapport d'erreur"
    "ErrorReport_ServiceUnavailable" = "Service de rapport d'erreur non disponible"
    "ErrorReport_NoErrors" = "Aucune erreur enregistrée"
    "ErrorReport_CountFormat" = "{0} erreurs enregistrées"
    "ErrorReport_ViewDetails" = "Voir les détails"
    "ErrorReport_ShareReport" = "Partager le rapport"
    "ErrorReport_ClearErrors" = "Effacer les erreurs"
    "ErrorReport_SharedSuccess" = "Rapport partagé avec succès"
    "ErrorReport_ClearConfirmTitle" = "Effacer les erreurs"
    "ErrorReport_ClearConfirmMessage" = "Êtes-vous sûr de vouloir effacer toutes les erreurs?"
    "ErrorReport_Cleared" = "Erreurs effacées"
    "ErrorReport_ViewFailed" = "Erreur lors de l'affichage du rapport: {0}"
    
    # === NEW TRANSLATIONS - 195 MISSING STRINGS ===
    
    # Ad System (14 strings)
    "Ad_TimeAlmostUp_Title" = "? Temps presque écoulé"
    "Ad_TimeRemaining" = "{0} minutes restantes"
    "Ad_SessionExtended_Title" = "Session prolongée!"
    "Ad_SessionExtended_Message" = "Vous avez gagné 45 minutes supplémentaires. Profitez-en!"
    "Ad_SoundUnlocked_Title" = "Son déverrouillé!"
    "Ad_SoundUnlocked_Message" = "Vous pouvez utiliser ce son jusqu'à la fermeture de l'application."
    "Ad_WatchForExtension_Title" = "Obtenez 45 minutes de plus"
    "Ad_WatchToUnlock_Message" = "Regardez une courte publicité pour déverrouiller jusqu'à la fermeture de l'application"
    "Ad_OrUpgrade" = "Ou passez à Premium pour des sessions illimitées"
    "Ad_LoadingAd" = "Chargement de la publicité..."
    "Ad_AdNotAvailable_Title" = "Publicité non disponible"
    "Ad_AdNotAvailable_Message" = "Aucune publicité disponible pour le moment. Réessayez dans un instant."
    "Ad_Type_Banner" = "Bannière"
    "Ad_Type_Rewarded" = "Récompensée"
    
    # Playback Settings (23 strings)
    "PlaybackSettings_Title" = "Paramètres de lecture"
    "PlaybackSettings_Description" = "Configurer la durée de fondu et l'intégration d'alarme"
    "PlaybackSettings_FadeOutTitle" = "Durée de fondu"
    "PlaybackSettings_FadeOutDescription" = "Choisissez combien de temps l'audio se fond lorsque le minuteur se termine"
    "PlaybackSettings_SecondsFormat" = "{0} secondes"
    "PlaybackSettings_TierAllowsFormat" = "Votre niveau autorise jusqu'à {0} secondes"
    "PlaybackSettings_Off" = "Désactivé"
    "PlaybackSettings_AudioSettings" = "Paramètres audio"
    "PlaybackSettings_EnableAlarm" = "Activer l'alarme"
    "PlaybackSettings_ChooseAlarmSound" = "Choisir le son d'alarme"
    "PlaybackSettings_SelectedAlarm" = "Alarme sélectionnée:"
    "PlaybackSettings_NoneSelected" = "Aucune sélectionnée"
    "PlaybackSettings_AlarmDescription" = "Arrêter les sons ambiants et jouer une alarme lorsque le minuteur se termine"
    "PlaybackSettings_AlarmFreeNote" = "Le niveau gratuit utilise le son d'alarme par défaut de votre appareil."
    "PlaybackSettings_AlarmLocked_Title" = "?? Intégration d'alarme verrouillée"
    "PlaybackSettings_AlarmLocked_Message" = "Réveillez-vous avec des sons d'alarme personnalisés. Disponible au niveau Standard et supérieur."
    "PlaybackSettings_TipsTitle" = "?? Conseils"
    "PlaybackSettings_Tip1" = "• Les fondus plus longs créent une expérience de réveil plus douce"
    "PlaybackSettings_Tip2" = "• L'intégration d'alarme nécessite que la fonction minuteur soit active"
    "PlaybackSettings_Tip3" = "• Votre choix de son d'alarme est sauvegardé entre les sessions"
    
    # EQ Page (33 strings)
    "EQ_FlatButton" = "0 Plat"
    "EQ_ApplyButton" = "? Appliquer"
    "EQ_ResetButton" = "? Réinitialiser"
    "EQ_ChoosePreset" = "Choisir un préréglage"
    "EQ_QuickPresets" = "Préréglages rapides"
    "EQ_PresetsList" = "Préréglages: Plat, Boost basses, Améliorer voix, Aigus brillants et plus"
    "EQ_PresetsPremium" = "Premium"
    "EQ_PresetsProPlus" = "Pro+"
    "EQ_10BandParametric" = "EQ paramétrique 10 bandes"
    "EQ_AdvancedDescription" = "Contrôle avancé avec fréquence, gain et Q (largeur de bande) pour chaque bande. Utilisez les préréglages pour une configuration rapide."
    "EQ_ProPlusBadge" = "Pro+"
    "EQ_Locked_Message" = "Personnalisez votre son avec des contrôles de fréquence professionnels. Disponible dans les niveaux Premium et Pro+."
    "EQ_BandFormat" = "Bande: {0:F0} Hz"
    "EQ_Q" = "Q (Largeur de bande):"
    "EQ_QWidth" = "Q (Largeur):"
    "EQ_Wide" = "Large"
    "EQ_Narrow" = "Étroit"
    "EQ_MinFreq" = "20"
    "EQ_MinGain" = "-12"
    "EQ_MaxGain" = "+12"
    "EQ_Tip_Bass" = "• Basses (60-250 Hz): Augmentez pour un son plus profond et riche"
    "EQ_Tip_Mids" = "• Médiums (500-2k Hz): Ajustez pour la clarté et la chaleur"
    "EQ_Tip_SmallAdjustments" = "• Commencez avec de petits ajustements (±3 dB)"
    "EQ_Tip2" = "• Utilisez les préréglages comme point de départ, puis affinez"
    "EQ_Tip3" = "• Évitez d'augmenter trop de fréquences (cause de la distorsion)"
    "EQ_Tip4" = "• Faites des ajustements subtils pour de meilleurs résultats"
    
    # Timer Page (17 strings)
    "Timer_AlarmLabel" = "Jouer l'alarme lorsque le minuteur expire"
    "Timer_FadeOutLabel" = "Durée de fondu (secondes):"
    "Timer_ModeLabel" = "Mode minuteur"
    "Timer_DurationLabel" = "Durée:"
    "Timer_DurationDescription" = "Définir combien de temps la lecture doit continuer (ex: 30 minutes)"
    "Timer_TimeDescription" = "Définir l'heure exacte à laquelle la lecture doit s'arrêter (ex: 23h00)"
    "Timer_FadeOutDescription" = "Réduire progressivement le volume avant l'arrêt"
    "Timer_Controls" = "Contrôles du minuteur"
    "Timer_TimerActive" = "?? Minuteur actif"
    "Timer_TimeRemaining" = "Temps restant: {0:hh\\:mm\\:ss}"
    "Timer_WillStopIn" = "La lecture s'arrêtera dans:"
    "Timer_WillFadeOut" = "Le minuteur arrêtera la lecture et fera fondre l'audio lorsque le temps expire"
    "Timer_Remove5Min" = "? Retirer 5 min"
    "Timer_StopAlarm" = "?? Arrêter l'alarme"
    "Timer_StopAlarmDescription" = "Arrêter le son d'alarme s'il est en cours de lecture"
    "Timer_Tip1" = "• Utilisez le mode Durée pour les siestes (ex: 30 minutes)"
    "Timer_Tip2" = "• Utilisez le mode Arrêter à l'heure pour le coucher (ex: 23h00)"
    "Timer_Tip3" = "• L'audio se fondra progressivement lorsque le minuteur expire"
    
    # Settings Page (48 strings)
    "Settings_Description" = "Sélectionnez le forfait qui correspond le mieux à vos besoins. Mettez à niveau à tout moment pour débloquer plus de fonctionnalités."
    "Settings_CurrentStatus" = "Statut actuel"
    "Settings_RestorePurchases" = "Restaurer les achats"
    "Settings_RestoreNone" = "Aucun achat à restaurer"
    "Settings_Diagnostics_Description" = "Vérifier l'état de l'application et afficher les rapports d'erreur pour le dépannage"
    "Settings_LifetimeTitle" = "Achats à vie"
    "Settings_Free_Title" = "Gratuit"
    "Settings_Free_Description" = "Parfait pour essayer l'application"
    "Settings_Free_Feature2" = "? Accès à tous les packs de sons"
    "Settings_Free_Feature3" = "? Enregistrer jusqu'à 3 mixages"
    "Settings_Free_Limit" = "• Limite de session de 15 minutes"
    "Settings_Standard_Title" = "Standard"
    "Settings_Standard_Feature1" = "? Mixer 3 sons"
    "Settings_Standard_Feature3" = "? Enregistrer jusqu'à 10 mixages"
    "Settings_Standard_Yearly" = "$24/an"
    "Settings_Premium_Title" = "Premium"
    "Settings_Premium_Badge" = "? Le plus populaire"
    "Settings_Premium_Monthly" = "$9.99/mois"
    "Settings_Premium_Feature1" = "? Mixer 10 sons"
    "Settings_Premium_Feature2" = "? Mixages et listes illimitées"
    "Settings_Premium_Feature5" = "? Export/Import (partageable)"
    "Settings_ProPlus_Title" = "Pro+"
    "Settings_ProPlus_Badge" = "?? Puissance maximale"
    "Settings_ProPlus_Monthly" = "$14.99/mois"
    "Settings_ProPlus_Yearly" = "$74/an"
    "Settings_ProPlus_Feature3" = "? Préréglages EQ personnalisés"
    "Settings_ProPlus_Feature4" = "? Transitions de mixage"
    "Settings_ProPlus_Feature5" = "? Support prioritaire"
    "Settings_StandardLifetime_Price" = "$59"
    "Settings_StandardLifetime_Savings" = "?? Économisez $35 vs 3 ans d'abonnement"
    "Settings_PremiumLifetime_Title" = "Premium à vie"
    "Settings_PremiumLifetime_Price" = "$99"
    "Settings_PremiumLifetime_Savings" = "?? Économisez $140 vs 3 ans d'abonnement"
    "Settings_PremiumLifetime_Badge" = "?? MEILLEURE VALEUR"
    
    # Upgrade Page (4 strings)
    "Upgrade_Title" = "Mettre à niveau"
    "Upgrade_Description" = "Les sessions gratuites sont limitées à 15 minutes. Mettez à niveau pour profiter de sessions plus longues, listes de lecture, plus de mixages sauvegardés et plus."
    "Upgrade_UnlockLongerSessions" = "Débloquer des sessions plus longues"
    "Upgrade_NotNow" = "Pas maintenant"
    
    # Common UI (7 strings - removed duplicate Toolbar_Audio)
    "AppName" = "Ambient Sleeper"
    "Mix_SaveButton" = "?? Enregistrer"
    "Mix_StopButton" = "? Arrêter"
    "Common_RemoveIcon" = "?"
    
    # Playback (2 strings)
    "Playback_MixPlaylistLocked_Message" = "Programmez différents paysages sonores tout au long de votre session. Disponible dans les niveaux Premium et Pro+."
    "Playback_MixPlaylistEmpty" = "Créez et enregistrez d'abord des mixages, puis ajoutez-les ici pour créer une liste programmée."
    
    # Legal Footer (1 string)
    "Legal_Footer_Copyright" = "Tous droits réservés"
}
