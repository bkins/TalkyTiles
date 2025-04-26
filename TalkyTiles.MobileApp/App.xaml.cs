using TalkyTiles.MobileApp.Views;

namespace TalkyTiles.MobileApp
{
    public partial class App : Application
    {
        public IServiceProvider Services { get; }

        public App (IServiceProvider serviceProvider)
        {
            InitializeComponent();
            Services = serviceProvider;

            //MainPage = new AppShell();
            //MainPage = new MainPage();

            MainPage = new NavigationPage(new MainPage());
        }
    }
}
