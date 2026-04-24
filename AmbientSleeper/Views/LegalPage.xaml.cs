using System.Text;
using AmbientSleeper.Resources.Strings;

namespace AmbientSleeper.Views;

public partial class LegalPage : ContentPage
{
    public LegalPage()
    {
        InitializeComponent();
      LoadLegalContent();
    }

    private async void LoadLegalContent()
  {
        try
        {
            var html = GenerateLegalHtml();
      LegalWebView.Source = new HtmlWebViewSource { Html = html };
            
            // Hide loading indicator after a short delay
            await Task.Delay(500);
     LoadingIndicator.IsVisible = false;
        }
  catch (Exception ex)
  {
      System.Diagnostics.Debug.WriteLine($"Failed to load legal content: {ex.Message}");
         LoadingIndicator.IsVisible = false;
      
   // Show error message in WebView
       var errorHtml = $@"
            <html>
    <head>
         <meta name='viewport' content='width=device-width, initial-scale=1.0'>
               <style>body {{ font-family: sans-serif; padding: 20px; }}</style>
            </head>
    <body>
     <h2>Error Loading Content</h2>
   <p>Unable to load legal information. Please try again later.</p>
          <p style='color: #666; font-size: 0.9em;'>{ex.Message}</p>
     </body>
     </html>";
 LegalWebView.Source = new HtmlWebViewSource { Html = errorHtml };
        }
    }

