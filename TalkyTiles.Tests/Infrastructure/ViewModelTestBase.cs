using Moq;
using Moq.AutoMock;
using TalkyTiles.Core.Services.Interfaces;
using TalkyTiles.Core.ViewModels;

namespace TalkyTiles.Tests.Infrastructure;

public abstract class ViewModelTestBase<T> where T : class
{
    protected AutoMocker Mocker    { get; }
    protected T          Sut { get; }

    protected ViewModelTestBase()
    {
        Mocker = new AutoMocker();

        // 1) Ensure the IAudio/Storage/UI mocks exist...
        var audioMock   = Mocker.GetMock<IAudioService>().Object;
        var storageMock = Mocker.GetMock<ITileStorageService>().Object;
        var uiMock      = Mocker.GetMock<IUiStateService>().Object;

        // 2) Create a Moq proxy of the concrete TileCanvasViewModel,
        //    supplying the three constructor dependencies:
        var canvasProxy = new Mock<TileCanvasViewModel>(
                MockBehavior.Loose
              , audioMock
              , storageMock
              , uiMock
        );

        // 3) Tell AutoMocker “when MainPageViewModel needs a TileCanvasViewModel,
        //    hand it this proxy instance.”
        Mocker.Use<TileCanvasViewModel>(canvasProxy.Object);

        Sut = Mocker.CreateInstance<T>();
    }
}
