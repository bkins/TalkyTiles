using TalkyTiles.Views;

namespace TalkyTiles
{
    public partial class App : Application
    {
        public IServiceProvider Services { get; }

        public App (IServiceProvider serviceProvider)
        {
            InitializeComponent();
            Services = serviceProvider;

            //MainPage = new AppShell();
            MainPage = new MainPage();
        }
    }
}