    private string GenerateLegalHtml()
    {
        var sb = new StringBuilder();
        var currentYear = DateTime.Now.Year;
        
        // HTML Header with styling (same as before)
        sb.AppendLine(@"<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Legal &amp; Disclaimer</title>
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
            border-bottom: 3px solid #e74c3c; 
          padding-bottom: 10px;
    margin-top: 0;
        }
     h2 { 
            color: #c0392b; 
        margin-top: 30px;
  border-bottom: 2px solid #ecf0f1;
     padding-bottom: 5px;
   }
        h3 { color: #7f8c8d; margin-top: 20px; }
.warning-box {
    background: #fff3cd;
 border-left: 4px solid #ffc107;
 padding: 15px;
            margin: 20px 0;
            border-radius: 4px;
        }
   .critical-box {
            background: #f8d7da;
            border-left: 4px solid #dc3545;
            padding: 15px;
   margin: 20px 0;
            border-radius: 4px;
     font-weight: bold;
        }
        .info-box {
        background: #d1ecf1;
            border-left: 4px solid #17a2b8;
 padding: 15px;
margin: 20px 0;
border-radius: 4px;
    }
    ul { padding-left: 20px; }
        li { margin: 8px 0; }
        .footer {
          text-align: center;
            margin-top: 40px;
        padding: 20px;
       background: #ecf0f1;
    border-radius: 4px;
            font-size: 0.9em;
            color: #7f8c8d;
        }
        @media (prefers-color-scheme: dark) {
            body { background: #1a1a1a; color: #e0e0e0; }
            .container { background: #2d2d2d; }
          h1 { color: #e74c3c; }
            h2 { color: #ec7063; }
      h3 { color: #bdc3c7; }
   .warning-box { background: #5d4e37; }
      .critical-box { background: #4a1a1a; }
            .info-box { background: #1a3a4a; }
          .footer { background: #34495e; color: #bdc3c7; }
        }
    </style>
</head>
<body>
<div class='container'>");

        // Main Title - LOCALIZED
  sb.AppendLine($"<h1>{AppResources.Legal_PageTitle}</h1>");

  // Critical Medical Disclaimer - LOCALIZED
        sb.AppendLine("<div class='critical-box'>");
        sb.AppendLine($"<h2 style='margin-top: 0; color: #dc3545;'>{AppResources.Legal_Critical_Title}</h2>");
        sb.AppendLine($"<p><strong>{AppResources.Legal_Critical_Statement}</strong></p>");
      sb.AppendLine($"<p>{AppResources.Legal_Critical_NotMedical}</p>");
        sb.AppendLine("</div>");

      // Entertainment Purpose - LOCALIZED
        sb.AppendLine($"<h2>{AppResources.Legal_Entertainment_Title}</h2>");
        sb.AppendLine("<div class='warning-box'>");
        sb.AppendLine($"<p>{AppResources.Legal_Entertainment_ForText}</p>");
        sb.AppendLine("<ul>");
        foreach (var item in AppResources.Legal_Entertainment_List.Split('|'))
        {
            sb.AppendLine($"<li>{item.Trim()}</li>");
    }
sb.AppendLine("</ul>");
        sb.AppendLine($"<p><strong>{AppResources.Legal_Entertainment_DoesNotText}</strong></p>");
        sb.AppendLine("<ul>");
        foreach (var item in AppResources.Legal_Entertainment_DoesNotList.Split('|'))
        {
            sb.AppendLine($"<li>{item.Trim()}</li>");
        }
        sb.AppendLine("</ul>");
        sb.AppendLine("</div>");

        // No Medical Advice - LOCALIZED
        sb.AppendLine($"<h2>{AppResources.Legal_NoAdvice_Title}</h2>");
   sb.AppendLine($"<p>{AppResources.Legal_NoAdvice_Description}</p>");
        sb.AppendLine("<div class='info-box'>");
        sb.AppendLine($"<p><strong>{AppResources.Legal_NoAdvice_SleepProblems}</strong></p>");
        sb.AppendLine("</div>");

    // Health Conditions - LOCALIZED
        sb.AppendLine($"<h2>{AppResources.Legal_Health_Title}</h2>");
        sb.AppendLine("<div class='warning-box'>");
    sb.AppendLine($"<p>{AppResources.Legal_Health_Consult}</p>");
        sb.AppendLine("<ul>");
        foreach (var condition in AppResources.Legal_Health_ConditionsList.Split('|'))
        {
   sb.AppendLine($"<li>{condition.Trim()}</li>");
        }
 sb.AppendLine("</ul>");
        sb.AppendLine($"<p><strong>{AppResources.Legal_Health_NoDelay}</strong></p>");
        sb.AppendLine("</div>");

        // Limitation of Liability - LOCALIZED
  sb.AppendLine($"<h2>{AppResources.Legal_Liability_Title}</h2>");
        
    sb.AppendLine($"<h3>{AppResources.Legal_Liability_Risk_Title}</h3>");
        sb.AppendLine($"<p>{AppResources.Legal_Liability_Risk_Text}</p>");
        sb.AppendLine("<ul>");
        foreach (var item in AppResources.Legal_Liability_Risk_List.Split('|'))
        {
            sb.AppendLine($"<li>{item.Trim()}</li>");
        }
 sb.AppendLine("</ul>");

        sb.AppendLine($"<h3>{AppResources.Legal_Liability_NoLiability_Title}</h3>");
        sb.AppendLine($"<p>{AppResources.Legal_Liability_NoLiability_Text}</p>");
     sb.AppendLine("<ul>");
        foreach (var item in AppResources.Legal_Liability_NoLiability_List.Split('|'))
        {
       sb.AppendLine($"<li>{item.Trim()}</li>");
 }
        sb.AppendLine("</ul>");

    sb.AppendLine($"<h3>{AppResources.Legal_Liability_Indemnification_Title}</h3>");
        sb.AppendLine($"<p><strong>{AppResources.Legal_Liability_Indemnification_Text}</strong></p>");
        sb.AppendLine("<ul>");
        foreach (var item in AppResources.Legal_Liability_Indemnification_List.Split('|'))
        {
        sb.AppendLine($"<li>{item.Trim()}</li>");
        }
        sb.AppendLine("</ul>");

        // Volume Warning - LOCALIZED
sb.AppendLine($"<h2>{AppResources.Legal_Volume_Title}</h2>");
        sb.AppendLine("<div class='critical-box'>");
        sb.AppendLine($"<p><strong>{AppResources.Legal_Volume_Warning}</strong></p>");
        sb.AppendLine($"<p>{AppResources.Legal_Volume_Text}</p>");
        sb.AppendLine("<ul>");
        foreach (var guideline in AppResources.Legal_Volume_Guidelines.Split('|'))
        {
            sb.AppendLine($"<li>{guideline.Trim()}</li>");
        }
    sb.AppendLine("</ul>");
        sb.AppendLine("</div>");

        // Emergency - LOCALIZED
        sb.AppendLine($"<h2>{AppResources.Legal_Emergency_Title}</h2>");
   sb.AppendLine($"<p>{AppResources.Legal_Emergency_Text}</p>");

        // Timer - LOCALIZED
   sb.AppendLine($"<h2>{AppResources.Legal_Timer_Title}</h2>");
        sb.AppendLine("<div class='warning-box'>");
        sb.AppendLine($"<p>{AppResources.Legal_Timer_Text}</p>");
        sb.AppendLine("<ul>");
        foreach (var item in AppResources.Legal_Timer_List.Split('|'))
        {
    sb.AppendLine($"<li>{item.Trim()}</li>");
        }
  sb.AppendLine("</ul>");
        sb.AppendLine("</div>");

   // Children - LOCALIZED
        sb.AppendLine($"<h2>{AppResources.Legal_Children_Title}</h2>");
    sb.AppendLine($"<p>{AppResources.Legal_Children_Text}</p>");
        sb.AppendLine("<ul>");
   foreach (var item in AppResources.Legal_Children_List.Split('|'))
        {
 sb.AppendLine($"<li>{item.Trim()}</li>");
        }
   sb.AppendLine("</ul>");

   // Data & Privacy - LOCALIZED
        sb.AppendLine($"<h2>{AppResources.Legal_Data_Title}</h2>");
    sb.AppendLine($"<p>{AppResources.Legal_Data_Text}</p>");
        sb.AppendLine("<ul>");
foreach (var item in AppResources.Legal_Data_List.Split('|'))
        {
          sb.AppendLine($"<li>{item.Trim()}</li>");
        }
    sb.AppendLine("</ul>");
      sb.AppendLine($"<p>{AppResources.Legal_Data_Export}</p>");

 // Third-Party - LOCALIZED
sb.AppendLine($"<h2>{AppResources.Legal_ThirdParty_Title}</h2>");
  sb.AppendLine($"<p>{AppResources.Legal_ThirdParty_Text}</p>");
        sb.AppendLine("<ul>");
     foreach (var item in AppResources.Legal_ThirdParty_List.Split('|'))
        {
            sb.AppendLine($"<li>{item.Trim()}</li>");
 }
        sb.AppendLine("</ul>");

        // Changes - LOCALIZED
        sb.AppendLine($"<h2>{AppResources.Legal_Changes_Title}</h2>");
        sb.AppendLine($"<p>{AppResources.Legal_Changes_Text}</p>");
        sb.AppendLine("<ul>");
        foreach (var item in AppResources.Legal_Changes_List.Split('|'))
  {
sb.AppendLine($"<li>{item.Trim()}</li>");
    }
      sb.AppendLine("</ul>");
        sb.AppendLine($"<p>{AppResources.Legal_Changes_NoLiability}</p>");

 // Governing Law - LOCALIZED
    sb.AppendLine($"<h2>{AppResources.Legal_Law_Title}</h2>");
   sb.AppendLine($"<p>{AppResources.Legal_Law_Text}</p>");

        // Contact - LOCALIZED
  sb.AppendLine($"<h2>{AppResources.Legal_Contact_Title}</h2>");
    sb.AppendLine($"<p>{AppResources.Legal_Contact_Text}</p>");

        // Acceptance - LOCALIZED
        sb.AppendLine($"<h2>{AppResources.Legal_Acceptance_Title}</h2>");
        sb.AppendLine("<div class='critical-box'>");
     sb.AppendLine($"<p><strong>{AppResources.Legal_Acceptance_Agree}</strong></p>");
        sb.AppendLine($"<p><strong>{AppResources.Legal_Acceptance_Disagree}</strong></p>");
        sb.AppendLine("</div>");

     // Footer - LOCALIZED
 sb.AppendLine("<div class='footer'>");
        sb.AppendLine("<p><strong>Ambient Sleeper</strong></p>");
        sb.AppendLine($"<p><strong>{AppResources.Help_Footer_AppVersion}:</strong> {AppInfo.VersionString}</p>");
        sb.AppendLine($"<p><strong>{AppResources.Help_Footer_LastUpdated}:</strong> {DateTime.Now:MMMM yyyy}</p>");
        sb.AppendLine($"<p>&copy; {currentYear} - {AppResources.Legal_Footer_Copyright}</p>");
      sb.AppendLine($"<p style='margin-top: 15px; font-size: 0.85em;'>{AppResources.Legal_Footer_Disclaimer}</p>");
        sb.AppendLine("</div>");

        sb.AppendLine("</div>");
        sb.AppendLine("</body>");
sb.AppendLine("</html>");

        return sb.ToString();
    }
}
