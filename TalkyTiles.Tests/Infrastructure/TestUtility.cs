using TalkyTiles.Core.Models;
using TalkyTiles.Core.Services.Interfaces;
using Moq;
using Moq.AutoMock;
using TalkyTiles.Core.ViewModels;

namespace TalkyTiles.Tests.Infrastructure;

public static class TestUtility
{

    /// <summary>
    /// Configure the storage service to return exactly the provided pages.
    /// </summary>
    public static void SetupStorageToReturnPages (AutoMocker mocker, params SoundPage[] pages)
    {
        mocker.GetMock<ITileStorageService>()
              .Setup(storageService => storageService.LoadAllPagesAsync())
              .ReturnsAsync(pages.ToList());
    }

    /// <summary>
    /// Create a SoundPage with the given name and button texts.
    /// </summary>
    public static SoundPage CreatePage (string          name
                                , params string[] buttonTexts)
    {
        var page = new SoundPage { Name = name };
        foreach (var text in buttonTexts)
        {
            page.Buttons.Add(new SoundButton { Text = text });
        }

        return page;
    }

    public static  Mock<IUiStateService> GetUiMock(AutoMocker mocker)
    {
        bool flag   = false;
        var  uiMock = mocker.GetMock<IUiStateService>();
        uiMock.Setup(x => x.IsEditMode).Returns(() => flag);
        uiMock.Setup(x => x.ToggleEditMode())
              .Callback(() => flag = ! flag);

        return uiMock;
    }

    public static SoundButtonViewModel CreateSut (SoundButton                   model
                                                , out Mock<IAudioService>       audioMock
                                                , out Mock<ITileStorageService> storageMock
                                                , out Mock<IUiStateService>     uiStateMock)
    {
        audioMock   = new Mock<IAudioService>();
        storageMock = new Mock<ITileStorageService>();
        uiStateMock = new Mock<IUiStateService>();

        return new SoundButtonViewModel(model
                                      , audioMock.Object
                                      , storageMock.Object
                                      , uiStateMock.Object
        );
    }

    public static TestableSoundButtonViewModel CreateTestableSut (SoundButton                   model
                                                                , out Mock<IAudioService>       audioMock
                                                                , out Mock<ITileStorageService> storageMock
                                                                , out Mock<IUiStateService>     uiMock)
    {
        audioMock   = new Mock<IAudioService>();
        storageMock = new Mock<ITileStorageService>();
        uiMock      = new Mock<IUiStateService>();

        return new TestableSoundButtonViewModel(model
                                              , audioMock.Object
                                              , storageMock.Object
                                              , uiMock.Object);
    }
}

public class TestableSoundButtonViewModel : SoundButtonViewModel
{
    public List<int> RecordedSeconds { get; } = new();

    public TestableSoundButtonViewModel (
            SoundButton         model
          , IAudioService       audio
          , ITileStorageService storage
          , IUiStateService     uiState)
            : base(model
                 , audio
                 , storage
                 , uiState)
    {
    }

    protected override Task Delay (int ms)
    {
        RecordedSeconds.Add(SecondsRemaining);

        return Task.CompletedTask;
    }
}
