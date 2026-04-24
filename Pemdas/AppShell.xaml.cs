namespace Pemdas
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("archive", typeof(Pages.PastGamesPage));
        }
    }
}
