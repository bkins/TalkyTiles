namespace TalkyTiles.Services;

public class AppServices
{
    public static T Get<T>() => ((App)App.Current).Services.GetRequiredService<T>();
}
