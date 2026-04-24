using System.Text;
using AmbientSleeper.Resources.Strings;

namespace AmbientSleeper.Views;

public partial class HelpPage : ContentPage
{
    public HelpPage()
    {
        InitializeComponent();
        LoadHelpContent();
    }

    private async void LoadHelpContent()
    {
        try
        {
            var html = GenerateHelpHtml();
            HelpWebView.Source = new HtmlWebViewSource { Html = html };
            
            // Hide loading indicator after a short delay
            await Task.Delay(500);
            LoadingIndicator.IsVisible = false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to load help content: {ex.Message}");
            LoadingIndicator.IsVisible = false;
            
            // Show error message in WebView
            var errorHtml = $@"
                <html>
                <head>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <style>body {{ font-family: sans-serif; padding: 20px; }}</style>
                </head>
                <body>
                    <h2>Error Loading Help</h2>
                    <p>Unable to load help content. Please try again later.</p>
                    <p style='color: #666; font-size: 0.9em;'>{ex.Message}</p>
                </body>
                </html>";
            HelpWebView.Source = new HtmlWebViewSource { Html = errorHtml };
        }
    }

    private string GenerateHelpHtml()
    {
        var sb = new StringBuilder();
        
        // HTML Header with styling
        sb.AppendLine(@"<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Ambient Sleeper Help</title>
    <style>
        body {
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, sans-serif;
            line-height: 1.6;
            color: #333;
            margin: 0;
            padding: 20px;
            background: #f5f5f5;
        }
        .container {
            max-width: 800px;
            margin: 0 auto;
            background: white;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }
        h1 { 
            color: #2c3e50; 
            border-bottom: 3px solid #3498db; 
            padding-bottom: 10px;
            margin-top: 0;
        }
        h2 { 
            color: #34495e; 
            margin-top: 30px;
            border-bottom: 2px solid #ecf0f1;
            padding-bottom: 5px;
        }
        h3 { color: #7f8c8d; margin-top: 20px; }
        .section {
            margin: 20px 0;
        }
        .feature-box {
            background: #ecf0f1;
            border-left: 4px solid #3498db;
            padding: 15px;
            margin: 15px 0;
            border-radius: 4px;
        }
        .tip {
            background: #d5f4e6;
            border-left: 4px solid #27ae60;
            padding: 15px;
            margin: 15px 0;
            border-radius: 4px;
        }
        .warning {
            background: #ffeaa7;
            border-left: 4px solid #fdcb6e;
            padding: 15px;
            margin: 15px 0;
            border-radius: 4px;
        }
        ul { padding-left: 20px; }
        li { margin: 8px 0; }
        table {
            width: 100%;
            border-collapse: collapse;
            margin: 20px 0;
        }
        th, td {
            border: 1px solid #bdc3c7;
            padding: 12px;
            text-align: left;
        }
        th {
            background: #3498db;
            color: white;
        }
        tr:nth-child(even) {
            background: #f8f9fa;
        }
        .tier {
            background: white;
            border: 2px solid #ecf0f1;
            border-radius: 8px;
            padding: 15px;
            margin: 15px 0;
        }
        .tier h3 {
            margin-top: 0;
            color: #3498db;
        }
        @media (prefers-color-scheme: dark) {
            body { background: #1a1a1a; color: #e0e0e0; }
            .container { background: #2d2d2d; }
            h1 { color: #3498db; }
            h2, h3 { color: #bdc3c7; }
            .feature-box { background: #34495e; }
            .tier { background: #34495e; border-color: #4a5f7f; }
            tr:nth-child(even) { background: #34495e; }
        }
    </style>
</head>
<body>
<div class='container'>");

        // Content - Using localized strings
        sb.AppendLine($"<h1>{AppResources.Help_Welcome_Title}</h1>");
        sb.AppendLine($"<p>{AppResources.Help_Welcome_Description}</p>");

        // Getting Started
        sb.AppendLine($"<h2>{AppResources.Help_GettingStarted_Title}</h2>");
        sb.AppendLine("<div class='section'>");
        sb.AppendLine($"<p>{AppResources.Help_GettingStarted_Description}</p>");
        sb.AppendLine("<ol>");
        sb.AppendLine($"<li><strong>{AppResources.Help_GettingStarted_Library}</strong></li>");
        sb.AppendLine($"<li><strong>{AppResources.Help_GettingStarted_Playback}</strong></li>");
        sb.AppendLine($"<li><strong>{AppResources.Help_GettingStarted_Timer}</strong></li>");
        sb.AppendLine("</ol>");
        sb.AppendLine("</div>");

        // Library Tab
        sb.AppendLine($"<h2>{AppResources.Help_Library_Title}</h2>");
        sb.AppendLine("<div class='feature-box'>");
        sb.AppendLine($"<h3>{AppResources.Help_Library_Bundles_Title}</h3>");
        sb.AppendLine($"<p>{AppResources.Help_Library_Bundles_Description}</p>");
        sb.AppendLine($"<h3>{AppResources.Help_Library_Playlists_Title}</h3>");
        sb.AppendLine($"<p>{AppResources.Help_Library_Playlists_Description}</p>");
        sb.AppendLine($"<h3>{AppResources.Help_Library_MixPlaylists_Title}</h3>");
        sb.AppendLine($"<p>{AppResources.Help_Library_MixPlaylists_Description}</p>");
        sb.AppendLine("</div>");

        // Playback Tab
        sb.AppendLine($"<h2>{AppResources.Help_Playback_Title}</h2>");
        sb.AppendLine("<div class='feature-box'>");
        sb.AppendLine($"<h3>{AppResources.Help_Playback_Mix_Title}</h3>");
        sb.AppendLine($"<p>{AppResources.Help_Playback_Mix_Description}</p>");
        sb.AppendLine("<ul>");
        // Split the tier string by |
        var tiers = AppResources.Help_Playback_Mix_Tiers.Split('|');
        foreach (var tier in tiers)
        {
            sb.AppendLine($"<li>{tier.Trim()}</li>");
        }
        sb.AppendLine("</ul>");
        sb.AppendLine($"<h3>{AppResources.Help_Playback_Playlist_Title}</h3>");
        sb.AppendLine($"<p>{AppResources.Help_Playback_Playlist_Description}</p>");
        sb.AppendLine($"<h3>{AppResources.Help_Playback_MixPlaylist_Title}</h3>");
        sb.AppendLine($"<p>{AppResources.Help_Playback_MixPlaylist_Description}</p>");
        sb.AppendLine("</div>");

        // Timer Tab
        sb.AppendLine($"<h2>{AppResources.Help_Timer_Title}</h2>");
        sb.AppendLine("<div class='feature-box'>");
        sb.AppendLine($"<h3>{AppResources.Help_Timer_Duration_Title}</h3>");
        sb.AppendLine($"<p>{AppResources.Help_Timer_Duration_Description}</p>");
        sb.AppendLine($"<h3>{AppResources.Help_Timer_StopAt_Title}</h3>");
        sb.AppendLine($"<p>{AppResources.Help_Timer_StopAt_Description}</p>");
        sb.AppendLine($"<h3>{AppResources.Help_Timer_Alarm_Title}</h3>");
        sb.AppendLine($"<p>{AppResources.Help_Timer_Alarm_Description}</p>");
        sb.AppendLine("</div>");

        // Advanced Features
        sb.AppendLine($"<h2>{AppResources.Help_Advanced_Title}</h2>");
        
        sb.AppendLine($"<h3>{AppResources.Help_Mixes_Title}</h3>");
        sb.AppendLine($"<p>{AppResources.Help_Mixes_Description}</p>");
        sb.AppendLine("<div class='tip'>");
        sb.AppendLine($"<strong>{AppResources.Help_Mixes_Tip}</strong>");
        sb.AppendLine("</div>");

        sb.AppendLine($"<h3>{AppResources.Help_EQ_Title}</h3>");
        sb.AppendLine($"<p>{AppResources.Help_EQ_Description}</p>");

        sb.AppendLine($"<h3>{AppResources.Help_Export_Title}</h3>");
        sb.AppendLine($"<p>{AppResources.Help_Export_Description}</p>");

        // Subscription Tiers
        sb.AppendLine($"<h2>{AppResources.Help_Tiers_Title}</h2>");
        
        sb.AppendLine("<div class='tier'>");
        sb.AppendLine($"<h3>{AppResources.Help_Tier_Free_Title}</h3>");
        sb.AppendLine("<ul>");
        foreach (var feature in AppResources.Help_Tier_Free_Features.Split('|'))
        {
            sb.AppendLine($"<li>{feature.Trim()}</li>");
        }
        sb.AppendLine("</ul>");
        sb.AppendLine("</div>");

        sb.AppendLine("<div class='tier'>");
        sb.AppendLine($"<h3>{AppResources.Help_Tier_Standard_Title}</h3>");
        sb.AppendLine("<ul>");
        foreach (var feature in AppResources.Help_Tier_Standard_Features.Split('|'))
        {
            sb.AppendLine($"<li>{feature.Trim()}</li>");
        }
        sb.AppendLine("</ul>");
        sb.AppendLine("</div>");

        sb.AppendLine("<div class='tier'>");
        sb.AppendLine($"<h3>{AppResources.Help_Tier_Premium_Title}</h3>");
        sb.AppendLine("<ul>");
        foreach (var feature in AppResources.Help_Tier_Premium_Features.Split('|'))
        {
            sb.AppendLine($"<li>{feature.Trim()}</li>");
        }
        sb.AppendLine("</ul>");
        sb.AppendLine("</div>");

        sb.AppendLine("<div class='tier'>");
        sb.AppendLine($"<h3>{AppResources.Help_Tier_ProPlus_Title}</h3>");
        sb.AppendLine("<ul>");
        foreach (var feature in AppResources.Help_Tier_ProPlus_Features.Split('|'))
        {
            sb.AppendLine($"<li>{feature.Trim()}</li>");
        }
        sb.AppendLine("</ul>");
        sb.AppendLine("</div>");

        // Tips & Tricks
        sb.AppendLine($"<h2>{AppResources.Help_Tips_Title}</h2>");
        sb.AppendLine("<div class='tip'>");
        sb.AppendLine("<ul>");
        sb.AppendLine($"<li><strong>{AppResources.Help_Tips_Simple}</strong></li>");
        sb.AppendLine($"<li><strong>{AppResources.Help_Tips_Volume}</strong></li>");
        sb.AppendLine($"<li><strong>{AppResources.Help_Tips_Timer}</strong></li>");
        sb.AppendLine($"<li><strong>{AppResources.Help_Tips_Save}</strong></li>");
        sb.AppendLine($"<li><strong>{AppResources.Help_Tips_Background}</strong></li>");
        sb.AppendLine("</ul>");
        sb.AppendLine("</div>");

        // Troubleshooting
        sb.AppendLine($"<h2>{AppResources.Help_Troubleshooting_Title}</h2>");
        sb.AppendLine("<div class='warning'>");
        sb.AppendLine("<ul>");
        sb.AppendLine($"<li><strong>{AppResources.Help_Troubleshooting_Audio}</strong></li>");
        sb.AppendLine($"<li><strong>{AppResources.Help_Troubleshooting_Timer}</strong></li>");
        sb.AppendLine($"<li><strong>{AppResources.Help_Troubleshooting_Data}</strong></li>");
        sb.AppendLine($"<li><strong>{AppResources.Help_Troubleshooting_Performance}</strong></li>");
        sb.AppendLine("</ul>");
        sb.AppendLine($"<p><strong>{AppResources.Help_Troubleshooting_More}</strong></p>");
        sb.AppendLine("</div>");

        // Footer
        sb.AppendLine("<div style='text-align: center; margin-top: 40px; padding: 20px; background: #ecf0f1; border-radius: 4px;'>");
        sb.AppendLine($"<p><strong>{AppResources.Help_Footer_AppVersion}:</strong> {AppInfo.VersionString}</p>");
        sb.AppendLine($"<p><strong>{AppResources.Help_Footer_LastUpdated}:</strong> {DateTime.Now:MMMM yyyy}</p>");
        sb.AppendLine("</div>");

        sb.AppendLine("</div>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");

        return sb.ToString();
    }
}
