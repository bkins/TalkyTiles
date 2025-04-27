// Tests/ViewModels/MainPageViewModelTests.cs

using Moq;
using TalkyTiles.Core.Models;
using TalkyTiles.Core.Services.Interfaces;
using TalkyTiles.Core.ViewModels;
using TalkyTiles.Tests.Infrastructure;

namespace TalkyTiles.Tests.ViewModels
{
    public class MainPageViewModelTests : ViewModelTestBase<MainPageViewModel>
    {
        private const double Tolerance = .01;

        [Fact]
        public void ToggleEditMode_Should_Call_UiStateService()
        {
            // Act
            Sut.ToggleEditMode();

            // Assert
            Mocker.GetMock<IUiStateService>()
                  .Verify(stateService => stateService.ToggleEditMode(), Times.Once);
        }

        [Fact]
        public async Task InitializeAsync_Should_Load_Buttons_From_Storage()
        {
            // Arrange
            var page = TestUtility.CreatePage("StoredPage", "A", "B");
            TestUtility.SetupStorageToReturnPages(Mocker, page);

            // Act
            await Sut.InitializeAsync();

            // Assert
            // Assert.Same(page, Sut.CurrentPage);
            Assert.Equal(2, Sut.Buttons.Count);
            Assert.Equal("A", Sut.Buttons[0].DisplayText);
            Assert.Equal("B", Sut.Buttons[1].DisplayText);
        }

        [Fact]
        public async Task InitializeAsync_Should_SeedData_When_NoPagesExist()
        {
            // Arrange
            TestUtility.SetupStorageToReturnPages(Mocker); // empty

            // Act
            await Sut.InitializeAsync();

            // Assert
            // Storage.SavePageAsync should have been called once with the seeded page
            Mocker.GetMock<ITileStorageService>()
                  .Verify(storageService => storageService.SavePageAsync
                                  (It.Is<SoundPage>(page => page.Name == "Page 1"))
                        , Times.Once);

            // Assert.NotNull(Sut.CurrentPage);
            Assert.NotEmpty(Sut.Buttons);
        }

        [Fact]
        public async Task AddNewTileAsync_Should_Add_Button_And_Save()
        {
            // Arrange
            var page = TestUtility.CreatePage("TestPage");
            TestUtility.SetupStorageToReturnPages(Mocker, page);
            await Sut.InitializeAsync();

            // Act
            await Sut.Canvas.AddNewTileAsync();

            // Assert
            Assert.Single(page.Buttons);
            Assert.Single(Sut.Buttons);
            Assert.Equal("New", page.Buttons[0].Text);

            Mocker.GetMock<ITileStorageService>()
                  .Verify(x => x.SavePageAsync(It.Is<SoundPage>(soundPage => soundPage.Buttons.Count == 1
                                                                          && soundPage.Buttons[0].Text == "New")),
                  Times.Once);
        }

        [Fact]
        public async Task SavePageAsync_Should_Sync_Buttons_And_Call_Save()
        {
            // Arrange
            var page = TestUtility.CreatePage("SaveTest", "Existing");
            TestUtility.SetupStorageToReturnPages(Mocker, page);

            await Sut.InitializeAsync();

            // simulate user moving/adding tiles
            Sut.Buttons.Clear();
            Sut.Buttons.Add(new SoundButtonViewModel(new SoundButton
                                                     {
                                                             Text = "Moved"
                                                           , X    = 5
                                                           , Y    = 7
                                                     }
                                                   , Mocker.GetMock<IAudioService>().Object
                                                   , Mocker.GetMock<ITileStorageService>().Object
                                                   , Mocker.GetMock<IUiStateService>().Object
                            ));

            // Act
            await Sut.Canvas.SaveCanvasAsync();

            // Assert
            Mocker.GetMock<ITileStorageService>()
                  .Verify(storageService => storageService.SavePageAsync
                          (It.Is<SoundPage>(soundPage => soundPage.Buttons.Count == 1
                                                      && soundPage.Buttons[0].Text == "Moved"
                                                      && Math.Abs(soundPage.Buttons[0].X - 5) < Tolerance
                                                      && Math.Abs(soundPage.Buttons[0].Y - 7) < Tolerance))
                         , Times.Once);
        }


        [Fact]
        public void Setting_IsEditMode_ToDifferentValue_CallsToggleOnce()
        {
            // Arrange
            var uiMock = TestUtility.GetUiMock(Mocker);

            // Act
            Sut.IsEditMode = true; // false → true

            // Assert
            uiMock.Verify(x => x.ToggleEditMode()
                        , Times.Once);
            Assert.True(Sut.IsEditMode);
        }

        [Fact]
        public void Setting_IsEditMode_ToSameValue_DoesNotCallToggle()
        {
            // Arrange
            var uiMock = TestUtility.GetUiMock(Mocker);

            // Act
            Sut.IsEditMode = false; // writing same value
            Sut.IsEditMode = false; // again

            // Assert
            uiMock.Verify(x => x.ToggleEditMode()
                        , Times.Never);
            Assert.False(Sut.IsEditMode);
        }
    }
}
