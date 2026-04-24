namespace BadlyDefined;

public partial class AppShell : Shell
{
    public AppShell()
    {
        System.Diagnostics.Debug.WriteLine("🔧 AppShell constructor started");

        try
        {
            System.Diagnostics.Debug.WriteLine("🔧 Calling InitializeComponent...");
            InitializeComponent();
            System.Diagnostics.Debug.WriteLine("✅ AppShell InitializeComponent completed");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"💥 APPSHELL INIT FAILED: {ex.GetType().Name}");
            System.Diagnostics.Debug.WriteLine($"💥 Message: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"💥 Stack: {ex.StackTrace}");
            System.Diagnostics.Debug.WriteLine($"💥 Inner: {ex.InnerException?.Message}");
            throw;
        }

        // Register routes for navigation
        System.Diagnostics.Debug.WriteLine("🔧 Registering routes...");
        Routing.RegisterRoute(nameof(Pages.GamePage), typeof(Pages.GamePage));
        Routing.RegisterRoute(nameof(Pages.ProfilePage), typeof(Pages.ProfilePage));
        Routing.RegisterRoute(nameof(Pages.TestModePage), typeof(Pages.TestModePage));
        System.Diagnostics.Debug.WriteLine("✅ AppShell constructor completed");
    }
}
