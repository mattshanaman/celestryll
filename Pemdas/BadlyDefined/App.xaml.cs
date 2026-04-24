using BadlyDefined.Services;

namespace BadlyDefined;

public partial class App : Application
{
    private readonly DatabaseService _databaseService;

    public App(DatabaseService databaseService)
    {
        System.Diagnostics.Debug.WriteLine("🔧 App constructor started");

        // Catch unhandled exceptions
        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            var exception = e.ExceptionObject as Exception;
            System.Diagnostics.Debug.WriteLine($"💥 UNHANDLED EXCEPTION: {exception?.GetType().Name}");
            System.Diagnostics.Debug.WriteLine($"💥 Message: {exception?.Message}");
            System.Diagnostics.Debug.WriteLine($"💥 Stack: {exception?.StackTrace}");
        };

        TaskScheduler.UnobservedTaskException += (s, e) =>
        {
            System.Diagnostics.Debug.WriteLine($"💥 UNOBSERVED TASK EXCEPTION: {e.Exception.GetType().Name}");
            System.Diagnostics.Debug.WriteLine($"💥 Message: {e.Exception.Message}");
            e.SetObserved();
        };

        try
        {
            System.Diagnostics.Debug.WriteLine("🔧 Calling InitializeComponent...");
            InitializeComponent();
            System.Diagnostics.Debug.WriteLine("✅ InitializeComponent completed");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"💥 INIT COMPONENT FAILED: {ex.GetType().Name}");
            System.Diagnostics.Debug.WriteLine($"💥 Message: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"💥 Stack: {ex.StackTrace}");
            throw;
        }

        _databaseService = databaseService;
        System.Diagnostics.Debug.WriteLine("✅ App constructor completed");
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        System.Diagnostics.Debug.WriteLine("🔧 CreateWindow called");
        try
        {
            var window = new Window(new AppShell());
            System.Diagnostics.Debug.WriteLine("✅ CreateWindow completed");
            return window;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"💥 CREATE WINDOW FAILED: {ex.GetType().Name}");
            System.Diagnostics.Debug.WriteLine($"💥 Message: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"💥 Stack: {ex.StackTrace}");
            throw;
        }
    }

    protected override void OnStart()
    {
        base.OnStart();

        // Initialize database on app start
        System.Diagnostics.Debug.WriteLine("🔧 App.OnStart called");

        // TEMPORARILY: Don't await - let it initialize in background
        Task.Run(async () =>
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("🔧 Starting database initialization...");
                await _databaseService.InitializeAsync();
                System.Diagnostics.Debug.WriteLine("✅ Database initialized successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Database initialization failed: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"❌ Message: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"❌ Stack: {ex.StackTrace}");
            }
        });

        System.Diagnostics.Debug.WriteLine("✅ App.OnStart completed");
    }
}
