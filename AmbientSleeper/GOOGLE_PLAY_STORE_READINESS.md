# 🚀 Google Play Store Readiness Implementation

## Overview
I have updated your `.NET MAUI` Android project configurations to comply with **Google Play Store requirements**. Google enforces several strict rules regarding how APKs/App Bundles are packaged, the metadata they contain, permissions, and service declarations.

## Summary of Changes

### 1. Application Identification & Targeting (`AmbientSleeper.csproj`)
* **Unique ApplicationId:** Google Play rejects default templates like `com.companyname.*`. It is now updated to `com.ambientsleeper.app`.
* **Versioning:** Added explicit `ApplicationDisplayVersion` (e.g., `1.0.0`) and `ApplicationVersion` (e.g., `10000`) codes instead of keeping defaults.
* **App Bundle (.aab) Generation:** Configured `<AndroidPackageFormat>aab</AndroidPackageFormat>` for Android `Release` builds (Google requires AABs for new deployments via Play Console).
* **Architectures:** Enforced 64-bit architecture constraints with `<RuntimeIdentifiers>android-arm;android-arm64;android-x86;android-x64</RuntimeIdentifiers>`.
* **AOT Compilation & Trimming:** Enabled `<RunAOTCompilation>` and `SdkOnly` linking for optimal performance on Release mode.

### 2. Android Manifest & Permissions (`Platforms/Android/AndroidManifest.xml`)
* **Wake Lock (`WAKE_LOCK`):** Added for background playing ambient sleepers so the device's CPU stays awake during sound sequences.
* **Google Play In-App Billing (`com.android.vending.BILLING`):** Added since the app has premium tiers and an upgrade page to authorize Google Play purchases.
* **Ad Mob Advertising ID (`AD_ID`):** Added the `com.google.android.gms.permission.AD_ID` permission, which is required by Google Play Store guidelines for Android 13+ devices if you implement an ad SDK (like the `AdvertisingService`/`BannerAdView` defined).
* **Notifications & Alarms:** Added `POST_NOTIFICATIONS` (Android 13+) and `SCHEDULE_EXACT_ALARM` for the `TimerViewModel` alarms and foreground notification services. Note: Avoided `USE_EXACT_ALARM` to prevent immediate rejection by Google Play Policy review, as it is heavily restricted.

### 3. Android 12+ API 31 Compliance (`Platforms/Android/MainActivity.cs`)
* **Exported Attribute:** Added `Exported = true` to the `[Activity]` tag for the `MainActivity`. Any activity declaring an `IntentFilter` (like `MainLauncher = true`) **must** explicitly specify whether it is exported, or installing the app on Android 12+ devices will crash upon launch.

## Next Steps
* The Android artifact can now be produced by compiling in **Release** mode (`dotnet build -c Release -f net9.0-android`). This will output the required `.aab` file format.
* Check your **Google Play Console** to declare the newly added permissions (`AD_ID` for Ads, and `SCHEDULE_EXACT_ALARM` if required to execute exact UI popups).
* Review your Privacy Policy (in `LegalPage.xaml`) and ensure it handles disclosures about Background Audio tracking.