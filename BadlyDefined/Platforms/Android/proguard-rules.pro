# R8 rules for BadlyDefined (.NET MAUI Android Release)
# R8 is the modern code shrinker and obfuscator for Android (replaces ProGuard)

# Keep MAUI essentials
-keep class Microsoft.Maui.** { *; }
-keep class Microsoft.Extensions.** { *; }

# Keep SQLite (critical for database)
-keep class SQLite.** { *; }
-keep class SQLitePCL.** { *; }
-dontwarn SQLite.**
-dontwarn SQLitePCL.**

# Keep our models (reflection used by SQLite)
-keep class BadlyDefined.Models.** { *; }
-keep class BadlyDefined.Services.** { *; }
-keep class BadlyDefined.ViewModels.** { *; }

# Keep CommunityToolkit
-keep class CommunityToolkit.Maui.** { *; }

# Keep Android resources
-keepclassmembers class **.R$* {
    public static <fields>;
}

# Keep native methods
-keepclasseswithmembernames class * {
    native <methods>;
}

# Preserve annotations (required for .NET reflection)
-keepattributes *Annotation*
-keepattributes Signature
-keepattributes InnerClasses
-keepattributes EnclosingMethod
-keepattributes SourceFile
-keepattributes LineNumberTable

# Keep enums
-keepclassmembers enum * {
    public static **[] values();
    public static ** valueOf(java.lang.String);
}

# Don't warn about missing classes (common in .NET)
-dontwarn java.lang.management.**
-dontwarn javax.naming.**

# R8 full mode optimizations
-allowaccessmodification
-repackageclasses
