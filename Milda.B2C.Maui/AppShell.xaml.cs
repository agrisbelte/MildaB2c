using Milda.B2C.Maui.Pages;

namespace Milda.B2C.Maui
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(ScopePage), typeof(ScopePage));
        }
    }
}
