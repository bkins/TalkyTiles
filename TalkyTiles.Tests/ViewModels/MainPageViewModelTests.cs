using Moq;
using TalkyTiles.Core.Models;
using TalkyTiles.Core.ViewModels;
using TalkyTiles.Core.Services.Interfaces;
using Xunit;

namespace TalkyTiles.Tests.ViewModels;

public class MainPageViewModelTests
{
    [Fact]
    public void ToggleEditMode_ShouldFlipIsEditMode()
    {
        // Arrange
        var mockAudioService   = new Mock<IAudioService>();
        var mockStorageService = new Mock<ITileStorageService>();
        var mockUiStateService = new Mock<IUiStateService>();

        bool isEditMode = false;
        mockUiStateService.Setup(stateService => stateService.IsEditMode)
                          .Returns(() => isEditMode);

        mockUiStateService.Setup(stateService => stateService.ToggleEditMode())
                          .Callback(() => isEditMode = ! isEditMode);

        var vm = new MainPageViewModel(mockAudioService.Object
                                     , mockStorageService.Object
                                     , mockUiStateService.Object);

        // Act
        bool initial = vm.IsEditMode;

        vm.ToggleEditModeCommand.Execute(null);

        // Assert
        Assert.NotEqual(initial
                      , vm.IsEditMode);
    }

    [Fact]
    public async Task AddNewTileAsync_ShouldAddButtonAndSave()
    {
        // Arrange
        var mockAudioService   = new Mock<IAudioService>();
        var mockStorageService = new Mock<ITileStorageService>();
        var mockUiStateService = new Mock<IUiStateService>();

        mockUiStateService.Setup(stateService => stateService.IsEditMode)
                          .Returns(false);

        var vm = new MainPageViewModel(mockAudioService.Object
                                     , mockStorageService.Object
                                     , mockUiStateService.Object);

        var page = new SoundPage
                   {
                           Name = "Test Page"
                         , Buttons = new List<SoundButton>()
                   };
        vm.GetType()
          .GetProperty("CurrentPage")!
          .SetValue(vm
                  , page); // Dirty reflection to inject CurrentPage because it's private set.

        int initialButtonCount = vm.Buttons.Count;

        // Act
        await vm.AddNewTileAsync();

        // Assert
        Assert.Equal(initialButtonCount + 1
                   , vm.Buttons.Count);

        Assert.Single(page.Buttons); // page should have 1 SoundButton now

        mockStorageService.Verify(storageService => storageService.SavePageAsync(It.Is<SoundPage>(p => p.Buttons.Count == 1))
                                , Times.Once);
    }
}
